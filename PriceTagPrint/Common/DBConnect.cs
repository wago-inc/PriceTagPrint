using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{
    public static class DBConnect
    {
        //public const string OrclConnectString = "User ID=WAG_USR1; Password=P; Data Source=ORCL.world;";
        public const string OrclConnectString = "User ID=WAG_USR1; Password=P; Data Source=";

        public const string OrclDataSource = "(DESCRIPTION =" +
                                                "(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.7)(PORT = 1521))" +
                                                "(CONNECT_DATA =" +
                                                  "(SERVER = DEDICATED)" +
                                                  "(SERVICE_NAME = orcl)" +
                                                ")" +
                                              ");";

        public const string TestMdbDataSource = @"\\Server00\h\database\発注受付処理\一般得意先\一般得意先発注受付_cl99.mdb";
        public static string TestMdbConnectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + TestMdbDataSource;

        public const string MdbDataSource = @"\\Server00\h\database\得意先商品台帳\得意先商品台帳.mdb";
        public static string MdbConnectionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + MdbDataSource;

        public const string MdbDataSource_ito = @"\\Server00\H\database\発注受付処理\イトウゴフク\Ac_SovI\ﾏｽﾀｰ.MDB";
        public static string MdbConnectionString_ito = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + MdbDataSource_ito;

        public const string MdbDataSource_maneki = @"\\Server00\H\database\発注受付処理\マネキ\マネキ発注受付.mdb";
        public static string MdbConnectionString_maneki = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + MdbDataSource_maneki;

    }
}
