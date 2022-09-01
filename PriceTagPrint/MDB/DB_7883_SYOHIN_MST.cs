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
    public class DB_7883_SYOHIN_MST
    {
        public string 会社コード { get; set; }
        public string JANコード { get; set; }
        public string SUBCLASSNo { get; set; }
        public long 税抜売価 { get; set; }
        public long 原価 { get; set; }
        public long 消費税率 { get; set; }
        public string 開始日 { get; set; }
        public string 終了日 { get; set; }
        public string 発注終了日 { get; set; }
        public string ベンダーコード { get; set; }
        public string 品名 { get; set; }
        public string 商品コード { get; set; }
        public string 賞味期限コード { get; set; }
        public long 発注単位 { get; set; }
        public string 棚番号 { get; set; }

        public DB_7883_SYOHIN_MST(string 会社コード, string JANコード, string SUBCLASSNo, long 税抜売価, long 原価, long 消費税率, string 開始日, string 終了日,
                                  string 発注終了日, string ベンダーコード, string 品名, string 商品コード, string 賞味期限コード, long 発注単位, string 棚番合)
        {
            this.会社コード = 会社コード;
            this.JANコード = JANコード;
            this.SUBCLASSNo = SUBCLASSNo;
            this.税抜売価 = 税抜売価;
            this.原価 = 原価;
            this.消費税率 = 消費税率;
            this.開始日 = 開始日;
            this.終了日 = 終了日;
            this.発注終了日 = 発注終了日;
            this.ベンダーコード = ベンダーコード;
            this.品名 = 品名;
            this.商品コード = 商品コード;
            this.賞味期限コード = 賞味期限コード;
            this.発注単位 = 発注単位;
            this.棚番号 = 棚番合;
        }
    }

    public class DB_7883_SYOHIN_MST_LIST
    {
        public List<DB_7883_SYOHIN_MST> QueryWhereJancd(string sttJancd, string endJancd)
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT 7883_SYOHIN_MST.*";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " 7883_SYOHIN_MST ";
            strSQL = strSQL + Environment.NewLine + "WHERE ";
            strSQL = strSQL + Environment.NewLine + "( ";
            strSQL = strSQL + Environment.NewLine + "  ((7883_SYOHIN_MST.[JANコード]) >= '" + sttJancd + "') AND";
            strSQL = strSQL + Environment.NewLine + "  ((7883_SYOHIN_MST.[JANコード]) <= '" + endJancd + "')";
            strSQL = strSQL + Environment.NewLine + "); ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_7883_SYOHIN_MST>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);

                OdbcCommand sqlCommand = new OdbcCommand(strSQL, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_7883_SYOHIN_MST
                        (
                            dr.Field<string>("会社コード"),
                            dr.Field<string>("JANコード"),
                            dr.Field<string>("SUBCLASSNo"),
                            dr.Field<long>("税抜売価"),
                            dr.Field<long>("原価"),
                            dr.Field<long>("消費税率"),
                            dr.Field<string>("開始日"),
                            dr.Field<string>("終了日"),
                            dr.Field<string>("発注終了日"),
                            dr.Field<string>("ベンダーコード"),
                            dr.Field<string>("品名"),
                            dr.Field<string>("商品コード"),
                            dr.Field<string>("賞味期限コード"),
                            dr.Field<long>("発注単位"),
                            dr.Field<string>("棚番合")
                        ));
                }
                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public List<DB_7883_SYOHIN_MST> QueryWhereAll()
        {
            var strSQL = "";
            strSQL = strSQL + Environment.NewLine + "SELECT 7883_SYOHIN_MST.*";
            strSQL = strSQL + Environment.NewLine + "FROM ";
            strSQL = strSQL + Environment.NewLine + " 7883_SYOHIN_MST; ";

            DataTable mdbDt = new DataTable();
            var results = new List<DB_7883_SYOHIN_MST>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);

                OdbcCommand sqlCommand = new OdbcCommand(strSQL, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_7883_SYOHIN_MST
                        (
                            dr.Field<string>("会社コード"),
                            dr.Field<string>("JANコード"),
                            dr.Field<string>("SUBCLASSNo"),
                            dr.Field<int>("税抜売価"),
                            dr.Field<int>("原価"),
                            dr.Field<int>("消費税率"),
                            dr.Field<string>("開始日"),
                            dr.Field<string>("終了日"),
                            dr.Field<string>("発注終了日"),
                            dr.Field<string>("ベンダーコード"),
                            dr.Field<string>("品名"),
                            dr.Field<string>("商品コード"),
                            dr.Field<string>("賞味期限コード"),
                            dr.Field<int>("発注単位"),
                            dr.Field<string>("棚番号")
                        ));
                }
                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
