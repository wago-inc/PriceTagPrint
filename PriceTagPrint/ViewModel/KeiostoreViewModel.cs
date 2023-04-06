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
    public class KeiostoreViewModel : ViewModelsBase
    {
        #region プロパティ
        // 発行区分
        public ReactiveProperty<int> HakkouTypeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedHakkouTypeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 受信日
        public ReactiveProperty<DateTime> JusinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today);

        // 納品日
        public ReactiveProperty<DateTime> NouhinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today.AddDays(1));

        // 分類コード
        public ReactiveProperty<int> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 開始単品ｺｰﾄﾞ
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了単品ｺｰﾄﾞ
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");

        // 発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        public ReactiveProperty<ObservableCollection<KeiostoreItem>> KeiostoreItems { get; set; }
                = new ReactiveProperty<ObservableCollection<KeiostoreItem>>();

        private List<KeiostoreData> KeiostoreDatas { get; set; } = new List<KeiostoreData>();

        #endregion

        private readonly string _grpName = @"8115_京王ストア";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\8115_京王ストア";
        private CsvUtility csvUtility = new CsvUtility();

        // 値札テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private TOKMTE_LIST tOKMTE_LIST;

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
        public KeiostoreViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            tOKMTE_LIST = new TOKMTE_LIST();

            CreateComboItems();

            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<int>();
            NefudaBangouText = new ReactiveProperty<int>();

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
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
            HakkouTypeItems.Value = new ObservableCollection<CommonIdName>(CreateHakkouTypeItems());
            BunruiCodeItems.Value = new ObservableCollection<CommonIdName>(CreateBunruiCodeItems());
            NefudaBangouItems.Value = new ObservableCollection<CommonIdName>(CreateNefudaBangouItems());
        }

        /// <summary>
        /// 分類コードItems生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateBunruiCodeItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 7098;
            item.Name = "7098：レッグ";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 7099;
            item2.Name = "7099：インナー";
            list.Add(item2);
            return list;
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
            item1.Name = "1：２１号ラベル";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：１１号タグ";
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
        /// 分類コードテキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void BunruiCodeTextChanged(int id)
        {
            var item = BunruiCodeItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedBunruiCodeIndex.Value = BunruiCodeItems.Value.IndexOf(item);
            }
            else
            {
                BunruiCodeText.Value = 7098;
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
                SelectedNefudaBangouIndex.Value = 0;
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
                BunruiCodeText.Value = item.Id;
            }
            else
            {
                BunruiCodeText.Value = 0;
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
            NouhinDate.Value = DateTime.Today.AddDays(1);
            SelectedHakkouTypeIndex.Value = 0;
            SelectedBunruiCodeIndex.Value = 0;
            SelectedNefudaBangouIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            TotalMaisu.Value = "";
            if (KeiostoreItems.Value != null && KeiostoreItems.Value.Any())
            {
                KeiostoreItems.Value.Clear();
            }
            HakkouTypeTextBox.Focus();
        }

        /// <summary>
        /// F5検索入力チェック
        /// </summary>
        /// <returns></returns>
        public bool InputCheck()
        {
            DateTime convDate;
            if (string.IsNullOrEmpty(this.JusinDatePicker.Text) || !DateTime.TryParse(this.JusinDatePicker.Text, out convDate))
            {
                MessageBox.Show("受信日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.JusinDatePicker.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.NouhinDatePicker.Text) || !DateTime.TryParse(this.NouhinDatePicker.Text, out convDate))
            {
                MessageBox.Show("納品日を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.NouhinDatePicker.Focus();
                return false;
            }
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.HakkouTypeTextBox.Focus();
                return false;
            }
            if (!this.BunruiCodeItems.Value.Select(x => x.Id).Any(id => id == this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!this.NefudaBangouItems.Value.Select(x => x.Id).Any(id => id == this.NefudaBangouText.Value))
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }            
            return true;
        }

        /// <summary>
        /// F5読込処理
        /// </summary>
        public void NefudaDataDisplay()
        {
            ProcessingSplash ps = new ProcessingSplash("データ作成中...", () =>
            {
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.KEIOSTORE, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value.ToString(), this.SttHincd.Value, this.EndHincd.Value);
                var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.KEIOSTORE);
                if (eosJutraList.Any() && tokmteList.Any())
                {
                    KeiostoreDatas.Clear();
                    KeiostoreDatas.AddRange(
                        eosJutraList
                            .GroupJoin(
                                   tokmteList,
                                   e => new
                                   {
                                       VHINCD = e.VHINCD.ToString().TrimEnd(),
                                       TOKCD = e.VRYOHNCD.ToString().TrimEnd(),
                                       HINCD = e.HINCD.ToString().TrimEnd(),
                                   },
                                   t => new
                                   {
                                       VHINCD = t.EOSHINID.TrimEnd(),
                                       TOKCD = t.TOKCD.TrimEnd(),
                                       HINCD = t.HINCD.TrimEnd(),
                                   },
                                   (eos, tok) => new
                                   {
                                       VRYOHNCD = eos.VRYOHNCD.TrimEnd(),
                                       VBUNCD = eos.VBUNCD.TrimEnd(),
                                       VRCVDT = eos.VRCVDT.TrimEnd(),
                                       VNOHINDT = eos.VNOHINDT.TrimEnd(),
                                       BUNRUICD = "51",
                                       TEIBAN = "1",
                                       VGNKTK = eos.VGNKTK,
                                       VURITK = eos.VURITK,
                                       VCOLCD = eos.VCOLCD.TrimEnd(),
                                       VSIZCD = eos.VSIZCD.TrimEnd(),
                                       VHINCD = eos.VHINCD.TrimEnd(),
                                       HINCD = eos.HINCD.TrimEnd(),
                                       VHINNMA = eos.VHINNMA.TrimEnd(),
                                       VSIZNM = eos.VSIZNM.TrimEnd(),
                                       VSURYO = eos.VSURYO,
                                       SIZCD = tok.Any() ? tok.FirstOrDefault().SIZCD.TrimStart(new Char[] { '0' }) : "",
                                   })
                            .GroupBy(a => new
                            {
                                a.VRYOHNCD,
                                a.VBUNCD,
                                a.VRCVDT,
                                a.VNOHINDT,
                                a.BUNRUICD,
                                a.TEIBAN,
                                a.VGNKTK,
                                a.VURITK,
                                a.VCOLCD,
                                a.VSIZCD,
                                a.VHINCD,
                                a.HINCD,
                                a.VHINNMA,
                                a.VSIZNM,
                                a.SIZCD
                            })
                            .Select(g => new KeiostoreData
                            {
                                VRYOHNCD = g.Key.VRYOHNCD,
                                VBUNCD = g.Key.VBUNCD,
                                DAICHU = g.Key.VBUNCD == "7098" ? "8" : g.Key.VBUNCD == "7099" ? "9" : "",
                                VRCVDT = g.Key.VRCVDT,
                                VNOHINDT = g.Key.VNOHINDT,
                                BUNRUICD = g.Key.BUNRUICD,  // 分類コード
                                TEIBAN = g.Key.TEIBAN,
                                VGNKTK = g.Key.VGNKTK,  // 原単価
                                VURITK = g.Key.VURITK,  // 売単価
                                VCOLCD = g.Key.VCOLCD,
                                VSIZCD = g.Key.VSIZCD,
                                VHINCD = g.Key.VHINCD,  // 相手先品番
                                HINCD = g.Key.HINCD,    // 和合商品コード
                                VHINNMA = g.Key.VHINNMA,
                                VSIZNM = g.Key.VSIZNM,
                                VSURYO = g.Sum(y => y.VSURYO),  // 数量
                                SIZCD = g.Key.SIZCD     // 値札区分
                            })
                            .Where(x => this.NefudaBangouText.Value == 2 ? x.SIZCD.TrimEnd() == "2" : x.SIZCD.TrimEnd() != "2")
                            .OrderBy(x => x.VHINCD)
                    );

                    if (KeiostoreDatas.Any())
                    {
                        KeiostoreItems.Value = new ObservableCollection<KeiostoreItem>();
                        var KeioModelList = new KeiostoreItemList();
                        var addItems = new ObservableCollection<KeiostoreItem>(KeioModelList.ConvertKeioDataToModel(KeiostoreDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<KeiostoreItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                      // 発行枚数に変更があったら合計発行枚数も変更する
                                      TotalMaisu.Value = KeiostoreItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        KeiostoreItems.Value = new ObservableCollection<KeiostoreItem>(setItems);
                        TotalMaisu.Value = KeiostoreItems.Value.Sum(x => x.発行枚数).ToString();
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
            return KeiostoreItems.Value != null &&
                   KeiostoreItems.Value.Any() &&
                   KeiostoreItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.KEIOSTORE + "_" + DateTime.Today.ToString("yyyyMMddmmhhss") + ".csv";
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
            var list = KeiostoreItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "部門",
                "大中区分",
                "取引先コード",
                "販促文字",
                "ﾒｰｶｰﾌﾘｰ",
                "本体価格",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("品名");
            datas.Columns.Remove("相手先品番");
            datas.Columns.Remove("伝票番号");
            datas.Columns.Remove("行番号");
            datas.Columns.Remove("原単価");
            datas.Columns.Remove("文字色");
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
            var layName = NefudaBangouText.Value == 1 ? @"00070-21号JAN2 ﾌﾟﾛﾊﾟｰ（本体・税込）.mllayx" : "00030-11号JAN2 ﾌﾟﾛﾊﾟｰ（本体・税込）.mllayx";
            var layNo = Path.Combine(CommonStrings.MLV5LAYOUT_PATH, _grpName) + @"\" + layName;
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

    public class KeiostoreItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private decimal _発行枚数;
        public decimal 発行枚数
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

        public string 部門 { get; set; }      // CSV & 表示
        private string _大中区分;
        public string 大中区分
        {
            get { return _大中区分; }
            set
            {
                if (value != this._大中区分)
                {
                    this._大中区分 = value;
                    this.文字色 = value.Length != 4 ? Brushes.Red : Brushes.Black;
                    this.OnPropertyChanged("大中区分");
                }
            }
        }
        public string 取引先コード { get; set; } = "543106"; // CSV & 表示
        public string 販促文字 { get; set; } = "";    // CSVのみ
        public string ﾒｰｶｰﾌﾘｰ { get; set; }   // CSV & 表示　※商品コードをセット
        public string 品名 { get; set; }      // 表示のみ
        public string 相手先品番 { get; set; }   // 表示のみ
        public string 伝票番号 { get; set; }    // 表示のみ
        public string 行番号 { get; set; }     // 表示のみ
        public decimal 原単価 { get; set; }    // 表示のみ
        public decimal 本体価格 { get; set; }    // CSV & 表示
        private Brush _文字色;
        public Brush 文字色
        {
            get { return _文字色; }
            set
            {
                if (value != this._文字色)
                {
                    this._文字色 = value;
                    this.OnPropertyChanged("文字色");
                }
            }
        }
        public KeiostoreItem(decimal 発行枚数, string 部門, string 大中区分, string 相手先品番, string 商品コード, string 品名,
                         decimal 原単価, decimal 本体価格)
        {
            this.発行枚数 = 発行枚数;
            this.部門 = 部門;
            this.大中区分 = 大中区分;
            this.相手先品番 = 相手先品番;
            this.ﾒｰｶｰﾌﾘｰ = 商品コード;
            this.品名 = 品名;
            this.原単価 = 原単価;
            this.本体価格 = 本体価格;
        }
    }
    public class KeiostoreItemList
    {
        public IEnumerable<KeiostoreItem> ConvertKeioDataToModel(List<KeiostoreData> datas)
        {
            var result = new List<KeiostoreItem>();
            var bumon = "";
            datas.ForEach(data =>
            {
                bumon = !string.IsNullOrEmpty(data.VBUNCD) && data.VBUNCD.TrimEnd().Length >= 4 ? data.VBUNCD.TrimEnd().Substring(2, 2) : "99";
                result.Add(
                    new KeiostoreItem(data.VSURYO, bumon, data.DAICHU, data.VHINCD, data.HINCD, data.VHINNMA, data.VGNKTK, data.VURITK));
            });
            return result;
        }
    }
}
