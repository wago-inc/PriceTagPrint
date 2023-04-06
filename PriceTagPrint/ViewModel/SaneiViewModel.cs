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
    public class SaneiViewModel : ViewModelsBase
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
        public ReactiveProperty<int> VbunCdText { get; set; }
        public ReactiveProperty<ObservableCollection<CommonIdName>> VbunCdItems { get; set; }
                = new ReactiveProperty<ObservableCollection<CommonIdName>>();
        public ReactiveProperty<int> SelectedVbunCdIndex { get; set; }
                = new ReactiveProperty<int>(0);

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

        private List<SaneiData> SaneiDatas { get; set; } = new List<SaneiData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<SaneiItem>> SaneiItems { get; set; }
                = new ReactiveProperty<ObservableCollection<SaneiItem>>();

        #endregion

        private readonly string _grpName = @"\【総額】サンエー_V5_RT308R";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private TOKMTE_LIST tOKMTE_LIST;
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
        public SaneiViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            tOKMTE_LIST = new TOKMTE_LIST();

            CreateComboItems();

            // コンボボックス初期値セット
            HakkouTypeText = new ReactiveProperty<int>(1);
            VbunCdText = new ReactiveProperty<int>(1);
            BunruiCodeText = new ReactiveProperty<string>("");
            NefudaBangouText = new ReactiveProperty<int>();

            // SubScribe定義
            HakkouTypeText.Subscribe(x => HakkouTypeTextChanged(x));
            VbunCdText.Subscribe(x => VbunCdTextChanged(x));
            BunruiCodeText.Subscribe(x => BunruiCodeTextChanged(x));
            NefudaBangouText.Subscribe(x => NefudaBangouTextChanged(x));

            SelectedHakkouTypeIndex.Subscribe(x => SelectedHakkouTypeIndexChanged(x));
            SelectedVbunCdIndex.Subscribe(x => SelectedVbunCdIndexChanged(x));
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
            VbunCdItems.Value = new ObservableCollection<CommonIdName>(CreateVbunCdItems());
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
        /// 分類コードItems生成
        /// </summary>
        /// <returns></returns>
        public List<CommonIdName> CreateVbunCdItems()
        {
            var list = new List<CommonIdName>();
            var item = new CommonIdName();
            item.Id = 1842;
            item.Name = "1842：レッグ";
            list.Add(item);
            var item2 = new CommonIdName();
            item2.Id = 1837;
            item2.Name = "1837：婦人インナー";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 1816;
            item3.Name = "1816：紳士インナー";
            list.Add(item3);
            return list.OrderBy(x => x.Id).ToList();
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
            item1.Name = "1：貼り札 白無地21号_JAN1段";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 2;
            item2.Name = "2：下げ札 白無地11号_JAN1段";
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
        private void VbunCdTextChanged(int id)
        {
            var item = VbunCdItems.Value.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                SelectedVbunCdIndex.Value = VbunCdItems.Value.IndexOf(item);
            }
        }

        /// <summary>
        /// 和合分類コードテキスト変更処理
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
        private void SelectedVbunCdIndexChanged(int idx)
        {
            var item = VbunCdItems.Value.Where((item, index) => index == idx).FirstOrDefault();
            if (item != null)
            {
                VbunCdText.Value = item.Id;
            }
            else
            {
                VbunCdText.Value = 0;
            }
        }

        /// <summary>
        /// 和合分類コードコンボ変更処理
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
            SelectedVbunCdIndex.Value = 0;
            BunruiCodeText.Value = "";
            SelectedNefudaBangouIndex.Value = 0;
            SttHincd.Value = "";
            EndHincd.Value = "";
            TotalMaisu.Value = "";
            SaneiDatas.Clear();
            if (SaneiItems.Value != null && SaneiItems.Value.Any())
            {
                SaneiItems.Value.Clear();
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
            if (string.IsNullOrEmpty(this.VbunCdText.Value.ToString()) || (this.VbunCdText.Value != 1842 && this.VbunCdText.Value != 1837 && this.VbunCdText.Value != 1816))
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
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.SANEI, JusinDate.Value, NouhinDate.Value);
                var tokmteList = tOKMTE_LIST.QueryWhereTcode(TidNum.SANEI);

                if (eosJutraList.Any() && tokmteList.Any())
                {
                    var innerJoinData = eosJutraList
                            .GroupJoin(
                                   tokmteList,
                                   e1 => new
                                   {
                                       VHINCD = e1.VHINCD.TrimEnd(),
                                       VRYOHNCD = e1.VRYOHNCD.ToString()
                                   },
                                   e2 => new
                                   {
                                       VHINCD = e2.EOSHINID.TrimEnd(),
                                       VRYOHNCD = e2.TOKCD.ToString()
                                   },
                                   (eosj, tkmte) => new
                                   {
                                       VRYOHNCD = eosj.VRYOHNCD,
                                       VRCVDT = eosj.VRCVDT,
                                       VNOHINDT = eosj.VNOHINDT,
                                       VBUNCD = eosj.VBUNCD,
                                       VHINCD = eosj.VHINCD.TrimEnd(),
                                       VHINNMA = eosj.VHINNMA.TrimEnd(),
                                       HINCD = eosj.HINCD.TrimEnd(),
                                       QOLTORID = eosj.QOLTORID.TrimEnd(),
                                       VURITK = eosj.VURITK,
                                       VIRISU = eosj.VIRISU,
                                       VSURYO = eosj.VSURYO,                                       
                                       VSIZNM = eosj.VSIZNM.TrimEnd(),
                                       VCOLNM = eosj.VCOLNM.TrimEnd(),
                                       EOSHINNA = tkmte.Any() ? tkmte.FirstOrDefault().EOSHINNA.TrimEnd() : "",
                                       SIZCD = tkmte.Any() ? tkmte.FirstOrDefault().SIZCD.TrimEnd() : "",
                                       EOSURITK = tkmte.Any() ? tkmte.FirstOrDefault().EOSURITK : 0,
                                   })
                            .Where(x => x.VBUNCD.TrimEnd() == this.VbunCdText.Value.ToString())
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
                            SaneiDatas.Clear();
                            SaneiDatas.AddRange(
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
                                               VRCVDT = a.VRCVDT,
                                               VNOHINDT = a.VNOHINDT,
                                               VHINCD = a.VHINCD,
                                               VHINNMA = a.VHINNMA,
                                               HINCD = a.HINCD,
                                               HINID = hin.Any() ? hin.FirstOrDefault().HINID.TrimEnd() : "",
                                               QOLTORID = a.QOLTORID,
                                               VIRISU = a.VIRISU,
                                               VURITK = a.EOSURITK > 0 ? a.EOSURITK : a.VURITK,
                                               VSURYO = a.VSURYO,
                                               HINNM = hin.Any() ? hin.FirstOrDefault().HINNMA : "",                                               
                                               VSIZNM = !string.IsNullOrEmpty(a.VSIZNM) ? a.VSIZNM : "　",
                                               VCOLNM = !string.IsNullOrEmpty(a.VCOLNM) ? a.VCOLNM : "　",
                                               MESSAGE = !string.IsNullOrEmpty(a.EOSHINNA) ? a.EOSHINNA : " ",
                                               BUMONCD = !string.IsNullOrEmpty(a.VBUNCD) && a.VBUNCD.Length > 3 ?
                                                            a.VBUNCD.Substring(0, 3) == "184" ? "1" :
                                                            a.VBUNCD.Substring(0, 3) == "183" ? "2" :
                                                            a.VBUNCD.Substring(0, 3) == "181" ? "3" :
                                                            a.VBUNCD.Substring(0, 3) == "182" ? "4" : "　"
                                                            : "　",
                                               TUIKA_KBN = "1",
                                               SET_INFO = "S",
                                           })
                                    .GroupBy(a => new
                                    {
                                        a.VRYOHNCD,
                                        a.VRCVDT,
                                        a.VNOHINDT,
                                        a.VHINCD,
                                        a.VHINNMA,
                                        a.HINCD,
                                        a.HINID,
                                        a.QOLTORID,
                                        a.VIRISU,
                                        a.VURITK,                                      
                                        a.HINNM,
                                        a.VSIZNM,
                                        a.VCOLNM,
                                        a.MESSAGE,
                                        a.BUMONCD,
                                        a.TUIKA_KBN,
                                        a.SET_INFO,
                                    })
                                    .Select(g => new SaneiData()
                                    {
                                        VRYOHNCD = g.Key.VRYOHNCD,
                                        VRCVDT = g.Key.VRCVDT,
                                        VNOHINDT = g.Key.VNOHINDT,
                                        VHINCD = g.Key.VHINCD,
                                        VHINNMA = g.Key.VHINNMA,
                                        HINCD = g.Key.HINCD,
                                        HINID = g.Key.HINID,
                                        QOLTORID = g.Key.QOLTORID,
                                        VIRISU = g.Key.VIRISU,
                                        VURITK = g.Key.VURITK,
                                        VSURYO = g.Sum(y => y.VSURYO),
                                        DSPHINNM = g.Key.HINNM,
                                        VSIZNM = g.Key.VSIZNM,
                                        VCOLNM = g.Key.VCOLNM,
                                        MESSAGE = g.Key.MESSAGE,
                                        BUMONCD = g.Key.BUMONCD,
                                        TUIKA_KBN = g.Key.TUIKA_KBN,
                                        SET_INFO = g.Key.SET_INFO,
                                    })
                                    .Where(x => !string.IsNullOrEmpty(this.SttHincd.Value) ?
                                                    int.TryParse(this.SttHincd.Value, out sttHincd) &&
                                                    int.TryParse(x.HINID, out aitSttHincd) ?
                                                        aitSttHincd >= sttHincd : true
                                                : true)
                                    .Where(x => !string.IsNullOrEmpty(this.EndHincd.Value) ?
                                                    int.TryParse(this.EndHincd.Value, out endHincd) &&
                                                    int.TryParse(x.HINID, out aitEndHincd) ?
                                                        aitEndHincd <= endHincd : true
                                                : true)
                                    .OrderBy(x => x.VRYOHNCD)
                                    .ThenBy(x => x.VRCVDT)
                                    .ThenBy(x => x.VHINCD)
                                    );
                        }

                        if (SaneiDatas.Any())
                        {
                            SaneiItems.Value = new ObservableCollection<SaneiItem>();
                            var SaneiModelList = new SaneiItemList();
                            var addItems = new ObservableCollection<SaneiItem>(SaneiModelList.ConvertSaneiDataToModel(SaneiDatas)).ToList();
                            // 直接ObservableにAddするとなぜか落ちるためListをかます。
                            var setItems = new List<SaneiItem>();
                            addItems.ForEach(item =>
                            {
                                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                      h => item.PropertyChanged += h,
                                      h => item.PropertyChanged -= h)
                                      .Subscribe(e =>
                                      {
                                          // 発行枚数に変更があったら合計発行枚数も変更する
                                          TotalMaisu.Value = SaneiItems.Value.Sum(x => x.発行枚数).ToString();
                                      });
                                setItems.Add(item);
                            });
                            SaneiItems.Value = new ObservableCollection<SaneiItem>(setItems);
                            TotalMaisu.Value = SaneiItems.Value.Sum(x => x.発行枚数).ToString();
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
            return SaneiItems.Value != null &&
                   SaneiItems.Value.Any() &&
                   SaneiItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.SANEI + "_" +
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
            var list = SaneiItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "納品月",
                "納品日",
                "追加情報",
                "部門コード",
                "仕入先コード",
                "JANコード",
                "品番",
                "カラー名",
                "サイズ名",
                "セット情報",
                "セット数",
                "メッセージ",
                "売価",
                "値下げラベルNo",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            // 不要なカラムの削除
            datas.Columns.Remove("表示用商品名");
            datas.Columns.Remove("表示用セット情報");
            datas.Columns.Remove("リスト表示商品名");
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
                            ? @"12_貼り札（大）白無地21号_JAN1段（税込自動計算）.mllayx"
                            : @"13_下げ札（小）白無地11号_JAN1段（税込自動計算）.mllayx";
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

    public class SaneiItem : INotifyPropertyChanged
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
        public string 納品月 { get; set; }    //csv
        public string 納品日 { get; set; }    //csv
        public string 追加情報 { get; set; }   //csv
        public string 部門コード { get; set; }  //csv
        public string 仕入先コード { get; set; }     //csv
        public string JANコード { get; set; }   //csv & 表示
        public string 品番 { get; set; }    //csv & 表示
        public string 表示用商品名 { get; set; }   //表示
        public string カラー名 { get; set; }  //csv & 表示
        public string サイズ名 { get; set; }  //csv & 表示
        public string セット情報 { get; set; }   //csv        
        public decimal セット数 { get; set; }  //csv
        public string メッセージ { get; set; }  //csv
        public decimal 売価 { get; set; }  //csv & 表示
        public string 値下げラベルNo { get; set; } //csv      
        public string 表示用セット情報 { get; set; }    //表示
        public string リスト表示商品名 { get; set; }   //表示
        public SaneiItem(decimal 発行枚数, string 納品月, string 納品日, string 追加情報, string 部門コード, string 仕入先コード,
                            string JANコード, string 品番, string 表示用商品名, string サイズ名, string カラー名,
                            string セット情報, decimal セット数, string メッセージ, decimal 売価, string 値下げラベルNo, 
                            string 表示用セット情報, string リスト表示商品名)
        {
            this.発行枚数 = 発行枚数;
            this.納品月 = 納品月;
            this.納品日 = 納品日;
            this.追加情報 = 追加情報;
            this.部門コード = 部門コード;
            this.仕入先コード = 仕入先コード;
            this.JANコード = JANコード;
            this.品番 = 品番;
            this.表示用商品名 = 表示用商品名;
            this.サイズ名 = サイズ名;
            this.カラー名 = カラー名;
            this.セット情報 = セット情報;
            this.セット数 = セット数;
            this.メッセージ = メッセージ;
            this.売価 = 売価;
            this.値下げラベルNo = 値下げラベルNo;
            this.表示用セット情報 = 表示用セット情報;
            this.リスト表示商品名 = リスト表示商品名;
        }
    }

    public class SaneiItemList
    {
        public IEnumerable<SaneiItem> ConvertSaneiDataToModel(List<SaneiData> datas)
        {
            var result = new List<SaneiItem>();
            DateTime convDate;
            string 納品月 = "";
            string 納品日 = "";
            string サイズ名 = "";
            string カラー名 = "";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            datas.ForEach(data =>
            {
                convDate = DateTime.ParseExact(data.VNOHINDT, "yyyyMMdd", null);
                納品月 = convDate != null ? convDate.Month.ToString() : "";
                納品日 = convDate != null ? convDate.Day.ToString() : "";                
                カラー名 = Microsoft.VisualBasic.Strings.StrConv(data.VCOLNM, Microsoft.VisualBasic.VbStrConv.Wide, 0x411);
                サイズ名 = Microsoft.VisualBasic.Strings.StrConv(data.VSIZNM, Microsoft.VisualBasic.VbStrConv.Wide, 0x411);
                result.Add(
                    new SaneiItem(data.VSURYO, 納品月, 納品日, data.TUIKA_KBN, data.BUMONCD, data.QOLTORID, data.VHINCD,
                                  data.HINCD, data.DSPHINNM, サイズ名, カラー名, data.SET_INFO,
                                  data.VIRISU, data.MESSAGE, data.VURITK, string.Empty, data.SET_INFO + data.VIRISU, data.VHINNMA));
            });
            return result;
        }
    }
}
