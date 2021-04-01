using PriceTagPrint.Common;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PriceTagPrint.ViewModel
{
    public class YamanakaViewModel : ViewModelsBase
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
        
        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // ＭＣコード
        public ReactiveProperty<int> McCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> McCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedMcCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

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

        //private List<YasusakiData> YasusakiDatas { get; set; } = new List<YasusakiData>();
        //// DataGrid Items
        //public ReactiveProperty<ObservableCollection<YasusakiItem>> YasusakiItems { get; set; }
        //        = new ReactiveProperty<ObservableCollection<YasusakiItem>>();

        #endregion

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;

        //private DB_0112_EOS_HACHU_LIST dB_0112_EOS_HACHU_LIST;
        //private WEB_TORIHIKISAKI_TANKA_LIST wEB_TORIHIKISAKI_TANKA_LIST;

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
                        NefudaDataDisplay();
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

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YamanakaViewModel()
        {
            //dB_0112_EOS_HACHU_LIST = new DB_0112_EOS_HACHU_LIST();
            //wEB_TORIHIKISAKI_TANKA_LIST = new WEB_TORIHIKISAKI_TANKA_LIST();
            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("910");
            NefudaBangouText = new ReactiveProperty<int>(1);
            McCodeText = new ReactiveProperty<int>(20);

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));
            SelectedMcCodeIndex.Subscribe(x => SelectedMcCodeIndexChanged(x));
        }

        #endregion

        #region コントロール生成・変更

        /// <summary>
        /// コンボボックスItem生成
        /// </summary>
        public void CreateComboItems()
        {
            var bunruis = new List<BunruiCode>() { new BunruiCode("0025", "ＥＯＳ定番") };
            HakkouTypeItems.Value = new ObservableCollection<CommonIdName>(CreateHakkouTypeItems());
            BunruiCodeItems.Value = new ObservableCollection<BunruiCode>(bunruis);
            NefudaBangouItems.Value = new ObservableCollection<CommonIdName>(CreateNefudaBangouItems());
            McCodeItems.Value = new ObservableCollection<CommonIdName>(CreateMcCodeItems());
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
            item.Id = 13;
            item.Name = "13-下げ札(小)プロパー";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 14;
            item2.Name = "14-下げ札(小)ご奉仕価格";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 15;
            item3.Name = "15-貼り札(中)プロパー";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 16;
            item4.Name = "16-貼り札(中)ご奉仕価格";
            list.Add(item4);
            return list;
        }
        
        /// <summary>
        /// ＭＣコードItems生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateMcCodeItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 20;
            item.Name = "20：定番";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 21;
            item2.Name = "21：スポット追加可";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 22;
            item3.Name = "22：スポット追加不可";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 23;
            item4.Name = "23：売出";
            list.Add(item4);
            var item5 = new CommonIdName();
            item5.Id = 24;
            item5.Name = "24：委託";
            list.Add(item5);
            return list;
        }

        /// <summary>
        /// 発注番号テキスト変更処理
        /// </summary>
        /// <param name="hno"></param>
        private void HachuBangouTextChanged(string hno)
        {
            if (!string.IsNullOrEmpty(hno))
            {
                //if (dB_0112_EOS_HACHU_LIST.QueryWhereHnoExists(hno))
                //{
                //    HnoResultString.Value = "登録済";
                //    HnoResultColor.Value = Brushes.Blue;
                //}
                //else
                //{
                //    HnoResultString.Value = "※未登録";
                //    HnoResultColor.Value = Brushes.Red;
                //}
            }
            else
            {
                //HnoResultString.Value = "";
                //HnoResultColor.Value = Brushes.Black;
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
        /// ＭＣコードテキスト変更処理
        /// </summary>
        /// <param name="id"></param>
        private void McCodeTextChanged(int id)
        {
            var item = McCodeItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedMcCodeIndex.Value = McCodeItems.Value.IndexOf(item);
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
        
        /// <summary>
        /// ＭＣコードコンボ変更処理
        /// </summary>
        /// <param name="idx"></param>
        private void SelectedMcCodeIndexChanged(int idx)
        {
            var item = McCodeItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                McCodeText.Value = item.Id;
            }
            else
            {
                McCodeText.Value = 0;
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
            BunruiCodeText.Value = "910";
            SelectedNefudaBangouIndex.Value = 0;
            SelectedMcCodeIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            SttEdaban.Value = "";
            EndEdaban.Value = "";
            TotalMaisu.Value = "";
            //YasusakiDatas.Clear();
            //YasusakiItems.Value.Clear();

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
            if (string.IsNullOrEmpty(this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.NefudaBangouText.Value < 13 || this.NefudaBangouText.Value > 16)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.McCodeText.Value < 20 || this.McCodeText.Value > 24)
            {
                MessageBox.Show("ＭＣコードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// F5検索処理
        /// </summary>
        public void NefudaDataDisplay()
        {
            //var w0112EosHchuList = dB_0112_EOS_HACHU_LIST.QueryWhereHno(this.HachuBangou.Value);

            //var wWebTorihikisakiTankaList = wEB_TORIHIKISAKI_TANKA_LIST.QueryWhereTcodeTenpo("112", "9999");

            
        }

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            //return YasusakiItems.Value != null &&
            //       YasusakiItems.Value.Any() &&
            //       YasusakiItems.Value.Sum(x => x.発行枚数) > 0;
            return false;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var path = @"c:\Program Files (x86)\MLV5\NEFUDA\";
            var fname = "0112" + "_" + ".csv";
            var fullName = path + fname;
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
            //var list = YasusakiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            //var datas = DataUtility.ToDataTable(list);
            //// 不要なカラムの削除
            //datas.Columns.Remove("商品名");
            //datas.Columns.Remove("単価");
            //datas.Columns.Remove("和合商品コード");
            //datas.Columns.Remove("相手先品番");
            //new CsvUtility().Write(datas, fullName, true);
        }

        /// <summary>
        /// F10プレビュー・F12印刷 外部アプリ（MLV5）起動
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="isPreview"></param>
        private void NefudaOutput(string fname, bool isPreview)
        {
            // ※振分発行用ＰＧ
            var appPath = @"C:\Program Files (x86)\SATO\MLV5\MLPrint.exe";
            var layPath = @"Y:\WAGOAPL\SATO\MLV5_Layout";
            var grpName = @"\0112_ヤスサキ\【総額対応】ヤスサキ_V5_RT308R_振分発行";
            var layName = @"41300-ﾔｽｻｷ_JAN1段＋税_ST308R_振分発行.mldenx";
            var layNo = layPath + @"\" + grpName + @"\" + layName;
            var dq = "\"";
            var args = dq + layNo + dq + " /g " + dq + fname + dq + (isPreview ? " /p " : " /o ");

            //Processオブジェクトを作成する
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //起動する実行ファイルのパスを設定する
            p.StartInfo.FileName = appPath;
            //コマンドライン引数を指定する
            p.StartInfo.Arguments = args;
            //起動する。プロセスが起動した時はTrueを返す。
            bool result = p.Start();
        }
        #endregion
    }
}
