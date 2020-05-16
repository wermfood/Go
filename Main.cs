using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wermwerx.Capture;

namespace DisembodiedHeads
{
    public partial class Main : Form
    {
        private bool m_IsSnipped = false;
        private bool m_IsFramelessMode = false;
        private bool m_IsRubberBandBoxBeingDragged = false;
        private bool m_IsFormBeingDragged = false;
        private Point m_LastLocation = Point.Empty;
        private Point m_MouseDownPoint = Point.Empty;
        private Size m_PreviousSize = Size.Empty;
        private FormWindowState m_PreviousWindowState = FormWindowState.Normal;
        private float m_ImageScale = 1.0f;
        private float m_PreviousImageScale = 1.0f;
        public Main()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);

            this.TopMost = true;

            var screenWindows = ScreenWindow.AvailableWindows;
            screenWindows.Insert(0, new ScreenWindow(IntPtr.Zero, "<Select Window>"));
            comboWindows.DataSource = screenWindows;
            comboWindows.DisplayMember = "Name";

            this.MinimumSize = new Size(10, 10);

            pictureBox.MouseWheel += PictureBox_MouseWheel;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (m_IsSnipped)
            {
                m_PreviousImageScale = m_ImageScale;
                const float scale_per_delta = 0.1f / 120;
                // Update the drawing based upon the mouse wheel scrolling.
                m_ImageScale += e.Delta * scale_per_delta;
                if (m_ImageScale < 0) m_ImageScale = 0;
            }
        }

        private void comboWindows_SelectedValueChanged(object sender, EventArgs e)
        {
            var screenWindow = ((ComboBox)sender).SelectedItem as ScreenWindow;
            Render(screenWindow);
            timer.Enabled = true;
        }

        private void Render(ScreenWindow screenWindow)
        {
            if (screenWindow != null && screenWindow.Handle != IntPtr.Zero)
            {
                Image img = ScreenCapture.Capture3(screenWindow);
                //IntPtr windowDC = User32.GetWindowDC(screenWindow.Handle);
                //User32.RECT rect = new User32.RECT();
                //User32.GetWindowRect(screenWindow.Handle, ref rect);


                //Bitmap img = new MyScreen().CaptureWindow(screenWindow.Handle);
                if (img != null)
                {
                    //img.Save("C:\\Hold\\Image.bmp");

                    float currentWidth = img.Width;
                    float currentHeight = img.Height;
                    float scaledWidth = currentWidth * m_ImageScale;
                    float scaledHeight = currentHeight * m_ImageScale;

                    Bitmap bmp = new Bitmap(img, new Size((int)scaledWidth, (int)scaledHeight));
                    img.Dispose();
                    this.pictureBox.BackgroundImage = bmp;

                    

                    if (m_ImageScale != m_PreviousImageScale)
                    {
                        AutoSizeWindow(new Rectangle(0, 0, (int)scaledWidth, (int)scaledHeight));
                    }

                }
            }
            if (m_IsRubberBandBoxBeingDragged)
            {
                DrawRubberBandBox(pictureBox, m_LastLocation);
            }
            

            GC.Collect();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Render(this.comboWindows.SelectedItem as ScreenWindow);
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button== MouseButtons.Left)
            {
                if (!m_IsFramelessMode && !m_IsSnipped)
                {
                    m_IsRubberBandBoxBeingDragged = true;
                    DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
                    //~m_MouseDownPoint = box.PointToClient(e.Location);
                    m_MouseDownPoint = e.Location;
                }else
                {
                    m_IsFormBeingDragged = true;
                    m_LastLocation = e.Location;
                }
            }
            #region OLD-Microsoft
            //// Set the isDrag variable to true and get the starting point 
            //// by using the PointToScreen method to convert form 
            //// coordinates to screen coordinates.
            //if (e.Button == MouseButtons.Left)
            //{
            //    m_IsDrag = true;
            //    Control control = (Control)sender;

            //    // Calculate the startPoint by using the PointToScreen 
            //    // method.
            //    m_StartPoint = control.PointToScreen(new Point(e.X, e.Y));
            //}
            #endregion


        }

        static public Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        public void DrawRubberBandBox(object sender,Point location)
        {
            DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
            box.Refresh();
            using (Graphics g = box.CreateGraphics())
            {
                Rectangle rect = GetRectangle(m_MouseDownPoint, location);
                g.DrawRectangle(Pens.Red, rect);
                m_LastLocation = location;
            }

            //ControlPaint.DrawReversibleFrame(m_TheRectangle,
            //    this.BackColor, FrameStyle.Dashed);

            // Calculate the endpoint and dimensions for the new 
            // rectangle, again using the PointToScreen method.
            //Point endPoint = ((Control)sender).PointToScreen(new Point(x,y));

            //int width = endPoint.X - m_StartPoint.X;
            //int height = endPoint.Y - m_StartPoint.Y;
            //m_TheRectangle = new Rectangle(m_StartPoint.X,
            //    m_StartPoint.Y, width, height);

            // Draw the new rectangle by calling DrawReversibleFrame
            // again.  
            //ControlPaint.DrawReversibleFrame(m_TheRectangle,
            //    this.BackColor, FrameStyle.Dashed);

        }
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!m_IsFramelessMode)
                {
                    if (m_IsRubberBandBoxBeingDragged)
                    {
                        DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
                        DrawRubberBandBox(sender, e.Location);
                        //~DrawRubberBandBox(sender, box.PointToClient(e.Location));
                    }
                }else
                {
                    if (m_IsFormBeingDragged)
                    {
                        int xLocation = this.Location.X - m_LastLocation.X + e.X;
                        int yLocation = this.Location.Y - m_LastLocation.Y + e.Y;

                        this.Location = new Point(xLocation, yLocation);
                        this.Update();
                    }
                }
            }


            #region OLD-MS
            // If the mouse is being dragged, 
            // undraw and redraw the rectangle as the mouse moves.
            //if (m_IsDrag)
            //{
            //    DrawRubberBandBox(sender,e.X, e.Y);
            //}

            //// Hide the previous rectangle by calling the 
            //// DrawReversibleFrame method with the same parameters.

            //{
            //    ControlPaint.DrawReversibleFrame(m_TheRectangle,
            //        this.BackColor, FrameStyle.Dashed);

            //    // Calculate the endpoint and dimensions for the new 
            //    // rectangle, again using the PointToScreen method.
            //    Point endPoint = ((Control)sender).PointToScreen(new Point(e.X, e.Y));

            //    int width = endPoint.X - m_StartPoint.X;
            //    int height = endPoint.Y - m_StartPoint.Y;
            //    m_TheRectangle = new Rectangle(m_StartPoint.X,
            //        m_StartPoint.Y, width, height);

            //    // Draw the new rectangle by calling DrawReversibleFrame
            //    // again.  
            //    ControlPaint.DrawReversibleFrame(m_TheRectangle,
            //        this.BackColor, FrameStyle.Dashed);
            //}
            #endregion
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button== MouseButtons.Left && !m_IsSnipped)
            {
                if (!m_IsFramelessMode)
                {
                    if (m_IsRubberBandBoxBeingDragged == true)
                    {
                        m_IsRubberBandBoxBeingDragged = false;
                        Rectangle rect = GetRectangle(m_MouseDownPoint, e.Location);
                        ((ScreenWindow)comboWindows.SelectedItem).SelectedRectangle = rect;
                        m_MouseDownPoint = Point.Empty;
                        m_LastLocation = Point.Empty;

                        //* Calculate the Border/Toolbar size
                        AutoSizeWindow(rect);
                        //int borderWidth = (this.Width - this.ClientSize.Width) / 2;
                        //int titleBarHeight = this.Height - this.ClientSize.Height - 2 * borderWidth;
                        //int comboHeight = comboWindows.Height + 15;
                        //int extraWidth = 8;
                        //if (rect.Width > this.MinimumSize.Width & rect.Height > this.MinimumSize.Height)
                        //{
                        //    m_PreviousSize = new Size(Size.Width, Size.Height);
                        //    m_PreviousWindowState = this.WindowState;
                        //    this.Size = new Size(rect.Width + borderWidth+extraWidth, rect.Height + titleBarHeight + comboHeight);
                        //    if (this.WindowState == FormWindowState.Maximized)
                        //    {
                        //        this.WindowState = FormWindowState.Normal;
                        //    }
                        //}

                        comboWindows.Visible = false;
                        this.MinimizeBox = false;
                        this.MaximizeBox = false;
                        //this.ControlBox = false;
                        m_IsSnipped = true;
                    }
                }else
                {
                    m_IsFormBeingDragged = false;
                    m_LastLocation = Point.Empty;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                var rect = this.ClientRectangle;
                Toggle();
                this.Location = new Point(this.Location.X + rect.X , this.Location.Y + rect.Y);
            }
            if (e.Button== MouseButtons.Middle)
            {
                if (!m_IsFramelessMode)
                {
                    ((ScreenWindow)comboWindows.SelectedItem).SelectedRectangle = Rectangle.Empty;
                    this.Size = m_PreviousSize;
                    this.WindowState = m_PreviousWindowState;
                    m_ImageScale = 1;
                    Toggle(true);
                }
            }



            //~Point start = pictureBox.PointToScreen(new Point(rect.X, rect.Y));



            //((ScreenWindow)comboWindows.SelectedItem).SelectedPoint = start;
            #region OLD-MS
            //// If the MouseUp event occurs, the user is not dragging.
            //if (e.Button == MouseButtons.Left)
            //{
            //    m_IsDrag = false;

            //    // Draw the rectangle to be evaluated. Set a dashed frame style 
            //    // using the FrameStyle enumeration.
            //    ControlPaint.DrawReversibleFrame(m_TheRectangle,
            //        this.BackColor, FrameStyle.Dashed);

            //    // Find out which controls intersect the rectangle and 
            //    // change their color. The method uses the RectangleToScreen  
            //    // method to convert the Control's client coordinates 
            //    // to screen coordinates.
            //    Rectangle controlRectangle;
            //    for (int i = 0; i < Controls.Count; i++)
            //    {
            //        controlRectangle = Controls[i].RectangleToScreen
            //            (Controls[i].ClientRectangle);
            //        if (controlRectangle.IntersectsWith(m_TheRectangle))
            //        {
            //            Controls[i].BackColor = Color.BurlyWood;
            //        }
            //    }


            //    ((ScreenWindow)comboWindows.SelectedItem).SelectedRectangle = new Rectangle(m_TheRectangle.X, m_TheRectangle.Y, m_TheRectangle.Width, m_TheRectangle.Height);



            //    // Reset the rectangle.
            //    m_TheRectangle = new Rectangle(0, 0, 0, 0);
            //}
            #endregion

        }

        private void AutoSizeWindow(Rectangle rect)
        {
            int borderWidth = (this.Width - this.ClientSize.Width) / 2;
            int titleBarHeight = this.Height - this.ClientSize.Height - 2 * borderWidth;
            int comboHeight = comboWindows.Height + 15;
            int extraWidth = 8;
            if (rect.Width > this.MinimumSize.Width & rect.Height > this.MinimumSize.Height)
            {
                m_PreviousSize = new Size(Size.Width, Size.Height);
                m_PreviousWindowState = this.WindowState;
                this.Size = new Size(rect.Width + borderWidth + extraWidth, rect.Height + titleBarHeight + comboHeight);
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void Toggle(bool forceDefaultReset=false)
        {
            if (!forceDefaultReset)
            {
                m_IsFramelessMode = !m_IsFramelessMode;
            }else
            {
                m_IsFramelessMode = false;
            }
            if (m_IsFramelessMode)
            {
                BackColor = Color.Lime;
                TransparencyKey = Color.Lime;
                FormBorderStyle = FormBorderStyle.None;
                //comboWindows.Visible = false;
            }
            else
            {
                BackColor = Color.Empty;
                TransparencyKey = Color.Empty;
                FormBorderStyle = FormBorderStyle.Sizable;
                //comboWindows.Visible = true;
            }
        }
    }
}
