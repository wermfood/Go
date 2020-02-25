using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisembodiedHeads
{
    public partial class DoubleBufferedPictureBox : System.Windows.Forms.PictureBox
    {
        public DoubleBufferedPictureBox()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        public DoubleBufferedPictureBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
