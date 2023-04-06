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
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class TakahashiViewModel : ViewModelsBase
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

        private List<TakahashiData> TakahashiDatas { get; set; } = new List<TakahashiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<TakahashiItem>> TakahashiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<TakahashiItem>>();

        #endregion

        private readonly string _grpName = @"\本店タカハシ";

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
        public TakahashiViewModel()
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
            item.Name = "1：◆貼札_税込併記(35×38)";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：◆吊り札_税込併記(35×38)";
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
                    if (dB_JYUCYU_LIST.QueryWhereTcodeHnoExists(TidNum.HONTENTAKAHASI, hno))
                    {
                        HnoResultString.Value = "登録済 " + Tid.HONTENTAKAHASI + "-" + Tnm.HONTENTAKAHASI;
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
            TakahashiDatas.Clear();
            if (TakahashiItems.Value != null && TakahashiItems.Value.Any())
            {
                TakahashiItems.Value.Clear();
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

                if (wJyucyuList.Any())
                {
                    TakahashiDatas.Clear();
                    TakahashiDatas.AddRange(
                        wJyucyuList
                            .GroupBy(j => new
                            {
                                TCODE = j.TCODE,
                                BUMON = j.BUMON.HasValue ? j.BUMON.ToString() : "",
                                JANCD = !string.IsNullOrEmpty(j.JANCD) ? j.JANCD.TrimEnd() : "",
                                BUNRUI = j.BUNRUI,
                                SCODE = j.SCODE.TrimEnd(),
                                SAIZUS = j.SAIZUS,
                                HINCD = string.Concat(j.BUNRUI, "-", j.SCODE.TrimEnd(), "-", j.SAIZUS.ToString("00")),
                                HINMEI = j.HINMEI.TrimEnd(),
                                SAIZUN = j.SAIZUN.TrimEnd(),
                                STANKA = j.STANKA,
                                HTANKA = j.HTANKA,
                                LOCTANA_SOKO_CODE = j.LOCTANA_SOKO_CODE,
                                LOCTANA_FLOOR_NO = j.LOCTANA_FLOOR_NO,
                                LOCTANA_TANA_NO = j.LOCTANA_TANA_NO,
                                LOCTANA_CASE_NO = j.LOCTANA_CASE_NO,
                            })
                             .Select(g => new TakahashiData
                             {
                                 TCODE = g.Key.TCODE,
                                 BUMON = g.Key.BUMON,
                                 JANCD = g.Key.JANCD,
                                 BUNRUI = g.Key.BUNRUI,
                                 SCODE = g.Key.SCODE,
                                 SAIZUS = g.Key.SAIZUS.ToString("00"),
                                 HINCD = g.Key.HINCD,
                                 HINMEI = g.Key.HINMEI,
                                 SAIZUN = g.Key.SAIZUN,
                                 STANKA = g.Key.STANKA,
                                 HTANKA = g.Key.HTANKA,
                                 LOCTANA_SOKO_CODE = g.Key.LOCTANA_SOKO_CODE.HasValue ? (int)g.Key.LOCTANA_SOKO_CODE : 0,
                                 LOCTANA_FLOOR_NO = g.Key.LOCTANA_FLOOR_NO.HasValue ? (int)g.Key.LOCTANA_FLOOR_NO : 0,
                                 LOCTANA_TANA_NO = g.Key.LOCTANA_TANA_NO.HasValue ? (int)g.Key.LOCTANA_TANA_NO : 0,
                                 LOCTANA_CASE_NO = g.Key.LOCTANA_CASE_NO.HasValue ? (int)g.Key.LOCTANA_CASE_NO : 0,
                                 TSU = g.Sum(y => y.HSU),
                             })
                             .Where(x => x.TSU > 0 &&
                                        (!string.IsNullOrEmpty(BunruiCodeText.Value) ? x.BUNRUI.ToString() == BunruiCodeText.Value : true))
                             .OrderBy(g => g.BUNRUI)
                             .ThenBy(g => g.LOCTANA_SOKO_CODE)
                             .ThenBy(g => g.LOCTANA_FLOOR_NO)
                             .ThenBy(g => g.LOCTANA_TANA_NO)
                             .ThenBy(g => g.LOCTANA_CASE_NO)
                             .ThenBy(g => g.SCODE)
                             .ThenBy(g => g.SAIZUS)
                         );

                    if (TakahashiDatas.Any())
                    {
                        TakahashiItems.Value = new ObservableCollection<TakahashiItem>();
                        var TakahashiModelList = new TakahashiItemList();
                        var addItems = new ObservableCollection<TakahashiItem>(TakahashiModelList.ConvertTakahashiDataToModel(TakahashiDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<TakahashiItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                      // 発行枚数に変更があったら合計発行枚数も変更する
                                      TotalMaisu.Value = TakahashiItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        TakahashiItems.Value = new ObservableCollection<TakahashiItem>(setItems);
                        TotalMaisu.Value = TakahashiItems.Value.Sum(x => x.発行枚数).ToString();
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
            return TakahashiItems.Value != null &&
                   TakahashiItems.Value.Any() &&
                   TakahashiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.HONTENTAKAHASI + "_" + this.HachuBangou.Value + ".csv";
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
            var list = TakahashiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "部門",
                "店番",
                "原価",
                "売価",
                "JANコード",
                "コメント",
                "商品コード",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用商品コード");
            datas.Columns.Remove("表示用商品名");
            datas.Columns.Remove("表示用サイズカラー名");
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
            var layName = NefudaBangouText.Value == 1 ?
                            @"00004-◆貼札_税込併記(35×38).mllayx" :
                            @"00008-◆吊り札_税込併記(35×38).mllayx";

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
    public class TakahashiItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private int _発行枚数;
        public int 発行枚数 // csv & 表示
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
        public string 部門 { get; set; } // csv1 & 表示 ※リストのクラス
        public string 店番 { get; set; }  // csv2 空白固定
        public int 原価 { get; set; } // csv3 & 表示
        public int 売価 { get; set; } // csv4 & 表示
        public string JANコード { get; set; }   // csv5 & 表示
        public string コメント { get; set; }   // csv6 空白固定
        public string 商品コード { get; set; }   // csv7
        public string 表示用商品コード { get; set; }    // 表示
        public string 表示用商品名 { get; set; } // 表示
        public string 表示用サイズカラー名 { get; set; } // 表示

        public TakahashiItem(int 発行枚数, string 部門, string 店番, int 原価, int 売価, string JANコード,
                             string コメント, string 商品コード, string 表示用商品コード, string 表示用商品名, string 表示用サイズカラー名)
        {
            this.発行枚数 = 発行枚数;
            this.部門 = 部門;
            this.店番 = 店番;
            this.原価 = 原価;
            this.売価 = 売価;
            this.JANコード = JANコード;
            this.コメント = コメント;
            this.商品コード = 商品コード;
            this.表示用商品コード = 表示用商品コード;
            this.表示用商品名 = 表示用商品名;
            this.表示用サイズカラー名 = 表示用サイズカラー名;
        }
    }

    public class TakahashiItemList
    {
        public IEnumerable<TakahashiItem> ConvertTakahashiDataToModel(List<TakahashiData> datas)
        {
            var result = new List<TakahashiItem>();
            string hincd = "";
            datas.ForEach(data =>
            {
                hincd = !string.IsNullOrEmpty(data.SCODE) && !string.IsNullOrEmpty(data.SCODE) ?
                        string.Concat(data.SCODE, "-", data.SAIZUS) :
                        !string.IsNullOrEmpty(data.SCODE) ? data.SCODE : "";
                result.Add(
                    new TakahashiItem(data.TSU, data.BUMON, "", data.STANKA, data.HTANKA, data.JANCD, 
                                      "", hincd, data.HINCD, data.HINMEI, data.SAIZUN));
            });
            return result;
        }
    }
}
