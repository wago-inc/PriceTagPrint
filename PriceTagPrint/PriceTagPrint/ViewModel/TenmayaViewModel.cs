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
    public class TenmayaViewModel : ViewModelsBase
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
        public ReactiveProperty<string> VbunCdText { get; set; }

        // 和合分類コード
        public ReactiveProperty<string> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 開始相手品番
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了相手品番
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<TenmayaData> TenmayaDatas { get; set; } = new List<TenmayaData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<TenmayaItem>> TenmayaItems { get; set; }
                = new ReactiveProperty<ObservableCollection<TenmayaItem>>();

        #endregion

        private readonly string _grpName = @"\天満屋ストア値札発行_V5_ST308R";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private EOSKNMTA_LIST eOSKNMTA_LIST;
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
        public TenmayaViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            eOSKNMTA_LIST = new EOSKNMTA_LIST();

            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            VbunCdText = new ReactiveProperty<string>("");
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>();

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));

            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

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
            item1.Name = "1：天満屋ストア_貼札_プロパー";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：天満屋ストア_下札_プロパー";
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
            NouhinDate.Value = DateTime.Today.AddDays(1);
            SelectedHakkouTypeIndex.Value = 0;
            VbunCdText.Value = "";
            BunruiCodeText.Value = "";
            SelectedNefudaBangouIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            TotalMaisu.Value = "";
            TenmayaDatas.Clear();
            if (TenmayaItems.Value != null && TenmayaItems.Value.Any())
            {
                TenmayaItems.Value.Clear();
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
            if (string.IsNullOrEmpty(this.VbunCdText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!string.IsNullOrEmpty(this.BunruiCodeText.Value) && !BunruiCodeItems.Value.Select(x => x.Id.TrimEnd()).Contains(this.BunruiCodeText.Value))
            {
                MessageBox.Show("和合分類を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.HakkouTypeTextBox.Focus();
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
        /// F5検索処理
        /// </summary>
        public void NefudaDataDisplay()
        {
            ProcessingSplash ps = new ProcessingSplash("データ作成中...", () =>
            {
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.TENMAYA, JusinDate.Value, NouhinDate.Value);
                var easKnmtaList = eOSKNMTA_LIST.QueryWhereTcode(TidNum.TENMAYA);

                if (eosJutraList.Any() && easKnmtaList.Any())
                {
                    var innerJoinData = eosJutraList
                            .Join(
                                   easKnmtaList,
                                   e1 => new
                                   {
                                       VRYOHNCD = e1.VRYOHNCD.ToString()
                                   },
                                   e2 => new
                                   {
                                       VRYOHNCD = e2.VRYOHNCD.ToString()
                                   },
                                   (eosj, eosm) => new
                                   {
                                       VRYOHNCD = eosj.VRYOHNCD,
                                       VRYOHNNM = eosm.VRYOHNNM,
                                       VRCVDT = eosj.VRCVDT,
                                       VNOHINDT = eosj.VNOHINDT,
                                       DATNO = eosj.DATNO,
                                       VROWNO = eosj.VROWNO,
                                       VBUNCD = eosj.VBUNCD,
                                       VHINCD = eosj.VHINCD.TrimEnd(),
                                       VHINNMA = eosj.VHINNMA.TrimEnd(),
                                       HINCD = eosj.HINCD.TrimEnd(),
                                       VCYOBI7 = eosj.VCYOBI7.TrimEnd(),
                                       QOLTORID = eosj.QOLTORID.TrimEnd(),
                                       VURITK = eosj.VURITK,
                                       VSURYO = eosj.VSURYO,
                                       VSIZNM = eosj.VSIZNM.TrimEnd(),
                                       VCOLNM = eosj.VCOLNM.TrimEnd(),
                                   })
                            .Where(x => x.VBUNCD.TrimEnd() == this.VbunCdText.Value)
                            .Where(x => !string.IsNullOrEmpty(this.BunruiCodeText.Value) ? x.HINCD.StartsWith(this.BunruiCodeText.Value) : true)
                            .OrderBy(x => x.VRYOHNCD)
                                    .ThenBy(x => x.VRCVDT)
                                    .ThenBy(x => x.VHINCD);

                    if (innerJoinData.Any() && hinmtaList.Any())
                    {
                        var dateNow = DateTime.Now;
                        if (hinmtaList.Any())
                        {
                            int sttHincd;
                            int endHincd;
                            int aitSttHincd;
                            int aitEndHincd;
                            TenmayaDatas.Clear();
                            TenmayaDatas.AddRange(
                                innerJoinData
                                    .GroupJoin(
                                           hinmtaList,
                                           e => new
                                           {
                                               HINCD = e.HINCD.ToString().TrimEnd(),
                                           },
                                           h => new
                                           {
                                               HINCD = h.HINCD.TrimEnd(),
                                           },
                                           (a, hin) => new
                                           {
                                               VRYOHNCD = a.VRYOHNCD,
                                               VRYOHNNM = a.VRYOHNNM,
                                               VRCVDT = a.VRCVDT,
                                               VNOHINDT = a.VNOHINDT,
                                               VCYOBI7 = a.VCYOBI7,
                                               HINBAN = !string.IsNullOrEmpty(a.VCYOBI7) && a.VCYOBI7.Contains("-") ?
                                                            a.VCYOBI7.Substring(0, a.VCYOBI7.IndexOf("-")) : "",
                                               VURITK = a.VURITK,
                                               JANCD = a.VHINCD,
                                               QOLTORID = a.QOLTORID,
                                               HINCD = a.HINCD,
                                               HINNM = hin.Any() ? hin.FirstOrDefault().HINNMA : "",
                                               VSURYO = a.VSURYO,
                                               VSIZNM = a.VSIZNM,
                                               VCOLNM = a.VCOLNM,
                                           })
                                    .GroupBy(a => new
                                    {
                                        a.VRYOHNCD,
                                        a.VRYOHNNM,
                                        a.VRCVDT,
                                        a.VNOHINDT,
                                        a.VCYOBI7,
                                        a.HINBAN,
                                        a.VURITK,
                                        a.JANCD,
                                        a.QOLTORID,
                                        a.HINCD,
                                        a.HINNM,
                                        a.VSIZNM,
                                        a.VCOLNM,
                                    })
                                    .Select(g => new TenmayaData()
                                    {
                                        VRYOHNCD = g.Key.VRYOHNCD,
                                        VRYOHNNM = g.Key.VRYOHNNM,
                                        VRCVDT = g.Key.VRCVDT,
                                        VNOHINDT = g.Key.VNOHINDT,
                                        VSURYO = g.Sum(y => y.VSURYO),
                                        VCYOBI7 = g.Key.VCYOBI7,
                                        HINBAN = g.Key.HINBAN,
                                        VURITK = g.Key.VURITK,
                                        VHINCD = g.Key.JANCD,
                                        QOLTORID = g.Key.QOLTORID,
                                        HINCD = g.Key.HINCD,
                                        HINNM = g.Key.HINNM,
                                        VSIZNM = g.Key.VSIZNM,
                                        VCOLNM = g.Key.VCOLNM,
                                    })
                                    .Where(x => !string.IsNullOrEmpty(this.SttHincd.Value) ?
                                                    int.TryParse(this.SttHincd.Value, out sttHincd) &&
                                                    int.TryParse(x.HINBAN, out aitSttHincd) ?
                                                        aitSttHincd >= sttHincd : true
                                                : true)
                                    .Where(x => !string.IsNullOrEmpty(this.EndHincd.Value) ?
                                                    int.TryParse(this.EndHincd.Value, out endHincd) &&
                                                    int.TryParse(x.HINBAN, out aitEndHincd) ?
                                                        aitEndHincd <= endHincd : true
                                                : true)
                                    .OrderBy(x => x.VRYOHNCD)
                                    .ThenBy(x => x.VRCVDT)
                                    .ThenBy(x => x.VHINCD)
                                    );
                        }

                        if (TenmayaDatas.Any())
                        {
                            TenmayaItems.Value = new ObservableCollection<TenmayaItem>();
                            var TenmayaModelList = new TenmayaItemList();
                            var addItems = new ObservableCollection<TenmayaItem>(TenmayaModelList.ConvertTenmayaDataToModel(TenmayaDatas)).ToList();
                            // 直接ObservableにAddするとなぜか落ちるためListをかます。
                            var setItems = new List<TenmayaItem>();
                            addItems.ForEach(item =>
                            {
                                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                      h => item.PropertyChanged += h,
                                      h => item.PropertyChanged -= h)
                                      .Subscribe(e =>
                                      {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = TenmayaItems.Value.Sum(x => x.発行枚数).ToString();
                                      });
                                setItems.Add(item);
                            });
                            TenmayaItems.Value = new ObservableCollection<TenmayaItem>(setItems);
                            TotalMaisu.Value = TenmayaItems.Value.Sum(x => x.発行枚数).ToString();
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
            return TenmayaItems.Value != null &&
                   TenmayaItems.Value.Any() &&
                   TenmayaItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.TENMAYA + "_" +
                        this.JusinDate.Value.ToString("yyyyMMdd") + "_" +
                        this.NouhinDate.Value.ToString("yyyyMMdd") + ".csv";
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
            var list = TenmayaItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "取引先コード",
                "EOSコード",
                "サイズ",
                "カラー",
                "販促",
                "略号",
                "フリー入力",
                "本体価格",
                "JANコード",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用サイズ");
            datas.Columns.Remove("表示用カラー");
            datas.Columns.Remove("表示用商品コード");
            datas.Columns.Remove("表示用商品名");
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
                            ? @"01_天満屋ストア【手入力】_貼札_プロパー.mllayx"
                            : @"02_天満屋ストア【手入力】_下札_プロパー.mllayx";
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

    public class TenmayaItem : INotifyPropertyChanged
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
        public decimal 発行枚数 //csv & 表示
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
        public string 取引先コード { get; set; }     //csv
        public string EOSコード { get; set; }     //csv & 表示
        public string サイズ { get; set; }  //csv
        public string 表示用サイズ { get; set; }  //表示
        public string カラー { get; set; }  //csv
        public string 表示用カラー { get; set; }  //表示
        public string 販促 { get; set; }  //csv　空白でOK
        public string 略号 { get; set; } //csv　固定で"WU"
        public string フリー入力 { get; set; } //csv 品番 + 枝番
        public string 表示用商品コード { get; set; }    //表示
        public decimal 本体価格 { get; set; }  //csv & 表示 税抜き売価に変換して表示
        public string JANコード { get; set; }   //csv & 表示
        public string 表示用商品名 { get; set; }   //表示

        public TenmayaItem(decimal 発行枚数, string 取引先コード, string EOSコード, string サイズ, string 表示用サイズ, string カラー,
                            string 表示用カラー, string 販促, string 略号, string フリー入力, string 表示用商品コード, decimal 本体価格,
                            string JANコード, string 表示用商品名)
        {
            this.発行枚数 = 発行枚数;
            this.取引先コード = 取引先コード;
            this.EOSコード = EOSコード;
            this.サイズ = サイズ;
            this.表示用サイズ = 表示用サイズ;
            this.カラー = カラー;
            this.表示用カラー = 表示用カラー;
            this.販促 = 販促;
            this.略号 = 略号;
            this.フリー入力 = フリー入力;
            this.表示用商品コード = 表示用商品コード;
            this.本体価格 = 本体価格;
            this.JANコード = JANコード;
            this.表示用商品名 = 表示用商品名;
        }
    }

    public class TenmayaItemList
    {
        public IEnumerable<TenmayaItem> ConvertTenmayaDataToModel(List<TenmayaData> datas)
        {
            var result = new List<TenmayaItem>();
            var henkanList = new TenmayaHenkanList();
            TenmayaHenkan shenkan;
            TenmayaHenkan chenkan;
            decimal zeinuki = 0m;
            string dispSize = "";
            string dispColor = "";
            var zeiritsu = Zeiritsu.items.FirstOrDefault(x => x.SttDate <= DateTime.Today && DateTime.Today <= x.EndDate)?.Kakeritsu ?? 1;
            datas.ForEach(data =>
            {
                shenkan = henkanList.GetSizeHenkanchiByName(data.VSIZNM, new TenmayaHenkan("", data.VSIZNM));
                chenkan = henkanList.GetColorHenkanchiByName(data.VCOLNM, new TenmayaHenkan("", data.VCOLNM));
                zeinuki = data.VURITK / zeiritsu;
                dispSize = shenkan.変換値 + " " + shenkan.変換A;
                dispColor = chenkan.変換値 + " " + chenkan.変換A;
                result.Add(
                    new TenmayaItem(data.VSURYO, data.QOLTORID, "", shenkan.変換値, dispSize, chenkan.変換値, dispColor,
                                    "", "WU", data.VCYOBI7, data.HINCD, zeinuki, data.VHINCD, data.HINNM));
            });
            return result;
        }
    }
}
