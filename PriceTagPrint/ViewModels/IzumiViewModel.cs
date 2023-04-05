using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EmployeeInfoManage.Common;
using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Models;
using PriceTagPrint.Views;
using PriceTagPrint.WAG_USR1;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace PriceTagPrint.ViewModels
{
    public class IzumiViewModel : ViewModelsBase
    {
        #region プロパティ

        // 選択ファイルパス
        public ReactiveProperty<string> FilePathText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<ObservableCollection<IzumiItem>> IzumiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<IzumiItem>>();

        // 発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        #endregion

        private readonly string _grpName = @"0160_イズミ\イズミ【BMS値札メッセージ】";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\0160_イズミ\イズミ【BMS値札メッセージ】";
        private CsvUtility csvUtility = new CsvUtility();

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
                    break;
                case "F5":
                    if (InputCheck())
                    {
                        CsvReadDisplay();
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

        #region

        public IzumiViewModel()
        {
            tOKMTE_LIST = new TOKMTE_LIST();
        }

        #endregion

        #region ファンクション
        /// <summary>
        /// F4 初期化処理
        /// </summary>
        public void Clear()
        {
            TotalMaisu.Value = "";
            if (IzumiItems.Value != null && IzumiItems.Value.Any())
            {
                IzumiItems.Value.Clear();
            }
            if (File.Exists(CommonStrings.IZUMI_SCV_PATH))
            {
                FilePathText.Value = CommonStrings.IZUMI_SCV_PATH;
            }
            else
            {
                FilePathText.Value = "";
            }
        }

        /// <summary>
        /// F5検索入力チェック
        /// </summary>
        /// <returns></returns>
        public bool InputCheck()
        {
            if (!File.Exists(FilePathText.Value))
            {
                MessageBox.Show("選択ファイルが存在しません。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(FilePathText.Value))
            {
                MessageBox.Show("対象データが存在しません。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// F5読込処理
        /// </summary>
        public void CsvReadDisplay()
        {
            if (!InputCheck())
            {
                return;
            }
            var tmpItems = new List<IzumiItem>();
            ProcessingSplash ps = new ProcessingSplash("データ作成中...", () =>
            {
                var dt = csvUtility.ReadCSV(false, FilePathText.Value);
                if (dt.Rows.Count > 0)
                {
                    IzumiItems.Value = new ObservableCollection<IzumiItem>();
                    var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.IZUMI);

                    foreach (var row in dt.Rows)
                    {
                        var cols = ((System.Data.DataRow)row).ItemArray.Select(x => x.ToString().Replace("\"", "")).ToArray();
                        var item = new IzumiItem(cols, tokmteList);
                        tmpItems.Add(item);
                    }
                }
            });
            //バックグラウンド処理が終わるまで表示して待つ
            ps.ShowDialog();

            if (ps.complete)
            {
                if (tmpItems.Any())
                {
                    tmpItems.ToList().ForEach(item =>
                    {
                        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                              h => item.PropertyChanged += h,
                              h => item.PropertyChanged -= h)
                              .Subscribe(e =>
                              {
                                  // 発行枚数に変更があったら合計発行枚数も変更する
                                  TotalMaisu.Value = IzumiItems.Value.Sum(x => x.Col025).ToString();
                              });
                        IzumiItems.Value.Add(item);
                    });
                    TotalMaisu.Value = IzumiItems.Value.Sum(x => x.Col025).ToString();
                }
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
            return IzumiItems.Value != null &&
                   IzumiItems.Value.Any() &&
                   IzumiItems.Value.Sum(x => x.Col025) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.IZUMI + "_90604200_" + DateTime.Today.ToString("yyyyMMddmmhhss") + "_nefuda.csv";
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
            var list = IzumiItems.Value.Where(x => x.Col025 > 0).ToList();
            var datas = DataUtility.ToDataTable(list);
            new CsvUtility().Write(datas, fullName, false);
        }

        /// <summary>
        /// F10プレビュー・F12印刷 外部アプリ（MLV5）起動
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="isPreview"></param>
        private void NefudaOutput(string fname, bool isPreview)
        {
            // ※振分発行用ＰＧ
            var layName = @"70104-イズミ【BMS値札メッセージ】.mldenx";
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

    public class IzumiItem
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        const int WAGO_HINCD_COL = 126;
        const int IZUMI_EDA_COL1 = 135;
        const int IZUMI_EDA_COL2 = 134;

        [Order(Value = 1)] public string Col001 { get; set; }   // csvのみ
        [Order(Value = 2)] public string Col002 { get; set; }   // csvのみ
        [Order(Value = 3)] public string Col003 { get; set; }   // csvのみ
        [Order(Value = 4)] public string Col004 { get; set; }   // csvのみ
        [Order(Value = 5)] public string Col005 { get; set; }   // csvのみ
        [Order(Value = 6)] public string Col006 { get; set; }   // csvのみ
        [Order(Value = 7)] public string Col007 { get; set; }   // csvのみ
        [Order(Value = 8)] public string Col008 { get; set; }   // csvのみ
        [Order(Value = 9)] public string Col009 { get; set; }   // csvのみ
        [Order(Value = 10)] public string Col010 { get; set; }   // csvのみ
        [Order(Value = 11)] public string Col011 { get; set; }   // csvのみ
        [Order(Value = 12)] public string Col012 { get; set; }   // csvのみ
        [Order(Value = 13)] public string Col013 { get; set; }   // csvのみ
        [Order(Value = 14)] public string Col014 { get; set; }   // csvのみ
        [Order(Value = 15)] public string Col015 { get; set; }   // csvのみ
        [Order(Value = 16)] public string Col016 { get; set; }   // 発行依頼番号
        [Order(Value = 17)] public string Col017 { get; set; }   // 値札台紙
        [Order(Value = 18)] public string Col018 { get; set; }   // 値札用途
        [Order(Value = 19)] public string Col019 { get; set; }   // 値札受渡方法
        [Order(Value = 20)] public string Col020 { get; set; }   // 値札作成依頼日
        [Order(Value = 21)] public string Col021 { get; set; }   // 値札納入日
        [Order(Value = 22)] public string Col022 { get; set; }   // csvのみ
        [Order(Value = 23)] public string Col023 { get; set; }   // csvのみ
        [Order(Value = 24)] public string Col024 { get; set; }   // 値札依頼明細番号
        private decimal _col025;
        [Order(Value = 25)]
        public decimal Col025                                   // 発行枚数
        {
            get { return _col025; }
            set
            {
                if (value != this._col025)
                {
                    this._col025 = value;
                    this.OnPropertyChanged("Col025");
                }
            }
        }
        [Order(Value = 26)] public string Col026 { get; set; }   // csvのみ
        [Order(Value = 27)] public string Col027 { get; set; }   // csvのみ
        [Order(Value = 28)] public string Col028 { get; set; }   // csvのみ
        [Order(Value = 29)] public string Col029 { get; set; }   // csvのみ
        [Order(Value = 30)] public string Col030 { get; set; }   // 商品名(印刷用)
        [Order(Value = 31)] public string Col031 { get; set; }   // csvのみ
        [Order(Value = 32)] public string Col032 { get; set; }   // csvのみ
        [Order(Value = 33)] public string Col033 { get; set; }   // csvのみ
        [Order(Value = 34)] public string Col034 { get; set; }   // サイズ(印刷用)
        [Order(Value = 35)] public string Col035 { get; set; }   // 価格①(税込)
        [Order(Value = 36)] public string Col036 { get; set; }   // 価格①(税抜)
        [Order(Value = 37)] public string Col037 { get; set; }   // 価格②(税込)
        [Order(Value = 38)] public string Col038 { get; set; }   // 価格②(税抜)
        [Order(Value = 39)] public string Col039 { get; set; }   // ﾊﾞﾝﾄﾞﾙｺｰﾄﾞ(印刷用)
        [Order(Value = 40)] public string Col040 { get; set; }   // csvのみ
        [Order(Value = 41)] public string Col041 { get; set; }   // csvのみ
        [Order(Value = 42)] public string Col042 { get; set; }   // csvのみ
        [Order(Value = 43)] public string Col043 { get; set; }   // csvのみ
        [Order(Value = 44)] public string Col044 { get; set; }   // csvのみ
        [Order(Value = 45)] public string Col045 { get; set; }   // csvのみ
        [Order(Value = 46)] public string Col046 { get; set; }   // csvのみ
        [Order(Value = 47)] public string Col047 { get; set; }   // csvのみ
        [Order(Value = 48)] public string Col048 { get; set; }   // 印字内容１
        [Order(Value = 49)] public string Col049 { get; set; }   // csvのみ
        [Order(Value = 50)] public string Col050 { get; set; }   // csvのみ
        [Order(Value = 51)] public string Col051 { get; set; }   // csvのみ
        [Order(Value = 52)] public string Col052 { get; set; }   // csvのみ
        [Order(Value = 53)] public string Col053 { get; set; }   // csvのみ
        [Order(Value = 54)] public string Col054 { get; set; }   // csvのみ
        [Order(Value = 55)] public string Col055 { get; set; }   // csvのみ
        [Order(Value = 56)] public string Col056 { get; set; }   // csvのみ
        [Order(Value = 57)] public string Col057 { get; set; }   // csvのみ
        [Order(Value = 58)] public string Col058 { get; set; }   // csvのみ
        [Order(Value = 59)] public string Col059 { get; set; }   // csvのみ
        [Order(Value = 60)] public string Col060 { get; set; }   // csvのみ
        [Order(Value = 61)] public string Col061 { get; set; }   // csvのみ
        [Order(Value = 62)] public string Col062 { get; set; }   // csvのみ
        [Order(Value = 63)] public string Col063 { get; set; }   // csvのみ
        [Order(Value = 64)] public string Col064 { get; set; }   // csvのみ
        [Order(Value = 65)] public string Col065 { get; set; }   // csvのみ
        [Order(Value = 66)] public string Col066 { get; set; }   // csvのみ
        [Order(Value = 67)] public string Col067 { get; set; }   // csvのみ
        [Order(Value = 68)] public string Col068 { get; set; }   // csvのみ
        [Order(Value = 69)] public string Col069 { get; set; }   // csvのみ
        [Order(Value = 70)] public string Col070 { get; set; }   // csvのみ
        [Order(Value = 71)] public string Col071 { get; set; }   // csvのみ
        [Order(Value = 72)] public string Col072 { get; set; }   // csvのみ
        [Order(Value = 73)] public string Col073 { get; set; }   // csvのみ
        [Order(Value = 74)] public string Col074 { get; set; }   // csvのみ
        [Order(Value = 75)] public string Col075 { get; set; }   // csvのみ
        [Order(Value = 76)] public string Col076 { get; set; }   // csvのみ
        [Order(Value = 77)] public string Col077 { get; set; }   // csvのみ
        [Order(Value = 78)] public string Col078 { get; set; }   // csvのみ
        [Order(Value = 79)] public string Col079 { get; set; }   // csvのみ
        [Order(Value = 80)] public string Col080 { get; set; }   // csvのみ
        [Order(Value = 81)] public string Col081 { get; set; }   // csvのみ
        [Order(Value = 82)] public string Col082 { get; set; }   // csvのみ
        [Order(Value = 83)] public string Col083 { get; set; }   // csvのみ
        [Order(Value = 84)] public string Col084 { get; set; }   // csvのみ
        [Order(Value = 85)] public string Col085 { get; set; }   // csvのみ
        [Order(Value = 86)] public string Col086 { get; set; }   // csvのみ
        [Order(Value = 87)] public string Col087 { get; set; }   // csvのみ
        [Order(Value = 88)] public string Col088 { get; set; }   // csvのみ
        [Order(Value = 89)] public string Col089 { get; set; }   // csvのみ
        [Order(Value = 90)] public string Col090 { get; set; }   // csvのみ
        [Order(Value = 91)] public string Col091 { get; set; }   // csvのみ
        [Order(Value = 92)] public string Col092 { get; set; }   // csvのみ
        [Order(Value = 93)] public string Col093 { get; set; }   // csvのみ
        [Order(Value = 94)] public string Col094 { get; set; }   // csvのみ
        [Order(Value = 95)] public string Col095 { get; set; }   // csvのみ
        [Order(Value = 96)] public string Col096 { get; set; }   // csvのみ
        [Order(Value = 97)] public string Col097 { get; set; }   // csvのみ
        [Order(Value = 98)] public string Col098 { get; set; }   // csvのみ
        [Order(Value = 99)] public string Col099 { get; set; }   // csvのみ
        [Order(Value = 100)] public string Col100 { get; set; }  // csvのみ
        [Order(Value = 101)] public string Col101 { get; set; }  // csvのみ
        [Order(Value = 102)] public string Col102 { get; set; }  // csvのみ
        [Order(Value = 103)] public string Col103 { get; set; }  // csvのみ
        [Order(Value = 104)] public string Col104 { get; set; }  // csvのみ
        [Order(Value = 105)] public string Col105 { get; set; }  // csvのみ
        [Order(Value = 106)] public string Col106 { get; set; }  // csvのみ
        [Order(Value = 107)] public string Col107 { get; set; }  // csvのみ
        [Order(Value = 108)] public string Col108 { get; set; }  // csvのみ
        [Order(Value = 109)] public string Col109 { get; set; }  // csvのみ
        [Order(Value = 110)] public string Col110 { get; set; }  // csvのみ
        [Order(Value = 111)] public string Col111 { get; set; }  // csvのみ
        [Order(Value = 112)] public string Col112 { get; set; }  // csvのみ
        [Order(Value = 113)] public string Col113 { get; set; }  // csvのみ
        [Order(Value = 114)] public string Col114 { get; set; }  // csvのみ
        [Order(Value = 115)] public string Col115 { get; set; }  // csvのみ
        [Order(Value = 116)] public string Col116 { get; set; }  // csvのみ
        [Order(Value = 117)] public string Col117 { get; set; }  // csvのみ
        [Order(Value = 118)] public string Col118 { get; set; }  // csvのみ
        [Order(Value = 119)] public string Col119 { get; set; }  // csvのみ
        [Order(Value = 120)] public string Col120 { get; set; }  // 商品分類(大)
        [Order(Value = 121)] public string Col121 { get; set; }  // 商品分類(中)
        [Order(Value = 122)] public string Col122 { get; set; }  // 商品分類(小)
        [Order(Value = 123)] public string Col123 { get; set; }  // 商品分類(細)
        [Order(Value = 124)] public string Col124 { get; set; }  // csvのみ
        [Order(Value = 125)] public string Col125 { get; set; }  // csvのみ
        [Order(Value = 126)] public string Col126 { get; set; }  // csvのみ
        [Order(Value = 127)] public string Col127 { get; set; }  // 和合商品コード
        [Order(Value = 128)] public string Col128 { get; set; }  // csvのみ
        [Order(Value = 129)] public string Col129 { get; set; }  // csvのみ
        [Order(Value = 130)] public string Col130 { get; set; }  // csvのみ
        [Order(Value = 131)] public string Col131 { get; set; }  // csvのみ
        [Order(Value = 132)] public string Col132 { get; set; }  // csvのみ
        [Order(Value = 133)] public string Col133 { get; set; }  // csvのみ
        [Order(Value = 134)] public string Col134 { get; set; }  // csvのみ
        [Order(Value = 135)] public string Col135 { get; set; }  // 相手品番②
        [Order(Value = 136)] public string Col136 { get; set; }  // 相手品番①
        [Order(Value = 137)] public string Col137 { get; set; }  // csvのみ
        [Order(Value = 138)] public string Col138 { get; set; }  // csvのみ
        [Order(Value = 139)] public string Col139 { get; set; }  // csvのみ
        [Order(Value = 140)] public string Col140 { get; set; }  // csvのみ
        public IzumiItem(string[] cols, List<TOKMTE> tokmtes)
        {
            var aitHinban = cols[WAGO_HINCD_COL].ToString().TrimEnd() + "-" +
                                cols[IZUMI_EDA_COL1].ToString().TrimEnd() + "-" +
                                cols[IZUMI_EDA_COL2].ToString().TrimEnd();
            cols[WAGO_HINCD_COL] = tokmtes.FirstOrDefault(x => x.DATKB == "1" && x.EOSHINID.TrimEnd() == aitHinban)?.HINCD.TrimEnd() ??
                                cols[WAGO_HINCD_COL].ToString().TrimEnd();

            Col001 = cols[0]?.ToString().Replace("\"", "") ?? "";
            Col002 = cols[1]?.ToString().Replace("\"", "") ?? "";
            Col003 = cols[2]?.ToString().Replace("\"", "") ?? "";
            Col004 = cols[3]?.ToString().Replace("\"", "") ?? "";
            Col005 = cols[4]?.ToString().Replace("\"", "") ?? "";
            Col006 = cols[5]?.ToString().Replace("\"", "") ?? "";
            Col007 = cols[6]?.ToString().Replace("\"", "") ?? "";
            Col008 = cols[7]?.ToString().Replace("\"", "") ?? "";
            Col009 = cols[8]?.ToString().Replace("\"", "") ?? "";
            Col010 = cols[9]?.ToString().Replace("\"", "") ?? "";
            Col011 = cols[10]?.ToString().Replace("\"", "") ?? "";
            Col012 = cols[11]?.ToString().Replace("\"", "") ?? "";
            Col013 = cols[12]?.ToString().Replace("\"", "") ?? "";
            Col014 = cols[13]?.ToString().Replace("\"", "") ?? "";
            Col015 = cols[14]?.ToString().Replace("\"", "") ?? "";
            Col016 = cols[15]?.ToString().Replace("\"", "") ?? "";
            Col017 = cols[16]?.ToString().Replace("\"", "") ?? "";
            Col018 = cols[17]?.ToString().Replace("\"", "") ?? "";
            Col019 = cols[18]?.ToString().Replace("\"", "") ?? "";
            Col020 = cols[19]?.ToString().Replace("\"", "") ?? "";
            Col021 = cols[20]?.ToString().Replace("\"", "") ?? "";
            Col022 = cols[21]?.ToString().Replace("\"", "") ?? "";
            Col023 = cols[22]?.ToString().Replace("\"", "") ?? "";
            Col024 = cols[23]?.ToString().Replace("\"", "") ?? "";
            Col025 = int.Parse(cols[24]?.ToString().Replace("\"", "") ?? "0");
            Col026 = cols[25]?.ToString().Replace("\"", "") ?? "";
            Col027 = cols[26]?.ToString().Replace("\"", "") ?? "";
            Col028 = cols[27]?.ToString().Replace("\"", "") ?? "";
            Col029 = cols[28]?.ToString().Replace("\"", "") ?? "";
            Col030 = cols[29]?.ToString().Replace("\"", "") ?? "";
            Col031 = cols[30]?.ToString().Replace("\"", "") ?? "";
            Col032 = cols[31]?.ToString().Replace("\"", "") ?? "";
            Col033 = cols[32]?.ToString().Replace("\"", "") ?? "";
            Col034 = cols[33]?.ToString().Replace("\"", "") ?? "";
            Col035 = cols[34]?.ToString().Replace("\"", "") ?? "";
            Col036 = cols[35]?.ToString().Replace("\"", "") ?? "";
            Col037 = cols[36]?.ToString().Replace("\"", "") ?? "";
            Col038 = cols[37]?.ToString().Replace("\"", "") ?? "";
            Col039 = cols[38]?.ToString().Replace("\"", "") ?? "";
            Col040 = cols[39]?.ToString().Replace("\"", "") ?? "";
            Col041 = cols[40]?.ToString().Replace("\"", "") ?? "";
            Col042 = cols[41]?.ToString().Replace("\"", "") ?? "";
            Col043 = cols[42]?.ToString().Replace("\"", "") ?? "";
            Col044 = cols[43]?.ToString().Replace("\"", "") ?? "";
            Col045 = cols[44]?.ToString().Replace("\"", "") ?? "";
            Col046 = cols[45]?.ToString().Replace("\"", "") ?? "";
            Col047 = cols[46]?.ToString().Replace("\"", "") ?? "";
            Col048 = cols[47]?.ToString().Replace("\"", "") ?? "";
            Col049 = cols[48]?.ToString().Replace("\"", "") ?? "";
            Col050 = cols[49]?.ToString().Replace("\"", "") ?? "";
            Col051 = cols[50]?.ToString().Replace("\"", "") ?? "";
            Col052 = cols[51]?.ToString().Replace("\"", "") ?? "";
            Col053 = cols[52]?.ToString().Replace("\"", "") ?? "";
            Col054 = cols[53]?.ToString().Replace("\"", "") ?? "";
            Col055 = cols[54]?.ToString().Replace("\"", "") ?? "";
            Col056 = cols[55]?.ToString().Replace("\"", "") ?? "";
            Col057 = cols[56]?.ToString().Replace("\"", "") ?? "";
            Col058 = cols[57]?.ToString().Replace("\"", "") ?? "";
            Col059 = cols[58]?.ToString().Replace("\"", "") ?? "";
            Col060 = cols[59]?.ToString().Replace("\"", "") ?? "";
            Col061 = cols[60]?.ToString().Replace("\"", "") ?? "";
            Col062 = cols[61]?.ToString().Replace("\"", "") ?? "";
            Col063 = cols[62]?.ToString().Replace("\"", "") ?? "";
            Col064 = cols[63]?.ToString().Replace("\"", "") ?? "";
            Col065 = cols[64]?.ToString().Replace("\"", "") ?? "";
            Col066 = cols[65]?.ToString().Replace("\"", "") ?? "";
            Col067 = cols[66]?.ToString().Replace("\"", "") ?? "";
            Col068 = cols[67]?.ToString().Replace("\"", "") ?? "";
            Col069 = cols[68]?.ToString().Replace("\"", "") ?? "";
            Col070 = cols[69]?.ToString().Replace("\"", "") ?? "";
            Col071 = cols[70]?.ToString().Replace("\"", "") ?? "";
            Col072 = cols[71]?.ToString().Replace("\"", "") ?? "";
            Col073 = cols[72]?.ToString().Replace("\"", "") ?? "";
            Col074 = cols[73]?.ToString().Replace("\"", "") ?? "";
            Col075 = cols[74]?.ToString().Replace("\"", "") ?? "";
            Col076 = cols[75]?.ToString().Replace("\"", "") ?? "";
            Col077 = cols[76]?.ToString().Replace("\"", "") ?? "";
            Col078 = cols[77]?.ToString().Replace("\"", "") ?? "";
            Col079 = cols[78]?.ToString().Replace("\"", "") ?? "";
            Col080 = cols[79]?.ToString().Replace("\"", "") ?? "";
            Col081 = cols[80]?.ToString().Replace("\"", "") ?? "";
            Col082 = cols[81]?.ToString().Replace("\"", "") ?? "";
            Col083 = cols[82]?.ToString().Replace("\"", "") ?? "";
            Col084 = cols[83]?.ToString().Replace("\"", "") ?? "";
            Col085 = cols[84]?.ToString().Replace("\"", "") ?? "";
            Col086 = cols[85]?.ToString().Replace("\"", "") ?? "";
            Col087 = cols[86]?.ToString().Replace("\"", "") ?? "";
            Col088 = cols[87]?.ToString().Replace("\"", "") ?? "";
            Col089 = cols[88]?.ToString().Replace("\"", "") ?? "";
            Col090 = cols[89]?.ToString().Replace("\"", "") ?? "";
            Col091 = cols[90]?.ToString().Replace("\"", "") ?? "";
            Col092 = cols[91]?.ToString().Replace("\"", "") ?? "";
            Col093 = cols[92]?.ToString().Replace("\"", "") ?? "";
            Col094 = cols[93]?.ToString().Replace("\"", "") ?? "";
            Col095 = cols[94]?.ToString().Replace("\"", "") ?? "";
            Col096 = cols[95]?.ToString().Replace("\"", "") ?? "";
            Col097 = cols[96]?.ToString().Replace("\"", "") ?? "";
            Col098 = cols[97]?.ToString().Replace("\"", "") ?? "";
            Col099 = cols[98]?.ToString().Replace("\"", "") ?? "";
            Col100 = cols[99]?.ToString().Replace("\"", "") ?? "";
            Col101 = cols[100]?.ToString().Replace("\"", "") ?? "";
            Col102 = cols[101]?.ToString().Replace("\"", "") ?? "";
            Col103 = cols[102]?.ToString().Replace("\"", "") ?? "";
            Col104 = cols[103]?.ToString().Replace("\"", "") ?? "";
            Col105 = cols[104]?.ToString().Replace("\"", "") ?? "";
            Col106 = cols[105]?.ToString().Replace("\"", "") ?? "";
            Col107 = cols[106]?.ToString().Replace("\"", "") ?? "";
            Col108 = cols[107]?.ToString().Replace("\"", "") ?? "";
            Col109 = cols[108]?.ToString().Replace("\"", "") ?? "";
            Col110 = cols[109]?.ToString().Replace("\"", "") ?? "";
            Col111 = cols[110]?.ToString().Replace("\"", "") ?? "";
            Col112 = cols[111]?.ToString().Replace("\"", "") ?? "";
            Col113 = cols[112]?.ToString().Replace("\"", "") ?? "";
            Col114 = cols[113]?.ToString().Replace("\"", "") ?? "";
            Col115 = cols[114]?.ToString().Replace("\"", "") ?? "";
            Col116 = cols[115]?.ToString().Replace("\"", "") ?? "";
            Col117 = cols[116]?.ToString().Replace("\"", "") ?? "";
            Col118 = cols[117]?.ToString().Replace("\"", "") ?? "";
            Col119 = cols[118]?.ToString().Replace("\"", "") ?? "";
            Col120 = cols[119]?.ToString().Replace("\"", "") ?? "";
            Col121 = cols[120]?.ToString().Replace("\"", "") ?? "";
            Col122 = cols[121]?.ToString().Replace("\"", "") ?? "";
            Col123 = cols[122]?.ToString().Replace("\"", "") ?? "";
            Col124 = cols[123]?.ToString().Replace("\"", "") ?? "";
            Col125 = cols[124]?.ToString().Replace("\"", "") ?? "";
            Col126 = cols[125]?.ToString().Replace("\"", "") ?? "";
            Col127 = cols[126]?.ToString().Replace("\"", "") ?? "";
            Col128 = cols[127]?.ToString().Replace("\"", "") ?? "";
            Col129 = cols[128]?.ToString().Replace("\"", "") ?? "";
            Col130 = cols[129]?.ToString().Replace("\"", "") ?? "";
            Col131 = cols[130]?.ToString().Replace("\"", "") ?? "";
            Col132 = cols[131]?.ToString().Replace("\"", "") ?? "";
            Col133 = cols[132]?.ToString().Replace("\"", "") ?? "";
            Col134 = cols[133]?.ToString().Replace("\"", "") ?? "";
            Col135 = cols[134]?.ToString().Replace("\"", "") ?? "";
            Col136 = cols[135]?.ToString().Replace("\"", "") ?? "";
            Col137 = cols[136]?.ToString().Replace("\"", "") ?? "";
            Col138 = cols[137]?.ToString().Replace("\"", "") ?? "";
            Col139 = cols[138]?.ToString().Replace("\"", "") ?? "";
            Col140 = cols[139]?.ToString().Replace("\"", "") ?? "";
        }
    }
}
