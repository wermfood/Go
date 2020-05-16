namespace Wermwerx.Capture
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class LockBitmap
    {
        private readonly Bitmap _source = null;
        private IntPtr _iptr = IntPtr.Zero;
        private BitmapData _bitmapData = null;

        public LockBitmap(Bitmap source)
        {
            this._source = source;
        }

        public Color GetPixel(int x, int y)
        {
            Color empty = Color.Empty;
            int num = this.Depth / 8;
            int index = ((y * this.Width) + x) * num;
            if (index > (this.Pixels.Length - num))
            {
                throw new IndexOutOfRangeException();
            }
            if (this.Depth == 0x20)
            {
                empty = Color.FromArgb(this.Pixels[index + 3], this.Pixels[index + 2], this.Pixels[index + 1], this.Pixels[index]);
            }
            if (this.Depth == 0x18)
            {
                empty = Color.FromArgb(this.Pixels[index + 2], this.Pixels[index + 1], this.Pixels[index]);
            }
            if (this.Depth == 8)
            {
                byte red = this.Pixels[index];
                empty = Color.FromArgb(red, red, red);
            }
            return empty;
        }

        public void LockBits()
        {
            try
            {
                this.Width = this._source.Width;
                this.Height = this._source.Height;
                int num = this.Width * this.Height;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                this.Depth = Image.GetPixelFormatSize(this._source.PixelFormat);
                if (((this.Depth != 8) && (this.Depth != 0x18)) && (this.Depth != 0x20))
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }
                this._bitmapData = this._source.LockBits(rect, ImageLockMode.ReadWrite, this._source.PixelFormat);
                this.Pixels = new byte[num * (this.Depth / 8)];
                this._iptr = this._bitmapData.Scan0;
                Marshal.Copy(this._iptr, this.Pixels, 0, this.Pixels.Length);
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            int num = this.Depth / 8;
            int index = ((y * this.Width) + x) * num;
            if (this.Depth == 0x20)
            {
                this.Pixels[index] = color.B;
                this.Pixels[index + 1] = color.G;
                this.Pixels[index + 2] = color.R;
                this.Pixels[index + 3] = color.A;
            }
            if (this.Depth == 0x18)
            {
                this.Pixels[index] = color.B;
                this.Pixels[index + 1] = color.G;
                this.Pixels[index + 2] = color.R;
            }
            if (this.Depth == 8)
            {
                this.Pixels[index] = color.B;
            }
        }

        public void UnlockBits()
        {
            try
            {
                Marshal.Copy(this.Pixels, 0, this._iptr, this.Pixels.Length);
                this._source.UnlockBits(this._bitmapData);
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
        }

        public byte[] Pixels { get; set; }

        public int Depth { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }
    }
}

