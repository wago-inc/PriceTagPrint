using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace PriceTagPrint.Common
{
    public static class RegistryUtil
    {
		/// <summary>
		/// 対象のアプリケーションがインストールされているかどうかを返します.
		/// </summary>
		/// <param name="appName"></param>
		/// <returns></returns>
		public static Boolean isInstalled(String appName)
		{

			List<UninstallApp> list = getUninstallAppList();
			var result = list.Where(x => x.DisplayName.Contains(appName)).ToList();
			if (result.Count > 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// アンインストールアプリケーションの一覧を作成します.
		/// </summary>
		/// <returns></returns>
		public static List<UninstallApp> getUninstallAppList()
		{
			String uninstallPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
			RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstallPath, false);
			List<UninstallApp> list = new List<UninstallApp>();
			if (regKey != null)
			{
				foreach (string subKey in regKey.GetSubKeyNames())
				{
					UninstallApp ap = new UninstallApp();
					RegistryKey appkey = Registry.LocalMachine.OpenSubKey(uninstallPath + "\\" + subKey, false);
					// 表示名
					String displayName = GetValue(appkey, "DisplayName");
					ap.DisplayName = displayName != "" ? displayName : subKey;
					// バージョン
					ap.DisplayVersion = GetValue(appkey, "DisplayVersion");
					// インストール日時
					ap.InstallDate = GetValue(appkey, "InstallDate");
					// 発行元
					ap.Publisher = GetValue(appkey, "Publisher");
					list.Add(ap);
				}
			}
			return list;
		}

		/// <summary>
		/// レジストリキーから指定した名前のデータを取得します.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static String GetValue(RegistryKey key, String name)
		{
			var value = key.GetValue(name);
			if (value != null)
			{
				return value.ToString();
			}
			return "";
		}
	}
}
