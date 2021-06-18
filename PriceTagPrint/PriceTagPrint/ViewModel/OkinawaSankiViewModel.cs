using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.WAGO2;
using PriceTagPrint.WAGO;
using Reactive.Bindings;
using PriceTagPrint.View;

namespace PriceTagPrint.ViewModel
{
    public class OkinawaSankiViewModel : ViewModelsBase
    {
        #region プロパティ
        // 発行区分
        public ReactiveProperty<int> HakkouTypeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedHakkouTypeIndex { get; set; }
                = new ReactiveProperty<int>(0);        

        // 発注番号
        public ReactiveProperty<string> HachuBangou { get; set; } = new ReactiveProperty<string>("");
        // 発注番号（存在結果情報）
        public ReactiveProperty<string> HnoResultString { get; set; } = new ReactiveProperty<string>("");
        // 発注番号（存在結果情報表示色） 
        public ReactiveProperty<Brush> HnoResultColor { get; set; } = new ReactiveProperty<Brush>(Brushes.Black);

        // センター
        public ReactiveProperty<string> CenterText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdNameStr>> CenterItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdNameStr>>();
        public ReactiveProperty<int> SelectedCenterIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 品番コード
        public ReactiveProperty<string> HinbanCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdNameStr>> HinbanCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdNameStr>>();
        public ReactiveProperty<int> SelectedHinbanCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
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

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<OkinawaSankiData> OkinawaSankiDatas { get; set; } = new List<OkinawaSankiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<OkinawaSankiItem>> OkinawaSankiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<OkinawaSankiItem>>();

        #endregion
        private int _TOKCD = 118;
        private string _TOKNM = "㈱三喜";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        private DB_0118_EOS_HACHU_LIST dB_0118_EOS_HACHU_LIST;
        private DB_0118_HACHUSYO_LIST dB_0118_HACHUSYO_LIST;
        private DB_0118_KAITUKESYO_LIST dB_0118_KAITUKESYO_LIST;
        private TOKMSTPF_LIST tOKMSTPF_LIST;
        private WEB_TORIHIKISAKI_TANKA_LIST wEB_TORIHIKISAKI_TANKA_LIST;

        List<DB_0118_EOS_HACHU> dB0118EosHachuList = new List<DB_0118_EOS_HACHU>();
        List<DB_0118_HACHUSYO> dB0118HchusyoList = new List<DB_0118_HACHUSYO>();
        List<DB_0118_KAITUKESYO> dB0118KaitukesyoList = new List<DB_0118_KAITUKESYO>();
        List<TOKMSTPF> tOKMSTPFs = new List<TOKMSTPF>();

        #region コマンドの実装
        private RelayCommand<string> funcActionCommand;
        public RelayCommand<string> FuncActionCommand
        {
            get { return funcActionCommand = funcActionCommand ?? new RelayCommand<string>(FuncAction); }
        }

        /// <summary>
        /// ファンクションキー処理
        /// </summary>
        /// <param name="parameter"></param>
        private void FuncAction(string parameter)
        {
            switch (parameter)
            {
                case "ESC":

                    break;
                case "F4":
                    Clear();
                    this.HakkouTypeTextBox.Focus();
                    this.HakkouTypeTextBox.SelectAll();
                    break;
                case "F5":
                    if (InputCheck())
                    {
                        NefudaDataDisplay();
                        this.HakkouTypeTextBox.Focus();
                        this.HakkouTypeTextBox.SelectAll();
                    }
                    break;
                case "F10":
                    if (!PrintCheck())
                    {
                        MessageBox.Show("対象データが存在しません。", "値札発行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        ExecPrint(true);
                        this.HakkouTypeTextBox.Focus();
                        this.HakkouTypeTextBox.SelectAll();
                    }
                    break;
                case "F12":
                    if (!PrintCheck())
                    {
                        MessageBox.Show("対象データが存在しません。", "値札発行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        ExecPrint(false);
                        this.HakkouTypeTextBox.Focus();
                        this.HakkouTypeTextBox.SelectAll();
                    }
                    break;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OkinawaSankiViewModel()
        {
            ProcessingSplash ps = new ProcessingSplash("起動中", () =>
            {
                dB_0118_EOS_HACHU_LIST = new DB_0118_EOS_HACHU_LIST();
                dB_0118_HACHUSYO_LIST = new DB_0118_HACHUSYO_LIST();
                dB_0118_KAITUKESYO_LIST = new DB_0118_KAITUKESYO_LIST();
                tOKMSTPF_LIST = new TOKMSTPF_LIST();
                wEB_TORIHIKISAKI_TANKA_LIST = new WEB_TORIHIKISAKI_TANKA_LIST();

                CreateComboItems();

                // コンボボックス初期値セット
                HakkouTypeText = new ReactiveProperty<int>(1);
                CenterText = new ReactiveProperty<string>("");
                HinbanCodeText = new ReactiveProperty<string>("");
                NefudaBangouText = new ReactiveProperty<int>(1);

                // SubScribe定義
                HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
                CenterText.Subscribe(x => CenterTextChanged(x));
                HinbanCodeText.Subscribe(x => HinbanCodeTextChanged(x));
                NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
                SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
                SelectedCenterIndex.Subscribe(x => SelectedCenterIndexChanged(x));
                SelectedHinbanCodeIndex.Subscribe(x => SelectedHinbanCodeIndexChanged(x));
                SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

                HachuBangou.Subscribe(x => HachuBangouTextChanged(x));

                SelectedCenterIndex.Value = CenterItems.Value.Select((item, index) => new { item, index }).FirstOrDefault(x => x.item.Id == "5").index;
            });
            //バックグラウンド処理が終わるまで表示して待つ
            ps.ShowDialog();

            if (ps.complete)
            {
                //処理が成功した
            }
            else
            {
                //処理が失敗した
            }
        }

        #endregion

        #region コントロール生成・変更

        /// <summary>
        /// コンボボックスItem生成
        /// </summary>
        public void CreateComboItems()
        {
            HakkouTypeItems.Value = new ObservableCollection<CommonIdName>(CreateHakkouTypeItems());
            CenterItems.Value = new ObservableCollection<CommonIdNameStr>(CreateCenterItems());
            HinbanCodeItems.Value = new ObservableCollection<CommonIdNameStr>(CreateHinbanCodeItems());
            NefudaBangouItems.Value = new ObservableCollection<CommonIdName>(CreateNefudaBangouItems());

            SelectedHakkouTypeIndex.Subscribe(x => HakkouTypeChanged(x));
        }

        /// <summary>
        /// 発行区分Items生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateHakkouTypeItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 1;
            item.Name = "1：ＥＯＳ発注";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：発注書";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 3;
            item3.Name = "3：買付書";
            list.Add(item3);
            return list;
        }

        /// <summary>
        /// センターItems生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdNameStr> CreateCenterItems()
        {
            var list = new List<CommonIdNameStr>();
            var dB0118Kubuns = new DB_0118_KUBUN_LIST().dB_0118_KUBUNs;
            dB0118Kubuns.Where(x => x.CALL_KEY == 4 && x.KBN_KEY1 != "0").ToList().ForEach(x =>
            {
                list.Add(new CommonIdNameStr()
                {
                    Id = x.KBN_KEY1,
                    Name = x.KBN_KEY1 + "：" + x.KBN_NAME2,
                });
            });
            list.Add(new CommonIdNameStr() { Id = "99", Name = "99：全て" });
            return list;
        }

        /// <summary>
        /// 品番コードItems生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdNameStr> CreateHinbanCodeItems()
        {
            var list = new List<CommonIdNameStr>();
            var dB0118Hinbans = new DB_0118_HINBAN_LIST().dB_0118_HINBANs.Select(x => new { HinbanCd = x.HINBANCD, HinbanNm = x.HINBANNM}).Distinct();
            
            dB0118Hinbans.ToList().ForEach(x =>
            {
                list.Add(new CommonIdNameStr()
                {
                    Id = x.HinbanCd,
                    Name = x.HinbanCd + "：" + x.HinbanNm,
                });
            });
            list.Insert(0, new CommonIdNameStr() { Id = "", Name = "" });
            return list;
        }

        /// <summary>
        /// 値札番号Items生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateNefudaBangouItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 1;
            item.Name = "1：ラベル";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：タグ";
            list.Add(item2);
            return list;
        }

        /// <summary>
        /// 発行区分変更処理
        /// </summary>
        /// <param name="index"></param>
        public void HakkouTypeChanged(int index)
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

        /// <summary>
        /// 発注番号テキスト変更処理
        /// </summary>
        /// <param name="hno"></param>
        private void HachuBangouTextChanged(string hno)
        {
            var tourokuFlg = false;
            string dbCentCd = "OC";
            if (!string.IsNullOrEmpty(hno))
            {
                int convTok;                               
                
                switch (this.HakkouTypeText.Value)
                {
                    case 1:
                        dB0118EosHachuList = dB_0118_EOS_HACHU_LIST.QueryWhereHno(hno);
                        if (dB0118EosHachuList.Any())
                        {
                            foreach(var item in dB0118EosHachuList)
                            {
                                dbCentCd = item.CENTCD;
                                if (item.CENTCD == "OC")
                                {
                                    _TOKCD = TidNum.OKINAWA_SANKI;
                                    break;
                                }
                                _TOKCD = int.TryParse(item.TOKCD, out convTok) ? convTok : TidNum.SANKI;
                            }

                            tOKMSTPFs = tOKMSTPF_LIST.QueryWhereTcodeTenpo(_TOKCD);
                            if (tOKMSTPFs.Any())
                            {
                                _TOKNM = tOKMSTPFs.FirstOrDefault()?.RYAKU ?? "";
                                tourokuFlg = true;
                            }
                            else
                            {
                                _TOKNM = "";
                                tourokuFlg = false;
                            }
                        }
                        else
                        {
                            tourokuFlg = false;
                        }
                        break;
                    case 2:
                        dB0118HchusyoList = dB_0118_HACHUSYO_LIST.QueryWhereHno(hno);
                        if (dB0118HchusyoList.Any())
                        {
                            foreach (var item in dB0118HchusyoList)
                            {
                                dbCentCd = item.CENTCD;
                                if (item.CENTCD == "OC")
                                {
                                    _TOKCD = TidNum.OKINAWA_SANKI;
                                    break;
                                }                                
                                _TOKCD = int.TryParse(item.TOKCD, out convTok) ? convTok : TidNum.SANKI;
                            }
                            tOKMSTPFs = tOKMSTPF_LIST.QueryWhereTcodeTenpo(_TOKCD);
                            if (tOKMSTPFs.Any())
                            {
                                _TOKNM = tOKMSTPFs.FirstOrDefault()?.RYAKU ?? "";
                                tourokuFlg = true;
                            }
                            else
                            {
                                _TOKNM = "";
                                tourokuFlg = false;
                            }
                        }
                        else
                        {
                            tourokuFlg = false;
                        }
                        break;
                    case 3:
                        dB0118KaitukesyoList = dB_0118_KAITUKESYO_LIST.QueryWhereHno(hno);
                        if (dB0118KaitukesyoList.Any())
                        {
                            foreach (var item in dB0118KaitukesyoList)
                            {
                                dbCentCd = item.CENTCD;
                                if (item.CENTCD == "OC")
                                {
                                    _TOKCD = TidNum.OKINAWA_SANKI;
                                    break;
                                }
                                _TOKCD = int.TryParse(item.TOKCD, out convTok) ? convTok : TidNum.SANKI;
                            }
                            tOKMSTPFs = tOKMSTPF_LIST.QueryWhereTcodeTenpo(_TOKCD);
                            if (tOKMSTPFs.Any())
                            {
                                _TOKNM = tOKMSTPFs.FirstOrDefault()?.RYAKU ?? "";
                                tourokuFlg = true;
                            }
                            else
                            {
                                _TOKNM = "";
                                tourokuFlg = false;
                            }
                        }
                        else
                        {
                            _TOKNM = "";
                            tourokuFlg = false;
                        }
                        break;
                }
            }
            else
            {
                HnoResultString.Value = "";
                HnoResultColor.Value = Brushes.Black;
                return;
            }

            if(tourokuFlg)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                SelectedCenterIndex.Value = CenterItems.Value.Select((item, index) => new { item, index })
                    .FirstOrDefault(x => Microsoft.VisualBasic.Strings.StrConv(x.item.Name, Microsoft.VisualBasic.VbStrConv.Narrow, 0x411).Contains(dbCentCd)).index;
                HnoResultString.Value = "登録済 " + _TOKCD.ToString("0000") + "-" + _TOKNM.TrimEnd();
                HnoResultColor.Value = Brushes.Blue;
            }
            else
            {
                _TOKCD = TidNum.SANKI;
                HnoResultString.Value = "※未登録";
                HnoResultColor.Value = Brushes.Red;
            }
        }

        /// <summary>
        /// 発行区分テキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void HakkouTypeTextChanged(int id)
        {
            var item = HakkouTypeItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedHakkouTypeIndex.Value = HakkouTypeItems.Value.IndexOf(item);
            }
        }

        /// <summary>
        /// センターテキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void CenterTextChanged(string id)
        {
            var item = CenterItems.Value.FirstOrDefault(x => x.Id.TrimEnd() == id.TrimEnd());
            if (item != null)
            {
                SelectedCenterIndex.Value = CenterItems.Value.IndexOf(item);
            }
            else
            {
                CenterText.Value = "5";
            }
        }

        /// <summary>
        /// 品番コードテキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void HinbanCodeTextChanged(string id)
        {
            var item = HinbanCodeItems.Value.FirstOrDefault(x => x.Id.TrimEnd() == id.TrimEnd().PadLeft(2, '0'));
            if (item != null)
            {
                SelectedHinbanCodeIndex.Value = HinbanCodeItems.Value.IndexOf(item);
            }
            else
            {
                SelectedHinbanCodeIndex.Value = 0;
                HinbanCodeText.Value = "";
            }
        }

        /// <summary>
        /// 値札番号テキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void NefudaBangouTextChanged(int id)
        {
            var item = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedNefudaBangouIndex.Value = NefudaBangouItems.Value.IndexOf(item);
            }
        }

        /// <summary>
        /// 発行区分コンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
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

        /// <summary>
        /// センターコンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
        private void SelectedCenterIndexChanged(int idx)
        {
            var item = CenterItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                CenterText.Value = item.Id.TrimEnd();
            }
            else
            {
                CenterText.Value = "";
            }
        }

        /// <summary>
        /// 品番コードコンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
        private void SelectedHinbanCodeIndexChanged(int idx)
        {
            var item = HinbanCodeItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                HinbanCodeText.Value = item.Id.TrimEnd();
            }
            else
            {
                HinbanCodeText.Value = "";
            }
        }

        /// <summary>
        /// 値札番号コンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
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

        #endregion

        #region ファンクション
        /// <summary>
        /// F4 初期化処理
        /// </summary>
        public void Clear()
        {
            SelectedHakkouTypeIndex.Value = 0;
            CenterText.Value = "";
            HinbanCodeText.Value = "";
            HachuBangou.Value = "";
            HnoResultString.Value = "";
            HnoResultColor.Value = Brushes.Black;
            SelectedNefudaBangouIndex.Value = 0;
            HinEnabled.Value = false;
            SttHincd.Value = "";
            EndHincd.Value = "";
            SttEdaban.Value = "";
            EndEdaban.Value = "";
            TotalMaisu.Value = "";
            OkinawaSankiDatas.Clear();
            if (OkinawaSankiItems.Value != null && OkinawaSankiItems.Value.Any())
            {
                OkinawaSankiItems.Value.Clear();
            }
            HakkouTypeTextBox.Focus();
        }

        /// <summary>
        /// F5検索入力チェック
        /// </summary>
        /// <returns></returns>
        public bool InputCheck()
        {
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 3)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.HachuBangou.Value))
            {
                MessageBox.Show("発注番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!string.IsNullOrEmpty(HnoResultString.Value) && HnoResultString.Value.Contains("未登録"))
            {
                MessageBox.Show("未登録の発注番号が選択されています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.CenterText.Value))
            {
                MessageBox.Show("センターを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.NefudaBangouText.Value < 1 || this.NefudaBangouText.Value > 2)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// F5検索処理
        /// </summary>
        public void NefudaDataDisplay()
        {
            ProcessingSplash ps = new ProcessingSplash("データ作成中...", () =>
            {
                var wWebTorihikisakiTankaList = wEB_TORIHIKISAKI_TANKA_LIST.QueryWhereTcodeTenpo(_TOKCD.ToString(), "9999");
                var selectCenter = CenterItems.Value.FirstOrDefault(x => x.Id == CenterText.Value)?.Name.Split("：").ElementAtOrDefault(1) ?? "";
                switch (HakkouTypeText.Value)
                {
                    case 1:
                        if (dB0118EosHachuList.Any() && tOKMSTPFs.Any() && wWebTorihikisakiTankaList.Any())
                        {
                            OkinawaSankiDatas.Clear();
                            OkinawaSankiDatas.AddRange(
                                dB0118EosHachuList.Where(x => x.NSU > 0)
                                    .Join(
                                           wWebTorihikisakiTankaList,
                                           e => new
                                           {
                                               SCODE = int.Parse(e.SCODE),
                                               BUNRUI = short.Parse(e.BUNRUI.ToString()),
                                               SAIZUS = short.Parse(e.SAIZUS.ToString()),
                                           },
                                           w => new
                                           {
                                               SCODE = w.HCODE,
                                               BUNRUI = w.BUNRUI,
                                               SAIZUS = w.SAIZU
                                           },
                                           (eos, tanka) => new
                                           {
                                               HNO = eos.HNO,
                                               TOKCD = eos.TOKCD,
                                               NEFUDA_KBN = string.IsNullOrEmpty(tanka.NEFUDA_KBN) ||
                                                            (!string.IsNullOrEmpty(tanka.NEFUDA_KBN) && tanka.NEFUDA_KBN != "2") ? "1" : tanka.NEFUDA_KBN,
                                               CENTCD = eos.CENTCD,
                                               HINBANCD = eos.HINBANCD,
                                               CYUBUNCD = eos.CYUBUNCD,
                                               BAIKA = eos.BAIKA,
                                               SYOHINCD = eos.SYOHINCD,
                                               BIKOU1 = tanka.BIKOU1,
                                               HINCD = eos.HINCD,
                                               NSU = eos.NSU,
                                           })
                                     .GroupBy(a => new
                                     {
                                         a.HNO,
                                         a.TOKCD,
                                         a.NEFUDA_KBN,
                                         a.CENTCD,
                                         a.HINBANCD,
                                         a.CYUBUNCD,
                                         a.BAIKA,
                                         a.SYOHINCD,
                                         a.BIKOU1,
                                         a.HINCD,
                                     })
                                     .Select(g => new OkinawaSankiData
                                     {
                                         HNO = g.Key.HNO,
                                         TOKCD = g.Key.TOKCD,
                                         NEFUDA_KBN = g.Key.NEFUDA_KBN,
                                         JIISYA = tOKMSTPFs.FirstOrDefault()?.JISYA ?? "",
                                         EOS = " ",
                                         CENTCD = g.Key.CENTCD,
                                         HINBANCD = g.Key.HINBANCD,
                                         CYUBUNCD = g.Key.CYUBUNCD,
                                         BAIKA = g.Key.BAIKA,
                                         SYOHINCD = g.Key.SYOHINCD,
                                         BIKOU1 = g.Key.BIKOU1,
                                         HINCD = g.Key.HINCD,
                                         NSU = g.Sum(y => y.NSU),
                                     })
                                     .Where(x => x.NSU > 0 &&
                                                 x.NEFUDA_KBN == this.NefudaBangouText.Value.ToString() &&
                                                 (!string.IsNullOrEmpty(CenterText.Value) && CenterText.Value != "99" ?
                                                    x.CENTCD == selectCenter : true) &&
                                                 (!string.IsNullOrEmpty(HinbanCodeText.Value) ?
                                                    x.HINBANCD == HinbanCodeText.Value : true))
                                     .OrderBy(g => g.HNO)
                                     .ThenBy(g => g.HINCD.Replace("-", ""))
                                 );
                            
                            if (OkinawaSankiDatas.Any())
                            {
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>();
                                var OkinawaSankiModelList = new OkinawaSankiItemList();
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>(OkinawaSankiModelList.ConvertOkinawaSankiDataToModel(OkinawaSankiDatas));
                                TotalMaisu.Value = OkinawaSankiItems.Value.Sum(x => x.発行枚数).ToString();
                            }
                            else
                            {
                                MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case 2:
                        if (dB0118HchusyoList.Any() && tOKMSTPFs.Any() && wWebTorihikisakiTankaList.Any())
                        {
                            OkinawaSankiDatas.Clear();
                            OkinawaSankiDatas.AddRange(
                                dB0118HchusyoList.Where(x => x.NSU > 0)
                                    .Join(
                                           wWebTorihikisakiTankaList,
                                           e => new
                                           {
                                               SCODE = int.Parse(e.SCODE),
                                               BUNRUI = short.Parse(e.BUNRUI.ToString()),
                                               SAIZUS = short.Parse(e.SAIZUS.ToString()),
                                           },
                                           w => new
                                           {
                                               SCODE = w.HCODE,
                                               BUNRUI = w.BUNRUI,
                                               SAIZUS = w.SAIZU
                                           },
                                           (eos, tanka) => new
                                           {
                                               HNO = eos.HNO,
                                               TOKCD = eos.TOKCD,
                                               NEFUDA_KBN = string.IsNullOrEmpty(tanka.NEFUDA_KBN) ||
                                                            (!string.IsNullOrEmpty(tanka.NEFUDA_KBN) && tanka.NEFUDA_KBN != "2") ? "1" : tanka.NEFUDA_KBN,
                                               CENTCD = eos.CENTCD,
                                               HINBANCD = eos.HINBANCD,
                                               CYUBUNCD = eos.CYUBUNCD,
                                               BAIKA = eos.BAIKA,
                                               SYOHINCD = eos.SYOHINCD,
                                               BIKOU1 = tanka.BIKOU1,
                                               HINCD = eos.HINCD,
                                               NSU = eos.NSU,
                                           })
                                     .GroupBy(a => new
                                     {
                                         a.HNO,
                                         a.TOKCD,
                                         a.NEFUDA_KBN,
                                         a.CENTCD,
                                         a.HINBANCD,
                                         a.CYUBUNCD,
                                         a.BAIKA,
                                         a.SYOHINCD,
                                         a.BIKOU1,
                                         a.HINCD,
                                     })
                                     .Select(g => new OkinawaSankiData
                                     {
                                         HNO = g.Key.HNO,
                                         TOKCD = g.Key.TOKCD,
                                         NEFUDA_KBN = g.Key.NEFUDA_KBN,
                                         JIISYA = tOKMSTPFs.FirstOrDefault()?.JISYA ?? "",
                                         EOS = " ",
                                         CENTCD = g.Key.CENTCD,
                                         HINBANCD = g.Key.HINBANCD,
                                         CYUBUNCD = g.Key.CYUBUNCD,
                                         BAIKA = g.Key.BAIKA,
                                         SYOHINCD = g.Key.SYOHINCD,
                                         BIKOU1 = g.Key.BIKOU1,
                                         HINCD = g.Key.HINCD,
                                         NSU = g.Sum(y => y.NSU),
                                     })
                                     .Where(x => x.NSU > 0 &&
                                                 x.NEFUDA_KBN == this.NefudaBangouText.Value.ToString() &&
                                                 (!string.IsNullOrEmpty(CenterText.Value) && CenterText.Value != "99" ?
                                                    x.CENTCD == selectCenter : true) &&
                                                 (!string.IsNullOrEmpty(HinbanCodeText.Value) ?
                                                    x.HINBANCD == HinbanCodeText.Value : true))
                                     .OrderBy(g => g.HNO)
                                     .ThenBy(g => g.HINCD.Replace("-", ""))
                                 );

                            if (OkinawaSankiDatas.Any())
                            {
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>();
                                var OkinawaSankiModelList = new OkinawaSankiItemList();
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>(OkinawaSankiModelList.ConvertOkinawaSankiDataToModel(OkinawaSankiDatas));
                                TotalMaisu.Value = OkinawaSankiItems.Value.Sum(x => x.発行枚数).ToString();
                            }
                            else
                            {
                                MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case 3:
                        if (dB0118KaitukesyoList.Any() && tOKMSTPFs.Any() && wWebTorihikisakiTankaList.Any())
                        {
                            OkinawaSankiDatas.Clear();
                            OkinawaSankiDatas.AddRange(
                                dB0118KaitukesyoList.Where(x => x.NSU > 0)
                                    .Select(kaituke => new
                                    {
                                        HNO = kaituke.HNO,
                                        TOKCD = kaituke.TOKCD,
                                        NEFUDA_KBN = "1",
                                        CENTCD = kaituke.CENTCD,
                                        HINBANCD = kaituke.HINBANCD,
                                        CYUBUNCD = kaituke.CYUBUNCD,
                                        BAIKA = kaituke.BAIKA,
                                        SYOHINCD = kaituke.SYOHINCD,
                                        BIKOU1 = " ",
                                        HINCD = " ",
                                        NSU = kaituke.NSU,
                                    })
                                     .GroupBy(a => new
                                     {
                                         a.HNO,
                                         a.TOKCD,
                                         a.NEFUDA_KBN,
                                         a.CENTCD,
                                         a.HINBANCD,
                                         a.CYUBUNCD,
                                         a.BAIKA,
                                         a.SYOHINCD,
                                         a.BIKOU1,
                                         a.HINCD,
                                     })
                                     .Select(g => new OkinawaSankiData
                                     {
                                         HNO = g.Key.HNO,
                                         TOKCD = g.Key.TOKCD,
                                         NEFUDA_KBN = g.Key.NEFUDA_KBN,
                                         JIISYA = tOKMSTPFs.FirstOrDefault()?.JISYA ?? "",
                                         EOS = " ",
                                         CENTCD = g.Key.CENTCD,
                                         HINBANCD = g.Key.HINBANCD,
                                         CYUBUNCD = g.Key.CYUBUNCD,
                                         BAIKA = g.Key.BAIKA,
                                         SYOHINCD = g.Key.SYOHINCD,
                                         BIKOU1 = g.Key.BIKOU1,
                                         HINCD = g.Key.HINCD,
                                         NSU = g.Sum(y => y.NSU),
                                     })
                                     .Where(x => x.NSU > 0 &&
                                                 x.NEFUDA_KBN == this.NefudaBangouText.Value.ToString() &&
                                                 (!string.IsNullOrEmpty(CenterText.Value) && CenterText.Value != "99" ?
                                                    x.CENTCD == selectCenter : true) &&
                                                 (!string.IsNullOrEmpty(HinbanCodeText.Value) ?
                                                    x.HINBANCD == HinbanCodeText.Value : true))
                                     .OrderBy(g => g.HNO)
                                     .ThenBy(g => g.SYOHINCD)
                                 );

                            if (OkinawaSankiDatas.Any())
                            {
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>();
                                var OkinawaSankiModelList = new OkinawaSankiItemList();
                                OkinawaSankiItems.Value = new ObservableCollection<OkinawaSankiItem>(OkinawaSankiModelList.ConvertOkinawaSankiDataToModel(OkinawaSankiDatas));
                                TotalMaisu.Value = OkinawaSankiItems.Value.Sum(x => x.発行枚数).ToString();
                            }
                            else
                            {
                                MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                }                
            });
            //バックグラウンド処理が終わるまで表示して待つ
            ps.ShowDialog();

            if (ps.complete)
            {
                //処理が成功した
            }
            else
            {
                //処理が失敗した
            }
        }

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return OkinawaSankiItems.Value != null &&
                   OkinawaSankiItems.Value.Any() &&
                   OkinawaSankiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.SANKI + "_" + this.HachuBangou.Value + ".csv";
            var fullName = Path.Combine(CommonStrings.CSV_PATH, fname);
            CsvExport(fullName);
            if (!File.Exists(fullName))
            {
                MessageBox.Show("CSVファイルのエクスポートに失敗しました。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NefudaOutput(fullName, isPreview);
        }

        /// <summary>
        /// F10プレビュー・F12印刷 CSV発行処理
        /// </summary>
        /// <param name="fullName"></param>
        private void CsvExport(string fullName)
        {
            var list = OkinawaSankiItems.Value.Where(x => x.発行枚数 > 0).OrderBy(x => x.JANフリー).ToList();
            var datas = DataUtility.ToDataTable(list);
            // 不要なカラムの削除
            datas.Columns.Remove("センター");
            new CsvUtility().Write(datas, fullName, true);
        }

        /// <summary>
        /// F10プレビュー・F12印刷 外部アプリ（MLV5）起動
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="isPreview"></param>
        private void NefudaOutput(string fname, bool isPreview)
        {
            // ※振分発行用ＰＧ
            var grpName = @"\0122_沖縄三喜\【総額対応】沖縄三喜_V5_RT308R_振分発行";
            var layName = @"00019-◆三喜（自動発行）.mldenx";
            var layNo = CommonStrings.MLV5LAYOUT_PATH + @"\" + grpName + @"\" + layName;
            var dq = "\"";
            var args = dq + layNo + dq + " /g " + dq + fname + dq + (isPreview ? " /p " : " /o ");

            //Processオブジェクトを作成する
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //起動する実行ファイルのパスを設定する
            p.StartInfo.FileName = CommonStrings.MLPRINTEXE_PATH;
            //コマンドライン引数を指定する
            p.StartInfo.Arguments = args;
            //起動する。プロセスが起動した時はTrueを返す。
            bool result = p.Start();
        }
        #endregion
    }

    /// <summary>
    /// データグリッド表示プロパティ
    /// CSVの出力にも流用
    /// </summary>
    public class OkinawaSankiItem
    {
        public int 発注No { get; set; }
        public string 取引先CD { get; set; }
        public string 値札No { get; set; }
        public string EOS { get; set; }
        public string 業者コード { get; set; }
        public string 部門 { get; set; }
        public string EOSコード2桁 { get; set; }
        public string EOS5 { get; set; }
        public int 販売価格 { get; set; }
        public string JANフリー { get; set; }
        public string メッセージ { get; set; }
        public string サイズ { get; set; }
        public string カラー { get; set; }
        public string 商品コード { get; set; }
        public int 発行枚数 { get; set; }
        public string センター { get; set; }

        public OkinawaSankiItem(int 発注No, string 取引先CD, string 値札No, string EOS, string 業者コード, string 部門,
                                string EOSコード2桁, string EOS5, int 販売価格, string JANフリー, string メッセージ,
                                string サイズ, string カラー, string 商品コード, int 発行枚数, string センター)
        {
            this.発注No = 発注No;
            this.取引先CD = 取引先CD;
            this.値札No = 値札No;
            this.EOS = EOS;
            this.業者コード = 業者コード;
            this.部門 = 部門;
            this.EOSコード2桁 = EOSコード2桁;
            this.EOS5 = EOS5;
            this.販売価格 = 販売価格;
            this.JANフリー = JANフリー;
            this.メッセージ = メッセージ;
            this.サイズ = サイズ;
            this.カラー = カラー;
            this.商品コード = 商品コード;
            this.発行枚数 = 発行枚数;
            this.センター = センター;
        }
    }

    public class OkinawaSankiItemList
    {
        public IEnumerable<OkinawaSankiItem> ConvertOkinawaSankiDataToModel(List<OkinawaSankiData> datas)
        {
            var result = new List<OkinawaSankiItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new OkinawaSankiItem(data.HNO, data.TOKCD, data.NEFUDA_KBN, data.EOS, data.JIISYA.TrimEnd(), data.HINBANCD + data.CYUBUNCD,
                                        "00", " ", data.BAIKA, data.SYOHINCD.TrimEnd(), data.BIKOU1, " ", " ", data.HINCD, data.NSU, data.CENTCD));
            });
            return result;
        }
    }
}
