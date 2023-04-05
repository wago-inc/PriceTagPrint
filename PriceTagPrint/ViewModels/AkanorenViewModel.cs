using PriceTagPrint.Common;
using PriceTagPrint.MDB;
using PriceTagPrint.Models;
using PriceTagPrint.Views;
using PriceTagPrint.WAG_USR1;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections;
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

namespace PriceTagPrint.ViewModels
{
    public class AkanorenViewModel : ViewModelsBase
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
        public ReactiveProperty<ObservableCollection<AKBUNRUICD>> BunruiCodeItems { get; set; }
                = new ReactiveProperty<ObservableCollection<AKBUNRUICD>>();
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

        private List<AkanorenData> AkanorenDatas { get; set; } = new List<AkanorenData>();
        // DataGrid Items
        public ReactiveProperty<ObservableCollection<AkanorenItem>> AkanorenItems { get; set; }
                = new ReactiveProperty<ObservableCollection<AkanorenItem>>();

        #endregion

        private readonly string _grpName = @"\0165_あかのれん";

        // 発行区分テキストボックス
        public TextBox HakkouTypeTextBox = null;
        public DatePicker JusinDatePicker = null;
        public DatePicker NouhinDatePicker = null;

        private EOSJUTRA_LIST eOSJUTRA_LIST;
        private EOSAKTRA_LIST eOSAKTRA_LIST;
        private HINMTA_LIST hINMTA_LIST;
        private List<HINMTA> hinmtaList;

        #region コマンドの実装

        private RelayCommand<string> funcActionCommand;
        public RelayCommand<string> FuncActionCommand
        {
            get { return funcActionCommand = funcActionCommand ?? new RelayCommand<string>(FuncAction); }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AkanorenViewModel()
        {
            eOSJUTRA_LIST = new EOSJUTRA_LIST();
            eOSAKTRA_LIST = new EOSAKTRA_LIST();

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
            var bunruis = new AKBUNRUICDLIST().list;
            bunruis.Insert(0, new AKBUNRUICD("", "", "", ""));
            HakkouTypeItems.Value = new ObservableCollection<CommonIdName>(CreateHakkouTypeItems());
            BunruiCodeItems.Value = new ObservableCollection<AKBUNRUICD>(bunruis);
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
            item1.Id = 51;
            item1.Name = "51-提げ札(大)";
            list.Add(item1);
            var item2 = new CommonIdName();
            item2.Id = 52;
            item2.Name = "52-提げ札(中)";
            list.Add(item2);
            var item3 = new CommonIdName();
            item3.Id = 55;
            item3.Name = "55-貼り札(中)";
            list.Add(item3);
            var item4 = new CommonIdName();
            item4.Id = 56;
            item4.Name = "56-貼り札(小)";
            list.Add(item4);
            var item5 = new CommonIdName();
            item5.Id = 67;
            item5.Name = "67-提げ札(大) お買得札";
            list.Add(item5);
            var item6 = new CommonIdName();
            item6.Id = 68;
            item6.Name = "68-提げ札(中) お買得札";
            list.Add(item6);
            var item7 = new CommonIdName();
            item7.Id = 69;
            item7.Name = "69-貼り札(中) お買得札";
            list.Add(item7);
            var item8 = new CommonIdName();
            item8.Id = 81;
            item8.Name = "81-アレン提げ札(大)";
            list.Add(item8);
            var item9 = new CommonIdName();
            item9.Id = 85;
            item9.Name = "85-アレン提げ札(大) ＳＡＬＥ";
            list.Add(item9);
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
        private void BunruiCodeTextChanged(string cd)
        {
            var item = BunruiCodeItems.Value.FirstOrDefault(x => x.BUNRUICD.TrimEnd() == cd.TrimEnd());
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
            if (item != null && !string.IsNullOrEmpty(item.BUNRUICD))
            {
                BunruiCodeText.Value = item.BUNRUICD.TrimEnd();
                var nefudas = eOSAKTRA_LIST.QueryNefudanoWhereDatno(TidNum.AKANOREN, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value);
                if(nefudas.Any() && nefudas.Count == 1)
                {
                    int convNefda;
                    if(int.TryParse(nefudas.First(), out convNefda))
                    {
                        var nefitem = NefudaBangouItems.Value.FirstOrDefault(x => x.Id == convNefda);
                        if (nefitem != null)
                        {
                            SelectedNefudaBangouIndex.Value = NefudaBangouItems.Value.IndexOf(nefitem);
                        }
                    }                    
                }
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
            AkanorenDatas.Clear();
            if (AkanorenItems.Value != null && AkanorenItems.Value.Any())
            {
                AkanorenItems.Value.Clear();
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
            if (string.IsNullOrEmpty(this.BunruiCodeText.Value) || (!string.IsNullOrEmpty(this.BunruiCodeText.Value) && !BunruiCodeItems.Value.Select(x => x.BUNRUICD.TrimEnd()).Contains(this.BunruiCodeText.Value)))
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
                var eosJutraList = eOSJUTRA_LIST.QueryWhereTcodeAndDates(TidNum.AKANOREN, JusinDate.Value, NouhinDate.Value, BunruiCodeText.Value);

                var datnos = new List<string>();
                if (eosJutraList.Any())
                {
                    datnos.AddRange(eosJutraList.GroupBy(x => x.DATNO).Select(g => g.Key).ToList());
                }
                List<EOSAKTRA> eosAktraList = new List<EOSAKTRA>();
                foreach (var datno in datnos)
                {
                    eosAktraList.AddRange(eOSAKTRA_LIST.QueryWhereDatno(datno));
                }
                if (eosAktraList.Any(x => x.AKA015 == NefudaBangouText.Value.ToString()))
                {
                    decimal cTanka = 0;
                    int sttHincd;
                    int endHincd;
                    int aitSttHincd;
                    int aitEndHincd;

                    var tmpData = eosAktraList
                        .Where(x => x.AKA015 == NefudaBangouText.Value.ToString())
                        .Join(
                            eosJutraList,
                            e1 => new
                            {
                                DATNO = e1.DATNO,
                                VROWNO = e1.VROWNO
                            },
                            e2 => new
                            {
                                DATNO = e2.DATNO,
                                VROWNO = e2.VROWNO
                            },
                            (aktra, jutra) => new
                            {
                                MAISU = jutra.VSURYO,
                                TORISAKICD = aktra.AKA023,
                                SIRJOKEN = aktra.AKA026,
                                SEASON = aktra.AKA036,
                                TONYTUKI = aktra.AKA037,
                                ENDTUKI = aktra.AKA038,
                                URITYPE = aktra.AKA016,
                                BUMOCD = aktra.AKA017,
                                CLASSCD = aktra.AKA018,
                                SHOHINSYUCD = aktra.AKA019,
                                AKHINBAN = aktra.AKA020,
                                COLCD = aktra.AKA021,
                                SIZCD = aktra.AKA022,
                                COLSIZPTCD = aktra.AKA039,
                                ADRNO = aktra.AKA035,
                                TAGINFO = aktra.AKA025,
                                WGHINBAN = !string.IsNullOrEmpty(jutra.HINCD.TrimEnd()) && jutra.HINCD.Contains("-") ? jutra.HINCD.TrimEnd().Substring(jutra.HINCD.IndexOf("-") + 1) : "",
                                HTANKA = decimal.TryParse(aktra.AKA030, out cTanka) ? cTanka : 0,
                                VRCVDT = jutra.VRCVDT,
                                HINCD = jutra.HINCD.TrimEnd(),
                                VHINCD = jutra.VHINCD.TrimEnd(),
                                VBUNCD = jutra.VBUNCD.TrimEnd(),
                                HINBAN = !string.IsNullOrEmpty(jutra.VCYOBI7) ? jutra.VCYOBI7.Substring(4).TrimEnd() : "",
                            })
                        .Where(x => (!string.IsNullOrEmpty(this.SttHincd.Value) ?
                                            int.TryParse(this.SttHincd.Value, out sttHincd) && int.TryParse(x.HINBAN, out aitSttHincd) ?
                                                aitSttHincd >= sttHincd : true
                                        : true) &&
                                    (!string.IsNullOrEmpty(this.EndHincd.Value) ?
                                            int.TryParse(this.EndHincd.Value, out endHincd) && int.TryParse(x.HINBAN, out aitEndHincd) ?
                                                aitEndHincd <= endHincd : true
                                        : true))
                            .OrderBy(x => x.VRCVDT)
                            .ThenBy(x => x.VHINCD);

                    
                    AkanorenDatas.Clear();
                    AkanorenDatas.AddRange(
                        tmpData.
                            GroupBy(a => new
                            {
                                a.TORISAKICD,
                                a.SIRJOKEN,
                                a.SEASON,
                                a.TONYTUKI,
                                a.ENDTUKI,
                                a.URITYPE,
                                a.BUMOCD,
                                a.CLASSCD,
                                a.SHOHINSYUCD,
                                a.AKHINBAN,
                                a.COLCD,
                                a.SIZCD,
                                a.COLSIZPTCD,
                                a.ADRNO,
                                a.TAGINFO,
                                a.WGHINBAN,
                                a.HTANKA,
                                a.VRCVDT,
                                a.HINCD,
                                a.VHINCD,
                            })
                            .Select(g => new AkanorenData()
                            {
                                MAISU = g.Sum(y => y.MAISU),
                                TORISAKICD = g.Key.TORISAKICD,
                                SIRJOKEN = g.Key.SIRJOKEN,
                                SEASON = g.Key.SEASON,
                                TONYTUKI = g.Key.TONYTUKI,
                                ENDTUKI = g.Key.ENDTUKI,
                                URITYPE = g.Key.URITYPE,
                                BUMOCD = g.Key.BUMOCD,
                                CLASSCD = g.Key.CLASSCD,
                                SHOHINSYUCD = g.Key.SHOHINSYUCD,
                                AKHINBAN = g.Key.AKHINBAN,
                                DISPAKHINBAN = g.Key.VHINCD,
                                COLCD = g.Key.COLCD,
                                SIZCD = g.Key.SIZCD,
                                COLSIZPTCD = g.Key.COLSIZPTCD,
                                ADRNO = g.Key.ADRNO,
                                TAGINFO = g.Key.TAGINFO,
                                WGHINBAN = g.Key.WGHINBAN,
                                DISPHINBAN = g.Key.HINCD,
                                HTANKA = g.Key.HTANKA,
                            })
                        );

                    if (AkanorenDatas.Any())
                    {
                        AkanorenItems.Value = new ObservableCollection<AkanorenItem>();
                        var AkanorenModelList = new AkanorenItemList();
                        var addItems = new ObservableCollection<AkanorenItem>(AkanorenModelList.ConvertAkanorenDataToModel(AkanorenDatas)).ToList();
                        // 直接ObservableにAddするとなぜか落ちるためListをかます。
                        var setItems = new List<AkanorenItem>();
                        addItems.ForEach(item =>
                        {
                            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                  h => item.PropertyChanged += h,
                                  h => item.PropertyChanged -= h)
                                  .Subscribe(e =>
                                  {
                                      // 発行枚数に変更があったら合計発行枚数も変更する
                                      TotalMaisu.Value = AkanorenItems.Value.Sum(x => x.発行枚数).ToString();
                                  });
                            setItems.Add(item);
                        });
                        AkanorenItems.Value = new ObservableCollection<AkanorenItem>(setItems);
                        TotalMaisu.Value = AkanorenItems.Value.Sum(x => x.発行枚数).ToString();
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
            return AkanorenItems.Value != null &&
                   AkanorenItems.Value.Any() &&
                   AkanorenItems.Value.Sum(x => x.発行枚数) > 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint(bool isPreview)
        {
            var fname = Tid.AKANOREN + "_" + this.BunruiCodeText.Value + "_" +
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
            var list = AkanorenItems.Value.Where(x => x.発行枚数 > 0).ToList();
            var csvColSort = new string[]
            {
                "取引先コード",
                "仕入条件",
                "季節区分",
                "投入月",
                "販売終了月",
                "売出区分",
                "部門コード",
                "クラスコード",
                "小品種コード",
                "自社品番",
                "カラーコード",
                "サイズコード",
                "色サイズパターンコード",
                "アドレスNo",
                "タグ情報",
                "和合品番",
                "標準価格",
                "発行枚数"
            };
            var datas = DataUtility.ToDataTable(list, csvColSort);
            datas.Columns.Remove("表示用自社品番");
            datas.Columns.Remove("表示用和合品番");
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
            var layName = NefudaBangouItems.Value.FirstOrDefault(x => x.Name.StartsWith(NefudaBangouText.Value.ToString()))?.Name + @".mllayx" ?? "";
            if (string.IsNullOrEmpty(layName))
            {
                MessageBox.Show("レイアウトファイルが見つかりません。", "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

    public class AkanorenItem : INotifyPropertyChanged
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
        public string 取引先コード { get; set; }  //csv
        public string 仕入条件 { get; set; }    //csv
        public string 季節区分 { get; set; }    //csv
        public string 投入月 { get; set; }     //csv
        public string 販売終了月 { get; set; }   //csv   値札データ.終了月
        public string 売出区分 { get; set; }    //csv & 表示
        public string 部門コード { get; set; }   //csv & 表示
        public string クラスコード { get; set; }  //csv & 表示  値札データ.品種コード
        public string 小品種コード { get; set; }  //csv & 表示  値札データ.品目コード
        public string 自社品番 { get; set; }   //csv   値札データ.連番
        public string 表示用自社品番 { get; set; }   //表示
        public string カラーコード { get; set; }  //csv & 表示
        public string サイズコード { get; set; }  //csv & 表示
        public string 色サイズパターンコード { get; set; } //csv & 表示
        public string アドレスNo { get; set; }  //csv & 表示
        public string タグ情報 { get; set; }    //csv & 表示  値札データ.値札情報
        public string 和合品番 { get; set; }   //csv
        public string 表示用和合品番 { get; set; }   //表示
        public decimal 標準価格 { get; set; }   //csv & 表示  値札データ.標準売単価

        public AkanorenItem(decimal 発行枚数, string 取引先コード, string 仕入条件, string 季節区分, string 投入月, string 販売終了月,
                            string 売出区分, string 部門コード, string クラスコード, string 小品種コード, string 自社品番, string 表示用自社品番, string カラーコード,
                            string サイズコード, string 色サイズパターンコード, string アドレスNo, string タグ情報, string 和合品番, string 表示用和合品番, decimal 標準価格)
        {
            this.発行枚数 = 発行枚数;
            this.取引先コード = 取引先コード;
            this.仕入条件 = 仕入条件;
            this.季節区分 = 季節区分;
            this.投入月 = 投入月;
            this.販売終了月 = 販売終了月;
            this.売出区分 = 売出区分;
            this.部門コード = 部門コード;
            this.クラスコード = クラスコード;
            this.小品種コード = 小品種コード;
            this.自社品番 = 自社品番;
            this.表示用自社品番 = 表示用自社品番;
            this.カラーコード = カラーコード;
            this.サイズコード = サイズコード;
            this.色サイズパターンコード = 色サイズパターンコード;
            this.アドレスNo = アドレスNo;
            this.タグ情報 = タグ情報;
            this.和合品番 = 和合品番;
            this.表示用和合品番 = 表示用和合品番;
            this.標準価格 = 標準価格;
        }
    }

    public class AkanorenItemList
    {
        public IEnumerable<AkanorenItem> ConvertAkanorenDataToModel(List<AkanorenData> datas)
        {
            var result = new List<AkanorenItem>();
            datas.ForEach(data =>
            {
                result.Add(
                    new AkanorenItem(data.MAISU, data.TORISAKICD, data.SIRJOKEN, data.SEASON, data.TONYTUKI, data.ENDTUKI, data.URITYPE,
                                     data.BUMOCD, data.CLASSCD, data.SHOHINSYUCD, data.AKHINBAN, data.DISPAKHINBAN, data.COLCD, data.SIZCD,
                                     data.COLSIZPTCD, data.ADRNO, data.TAGINFO, data.WGHINBAN, data.DISPHINBAN, data.HTANKA));
            });
            return result;
        }
    }
}
