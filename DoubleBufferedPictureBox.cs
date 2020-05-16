namespace DisembodiedHeads
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    public class DoubleBufferedPictureBox : PictureBox
    {
        private IContainer components;

        public DoubleBufferedPictureBox()
        {
            this.components = null;
            this.InitializeComponent();
            this.DoubleBuffered = true;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        public DoubleBufferedPictureBox(IContainer container)
        {
            this.components = null;
            container.Add(this);
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }
    }
}

