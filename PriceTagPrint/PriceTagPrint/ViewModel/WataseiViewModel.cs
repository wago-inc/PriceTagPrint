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
using PriceTagPrint.View;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class WataseiViewModel : ViewModelsBase
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

        // 開始JANコード
        public ReactiveProperty<string> SttJancd { get; set; } = new ReactiveProperty<string>("");
        // 終了JANコード
        public ReactiveProperty<string> EndJancd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<WataseiData> WataseiDatas { get; set; } = new List<WataseiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<WataseiItem>> WataseiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<WataseiItem>>();

        #endregion

        private readonly string _grpName = @"\7858_わたせい\【総額対応】わたせい_V5_RT308R";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        private DB_JYUCYU_LIST dB_JYUCYU_LIST;

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
                    break;
                case "F5":
                    if (InputCheck())
                    {
                        NefudaDataDisplay();
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
                    }
                    break;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WataseiViewModel()
        {
            dB_JYUCYU_LIST = new DB_JYUCYU_LIST();
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
            item.Name = "1：ラベル（３８×３５）";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：タグ（３８×３５）";
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
                    if (dB_JYUCYU_LIST.QueryWhereTcodeHnoExists(TidNum.WATASEI, hno))
                    {
                        HnoResultString.Value = "登録済 " + Tid.WATASEI + "-" + Tnm.WATASEI;
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
            SttJancd.Value = "";
            EndJancd.Value = "";
            TotalMaisu.Value = "";
            WataseiDatas.Clear();
            if (WataseiItems.Value != null && WataseiItems.Value.Any())
            {
                WataseiItems.Value.Clear();
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
            if(this.HakkouTypeText.Value == 2)
            {
                int sttHin;
                int endHin;
                int sttEda;
                int endEda;
                int sttJan;
                int endJan;
                if(!string.IsNullOrEmpty(SttHincd.Value) && !int.TryParse(SttHincd.Value, out sttHin))
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

                if (!string.IsNullOrEmpty(SttJancd.Value) && !int.TryParse(SttJancd.Value, out sttJan))
                {
                    MessageBox.Show("開始JANコードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(EndJancd.Value) && !int.TryParse(EndJancd.Value, out endJan))
                {
                    MessageBox.Show("終了JANコードを正しく入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!string.IsNullOrEmpty(SttJancd.Value) && !string.IsNullOrEmpty(EndJancd.Value) &&
                    int.TryParse(SttJancd.Value, out sttJan) && int.TryParse(EndJancd.Value, out endJan) &&
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

                if (wJyucyuList.Any())
                {
                    WataseiDatas.Clear();
                    WataseiDatas.AddRange(
                        wJyucyuList
                            .GroupBy(j => new
                            {
                                TCODE = j.TCODE,
                                NEFUDA_KBN = j.NEFUDA_KBN,
                                HNO = j.HNO,
                                BUNRUI = j.BUNRUI,
                                LOCTANA_SOKO_CODE = j.LOCTANA_SOKO_CODE,
                                LOCTANA_FLOOR_NO = j.LOCTANA_FLOOR_NO,
                                LOCTANA_TANA_NO = j.LOCTANA_TANA_NO,
                                LOCTANA_CASE_NO = j.LOCTANA_CASE_NO,
                                SCODE = j.SCODE.TrimEnd(),
                                SAIZUS = j.SAIZUS,
                                HINCD = string.Concat(j.BUNRUI, "-", j.SCODE.TrimEnd().PadLeft(5, '0'), "-", j.SAIZUS.ToString("00")),
                                JANCD = !string.IsNullOrEmpty(j.JANCD) ? j.JANCD.TrimEnd() : "",
                                HINMEI = j.HINMEI.TrimEnd(),
                                SAIZUN = j.SAIZUN.TrimEnd(),
                                HTANKA = j.HTANKA,
                                JYODAI = j.JYODAI,
                                BUMON = j.BUMON,
                            })
                             .Select(g => new WataseiData
                             {
                                 TCODE = g.Key.TCODE,
                                 NEFUDA_KBN = g.Key.NEFUDA_KBN,
                                 HNO = g.Key.HNO,
                                 BUNRUI = g.Key.BUNRUI,
                                 LOCTANA_SOKO_CODE = g.Key.LOCTANA_SOKO_CODE.HasValue ? (int)g.Key.LOCTANA_SOKO_CODE : 0,
                                 LOCTANA_FLOOR_NO = g.Key.LOCTANA_FLOOR_NO.HasValue ? (int)g.Key.LOCTANA_FLOOR_NO : 0,
                                 LOCTANA_TANA_NO = g.Key.LOCTANA_TANA_NO.HasValue ? (int)g.Key.LOCTANA_TANA_NO : 0,
                                 LOCTANA_CASE_NO = g.Key.LOCTANA_CASE_NO.HasValue ? (int)g.Key.LOCTANA_CASE_NO : 0,
                                 SCODE = g.Key.SCODE,
                                 SAIZUS = g.Key.SAIZUS.ToString("00"),
                                 HINCD = g.Key.HINCD,
                                 JANCD = g.Key.JANCD,
                                 HINMEI = g.Key.HINMEI,
                                 SAIZUN = g.Key.SAIZUN,
                                 HTANKA = g.Key.HTANKA,
                                 JYODAI = g.Key.JYODAI,
                                 BUMON = g.Key.BUMON,
                                 TSU = g.Sum(y => y.TSU),
                             })
                             .Where(x => x.NEFUDA_KBN == NefudaBangouText.Value &&
                                         (!string.IsNullOrEmpty(BunruiCodeText.Value) ? x.BUNRUI.ToString() == BunruiCodeText.Value : true))
                             .OrderBy(g => g.BUNRUI)
                             .ThenBy(g => g.LOCTANA_SOKO_CODE)
                             .ThenBy(g => g.LOCTANA_FLOOR_NO)
                             .ThenBy(g => g.LOCTANA_TANA_NO)
                             .ThenBy(g => g.LOCTANA_CASE_NO)
                             .ThenBy(g => g.SCODE)
                             .ThenBy(g => g.SAIZUS)
                         );

                    if (WataseiDatas.Any())
                    {
                        WataseiItems.Value = new ObservableCollection<WataseiItem>();
                        var wataseiModelList = new WataseiItemList();
                        WataseiItems.Value = new ObservableCollection<WataseiItem>(wataseiModelList.ConvertWataseiDataToModel(WataseiDatas));
                        TotalMaisu.Value = WataseiItems.Value.Sum(x => x.発行枚数).ToString();
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
                this.HakkouTypeTextBox.Focus();
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
            return WataseiItems.Value != null &&
                   WataseiItems.Value.Any() &&
                   WataseiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.WATASEI + "_" + this.HachuBangou.Value + ".csv";
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
            var list = WataseiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var datas = DataUtility.ToDataTable(list);
            // 不要なカラムの削除
            datas.Columns.Remove("発注No");
            datas.Columns.Remove("取引先CD");
            datas.Columns.Remove("値札No");
            datas.Columns.Remove("商品コード");
            datas.Columns.Remove("JANコード");
            datas.Columns.Remove("市価");
            datas.Columns.Remove("サイズ名");
            datas.Columns.Remove("カラー名");
            datas.Columns.Remove("商品名");
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
            var layName = NefudaBangouText.Value == 1 ? @"貼り札.mllayx" : "下札.mllayx";

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
    public class WataseiItem
    {
        public int 発注No { get; set; }
        public string 取引先CD { get; set; }
        public string 値札No { get; set; }
        public string 商品コード { get; set; }
        public string JANコード { get; set; }
        public int 市価 { get; set; }        
        public string サイズ名 { get; set; }
        public string カラー名 { get; set; }
        public string 商品名 { get; set; }
        public string 部門 { get; set; }  // CSV
        public int 分類 { get; set; } // CSV
        public string 品番 { get; set; }  // CSV
        public string サイズ { get; set; }    // CSV
        public int 本体価格 { get; set; }  // CSV
        public int 発行枚数 { get; set; }   // CSV

        public WataseiItem(int 発注No, string 取引先CD, string 値札No, string 商品コード, string JANコード, int 市価,
                           int 売価, string サイズ名, string カラー名, string 商品名, string 部門, int 分類, string 品番,
                           string サイズ, int 発行枚数)
        {
            this.発注No = 発注No;
            this.取引先CD = 取引先CD;
            this.値札No = 値札No;
            this.商品コード = 商品コード;
            this.JANコード = JANコード;
            this.市価 = 市価;
            this.本体価格 = 売価;
            this.サイズ名 = サイズ名;
            this.カラー名 = カラー名;
            this.商品名 = 商品名;
            this.部門 = 部門;
            this.分類 = 分類;
            this.品番 = 品番;
            this.サイズ = サイズ;
            this.発行枚数 = 発行枚数;
        }
    }

    public class WataseiItemList
    {
        public IEnumerable<WataseiItem> ConvertWataseiDataToModel(List<WataseiData> datas)
        {
            var result = new List<WataseiItem>();
            int shika = 0;
            int bunrui = 0;
            datas.ForEach(data =>
            {
                shika = data.HTANKA == data.JYODAI || data.HTANKA > data.JYODAI ? 0 : data.JYODAI;
                bunrui = data.BUNRUI == 148 ? 918 : data.BUNRUI;
                result.Add(
                    new WataseiItem(data.HNO, data.TCODE.ToString(), data.NEFUDA_KBN.ToString(), data.HINCD, data.JANCD, shika, data.HTANKA, data.SAIZUN,
                                    " ", data.HINMEI, data.BUMON.ToString("000"), bunrui, data.SCODE, data.SAIZUS, data.TSU));
            });
            return result;
        }
    }
}
