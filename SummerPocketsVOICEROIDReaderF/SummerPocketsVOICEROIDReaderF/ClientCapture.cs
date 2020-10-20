using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SummerPocketsVOICEROIDReaderF
{
    /// <summary>
    /// https://ja.stackoverflow.com/questions/21328/c-process%e3%81%a7%e6%8c%87%e5%ae%9a%e3%81%97%e3%81%9f%e5%88%a5%e3%83%97%e3%83%ad%e3%82%bb%e3%82%b9%e3%81%ae%e3%82%a6%e3%82%a4%e3%83%b3%e3%83%89%e3%82%a6%e3%82%92-%e3%82%af%e3%83%a9%e3%82%a4%e3%82%a2%e3%83%b3%e3%83%88%e9%a0%98%e5%9f%9f%e3%81%ae%e3%81%bf%e3%82%ad%e3%83%a3%e3%83%97%e3%83%81%e3%83%a3%e3%81%97%e3%81%9f%e3%81%84 様より
    /// </summary>
    public static class ClientCapture
    {
        //クライアント領域キャプチャ用のメソッドと、戻り値用の構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hwnd, out POINT lpPoint);

        /// <summary>
        /// 画面をキャプチャしてBitmapを返す。
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>クライアント領域のBitmap。キャプチャに失敗した場合null。</returns>
        public static Bitmap GetBitmap(IntPtr handle)
        {
            var rect = new RECT();
            if (GetClientRect(handle, out rect) == 0)
            {
                //キャプチャ失敗
                return null;
            }
            var size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            if (size.Width <= 0 || size.Height <= 0)
            {
                //キャプチャ失敗
                return null;
            }
            var img = new Bitmap(size.Width, size.Height);
            var pt = new POINT { x = rect.left, y = rect.top };
            ClientToScreen(handle, out pt);
            using (var g = Graphics.FromImage(img))
            {
                g.CopyFromScreen(pt.x, pt.y, 0, 0, img.Size);
            }
            return img;
        }
    }
}
