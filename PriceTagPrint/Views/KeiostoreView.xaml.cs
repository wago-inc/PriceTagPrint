using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Oracle.DataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.Models;
using PriceTagPrint.ViewModels;
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

namespace PriceTagPrint.Views
{
    /// <summary>
    /// KeiostoreView.xaml の相互作用ロジック
    /// </summary>
    public partial class KeiostoreView : MetroWindow
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

        /// <summary>
        /// 数字のみテキストの入力制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewNumericTextInput(object sender, TextCompositionEventArgs e)
        {
            // 入力値が数値か否か判定し、数値ではない場合、処理済みにします。
            var regex = new Regex("[^0-9]+");
            var text = e.Text;
            var result = regex.IsMatch(text);
            e.Handled = result;
        }

        /// <summary>
        /// 日付テキストの入力制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewDateTextInput(object sender, TextCompositionEventArgs e)
        {
            // 入力値が数値か否か判定し、数値ではない場合、処理済みにします。
            var regex = new Regex("[^0-9/]+");
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
                    ((KeiostoreViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = -1;
                    ((KeiostoreViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = 0;
                }
                else if (txt.Name == "BunruiCodeText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((KeiostoreViewModel)this.DataContext).SelectedBunruiCodeIndex.Value = -1;
                    ((KeiostoreViewModel)this.DataContext).SelectedBunruiCodeIndex.Value = 0;
                }
                else if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((KeiostoreViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((KeiostoreViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
            }
        }
        public KeiostoreView()
        {
            InitializeComponent();
            this.HakkouTypeText.Focus();
            ((KeiostoreViewModel)this.DataContext).HakkouTypeTextBox = this.HakkouTypeText;
            ((KeiostoreViewModel)this.DataContext).JusinDatePicker = this.JusinbiDatePicker;
            ((KeiostoreViewModel)this.DataContext).NouhinDatePicker = this.NouhinbiDatePicker;
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
                    ((KeiostoreViewModel)this.DataContext).Clear();
                    this.HakkouTypeText.Focus();
                    this.HakkouTypeText.SelectAll();
                    break;
                case "F5":
                    if (((KeiostoreViewModel)this.DataContext).InputCheck())
                    {
                        ((KeiostoreViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F10":
                    if (((KeiostoreViewModel)this.DataContext).PrintCheck())
                    {
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((KeiostoreViewModel)this.DataContext).ExecPrint(true);
                            this.HakkouTypeText.Focus();
                            this.HakkouTypeText.SelectAll();
                        }
                    }
                    else
                    {
                        MessageBox.Show("対象データが存在しません。", "値札発行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "F12":
                    if (((KeiostoreViewModel)this.DataContext).PrintCheck())
                    {
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((KeiostoreViewModel)this.DataContext).ExecPrint(false);
                            this.HakkouTypeText.Focus();
                            this.HakkouTypeText.SelectAll();
                        }
                    }
                    else
                    {
                        MessageBox.Show("対象データが存在しません。", "値札発行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
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
                if (!string.IsNullOrEmpty(this.HakkouTypeText.Text))
                {
                    if (((KeiostoreViewModel)this.DataContext).InputCheck())
                    {
                        // 何故か変更通知が飛ばないので検索処理直前にセット
                        ((KeiostoreViewModel)this.DataContext).SttHincd.Value = this.SttHincdText.Text;
                        ((KeiostoreViewModel)this.DataContext).EndHincd.Value = this.EndHincdText.Text;
                        ((KeiostoreViewModel)this.DataContext).NefudaDataDisplay();
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

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            var selectedItem = combo.SelectedItem;
            int convTxt;
            if (selectedItem != null)
            {
                if (combo.Name == "BunruiCodeComboBox" && BunruiCodeText != null && int.TryParse(BunruiCodeText.Text, out convTxt))
                {
                    if (((CommonIdName)selectedItem).Id != convTxt)
                    {
                        BunruiCodeText.Focus();
                        BunruiCodeText.SelectAll();
                    }
                }
                else if (combo.Name == "NefudaBangouComboBox" && NefudaBangouText != null && int.TryParse(NefudaBangouText.Text, out convTxt))
                {
                    if (((CommonIdName)selectedItem).Id != convTxt)
                    {
                        NefudaBangouText.Focus();
                        NefudaBangouText.SelectAll();
                    }
                }
            }
        }
    }
}
