using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.View;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PriceTagPrint.Model.MainWindowModel;

namespace PriceTagPrint.ViewModel
{
    public enum MenuKind { Auto, Input}
    class MainWindowViewModel : ViewModelsBase
    {
        public ReactiveProperty<MenuKind> SubMenuKind { get; set; } = new ReactiveProperty<MenuKind>(MenuKind.Auto);

        public ReactiveProperty<string> SubTitleText { get; set; } = new ReactiveProperty<string>("自動発行メニュー");

        public ReactiveProperty<Visibility> AutoMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Visible);

        public ReactiveProperty<Visibility> InputMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Hidden);

        public Window window;
        public double Number { get; set; }
        private TorihikisakiList Torihikisakis;

        public MainWindowViewModel()
        {
            Torihikisakis = new TorihikisakiList();
            this.SelectAutoCommand = new DelegateCommand<string>(ShowAutoDisplay);
            this.SelectInputCommand = new DelegateCommand<string>(ShowInputDisplay);

            SubMenuKind.Subscribe(x => MenuKindChanged(x));
        }
        public ICommand SelectAutoCommand { get; private set; }
        public ICommand SelectInputCommand { get; private set; }
        private void MenuKindChanged(MenuKind kind)
        {
            switch (kind)
            {
                case MenuKind.Auto:
                    InputMenuVisibility.Value = Visibility.Collapsed;
                    AutoMenuVisibility.Value = Visibility.Visible;                    
                    SubTitleText.Value = "自動発行メニュー";
                    break;
                case MenuKind.Input:
                    AutoMenuVisibility.Value = Visibility.Collapsed;
                    InputMenuVisibility.Value = Visibility.Visible;                    
                    SubTitleText.Value = "手動発行メニュー";
                    break;
            }
        }
        private void ShowInputDisplay(string tcode)
        {
            if (!RegistryUtil.isInstalled("Multi LABELIST V5"))
            {
                MessageBox.Show("Multi LABELIST V5がインストールされていません。", "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var torihikisaki = Torihikisakis.list.FirstOrDefault(x => x.Tcode == tcode && x.Kind == HakkouKind.Input);
            if (torihikisaki != null)
            {
                switch (torihikisaki.Tcode)
                {
                    case Tid.AJU:

                        break;
                }
            }
        }
        private void ShowAutoDisplay(string tcode)
        {
            if (!RegistryUtil.isInstalled("Multi LABELIST V5"))
            {
                MessageBox.Show("Multi LABELIST V5がインストールされていません。", "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var torihikisaki = Torihikisakis.list.FirstOrDefault(x => x.Tcode == tcode && x.Kind != HakkouKind.Input);
            if(torihikisaki != null)
            {
                switch(torihikisaki.Tcode)
                {
                    case Tid.YASUSAKI:
                        {
                            var view = new YasusakiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.YAMANAKA:
                        {
                            var view = new YamanakaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }                        
                        break;
                    case Tid.MARUYOSI:
                        {
                            var view = new MaruyoshiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.OKINAWA_SANKI:
                        {
                            var view = new OkinawaSankiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.WATASEI:
                        {
                            var view = new WataseiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    default:break;
                }
            }
        }
    }
    
}
