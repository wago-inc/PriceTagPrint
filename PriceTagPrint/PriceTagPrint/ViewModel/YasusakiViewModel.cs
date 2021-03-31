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
    public class RelayCommand : ICommand
    {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public RelayCommand(Action action)
        {//コンストラクタでActionを登録
            _action = action;
        }

        #region ICommandインターフェースの必須実装

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {//とりあえずActionがあれば実行可能
            return _action != null;
        }

        public void Execute(object parameter)
        {//今回は引数を使わずActionを実行
            _action?.Invoke();
        }

        #endregion
    }

    public class HakkouType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NefudaBangou
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class YasusakiViewModel : ViewModelsBase
    {
        // 発行区分
        public ReactiveProperty<int> HakkouTypeText { get; set; }
        public ReactiveProperty<ObservableCollection<HakkouType>> HakkouTypeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<HakkouType>>();
        public ReactiveProperty<int> SelectedHakkouTypeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 分類コード
        public ReactiveProperty<string> BunruiCodeText { get; set; }
        public ReactiveProperty<ObservableCollection<BunruiCode>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<BunruiCode>>();
        public ReactiveProperty<int> SelectedBunruiCodeIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 発注番号
        public ReactiveProperty<string> HachuBangou { get; set; } = new ReactiveProperty<string>("");
        // 発注番号（存在結果情報）
        public ReactiveProperty<string> HnoResultString { get; set; } = new ReactiveProperty<string>("");
        // 発注番号（存在結果情報表示色） 
        public ReactiveProperty<Brush> HnoResultColor { get; set; } = new ReactiveProperty<Brush>(Brushes.Black);

        // 値札番号
        public ReactiveProperty<int> NefudaBangouText { get; set; }
        public ReactiveProperty<ObservableCollection<NefudaBangou>> NefudaBangouItems { get; set; }
                = new ReactiveProperty<ObservableCollection<NefudaBangou>>();
        public ReactiveProperty<int> SelectedNefudaBangouIndex { get; set; }
                = new ReactiveProperty<int>(0);

        // 商品コード表示・非表示
        public ReactiveProperty<bool> HinEnabled { get; set; } = new ReactiveProperty<bool>(false);
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

        public TextBox HakkouTypeTextBox = null;
        private List<YasusakiData> YasusakiDatas { get; set; } = new List<YasusakiData>();
        public ReactiveProperty<ObservableCollection<YasusakiItem>> YasusakiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<YasusakiItem>>();

        private DB_0112_EOS_HACHU_LIST dB_0112_EOS_HACHU_LIST;

        public void Loaded()
        {
            Task.Run(() =>
            {

            });
        }

        #region コマンドの実装
        private RelayCommand<string> funcActionCommand;
        public RelayCommand<string> FuncActionCommand
        {
            get { return funcActionCommand = funcActionCommand ?? new RelayCommand<string>(FuncAction); }
        }

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

        public YasusakiViewModel()
        {
            dB_0112_EOS_HACHU_LIST = new DB_0112_EOS_HACHU_LIST();
            CreateComboItems();

            HakkouTypeText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("910");
            NefudaBangouText = new ReactiveProperty<int>(1);

            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));
            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedBunruiCodeIndex.Subscribe(x => SelectedBunruiCodeIndexChanged(x));
            SelectedNefudaBangouIndex.Subscribe(x => SelectedNefudaBangouIndexChanged(x));

            HachuBangou.Subscribe(x => HachuBangouTextChanged(x));
        }

        private void HachuBangouTextChanged(string hno)
        {
            if (!string.IsNullOrEmpty(hno))
            {
                if (dB_0112_EOS_HACHU_LIST.QueryWhereHnoExists(hno))
                {
                    HnoResultString.Value = "登録済";
                    HnoResultColor.Value = Brushes.Blue;
                }
                else
                {
                    HnoResultString.Value = "※未登録";
                    HnoResultColor.Value = Brushes.Red;
                }
            }
            else
            {
                HnoResultString.Value = "";
                HnoResultColor.Value = Brushes.Black;
            }
        }

        private void HakkouTypeTextChanged(int id)
        {
            var item = HakkouTypeItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedHakkouTypeIndex.Value = HakkouTypeItems.Value.IndexOf(item);
            }
        }

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

        private void NefudaBangouTextChanged(int id)
        {
            var item = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedNefudaBangouIndex.Value = NefudaBangouItems.Value.IndexOf(item);
            }
        }

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

        public void Clear()
        {
            SelectedHakkouTypeIndex.Value = 0;
            BunruiCodeText.Value = "910";
            HachuBangou.Value = "";
            HnoResultString.Value = "";
            HnoResultColor.Value = Brushes.Black;
            SelectedNefudaBangouIndex.Value = 0;
            HinEnabled.Value = false;
            SttHincd.Value = "";
            EndHincd.Value = "";
            SttEdaban.Value = "";
            EndEdaban.Value = "";
            TotalMaisu.Value = "";
            YasusakiDatas.Clear();
            YasusakiItems.Value.Clear();

            HakkouTypeTextBox.Focus();
        }

        public bool InputCheck()
        {
            if (this.HakkouTypeText.Value < 1 || this.HakkouTypeText.Value > 2)
            {
                MessageBox.Show("発行区分を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.HachuBangou.Value))
            {
                MessageBox.Show("発注番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!string.IsNullOrEmpty(HnoResultString.Value) && HnoResultString.Value.Contains("未登録"))
            {
                MessageBox.Show("未登録の発注番号が選択されています。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (this.NefudaBangouText.Value < 1 || this.NefudaBangouText.Value > 2)
            {
                MessageBox.Show("値札番号を選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void NefudaDataDisplay()
        {
            var w0112EosHchuList = dB_0112_EOS_HACHU_LIST.QueryWhereHno(this.HachuBangou.Value);

            var wWEB_TORIHIKISAKI_TANKA = new WEB_TORIHIKISAKI_TANKA_LIST();
            var wWebTorihikisakiTankaList = wWEB_TORIHIKISAKI_TANKA.QueryWhereTcodeTenpo("112", "9999");

            if (w0112EosHchuList.Any() && this.HakkouTypeText.Value == 2)
            {
                int sttHincd;
                int endHincd;
                int sttEdaban;
                int endEdaban;
                int scode;

                w0112EosHchuList = w0112EosHchuList.Where(x => (int.TryParse(this.SttHincd.Value, out sttHincd) && int.TryParse(x.SCODE, out scode) ? scode >= sttHincd : true) &&
                                            (int.TryParse(this.EndHincd.Value, out endHincd) && int.TryParse(x.SCODE, out scode) ? scode <= endHincd : true) &&
                                            (int.TryParse(this.SttEdaban.Value, out sttEdaban) ? x.SAIZUS >= sttEdaban : true) &&
                                            (int.TryParse(this.EndEdaban.Value, out endEdaban) ? x.SAIZUS <= endEdaban : true))
                                    .ToList();
            }

            if (w0112EosHchuList.Any() && wWebTorihikisakiTankaList.Any())
            {
                YasusakiDatas.Clear();
                YasusakiDatas.AddRange(
                    w0112EosHchuList.Where(x => x.NSU > 0 && x.BUNRUI == int.Parse(this.BunruiCodeText.Value))
                        .Join(
                               wWebTorihikisakiTankaList.Where(x => x.NEFUDA_KBN == this.NefudaBangouText.Value.ToString()),
                               e => new
                               {
                                   TOKCD = short.Parse(e.TOKCD),
                                   BUNRUI = short.Parse(e.BUNRUI.ToString()),
                                   SCODE = int.Parse(e.SCODE),
                                   SAIZUS = short.Parse(e.SAIZUS.ToString()),
                               },
                               w => new
                               {
                                   TOKCD = w.TCODE,
                                   BUNRUI = w.BUNRUI,
                                   SCODE = w.HCODE,
                                   SAIZUS = w.SAIZU
                               },
                               (eos, tanka) => new
                               {
                                   HNO = eos.HNO,
                                   TOKCD = eos.TOKCD,
                                   SYOHINCD = eos.SYOHINCD,
                                   JANCD = eos.JANCD,
                                   BUNRUI = eos.BUNRUI,
                                   SCODE = eos.SCODE,
                                   SAIZUS = eos.SAIZUS,
                                   HINCD = eos.HINCD,
                                   HATYUBI = eos.HATYUBI,
                                   NOUHINBI = eos.NOUHINBI,
                                   NSU = eos.NSU,
                                   BAIKA = eos.BAIKA,
                                   EOS_SYOHINNM = eos.EOS_SYOHINNM,
                                   GENKA = eos.GENKA,
                                   SKBN = tanka.SKBN,
                                   NEFUDA_KBN = tanka.NEFUDA_KBN,
                                   NETUKE_BUNRUI = tanka.NETUKE_BUNRUI,
                                   BIKOU1 = tanka.BIKOU1,
                                   BIKOU2 = tanka.BIKOU2
                               })
                         .GroupBy(a => new
                         {
                             a.HNO,
                             a.TOKCD,
                             a.SYOHINCD,
                             a.JANCD,
                             a.BUNRUI,
                             a.SCODE,
                             a.SAIZUS,
                             a.HINCD,
                             a.HATYUBI,
                             a.NOUHINBI,
                             a.BAIKA,
                             a.EOS_SYOHINNM,
                             a.GENKA,
                             a.SKBN,
                             a.NEFUDA_KBN,
                             a.NETUKE_BUNRUI,
                             a.BIKOU1,
                             a.BIKOU2
                         })
                         .Select(g => new YasusakiData
                         {
                             HNO = g.Key.HNO,
                             TOKCD = g.Key.TOKCD,
                             SYOHINCD = g.Key.SYOHINCD,
                             JANCD = g.Key.JANCD,
                             BUNRUI = g.Key.BUNRUI,
                             SCODE = g.Key.SCODE,
                             SAIZUS = g.Key.SAIZUS,
                             HINCD = g.Key.HINCD,
                             HATYUBI = g.Key.HATYUBI,
                             NOUHINBI = g.Key.NOUHINBI,
                             NSU = g.Sum(y => y.NSU),
                             BAIKA = g.Key.BAIKA,
                             EOS_SYOHINNM = g.Key.EOS_SYOHINNM,
                             GENKA = g.Key.GENKA,
                             SKBN = g.Key.SKBN,
                             NEFUDA_KBN = g.Key.NEFUDA_KBN,
                             NETUKE_BUNRUI = g.Key.NETUKE_BUNRUI,
                             BIKOU1 = g.Key.BIKOU1,
                             BIKOU2 = g.Key.BIKOU2
                         })
                         .OrderBy(g => g.HNO)
                         .OrderBy(g => g.SYOHINCD)
                     );

                if (YasusakiItems.Value == null)
                {
                    YasusakiItems.Value = new ObservableCollection<YasusakiItem>();
                }
                if (YasusakiDatas.Any())
                {
                    YasusakiItems.Value.Clear();
                    var yasusakiModelList = new YasusakiItemList();
                    YasusakiItems.Value = new ObservableCollection<YasusakiItem>(yasusakiModelList.ConvertYasusakiDataToModel(YasusakiDatas));
                    TotalMaisu.Value = YasusakiItems.Value.Sum(x => x.発行枚数).ToString();
                }
            }
            else
            {
                MessageBox.Show("発注データが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CreateComboItems()
        {
            var bunruis = new BunruiCodeList().GetBunruiCodes();
            bunruis.Insert(0, new BunruiCode("", ""));
            HakkouTypeItems.Value = new ObservableCollection<HakkouType>(GetHakkouTypeItems());
            BunruiCodeItems.Value = new ObservableCollection<BunruiCode>(bunruis);
            NefudaBangouItems.Value = new ObservableCollection<NefudaBangou>(GetNefudaBangouItems());

            SelectedHakkouTypeIndex.Subscribe(x => BunruiChanged(x));
        }

        public void BunruiChanged(int index)
        {
            switch (index)
            {
                case 0:
                    HinEnabled.Value = false;
                    break;
                case 1:
                    HinEnabled.Value = true;
                    break;
                default:
                    break;
            }
        }

        public List<HakkouType> GetHakkouTypeItems()
        {
            var list = new List<HakkouType>();
            var item = new HakkouType();
            item.Id = 1;
            item.Name = "1：新規発行";
            list.Add(item);
            var item2 = new HakkouType();
            item2.Id = 2;
            item2.Name = "2：再発行";
            list.Add(item2);
            return list;
        }

        public List<NefudaBangou> GetNefudaBangouItems()
        {
            var list = new List<NefudaBangou>();
            var item = new NefudaBangou();
            item.Id = 1;
            item.Name = "1：ラベル";
            list.Add(item);
            var item2 = new NefudaBangou();
            item2.Id = 2;
            item2.Name = "2：タグ";
            list.Add(item2);
            return list;
        }

        public bool PrintCheck()
        {
            return YasusakiItems.Value != null &&
                   YasusakiItems.Value.Any() &&
                   YasusakiItems.Value.Sum(x => x.発行枚数) > 0;
        }
        public void ExecPrint(bool isPreview)
        {
            var path = @"c:\Program Files (x86)\MLV5\NEFUDA\";
            var fname = "0112" + "_" + this.HachuBangou.Value + ".csv";
            var fullName = path + fname;
            CsvExport(fullName);
            if (!File.Exists(fullName))
            {
                MessageBox.Show("CSVファイルのエクスポートに失敗しました。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NefudaOutput(fullName, isPreview);
        }
        private void CsvExport(string fullName)
        {            
            var list = YasusakiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var datas = DataUtility.ToDataTable(list);
            datas.Columns.Remove("商品名");
            datas.Columns.Remove("単価");
            new CsvUtility().Write(datas, fullName, true);
        }

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
    }
    public class YasusakiItem
    {
        public int 発注No { get; set; }
        public string 取引先CD { get; set; }
        public string 値札No { get; set; }
        public string 発注日 { get; set; }
        public string 納品日 { get; set; }
        public string ｸﾗｽｺｰﾄﾞ { get; set; }
        public string 品番 { get; set; }
        public string 枝番 { get; set; }
        public string ｻｲｽﾞｺｰﾄﾞ { get; set; }
        public string 規格表現文字 { get; set; }
        public string 売切月 { get; set; }
        public string JAN { get; set; }
        public int 本体価格 { get; set; }
        public string 商品コード { get; set; }
        public int 発行枚数 { get; set; }
        public string 商品名 { get; set; }
        public int 単価 { get; set; }
        
        public YasusakiItem(int 発注No, string 取引先CD, string 値札No, string 発注日, string 納品日,
                            string ｸﾗｽｺｰﾄﾞ, string 品番, string 枝番, string ｻｲｽﾞｺｰﾄﾞ, string 規格表現文字,
                            string 売切月, string JAN, int 本体価格, string 商品コード, int 発行枚数,
                             string 商品名, int 単価)
        {
            this.発注No = 発注No;
            this.取引先CD = 取引先CD;
            this.値札No = 値札No;
            this.発注日 = 発注日;
            this.納品日 = 納品日;
            this.ｸﾗｽｺｰﾄﾞ = ｸﾗｽｺｰﾄﾞ;
            this.品番 = 品番;
            this.枝番 = 枝番;
            this.ｻｲｽﾞｺｰﾄﾞ = ｻｲｽﾞｺｰﾄﾞ;
            this.規格表現文字 = 規格表現文字;
            this.売切月 = 売切月;
            this.JAN = JAN;            
            this.売切月 = 売切月;
            this.品番 = 品番;
            this.JAN = JAN;
            this.本体価格 = 本体価格;
            this.商品コード = 商品コード;
            this.発行枚数 = 発行枚数;
            this.商品名 = 商品名;            
            this.単価 = 単価;            
        }
    }

    public class YasusakiItemList
    {
        public IEnumerable<YasusakiItem> ConvertYasusakiDataToModel(List<YasusakiData> datas)
        {
            var result = new List<YasusakiItem>();
            var uritukiList = new DB_0112_URITUKI_LIST();
            var urituki = "";
            var beforeNouhinbi = DateTime.MinValue;
            var beforeSkbn = "";
            datas.ForEach(data =>
            {
                if (beforeNouhinbi != data.NOUHINBI || beforeSkbn != data.SKBN)
                {
                    urituki = uritukiList.GetURITUKI(data.NOUHINBI, data.SKBN);
                }
                result.Add(
                    new YasusakiItem(data.HNO, data.TOKCD, data.NEFUDA_KBN, data.HATYUBI.ToString("yyyyMMdd"), data.NOUHINBI.ToString("yyyyMMdd"), data.NETUKE_BUNRUI, data.SCODE,
                                     data.SAIZUS.ToString("00"), data.BIKOU1, data.BIKOU2, urituki, data.JANCD, data.BAIKA, data.SYOHINCD, data.NSU,
                                     data.EOS_SYOHINNM, data.GENKA));
                beforeNouhinbi = data.NOUHINBI; 
                beforeSkbn = data.SKBN;
            });
            return result;
        }
    }
}
