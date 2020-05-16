namespace DisembodiedHeads
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Wermwerx.Capture;

    public class Main : Form
    {
        private Color m_ChromaColor = Color.LimeGreen;
        private bool m_IsPickingChromaColor = false;
        private bool m_IsSnipped = false;
        private bool m_IsFramelessMode = false;
        private bool m_IsRubberBandBoxBeingDragged = false;
        private bool m_IsFormBeingDragged = false;
        private Point m_LastLocation = Point.Empty;
        private Point m_MouseDownPoint = Point.Empty;
        private Size m_PreviousSize = Size.Empty;
        private FormWindowState m_PreviousWindowState = FormWindowState.Normal;
        private float m_ImageScale = 1f;
        private float m_PreviousImageScale = 1f;
        private IContainer components = null;
        private ComboBox comboWindows;
        private Timer timer;
        private DoubleBufferedPictureBox pictureBox;
        private DoubleBufferedPictureBox pictureBoxChromaColor;

        public Main()
        {
            this.InitializeComponent();
            this.InitializeForm();
        }

        private void AutoSizeWindow(Rectangle rect)
        {
            int num = (base.Width - base.ClientSize.Width) / 2;
            int num2 = (base.Height - base.ClientSize.Height) - (2 * num);
            int num3 = this.comboWindows.Height + 15;
            int num4 = 8;
            if ((rect.Width > this.MinimumSize.Width) & (rect.Height > this.MinimumSize.Height))
            {
                this.m_PreviousSize = new Size(base.Size.Width, base.Size.Height);
                this.m_PreviousWindowState = base.WindowState;
                base.Size = new Size((rect.Width + num) + num4, (rect.Height + num2) + num3);
                if (base.WindowState == FormWindowState.Maximized)
                {
                    base.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void comboWindows_SelectedValueChanged(object sender, EventArgs e)
        {
            ScreenWindow selectedItem = ((ComboBox) sender).SelectedItem as ScreenWindow;
            this.Render(selectedItem);
            if (this.pictureBox.Image != null)
            {
                this.m_ChromaColor = this.GetColorFromImage(this.pictureBox.Image, 0, 0);
                this.pictureBoxChromaColor.Image = this.CreateSwatch(0x15, 0x15, this.m_ChromaColor);
            }
            this.timer.Enabled = true;
        }

        private Image CreateSwatch(int width, int height, Color color)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(color);
            graphics.Dispose();
            return image;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void DrawRubberBandBox(object sender, Point location)
        {
            DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
            box.Refresh();
            using (Graphics graphics = box.CreateGraphics())
            {
                Rectangle rect = GetRectangle(this.m_MouseDownPoint, location);
                graphics.DrawRectangle(Pens.Red, rect);
                this.m_LastLocation = location;
            }
        }

        private Color GetColorFromImage(Image image, int x, int y)
        {
            Color chromaColor = this.m_ChromaColor;
            LockBitmap bitmap2 = new LockBitmap(new Bitmap(image));
            try
            {
                bitmap2.LockBits();
                chromaColor = bitmap2.GetPixel(x, y);
                bitmap2.UnlockBits();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
            return chromaColor;
        }

        public static Rectangle GetRectangle(Point p1, Point p2) => 
            new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs((int) (p1.X - p2.X)), Math.Abs((int) (p1.Y - p2.Y)));

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(Main));
            this.comboWindows = new ComboBox();
            this.timer = new Timer(this.components);
            this.pictureBoxChromaColor = new DoubleBufferedPictureBox(this.components);
            this.pictureBox = new DoubleBufferedPictureBox(this.components);
            ((ISupportInitialize) this.pictureBoxChromaColor).BeginInit();
            ((ISupportInitialize) this.pictureBox).BeginInit();
            base.SuspendLayout();
            this.comboWindows.FormattingEnabled = true;
            this.comboWindows.Location = new Point(0, 0);
            this.comboWindows.Name = "comboWindows";
            this.comboWindows.Size = new Size(0x2fe, 0x15);
            this.comboWindows.TabIndex = 1;
            this.comboWindows.SelectedValueChanged += new EventHandler(this.comboWindows_SelectedValueChanged);
            this.timer.Interval = 1;
            this.timer.Tick += new EventHandler(this.timer_Tick);
            this.pictureBoxChromaColor.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxChromaColor.Location = new Point(0x305, 0);
            this.pictureBoxChromaColor.Name = "pictureBoxChromaColor";
            this.pictureBoxChromaColor.Size = new Size(0x15, 0x15);
            this.pictureBoxChromaColor.TabIndex = 3;
            this.pictureBoxChromaColor.TabStop = false;
            this.pictureBoxChromaColor.MouseUp += new MouseEventHandler(this.pictureBoxChromaColor_MouseUp);
            this.pictureBox.BackgroundImageLayout = ImageLayout.None;
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.Location = new Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new Size(800, 450);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new MouseEventHandler(this.Main_MouseDown);
            this.pictureBox.MouseMove += new MouseEventHandler(this.Main_MouseMove);
            this.pictureBox.MouseUp += new MouseEventHandler(this.Main_MouseUp);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackgroundImageLayout = ImageLayout.None;
            base.ClientSize = new Size(800, 450);
            base.Controls.Add(this.pictureBoxChromaColor);
            base.Controls.Add(this.comboWindows);
            base.Controls.Add(this.pictureBox);
            //base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "Main";
            this.Text = "Disembodied Heads";
            ((ISupportInitialize) this.pictureBoxChromaColor).EndInit();
            ((ISupportInitialize) this.pictureBox).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeForm()
        {
            this.DoubleBuffered = true;
            base.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.TopMost = true;
            List<ScreenWindow> availableWindows = ScreenWindow.AvailableWindows;
            availableWindows.Insert(0, new ScreenWindow(IntPtr.Zero, "<Select Window>"));
            this.comboWindows.DataSource = availableWindows;
            this.comboWindows.DisplayMember = "Name";
            this.MinimumSize = new Size(10, 10);
            this.pictureBoxChromaColor.Image = this.CreateSwatch(0x15, 0x15, this.m_ChromaColor);
            this.pictureBox.MouseWheel += new MouseEventHandler(this.PictureBox_MouseWheel);
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && !this.m_IsPickingChromaColor)
            {
                if (this.m_IsFramelessMode || this.m_IsSnipped)
                {
                    this.m_IsFormBeingDragged = true;
                    this.m_LastLocation = e.Location;
                }
                else
                {
                    this.m_IsRubberBandBoxBeingDragged = true;
                    DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
                    this.m_MouseDownPoint = e.Location;
                }
            }
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_IsPickingChromaColor)
            {
                Cursor.Current = Cursors.Cross;
                this.pictureBoxChromaColor.Image.Dispose();
                this.pictureBoxChromaColor.Image = this.CreateSwatch(0x15, 0x15, this.GetColorFromImage(this.pictureBox.Image, e.X, e.Y));
            }
            if ((e.Button == MouseButtons.Left) && !this.m_IsPickingChromaColor)
            {
                if (!this.m_IsFramelessMode)
                {
                    if (this.m_IsRubberBandBoxBeingDragged)
                    {
                        DoubleBufferedPictureBox box = sender as DoubleBufferedPictureBox;
                        this.DrawRubberBandBox(sender, e.Location);
                    }
                }
                else if (this.m_IsFormBeingDragged)
                {
                    int x = (base.Location.X - this.m_LastLocation.X) + e.X;
                    base.Location = new Point(x, (base.Location.Y - this.m_LastLocation.Y) + e.Y);
                    base.Update();
                }
            }
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && !this.m_IsSnipped)
            {
                if (this.m_IsPickingChromaColor)
                {
                    this.m_IsPickingChromaColor = false;
                    this.m_ChromaColor = this.GetColorFromImage(this.pictureBoxChromaColor.Image, 0, 0);
                    Cursor.Current = Cursors.Default;
                }
                if (this.m_IsFramelessMode || this.m_IsPickingChromaColor)
                {
                    this.m_IsFormBeingDragged = false;
                    this.m_LastLocation = Point.Empty;
                }
                else if (this.m_IsRubberBandBoxBeingDragged)
                {
                    this.m_IsRubberBandBoxBeingDragged = false;
                    Rectangle rect = GetRectangle(this.m_MouseDownPoint, e.Location);
                    ((ScreenWindow) this.comboWindows.SelectedItem).SelectedRectangle = rect;
                    this.m_MouseDownPoint = Point.Empty;
                    this.m_LastLocation = Point.Empty;
                    this.AutoSizeWindow(rect);
                    this.comboWindows.Visible = false;
                    base.MinimizeBox = false;
                    base.MaximizeBox = false;
                    this.m_IsSnipped = true;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (this.m_IsPickingChromaColor)
                {
                    this.m_IsPickingChromaColor = false;
                }
                else
                {
                    Rectangle clientRectangle = base.ClientRectangle;
                    this.Toggle(false);
                    base.Location = new Point(base.Location.X + clientRectangle.X, base.Location.Y + clientRectangle.Y);
                }
            }
            if (((e.Button == MouseButtons.Middle) && !this.m_IsPickingChromaColor) && !this.m_IsFramelessMode)
            {
                ((ScreenWindow) this.comboWindows.SelectedItem).SelectedRectangle = Rectangle.Empty;
                base.Size = this.m_PreviousSize;
                base.WindowState = this.m_PreviousWindowState;
                this.m_ImageScale = 1f;
                this.m_IsSnipped = false;
                this.Toggle(true);
            }
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.m_IsSnipped)
            {
                this.m_PreviousImageScale = this.m_ImageScale;
                this.m_ImageScale += e.Delta * 0.0008333334f;
                if (this.m_ImageScale < 0f)
                {
                    this.m_ImageScale = 0f;
                }
            }
        }

        private void pictureBoxChromaColor_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.m_IsPickingChromaColor = true;
                Cursor.Current = Cursors.Cross;
            }
        }

        private void Render(ScreenWindow screenWindow)
        {
            if ((screenWindow != null) && (screenWindow.Handle != IntPtr.Zero))
            {
                Image original = ScreenCapture.Capture3(screenWindow);
                if (original != null)
                {
                    float num3 = original.Width * this.m_ImageScale;
                    float num4 = original.Height * this.m_ImageScale;
                    Bitmap bitmap = new Bitmap(original, new Size((int) num3, (int) num4));
                    original.Dispose();
                    this.pictureBox.Image = bitmap;
                    if (!(this.m_ImageScale == this.m_PreviousImageScale))
                    {
                        this.AutoSizeWindow(new Rectangle(0, 0, (int) num3, (int) num4));
                    }
                }
            }
            if (this.m_IsRubberBandBoxBeingDragged)
            {
                this.DrawRubberBandBox(this.pictureBox, this.m_LastLocation);
            }
            GC.Collect();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Render(this.comboWindows.SelectedItem as ScreenWindow);
        }

        private void Toggle(bool forceDefaultReset = false)
        {
            this.m_IsFramelessMode = !forceDefaultReset && !this.m_IsFramelessMode;
            if (this.m_IsFramelessMode)
            {
                this.BackColor = this.m_ChromaColor;
                base.TransparencyKey = this.m_ChromaColor;
                base.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                this.BackColor = Color.Empty;
                base.TransparencyKey = Color.Empty;
                base.FormBorderStyle = FormBorderStyle.Sizable;
                if (this.m_IsSnipped)
                {
                    this.comboWindows.Visible = false;
                    this.pictureBoxChromaColor.Visible = false;
                }
                else
                {
                    this.comboWindows.Visible = true;
                    this.pictureBoxChromaColor.Visible = true;
                }
            }
        }
    }
}

