using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.View;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PriceTagPrint.Model.MainWindowModel;

namespace PriceTagPrint.ViewModel
{
    public enum MenuKind { Auto, Ryohan, Ippan, Other}
    class MainWindowViewModel : ViewModelsBase
    {
        public ReactiveProperty<MenuKind> SubMenuKind { get; set; } = new ReactiveProperty<MenuKind>(MenuKind.Auto);

        public ReactiveProperty<string> SubTitleText { get; set; } = new ReactiveProperty<string>("自動発行メニュー");

        public ReactiveProperty<Visibility> AutoMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Visible);

        public ReactiveProperty<Visibility> RyohanMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Hidden);

        public ReactiveProperty<Visibility> IppanMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Hidden);

        public ReactiveProperty<Visibility> OtherMenuVisibility { get; set; } = new ReactiveProperty<Visibility>(Visibility.Hidden);

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
                    RyohanMenuVisibility.Value = Visibility.Collapsed;
                    IppanMenuVisibility.Value = Visibility.Collapsed;
                    OtherMenuVisibility.Value = Visibility.Collapsed;
                    AutoMenuVisibility.Value = Visibility.Visible;
                    
                    SubTitleText.Value = "自 動 発 行 メ ニ ュ ー";
                    break;
                case MenuKind.Ryohan:
                    AutoMenuVisibility.Value = Visibility.Collapsed;
                    IppanMenuVisibility.Value = Visibility.Collapsed;
                    OtherMenuVisibility.Value = Visibility.Collapsed;
                    RyohanMenuVisibility.Value = Visibility.Visible;
                    
                    SubTitleText.Value = "量 販 店 手 動 発 行 メ ニ ュ ー";
                    break;
                case MenuKind.Ippan:
                    AutoMenuVisibility.Value = Visibility.Collapsed;
                    RyohanMenuVisibility.Value = Visibility.Collapsed;
                    OtherMenuVisibility.Value = Visibility.Collapsed;
                    IppanMenuVisibility.Value = Visibility.Visible;
                    
                    SubTitleText.Value = "一 般 店 手 動 発 行 メ ニ ュ ー";
                    break;
                case MenuKind.Other:
                    AutoMenuVisibility.Value = Visibility.Collapsed;
                    RyohanMenuVisibility.Value = Visibility.Collapsed;
                    IppanMenuVisibility.Value = Visibility.Collapsed;
                    OtherMenuVisibility.Value = Visibility.Visible;

                    SubTitleText.Value = "そ の 他 手 動 発 行 メ ニ ュ ー";
                    break;
            }
        }
        private void ShowInputDisplay(string tcode)
        {
            //if (!RegistryUtil.isInstalled("Multi LABELIST V5"))
            //{
            //    MessageBox.Show("Multi LABELIST V5がインストールされていません。", "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            var torihikisaki = Torihikisakis.list.FirstOrDefault(x => x.Tcode == tcode && x.Kind != HakkouKind.Auto);
            if (torihikisaki != null)
            {
                var view = new LayoutSelectView(torihikisaki);
                view.Owner = window;
                view.Show();
                view.Owner.Hide();
            }
        }

        private void ShowAutoDisplay(string tcode)
        {
            DirectoryInfo di = new DirectoryInfo(CommonStrings.CSV_PATH);
            FileAttributes fas = File.GetAttributes(CommonStrings.CSV_PATH);

            if (!di.Exists)
            {
                string msg = string.Format("CSV出力パス：{0}が存在しません。作成してください。", CommonStrings.CSV_PATH);
                MessageBox.Show(msg, "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if((fas & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                string msg = string.Format("CSV出力パス：{0}が読み取り専用になっています。属性を変更してください。", CommonStrings.CSV_PATH);
                MessageBox.Show(msg, "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //if (!RegistryUtil.isInstalled("Multi LABELIST V5"))
            //{
            //    MessageBox.Show("Multi LABELIST V5がインストールされていません。", "起動時チェック", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

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
                    case Tid.KYOEI:
                        {
                            var view = new KyoueiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.ITOGOFUKU:
                        {
                            var view = new ItougofukuView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.MANEKI:
                        {
                            var view = new ManekiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.HOKKAIDO_SANKI:
                        {
                            var view = new HokaidoSankiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.TENMAYA:
                        {
                            var view = new TenmayaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.SANEI:
                        {
                            var view = new SaneiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.COSMOMATUOKA:
                        {
                            var view = new CosmoMatsuokaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.FUJIYA:
                        {
                            var view = new FujiyaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.MIYAMA:
                        {
                            var view = new MiyamaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.HONTENTAKAHASI:
                        {
                            var view = new TakahashiView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.AKANOREN:
                        {
                            var view = new AkanorenView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.OKADA:
                        {
                            var view = new OkadaView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.KEIOSTORE:
                        {
                            var view = new KeiostoreView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.NANKOKU:
                        {
                            var view = new NankokuView();
                            view.Owner = window;
                            view.Show();
                            view.Owner.Hide();
                        }
                        break;
                    case Tid.IZUMI:
                        {
                            var view = new IzumiView();
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
