using System.Drawing;
using System.Runtime.InteropServices;

namespace BotUtilities.ImageProcessing.Types
{
    // fast wie Point. aber mit der fixen anordnung das X die ersten 4 bytes hat und Y die nächsten

    [StructLayout(LayoutKind.Sequential)]
    public struct Spot
    {
        public static readonly Spot Empty = default(Spot);

        public int X;
        public int Y;

        public bool IsEmpty { get { return X == 0 && Y == 0; } }

        public Spot(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Spot(int dw)
        {
            X = (int)((short)(dw) & 65535); // LO WORD
            Y = (int)((short)(dw) >> 16 & 65535); // HI WORD
        }

        public static implicit operator Point(Spot spot)
        {
            return new Point(spot.X, spot.Y);
        }

        public override string ToString()
        {
            return "{ X = " + X + "; Y = " + Y + " }";
        }
    }
}
