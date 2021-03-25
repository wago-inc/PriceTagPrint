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
using static PriceTagPrint.Model.MainWindowModel;

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

        private List<YasusakiData> YasusakiDatas { get; set; } = new List<YasusakiData>();

        private ObservableCollection<YasusakiModel> YasusakiItems { get; set; } = new ObservableCollection<YasusakiModel>();
        public void Loaded()
        {
            Task.Run(() =>
            {

            });
        }

        public YasusakiViewModel()
        {
            this.FuncCommand = new DelegateCommand<string>(FuncAction);
        }
        public ICommand FuncCommand { get; private set; }
        private void FuncAction(string funcId)
        {
            switch (funcId)
            {
                case "5":
                    NefudaDataDisplay();
                    break;
            }
        }

        private void NefudaDataDisplay()
        {
            var w0112_EOS_HACHU = new DB_0112_EOS_HACHU_LIST();
            var w0112EosHchuList = w0112_EOS_HACHU.QueryWhereHno("228585");

            var wWEB_TORIHIKISAKI_TANKA = new WEB_TORIHIKISAKI_TANKA_LIST();
            var wWebTorihikisakiTankaList = wWEB_TORIHIKISAKI_TANKA.QueryWhereTcodeTenpo("112", "9999");

            if (w0112EosHchuList.Any() && wWebTorihikisakiTankaList.Any())
            {
                YasusakiDatas.Clear();
                YasusakiDatas.AddRange(
                    w0112EosHchuList.Where(x => x.NSU > 0 && x.BUNRUI == 910)
                        .Join(
                               wWebTorihikisakiTankaList.Where(x => x.NEFUDA_KBN == "2"),
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

                if (YasusakiItems == null)
                {
                    YasusakiItems = new ObservableCollection<YasusakiModel>();
                }
                if (YasusakiDatas.Any())
                {
                    YasusakiItems.Clear();
                    var yasusakiModelList = new YasusakiModelList();
                    YasusakiItems = new ObservableCollection<YasusakiModel>(yasusakiModelList.ConvertYasusakiDataToModel(YasusakiDatas));
                }
            }
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
