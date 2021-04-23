using Microsoft.WindowsAPICodePack.Dialogs;
using Oracle.DataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.Model;
using PriceTagPrint.ViewModel;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MSAPI = Microsoft.WindowsAPICodePack;

namespace PriceTagPrint.View
{
    /// <summary>
    /// ItougofukuView.xaml の相互作用ロジック
    /// </summary>
    public partial class ItougofukuView : Window
    {
        #region "最大化・最小化・閉じるボタンの非表示設定"

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }

        #endregion

        /// <summary>
        /// 発行区分・値札区分テキストの空白LostFocus処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (string.IsNullOrEmpty(txt.Text))
            {
                if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((ItougofukuViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((ItougofukuViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
            }
        }

        /// <summary>
        /// 値札番号エンターで検索処理実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int nefudaBangou;
                ((ItougofukuViewModel)this.DataContext).NefudaBangouText.Value = int.TryParse(this.NefudaBangouText.Text, out nefudaBangou) ? nefudaBangou : 0;
                ((ItougofukuViewModel)this.DataContext).CsvReadDisplay();
            }
        }

        public ItougofukuView()
        {
            InitializeComponent();
            NefudaBangouText.Focus();
            ((ItougofukuViewModel)this.DataContext).NefudaBangouTextBox = this.NefudaBangouText;
        }

        /// <summary>
        /// ファンクションキークリック処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            var parameter = e.RoutedEvent.Name == "Executed" ?
                            ((System.Windows.Input.ExecutedRoutedEventArgs)e).Parameter :
                            ((System.Windows.Controls.Primitives.ButtonBase)e.Source).CommandParameter;
            switch (parameter)
            {
                case "Esc":
                    this.Close();
                    var view = new MainWindow();
                    view.Show();
                    break;
                case "F4":
                    ((ItougofukuViewModel)this.DataContext).Clear();
                    break;
                case "F5":
                    if (((ItougofukuViewModel)this.DataContext).InputCheck())
                    {
                        ((ItougofukuViewModel)this.DataContext).CsvReadDisplay();
                    }
                    break;
                case "F10":
                    if (((ItougofukuViewModel)this.DataContext).PrintCheck())
                    {
                        ((ItougofukuViewModel)this.DataContext).ExecPrint(true);
                    }
                    break;
                case "F12":
                    if (((ItougofukuViewModel)this.DataContext).PrintCheck())
                    {
                        ((ItougofukuViewModel)this.DataContext).ExecPrint(false);
                    }
                    break;
            }
        }

        private void Act_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Name == "btnFileSelect")
            {
                using (var cofd = new CommonOpenFileDialog()
                {
                    Title = "フォルダを選択してください",
                    InitialDirectory = CommonStrings.ITO_SCV_DIR_PATH,
                    // ファイル選択モードにする
                    IsFolderPicker = false,
                })
                {
                    if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return;
                    }

                                ((ItougofukuViewModel)this.DataContext).FilePathText.Value = cofd.FileName;
                }
            }
            else if (button.Name == "btnFileRead")
            {
                if (((ItougofukuViewModel)this.DataContext).InputCheck())
                {
                    ((ItougofukuViewModel)this.DataContext).CsvReadDisplay();
                }
            }
        }
    }
}
