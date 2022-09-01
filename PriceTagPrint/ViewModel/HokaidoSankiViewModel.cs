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
    public　class HokaidoSankiViewModel : ViewModelsBase
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

        // 開始相手品番
        public ReactiveProperty<string> SttHincd { get; set; } = new ReactiveProperty<string>("");
        // 終了相手品番
        public ReactiveProperty<string> EndHincd { get; set; } = new ReactiveProperty<string>("");

        //発行枚数計
        public ReactiveProperty<string> TotalMaisu { get; set; } = new ReactiveProperty<string>("");

        private List<HokaidoSankiData> HokaidoSankiDatas { get; set; } = new List<HokaidoSankiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<HokaidoSankiItem>> HokaidoSankiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<HokaidoSankiItem>>();

        #endregion

        private readonly string _grpName = @"\0121_北海道三喜\【総額新フォーマット】北海道三喜_V5_ST308R";

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
        public HokaidoSankiViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            eOSKNMTA_LIST = new EOSKNMTA_LIST();

            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
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
            item1.Id = 0;
            item1.Name = "0：三喜　白ラベルＪＡＮ";
            list.Add(item1);
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
            BunruiCodeText.Value = "";
            SelectedNefudaBangouIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            TotalMaisu.Value = "";
            HokaidoSankiDatas.Clear();
            if (HokaidoSankiItems.Value != null && HokaidoSankiItems.Value.Any())
            {
                HokaidoSankiItems.Value.Clear();
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
            if (!string.IsNullOrEmpty(this.BunruiCodeText.Value) && !BunruiCodeItems.Value.Select(x => x.Id.TrimEnd()).Contains(this.BunruiCodeText.Value))
            {
                MessageBox.Show("分類コードを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.HOKKAIDO_SANKI, JusinDate.Value, NouhinDate.Value);
                var easKnmtaList = eOSKNMTA_LIST.QueryWhereTcode(TidNum.HOKKAIDO_SANKI);

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
                                       VHINCD = eosj.VHINCD.TrimEnd(),
                                       VHINNMA = eosj.VHINNMA.TrimEnd(),
                                       HINCD = eosj.HINCD.TrimEnd(),
                                       VCYOBI3 = eosj.VCYOBI3.TrimEnd(),
                                       VCYOBI7 = eosj.VCYOBI7.TrimEnd(),
                                       QOLTORID = eosj.QOLTORID.TrimEnd(),
                                       VURITK = eosj.VURITK,
                                       VSURYO = eosj.VSURYO,
                                   })
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
                            decimal convdec;
                            HokaidoSankiDatas.Clear();
                            HokaidoSankiDatas.AddRange(
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
                                               VCYOBI3 = a.VCYOBI3,
                                               VCYOBI7 = a.VCYOBI7,
                                               HINBAN = !string.IsNullOrEmpty(a.VCYOBI7) ? a.VCYOBI7.Substring(3) : "",
                                               HINEDA = !string.IsNullOrEmpty(a.HINCD) ? a.HINCD.Substring(4) : "",
                                               VURITK = a.VURITK,
                                               JANCD = a.VHINCD,
                                               QOLTORID = a.QOLTORID,
                                               HINCD = a.HINCD,
                                               HINNM = hin.Any() ? hin.FirstOrDefault().HINNMA : "",
                                               EOSHINNM = a.VHINNMA,
                                               VSURYO = a.VSURYO,
                                           })
                                    .GroupBy(a => new
                                    {
                                        a.VRYOHNCD,
                                        a.VRYOHNNM,
                                        a.VRCVDT,
                                        a.VNOHINDT,
                                        a.VCYOBI3,
                                        a.VCYOBI7,
                                        a.HINBAN,
                                        a.HINEDA,
                                        a.VURITK,
                                        a.JANCD,
                                        a.QOLTORID,
                                        a.HINCD,
                                        a.HINNM,
                                        a.EOSHINNM,
                                    })
                                    .Select(g => new HokaidoSankiData()
                                    {
                                        VRYOHNCD = g.Key.VRYOHNCD,
                                        VRYOHNNM = g.Key.VRYOHNNM,
                                        VRCVDT = g.Key.VRCVDT,
                                        VNOHINDT = g.Key.VNOHINDT,
                                        VSURYO = g.Sum(y => y.VSURYO),
                                        VCYOBI3 = g.Key.VCYOBI3,
                                        VCYOBI7 = g.Key.VCYOBI7,
                                        HINBAN = g.Key.HINBAN,
                                        HINEDA = g.Key.HINEDA,
                                        VURITK = g.Key.VURITK,
                                        VHINCD = g.Key.JANCD,
                                        TNANM = TanabanCheck(g.Key.EOSHINNM),
                                        QOLTORID = g.Key.QOLTORID,
                                        HINCD = g.Key.HINCD,
                                        HINNM = g.Key.HINNM,
                                        EOSHINNM = g.Key.EOSHINNM
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

                        if (HokaidoSankiDatas.Any())
                        {
                            HokaidoSankiItems.Value = new ObservableCollection<HokaidoSankiItem>();
                            var HokaidoSankiModelList = new HokaidoSankiItemList();
                            var addItems = new ObservableCollection<HokaidoSankiItem>(HokaidoSankiModelList.ConvertHokaidoSankiDataToModel(HokaidoSankiDatas)).ToList();
                            // 直接ObservableにAddするとなぜか落ちるためListをかます。
                            var setItems = new List<HokaidoSankiItem>();
                            addItems.ForEach(item =>
                            {
                                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                      h => item.PropertyChanged += h,
                                      h => item.PropertyChanged -= h)
                                      .Subscribe(e =>
                                      {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = HokaidoSankiItems.Value.Sum(x => x.発行枚数).ToString();
                                      });
                                setItems.Add(item);
                            });
                            HokaidoSankiItems.Value = new ObservableCollection<HokaidoSankiItem>(setItems);
                            TotalMaisu.Value = HokaidoSankiItems.Value.Sum(x => x.発行枚数).ToString();
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

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return HokaidoSankiItems.Value != null &&
                   HokaidoSankiItems.Value.Any() &&
                   HokaidoSankiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.HOKKAIDO_SANKI + "_" +
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
            var list = HokaidoSankiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "メッセージ",
                "メッセージ_2",
                "大中分類",
                "EOS2桁",
                "売価",
                "JANコード",
                "取引先コード",
                "取引先後3",
                "品番",
                "納品日",
                "Eマーク",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用品番");
            datas.Columns.Remove("棚番");
            datas.Columns.Remove("品名");
            datas.Columns.Remove("表示用大中分類");
            datas.Columns.Remove("EOS品名");
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
            var layName = @"01 三喜　白ラベルＪＡＮ.mllayx";
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

    public class HokaidoSankiItem : INotifyPropertyChanged
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
        public string メッセージ { get; set; }     //csv
        public string メッセージ_2 { get; set; }     //csv & 表示
        public string JANコード { get; set; }  //csv & 表示
        public string 表示用品番 { get; set; }  //表示
        public string 品番 { get; set; }  //csv
        public string 棚番 { get; set; }  //表示
        public string 品名 { get; set; }  //表示
        public string 表示用大中分類 { get; set; } //表示
        public string 大中分類 { get; set; } //csv
        public decimal 売価 { get; set; }  //csv & 表示
        public string EOS2桁 { get; set; }   //csv
        public string 取引先コード { get; set; }   //csv
        public string 取引先後3 { get; set; }  //csv
        public string EOS品名 { get; set; }        
        public string 納品日 { get; set; }   //csv
        public string Eマーク { get; set; }    //csv

        public HokaidoSankiItem(decimal 発行枚数, string メッセージ, string メッセージ_2, string 表示用大中分類, string 大中分類, string EOS2桁,
                            decimal 売価, string JANコード, string 取引先コード, string 取引先後3, string 表示用品番, string 品番,
                            string 品名, string EOS品名, string 棚番, string 納品日, string Eマーク)
        {
            this.発行枚数 = 発行枚数;
            this.メッセージ = メッセージ;
            this.メッセージ_2 = メッセージ_2;
            this.表示用大中分類 = 表示用大中分類;
            this.大中分類 = 大中分類;
            this.EOS2桁 = EOS2桁;
            this.売価 = 売価;
            this.JANコード = JANコード;
            this.取引先コード = 取引先コード;
            this.取引先後3 = 取引先後3;
            this.表示用品番 = 表示用品番;
            this.品番 = 品番;
            this.品名 = 品名;
            this.EOS品名 = EOS品名;
            this.棚番 = 棚番;
            this.納品日 = 納品日;
            this.Eマーク = Eマーク;
        }
    }

    public class HokaidoSankiItemList
    {
        public IEnumerable<HokaidoSankiItem> ConvertHokaidoSankiDataToModel(List<HokaidoSankiData> datas)
        {
            var result = new List<HokaidoSankiItem>();
            var daichuBunrui = "";
            var nouhinbi = "";
            datas.ForEach(data =>
            {
                daichuBunrui = !string.IsNullOrEmpty(data.VCYOBI3) && data.VCYOBI3.Length == 4 ? data.VCYOBI3.Substring(0,2) + data.VCYOBI3.Substring(data.VCYOBI3.Length - 1) : "";
                nouhinbi = !string.IsNullOrEmpty(data.VNOHINDT) && data.VNOHINDT.Length == 8 ? data.VNOHINDT.Substring(2) : "";
                result.Add(
                    new HokaidoSankiItem(data.VSURYO, "", "", data.VCYOBI3, daichuBunrui, "00", data.VURITK,
                                      data.VHINCD, data.QOLTORID, "000", data.HINCD, data.HINEDA, data.HINNM,
                                      data.EOSHINNM, data.TNANM, nouhinbi, "1"));
            });
            return result;
        }
    }
}
