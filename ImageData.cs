using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using BotUtilities.ImageProcessing.Native;

namespace BotUtilities.ImageProcessing.Types
{
    public class ImageData
    {
        public byte[] RawData;

        public int Stride;

        public Spot Mid;

        public int Width;
        public int Height;

        public Rectangle Bounds;

        public ImageData(string path)
        {
            FromBitmap(new Bitmap(path));
        }
        public ImageData(IntPtr hwnd)
        {
            FromWindow(hwnd);
        }
        public ImageData(Bitmap bmp)
        {
            FromBitmap(bmp);
        }

        private void FromWindow(IntPtr hwnd)
        {
            IntPtr hDc = User32.GetDC(hwnd);

            IntPtr memhDC = Gdi32.CreateCompatibleDC(hDc);

            User32.RECT clientSize;
            User32.GetClientRect(hwnd, out clientSize);

            Point bounds = new Point(clientSize.Right - clientSize.Left, clientSize.Bottom - clientSize.Top);

            IntPtr hBmp = Gdi32.CreateCompatibleBitmap(hDc, bounds.X, bounds.Y);

            Gdi32.SelectObject(memhDC, hBmp);

            Gdi32.BitBlt(memhDC, 0, 0, bounds.X, bounds.Y, hDc, 0, 0, Gdi32.TernaryRasterOperations.SRCCOPY);

            Bitmap bmp = Bitmap.FromHbitmap(hBmp);

            Gdi32.DeleteObject(hBmp);
            User32.ReleaseDC(hwnd, hDc);
            Gdi32.DeleteDC(memhDC);

            FromBitmap(bmp);
        }

        private void FromBitmap(Bitmap bmp)
        {
            Width = bmp.Width;
            Height = bmp.Height;

            Mid = new Spot(Width / 2, Height / 2);

            Bounds = new Rectangle(0, 0, Width, Height);

            BitmapData bmpData = bmp.LockBits(Bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            Stride = bmpData.Stride;

            RawData = new byte[bmpData.Stride * Height];

            Marshal.Copy(bmpData.Scan0, RawData, 0, RawData.Length);

            bmp.UnlockBits(bmpData);
        }

        public Bitmap ToBitmap()
        {
            Bitmap buffer = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = buffer.LockBits(Bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            Marshal.Copy(RawData, 0, bmpData.Scan0, RawData.Length);

            buffer.UnlockBits(bmpData);

            return buffer;
        }

        public void SaveBitmap(string path)
        {
            ToBitmap().Save(path);
        }

        public void Resize(int width, int height)
        {
            FromBitmap(new Bitmap(ToBitmap(), width, height));
        }

        public PixelData GetPixel(int x, int y)
        {
            int index = y * Stride + x * 3;
            return new PixelData(RawData[index + 2], RawData[index + 1], RawData[index]);
        }

        public PixelData GetPixel(Point point)
        {
            return GetPixel(point.X, point.Y);
        }

        public void SetPixel(int x, int y, PixelData pixel)
        {
            int index = y * Stride + x * 3;
            RawData[index + 2] = pixel.R;
            RawData[index + 1] = pixel.G;
            RawData[index] = pixel.B;
        }

        public void SetPixel(Point point, PixelData pixel)
        {
            SetPixel(point.X, point.Y, pixel);
        }
    }
}
