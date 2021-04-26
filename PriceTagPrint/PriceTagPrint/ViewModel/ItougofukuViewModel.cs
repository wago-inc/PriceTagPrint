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
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModel
{
    public class ItougofukuViewModel : ViewModelsBase
    {
        #region プロパティ

        // 選択ファイルパス
        public ReactiveProperty<string> FilePathText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<ObservableCollection<ItougofukuItem>> ItougofukuItems { get; set; }
                = new ReactiveProperty<ObservableCollection<ItougofukuItem>>();

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        #endregion

        private readonly string _grpName = @"7705_イトウゴフク\2020年総額表示_V5_ST308R";
        private readonly string _grpFullName = @"Y:\WAGOAPL\SATO\MLV5_Layout\7705_イトウゴフク\2020年総額表示_V5_ST308R";
        private CsvUtility csvUtility = new CsvUtility();

        // 値札テキストボックス
        public TextBox NefudaBangouTextBox = null;

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

        public ItougofukuViewModel()
        {
            CreateComboItems();

            ItougofukuItems.Value = new ObservableCollection<ItougofukuItem>();
            NefudaBangouText = new ReactiveProperty<int>(0);

            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

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

        #region ファンクション
        /// <summary>
        /// F4 初期化処理
        /// </summary>
        public void Clear()
        {
            FilePathText.Value = "";
            SelectedNefudaBangouIndex.Value = 0;
            ItougofukuItems.Value.Clear();
            TotalMaisu.Value = "";
            NefudaBangouTextBox.Focus();
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
            var dt = csvUtility.ReadCSV(true, FilePathText.Value);
            if (dt.Rows.Count > 0)
            {
                ItougofukuItems.Value.Clear();
            }
            foreach (DataRow row in dt.Rows)
            {
                var arr = row.ItemArray.Cast<string>().ToArray();

                ItougofukuItems.Value.Add(
                    new ItougofukuItem
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
                        arr.ElementAtOrDefault(25),
                        arr.ElementAtOrDefault(26),
                        arr.ElementAtOrDefault(27),
                        arr.ElementAtOrDefault(28),
                        arr.ElementAtOrDefault(29),
                        arr.ElementAtOrDefault(30),
                        arr.ElementAtOrDefault(31)
                        )
                    );
            }
            TotalMaisu.Value = ItougofukuItems.Value.Any() ?
                                ItougofukuItems.Value.Sum(x => x.数量計).ToString() : "";
        }

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return ItougofukuItems.Value != null &&
                   ItougofukuItems.Value.Any() &&
                   ItougofukuItems.Value.Sum(x => x.数量計) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.KYOEI + "_" +
                        Path.GetFileNameWithoutExtension(FilePathText.Value) + "_" + 
                        DateTime.Today.ToString("yyyyMMddmmhhss") + ".csv";
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
            var list = ItougofukuItems.Value.Where(x => x.数量計 > 0).ToList();

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

    public class ItougofukuItem
    {
        public string 帳票種別 { get; set; }
        public string 仕入先 { get; set; }
        public string 仕入先名 { get; set; }
        public string 商品コード { get; set; }
        public string 発注日 { get; set; }
        public string 納入日 { get; set; }
        public string 販売開始 { get; set; }
        public string メーカーコード { get; set; }
        public string クラス { get; set; }
        public string クラス名 { get; set; }
        public string ユニット { get; set; }
        public string 商品名 { get; set; }
        public string サイズ { get; set; }
        public string カラー { get; set; }
        public string 原単価 { get; set; }
        public string 売単価 { get; set; }
        public string 税込売価 { get; set; }
        public string コメント { get; set; }
        public string 商品区分 { get; set; }
        public string シーズン { get; set; }
        public int 数量計 { get; set; }
        public string セット数 { get; set; }
        public string 入数 { get; set; }
        public string 値札 { get; set; }
        public string 店数量 { get; set; }
        public string 納入場所 { get; set; }
        public string バイヤー { get; set; }
        public string 発注No { get; set; }
        public string 行No { get; set; }
        public string 下代 { get; set; }
        public string 税込金額 { get; set; }
        public string 部門コード { get; set; }

        public ItougofukuItem(string 帳票種別, string 仕入先, string 仕入先名, string 商品コード, string 発注日, string 納入日, string 販売開始, string メーカーコード,
                              string クラス, string クラス名, string ユニット, string 商品名, string サイズ, string カラー, string 原単価, string 売単価, string 税込売価,
                              string コメント, string 商品区分, string シーズン, string 数量計, string セット数, string 入数, string 値札, string 店数量, string 納入場所,
                              string バイヤー, string 発注No, string 行No, string 下代, string 税込金額, string 部門コード)
        {
            int conv;
            this.帳票種別 = 帳票種別;
            this.仕入先 = 仕入先;
            this.仕入先名 = 仕入先名;
            this.商品コード = 商品コード;
            this.発注日 = 発注日;
            this.納入日 = 納入日;
            this.販売開始 = 販売開始;
            this.メーカーコード = メーカーコード;
            this.クラス = クラス;
            this.クラス名 = クラス名;
            this.ユニット = ユニット;
            this.商品名 = 商品名;
            this.サイズ = サイズ;
            this.カラー = カラー;
            this.原単価 = 原単価;
            this.売単価 = 売単価;
            this.税込売価 = 税込売価;
            this.コメント = コメント;
            this.商品区分 = 商品区分;
            this.シーズン = シーズン;
            this.数量計 = int.TryParse(数量計, out conv) ? conv : 0;
            this.セット数 = セット数;
            this.入数 = 入数;
            this.値札 = 値札;
            this.店数量 = 店数量;
            this.納入場所 = 納入場所;
            this.バイヤー = バイヤー;
            this.発注No = 発注No;
            this.行No = 行No;
            this.下代 = 下代;
            this.税込金額 = 税込金額;
            this.部門コード = 部門コード;
        }
    }
}
