using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.WAGO;
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

        // 受信日
        public ReactiveProperty<DateTime> JusinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today);

        // 納品日
        public ReactiveProperty<DateTime> NouhinDate { get; set; } = new ReactiveProperty<DateTime>(DateTime.Today.AddDays(1));

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

        // 開始相手品番
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了相手品番
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<YamanakaData> YamanakaDatas { get; set; } = new List<YamanakaData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<YamanakaItem>> YamanakaItems { get; set; }
                = new ReactiveProperty<ObservableCollection<YamanakaItem>>();

        #endregion

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private TOKMTE_LIST tOKMTE_LIST;
        private DB_0127_HANSOKU_BAIKA_CONV_LIST dB_0127_HANSOKU_LIST ;

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
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            tOKMTE_LIST = new TOKMTE_LIST();
            dB_0127_HANSOKU_LIST = new DB_0127_HANSOKU_BAIKA_CONV_LIST();

            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("0025");
            NefudaBangouText = new ReactiveProperty<int>(13);
            McCodeText = new ReactiveProperty<int>(20);

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            McCodeText.Subscribe(x => McCodeTextChanged(x));

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
            var bunruis = new List<BunruiCode>() 
            { 
                new BunruiCode("",""),
                new BunruiCode("0025", "ＥＯＳ定番") 
            };
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
            var item1 = new CommonIdName();
            item1.Id = 13;
            item1.Name = "13-下げ札(小)プロパー";
            list.Add(item1);
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
            var item1 = new CommonIdName();
            item1.Id = 20;
            item1.Name = "20：定番";
            list.Add(item1);
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
                BunruiCodeText.Value = "0025";
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
            JusinDate.Value = DateTime.Today;
            NouhinDate.Value = DateTime.Today.AddDays(1);
            SelectedHakkouTypeIndex.Value = 0;
            BunruiCodeText.Value = "0025";
            SelectedNefudaBangouIndex.Value = 0;
            SelectedMcCodeIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            TotalMaisu.Value = "";
            YamanakaDatas.Clear();
            YamanakaItems.Value.Clear();

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
            var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(127, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value);
            var tokmteList = tOKMTE_LIST.QueryWhereTcode(127);

            if(eosJutraList.Any() && tokmteList.Any())
            {
                int sttHincd;
                int endHincd;
                int vhincd;

                YamanakaDatas.Clear();
                YamanakaDatas.AddRange(
                    eosJutraList
                        .GroupJoin(
                               tokmteList,
                               e => new
                               {
                                   HINCD = e.VHINCD.ToString().TrimEnd(),
                                   TOKCD = e.VRYOHNCD.ToString().TrimEnd(),
                               },
                               t => new
                               {
                                   HINCD = t.EOSHINID.TrimEnd(),
                                   TOKCD = t.TOKCD.TrimEnd(),
                               },
                               (eos, tok) => new
                               {
                                   HNO = "",
                                   VRYOHNCD = eos.VRYOHNCD,
                                   NEFUDANO = "",
                                   VRCVDT = eos.VRCVDT,
                                   VNOHINDT = eos.VNOHINDT,
                                   QOLTORID = eos.QOLTORID,
                                   COLCD = tok.Any() ? tok.FirstOrDefault().COLCD.TrimStart(new Char[] { '0' }) : "",
                                   FACENO = "",
                                   VHINCD = eos.VHINCD,
                                   MCCD = "20",
                                   VCYOBI7 = eos.VCYOBI7,                                   
                                   VURITK = eos.VURITK,
                                   HANSOKUMOJI2 = "",
                                   HANSOKUMOJIDISP = "",
                                   TOUHINBAN = "",
                                   HINCD = eos.HINCD,
                                   VHINNMA = eos.VHINNMA,
                                   VSURYO = eos.VSURYO,
                               })
                         .GroupBy(a => new
                         {
                             a.HNO,
                             a.VRYOHNCD,
                             a.NEFUDANO,
                             a.VRCVDT,
                             a.VNOHINDT,
                             a.QOLTORID,
                             a.COLCD,
                             a.FACENO,
                             a.VHINCD,
                             a.MCCD,
                             a.VCYOBI7,                             
                             a.VURITK,
                             a.HANSOKUMOJI2,
                             a.HANSOKUMOJIDISP,
                             a.TOUHINBAN,
                             a.HINCD,
                             a.VHINNMA,
                         })                         
                         .Select(g => new YamanakaData
                         {
                             HNO = g.Key.HNO,
                             VRYOHNCD = g.Key.VRYOHNCD,
                             NEFUDANO = GetNefudaBangou(g.Key.VHINNMA, "ﾂﾘ", "3ﾖﾘ"),
                             VRCVDT = g.Key.VRCVDT,
                             VNOHINDT = g.Key.VNOHINDT,
                             QOLTORID = g.Key.QOLTORID,
                             COLCD = !string.IsNullOrEmpty(g.Key.COLCD) ? g.Key.COLCD : "0",
                             FACENO = g.Key.FACENO,
                             VHINCD = g.Key.VHINCD.TrimEnd(),
                             MCCD = McCodeText.Value.ToString(),
                             VCYOBI7 = g.Key.VCYOBI7.TrimEnd(),
                             VURITK = GetNefudaBaika(g.Key.VURITK, g.Key.VHINNMA, "3ﾖﾘ")?.値付売価 ?? g.Key.VURITK,
                             HANSOKUMOJI2 = GetNefudaBaika(g.Key.VURITK, g.Key.VHINNMA, "3ﾖﾘ")?.販促文字2 ?? g.Key.HANSOKUMOJI2,
                             HANSOKUMOJIDISP = GetNefudaBaika(g.Key.VURITK, g.Key.VHINNMA, "3ﾖﾘ")?.販促文字表示名 ?? g.Key.HANSOKUMOJIDISP,
                             TOUHINBAN = g.Key.HINCD.Substring(4),
                             HINCD = g.Key.HINCD.TrimEnd(),
                             VHINNMA = g.Key.VHINNMA.TrimEnd(),
                             VSURYO = g.Sum(y => y.VSURYO),
                         })
                         .Where(x => x.NEFUDANO == NefudaBangouText.Value.ToString() && !string.IsNullOrEmpty(x.COLCD) && x.COLCD != "0" &&
                                     (!string.IsNullOrEmpty(this.SttHincd.Value) && 
                                      int.TryParse(this.SttHincd.Value, out sttHincd) && 
                                      int.TryParse(x.VHINCD, out vhincd) ? vhincd >= sttHincd : true) &&
                                     (!string.IsNullOrEmpty(this.EndHincd.Value) &&
                                      int.TryParse(this.EndHincd.Value, out endHincd) &&
                                      int.TryParse(x.VHINCD, out vhincd) ? vhincd <= endHincd : true))
                         .OrderBy(x => x.COLCD)
                         .ThenBy(x => x.VHINCD)
                     );

                if (YamanakaItems.Value == null)
                {
                    YamanakaItems.Value = new ObservableCollection<YamanakaItem>();
                }
                if (YamanakaDatas.Any())
                {
                    YamanakaItems.Value.Clear();
                    var yamanakaModelList = new YamanakaItemList();
                    YamanakaItems.Value = new ObservableCollection<YamanakaItem>(yamanakaModelList.ConvertYamanakaDataToModel(YamanakaDatas));
                    TotalMaisu.Value = YamanakaItems.Value.Sum(x => x.発行枚数).ToString();
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

        private string GetNefudaBangou(string hinnm, string typenm1, string typenm2)
        {            
            string SearchChar;
            string SearchChar2;
            string wk_Char;

            var wNEFUDA_NO = "15";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            wk_Char = Microsoft.VisualBasic.Strings.StrConv(hinnm.TrimEnd(), Microsoft.VisualBasic.VbStrConv.Narrow, 0x411);
            wk_Char = wk_Char.ToUpper();

            SearchChar = typenm1;
            if(wk_Char.Contains(SearchChar))
            {
                wNEFUDA_NO = "13";
            }

            // ※ご奉仕価格のチェック
            SearchChar2 = typenm2;
            if (wk_Char.Contains(SearchChar2))
            {
                switch (wNEFUDA_NO)
                {
                    case "13":
                        wNEFUDA_NO = "14";
                        break;
                    case "15":
                        wNEFUDA_NO = "16";
                        break;
                }
            }
            return wNEFUDA_NO;
        }

        private DB_0127_HANSOKU_BAIKA_CONV? GetNefudaBaika(decimal baika, string hinnm, string typenm = "3ﾖﾘ", string tokcd = "000127")
        {
            DB_0127_HANSOKU_BAIKA_CONV res = null;
            var wNEFUDA_BAIKA = baika;
            string SearchChar;
            string wk_Char;

            wk_Char = Microsoft.VisualBasic.Strings.StrConv(hinnm.TrimEnd(), Microsoft.VisualBasic.VbStrConv.Narrow, 0x411);
            wk_Char = wk_Char.ToUpper();

            SearchChar = typenm;
            if (wk_Char.Contains(SearchChar))
            {
                res = dB_0127_HANSOKU_LIST.list.FirstOrDefault(x => x.得意先CD  == tokcd && x.名称 == typenm && x.売単価 == baika);
            }
            return res;
        }

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return YamanakaItems.Value != null &&
                   YamanakaItems.Value.Any() &&
                   YamanakaItems.Value.Sum(x => x.発行枚数) > 0;
            return false;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var path = @"c:\Program Files (x86)\MLV5\NEFUDA\";
            var fname = "0127" + "_" + 
                        this.JusinDate.Value.ToString("yyyyMMdd") + "_" +
                        this.NouhinDate.Value.ToString("yyyyMMdd") + "_" +
                        this.BunruiCodeText.Value + ".csv";
            var fullName = Path.Combine(path, fname);
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
            var list = YamanakaItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var datas = DataUtility.ToDataTable(list);
            // 不要なカラムの削除
            datas.Columns.Remove("販促文字表示名");
            datas.Columns.Remove("商品名");
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
            var appPath = @"C:\Program Files (x86)\SATO\MLV5\MLPrint.exe";
            var layPath = @"Y:\WAGOAPL\SATO\MLV5_Layout";
            var grpName = @"\0127_ヤマナカ\【総額対応】ヤマナカ_V5_RT308R_振分発行";
            var layName = @"ヤマナカESPO_V5_ST308R_振分発行.mldenx";
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

    public class YamanakaItem
    {
        public string 発注No { get; set; }
        public string 得意先CD { get; set; }
        public string 値札No { get; set; }
        public string 受信日 { get; set; }
        public string 納品日 { get; set; }
        public string 取引先コード { get; set; }
        public string デプトクラスコード { get; set; }
        public string フェイス番号 { get; set; }
        public string 品番 { get; set; }
        public string ＭＣコード { get; set; }
        public string JAN13桁 { get; set; }
        public decimal 本体売価 { get; set; }
        public string 販促文字2 { get; set; }
        public string 販促文字表示名 { get; set; }
        public string 当社品番 { get; set; }
        public string 商品コード { get; set; }
        public string 商品名 { get; set; }
        public decimal 発行枚数 { get; set; }

        public YamanakaItem(string 発注No, string 得意先CD, string 値札No, string 受信日, string 納品日,
                            string 取引先コード, string デプトクラスコード, string フェイス番号, string 品番, 
                            string ＭＣコード,　string JAN13桁, decimal 本体売価, string 販促文字2, string 販促文字表示名,
                            string 当社品番, string 商品コード, string 商品名, decimal 発行枚数)
        {
            this.発注No = 発注No;
            this.得意先CD = 得意先CD;
            this.値札No = 値札No;
            this.受信日 = 受信日;
            this.納品日 = 納品日;
            this.取引先コード = 取引先コード;
            this.デプトクラスコード = デプトクラスコード;
            this.フェイス番号 = フェイス番号;
            this.品番 = 品番;
            this.ＭＣコード = ＭＣコード;
            this.JAN13桁 = JAN13桁;
            this.本体売価 = 本体売価;
            this.販促文字2 = 販促文字2;
            this.販促文字表示名 = 販促文字表示名;
            this.当社品番 = 当社品番;
            this.商品コード = 商品コード;
            this.商品名 = 商品名;
            this.発行枚数 = 発行枚数;
        }
    }

    public class YamanakaItemList
    {
        public IEnumerable<YamanakaItem> ConvertYamanakaDataToModel(List<YamanakaData> datas)
        {
            var result = new List<YamanakaItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new YamanakaItem(data.HNO, data.VRYOHNCD, data.NEFUDANO, data.VRCVDT, data.VNOHINDT, data.QOLTORID,
                                     data.COLCD, data.FACENO, data.VHINCD, data.MCCD, data.VCYOBI7, data.VURITK, data.HANSOKUMOJI2,
                                     data.HANSOKUMOJIDISP, data.TOUHINBAN, data.HINCD, data.VHINNMA, data.VSURYO));                
            });
            return result;
        }
    }
}
