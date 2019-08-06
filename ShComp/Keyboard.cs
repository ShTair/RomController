using System.Runtime.InteropServices;

namespace ShComp
{
    public static class Keyboard
    {
        public static void SendKey(byte code)
        {
            keybd_event(code, 0, 0, 0);
            keybd_event(code, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void SendKeys(string keys)
        {
            System.Windows.Forms.SendKeys.SendWait(keys);
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const uint KEYEVENTF_KEYUP = 0x0002;
    }
}
