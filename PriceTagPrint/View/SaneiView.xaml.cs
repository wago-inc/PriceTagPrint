﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
using MahApps.Metro.Controls;

namespace PriceTagPrint.View
{
    /// <summary>
    /// SaneiView.xaml の相互作用ロジック
    /// </summary>
    public partial class SaneiView : MetroWindow
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
                    ((SaneiViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = -1;
                    ((SaneiViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = 0;
                }
                else if (txt.Name == "VbunCdText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((SaneiViewModel)this.DataContext).SelectedVbunCdIndex.Value = -1;
                    ((SaneiViewModel)this.DataContext).SelectedVbunCdIndex.Value = 0;
                }
                else if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((SaneiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((SaneiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
            }
        }
        public SaneiView()
        {
            InitializeComponent();
            this.HakkouTypeText.Focus();
            // ViewModelからも初期フォーカスをセットできるように発行区分テキストを渡す。
            ((SaneiViewModel)this.DataContext).HakkouTypeTextBox = this.HakkouTypeText;
            ((SaneiViewModel)this.DataContext).JusinDatePicker = this.JusinbiDatePicker;
            ((SaneiViewModel)this.DataContext).NouhinDatePicker = this.NouhinbiDatePicker;
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
                    ((SaneiViewModel)this.DataContext).Clear();
                    this.HakkouTypeText.Focus();
                    this.HakkouTypeText.SelectAll();
                    break;
                case "F5":
                    if (((SaneiViewModel)this.DataContext).InputCheck())
                    {
                        ((SaneiViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F10":
                    if (((SaneiViewModel)this.DataContext).PrintCheck())
                    {
                        if (this.NefudaBangouText.IsFocused)
                        {
                            this.Fnc10.Focus();
                        }
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((SaneiViewModel)this.DataContext).ExecPrint(true);
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
                    if (((SaneiViewModel)this.DataContext).PrintCheck())
                    {
                        if (this.NefudaBangouText.IsFocused)
                        {
                            this.Fnc12.Focus();
                        }
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((SaneiViewModel)this.DataContext).ExecPrint(false);
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
                    if (((SaneiViewModel)this.DataContext).InputCheck())
                    {
                        // 何故か変更通知が飛ばないので検索処理直前にセット
                        ((SaneiViewModel)this.DataContext).SttHincd.Value = this.SttHincdText.Text;
                        ((SaneiViewModel)this.DataContext).EndHincd.Value = this.EndHincdText.Text;
                        ((SaneiViewModel)this.DataContext).NefudaDataDisplay();
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
