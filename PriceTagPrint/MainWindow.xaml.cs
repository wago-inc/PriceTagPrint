﻿using PriceTagPrint.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PriceTagPrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            vm.window = this;
            this.DataContext = vm;
        }

        public MainWindow(MenuKind menuKind)
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            vm.window = this;            
            this.DataContext = vm;
            vm.SubMenuKind.Value = menuKind;
        }
        private void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }
}
