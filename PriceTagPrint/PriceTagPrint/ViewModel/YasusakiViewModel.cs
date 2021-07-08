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
    public class YasusakiViewModel : ViewModelsBase
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

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<YasusakiData> YasusakiDatas { get; set; } = new List<YasusakiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<YasusakiItem>> YasusakiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<YasusakiItem>>();

        #endregion

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        private DB_0112_EOS_HACHU_LIST dB_0112_EOS_HACHU_LIST;
        private WEB_TORIHIKISAKI_TANKA_LIST wEB_TORIHIKISAKI_TANKA_LIST;

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
        public YasusakiViewModel()
        {
            dB_0112_EOS_HACHU_LIST = new DB_0112_EOS_HACHU_LIST();
            wEB_TORIHIKISAKI_TANKA_LIST = new WEB_TORIHIKISAKI_TANKA_LIST();
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
            if (!string.IsNullOrEmpty(hno))
            {
                if (dB_0112_EOS_HACHU_LIST.QueryWhereHnoExists(hno))
                {
                    HnoResultString.Value = "登録済 " + Tid.YASUSAKI + "-" + Tnm.YASUSAKI;
                    HnoResultColor.Value = Brushes.Blue;
                }
                else
                {
                    HnoResultString.Value = "※未登録";
                    HnoResultColor.Value = Brushes.Red;
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
            TotalMaisu.Value = "";
            YasusakiDatas.Clear();
            if (YasusakiItems.Value != null && YasusakiItems.Value.Any())
            {
                YasusakiItems.Value.Clear();
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
            if (!string.IsNullOrEmpty(this.BunruiCodeText.Value) && !BunruiCodeItems.Value.Select(x => x.Id.TrimEnd()).Contains(this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                var w0112EosHchuList = dB_0112_EOS_HACHU_LIST.QueryWhereHno(this.HachuBangou.Value);

                var wWebTorihikisakiTankaList = wEB_TORIHIKISAKI_TANKA_LIST.QueryWhereTcodeTenpo(TidNum.YASUSAKI.ToString(), "9999");

                if (w0112EosHchuList.Any() && this.HakkouTypeText.Value == 2)
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
                        w0112EosHchuList.Where(x => x.NSU > 0 && !string.IsNullOrEmpty(this.BunruiCodeText.Value) ? x.BUNRUI == int.Parse(this.BunruiCodeText.Value) : true)
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
                             .Where(g => g.NSU > 0)
                             .OrderBy(g => g.HNO)
                             .ThenBy(g => g.SYOHINCD.Replace("-", ""))
                         );

                    
                    if (YasusakiDatas.Any())
                    {
                        YasusakiItems.Value = new ObservableCollection<YasusakiItem>();
                        var yasusakiModelList = new YasusakiItemList();
                        var addItems = new ObservableCollection<YasusakiItem>(yasusakiModelList.ConvertYasusakiDataToModel(YasusakiDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<YasusakiItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = YasusakiItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        YasusakiItems.Value = new ObservableCollection<YasusakiItem>(setItems);
                        TotalMaisu.Value = YasusakiItems.Value.Sum(x => x.発行枚数).ToString();
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
            return YasusakiItems.Value != null &&
                   YasusakiItems.Value.Any() &&
                   YasusakiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.YASUSAKI + "_" + this.HachuBangou.Value + ".csv";
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
            var list = YasusakiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var datas = DataUtility.ToDataTable(list);
            // 不要なカラムの削除
            datas.Columns.Remove("商品名");
            datas.Columns.Remove("単価");
            datas.Columns.Remove("和合商品コード");
            datas.Columns.Remove("相手先品番");
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
            var grpName = @"\0112_ヤスサキ\【総額対応】ヤスサキ_V5_RT308R_振分発行";
            var layName = @"41300-ﾔｽｻｷ_JAN1段＋税_ST308R_振分発行.mldenx";
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
    public class YasusakiItem : INotifyPropertyChanged
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
        public int 発行枚数
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
        public int 発注No { get; set; }
        public string 取引先CD { get; set; }
        public string 値札No { get; set; }
        public string 発注日 { get; set; }
        public string 納品日 { get; set; }
        public string ｸﾗｽｺｰﾄﾞ { get; set; }
        public string 品番 { get; set; }
        public string 枝番 { get; set; }
        public string ｻｲｽﾞｺｰﾄﾞ { get; set; }
        public string 規格表現文字 { get; set; }
        public string 売切月 { get; set; }
        public string JAN { get; set; }
        public int 本体価格 { get; set; }
        public string 商品コード { get; set; }
        public string 商品名 { get; set; }
        public int 単価 { get; set; }
        public string 和合商品コード { get; set; }
        public string 相手先品番 { get; set; }

        public YasusakiItem(int 発注No, string 取引先CD, string 値札No, string 発注日, string 納品日,
                            string ｸﾗｽｺｰﾄﾞ, string 品番, string 枝番, string ｻｲｽﾞｺｰﾄﾞ, string 規格表現文字,
                            string 売切月, string JAN, int 本体価格, string 商品コード, int 発行枚数,
                             string 商品名, int 単価, string 和合商品コード, string 相手先品番)
        {
            this.発注No = 発注No;
            this.取引先CD = 取引先CD;
            this.値札No = 値札No;
            this.発注日 = 発注日;
            this.納品日 = 納品日;
            this.ｸﾗｽｺｰﾄﾞ = ｸﾗｽｺｰﾄﾞ;
            this.品番 = 品番;
            this.枝番 = 枝番;
            this.ｻｲｽﾞｺｰﾄﾞ = ｻｲｽﾞｺｰﾄﾞ;
            this.規格表現文字 = 規格表現文字;
            this.売切月 = 売切月;
            this.JAN = JAN;
            this.売切月 = 売切月;
            this.品番 = 品番;
            this.JAN = JAN;
            this.本体価格 = 本体価格;
            this.商品コード = 商品コード;
            this.発行枚数 = 発行枚数;
            this.商品名 = 商品名;
            this.単価 = 単価;
            this.和合商品コード = 和合商品コード;
            this.相手先品番 = 相手先品番;
        }
    }

    public class YasusakiItemList
    {
        public IEnumerable<YasusakiItem> ConvertYasusakiDataToModel(List<YasusakiData> datas)
        {
            var result = new List<YasusakiItem>();
            var uritukiList = new DB_0112_URITUKI_LIST();
            var urituki = "";
            var beforeNouhinbi = DateTime.MinValue;
            var beforeSkbn = "";
            datas.ForEach(data =>
            {
                if (beforeNouhinbi != data.NOUHINBI || beforeSkbn != data.SKBN)
                {
                    urituki = uritukiList.GetURITUKI(data.NOUHINBI, data.SKBN);
                }
                result.Add(
                    new YasusakiItem(data.HNO, data.TOKCD, data.NEFUDA_KBN, data.HATYUBI.ToString("yyyyMMdd"), data.NOUHINBI.ToString("yyyyMMdd"), data.NETUKE_BUNRUI, data.SCODE,
                                     data.SAIZUS.ToString("00"), data.BIKOU1, data.BIKOU2, urituki, data.JANCD, data.BAIKA, data.SYOHINCD, data.NSU,
                                     data.EOS_SYOHINNM, data.GENKA, data.HINCD, data.SYOHINCD));
                beforeNouhinbi = data.NOUHINBI;
                beforeSkbn = data.SKBN;
            });
            return result;
        }
    }
}
