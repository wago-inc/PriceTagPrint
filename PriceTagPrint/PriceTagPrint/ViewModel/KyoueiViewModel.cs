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
    public class KyoueiViewModel : ViewModelsBase
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

        public ReactiveProperty<ObservableCollection<KyoeiItem>> KyoeiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<KyoeiItem>>();

        private List<KyoeiData> KyoeiDatas { get; set; } = new List<KyoeiData>();

        #endregion

        private readonly string _grpName = @"0101_キョーエイ\【総額表示】_V5_ST308R";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\0101_キョーエイ\【総額表示】_V5_ST308R";
        private CsvUtility csvUtility = new CsvUtility();

        // 値札テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private TOKMTE_TSURI_LIST tOKMTE_LIST;

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

        public KyoueiViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            tOKMTE_LIST = new TOKMTE_TSURI_LIST();

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
            item.Id = 51;
            item.Name = "51：定番";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 511;
            item2.Name = "511：スポット";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 52;
            item3.Name = "52：スポット他";
            list.Add(item3);
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
                BunruiCodeText.Value = 3;
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
            //KyoeiiDatas.Clear();
            if (KyoeiItems.Value != null && KyoeiItems.Value.Any())
            {
                KyoeiItems.Value.Clear();
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
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.KYOEI, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value.ToString(), this.SttHincd.Value, this.EndHincd.Value);
                //todo:元のプログラムがゴミなのでTOKMTEをwag_usr1ではなく値札出力のアクセスに定義している。
                // 対策：wag_usr1にTOKMTEをコピーして吊り札判定に特化したTOKMTE_TSURIを作成した。
                var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.KYOEI);
                if (eosJutraList.Any() && tokmteList.Any())
                {
                    KyoeiDatas.Clear();
                    KyoeiDatas.AddRange(
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
                                       NOUKI = "0000",
                                       HINBAN = "",
                                       VGNKTK = eos.VGNKTK,
                                       VURITK = eos.VURITK,
                                       VCOLCD = eos.VCOLCD.TrimEnd(),
                                       VSIZCD = eos.VSIZCD.TrimEnd(),
                                       VHINCD = eos.VHINCD.TrimEnd(),
                                       HINCD = eos.HINCD.TrimEnd(),
                                       LOCCD = "",
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
                                a.NOUKI,
                                a.HINBAN,
                                a.VGNKTK,
                                a.VURITK,
                                a.VCOLCD,
                                a.VSIZCD,
                                a.VHINCD,
                                a.HINCD,
                                a.LOCCD,
                                a.VHINNMA,
                                a.VSIZNM,
                                a.SIZCD
                            })
                            .Select(g => new KyoeiData
                            {
                                VRYOHNCD = g.Key.VRYOHNCD,
                                VBUNCD = g.Key.VBUNCD,
                                VRCVDT = g.Key.VRCVDT,
                                VNOHINDT = g.Key.VNOHINDT,
                                BUNRUICD = g.Key.BUNRUICD,  // 分類コード
                                TEIBAN = g.Key.TEIBAN,
                                NOUKI = g.Key.NOUKI,
                                HINBAN = g.Key.HINBAN,
                                VGNKTK = g.Key.VGNKTK,  // 原単価
                                VURITK = g.Key.VURITK,  // 売単価
                                VCOLCD = g.Key.VCOLCD,
                                VSIZCD = g.Key.VSIZCD,
                                VHINCD = g.Key.VHINCD,  // 京屋商品コード
                                HINCD = g.Key.HINCD,    // 和合商品コード
                                LOCCD = g.Key.LOCCD,
                                VHINNMA = g.Key.VHINNMA,
                                VSIZNM = g.Key.VSIZNM,
                                VSURYO = g.Sum(y => y.VSURYO),  // 数量
                                SIZCD = g.Key.SIZCD     // 値札区分
                            })
                            .Where(x => this.NefudaBangouText.Value == 2 ? x.SIZCD.TrimEnd() == "2" : x.SIZCD.TrimEnd() != "2")
                            .OrderBy(x => x.VHINCD)
                    );

                    if (KyoeiDatas.Any())
                    {
                        KyoeiItems.Value = new ObservableCollection<KyoeiItem>();
                        var KyoeiModelList = new KyoeiItemList();
                        var addItems = new ObservableCollection<KyoeiItem>(KyoeiModelList.ConvertKyoeiDataToModel(KyoeiDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<KyoeiItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = KyoeiItems.Value.Sum(x => x.数量).ToString();
                                  });
                            setItems.Add(item);
                        });
                        KyoeiItems.Value = new ObservableCollection<KyoeiItem>(setItems);
                        TotalMaisu.Value = KyoeiItems.Value.Sum(x => x.数量).ToString();
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
            return KyoeiItems.Value != null &&
                   KyoeiItems.Value.Any() &&
                   KyoeiItems.Value.Sum(x => x.数量) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.KYOEI + "_EOWPR01_" + DateTime.Today.ToString("yyyyMMddmmhhss") + ".csv";
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
            var list = KyoeiItems.Value.Where(x => x.数量 > 0).ToList();
            var csvColSort = new string[]
            {
                "分類コード",
                "伝票番号",
                "行番号",
                "京屋商品コード",
                "商品コード",
                "マーク",
                "品名",
                "規格",
                "文字予備７",
                "数量",
                "原単価",
                "売単価"
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
            var layName = NefudaBangouText.Value == 1 ? @"本体価格２１号ラベル.mllayx" : "本体価格１１号タグ.mllayx";
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

    public class KyoeiItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string 分類コード { get; set; }
        public string 伝票番号 { get; set; }       // 空で渡す
        public string 行番号 { get; set; }         // 空で渡す
        public string 京屋商品コード { get; set; }
        public string 商品コード { get; set; }
        public string マーク { get; set; }         // 空で渡す
        public string 品名 { get; set; }
        public string 規格 { get; set; }           // 空で渡す
        public string 文字予備７ { get; set; }   　// 空で渡す

        private decimal _数量;
        public decimal 数量
        {
            get { return _数量; }
            set
            {
                if (value != this._数量)
                {
                    this._数量 = value;
                    this.OnPropertyChanged("数量");
                }
            }
        }
        public decimal 原単価 { get; set; }
        public decimal 売単価 { get; set; }

        public KyoeiItem(string 分類コード, string 京屋商品コード, string 商品コード, string 品名,
                         decimal 数量, decimal 原単価, decimal 売単価, string 色コード)
        {
            this.分類コード = 分類コード;
            this.京屋商品コード = 京屋商品コード;
            this.商品コード = 商品コード;
            this.品名 = 品名;
            this.数量 = 数量;
            this.原単価 = 原単価;
            this.売単価 = 売単価;
        }
    }
    public class KyoeiItemList
    {
        public IEnumerable<KyoeiItem> ConvertKyoeiDataToModel(List<KyoeiData> datas)
        {
            var result = new List<KyoeiItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new KyoeiItem(data.VBUNCD, data.VHINCD, data.HINCD, data.VHINNMA, data.VSURYO, data.VGNKTK, data.VURITK, data.VCOLCD));
            });
            return result;
        }
    }
}
