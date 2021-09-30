using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.View;
using PriceTagPrint.WAG_USR1;
using PriceTagPrint.WAGO;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PriceTagPrint.ViewModel
{
    public class FujiyaViewModel : ViewModelsBase
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

        // 受信日
        public ReactiveProperty<DateTime> JusinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today);

        // 分類コード
        public ReactiveProperty<string> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 季節コード
        public ReactiveProperty<string> ShikibetsuNoText { get; set; }

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 開始相手品番
        public ReactiveProperty<string> SttScode { get; set; } = new ReactiveProperty<string>("");
        // 終了相手品番
        public ReactiveProperty<string> EndScode { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<FujiyaData> FujiyaDatas { get; set; } = new List<FujiyaData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<FujiyaItem>> FujiyaItems { get; set; }
                = new ReactiveProperty<ObservableCollection<FujiyaItem>>();

        #endregion

        private readonly string _grpName = @"\フジヤ【総額表示】";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public TextBox ShikibetsuNoTextBox = null;
        public DatePicker JusinDatePicker = null;

        private LOC_LOCTANA_LIST lOCTANA_LIST;
        private DB_JYUCYU_LIST dB_JYUCYU_LIST;
        private FujiyaSyuuSuList syuuSuList;

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
        public FujiyaViewModel()
        {
            lOCTANA_LIST = new LOC_LOCTANA_LIST();
            dB_JYUCYU_LIST = new DB_JYUCYU_LIST();
            syuuSuList = new FujiyaSyuuSuList();
            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>();
            ShikibetsuNoText = new ReactiveProperty<string>();

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            HachuBangou.Subscribe(x => HachuBangouTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));

            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));
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
            var item1 = new CommonIdName();
            item1.Id = 1;
            item1.Name = "1：21号貼札";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：11号吊札";
            list.Add(item2);
            return list;
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
        /// 発注番号テキスト変更処理
        /// </summary>
        /// <param name="hno"></param>
        private void HachuBangouTextChanged(string hno)
        {
            DateTime convDate;
            if (this.JusinDatePicker != null && !string.IsNullOrEmpty(this.JusinDatePicker.Text) && DateTime.TryParse(this.JusinDatePicker.Text, out convDate))
            {
                if (!string.IsNullOrEmpty(hno))
                {
                    ProcessingSplash ps = new ProcessingSplash("発注番号確認中...", () =>
                    {
                        if (dB_JYUCYU_LIST.QueryWhereHnoJdateTcodeTenpoExists(TidNum.ZENSHOREN, TidNum.FUJIYA, convDate, hno))
                        {
                            HnoResultString.Value = "登録済 " + Tid.FUJIYA + "-" + Tnm.FUJIYA;
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
            else
            {
                if (this.JusinDatePicker != null)
                {
                    MessageBox.Show("受注日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    HnoResultString.Value = "";
                    HnoResultColor.Value = Brushes.Black;
                    this.JusinDatePicker.Focus();
                }
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
            else
            {
                NefudaBangouText.Value = 1;
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
                NefudaBangouText.Value = -1;
            }
        }

        #endregion

        #region ファンクション
        /// <summary>
        /// F4 初期化処理
        /// </summary>
        public void Clear()
        {
            JusinDate.Value = DateTime.Today;
            SelectedHakkouTypeIndex.Value = 0;
            ShikibetsuNoText.Value = "";
            BunruiCodeText.Value = "";
            HachuBangou.Value = "";
            HnoResultString.Value = "";
            HnoResultColor.Value = Brushes.Black;
            SelectedNefudaBangouIndex.Value = 0;
            SttScode.Value = "";
            EndScode.Value = "";
            TotalMaisu.Value = "";
            FujiyaDatas.Clear();
            if (FujiyaItems.Value != null && FujiyaItems.Value.Any())
            {
                FujiyaItems.Value.Clear();
            }
            HakkouTypeTextBox.Focus();
        }

        /// <summary>
        /// F5検索入力チェック
        /// </summary>
        /// <returns></returns>
        public bool InputCheck()
        {
            int sttHin;
            int endHin;
            DateTime convDate;
            if (string.IsNullOrEmpty(this.JusinDatePicker.Text) || !DateTime.TryParse(this.JusinDatePicker.Text, out convDate))
            {
                MessageBox.Show("受注日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.JusinDatePicker.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.ShikibetsuNoText.Value))
            {
                MessageBox.Show("識別番号を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);

                this.ShikibetsuNoTextBox.Focus();
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
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.HakkouTypeTextBox.Focus();
                return false;
            }
            if (!string.IsNullOrEmpty(SttScode.Value) && !string.IsNullOrEmpty(EndScode.Value) &&
                    int.TryParse(SttScode.Value, out sttHin) && int.TryParse(EndScode.Value, out endHin) &&
                    sttHin > endHin)
            {
                MessageBox.Show("品番の大小関係が逆転しています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                var wJyucyuList = dB_JYUCYU_LIST.QueryWhereHno(this.HachuBangou.Value);
                var wLoctana = lOCTANA_LIST.QueryWhereWagoAllLocation();
                if (wJyucyuList.Any())
                {
                    int sttScode;
                    int endScode;
                    int dbScode;
                    long bunrui;

                    wJyucyuList = wJyucyuList.Where(x =>
                                                (!string.IsNullOrEmpty(this.SttScode.Value) ?
                                                    int.TryParse(this.SttScode.Value, out sttScode) && int.TryParse(x.SCODE, out dbScode) ?
                                                        dbScode >= sttScode :
                                                    true :
                                                 true) &&
                                                (!string.IsNullOrEmpty(this.EndScode.Value) ?
                                                    int.TryParse(this.EndScode.Value, out endScode) && int.TryParse(x.SCODE, out dbScode) ? dbScode <= endScode :
                                                    true :
                                                 true) &&
                                                (!string.IsNullOrEmpty(this.BunruiCodeText.Value) ?
                                                    long.TryParse(this.BunruiCodeText.Value, out bunrui) ? x.BUNRUI == bunrui :
                                                    true :
                                                 true))
                                        .ToList();

                    if (wJyucyuList.Any())
                    {
                        FujiyaDatas.Clear();
                        FujiyaDatas.AddRange(
                            wJyucyuList
                                .GroupJoin(
                                wLoctana,
                                e1 => new
                                {
                                    hincd = e1.SCODE.TrimEnd(),
                                    sizecol = e1.SAIZUS
                                },
                                e2 => new
                                {
                                    hincd = e2.LOCTANA_SYOHIN_CODE.ToString(),
                                    sizecol = e2.LOCTANA_SIZECOLOR_CODE
                                },
                                (juc, loc) => new
                                {
                                    TSU = juc.TSU,
                                    SHIKIBETSU = this.ShikibetsuNoText.Value,
                                    TORIHIKISAKICD = "886",
                                    BUMONCD = !string.IsNullOrEmpty(juc.NETUKE_BUNRUI) && juc.NETUKE_BUNRUI.Contains("-") ?
                                                juc.NETUKE_BUNRUI.TrimEnd().Split("-").FirstOrDefault() ?? "" : "",
                                    CHUBUNRUI = !string.IsNullOrEmpty(juc.NETUKE_BUNRUI) && juc.NETUKE_BUNRUI.Contains("-") ?
                                                    juc.NETUKE_BUNRUI.TrimEnd().Split("-").LastOrDefault() ?? "" : "",
                                    SIRESYU = syuuSuList.GetFujimaSyuuSuByDate(JusinDate.Value)?.週数 ?? "",
                                    AITE_HINBAN = juc.JANCD.TrimEnd(),
                                    HINCD = !string.IsNullOrEmpty(juc.SCODEP) ? juc.SCODEP.TrimEnd() :
                                                juc.BUNRUI + "-" + juc.SCODE.TrimEnd() + "-" + juc.SAIZUS.ToString("00"),
                                    BUNRUI = juc.BUNRUI,
                                    HINBAN = juc.SCODE.TrimEnd(),
                                    EDABAN = juc.SAIZU?.ToString("00") ?? "",
                                    HINNMA = juc.HINMEI,
                                    STANKA = juc.STANKA,
                                    HTANKA = juc.HTANKA,
                                    ZBAIKA = juc?.ZBAIKA ?? 0,
                                    LOCTANA_SOKO_CODE = loc.Any() ? loc.FirstOrDefault().LOCTANA_SOKO_CODE : juc.LOCTANA_SOKO_CODE,
                                    LOCTANA_FLOOR_NO = loc.Any() ? loc.FirstOrDefault().LOCTANA_FLOOR_NO : juc.LOCTANA_FLOOR_NO,
                                    LOCTANA_TANA_NO = loc.Any() ? loc.FirstOrDefault().LOCTANA_TANA_NO : juc.LOCTANA_TANA_NO,
                                    LOCTANA_CASE_NO = loc.Any() ? loc.FirstOrDefault().LOCTANA_CASE_NO : juc.LOCTANA_CASE_NO,
                                })
                                .GroupBy(j => new
                                {
                                    TSU = j.TSU,
                                    SHIKIBETSU = this.ShikibetsuNoText.Value,
                                    TORIHIKISAKICD = "886",
                                    BUMONCD = j.BUMONCD,
                                    CHUBUNRUI = j.CHUBUNRUI,
                                    SIRESYU = j.SIRESYU,
                                    AITE_HINBAN = j.AITE_HINBAN,
                                    HINCD = j.HINCD,
                                    BUNRUI = j.BUNRUI,
                                    HINBAN = j.HINBAN,
                                    EDABAN = j.EDABAN,
                                    HINNMA = j.HINNMA,
                                    STANKA = j.STANKA,
                                    HTANKA = j.HTANKA,
                                    ZBAIKA = j.ZBAIKA,
                                    LOCTANA_SOKO_CODE = j.LOCTANA_SOKO_CODE,
                                    LOCTANA_FLOOR_NO = j.LOCTANA_FLOOR_NO,
                                    LOCTANA_TANA_NO = j.LOCTANA_TANA_NO,
                                    LOCTANA_CASE_NO = j.LOCTANA_CASE_NO,
                                })
                                 .Select(g => new FujiyaData
                                 {
                                     SHIKIBETSU = g.Key.SHIKIBETSU,
                                     TORIHIKICD = g.Key.TORIHIKISAKICD,
                                     BUMONCD = g.Key.BUMONCD,
                                     CHUBUNRUI = g.Key.CHUBUNRUI,
                                     SIRESYU = g.Key.SIRESYU,
                                     AITE_HINBAN = g.Key.AITE_HINBAN,
                                     HINCD = g.Key.HINCD,
                                     HINBAN = g.Key.HINBAN,
                                     EDABAN = g.Key.EDABAN,
                                     HINNMA = g.Key.HINNMA,
                                     STANKA = g.Key.STANKA,
                                     HTANKA = g.Key.HTANKA,
                                     ZBAIKA = g.Key.ZBAIKA,
                                     LOCTANA_SOKO_CODE = g.Key.LOCTANA_SOKO_CODE.HasValue ? (int)g.Key.LOCTANA_SOKO_CODE : 0,
                                     LOCTANA_FLOOR_NO = g.Key.LOCTANA_FLOOR_NO.HasValue ? (int)g.Key.LOCTANA_FLOOR_NO : 0,
                                     LOCTANA_TANA_NO = g.Key.LOCTANA_TANA_NO.HasValue ? (int)g.Key.LOCTANA_TANA_NO : 0,
                                     LOCTANA_CASE_NO = g.Key.LOCTANA_CASE_NO.HasValue ? (int)g.Key.LOCTANA_CASE_NO : 0,
                                     TSU = g.Sum(y => y.TSU),
                                 })
                             .OrderBy(g => g.LOCTANA_SOKO_CODE)
                             .ThenBy(g => g.LOCTANA_FLOOR_NO)
                             .ThenBy(g => g.LOCTANA_TANA_NO)
                             .ThenBy(g => g.HINBAN)
                             .ThenBy(g => g.EDABAN)
                         );

                        if (FujiyaDatas.Any())
                        {
                            FujiyaItems.Value = new ObservableCollection<FujiyaItem>();
                            var FujiyaModelList = new FujiyaItemList();
                            var addItems = new ObservableCollection<FujiyaItem>(FujiyaModelList.ConvertFujiyaDataToModel(FujiyaDatas)).ToList();
                            // 直接ObservableにAddするとなぜか落ちるためListをかます。
                            var setItems = new List<FujiyaItem>();
                            addItems.ForEach(item =>
                            {
                                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                      h => item.PropertyChanged += h,
                                      h => item.PropertyChanged -= h)
                                      .Subscribe(e =>
                                      {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = FujiyaItems.Value.Sum(x => x.発行枚数).ToString();
                                      });
                                setItems.Add(item);
                            });
                            FujiyaItems.Value = new ObservableCollection<FujiyaItem>(setItems);
                            TotalMaisu.Value = FujiyaItems.Value.Sum(x => x.発行枚数).ToString();
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
            return FujiyaItems.Value != null &&
                   FujiyaItems.Value.Any() &&
                   FujiyaItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.ZENSHOREN + "_" +
                        this.HachuBangou.Value + "_" +
                        this.JusinDate.Value.ToString("yyyyMMdd") + ".csv";
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
            var list = FujiyaItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "識別番号",
                "取引先コード",
                "部門コード",
                "中分類",
                "仕入週",
                "相手先品番",
                "商品コード",
                "原単価",
                "本体価格",
                "売価",
                "備考",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用商品コード");
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
            var layName = NefudaBangouText.Value == 1
                            ? @"00005-◆21号貼札.mllayx"
                            : @"00006-■11号吊札.mllayx";
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

    public class FujiyaItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private long _発行枚数;
        public long 発行枚数 //csv & 表示
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
        public string 識別番号 { get; set; }     //csv & 表示
        public string 取引先コード { get; set; }     //csv　※886固定
        public string 部門コード { get; set; }  //csv & 表示
        public string 中分類 { get; set; }  //csv & 表示
        public string 仕入週 { get; set; }  //csv & 表示
        public string 相手先品番 { get; set; }  //csv & 表示
        public string 商品コード { get; set; }  //csv ※品番 + "-" + 枝番
        public string 表示用商品コード { get; set; }  //表示
        public string 商品名 { get; set; } //表示
        public int 原単価 { get; set; }  //csv & 表示
        public int 本体価格 { get; set; }  //csv & 表示
        public int 売価 { get; set; }  //csv & 表示
        public string 備考 { get; set; }  //csv & 表示

        public FujiyaItem(long 発行枚数, string 識別番号, string 取引先コード, string 部門コード, string 中分類,
                                 string 仕入週, string 相手先品番, string 商品コード, string 表示用商品コード,
                                 string 商品名, int 原単価, int 本体価格, int 売価, string 備考)
        {
            this.発行枚数 = 発行枚数;
            this.識別番号 = 識別番号;
            this.取引先コード = 取引先コード;
            this.部門コード = 部門コード;
            this.中分類 = 中分類;
            this.仕入週 = 仕入週;
            this.相手先品番 = 相手先品番;
            this.商品コード = 商品コード;
            this.表示用商品コード = 表示用商品コード;
            this.商品名 = 商品名;
            this.原単価 = 原単価;
            this.本体価格 = 本体価格;
            this.売価 = 売価;
            this.備考 = 備考;
        }
    }

    public class FujiyaItemList
    {
        public IEnumerable<FujiyaItem> ConvertFujiyaDataToModel(List<FujiyaData> datas)
        {
            var result = new List<FujiyaItem>();
            var csvHincd = "";
            datas.ForEach(data =>
            {
                csvHincd = data.HINBAN + "-" + data.EDABAN;
                result.Add(
                    new FujiyaItem(data.TSU, data.SHIKIBETSU, data.TORIHIKICD, data.BUMONCD, data.CHUBUNRUI,
                                   data.SIRESYU, data.AITE_HINBAN, csvHincd, data.HINCD, data.HINNMA,
                                   data.STANKA, data.HTANKA, data.ZBAIKA, ""));
            });
            return result;
        }
    }
}
