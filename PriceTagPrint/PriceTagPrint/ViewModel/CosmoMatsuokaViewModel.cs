using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.View;
using PriceTagPrint.WAG_USR1;
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
    public class CosmoMatsuokaViewModel : ViewModelsBase
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

        // 分類コード
        public ReactiveProperty<string> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 季節コード
        public ReactiveProperty<string> KisetsuCodeText { get; set; }

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

        private List<CosmoMatsuokaData> CosmoMatsuokaDatas { get; set; } = new List<CosmoMatsuokaData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<CosmoMatsuokaItem>> CosmoMatsuokaItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CosmoMatsuokaItem>>();

        #endregion

        private readonly string _grpName = @"\マツオカ RT308R【総額表示】";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public TextBox KisetsuCodeTextBox = null;
        public DatePicker JusinDatePicker = null;

        private DB_7883_EOS_HACHU_LIST eOSHACHU_LIST;
        private DB_7883_SYOHIN_MST_LIST sYOHIN_LIST;
        private List<DB_7883_SYOHIN_MST> syohinList;
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
        public CosmoMatsuokaViewModel()
        {
            eOSHACHU_LIST = new DB_7883_EOS_HACHU_LIST();

            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>();
            KisetsuCodeText = new ReactiveProperty<string>();

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));

            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

            ProcessingSplash ps = new ProcessingSplash("起動中", () =>
            {
                sYOHIN_LIST = new DB_7883_SYOHIN_MST_LIST();
                syohinList = sYOHIN_LIST.QueryWhereAll();
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
            item1.Name = "1：無地ラベル1段(総額表示)";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：無地タグ1段(総額表示)";
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
            KisetsuCodeText.Value = "";
            BunruiCodeText.Value = "";
            SelectedNefudaBangouIndex.Value = 0;
            SttScode.Value = "";
            EndScode.Value = "";
            TotalMaisu.Value = "";
            CosmoMatsuokaDatas.Clear();
            if (CosmoMatsuokaItems.Value != null && CosmoMatsuokaItems.Value.Any())
            {
                CosmoMatsuokaItems.Value.Clear();
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
            if (string.IsNullOrEmpty(this.KisetsuCodeText.Value))
            {
                MessageBox.Show("季節コードを入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);

                this.KisetsuCodeTextBox.Focus();
                return false;
            }
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.HakkouTypeTextBox.Focus();
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
                var eosHachuList = eOSHACHU_LIST.QueryWhereDate(JusinDate.Value);

                if (eosHachuList.Any())
                {
                    int sttScode;
                    int endScode;
                    int dbScode;
                    long bunrui;

                    eosHachuList = eosHachuList.Where(x =>
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



                    if (eosHachuList.Any() && syohinList.Any())
                    {
                        CosmoMatsuokaDatas.Clear();
                        CosmoMatsuokaDatas.AddRange(
                            eosHachuList
                            .GroupJoin(
                                   syohinList,
                                   eos => new
                                   {
                                       JANCD = eos.EOS_SYOHINCD.TrimEnd(),
                                   },
                                   shn => new
                                   {
                                       JANCD = shn.JANコード.TrimEnd(),
                                   },
                                   (eos, shn) => new
                                   {
                                       HINCD = eos.HINCD.TrimEnd(),
                                       HINNMA = eos.EOS_SYOHINNM.TrimEnd(),
                                       HINNMB = eos.HINNMB.TrimEnd(),
                                       SCODE = eos.SCODE.TrimEnd(),
                                       EDANO = eos.SAIZUS.ToString("00"),
                                       SIRCD = eos.EOS_SIRESAKICD.TrimEnd(),
                                       SIRDATE = !string.IsNullOrEmpty(eos.EOS_HATYUBI) && eos.EOS_HATYUBI.Length >= 5 ? eos.EOS_HATYUBI.Substring(eos.EOS_HATYUBI.Length - 5) : "",
                                       GENKA = eos.EOS_GENKA,
                                       BAIKA = eos.EOS_BAIKA,
                                       JANCD = eos.EOS_SYOHINCD.TrimEnd(),
                                       SUBCLASSNo = shn.Any() ? shn.FirstOrDefault().SUBCLASSNo.TrimEnd() : "",
                                       TNANO = shn.Any() ? shn.FirstOrDefault().棚番号.TrimEnd() : "",
                                       HSU = eos.EOS_HSU,
                                   })
                            .GroupBy(a => new
                            {
                                HINCD = a.HINCD,
                                HINNMA = a.HINNMA,
                                HINNMB = a.HINNMB,
                                SCODE = a.SCODE,
                                EDANO = a.EDANO,
                                SIRCD = a.SIRCD,
                                SIRDATE = a.SIRDATE,
                                GENKA = a.GENKA,
                                BAIKA = a.BAIKA,
                                JANCD = a.JANCD,
                                SUBCLASSNo = a.SUBCLASSNo,
                                TNANO = a.TNANO,
                            })
                        .Select(g => new CosmoMatsuokaData
                        {
                            HINCD = g.Key.HINCD,
                            HINNMA = g.Key.HINNMA,
                            HINNMB = g.Key.HINNMB,
                            HINBAN = g.Key.SCODE + "-" + g.Key.EDANO,
                            SIRCD = g.Key.SIRCD,
                            SIRDATE = g.Key.SIRDATE,
                            GENKA = g.Key.GENKA,
                            BAIKA = g.Key.BAIKA,
                            JANCD = g.Key.JANCD,
                            SUBCLASSNo = g.Key.SUBCLASSNo,
                            TNANO = g.Key.TNANO,
                            SEASONCD = this.KisetsuCodeText.Value,
                            HSU = g.Sum(y => y.HSU),
                        })
                        .Where(x => x.HSU > 0)
                        .OrderBy(g => g.HINCD));

                        if (CosmoMatsuokaDatas.Any())
                        {
                            CosmoMatsuokaItems.Value = new ObservableCollection<CosmoMatsuokaItem>();
                            var CosmoMatsuokaModelList = new CosmoMatsuokaItemList();
                            var addItems = new ObservableCollection<CosmoMatsuokaItem>(CosmoMatsuokaModelList.ConvertCosmoMatsuokaDataToModel(CosmoMatsuokaDatas)).ToList();
                            // 直接ObservableにAddするとなぜか落ちるためListをかます。
                            var setItems = new List<CosmoMatsuokaItem>();
                            addItems.ForEach(item =>
                            {
                                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                      h => item.PropertyChanged += h,
                                      h => item.PropertyChanged -= h)
                                      .Subscribe(e =>
                                      {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = CosmoMatsuokaItems.Value.Sum(x => x.発行枚数).ToString();
                                      });
                                setItems.Add(item);
                            });
                            CosmoMatsuokaItems.Value = new ObservableCollection<CosmoMatsuokaItem>(setItems);
                            TotalMaisu.Value = CosmoMatsuokaItems.Value.Sum(x => x.発行枚数).ToString();
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
            return CosmoMatsuokaItems.Value != null &&
                   CosmoMatsuokaItems.Value.Any() &&
                   CosmoMatsuokaItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.COSMOMATUOKA + "_" +
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
            var list = CosmoMatsuokaItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "仕入先コード",
                "仕入年月日",
                "原価",
                "売価",
                "タイトル01",
                "タイトル02",
                "タイトル03",
                "JANコード",
                "サブクラスNo",
                "棚番号",
                "商品コード",
                "入荷数",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用サブクラスNo");
            datas.Columns.Remove("表示用商品コード");
            datas.Columns.Remove("商品名");
            datas.Columns.Remove("サイズカラー");
            datas.Columns.Remove("季節CD");
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
                            ? @"無地ラベル1段(総額表示).mllayx"
                            : @"無地タグ1段(総額表示).mllayx";
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

    public class CosmoMatsuokaItem : INotifyPropertyChanged
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
        public string 仕入先コード { get; set; }     //csv
        public string 仕入年月日 { get; set; }     //csv
        public long 原価 { get; set; }  //csv & 表示
        public long 売価 { get; set; }  //csv & 表示
        public string タイトル01 { get; set; }  //csv
        public string タイトル02 { get; set; }  //csv
        public string タイトル03 { get; set; }  //csv
        public string JANコード { get; set; }  //csv & 表示
        public string 表示用サブクラスNo { get; set; }  //表示
        public string サブクラスNo { get; set; }  //csv
        public string 棚番号 { get; set; } //csv & 表示
        public string 表示用商品コード { get; set; } //表示
        public string 商品コード { get; set; }    //csv
        public string 商品名 { get; set; }     //表示
        public string サイズカラー { get; set; }     //表示
        public string 季節CD { get; set; }     //表示
        public int 入荷数 { get; set; }   //csv

        public CosmoMatsuokaItem(long 発行枚数, string 仕入先コード, string 仕入年月日, long 原価, long 売価,
                                 string タイトル01, string タイトル02, string タイトル03, string JANコード,
                                 string サブクラスNo, string 棚番号, string 表示用商品コード, string 商品コード,
                                 string 商品名, string サイズカラー, string 季節CD, int 入荷数)
        {

            this.発行枚数 = 発行枚数;
            this.仕入先コード = 仕入先コード;
            this.仕入年月日 = 仕入年月日;
            this.原価 = 原価;
            this.売価 = 売価;
            this.タイトル01 = タイトル01;
            this.タイトル02 = タイトル02;
            this.タイトル03 = タイトル03;
            this.JANコード = JANコード;
            this.表示用サブクラスNo = サブクラスNo;
            this.サブクラスNo = サブクラスNo + 季節CD;
            this.棚番号 = 棚番号;
            this.表示用商品コード = 表示用商品コード;
            this.商品コード = 商品コード;
            this.商品名 = 商品名;
            this.サイズカラー = サイズカラー;
            this.季節CD = 季節CD;
            this.入荷数 = 入荷数;
        }
    }

    public class CosmoMatsuokaItemList
    {
        public IEnumerable<CosmoMatsuokaItem> ConvertCosmoMatsuokaDataToModel(List<CosmoMatsuokaData> datas)
        {
            var result = new List<CosmoMatsuokaItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new CosmoMatsuokaItem(data.HSU, data.SIRCD, data.SIRDATE, data.GENKA, data.BAIKA, "3",
                                    "", "", data.JANCD, data.SUBCLASSNo, data.TNANO, data.HINCD,
                                    data.HINBAN, data.HINNMA, data.HINNMB, data.SEASONCD, data.HSU));
            });
            return result;
        }
    }
}
