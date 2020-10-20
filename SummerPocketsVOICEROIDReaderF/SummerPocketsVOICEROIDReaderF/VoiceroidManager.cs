using System;
using System.Diagnostics;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;

namespace SummerPocketsVOICEROIDReaderF
{
    public static class VoiceroidManager
    {
        private static IntPtr hWnd = IntPtr.Zero;
        /// <summary>
        /// https://hgotoh.jp/wiki/doku.php/documents/voiceroid/tips/tips-003 様より
        /// </summary>

        // テキスト転記と再生ボタン押下
        public static void Talk(string talkText)
        {
            if (hWnd == IntPtr.Zero)
                hWnd = GetVoiceroid2HWnd();

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("VOICEROID2を起動してください。");
                return;
            }

            try
            {
                // プロセスに接続する
                var app = new WindowsAppFriend(hWnd);

                // テキスト入力欄と再生ボタンを特定する
                var uiTreeTop = WindowControl.FromZTop(app);
                var textEditView = uiTreeTop.GetFromTypeFullName("AI.Talk.Editor.TextEditView")[0].LogicalTree();
                var talkTextBox = new WPFTextBox(textEditView[4]);
                var playButton = new WPFButtonBase(textEditView[6]);

                // テキストを入力し、再生する
                talkTextBox.EmulateChangeText(talkText);
                playButton.EmulateClick();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                hWnd = IntPtr.Zero;
            }
        }

        // VOICEROID2 EDITOR ウインドウハンドル検索
        private static IntPtr GetVoiceroid2HWnd()
        {
            var voiceroidProcesses = Process.GetProcessesByName("VoiceroidEditor");
            if (voiceroidProcesses.Length == 0)
                return IntPtr.Zero;
            var process = voiceroidProcesses[0];

            return process.MainWindowHandle;
        }
    }
}