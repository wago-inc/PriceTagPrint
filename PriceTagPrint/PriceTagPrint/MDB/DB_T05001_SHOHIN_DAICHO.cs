using PriceTagPrint.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_T05001_SHOHIN_DAICHO
    {
        public string バーコード { get; set; }
        public string 品番 { get; set; }
        public string 仕入週 { get; set; }
        public string 商品名 { get; set; }
        public byte? 値札No { get; set; }
        public short? 商品区分コード { get; set; }
        public short? 商品摘要コード1 { get; set; }
        public short? 商品摘要コード2 { get; set; }
        public short? 商品摘要コード3 { get; set; }
        public short? 分類2コード { get; set; }
        public short? 仕入先コード { get; set; }
        public string 画像名1 { get; set; }
        public DB_T05001_SHOHIN_DAICHO(string バーコード, string 品番, string 仕入週, string 商品名, byte? 値札No, short? 商品区分コード, short? 商品摘要コード1,
                                       short? 商品摘要コード2, short? 商品摘要コード3, short? 分類2コード, short? 仕入先コード, string 画像名1)
        {
            this.バーコード = バーコード;
            this.品番 = 品番;
            this.仕入週 = 仕入週;
            this.商品名 = 商品名;
            this.値札No = 値札No;
            this.商品区分コード = 商品区分コード;
            this.商品摘要コード1 = 商品摘要コード1;
            this.商品摘要コード2 = 商品摘要コード2;
            this.商品摘要コード3 = 商品摘要コード3;
            this.分類2コード = 分類2コード;
            this.仕入先コード = 仕入先コード;
            this.画像名1 = 画像名1;
        }
    }

    public class DB_T05001_SHOHIN_DAICHO_LIST
    {
        public List<DB_T05001_SHOHIN_DAICHO> QueryWhereJancd(string sttJancd, string endJancd)
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT T05001_商品台帳.*";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " T05001_商品台帳 ";
            strSQL = strSQL + Environment.NewLine + "WHERE ";
            strSQL = strSQL + Environment.NewLine + "( ";
            strSQL = strSQL + Environment.NewLine + "  ((T05001_商品台帳.[バーコード]) >= '" + sttJancd + "') AND";
            strSQL = strSQL + Environment.NewLine + "  ((T05001_商品台帳.[バーコード]) <= '" + endJancd + "')";
            strSQL = strSQL + Environment.NewLine + "); ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_T05001_SHOHIN_DAICHO>();
            // 読み込み
            try
            {
                using (OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString_ito))
                {
                    mdbConn.Open();

                    OdbcDataAdapter adapter = new OdbcDataAdapter(strSQL, mdbConn);
                    adapter.Fill(mdbDt);
                    foreach (DataRow dr in mdbDt.Rows)
                    {
                        results.Add(new DB_T05001_SHOHIN_DAICHO
                            (
                                dr.Field<string>("バーコード"),
                                dr.Field<string>("品番"),
                                dr.Field<string>("仕入週"),
                                dr.Field<string>("商品名"),
                                dr.Field<byte?>("値札No"),
                                dr.Field<short?>("商品区分コード"),
                                dr.Field<short?>("商品摘要コード1"),
                                dr.Field<short?>("商品摘要コード2"),
                                dr.Field<short?>("商品摘要コード3"),
                                dr.Field<short?>("分類2コード"),
                                dr.Field<short?>("仕入先コード"),
                                dr.Field<string>("画像名1")
                            ));
                    }
                    return results;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
