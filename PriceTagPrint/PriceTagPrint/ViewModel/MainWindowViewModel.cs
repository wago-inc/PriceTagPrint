using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PriceTagPrint.Model.MainWindowModel;

namespace PriceTagPrint.ViewModel
{
    class MainWindowViewModel : ViewModelsBase
    {
        public Window window;
        public double Number { get; set; }

        public MainWindowViewModel()
        {
            this.SelectCommand = new DelegateCommand<string>(ShowDisplay);
        }
        public ICommand SelectCommand { get; private set; }
        
        private void ShowDisplay(string id)
        {
            if (!RegistryUtil.isInstalled("Multi LABELIST V5"))
            {
                MessageBox.Show("Multi LABELIST V5がインストールされていません。", "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var torihikisaki = new TorihikisakiList().list.FirstOrDefault(x => x.Id == id);
            if(torihikisaki != null)
            {
                switch(torihikisaki.Id)
                {
                    case TorihikisakiId.YASUSAKI:
                        var view = new YasusakiView();
                        view.Owner = window;
                        view.Show();
                        view.Owner.Hide();
                        break;
                    case TorihikisakiId.YAMANAKA:
                        break;
                    case TorihikisakiId.MARUYOSI:
                        break;
                    case TorihikisakiId.OKINAWA_SANKI:
                        break;
                    default:break;
                }
            }
        }
    }
    
}
