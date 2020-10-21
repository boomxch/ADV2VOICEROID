using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;

namespace SummerPocketsVOICEROIDReaderF
{
    // 一文字ずつトリムして並列処理を行う(OCR)
    // 訓練データを特定フォントに最適化する
    class Program
    {
        private static readonly Timer timer = new Timer(1500);
        private static IntPtr mainWindowHandle;
        private static int beforeImage;

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetForegroundWindow();

        static void Main()
        {
            // ゲームのプロセス取得
            var processes = Process.GetProcessesByName("SiglusEngine");
            if (!processes.Any())
            {
                Console.WriteLine("SiglusEngineを起動してください。");
                Console.WriteLine("Press enter to finish...");
                Console.ReadLine();
                return;
            }

            mainWindowHandle = processes.First().MainWindowHandle;

            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            Console.ReadLine();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (GetForegroundWindow() != mainWindowHandle)
            {
                Console.WriteLine("ADVアプリはインアクティブです。");
                return;
            }

            var text = GetTextFromWindow();

            if (text == "") return;

            Console.WriteLine(text);
            var last = text.Last();
            if (last != '。' && last != '」' && last != '?' && last != '・' && last != '…') return;

            // VOICEROID2で再生
            VoiceroidManager.Talk(text);
        }

        private static string GetTextFromWindow()
        {
            // 画面キャプチャ
            var bmp = ClientCapture.GetBitmap(mainWindowHandle);

            // トリミング
            var rect = new Rectangle(
                ImageProcessor.ConvertFullHDToBmpLength(350, true, bmp), ImageProcessor.ConvertFullHDToBmpLength(840, false, bmp),
                ImageProcessor.ConvertFullHDToBmpLength(1650 - 350, true, bmp), ImageProcessor.ConvertFullHDToBmpLength(1000 - 840, false, bmp));
            bmp = bmp.Clone(rect, bmp.PixelFormat);

            // 画像加工(文字を識別しやすくする)
            ImageProcessor.ConvertWhiteAndOther(bmp);

            // 直前の画像と一致していないか判別
            var num = ImageProcessor.CountWhite(bmp);
            if (num == beforeImage)
                return "";

            beforeImage = num;

            // OCRによる文字読み取り
            var ocrManager = new OcrManager();
            var text = ocrManager.GetTextFromBitmap(bmp);

            // 加工 issue#1
            text = text.Replace("聚", "♪");

            // 加工 issue#2(一部)
            if (text.IndexOf('」') >= text.IndexOf('「')) return text;

            var index = text.IndexOf('」');
            text = text.Substring(index + 1, text.Length - index -1) + text.Substring(0, index);

            return text;
        }
    }
}
