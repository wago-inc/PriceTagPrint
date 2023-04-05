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
    /// IzumiView.xaml の相互作用ロジック
    /// </summary>
    public partial class IzumiView : MetroWindow
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

        public IzumiView()
        {
            InitializeComponent();
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
                    ((IzumiViewModel)this.DataContext).Clear();
                    break;
                case "F5":
                    if (((IzumiViewModel)this.DataContext).InputCheck())
                    {
                        ((IzumiViewModel)this.DataContext).CsvReadDisplay();
                    }
                    break;
                case "F10":
                    if (((IzumiViewModel)this.DataContext).PrintCheck())
                    {
                        ((IzumiViewModel)this.DataContext).ExecPrint(true);
                    }
                    break;
                case "F12":
                    if (((IzumiViewModel)this.DataContext).PrintCheck())
                    {
                        ((IzumiViewModel)this.DataContext).ExecPrint(false);
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
                    InitialDirectory = System.IO.Path.GetDirectoryName(CommonStrings.IZUMI_SCV_PATH),
                    // ファイル選択モードにする
                    IsFolderPicker = false,
                })
                {
                    if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                    {
                        return;
                    }

                                ((IzumiViewModel)this.DataContext).FilePathText.Value = cofd.FileName;
                }
            }
            else if (button.Name == "btnFileRead")
            {
                if (((IzumiViewModel)this.DataContext).InputCheck())
                {
                    ((IzumiViewModel)this.DataContext).CsvReadDisplay();
                }
            }
        }
    }
}
