using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class RelayCommand : ICommand
    {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public RelayCommand(Action action)
        {//コンストラクタでActionを登録
            _action = action;
        }

        #region ICommandインターフェースの必須実装

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {//とりあえずActionがあれば実行可能
            return _action != null;
        }

        public void Execute(object parameter)
        {//今回は引数を使わずActionを実行
            _action?.Invoke();
        }

        #endregion
    }

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
        public ReactiveProperty<int> HakkouTypeText { get; set; }
        public ReactiveProperty<ObservableCollection<HakkouType>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<HakkouType>>();
        public ReactiveProperty<int> SelectedHakkouTypeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 分類コード
        public ReactiveProperty<string> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 発注番号
        public ReactiveProperty<string> HachuBangou { get; set; } = new ReactiveProperty<string>("");

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; } 
        public ReactiveProperty<ObservableCollection<NefudaBangou>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<NefudaBangou>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 商品コード表示・非表示
        public ReactiveProperty<bool> HinEnabled { get; set; } = new ReactiveProperty<bool>(false);
        // 開始商品コード
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了商品コード
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");
        // 開始枝番
        public ReactiveProperty<string> SttEdaban { get; set; } = new ReactiveProperty<string>("");
        //終了枝番
        public ReactiveProperty<string> EndEdaban { get; set; } = new ReactiveProperty<string>("");

        private List<YasusakiData> YasusakiDatas { get; set; } = new List<YasusakiData>();
        public ReactiveProperty<ObservableCollection<YasusakiItem>> YasusakiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<YasusakiItem>>();
        public void Loaded()
        {
            Task.Run(() =>
            {

            });
        }

        #region コマンドの実装
        private RelayCommand<string> funcActionCommand;
        public RelayCommand<string> FuncActionCommand
        {
            get { return funcActionCommand = funcActionCommand ?? new RelayCommand<string>(FuncAction); }
        }

        private void FuncAction(string parameter)
        {
            switch (parameter)
            {
                case "ESC":

                    break;
                case "5":
                    if (InputCheck())
                    {
                        NefudaDataDisplay();
                    }                    
                    break;
            }
        }
        #endregion

        public YasusakiViewModel()
        {
            CreateComboItems();

            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("910");
            NefudaBangouText = new ReactiveProperty<int>(1);

            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));
        }

        private void HakkouTypeTextChanged(int id)
        {
            var item = HakkouTypeItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedHakkouTypeIndex.Value = HakkouTypeItems.Value.IndexOf(item);
            }
        }

        private void BunruiCodeTextChanged(string id)
        {
            var item = BunruiCodeItems.Value.FirstOrDefault(x => x.Id.TrimEnd() == id.TrimEnd());
            if (item != null)
            {
                SelectedBunruiCodeIndex.Value = BunruiCodeItems.Value.IndexOf(item);
            }
            else
            {
                SelectedBunruiCodeIndex.Value = 0;
                BunruiCodeText.Value = "";
            }
        }

        private void NefudaBangouTextChanged(int id)
        {
            var item = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedNefudaBangouIndex.Value = NefudaBangouItems.Value.IndexOf(item);
            }
        }

        private void SelectedHakkouTypeIndexChanged(int idx)
        {
            var item = HakkouTypeItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                HakkouTypeText.Value = item.Id;
            }
            else
            {
                HakkouTypeText.Value = 0;
            }
        }

        private void SelectedBunruiCodeIndexChanged(int idx)
        {
            var item = BunruiCodeItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                BunruiCodeText.Value = item.Id.TrimEnd();
            }
            else
            {
                BunruiCodeText.Value = "";
            }
        }

        private void SelectedNefudaBangouIndexChanged(int idx)
        {
            var item = NefudaBangouItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                NefudaBangouText.Value = item.Id;
            }
            else
            {
                NefudaBangouText.Value = 0;
            }
        }

        public bool InputCheck()
        {
            if(this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.HachuBangou.Value))
            {
                MessageBox.Show("発注番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if(this.NefudaBangouText.Value < 1 || this.NefudaBangouText.Value > 2)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void NefudaDataDisplay()
        {
            var w0112_EOS_HACHU = new DB_0112_EOS_HACHU_LIST();
            var w0112EosHchuList = w0112_EOS_HACHU.QueryWhereHno(this.HachuBangou.Value);

            var wWEB_TORIHIKISAKI_TANKA = new WEB_TORIHIKISAKI_TANKA_LIST();
            var wWebTorihikisakiTankaList = wWEB_TORIHIKISAKI_TANKA.QueryWhereTcodeTenpo("112", "9999");

            if(this.HakkouTypeText.Value == 2)
            {
                int sttHincd;
                int endHincd;
                int sttEdaban;
                int endEdaban;
                int scode;

                w0112EosHchuList = w0112EosHchuList.Where(x => (int.TryParse(this.SttHincd.Value, out sttHincd) && int.TryParse(x.SCODE, out scode) ? scode >= sttHincd : true) &&
                                            (int.TryParse(this.EndHincd.Value, out endHincd) && int.TryParse(x.SCODE, out scode) ? scode <= endHincd : true) &&
                                            (int.TryParse(this.SttEdaban.Value, out sttEdaban) ? x.SAIZUS >= sttEdaban : true) &&
                                            (int.TryParse(this.EndEdaban.Value, out endEdaban) ? x.SAIZUS <= endEdaban : true))
                                    .ToList();
            }
            
            if (w0112EosHchuList.Any() && wWebTorihikisakiTankaList.Any())
            {
                YasusakiDatas.Clear();
                YasusakiDatas.AddRange(
                    w0112EosHchuList.Where(x => x.NSU > 0 && x.BUNRUI == int.Parse(this.BunruiCodeText.Value))
                        .Join(
                               wWebTorihikisakiTankaList.Where(x => x.NEFUDA_KBN == this.NefudaBangouText.Value.ToString()),
                               e => new
                               {
                                   TOKCD = short.Parse(e.TOKCD),
                                   BUNRUI = short.Parse(e.BUNRUI.ToString()),
                                   SCODE = int.Parse(e.SCODE),
                                   SAIZUS = short.Parse(e.SAIZUS.ToString()),
                               },
                               w => new
                               {
                                   TOKCD = w.TCODE,
                                   BUNRUI = w.BUNRUI,
                                   SCODE = w.HCODE,
                                   SAIZUS = w.SAIZU
                               },
                               (eos, tanka) => new
                               {
                                   HNO = eos.HNO,
                                   TOKCD = eos.TOKCD,
                                   SYOHINCD = eos.SYOHINCD,
                                   JANCD = eos.JANCD,
                                   BUNRUI = eos.BUNRUI,
                                   SCODE = eos.SCODE,
                                   SAIZUS = eos.SAIZUS,
                                   HINCD = eos.HINCD,
                                   HATYUBI = eos.HATYUBI,
                                   NOUHINBI = eos.NOUHINBI,
                                   NSU = eos.NSU,
                                   BAIKA = eos.BAIKA,
                                   EOS_SYOHINNM = eos.EOS_SYOHINNM,
                                   GENKA = eos.GENKA,
                                   SKBN = tanka.SKBN,
                                   NEFUDA_KBN = tanka.NEFUDA_KBN,
                                   NETUKE_BUNRUI = tanka.NETUKE_BUNRUI,
                                   BIKOU1 = tanka.BIKOU1,
                                   BIKOU2 = tanka.BIKOU2
                               })
                         .GroupBy(a => new
                         {
                             a.HNO,
                             a.TOKCD,
                             a.SYOHINCD,
                             a.JANCD,
                             a.BUNRUI,
                             a.SCODE,
                             a.SAIZUS,
                             a.HINCD,
                             a.HATYUBI,
                             a.NOUHINBI,
                             a.BAIKA,
                             a.EOS_SYOHINNM,
                             a.GENKA,
                             a.SKBN,
                             a.NEFUDA_KBN,
                             a.NETUKE_BUNRUI,
                             a.BIKOU1,
                             a.BIKOU2
                         })
                         .Select(g => new YasusakiData
                         {
                             HNO = g.Key.HNO,
                             TOKCD = g.Key.TOKCD,
                             SYOHINCD = g.Key.SYOHINCD,
                             JANCD = g.Key.JANCD,
                             BUNRUI = g.Key.BUNRUI,
                             SCODE = g.Key.SCODE,
                             SAIZUS = g.Key.SAIZUS,
                             HINCD = g.Key.HINCD,
                             HATYUBI = g.Key.HATYUBI,
                             NOUHINBI = g.Key.NOUHINBI,
                             NSU = g.Sum(y => y.NSU),
                             BAIKA = g.Key.BAIKA,
                             EOS_SYOHINNM = g.Key.EOS_SYOHINNM,
                             GENKA = g.Key.GENKA,
                             SKBN = g.Key.SKBN,
                             NEFUDA_KBN = g.Key.NEFUDA_KBN,
                             NETUKE_BUNRUI = g.Key.NETUKE_BUNRUI,
                             BIKOU1 = g.Key.BIKOU1,
                             BIKOU2 = g.Key.BIKOU2
                         })
                     );

                if (YasusakiItems.Value == null)
                {
                    YasusakiItems.Value = new ObservableCollection<YasusakiItem>();
                }
                if (YasusakiDatas.Any())
                {
                    YasusakiItems.Value.Clear();
                    var yasusakiModelList = new YasusakiItemList();
                    YasusakiItems.Value = new ObservableCollection<YasusakiItem>(yasusakiModelList.ConvertYasusakiDataToModel(YasusakiDatas));
                }
            }
        }
        public void CreateComboItems()
        {
            var bunruis = new BunruiCodeList().GetBunruiCodes();
            bunruis.Insert(0, new BunruiCode("", ""));
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
    public class YasusakiItem
    {
        public int 発行枚数 { get; set; }
        public string 売切月 { get; set; }
        public string 品番 { get; set; }
        public string JAN { get; set; }
        public string 商品コード { get; set; }
        public string 商品名 { get; set; }
        public int 売価 { get; set; }
        public int 単価 { get; set; }
        public string 値札番号 { get; set; }
        public YasusakiItem(int 発行枚数, string 売切月, string 品番, string JAN, string 商品コード,
                             string 商品名, int 売価, int 単価, string 値札番号)
        {
            this.発行枚数 = 発行枚数;
            this.売切月 = 売切月;
            this.品番 = 品番;
            this.JAN = JAN;
            this.商品コード = 商品コード;
            this.商品名 = 商品名;
            this.売価 = 売価;
            this.単価 = 単価;
            this.値札番号 = 値札番号;
        }
    }

    public class YasusakiItemList
    {
        public IEnumerable<YasusakiItem> ConvertYasusakiDataToModel(List<YasusakiData> datas)
        {
            var result = new List<YasusakiItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new YasusakiItem(data.NSU, "41", data.SYOHINCD, data.JANCD, data.HINCD,
                            data.EOS_SYOHINNM, data.BAIKA, data.GENKA, data.NEFUDA_KBN));
            });
            return result;
        }
    }
}
