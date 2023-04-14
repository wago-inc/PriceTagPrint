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
    public class SunnyMartViewModel : ViewModelsBase
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

        public ReactiveProperty<ObservableCollection<SunnyMartItem>> SunnyMartItems { get; set; }
                = new ReactiveProperty<ObservableCollection<SunnyMartItem>>();

        private List<SunnyMartData> SunnyMartDatas { get; set; } = new List<SunnyMartData>();

        #endregion

        private readonly string _grpName = @"0105_サニーマート";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\0105_サニーマート";
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
        public SunnyMartViewModel()
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
            item.Id = 5191;
            item.Name = "5191：0051 91";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 5192;
            item2.Name = "5192：0051 92";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 5491;
            item3.Name = "5491：0054 91";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 5492;
            item4.Name = "5492：0054 92";
            list.Add(item4);
            var item5 = new CommonIdName();
            item5.Id = 5591;
            item5.Name = "5591：0055 91";
            list.Add(item5);
            var item6 = new CommonIdName();
            item6.Id = 5592;
            item6.Name = "5592：0055 92";
            list.Add(item6);
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
            item1.Id = 46;
            item1.Name = "00046-●タグ小 JAN13_３７×４０_総額";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 47;
            item2.Name = "00047-●タグ小 JAN8_３７×４０_総額";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 49;
            item3.Name = "00049-●ﾗﾍﾞﾙ小 JAN13_３５×３７_総額";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 50;
            item4.Name = "00050-●ﾗﾍﾞﾙ小 JAN8_３５×３７_総額";
            list.Add(item4);
            var item5 = new CommonIdName();
            item5.Id = 51;
            item5.Name = "00051-●ﾗﾍﾞﾙ売価のみ(ｸﾞﾝｾﾞ用)_３１×３６_総額.";
            list.Add(item5);
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
                BunruiCodeText.Value = 5191;
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
                NefudaBangouText.Value = 46;
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
            if (SunnyMartItems.Value != null && SunnyMartItems.Value.Any())
            {
                SunnyMartItems.Value.Clear();
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
                var aite_bunrui = BunruiCodeText.Value.ToString().Substring(0, 2).PadLeft(4, '0');
                var wago_bunrui = BunruiCodeText.Value.ToString().Substring(2, 2);
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.SUNNY_MART, JusinDate.Value, NouhinDate.Value, aite_bunrui, this.SttHincd.Value, this.EndHincd.Value)
                                    .Where(x => x.HINCD.StartsWith(wago_bunrui));
                var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.SUNNY_MART);
                if (eosJutraList.Any() && tokmteList.Any())
                {
                    SunnyMartDatas.Clear();
                    SunnyMartDatas.AddRange(
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
                                       VURITK = eos.VURITK,
                                       VHINCD = eos.VHINCD.TrimEnd(),
                                       HINCD = eos.HINCD.TrimEnd(),
                                       VHINNM = !string.IsNullOrEmpty(eos.VHINNMA.TrimEnd()) ? eos.VHINNMA.TrimEnd() : eos.VHINNMB.TrimEnd(),
                                       VCYOBI7 = eos.VCYOBI7.TrimEnd(),
                                       VSURYO = eos.VSURYO,
                                   })
                            .GroupBy(a => new
                            {
                                a.VRYOHNCD,
                                a.VBUNCD,
                                a.VRCVDT,
                                a.VNOHINDT,
                                a.VURITK,
                                a.VHINCD,
                                a.HINCD,
                                a.VHINNM,
                                a.VCYOBI7
                            })
                            .Select(g => new SunnyMartData
                            {
                                VRYOHNCD = g.Key.VRYOHNCD,
                                VBUNCD = g.Key.VBUNCD,
                                VRCVDT = g.Key.VRCVDT,
                                VNOHINDT = g.Key.VNOHINDT,
                                VURITK = g.Key.VURITK,  // 売単価
                                VHINCD = g.Key.VHINCD,  // 相手先品番
                                HINCD = g.Key.HINCD,    // 和合商品コード
                                VHINNM = g.Key.VHINNM,
                                VCYOBI7 = g.Key.VCYOBI7,
                                VSURYO = g.Sum(y => y.VSURYO),  // 数量
                            })
                            .OrderBy(x => x.HINCD)
                    );

                    if (SunnyMartDatas.Any())
                    {
                        SunnyMartItems.Value = new ObservableCollection<SunnyMartItem>();
                        var SunnyMartModelList = new SunnyMartItemList();
                        var addItems = new ObservableCollection<SunnyMartItem>(SunnyMartModelList.ConvertSunnyDataToModel(SunnyMartDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<SunnyMartItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                      // 発行枚数に変更があったら合計発行枚数も変更する
                                      TotalMaisu.Value = SunnyMartItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        SunnyMartItems.Value = new ObservableCollection<SunnyMartItem>(setItems);
                        TotalMaisu.Value = SunnyMartItems.Value.Sum(x => x.発行枚数).ToString();
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
            return SunnyMartItems.Value != null &&
                   SunnyMartItems.Value.Any() &&
                   SunnyMartItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.SUNNY_MART + "_" + DateTime.Today.ToString("yyyyMMddmmhhss") + ".csv";
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
            var list = SunnyMartItems.Value.Where(x => x.発行枚数 > 0).ToList();
            DataTable datas;
            if (NefudaBangouText.Value != 51)
            {
                var csvColSort = new string[]
                {
                    "月",
                    "下",
                    "部門",
                    "JANコード",
                    "売価",
                    "発行枚数"
                };
                datas = DataUtility.ToDataTable(list, csvColSort);
                // 不要なカラムの削除
                datas.Columns.Remove("相手品番");
                datas.Columns.Remove("商品コード");
                datas.Columns.Remove("商品名");
                datas.Columns.Remove("税込売価");
            }
            else
            {
                var csvColSort = new string[]
                {
                    "売価",
                    "発行枚数"
                };
                datas = DataUtility.ToDataTable(list, csvColSort);
                // 不要なカラムの削除
                datas.Columns.Remove("月");
                datas.Columns.Remove("下");
                datas.Columns.Remove("部門");
                datas.Columns.Remove("JANコード");
                datas.Columns.Remove("相手品番");
                datas.Columns.Remove("商品コード");
                datas.Columns.Remove("商品名");
                datas.Columns.Remove("税込売価");
            }
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
            var layName = NefudaBangouItems.Value.Select(x => x.Name).ToArray()[SelectedNefudaBangouIndex.Value] + ".mllayx";
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

    public class SunnyMartItem : INotifyPropertyChanged
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

        public string 相手品番 { get; set; }    //表示のみ VHINCD
        public string 商品コード { get; set; }   //表示のみ HINCD
        public string 商品名 { get; set; } //表示のみ VHINNMA
        public string 月 { get; set; }   //表示＆CSV VNOHINDTの月
        public int 下 { get; set; }   //表示＆CSV VNOHINDTの日
        public string 部門 { get; set; }  //表示＆CSV HINCLIDで判定（91は504, 92は505）
        public string JANコード { get; set; }//表示＆CSV VCYOBI7
        public decimal 売価 { get; set; } //表示＆CSV VURITK
        public decimal 税込売価 { get; set; }   //表示 

        public SunnyMartItem(decimal 発行枚数, string 相手品番, string 商品コード, string 商品名, string 月, int 下,
                             string 部門, string JANコード, decimal 売価, decimal 税込売価)
        {
            this.発行枚数 = 発行枚数;
            this.相手品番 = 相手品番;
            this.商品コード = 商品コード;
            this.商品名 = 商品名;
            this.月 = 月;
            this.下 = 下;
            this.部門 = 部門;
            this.JANコード = JANコード;
            this.売価 = 売価;
            this.税込売価 = 税込売価;
        }
    }

    public class SunnyMartItemList
    {
        public IEnumerable<SunnyMartItem> ConvertSunnyDataToModel(List<SunnyMartData> datas)
        {
            var result = new List<SunnyMartItem>();
            int sage = 99;
            int day;
            string bumon = "";
            string bunrui;
            var zeiritsu = Zeiritsu.items.FirstOrDefault(x => x.SttDate <= DateTime.Today && DateTime.Today <= x.EndDate)?.Kakeritsu ?? 1;

            datas.ForEach(data =>
            {
                day = int.TryParse(data.VNOHINDT.Substring(6, 2), out day) ? day : 0;
                if (1 <= day && day <= 9) sage = 0;
                else if (10 <= day && day <= 19) sage = 1;
                else if (20 <= day && day <= 29) sage = 2;
                else if (30 <= day) sage = 3;

                bunrui = data.HINCD.Substring(0, 2);
                bumon = bunrui == "91" ? "504" : bunrui == "92" ? "505" : "";
                result.Add(
                    new SunnyMartItem(data.VSURYO, data.VHINCD, data.HINCD, data.VHINNM, data.VNOHINDT.Substring(4, 2),
                                      sage, bumon, data.VCYOBI7, data.VURITK, Math.Floor(data.VURITK * zeiritsu)));
            });
            return result;
        }
    }
}
