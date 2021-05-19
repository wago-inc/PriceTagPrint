using PriceTagPrint.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace PriceTagPrint.View
{
    /// <summary>
    /// YamanakaView.xaml の相互作用ロジック
    /// </summary>
    public partial class YamanakaView : Window
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
        /// 発行区分・値札区分テキストの入力制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 入力値が数値か否か判定し、数値ではない場合、処理済みにします。
            var regex = new Regex("[^1-2]+");
            var text = e.Text;
            var result = regex.IsMatch(text);
            e.Handled = result;
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けの場合
            if (e.Command == ApplicationCommands.Paste)
            {
                // 処理済みにします。
                e.Handled = true;
            }
        }

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
                if (txt.Name == "HakkouTypeText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((YamanakaViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = -1;
                    ((YamanakaViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = 0;
                }
                else if (txt.Name == "BunruiCodeText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((YamanakaViewModel)this.DataContext).SelectedBunruiCodeIndex.Value = -1;
                    ((YamanakaViewModel)this.DataContext).SelectedBunruiCodeIndex.Value = 0;
                }
                else if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((YamanakaViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((YamanakaViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
                else if (txt.Name == "McCodeText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((YamanakaViewModel)this.DataContext).SelectedMcCodeIndex.Value = -1;
                    ((YamanakaViewModel)this.DataContext).SelectedMcCodeIndex.Value = 0;
                }
            }
        }

        public YamanakaView()
        {
            InitializeComponent();
            this.HakkouTypeText.Focus();
            // ViewModelからも初期フォーカスをセットできるように発行区分テキストを渡す。
            ((YamanakaViewModel)this.DataContext).HakkouTypeTextBox = this.HakkouTypeText;
            ((YamanakaViewModel)this.DataContext).JusinDatePicker = this.JusinbiDatePicker;
            ((YamanakaViewModel)this.DataContext).NouhinDatePicker = this.NouhinbiDatePicker;
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
                    ((YamanakaViewModel)this.DataContext).Clear();
                    this.HakkouTypeText.Focus();
                    this.HakkouTypeText.SelectAll();
                    break;
                case "F5":
                    if (((YamanakaViewModel)this.DataContext).InputCheck())
                    {
                        ((YamanakaViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F10":
                    if (((YamanakaViewModel)this.DataContext).PrintCheck())
                    {
                        ((YamanakaViewModel)this.DataContext).ExecPrint(true);
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F12":
                    if (((YamanakaViewModel)this.DataContext).PrintCheck())
                    {
                        ((YamanakaViewModel)this.DataContext).ExecPrint(false);
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
            }
        }

        /// <summary>
        /// F5検索クリック時の入力チェック
        /// </summary>
        /// <returns></returns>
        //public bool InputCheck()
        //{
        //    DateTime convDate;
        //    if (string.IsNullOrEmpty(this.HakkouTypeText.Text))
        //    {
        //        MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        this.HakkouTypeText.Focus();
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.JusinbiDatePicker.Text) || !DateTime.TryParse(this.JusinbiDatePicker.Text, out convDate))
        //    {
        //        MessageBox.Show("受信日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        this.JusinbiDatePicker.Focus();
        //        return false;
        //    }
        //    if (!string.IsNullOrEmpty(this.NouhinbiDatePicker.Text) || !DateTime.TryParse(this.NouhinbiDatePicker.Text, out convDate))
        //    {
        //        MessageBox.Show("納品日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        this.NouhinbiDatePicker.Focus();
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.NefudaBangouText.Text))
        //    {
        //        MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.McCodeText.Text))
        //    {
        //        MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 値札番号エンターで検索処理実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(this.HakkouTypeText.Text))
                {
                    if (((YamanakaViewModel)this.DataContext).InputCheck())
                    {
                        int nefudaBangou;
                        ((YamanakaViewModel)this.DataContext).NefudaBangouText.Value = int.TryParse(this.NefudaBangouText.Text, out nefudaBangou) ? nefudaBangou : 0;
                        ((YamanakaViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                }
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
    }
}
