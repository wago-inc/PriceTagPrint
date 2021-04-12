using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Model;
using PriceTagPrint.WAG_USR1;
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
    public class MaruyoshiViewModel : ViewModelsBase
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

        // 開始相手品番
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了相手品番
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<MaruyoshiData> MaruyoshiDatas { get; set; } = new List<MaruyoshiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<MaruyoshiItem>> MaruyoshiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<MaruyoshiItem>>();

        #endregion

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private EOSKNMTA_LIST eOSKNMTA_LIST;
        private HINMTA_LIST hINMTA_LIST;
        private DB_0127_HANSOKU_BAIKA_CONV_LIST dB_0127_HANSOKU_LIST;
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
        public MaruyoshiViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            eOSKNMTA_LIST = new EOSKNMTA_LIST();
            hINMTA_LIST = new HINMTA_LIST();
            dB_0127_HANSOKU_LIST = new DB_0127_HANSOKU_BAIKA_CONV_LIST();
            hinmtaList = hINMTA_LIST.QueryWhereAll();
            CreateComboItems();

            // コンボボックス初期値セット
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
        /// 発行区分Items生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateBunruiCodeItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 3;
            item.Name = "3：インナー";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 6;
            item2.Name = "6：インナー";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 9;
            item3.Name = "9：レッグ";
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
            item1.Id = 0;
            item1.Name = "0：２１号ラベルプロパー";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：１２号タグプロパー";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 3;
            item3.Name = "3：１１号タグプロパー";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 5;
            item4.Name = "5：２１号ラベルプロパー";
            list.Add(item4);
            var item5 = new CommonIdName();
            item5.Id = 6;
            item5.Name = "6：値下げラベル";
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
            else
            {
                NefudaBangouText.Value = 0;
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
            MaruyoshiDatas.Clear();
            if (MaruyoshiItems.Value != null && MaruyoshiItems.Value.Any())
            {
                MaruyoshiItems.Value.Clear();
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
        /// F5検索処理
        /// </summary>
        public void NefudaDataDisplay()
        {
            var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(102, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value.ToString("000"));
            var easKnmtaList = eOSKNMTA_LIST.QueryWhereTcode(102);

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
                                   VBUNCD = eosj.VBUNCD,
                                   DATNO = eosj.DATNO,
                                   VROWNO = eosj.VROWNO,
                                   VHEAD1 = eosj.VHEAD1,
                                   VBODY1 = eosj.VBODY1,
                                   VHINCD = eosj.VHINCD,
                                   HINCD = eosj.HINCD,
                               })
                        .OrderBy(x => x.VRYOHNCD)
                                .ThenBy(x => x.VRCVDT)
                                .ThenBy(x => x.VBUNCD)
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
                        decimal convdec;
                        MaruyoshiDatas.Clear();
                        MaruyoshiDatas.AddRange(
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
                                           RPTCLTID =  " ",
                                           VRYOHNCD = a.VRYOHNCD,
                                           VRYOHNNM = a.VRYOHNNM,
                                           VRCVDT = a.VRCVDT,
                                           VNOHINDT = a.VNOHINDT,
                                           VBUNCD = !string.IsNullOrEmpty(a.VHEAD1) ? "0" + a.VHEAD1.Substring(18, 1) : " ",
                                           DATNO = a.DATNO,
                                           VROWNO = a.VROWNO,
                                           NEFCMA = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(5, 4) : " ",
                                           NEFCMB = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(69, 10) : " ",
                                           NEFCMB2 = " ",
                                           NEFCMC = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(79, 25) : " ",
                                           NEFCMD = !string.IsNullOrEmpty(a.VBODY1) && a.VBUNCD.TrimEnd() != "009" ? a.VBODY1.Substring(109, 2) + " " + a.VBODY1.Substring(104, 5) : " ",
                                           NEFCMD2 = !string.IsNullOrEmpty(a.VBODY1) && a.VBUNCD.TrimEnd() != "009" ? a.VBODY1.Substring(109, 2) : " ",
                                           NEFCME = !string.IsNullOrEmpty(a.VBODY1) && a.VBUNCD.TrimEnd() != "009" ? a.VHEAD1.Substring(18, 1) + a.VBODY1.Substring(116, 2) : " ",
                                           NEFCMF = !string.IsNullOrEmpty(a.VBODY1) && a.VBUNCD.TrimEnd() != "009" ? a.VBODY1.Substring(111, 5) : " ",
                                           NEFCMG = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(10, 4) : " ",
                                           NEFCMH = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(15, 2) : " ",
                                           NEFCMI = !string.IsNullOrEmpty(a.VBODY1) ? 
                                                        a.VBODY1.Substring(118, 1) == "1" ? "8" :
                                                        a.VBODY1.Substring(118, 1) == "2" ? "2" : " " : " ",
                                           NEFCMJ = !string.IsNullOrEmpty(a.VBODY1) ?
                                                        a.VBODY1.Substring(126, 1) == "1" ? "T" :
                                                        a.VBODY1.Substring(126, 1) == "2" ? "Y" :
                                                        a.VBODY1.Substring(126, 1) == "3" ? "X" : " " : " ",
                                           NEFCMK = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(119, 1) : " ",
                                           NEFTKA = !string.IsNullOrEmpty(a.VBODY1) &&  
                                                        (a.VBODY1.Substring(119, 1) == "2" || 
                                                         a.VBODY1.Substring(119, 1) == "4" || 
                                                         a.VBODY1.Substring(119, 1) == "6") && decimal.TryParse(a.VBODY1.Substring(120, 6), out convdec) ? convdec : 0,
                                           NEFTKB = !string.IsNullOrEmpty(a.VBODY1) && decimal.TryParse(a.VBODY1.Substring(42, 7), out convdec) ? convdec : 0,
                                           NEFSUA = !string.IsNullOrEmpty(a.VBODY1) && decimal.TryParse(a.VBODY1.Substring(27, 5), out convdec) ? convdec : 0,
                                           //NEFTKB2 = !string.IsNullOrEmpty(a.VBODY1) && decimal.TryParse(a.VBODY1.Substring(42, 7), out convdec) ? Math.Floor(convdec * 1.1m) : 0, 
                                           NEFTKB2 = !string.IsNullOrEmpty(a.VBODY1) && decimal.TryParse(a.VBODY1.Substring(42, 7), out convdec) ? convdec : 0,
                                           NEFSEZ = !string.IsNullOrEmpty(a.VBODY1) ? a.VBODY1.Substring(101, 2) : " ",
                                           VHINCD = a.VHINCD,
                                           HINCD = a.HINCD,
                                           JANCD = hin.Any() ? hin.FirstOrDefault().JANCD : "",
                                           WRTDT = dateNow.ToString("yyyyMMdd"),
                                           WRTTM = dateNow.ToString("hhmmss"),
                                       })
                                .GroupBy(a => new 
                                {
                                    a.RPTCLTID,
                                    a.VRYOHNCD,
                                    a.VRYOHNNM,
                                    a.VRCVDT,
                                    a.VNOHINDT,
                                    a.VBUNCD,
                                    a.NEFCMA,
                                    a.NEFCMB,
                                    a.NEFCMB2,
                                    a.NEFCMC,
                                    a.NEFCMD,
                                    a.NEFCMD2,
                                    a.NEFCME,
                                    a.NEFCMF,
                                    a.NEFCMG,
                                    a.NEFCMH,
                                    a.NEFCMI,
                                    a.NEFCMJ,
                                    a.NEFCMK,
                                    a.NEFTKA,
                                    a.NEFTKB,
                                    a.WRTTM,
                                    a.WRTDT,
                                    a.VHINCD,
                                    a.HINCD,
                                    a.NEFTKB2,
                                    a.NEFSEZ,
                                    a.JANCD
                                })
                                .Select(g => new MaruyoshiData() 
                                {
                                    RPTCLTID = g.Key.RPTCLTID,
                                    VRYOHNCD = g.Key.VRYOHNCD,
                                    VRYOHNNM = g.Key.VRYOHNNM,
                                    VRCVDT = g.Key.VRCVDT,
                                    VNOHINDT = g.Key.VNOHINDT,
                                    VBUNCD = g.Key.VBUNCD,
                                    NEFCMA = g.Key.NEFCMA,
                                    NEFCMB = g.Key.NEFCMB,
                                    NEFCMB2 = TanabanCheck(g.Key.NEFCMC),
                                    NEFCMC = g.Key.NEFCMC,
                                    NEFCMD = g.Key.NEFCMD,
                                    NEFCMD2 = g.Key.NEFCMD2,
                                    NEFCME = g.Key.NEFCME.TrimEnd(),
                                    NEFCMF = g.Key.NEFCMF,
                                    NEFCMG = g.Key.NEFCMG,
                                    NEFCMH = g.Key.NEFCMH,
                                    NEFCMI = g.Key.NEFCMI,
                                    NEFCMJ = g.Key.NEFCMJ,
                                    NEFCMK = g.Key.NEFCMK,
                                    NEFTKA = g.Key.NEFTKA,
                                    NEFTKB = g.Key.NEFTKB,
                                    NEFSUA = g.Sum(y => y.NEFSUA),
                                    WRTTM = g.Key.WRTTM,
                                    WRTDT = g.Key.WRTDT,
                                    VHINCD = g.Key.VHINCD,
                                    HINCD = g.Key.HINCD,
                                    NEFTKB2 = g.Key.NEFTKB2,
                                    NEFSEZ = g.Key.NEFSEZ,
                                    JANCD = g.Key.JANCD
                                })
                                .Where(x => !string.IsNullOrEmpty(this.SttHincd.Value) ? 
                                                int.TryParse(this.SttHincd.Value, out sttHincd) && 
                                                int.TryParse(x.NEFCMG, out aitSttHincd) ? 
                                                    aitSttHincd >= sttHincd : true
                                            : true)
                                .Where(x => !string.IsNullOrEmpty(this.EndHincd.Value) ?
                                                int.TryParse(this.EndHincd.Value, out endHincd) &&
                                                int.TryParse(x.NEFCMG, out aitEndHincd) ?
                                                    aitEndHincd <= endHincd : true
                                            : true)
                                .Where(x => this.NefudaBangouText.Value != 0 ? 
                                                this.NefudaBangouText.Value != 6 ? 
                                                x.NEFCMK == this.NefudaBangouText.Value.ToString() : x.NEFTKA != 0 : true)
                                .OrderBy(x => x.VRYOHNCD)
                                .ThenBy(x => x.VRCVDT)
                                .ThenBy(x => x.VBUNCD)
                                .ThenBy(x => x.VHINCD)
                                );
                    }

                    if (MaruyoshiItems.Value == null)
                    {
                        MaruyoshiItems.Value = new ObservableCollection<MaruyoshiItem>();
                    }
                    if (MaruyoshiDatas.Any())
                    {
                        MaruyoshiItems.Value.Clear();
                        var maruyoshiModelList = new MaruyoshiItemList();
                        MaruyoshiItems.Value = new ObservableCollection<MaruyoshiItem>(maruyoshiModelList.ConvertMaruyoshiDataToModel(MaruyoshiDatas));
                        TotalMaisu.Value = MaruyoshiItems.Value.Sum(x => x.発行枚数).ToString();
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
            this.HakkouTypeTextBox.Focus();
        }

        private string TanabanCheck(string inStr)
        {
            string SearchChar;
            string wk_Char;
            string out_Char = "";

            int myPos;
            int svPos1;
            int svPos2;

            int chk_Cnt;

            if (string.IsNullOrEmpty(inStr))
            {
                return string.Empty;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            wk_Char = Microsoft.VisualBasic.Strings.StrConv(inStr.TrimEnd(), Microsoft.VisualBasic.VbStrConv.Narrow, 0x411);
            wk_Char = wk_Char.ToUpper();

            // ◆"-"の位置をチェック
            SearchChar = "-";
            svPos1 = 0;
            if (wk_Char.Contains(SearchChar))
            {
                svPos1 = wk_Char.IndexOf(SearchChar) + 1;
            }
            else
            {
                return "";// ※"-"無しの場合は処理終了
            }

            // ◆"A-Z"の文字を検索
            char moji;

            svPos2 = 0;
            for (moji = 'A'; moji <= 'Z'; ++moji) // ※ASC(65)=A、ASC(90)=Z
            {
                SearchChar = moji.ToString();

                myPos = wk_Char.Contains(SearchChar) ? wk_Char.IndexOf(SearchChar) + 1 : 0;
                if (wk_Char.Contains(SearchChar))
                {
                    if (svPos2 == 0)
                    {
                        svPos2 = myPos;
                    }                        
                    else if (myPos < svPos2)
                    {
                        svPos2 = myPos;
                    }                        
                }
            }

            // ◆"-"と"A-Z"の両方が含まれており、"-"より前に"A-Z"が存在する場合のみ出力文字をセットする
            if (svPos1 > 0 & svPos2 > 0)
            {
                if (svPos2 < svPos1)
                {
                    out_Char = (inStr.Substring(svPos2 - 1, svPos1 + 3)).TrimEnd();
                }
            }

            // ◆出力文字の最後に"0-9"以外の文字が含まれている場合は除く
            if (!string.IsNullOrEmpty(out_Char))
            {
                var chk_Str = out_Char.Substring(out_Char.Length - 1);
                if (System.Text.RegularExpressions.Regex.IsMatch(chk_Str, @"^[0-9]+$"))
                {
                }
                else
                {
                    out_Char = (out_Char.Substring(0, out_Char.Length - 1)).TrimEnd();
                }                    
            }

            // ◆出力文字に"A-Z"、"0-9"、"-"以外の文字が含まれていないかチェック
            if (!string.IsNullOrEmpty(out_Char))
            {
                chk_Cnt = 0;
                var j = out_Char.Length;
                for (var i = 0; i < j; i++)
                {
                    var chk_Str = out_Char.Substring(i, 1);
                    if (System.Text.RegularExpressions.Regex.IsMatch(chk_Str, @"^[A-Z]+$"))
                    {
                        chk_Cnt = chk_Cnt + 1;
                    }
                    if (System.Text.RegularExpressions.Regex.IsMatch(chk_Str, @"^[0-9]+$"))
                    {
                        chk_Cnt = chk_Cnt + 1;
                    }
                    if (chk_Str == "-")
                    {
                        chk_Cnt = chk_Cnt + 1;
                    }                        
                }
                if (chk_Cnt < j)
                {
                    out_Char = " ";
                }                    
            }
            return out_Char;
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
            if (wk_Char.Contains(SearchChar))
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
                res = dB_0127_HANSOKU_LIST.list.FirstOrDefault(x => x.得意先CD == tokcd && x.名称 == typenm && x.売単価 == baika);
            }
            return res;
        }

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return MaruyoshiItems.Value != null &&
                   MaruyoshiItems.Value.Any() &&
                   MaruyoshiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var path = @"c:\Program Files (x86)\MLV5\NEFUDA\";
            var fname = "0102" + "_" +
                        this.JusinDate.Value.ToString("yyyyMMdd") + "_" +
                        this.NouhinDate.Value.ToString("yyyyMMdd") + "_" +
                        this.BunruiCodeText.Value.ToString("000") + ".csv";
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
            var list = MaruyoshiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            list.ForEach(x => 
            { 
                x.サイズ = ""; 
                x.カラー = ""; 
                if(!string.IsNullOrEmpty(x.棚番))
                {
                    x.品番 = x.棚番;
                }
            });

            var datas = DataUtility.ToDataTable(list);
            // 不要なカラムの削除
            datas.Columns.Remove("棚番");
            datas.Columns.Remove("品名");
            datas.Columns.Remove("組");
            datas.Columns.Remove("FLG");
            datas.Columns.Remove("タグ");
            datas.Columns.Remove("消売価");
            datas.Columns.Remove("売価");
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
            var grpName = @"0102_マルヨシ\マルヨシセンター(総額対応)_V5 ST308R";
            var layName = @"通常貼り札.mllayx";
            var layNo = CommonStrings.MLV5LAYOUT_PATH + @"\" + grpName + @"\" + layName;
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

    public class MaruyoshiItem
    {
        public decimal 発行枚数 { get; set; }   //csv
        public string カラー { get; set; }
        public string サイズ { get; set; }
        public string シーズンコード { get; set; } //csv
        public string クラスCD { get; set; }   //csv
        public string 追加 { get; set; }  //csv
        public decimal 税込売価 { get; set; }   //csv
        public string 品番 { get; set; }  //csv
        public string 単品 { get; set; }  //csv
        public string JANコード { get; set; }  //csv        
        
        
        public string 棚番 { get; set; }
        public string 品名 { get; set; }                
        public string 組 { get; set; }
        public string FLG { get; set; }        
        public string タグ { get; set; }        
        public decimal 消売価 { get; set; }
        public decimal 売価 { get; set; }        

        public MaruyoshiItem(decimal 発行枚数, string クラスCD, string 品番, string 棚番,
                            string 品名, string カラー, string サイズ, string 単品, string 組, string FLG,
                            string 追加, string タグ, decimal 消売価, decimal 売価, decimal 税込売価, string JANコード,
                            string シーズンコード)
        {
            this.発行枚数 = 発行枚数;            
            this.クラスCD = クラスCD;
            this.品番 = 品番;
            this.棚番 = 棚番;
            this.品名 = 品名;
            this.カラー = カラー;
            this.サイズ = サイズ;
            this.単品 = 単品;
            this.組 = 組;
            this.FLG = FLG;
            this.追加 = 追加;
            this.タグ = タグ;
            this.消売価 = 消売価;
            this.売価 = 売価;
            this.税込売価 = 税込売価;
            this.JANコード = JANコード;
            this.シーズンコード = シーズンコード;
        }
    }

    public class MaruyoshiItemList
    {
        public IEnumerable<MaruyoshiItem> ConvertMaruyoshiDataToModel(List<MaruyoshiData> datas)
        {
            var result = new List<MaruyoshiItem>();
            var hinban = "";
            var jancd = "";
            datas.ForEach(data =>
            {
                hinban = !string.IsNullOrEmpty(data.NEFCMB) ? data.NEFCMB.TrimEnd() : data.NEFCMB2.TrimEnd();
                jancd = !string.IsNullOrEmpty(data.JANCD) ? data.JANCD : " ";
                result.Add(
                    new MaruyoshiItem(data.NEFSUA, data.NEFCMA, hinban, data.NEFCMB2, data.NEFCMC, data.NEFCMD,
                                      data.NEFCME + " " + data.NEFCMF, data.NEFCMG, data.NEFCMH, data.NEFCMI, data.NEFCMJ, 
                                      data.NEFCMK, data.NEFTKA, data.NEFTKB, data.NEFTKB2, jancd, data.NEFSEZ));
            });
            return result;
        }
    }
}
