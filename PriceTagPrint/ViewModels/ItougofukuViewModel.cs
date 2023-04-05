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
using PriceTagPrint.Models;
using PriceTagPrint.Views;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModels
{
    public class ItougofukuViewModel : ViewModelsBase
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
                = new ReactiveProperty<int>(1);

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

        // 開始JANコード
        public ReactiveProperty<string> SttJancd { get; set; } = new ReactiveProperty<string>("");
        // 終了JANコード
        public ReactiveProperty<string> EndJancd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<ItougofukuData> ItougofukuDatas { get; set; } = new List<ItougofukuData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<ItougofukuItem>> ItougofukuItems { get; set; }
                = new ReactiveProperty<ObservableCollection<ItougofukuItem>>();

        #endregion

        private readonly string _grpName = @"\7705_イトウゴフク\2020年総額表示_V5_ST308R";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\7705_イトウゴフク\2020年総額表示_V5_ST308R";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        private DB_JYUCYU_LIST dB_JYUCYU_LIST;
        private DB_T05001_SHOHIN_DAICHO_LIST dB_T05001_SHOHIN_DAICHO;
        private DB_T05007_SHOHIN_TEKIYO_DAICHO_LIST dB_T05007_SHOHIN_TEKIYO_DAICHO;
        private DB_T05005_SHOHIN_BUNRUI2_DAICHO_LIST dB_T05005_SHOHIN_BUNRUI2_DAICHO;

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
        public ItougofukuViewModel()
        {
            dB_JYUCYU_LIST = new DB_JYUCYU_LIST();
            dB_T05001_SHOHIN_DAICHO = new DB_T05001_SHOHIN_DAICHO_LIST();
            dB_T05007_SHOHIN_TEKIYO_DAICHO = new DB_T05007_SHOHIN_TEKIYO_DAICHO_LIST();
            dB_T05005_SHOHIN_BUNRUI2_DAICHO = new DB_T05005_SHOHIN_BUNRUI2_DAICHO_LIST();
            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>(3);

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

            HachuBangou.Subscribe(x => HachuBangouTextChanged(x));
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
            var searchExtension = "*." + CommonStrings.INPUT_EXTENSION;
            var files = Directory.EnumerateFiles(_grpFullName, searchExtension);
            var list = new List<CommonIdName>();
            var id = 2;
            foreach (var file in files)
            {
                var fName = Path.GetFileNameWithoutExtension(file);
                var item = new CommonIdName();
                item.Id = id;
                item.Name = id + "：" + fName;
                list.Add(item);
                id++;
            }
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
                    if (dB_JYUCYU_LIST.QueryWhereTcodeHnoExists(TidNum.ITOGOFUKU, hno))
                    {
                        HnoResultString.Value = "登録済 " + Tid.ITOGOFUKU + "-" + Tnm.ITOGOFUKU;
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
                NefudaBangouText.Value = 3;
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
            SelectedNefudaBangouIndex.Value = 1;
            HinEnabled.Value = false;
            SttHincd.Value = "";
            EndHincd.Value = "";
            SttEdaban.Value = "";
            EndEdaban.Value = "";
            SttJancd.Value = "";
            EndJancd.Value = "";
            TotalMaisu.Value = "";
            ItougofukuDatas.Clear();
            if (ItougofukuItems.Value != null && ItougofukuItems.Value.Any())
            {
                ItougofukuItems.Value.Clear();
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
            if (string.IsNullOrEmpty(BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.NefudaBangouText.Value < 2 || this.NefudaBangouText.Value > 3)
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
                long sttJan;
                long endJan;
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

                if (!string.IsNullOrEmpty(SttJancd.Value) && !long.TryParse(SttJancd.Value, out sttJan))
                {
                    MessageBox.Show("開始JANコードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(EndJancd.Value) && !long.TryParse(EndJancd.Value, out endJan))
                {
                    MessageBox.Show("終了JANコードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttJancd.Value) && !string.IsNullOrEmpty(EndJancd.Value) &&
                    long.TryParse(SttJancd.Value, out sttJan) && long.TryParse(EndJancd.Value, out endJan) &&
                    sttJan > endJan)
                {
                    MessageBox.Show("JANコードの大小関係が逆転しています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    long sttJancd;
                    long endJancd;
                    int scode;
                    long jancd;

                    wJyucyuList = wJyucyuList.Where(x =>
                                                (int.TryParse(this.SttHincd.Value, out sttHincd) && int.TryParse(x.SCODE, out scode) ? scode >= sttHincd : true) &&
                                                (int.TryParse(this.EndHincd.Value, out endHincd) && int.TryParse(x.SCODE, out scode) ? scode <= endHincd : true) &&
                                                (int.TryParse(this.SttEdaban.Value, out sttEdaban) ? x.SAIZUS >= sttEdaban : true) &&
                                                (int.TryParse(this.EndEdaban.Value, out endEdaban) ? x.SAIZUS <= endEdaban : true) &&
                                                (long.TryParse(this.SttJancd.Value, out sttJancd) && long.TryParse(x.JANCD, out jancd) ? jancd >= sttJancd : true) &&
                                                (long.TryParse(this.EndJancd.Value, out endJancd) && long.TryParse(x.JANCD, out jancd) ? jancd <= endJancd : true))
                                        .ToList();
                }

                var sttJan = wJyucyuList.OrderBy(x => x.JANCD).FirstOrDefault()?.JANCD ?? "";
                var endJan = wJyucyuList.OrderByDescending(x => x.JANCD).FirstOrDefault()?.JANCD ?? "";
                var shohinDaicho = dB_T05001_SHOHIN_DAICHO.QueryWhereJancd(sttJan, endJan);
                
                if (wJyucyuList.Any() && shohinDaicho.Any())
                {
                    var zeiritsu = Zeiritsu.items.FirstOrDefault(x => x.SttDate <= DateTime.Today && DateTime.Today <= x.EndDate)?.Kakeritsu ?? 1;
                    var shohinTekiyoDicho = dB_T05007_SHOHIN_TEKIYO_DAICHO.QueryWhereAll();
                    var shohinBunrui2Dicho = dB_T05005_SHOHIN_BUNRUI2_DAICHO.QueryWhereAll();
                    ItougofukuDatas.Clear();
                    ItougofukuDatas.AddRange(
                        wJyucyuList
                            .GroupJoin(
                                   shohinDaicho,
                                   j => new
                                   {
                                       JANCD = j.JANCD.ToString().TrimEnd(),
                                   },
                                   s => new
                                   {
                                       JANCD = s.バーコード.TrimEnd(),
                                   },
                                   (juc, hin) => new
                                   {
                                       TCODE = juc.TCODE,
                                       NEFUDANO = hin.Any() ? hin.FirstOrDefault().値札No : 0,
                                       SIRESYU = hin.Any() ? hin.FirstOrDefault().仕入週.TrimEnd() : "",
                                       TSU = juc.TSU,
                                       TEKIYOCD3 = hin.Any() ? hin.FirstOrDefault()?.商品摘要コード3 ?? (short)0 : (short)0,
                                       HINBAN = hin.Any() ? hin.FirstOrDefault().品番.TrimEnd() : "",
                                       JANCD = juc.JANCD,
                                       STANKA = juc.STANKA,
                                       HTANKA = juc.HTANKA,
                                       JYODAI = juc.JYODAI,
                                       TEKIYOCD1 = hin.Any() ? hin.FirstOrDefault().商品摘要コード1 : 0,
                                       TEKIYOCD2 = hin.Any() ? hin.FirstOrDefault().商品摘要コード2 : 0,
                                       BUMONCD = hin.Any() ? hin.FirstOrDefault().グループコード : 0,
                                       BUNRUI2CD = hin.Any() ? hin.FirstOrDefault().分類2コード : 0,                                       
                                       HINNM = hin.Any() ? hin.FirstOrDefault().商品名.TrimEnd() : "",
                                       HNO = juc.HNO,
                                       LOCTANA_SOKO_CODE = juc.LOCTANA_SOKO_CODE,
                                       LOCTANA_FLOOR_NO = juc.LOCTANA_FLOOR_NO,
                                       LOCTANA_TANA_NO = juc.LOCTANA_TANA_NO,
                                       LOCTANA_CASE_NO = juc.LOCTANA_CASE_NO,
                                       BUNRUI = juc.BUNRUI,
                                       SCODE = juc.SCODE,
                                       SAIZUS = juc.SAIZUS,
                                       UNITCD = hin.Any() ? hin.FirstOrDefault()?.画像名1 ?? "" : "",
                                       SIRECD = hin.Any() ? hin.FirstOrDefault().仕入先コード : 0,
                                   })                            
                            .GroupJoin(
                                    shohinTekiyoDicho,
                                    a => new
                                    {
                                        TEKIYO = a.TEKIYOCD3,
                                    },
                                    t => new
                                    {
                                        TEKIYO = t.商品摘要コード,
                                    },
                                    (atbl, tekiyo) => new
                                    {
                                        TCODE = atbl.TCODE,
                                        NEFUDANO = atbl.NEFUDANO,
                                        SIRESYU = atbl.SIRESYU,
                                        TSU = atbl.TSU,
                                        TEKIYOCD3 = atbl.TEKIYOCD3,
                                        SHOHINTEKYONM = tekiyo.Any() ? tekiyo.FirstOrDefault().商品摘要名.TrimEnd() : "",
                                        HINBAN = atbl.HINBAN,
                                        JANCD = atbl.JANCD,
                                        STANKA = atbl.STANKA,
                                        HTANKA = atbl.HTANKA,
                                        ZTANKA = Math.Floor(atbl.HTANKA * zeiritsu),
                                        JYODAI = atbl.JYODAI,
                                        TEKIYOCD1 = atbl.TEKIYOCD1,
                                        TEKIYOCD2 = atbl.TEKIYOCD2,
                                        BUMONCD = atbl.BUMONCD,
                                        BUNRUI2CD = atbl.BUNRUI2CD,
                                        HINNM = atbl.HINNM,
                                        HNO = atbl.HNO,
                                        LOCTANA_SOKO_CODE = atbl.LOCTANA_SOKO_CODE,
                                        LOCTANA_FLOOR_NO = atbl.LOCTANA_FLOOR_NO,
                                        LOCTANA_TANA_NO = atbl.LOCTANA_TANA_NO,
                                        LOCTANA_CASE_NO = atbl.LOCTANA_CASE_NO,
                                        BUNRUI = atbl.BUNRUI,
                                        SCODE = atbl.SCODE,
                                        SAIZUS = atbl.SAIZUS,
                                        UNITCD = atbl.UNITCD,
                                        SIRECD = atbl.SIRECD,
                                    }
                            )
                            .GroupJoin(
                                    shohinBunrui2Dicho,
                                    a => new
                                    {
                                        BUNRUI2 = a.BUNRUI2CD.ToString() ?? "",
                                    },
                                    b => new
                                    {
                                        BUNRUI2 = b.商品分類2コード.ToString(),
                                    },
                                    (atbl, bunrui) => new
                                    {
                                        TCODE = atbl.TCODE,
                                        NEFUDANO = atbl.NEFUDANO,
                                        SIRESYU = atbl.SIRESYU,
                                        TSU = atbl.TSU,
                                        TEKIYOCD3 = atbl.TEKIYOCD3,
                                        SHOHINTEKYONM = atbl.SHOHINTEKYONM,
                                        HINBAN = atbl.HINBAN,
                                        JANCD = atbl.JANCD,
                                        STANKA = atbl.STANKA,
                                        HTANKA = atbl.HTANKA,
                                        ZTANKA = Math.Floor(atbl.HTANKA * zeiritsu),
                                        JYODAI = atbl.JYODAI,
                                        TEKIYOCD1 = atbl.TEKIYOCD1,
                                        TEKIYOCD2 = atbl.TEKIYOCD2,
                                        BUMONCD = atbl.BUMONCD,
                                        BUNRUI2CD = atbl.BUNRUI2CD,
                                        BUNRUI2NM = bunrui.Any() ? bunrui.FirstOrDefault().商品分類2名.TrimEnd() : "",
                                        HINNM = atbl.HINNM,
                                        HNO = atbl.HNO,
                                        LOCTANA_SOKO_CODE = atbl.LOCTANA_SOKO_CODE,
                                        LOCTANA_FLOOR_NO = atbl.LOCTANA_FLOOR_NO,
                                        LOCTANA_TANA_NO = atbl.LOCTANA_TANA_NO,
                                        LOCTANA_CASE_NO = atbl.LOCTANA_CASE_NO,
                                        BUNRUI = atbl.BUNRUI,
                                        SCODE = atbl.SCODE,
                                        SAIZUS = atbl.SAIZUS,
                                        UNITCD = atbl.UNITCD,
                                        SIRECD = atbl.SIRECD,
                                    }
                            )
                            .GroupBy(a => new
                            {
                                TCODE = a.TCODE,
                                NEFUDANO = a.NEFUDANO,
                                SIRESYU = a.SIRESYU,
                                TEKIYOCD3 = a.TEKIYOCD3,
                                SHOHINTEKYONM = a.SHOHINTEKYONM,
                                HINBAN = a.HINBAN,
                                JANCD = a.JANCD,
                                STANKA = a.STANKA,
                                HTANKA = a.HTANKA,
                                ZTANKA = a.ZTANKA,
                                JYODAI = a.JYODAI,
                                TEKIYOCD1 = a.TEKIYOCD1,
                                TEKIYOCD2 = a.TEKIYOCD2,
                                BUMONCD = a.BUMONCD,
                                BUNRUI2CD = a.BUNRUI2CD,
                                BUNRUI2NM = a.BUNRUI2NM,
                                HINNM = a.HINNM,
                                HNO = a.HNO,
                                LOCTANA_SOKO_CODE = a.LOCTANA_SOKO_CODE,
                                LOCTANA_FLOOR_NO = a.LOCTANA_FLOOR_NO,
                                LOCTANA_TANA_NO = a.LOCTANA_TANA_NO,
                                LOCTANA_CASE_NO = a.LOCTANA_CASE_NO,
                                BUNRUI = a.BUNRUI,
                                SCODE = a.SCODE,
                                SAIZUS = a.SAIZUS,
                                UNITCD = a.UNITCD,
                                SIRECD = a.SIRECD,
                            })
                        .Select(g => new ItougofukuData
                        {
                            TCODE = g.Key.TCODE,
                            NEFUDANO = g.Key.NEFUDANO ?? 0,
                            SIRESYU = g.Key.SIRESYU,
                            TEKIYOCD3 = g.Key.TEKIYOCD3,
                            SHOHINTEKYONM = g.Key.SHOHINTEKYONM,
                            HINBAN = g.Key.HINBAN,
                            JANCD = g.Key.JANCD,
                            STANKA = g.Key.STANKA,
                            HTANKA = g.Key.HTANKA,
                            ZTANKA = g.Key.ZTANKA,
                            JYODAI = g.Key.JYODAI ?? 0,
                            TEKIYOCD1 = g.Key.TEKIYOCD1 ?? 0,
                            TEKIYONM1 = shohinTekiyoDicho.FirstOrDefault(x => x.商品摘要コード == (g.Key.TEKIYOCD1 ?? 0))?.商品摘要名 ?? "",
                            TEKIYOCD2 = g.Key.TEKIYOCD2 ?? 0,
                            TEKIYONM2 = shohinTekiyoDicho.FirstOrDefault(x => x.商品摘要コード == (g.Key.TEKIYOCD2 ?? 0))?.商品摘要名 ?? "",
                            BUMONCD = g.Key.BUMONCD ?? 0,
                            BUNRUI2CD = g.Key.BUNRUI2CD ?? 0,
                            BUNRUI2NM = g.Key.BUNRUI2NM,
                            HINNM = g.Key.HINNM,
                            HNO = g.Key.HNO,                                 
                            LOCTANA_SOKO_CODE = g.Key.LOCTANA_SOKO_CODE.HasValue ? (int)g.Key.LOCTANA_SOKO_CODE : 0,
                            LOCTANA_FLOOR_NO = g.Key.LOCTANA_FLOOR_NO.HasValue ? (int)g.Key.LOCTANA_FLOOR_NO : 0,
                            LOCTANA_TANA_NO = g.Key.LOCTANA_TANA_NO.HasValue ? (int)g.Key.LOCTANA_TANA_NO : 0,
                            LOCTANA_CASE_NO = g.Key.LOCTANA_CASE_NO.HasValue ? (int)g.Key.LOCTANA_CASE_NO : 0,
                            BUNRUI = g.Key.BUNRUI,
                            SCODE = g.Key.SCODE,
                            SAIZUS = g.Key.SAIZUS,
                            UNITCD = g.Key.UNITCD,
                            SIRECD = g.Key?.SIRECD.ToString() ?? "",
                            TSU = g.Sum(y => y.TSU),
                        })
                        .Where(x => x.TSU > 0 &&
                                    x.NEFUDANO == NefudaBangouText.Value &&
                                    (!string.IsNullOrEmpty(BunruiCodeText.Value) ? x.BUNRUI.ToString() == BunruiCodeText.Value : true))
                        .OrderBy(g => g.BUNRUI)
                        .ThenBy(g => g.LOCTANA_SOKO_CODE)
                        .ThenBy(g => g.LOCTANA_FLOOR_NO)
                        .ThenBy(g => g.LOCTANA_TANA_NO)
                        .ThenBy(g => g.LOCTANA_CASE_NO)
                        .ThenBy(g => g.SCODE)
                        .ThenBy(g => g.SAIZUS)
                    );

                    if (ItougofukuDatas.Any())
                    {
                        ItougofukuItems.Value = new ObservableCollection<ItougofukuItem>();
                        var ItougofukuModelList = new ItougofukuItemList();
                        var addItems = new ObservableCollection<ItougofukuItem>(ItougofukuModelList.ConvertItougofukuDataToModel(ItougofukuDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<ItougofukuItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = ItougofukuItems.Value.Sum(x => x.数量計).ToString();
                                  });
                            setItems.Add(item);
                        });
                        ItougofukuItems.Value = new ObservableCollection<ItougofukuItem>(setItems);
                        TotalMaisu.Value = ItougofukuItems.Value.Sum(x => x.数量計).ToString();
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
            return ItougofukuItems.Value != null &&
                   ItougofukuItems.Value.Any() &&
                   ItougofukuItems.Value.Sum(x => x.数量計) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.ITOGOFUKU + "_" + this.HachuBangou.Value + ".csv";
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
            var list = ItougofukuItems.Value.Where(x => x.数量計 > 0).ToList();
            var csvColSort = new string[]
            {
                "帳票種別",
                "仕入先",
                "仕入先名",
                "商品コード",
                "発注日",
                "納入日",
                "販売開始",
                "メーカーコード",
                "クラス",
                "クラス名",
                "ユニット",
                "商品名",
                "サイズ",
                "カラー",
                "原単価",
                "売単価",
                "税込売価",
                "コメント",
                "商品区分",
                "シーズン",
                "数量計",
                "部門コード"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
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
            var names = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == NefudaBangouText.Value).Name.Split("：");
            var layName = (names.LastOrDefault() ?? "") + ".mllayx";

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

    public class ItougofukuItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string 帳票種別 { get; set; }
        public string 仕入先 { get; set; }
        public string 仕入先名 { get; set; }
        public string 商品コード { get; set; }
        public string 発注日 { get; set; }
        public string 納入日 { get; set; }
        public string 販売開始 { get; set; }
        public string メーカーコード { get; set; }
        public string クラス { get; set; }
        public string クラス名 { get; set; }
        public string ユニット { get; set; }
        public string 商品名 { get; set; }
        public string サイズ { get; set; }
        public string カラー { get; set; }
        public string 原単価 { get; set; }
        public string 売単価 { get; set; }
        public string 税込売価 { get; set; }
        public string コメント { get; set; }
        public string 商品区分 { get; set; }
        public string シーズン { get; set; }

        private int _数量計;
        public int 数量計
        {
            get { return _数量計; }
            set
            {
                if (value != this._数量計)
                {
                    this._数量計 = value;
                    this.OnPropertyChanged("数量計");
                }
            }
        }
        public string 部門コード { get; set; }

        public ItougofukuItem(string 帳票種別, string 仕入先, string 仕入先名, string 商品コード, string 発注日, string 納入日, string 販売開始, string メーカーコード,
                              string クラス, string クラス名, string ユニット, string 商品名, string サイズ, string カラー, string 原単価, string 売単価, string 税込売価,
                              string コメント, string 商品区分, string シーズン, string 数量計, string 部門コード)
        {
            int conv;
            this.帳票種別 = 帳票種別;
            this.仕入先 = 仕入先;
            this.仕入先名 = 仕入先名;
            this.商品コード = 商品コード;
            this.発注日 = 発注日;
            this.納入日 = 納入日;
            this.販売開始 = 販売開始;
            this.メーカーコード = メーカーコード;
            this.クラス = クラス;
            this.クラス名 = クラス名;
            this.ユニット = ユニット;
            this.商品名 = 商品名;
            this.サイズ = サイズ;
            this.カラー = カラー;
            this.原単価 = 原単価;
            this.売単価 = 売単価;
            this.税込売価 = 税込売価;
            this.コメント = コメント;
            this.商品区分 = 商品区分;
            this.シーズン = シーズン;
            this.数量計 = int.TryParse(数量計, out conv) ? conv : 0;
            this.部門コード = 部門コード;
        }
    }

    public class ItougofukuItemList
    {
        public IEnumerable<ItougofukuItem> ConvertItougofukuDataToModel(List<ItougofukuData> datas)
        {
            var result = new List<ItougofukuItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new ItougofukuItem("", data.SIRECD, "", data.JANCD, data.HDATE, data.NDATE, "", data.HINBAN,
                                    data.BUNRUI2CD.ToString(), data.BUNRUI2NM, data.UNITCD, data.HINNM, data.TEKIYONM1, data.TEKIYONM2,
                                    data.STANKA.ToString(), data.HTANKA.ToString(), data.ZTANKA.ToString(), "", "", "", data.TSU.ToString(), data.BUMONCD.ToString()));
            });
            return result;
        }
    }
}
