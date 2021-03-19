using PriceTagPrint.Model;
using PriceTagPrint.ViewModel;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PriceTagPrint.View
{
    /// <summary>
    /// YasusakiView.xaml の相互作用ロジック
    /// </summary>
    public partial class YasusakiView : Window
    {
        /// <summary>
        /// 接続文字列
        /// </summary>
        private static string MDB_FILE = @"\\Server00\h\database\得意先商品台帳\得意先商品台帳.mdb";
        private ReactiveProperty<ObservableCollection<YasusakiModel>> YasusakiItems { get; set; } = 
            new ReactiveProperty<ObservableCollection<YasusakiModel>>();

        public YasusakiView()
        {
            InitializeComponent();
        }

        private void F5Action(object sender, RoutedEventArgs e)
        {
            string strSQL;
            strSQL = "SELECT " + Environment.NewLine;
            strSQL += " A.HNO, " + Environment.NewLine;
            strSQL += " A.TOKCD, " + Environment.NewLine;
            strSQL += " A.SYOHINCD, " + Environment.NewLine;
            strSQL += " A.JANCD, " + Environment.NewLine;
            strSQL += " A.BUNRUI, " + Environment.NewLine;
            strSQL += " A.SCODE, " + Environment.NewLine;
            strSQL += " A.SAIZUS, " + Environment.NewLine;
            strSQL += " A.HINCD, " + Environment.NewLine;
            strSQL += " A.HATYUBI, " + Environment.NewLine;
            strSQL += " A.NOUHINBI, " + Environment.NewLine;
            strSQL += " sum(A.NSU) AS NSU_G, " + Environment.NewLine;
            strSQL += " A.BAIKA, " + Environment.NewLine;
            strSQL += " A.EOS_SYOHINNM, " + Environment.NewLine;
            strSQL += " A.GENKA, " + Environment.NewLine;
            strSQL += " B.SKBN, " + Environment.NewLine;
            strSQL += " B.NEFUDA_KBN, " + Environment.NewLine;
            strSQL += " B.NETUKE_BUNRUI, " + Environment.NewLine;
            strSQL += " B.BIKOU1, " + Environment.NewLine;
            strSQL += " B.BIKOU2 " + Environment.NewLine;
            strSQL += "FROM " + Environment.NewLine;
            strSQL += "( " + Environment.NewLine;
            strSQL += "SELECT *, " + Environment.NewLine;
            strSQL += " val(TOKCD) AS TCODE, " + Environment.NewLine;
            strSQL += " val(SCODE) As HCODE " + Environment.NewLine;
            strSQL += "FROM " + Environment.NewLine;
            strSQL += " 0112_EOS_HACHU " + Environment.NewLine;
            strSQL += "WHERE " + Environment.NewLine;
            strSQL += " HNO = 228585 " + Environment.NewLine;
            strSQL += ") A " + Environment.NewLine;
            strSQL += "INNER JOIN " + Environment.NewLine;
            strSQL += "( " + Environment.NewLine;
            strSQL += "SELECT * " + Environment.NewLine;
            strSQL += "FROM " + Environment.NewLine;
            strSQL += " TOKSYOMS " + Environment.NewLine;
            strSQL += "WHERE " + Environment.NewLine;
            strSQL += " TCODE = 112 " + Environment.NewLine;
            strSQL += " AND TENPO = 9999 " + Environment.NewLine;
            strSQL += ") B " + Environment.NewLine;
            strSQL += "ON " + Environment.NewLine;
            strSQL += " A.TCODE = B.TCODE " + Environment.NewLine;
            strSQL += " AND A.BUNRUI = B.BUNRUI " + Environment.NewLine;
            strSQL += " AND A.HCODE = B.HCODE " + Environment.NewLine;
            strSQL += " AND A.SAIZUS = B.SAIZU " + Environment.NewLine;
            strSQL += "GROUP BY " + Environment.NewLine;
            strSQL += " A.HNO, " + Environment.NewLine;
            strSQL += " A.TOKCD, " + Environment.NewLine;
            strSQL += " A.SYOHINCD, " + Environment.NewLine;
            strSQL += " A.JANCD, " + Environment.NewLine;
            strSQL += " A.BUNRUI, " + Environment.NewLine;
            strSQL += " A.SCODE, " + Environment.NewLine;
            strSQL += " A.SAIZUS, " + Environment.NewLine;
            strSQL += " A.HINCD, " + Environment.NewLine;
            strSQL += " A.HATYUBI, " + Environment.NewLine;
            strSQL += " A.NOUHINBI, " + Environment.NewLine;
            strSQL += " A.BAIKA, ";
            strSQL += " A.EOS_SYOHINNM, " + Environment.NewLine;
            strSQL += " A.GENKA, " + Environment.NewLine;
            strSQL += " B.SKBN, " + Environment.NewLine;
            strSQL += " B.NEFUDA_KBN, " + Environment.NewLine;
            strSQL += " B.NETUKE_BUNRUI, " + Environment.NewLine;
            strSQL += " B.BIKOU1, " + Environment.NewLine;
            strSQL += " B.BIKOU2 " + Environment.NewLine;
            strSQL += "HAVING " + Environment.NewLine;
            strSQL += " Sum (A.NSU) > 0 " + Environment.NewLine;
            strSQL += " AND B.NEFUDA_KBN = '1' " + Environment.NewLine;
            strSQL += " AND A.BUNRUI = 910 " + Environment.NewLine;
            strSQL += "ORDER BY " + Environment.NewLine;
            strSQL += " A.HNO, " + Environment.NewLine;
            strSQL += " A.SYOHINCD" + Environment.NewLine;

            var sql = "select * from 0112_EOS_HACHU where HNO = 228585";
            string connectionString = string.Format("Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq={0}", MDB_FILE);
            DataTable dt = new DataTable();
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                    adapter.Fill(dt);

                    //datagrid.DataContext = dt;  // DataGridへの表示
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private IEnumerable<YasusakiModel> DataReaderConvertToClass(OleDbDataReader dataReader)
        {
            while (dataReader.Read())
            {
                var model = new YasusakiModel();
                var properties = model.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    var data = dataReader.GetValue(i);
                    if (!data.GetType().Equals(DBNull.Value.GetType())) properties[i].SetValue(model, data);
                    //これでもOK
                    //if (!string.IsNullOrWhiteSpace(data.ToString())) properties[i].SetValue(person, data);
                }
                yield return model;
            }
        }
    }    
}
