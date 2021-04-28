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
using PriceTagPrint.WAG_USR1;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class KyoueiViewModel : ViewModelsBase
    {
        #region プロパティ

        // 受信日
        public ReactiveProperty<DateTime> JusinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today);

        // 納品日
        public ReactiveProperty<DateTime> NouhinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today.AddDays(1));

        // 選択ファイルパス
        public ReactiveProperty<string> FilePathText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<ObservableCollection<KyoeiItem>> KyoeiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<KyoeiItem>>();

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        #endregion

        private readonly string _grpName = @"0101_キョーエイ\【総額表示】_V5_ST308R";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\0101_キョーエイ\【総額表示】_V5_ST308R";
        private CsvUtility csvUtility = new CsvUtility();

        // 値札テキストボックス
        public TextBox NefudaBangouTextBox = null;
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

        public KyoueiViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            tOKMTE_LIST = new TOKMTE_TSURI_LIST();

            CreateComboItems();

            KyoeiItems.Value = new ObservableCollection<KyoeiItem>();
            NefudaBangouText = new ReactiveProperty<int>(0);

            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

            //FilePathSetExists(CommonStrings.KYOEI_SCV_PATH);            

        }

        /// <summary>
        /// コンボボックスItem生成
        /// </summary>
        public void CreateComboItems()
        {
            NefudaBangouItems.Value = new ObservableCollection<CommonIdName>(CreateNefudaBangouItems());
        }

        /// <summary>
        /// 値札番号Items生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateNefudaBangouItems()
        {
            var searchExtension = "*." + CommonStrings.INPUT_EXTENSION;
            var files = Directory.EnumerateFiles(_grpFullName, searchExtension);
            var list = new List<CommonIdName>();
            var id = 0;
            foreach (var file in files)
            {
                var fName = Path.GetFileNameWithoutExtension(file);
                var item = new CommonIdName();
                item.Id = id;
                item.Name = fName;
                list.Add(item);
                id++;
            }
            return list;
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

        public void FilePathSetExists(string path)
        {
            if (File.Exists(path))
            {
                FilePathText.Value = CommonStrings.KYOEI_SCV_PATH;
            }
            else
            {
                MessageBox.Show("EOWPR01.CSVファイルが存在しません。確認してください。", "ファイル存在チェック", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region ファンクション
        /// <summary>
        /// F4 初期化処理
        /// </summary>
        public void Clear()
        {
            if (File.Exists(CommonStrings.KYOEI_SCV_PATH))
            {
                FilePathText.Value = CommonStrings.KYOEI_SCV_PATH;
            }
            else
            {
                FilePathText.Value = "";
            }
            JusinDate.Value = DateTime.Today;
            NouhinDate.Value = DateTime.Today.AddDays(1);
            SelectedNefudaBangouIndex.Value = 0;
            KyoeiItems.Value.Clear();
            TotalMaisu.Value = "";
            NefudaBangouTextBox.Focus();
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
            if (this.NefudaBangouText.Value < 0 || this.NefudaBangouText.Value > 1)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            
            var dt = csvUtility.ReadCSV(true, FilePathText.Value);
            
            if (dt.Rows.Count > 0)
            {
                KyoeiItems.Value.Clear();

                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.KYOEI, JusinDate.Value, NouhinDate.Value);
                //todo:元のプログラムがゴミなのでTOKMTEをwag_usr1ではなく値札出力のアクセスに定義している。
                // 対策：wag_usr1にTOKMTEをコピーして吊り札判定に特化したTOKMTE_TSURIを作成した。
                var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.KYOEI);
                if(eosJutraList.Any() && tokmteList.Any())
                {
                    var turifudaHincds = eosJutraList
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
                                   HINCD = eos.HINCD,
                                   SIZCD = tok.Any() ? tok.FirstOrDefault().SIZCD.TrimStart(new Char[] { '0' }).TrimEnd() : "",
                               }).OrderBy(x => x.HINCD)
                               .Where(x => x.SIZCD == "2")
                               .Select(x => x.HINCD.TrimEnd())
                               .ToList();

                    foreach (DataRow row in dt.Rows)
                    {
                       // 0:１１号吊り 1:２１号貼り
                       var addFlg = NefudaBangouText.Value == 0 ?
                                   turifudaHincds.Contains(row.Field<string>("商品コード_02").TrimEnd()) :
                                   !turifudaHincds.Contains(row.Field<string>("商品コード_02").TrimEnd());
                        if (addFlg)
                        {
                            var arr = row.ItemArray.Cast<string>().ToArray();

                            KyoeiItems.Value.Add(
                                new KyoeiItem
                                (
                                    arr.ElementAtOrDefault(0),
                                    arr.ElementAtOrDefault(1),
                                    arr.ElementAtOrDefault(2),
                                    arr.ElementAtOrDefault(3),
                                    arr.ElementAtOrDefault(4),
                                    arr.ElementAtOrDefault(5),
                                    arr.ElementAtOrDefault(6),
                                    arr.ElementAtOrDefault(7),
                                    arr.ElementAtOrDefault(8),
                                    arr.ElementAtOrDefault(9),
                                    arr.ElementAtOrDefault(10),
                                    arr.ElementAtOrDefault(11),
                                    arr.ElementAtOrDefault(12),
                                    arr.ElementAtOrDefault(13),
                                    arr.ElementAtOrDefault(14),
                                    arr.ElementAtOrDefault(15),
                                    arr.ElementAtOrDefault(16),
                                    arr.ElementAtOrDefault(17),
                                    arr.ElementAtOrDefault(18),
                                    arr.ElementAtOrDefault(19),
                                    arr.ElementAtOrDefault(20),
                                    arr.ElementAtOrDefault(21),
                                    arr.ElementAtOrDefault(22),
                                    arr.ElementAtOrDefault(23),
                                    arr.ElementAtOrDefault(24),
                                    arr.ElementAtOrDefault(25)
                                    )
                                );
                        }
                    }
                }
                else
                {
                    MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (KyoeiItems.Value.Any())
                {
                    TotalMaisu.Value = KyoeiItems.Value.Sum(x => x.数量).ToString();
                }
                else
                {
                    MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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

            var datas = DataUtility.ToDataTable(list);
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
            var layName = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == NefudaBangouText.Value)?.Name + "." + CommonStrings.INPUT_EXTENSION ?? "";
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

    public class KyoeiItem
    {
        public string 分類コード { get; set; }
        public string 伝票番号 { get; set; }
        public string 行番号 { get; set; }
        public string 京屋商品コード { get; set; }
        public string 商品コード { get; set; }
        public string マーク { get; set; }
        public string 品名 { get; set; }
        public string 規格 { get; set; }
        public string 文字予備７ { get; set; }
        public int 数量 { get; set; }
        public string 原単価 { get; set; }
        public string 売単価 { get; set; }
        public string 店コード1 { get; set; }
        public string 店コード2 { get; set; }
        public string 店コード3 { get; set; }
        public string 店コード4 { get; set; }
        public string 店コード5 { get; set; }
        public string 店コード6 { get; set; }
        public string 受注数量1 { get; set; }
        public string 受注数量2 { get; set; }
        public string 受注数量3 { get; set; }
        public string 受注数量4 { get; set; }
        public string 受注数量5 { get; set; }
        public string 受注数量6 { get; set; }
        public string 件数 { get; set; }
        public string 色コード { get; set; }

        public KyoeiItem(string 分類コード, string 伝票番号, string 行番号, string 京屋商品コード, string 商品コード,
                         string マーク, string 品名, string 規格, string 文字予備７, string 数量, string 原単価, string 売単価,
                         string 店コード1, string 店コード2, string 店コード3, string 店コード4, string 店コード5, string 店コード6,
                         string 受注数量1, string 受注数量2, string 受注数量3, string 受注数量4, string 受注数量5, string 受注数量6,
                         string 件数, string 色コード)
        {
            int conv;
            this.分類コード = 分類コード;
            this.伝票番号 = 伝票番号;
            this.行番号 = 行番号;
            this.京屋商品コード = 京屋商品コード;
            this.商品コード = 商品コード;
            this.マーク = マーク;
            this.品名 = 品名;
            this.規格 = 規格;
            this.文字予備７ = 文字予備７;
            this.数量 = int.TryParse(数量, out conv) ? conv : 0;
            this.原単価 = 原単価;
            this.売単価 = 売単価;
            this.店コード1 = 店コード1;
            this.店コード2 = 店コード2;
            this.店コード3 = 店コード3;
            this.店コード4 = 店コード4;
            this.店コード5 = 店コード5;
            this.店コード6 = 店コード6;
            this.受注数量1 = 受注数量1;
            this.受注数量2 = 受注数量2;
            this.受注数量3 = 受注数量3;
            this.受注数量4 = 受注数量4;
            this.受注数量5 = 受注数量5;
            this.受注数量6 = 受注数量6;
            this.件数 = 件数;
            this.色コード = 色コード;
        }
    }
}
