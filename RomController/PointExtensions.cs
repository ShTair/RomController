using System.Drawing;

namespace RomController
{
    public static class PointExtensions
    {
        public static void Click(this Point point)
        {
            Mouse.Click(point.X, point.Y);
        }

        public static void Move(this Point point)
        {
            Mouse.Move(point.X, point.Y);
        }
    }
}
