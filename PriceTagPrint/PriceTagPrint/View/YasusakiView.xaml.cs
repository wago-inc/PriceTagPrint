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

namespace PriceTagPrint.View
{
    /// <summary>
    /// YasusakiView.xaml の相互作用ロジック
    /// </summary>
    public partial class YasusakiView : Window
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
                    ((YasusakiViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = -1;
                    ((YasusakiViewModel)this.DataContext).SelectedHakkouTypeIndex.Value = 0;
                }
                else if (txt.Name == "NefudaBangouText")
                {
                    // 変更通知が飛ばないため一旦-1をセット
                    ((YasusakiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = -1;
                    ((YasusakiViewModel)this.DataContext).SelectedNefudaBangouIndex.Value = 0;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YasusakiView()
        {
            InitializeComponent();
            this.HakkouTypeText.Focus();
            // ViewModelからも初期フォーカスをセットできるように発行区分テキストを渡す。
            ((YasusakiViewModel)this.DataContext).HakkouTypeTextBox = this.HakkouTypeText;
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
                    ((YasusakiViewModel)this.DataContext).Clear();
                    this.HakkouTypeText.Focus();
                    this.HakkouTypeText.SelectAll();
                    break;
                case "F5":
                    if (((YasusakiViewModel)this.DataContext).InputCheck())
                    {
                        ((YasusakiViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F10":
                    if (((YasusakiViewModel)this.DataContext).PrintCheck())
                    {
                        ((YasusakiViewModel)this.DataContext).ExecPrint(true);
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                    break;
                case "F12":
                    if (((YasusakiViewModel)this.DataContext).PrintCheck())
                    {
                        ((YasusakiViewModel)this.DataContext).ExecPrint(false);
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
        //    if (string.IsNullOrEmpty(this.HakkouTypeText.Text))
        //    {
        //        MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.HachuNumberTextBox.Text))
        //    {
        //        MessageBox.Show("発注番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    if (!string.IsNullOrEmpty(HnoResultTextBox.Text) && HnoResultTextBox.Text.Contains("未登録"))
        //    {
        //        MessageBox.Show("未登録の発注番号が選択されています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.BunruiCodeText.Text))
        //    {
        //        MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.NefudaBangouText.Text))
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
                    if (((YasusakiViewModel)this.DataContext).InputCheck())
                    {
                        int nefudaBangou;
                        ((YasusakiViewModel)this.DataContext).NefudaBangouText.Value = int.TryParse(this.NefudaBangouText.Text, out nefudaBangou) ? nefudaBangou : 0;
                        ((YasusakiViewModel)this.DataContext).NefudaDataDisplay();
                        this.HakkouTypeText.Focus();
                        this.HakkouTypeText.SelectAll();
                    }
                }
            }
        }
    }
}
