using System.Drawing;
using System.Runtime.InteropServices;

namespace BotUtilities.ImageProcessing.Types
{

    // fast wie Rectangle aber dazu gedacht einen bereich auf dem bildschirm einzugrenzen

    [StructLayout(LayoutKind.Sequential)]
    public struct Boundary
    {
        public static readonly Boundary Empty = default(Boundary);

        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public Boundary(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public static implicit operator Rectangle(Boundary bounds)
        {
            return new Rectangle(bounds.Left, bounds.Right, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
        }

        public override string ToString()
        {
            return "{ Left = " + Left + "; Right = " + Right + "; Top = " + Top + "; Bottom = " + Bottom + " }";
        }
    }
}
