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
    /// KyoueiView.xaml の相互作用ロジック
    /// </summary>
    public partial class KyoueiView : Window
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
                    ((KyoueiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((KyoueiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
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
                ((KyoueiViewModel)this.DataContext).NefudaBangouText.Value = int.TryParse(this.NefudaBangouText.Text, out nefudaBangou) ? nefudaBangou : 0;
                ((KyoueiViewModel)this.DataContext).CsvReadDisplay();
            }
        }

        public KyoueiView()
        {
            InitializeComponent();
            NefudaBangouText.Focus();
            ((KyoueiViewModel)this.DataContext).NefudaBangouTextBox = this.NefudaBangouText;
            ((KyoueiViewModel)this.DataContext).JusinDatePicker = this.JusinbiDatePicker;
            ((KyoueiViewModel)this.DataContext).NouhinDatePicker = this.NouhinbiDatePicker;
            this.JusinbiDatePicker.Focus();
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
                    this.Owner.Show();
                    break;
                case "F4":
                    ((KyoueiViewModel)this.DataContext).Clear();
                    break;
                case "F5":
                    if (((KyoueiViewModel)this.DataContext).InputCheck())
                    {
                        ((KyoueiViewModel)this.DataContext).CsvReadDisplay();
                    }
                    break;
                case "F10":
                    if (((KyoueiViewModel)this.DataContext).PrintCheck())
                    {
                        ((KyoueiViewModel)this.DataContext).ExecPrint(true);
                    }
                    break;
                case "F12":
                    if (((KyoueiViewModel)this.DataContext).PrintCheck())
                    {
                        ((KyoueiViewModel)this.DataContext).ExecPrint(false);
                    }
                    break;
            }
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    UIElement element = e.OriginalSource as UIElement;
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    UIElement element = sender as UIElement;
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    e.Handled = true;
                }
            }
        }

        // 不正な日時を入力した場合、確定のタイミングで呼び出される
        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            if (sender is DatePicker)
            {
                DatePicker picker = (DatePicker)sender;

                // 文字列をDateTimeに
                DateTime convDt;

                try
                {
                    // 入力された"yyyyMMdd"書式での日付でDateTimeに変換
                    convDt = System.DateTime.ParseExact(picker.Text, "yyyyMMdd",
                                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                System.Globalization.DateTimeStyles.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return;
                }

                // DatePicker用のDateTimeをセット
                picker.SelectedDate = convDt;
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
                    InitialDirectory = System.IO.Path.GetDirectoryName(CommonStrings.KYOEI_SCV_PATH),
                    // ファイル選択モードにする
                    IsFolderPicker = false,
                })
                {
                    if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return;
                    }
                    ((KyoueiViewModel)this.DataContext).FilePathText.Value = cofd.FileName;
                    this.JusinbiDatePicker.Focus();
                }
            }
            else if (button.Name == "btnFileRead")
            {
                if (((KyoueiViewModel)this.DataContext).InputCheck())
                {
                    ((KyoueiViewModel)this.DataContext).CsvReadDisplay();
                }
            }
        }
    }
}
