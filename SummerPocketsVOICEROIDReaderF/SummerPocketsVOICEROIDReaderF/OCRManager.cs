using System;
using System.Drawing;
using System.IO;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Tesseract;

namespace SummerPocketsVOICEROIDReaderF
{
    public class OcrManager : IDisposable
    {
        private const string LangPath = @"tessdata";
        private const string LngStr = "jpnv1";
        private TesseractEngine _tesseract;

        public OcrManager()
        {
            _tesseract = new TesseractEngine(LangPath, LngStr);
            _tesseract.SetVariable("tessedit_char_blacklist",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz〈ヽ〉'〝<〟‥“\\ゐ=`”_.丿″\"");
        }

        public string GetTextFromBitmap(Image bmp)
        {
            var pix = Pix.LoadTiffFromMemory(GetByteArrayFromImage(bmp));
            var page = _tesseract.Process(pix);
            var text = page.GetText().Replace(" ", "").Replace("　", "").Replace("\n", "");

            return text;
        }

        private byte[] GetByteArrayFromImage(Image bmp)
        {
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Tiff);
            return ms.GetBuffer();
        }

        public void Dispose()
        {
            _tesseract?.Dispose();
        }
    }
}
