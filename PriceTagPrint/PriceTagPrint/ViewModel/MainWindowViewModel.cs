using Oracle.ManagedDataAccess.Client;
using PriceTagPrint.Common;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PriceTagPrint.Model.MainWindowModel;

namespace PriceTagPrint.ViewModel
{
    class MainWindowViewModel : ViewModelsBase
    {
        public double Number { get; set; }

        public MainWindowViewModel()
        {
            this.SelectCommand = new DelegateCommand<string>(ShowDisplay);
        }
        public ICommand SelectCommand { get; private set; }

        private void ShowDisplay(string id)
        {

            //データベース接続テスト STT
            try
            {
                using (OracleConnection conn = new OracleConnection())
                {
                    OracleConnection con = new OracleConnection();
                    string dataSource = "(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.7)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = orcl)))";

                    con.ConnectionString = "User ID=WAG_USR1; Password=P; Data Source=" + dataSource + ";";
                    con.Open();

                    string sql = "SELECT * FROM CLSMTA WHERE CLSKB = 7";
                    OracleCommand cmd = new OracleCommand(sql, con);

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0}:{1}", reader["CLSID"], reader["CLSNM"]));
                    }

                    reader.Close();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            //データベース接続テスト END



            var aaa = "";
            var torihikisaki = new TorihikisakiList().list.FirstOrDefault(x => x.Id == id);
            if(torihikisaki != null)
            {
                switch(torihikisaki.Id)
                {
                    case TorihikisakiId.YASUSAKI:
                        break;
                    case TorihikisakiId.YAMANAKA:
                        break;
                    case TorihikisakiId.MARUYOSI:
                        break;
                    case TorihikisakiId.OKINAWA_SANKI:
                        break;
                    default:break;
                }
            }               

        }
    }
    
}
