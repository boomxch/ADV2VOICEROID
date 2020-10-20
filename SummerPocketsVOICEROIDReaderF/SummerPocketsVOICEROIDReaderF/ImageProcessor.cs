using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SummerPocketsVOICEROIDReaderF
{
    public static class ImageProcessor
    {
        public static int ConvertFullHDToBmpLength(int length, bool isHorizontal, Image bmp)
        {
            var d = isHorizontal ? (double)length / 1920 : (double)length / 1080;
            return (int)Math.Round(d * (isHorizontal ? bmp.Width : bmp.Height));
        }

        /// <summary>
        /// Bitmapのピクセルデータに高速にアクセスし、RGB値がそれぞれ近いものを(buf[i], buf[i], buf[i])とし、他を(0,0,0)に変換する
        /// </summary>
        /// <param name="bitmap"></param>
        public static void ConvertWhiteAndOther(Bitmap bitmap)
        {
            const int threshold = 5;

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            var buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            for (var i = 0; i < buf.Length;)
            {
                var num = (Math.Abs(buf[i] - buf[i + 1]) < threshold && Math.Abs(buf[i] - buf[i + 2]) < threshold) ? buf[i] : (byte)0;
                buf[i++] = num;
                buf[i++] = num;
                buf[i++] = num;
                i++;
            }
            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);
        }

        /// <summary>
        /// Bitmapのピクセルデータに高速にアクセスし、RGB値がそれぞれ定数値以上のものを(255,255,255)とし、他を(0,0,0)に変換する
        /// </summary>
        /// <param name="bitmap"></param>
        public static void ConvertWhiteAndOtherByThreshold(Bitmap bitmap)
        {
            const int threshold = 150;

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            var buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            for (var i = 0; i < buf.Length;)
            {
                var num = (buf[i] > threshold && buf[i + 1] > threshold && buf[i + 2] > threshold) ? (byte)255 : (byte)0;
                buf[i++] = num;
                buf[i++] = num;
                buf[i++] = num;
                i++;
            }
            Marshal.Copy(buf, 0, data.Scan0, buf.Length);
            bitmap.UnlockBits(data);
        }

        public static int CountWhite(Bitmap bitmap)
        {
            const int threshold = 230;

            var ans = 0;

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            var buf = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);

            for (var i = 0; i < buf.Length; i += 4)
                if (buf[i] >= threshold && buf[i + 1] >= threshold && buf[i + 2] >= threshold) ans++;

            bitmap.UnlockBits(data);

            return ans;
        }
    }
}
