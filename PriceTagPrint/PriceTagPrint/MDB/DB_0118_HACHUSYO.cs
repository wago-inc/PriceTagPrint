﻿using PriceTagPrint.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Windows;

namespace PriceTagPrint.MDB
{
    public class DB_0118_HACHUSYO
    {
        /// <summary>
        /// 発注番号
        /// </summary>
        public int HNO { get; set; }
        /// <summary>
        /// 買付書コード
        /// </summary>
        public string KAITUKECD { get; set; }
        /// <summary>
        /// 得意先コード
        /// </summary>
        public string TOKCD { get; set; }
        /// <summary>
        /// 店舗コード
        /// </summary>
        public short TENCD { get; set; }
        /// <summary>
        /// 相手商品コード
        /// </summary>
        public string SYOHINCD { get; set; }
        /// <summary>
        /// 入力順序
        /// </summary>
        public int SEQNO { get; set; }    
        /// <summary>
        /// センターコード
        /// </summary>
        public string CENTCD { get; set; }
        /// <summary>
        /// 品番コード（※伝票分類）
        /// </summary>
        public string HINBANCD { get; set; }
        /// <summary>
        /// 中分類コード
        /// </summary>
        public string CYUBUNCD { get; set; }
        /// <summary>
        /// 売単価
        /// </summary>
        public int BAIKA { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 納品予定数
        /// </summary>
        public short NSU { get; set; }
        /// <summary>
        /// 社内サイズ・カラーコード
        /// </summary>
        public int SAIZUS { get; set; }
        /// <summary>
        /// 分類コード
        /// </summary>
        public int BUNRUI { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string SCODE { get; set; }

        public DB_0118_HACHUSYO(int hno, string kaitukecd, string tokcd, short tencd, string syohincd, int seqno,
                                string centcd, string hinbancd, string cyubuncd, int baika,
                                 string hincd, short nsu, int saizus, int bunrui, string scode)
        {
            this.HNO = hno;
            this.KAITUKECD = kaitukecd;
            this.TOKCD = tokcd;
            this.TENCD = tencd;
            this.SYOHINCD = syohincd;
            this.SEQNO = seqno;
            this.CENTCD = centcd;
            this.HINBANCD = hinbancd;
            this.CYUBUNCD = cyubuncd;
            this.BAIKA = baika;
            this.HINCD = hincd;
            this.NSU = nsu;
            this.SAIZUS = saizus;
            this.BUNRUI = bunrui;
            this.SCODE = scode;
        }
    }

    public class DB_0118_HACHUSYO_LIST
    {
        public List<DB_0118_HACHUSYO> QueryWhereHno(string hno)
        {
            var sql = "SELECT * " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " 0118_HACHUSYO " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0118_HACHUSYO>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);

                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                foreach (DataRow dr in mdbDt.Rows)
                {
                    results.Add(new DB_0118_HACHUSYO
                        (
                            dr.Field<int>("HNO"),
                            dr.Field<string>("KAITUKECD"),
                            dr.Field<string>("TOKCD"),
                            dr.Field<short>("TENCD"),
                            dr.Field<string>("SYOHINCD"),
                            dr.Field<int>("SEQNO"),
                            dr.Field<string>("CENTCD"),
                            dr.Field<string>("HINBANCD"),
                            dr.Field<string>("CYUBUNCD"),
                            dr.Field<int>("BAIKA"),
                            dr.Field<string>("HINCD"),
                            dr.Field<short>("NSU"),
                            dr.Field<int>("SAIZUS"),
                            dr.Field<int>("BUNRUI"),
                            dr.Field<string>("SCODE")
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

        public bool QueryWhereHnoExists(string hno)
        {
            var sql = "SELECT HNO " + Environment.NewLine;
            sql += "FROM " + Environment.NewLine;
            sql += " 0118_HACHUSYO " + Environment.NewLine;
            sql += "WHERE " + Environment.NewLine;
            sql += " HNO = " + hno;

            DataTable mdbDt = new DataTable();
            var results = new List<DB_0118_HACHUSYO>();
            // 読み込み
            try
            {
                OdbcConnection mdbConn = new OdbcConnection(DBConnect.MdbConnectionString);
                OdbcCommand sqlCommand = new OdbcCommand(sql, mdbConn);
                sqlCommand.CommandTimeout = 30;

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(mdbDt);
                adapter.Dispose();
                sqlCommand.Dispose();

                return mdbDt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
