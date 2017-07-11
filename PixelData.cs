using System.Drawing;
using System.Runtime.InteropServices;

namespace BotUtilities.ImageProcessing.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PixelData
    {
        public byte B;
        public byte G;
        public byte R;

        public PixelData(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public PixelData(int hexColor)
        {
            Color color = Color.FromArgb(hexColor);
            R = color.R;
            G = color.G;
            B = color.B;
        }
        public PixelData(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }

        public int ToInt()
        {
            return Color.FromArgb(R, G, B).ToArgb();
        }

        public bool Compare(PixelData sec, byte variation = 0)
        {
            return (sec.R >= R - variation && sec.R <= R + variation) && (sec.G >= G - variation && sec.G <= G + variation) && (sec.B >= B - variation && sec.B <= B + variation);
        }
    }
}
