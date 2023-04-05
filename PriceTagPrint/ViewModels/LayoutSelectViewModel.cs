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
using PriceTagPrint.Models;
using PriceTagPrint.WAGO2;
using Reactive.Bindings;

namespace PriceTagPrint.ViewModels
{
    public class LayoutItem
    { 
        public bool Actionable { get; set; }
        public string LayoutName { get; set; }
        public string LayoutPath { get; set; }

        public LayoutItem(bool actionable, string layoutName, string layoutPath)
        {
            this.Actionable = actionable;
            this.LayoutName = layoutName;
            this.LayoutPath = layoutPath;
        }
    }
    public class LayoutSelectViewModel : ViewModelsBase
    {
        public ReactiveProperty<string> TokuisakiName { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> DirectoryName { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<ObservableCollection<LayoutItem>> LayoutItems { get; set; }
                = new ReactiveProperty<ObservableCollection<LayoutItem>>();

        public ReactiveProperty<int> LayoutSelectedIndex { get; set; }
                = new ReactiveProperty<int>();
        public LayoutSelectViewModel()
        {
            LayoutItems.Value = new ObservableCollection<LayoutItem>();            
        }

        public void CreateItems(Torihikisaki tori)
        {
            TokuisakiName.Value = tori.Name + " 値札レイアウト選択";            
            // 振分発行の場合は.mldenx
            // 通常入力の場合は.mllayx
            foreach (var t in tori.LayoutDirs)
            {
                var searchExtension = t.IsAuto ? "*." + CommonStrings.AUTO_EXTENSION : "*." + CommonStrings.INPUT_EXTENSION;
                var files = Directory.EnumerateFiles(t.Path, searchExtension);
                DirectoryName.Value += string.IsNullOrEmpty(DirectoryName.Value) ? t.Path : Environment.NewLine + t.Path;
                foreach (var file in files)
                {
                    var fName = Path.GetFileNameWithoutExtension(file);
                    LayoutItems.Value.Add(new LayoutItem(true, fName, file));
                }
            }
        }

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
                case "F12":
                    if (!PrintCheck())
                    {
                        MessageBox.Show("対象データが存在しません。", "値札発行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (MessageBox.Show("値札の発行を行いますか？", "値札発行確認", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        ExecPrint();
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// F10プレビュー・F12印刷前データ確認
        /// </summary>
        /// <returns></returns>
        public bool PrintCheck()
        {
            return LayoutItems.Value != null &&
                   LayoutItems.Value.Any() &&
                   LayoutSelectedIndex.Value >= 0;
        }

        /// <summary>
        /// F10プレビュー・F12印刷 実処理
        /// </summary>
        /// <param name="isPreview"></param>
        public void ExecPrint()
        {
            var item = LayoutItems.Value.Select((value, index) => new { value, index })
                            .FirstOrDefault(x => x.index == LayoutSelectedIndex.Value)?.value ?? null;
            if(item != null)
            {
                MLVExecute(item);
            }
                           
        }

        public void Act_Click(LayoutItem item)
        {
            MLVExecute(item);
        }

        /// <summary>
        /// F10プレビュー・F12印刷 外部アプリ（MLV5）起動
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="isPreview"></param>
        private void MLVExecute(LayoutItem item)
        {           
            var layNo = item.LayoutPath;
            var dq = "\"";
            var args = dq + layNo + dq;

            //Processオブジェクトを作成する
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //起動する実行ファイルのパスを設定する
            p.StartInfo.FileName = CommonStrings.MLPRINTEXE_PATH;
            //コマンドライン引数を指定する
            p.StartInfo.Arguments = args;
            //起動する。プロセスが起動した時はTrueを返す。
            bool result = p.Start();
        }
    }
}
