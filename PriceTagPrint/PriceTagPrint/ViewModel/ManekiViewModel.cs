using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
using PriceTagPrint.View;
using PriceTagPrint.WAG_USR1;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class ManekiViewModel : ViewModelsBase
    {
        #region プロパティ
        // 発行区分
        public ReactiveProperty<int> HakkouTypeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
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
        // 発注番号（存在結果情報）
        public ReactiveProperty<string> HnoResultString { get; set; } = new ReactiveProperty<string>("");
        // 発注番号（存在結果情報表示色） 
        public ReactiveProperty<Brush> HnoResultColor { get; set; } = new ReactiveProperty<Brush>(Brushes.Black);

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

        // 開始SKU番号
        public ReactiveProperty<string> SttSkucd { get; set; } = new ReactiveProperty<string>("");
        // 終了SKU番号
        public ReactiveProperty<string> EndSkucd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<ManekiData> ManekiDatas { get; set; } = new List<ManekiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<ManekiItem>> ManekiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<ManekiItem>>();

        #endregion

        private readonly string _grpName = @"\2101_マネキ\【総額表示】_V5_ST308R";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        private DB_JYUCYU_LIST dB_JYUCYU_LIST;
        private DB_2101_JYUCYU_LIST dB_2101_JYUCYU_LIST;
        private HINMTA_LIST hINMTA_LIST;
        private List<HINMTA> hinmtaList;

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
        public ManekiViewModel()
        {
            dB_JYUCYU_LIST = new DB_JYUCYU_LIST();
            dB_2101_JYUCYU_LIST = new DB_2101_JYUCYU_LIST();
            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>(1);

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

            HachuBangou.Subscribe(x => HachuBangouTextChanged(x));

            ProcessingSplash ps = new ProcessingSplash("起動中", () =>
            {
                hINMTA_LIST = new HINMTA_LIST();
                hinmtaList = hINMTA_LIST.QueryWhereAll();
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
            var bunruis = new BunruiCodeList().GetBunruiCodes();
            bunruis.Insert(0, new BunruiCode("", ""));
            HakkouTypeItems.Value = new ObservableCollection<CommonIdName>(CreateHakkouTypeItems());
            BunruiCodeItems.Value = new ObservableCollection<BunruiCode>(bunruis);
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
            item.Name = "1：新規発行";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：再発行";
            list.Add(item2);
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
            item.Name = "1：シール無地（３７×４０）";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：タグ無地（３７×４０）";
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
            if (!string.IsNullOrEmpty(hno))
            {
                ProcessingSplash ps = new ProcessingSplash("発注番号確認中...", () =>
                {
                    if (dB_2101_JYUCYU_LIST.QueryWhereHnoExists(hno))
                    {
                        HnoResultString.Value = "登録済 " + Tid.MANEKI + "-" + Tnm.MANEKI;
                        HnoResultColor.Value = Brushes.Blue;
                    }
                    else
                    {
                        HnoResultString.Value = "※未登録";
                        HnoResultColor.Value = Brushes.Red;
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
            else
            {
                HnoResultString.Value = "";
                HnoResultColor.Value = Brushes.Black;
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
        /// 分類コードテキスト変更処理
        /// </summary>
        /// <param name="id"></param>
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
        /// 分類コードコンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
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
            BunruiCodeText.Value = "";
            HachuBangou.Value = "";
            HnoResultString.Value = "";
            HnoResultColor.Value = Brushes.Black;
            SelectedNefudaBangouIndex.Value = 0;
            HinEnabled.Value = false;
            SttHincd.Value = "";
            EndHincd.Value = "";
            SttEdaban.Value = "";
            EndEdaban.Value = "";
            SttSkucd.Value = "";
            EndSkucd.Value = "";
            TotalMaisu.Value = "";
            ManekiDatas.Clear();
            if (ManekiItems.Value != null && ManekiItems.Value.Any())
            {
                ManekiItems.Value.Clear();
            }
            HakkouTypeTextBox.Focus();
        }

        /// <summary>
        /// F5検索入力チェック
        /// </summary>
        /// <returns></returns>
        public bool InputCheck()
        {
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
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
            if (this.NefudaBangouText.Value < 1 || this.NefudaBangouText.Value > 2)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.HakkouTypeText.Value == 2)
            {
                int sttHin;
                int endHin;
                int sttEda;
                int endEda;
                int sttSku;
                int endSku;
                if (!string.IsNullOrEmpty(SttHincd.Value) && !int.TryParse(SttHincd.Value, out sttHin))
                {
                    MessageBox.Show("開始商品コードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(EndHincd.Value) && !int.TryParse(EndHincd.Value, out endHin))
                {
                    MessageBox.Show("終了商品コードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttEdaban.Value) && !int.TryParse(SttEdaban.Value, out sttEda))
                {
                    MessageBox.Show("開始商品枝番を正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(EndEdaban.Value) && !int.TryParse(EndEdaban.Value, out endEda))
                {
                    MessageBox.Show("終了商品枝番を正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttHincd.Value) && !string.IsNullOrEmpty(EndHincd.Value) &&
                    int.TryParse(SttHincd.Value, out sttHin) && int.TryParse(EndHincd.Value, out endHin) &&
                    sttHin > endHin)
                {
                    MessageBox.Show("商品コードの大小関係が逆転しています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttEdaban.Value) && !string.IsNullOrEmpty(EndEdaban.Value) &&
                    int.TryParse(SttEdaban.Value, out sttEda) && int.TryParse(EndEdaban.Value, out endEda) &&
                    sttEda > endEda)
                {
                    MessageBox.Show("商品枝番の大小関係が逆転しています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!string.IsNullOrEmpty(SttSkucd.Value) && !int.TryParse(SttSkucd.Value, out sttSku))
                {
                    MessageBox.Show("開始SKU番号を正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(EndSkucd.Value) && !int.TryParse(EndSkucd.Value, out endSku))
                {
                    MessageBox.Show("終了SKU番号を正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttSkucd.Value) && !string.IsNullOrEmpty(EndSkucd.Value) &&
                    int.TryParse(SttSkucd.Value, out sttSku) && int.TryParse(EndSkucd.Value, out endSku) &&
                    sttSku > endSku)
                {
                    MessageBox.Show("SKU番号の大小関係が逆転しています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
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
                var wJyucyuList = dB_JYUCYU_LIST.QueryWhereHno(this.HachuBangou.Value);

                if (wJyucyuList.Any() && this.HakkouTypeText.Value == 2)
                {
                    int sttHincd;
                    int endHincd;
                    int sttEdaban;
                    int endEdaban;
                    int sttSkucd;
                    int endSkucd;
                    int scode;

                    wJyucyuList = wJyucyuList.Where(x =>
                                                (int.TryParse(this.SttHincd.Value, out sttHincd) && int.TryParse(x.SCODE, out scode) ? scode >= sttHincd : true) &&
                                                (int.TryParse(this.EndHincd.Value, out endHincd) && int.TryParse(x.SCODE, out scode) ? scode <= endHincd : true) &&
                                                (int.TryParse(this.SttEdaban.Value, out sttEdaban) ? x.SAIZUS >= sttEdaban : true) &&
                                                (int.TryParse(this.EndEdaban.Value, out endEdaban) ? x.SAIZUS <= endEdaban : true) &&
                                                (int.TryParse(this.SttSkucd.Value, out sttSkucd) ? x.SKU >= sttSkucd : true) &&
                                                (int.TryParse(this.EndSkucd.Value, out endSkucd) ? x.SKU <= endSkucd : true))
                                        .ToList();
                }

                if (wJyucyuList.Any())
                {
                    ManekiDatas.Clear();
                    ManekiDatas.AddRange(
                        wJyucyuList
                            .GroupBy(j => new
                            {
                                TCODE = j.TCODE,
                                HNO = j.HNO,
                                SKU = j.SKU,
                                ITEMCD = j.ITEMCD,
                                TKBN = j.TKBN,
                                JTBLCD = j.JTBLCD,
                                SAIZU = j.SAIZU,
                                COLOR = j.COLOR,
                                BUMON = j.BUMON,
                                HENCD = j.HENCD,
                                JYODAI = j.JYODAI,
                                HTANKA = j.HTANKA,
                                HINMEIN = j.HINMEIN,
                                STANKA = j.STANKA,
                                LOCTANA_SOKO_CODE = j.LOCTANA_SOKO_CODE,
                                LOCTANA_FLOOR_NO = j.LOCTANA_FLOOR_NO,
                                LOCTANA_TANA_NO = j.LOCTANA_TANA_NO,
                                LOCTANA_CASE_NO = j.LOCTANA_CASE_NO,
                                BUNRUI = j.BUNRUI,
                                SCODE = j.SCODE.TrimEnd(),
                                SAIZUS = j.SAIZUS,
                                NEFUDA_KBN = NefudaBangouText.Value,
                            })
                             .Select(g => new ManekiData
                             {
                                 TCODE = g.Key.TCODE,
                                 HNO = g.Key.HNO,
                                 SKU = g.Key.SKU,
                                 ITEMCD = g.Key.ITEMCD,
                                 TKBN = g.Key.TKBN,
                                 JTBLCD = g.Key.JTBLCD?.ToString() ?? "",
                                 SAIZU = g.Key.SAIZU.HasValue ? (int)g.Key.SAIZU : 0,
                                 COLCD = g.Key.COLOR.HasValue ? (int)g.Key.COLOR : 0,
                                 BUMON = g.Key.BUMON,
                                 HENCD = g.Key.HENCD,
                                 JYODAI = g.Key.JYODAI,
                                 HTANKA = g.Key.HTANKA,
                                 HINMEIN = g.Key.HINMEIN,
                                 STANKA = g.Key.STANKA,
                                 LOCTANA_SOKO_CODE = g.Key.LOCTANA_SOKO_CODE.HasValue ? (int)g.Key.LOCTANA_SOKO_CODE : 0,
                                 LOCTANA_FLOOR_NO = g.Key.LOCTANA_FLOOR_NO.HasValue ? (int)g.Key.LOCTANA_FLOOR_NO : 0,
                                 LOCTANA_TANA_NO = g.Key.LOCTANA_TANA_NO.HasValue ? (int)g.Key.LOCTANA_TANA_NO : 0,
                                 LOCTANA_CASE_NO = g.Key.LOCTANA_CASE_NO.HasValue ? (int)g.Key.LOCTANA_CASE_NO : 0,
                                 BUNRUI = g.Key.BUNRUI,
                                 SCODE = g.Key.SCODE.TrimEnd(),
                                 SAIZUS = g.Key.SAIZUS.ToString("00"),
                                 NEFUDA_KBN = g.Key.NEFUDA_KBN,
                                 TSU = g.Sum(y => y.TSU),
                             })
                             .Where(x => !string.IsNullOrEmpty(BunruiCodeText.Value) ? x.BUNRUI.ToString() == BunruiCodeText.Value : true)
                             .OrderBy(g => g.BUNRUI)
                             .ThenBy(g => g.LOCTANA_SOKO_CODE)
                             .ThenBy(g => g.LOCTANA_FLOOR_NO)
                             .ThenBy(g => g.LOCTANA_TANA_NO)
                             .ThenBy(g => g.LOCTANA_CASE_NO)
                             .ThenBy(g => g.SCODE)
                             .ThenBy(g => g.SAIZUS)
                         );

                    if (ManekiDatas.Any())
                    {
                        var skuSortedItems = ManekiDatas.OrderBy(x => x.SKU).ToList();
                        var sttSku = skuSortedItems.FirstOrDefault()?.SKU ?? 0;
                        var endSku = skuSortedItems.LastOrDefault()?.SKU ?? 0;
                        var manekiJuchuList = dB_2101_JYUCYU_LIST.QueryWhereHnoSkuBetween(HachuBangou.Value, sttSku.ToString(), endSku.ToString());

                        ManekiDatas.ForEach(m =>
                        {
                            var mJuchu = manekiJuchuList.Where(x => x.SKU == m.SKU);
                            var mTsu = mJuchu.Sum(x => x.TSU);
                            var mJtbl = mJuchu.FirstOrDefault()?.JTBLCD.ToString() ?? "";
                            // マネキ用のJYUCYUテーブルの発行枚数と条件テーブルで更新
                            if (mTsu != m.TSU)
                            {
                                m.TSU = mTsu;
                            }
                            if (mJtbl != m.JTBLCD)
                            {
                                m.JTBLCD = mJtbl;
                            }
                            // グンゼ値付不要商品の発行枚数を0に更新
                            var hinban = m.BUNRUI + "-" + m.SCODE;
                            hinban += !string.IsNullOrEmpty(m.SAIZUS) ? "-" + m.SAIZUS.PadLeft(2, '0') : "";
                            if (hinmtaList.Any(h => h.HINCD.TrimEnd() == hinban && h.HINTKSID != "00"))
                            {
                                m.TSU = 0;
                            }
                        });

                        ManekiItems.Value = new ObservableCollection<ManekiItem>();
                        var ManekiModelList = new ManekiItemList();
                        var addItems = new ObservableCollection<ManekiItem>(ManekiModelList.ConvertManekiDataToModel(ManekiDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<ManekiItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = ManekiItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        ManekiItems.Value = new ObservableCollection<ManekiItem>(setItems);
                        TotalMaisu.Value = ManekiItems.Value.Sum(x => x.発行枚数).ToString();
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
            return ManekiItems.Value != null &&
                   ManekiItems.Value.Any() &&
                   ManekiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.MANEKI + "_" + this.HachuBangou.Value + ".csv";
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
            var list = ManekiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "納品月",
                "上中下旬CD",
                "アイテムCD",
                "定番区分",
                "メーカー品番",
                "条件テーブル",
                "サイズ",
                "カラー",
                "部門CD",
                "参考上代",
                "クラスCD",
                "管理番号",
                "下代変換CD",
                "上代",
                "BER上代",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("下代");
            datas.Columns.Remove("商品名");
            datas.Columns.Remove("値札No");
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
            var layName = NefudaBangouText.Value == 1 ? @"00010-シール３７×４０_総額.mllayx" : "00009-タグ３７×４０_総額.mllayx";

            var layNo = CommonStrings.MLV5LAYOUT_PATH + @"\" + _grpName + @"\" + layName;
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
    public class ManekiItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string 納品月 { get; set; }    // CSV
        public string 上中下旬CD { get; set; } // CSV
        public int アイテムCD { get; set; }    // CSV
        public string 定番区分 { get; set; }    // CSV
        public string メーカー品番 { get; set; }    // CSV
        public string 条件テーブル { get; set; }  // CSV 条件ﾃｰﾌﾞﾙ名
        public string サイズ { get; set; } // CSV ｻｲｽﾞ名
        public string カラー { get; set; } // CSV ｶﾗｰCD
        public int 部門CD { get; set; }  // CSV 部門CD
        public int 参考上代 { get; set; } // CSV 参考上代
        public string クラスCD { get; set; } // CSV ｸﾗｽCD
        public int 管理番号 { get; set; }  // CSV
        public int 下代変換CD { get; set; }  // CSV        
        public int 上代 { get; set; } // CSV 上代
        public int BER上代 { get; set; }  // CSV BER上代

        private int _発行枚数;
        public int 発行枚数 // CSV
        {
            get { return _発行枚数; }
            set
            {
                if (value != this._発行枚数)
                {
                    this._発行枚数 = value;
                    this.OnPropertyChanged("発行枚数");
                }
            }
        }
        public int 下代 { get; set; }        
        public string 商品名 { get; set; }
        public int 値札No { get; set; }


        public ManekiItem(int SKU番号, string 商品CD, string 商品名, string サイズ, string カラー, int 下代, int 参考上代, int 上代,
                          int BER上代, int 部門, string クラス, int アイテム, int 下代変換CD, string 定番区分, string 条件テーブル,
                          int 値札No, string 納品月, string 上中下旬CD, int 発行枚数)
        {
            this.管理番号 = SKU番号;
            this.メーカー品番 = 商品CD;
            this.商品名 = 商品名;
            this.サイズ = サイズ;
            this.カラー = カラー;
            this.下代 = 下代;
            this.参考上代 = 参考上代;
            this.上代 = 上代;
            this.BER上代 = BER上代;
            this.部門CD = 部門;
            this.クラスCD = クラス;
            this.アイテムCD = アイテム;
            this.下代変換CD = 下代変換CD;
            this.定番区分 = 定番区分;
            this.条件テーブル = 条件テーブル;
            this.値札No = 値札No;
            this.納品月 = 納品月;
            this.上中下旬CD = 上中下旬CD;
            this.発行枚数 = 発行枚数;
        }
    }

    public class ManekiItemList
    {
        public IEnumerable<ManekiItem> ConvertManekiDataToModel(List<ManekiData> datas)
        {
            var result = new List<ManekiItem>();
            var hinban = "";
            var size = "";
            var color = "";
            int shika = 0;
            var classcd = "";
            var nMonth = DateTime.Today.Month.ToString("00");
            datas.ForEach(data =>
            {
                hinban = data.BUNRUI + "-" + data.SCODE;
                hinban += !string.IsNullOrEmpty(data.SAIZUS) ? "-" + data.SAIZUS.PadLeft(2, '0') : "";
                size = data.SAIZU.ToString("000");
                color = data.COLCD.ToString("000");
                shika = data.JYODAI > 0 && data.HTANKA != data.JYODAI ? data.JYODAI : data.HTANKA;
                var item = data.ITEMCD.ToString();
                classcd = !string.IsNullOrEmpty(item) && item.Length >= 2 ? item.Substring(0, 2) : "";
                result.Add(
                    new ManekiItem(data.SKU, hinban, data.HINMEIN, size, color, data.STANKA, shika, data.HTANKA, data.HTANKA,
                                   data.BUMON, classcd, data.ITEMCD, data.HENCD, "*", data.JTBLCD, data.NEFUDA_KBN, nMonth,
                                   "0", data.TSU));
            });
            return result;
        }
    }
}
