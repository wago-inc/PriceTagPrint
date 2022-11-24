﻿using MahApps.Metro.Controls;
using PriceTagPrint.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    /// NankokuView.xaml の相互作用ロジック
    /// </summary>
    public partial class NankokuView : MetroWindow
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
                    ((NankokuViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = -1;
                    ((NankokuViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = 0;
                }
                else if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((NankokuViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((NankokuViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
            }
        }

        public NankokuView()
        {
            InitializeComponent();
            this.HakkouTypeText.Focus();
            ((NankokuViewModel)this.DataContext).HakkouTypeTextBox = this.HakkouTypeText;
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
                    ((NankokuViewModel)this.DataContext).Clear();
                    this.HakkouTypeText.Focus();
                    this.HakkouTypeText.SelectAll();
                    break;
                case "F5":
                    if (((NankokuViewModel)this.DataContext).InputCheck())
                    {
                        ((NankokuViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F10":
                    if (((NankokuViewModel)this.DataContext).PrintCheck())
                    {
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((NankokuViewModel)this.DataContext).ExecPrint(true);
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
                    if (((NankokuViewModel)this.DataContext).PrintCheck())
                    {
                        if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            ((NankokuViewModel)this.DataContext).ExecPrint(false);
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
                    if (((NankokuViewModel)this.DataContext).InputCheck())
                    {
                        int nefudaBangou;
                        ((NankokuViewModel)this.DataContext).NefudaBangouText.Value = int.TryParse(this.NefudaBangouText.Text, out nefudaBangou) ? nefudaBangou : 0;
                        ((NankokuViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                }
            }
        }
    }
}
