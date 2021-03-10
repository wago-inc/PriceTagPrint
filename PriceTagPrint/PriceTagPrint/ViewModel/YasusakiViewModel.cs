using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class HakkouType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NefudaBangou
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class YasusakiViewModel : ViewModelsBase
    {
        // 発行区分
        public ReactiveProperty<ObservableCollection<HakkouType>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<HakkouType>>();
        public ReactiveProperty<int> SelectedHakkouTypeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 分類コード
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 発注番号
        public ReactiveProperty<string> HachuBangou { get; set; } = new ReactiveProperty<string>("");

        // 値札番号
        public ReactiveProperty<ObservableCollection<NefudaBangou>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<NefudaBangou>>();        
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 商品コード表示・非表示
        public ReactiveProperty<bool> HinEnabled { get; set; } = new ReactiveProperty<bool>(false);

        public void Loaded()
        {
            Task.Run(() =>
            {
                
            });
        }

        public YasusakiViewModel()
        {
            
        }

        public void CreateComboItems()
        {
            var bunruis = new BunruiCodeList().GetBunruiCodes();

            HakkouTypeItems.Value = new ObservableCollection<HakkouType>(GetHakkouTypeItems());            
            BunruiCodeItems.Value = new ObservableCollection<BunruiCode>(bunruis);            
            NefudaBangouItems.Value = new ObservableCollection<NefudaBangou>(GetNefudaBangouItems());

            SelectedHakkouTypeIndex.Subscribe(x => BunruiChanged(x));
        }

        public void BunruiChanged(int index)
        {
            switch (index)
            {
                case 0:
                    HinEnabled.Value = false;
                    break;
                case 1:
                    HinEnabled.Value = true;
                    break;
                default:
                    break;
            }
        }

        public List<HakkouType> GetHakkouTypeItems()
        {
            var list = new List<HakkouType>();
            var item = new HakkouType();
            item.Id = 1;
            item.Name = "1：新規発行";
            list.Add(item);
            var item2 = new HakkouType();
            item2.Id = 2;
            item2.Name = "2：再発行";
            list.Add(item2);
            return list;
        }

        public List<NefudaBangou> GetNefudaBangouItems()
        {
            var list = new List<NefudaBangou>();
            var item = new NefudaBangou();
            item.Id = 1;
            item.Name = "1：ラベル";
            list.Add(item);
            var item2 = new NefudaBangou();
            item2.Id = 2;
            item2.Name = "2：タグ";
            list.Add(item2);
            return list;
        }
    }
}
