using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{
    public class ZeiItem
    {
        public decimal Tax { get; set; }
        public decimal Kakeritsu { get; set; }
        public DateTime SttDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ZeiItem(decimal tax, decimal kakeritsu, DateTime sttDate, DateTime endDate)
        {
            this.Tax = tax;
            this.Kakeritsu = kakeritsu;
            this.SttDate = sttDate;
            this.EndDate = endDate;
        }
    }
    public static class Zeiritsu
    {
        public static List<ZeiItem> items = new List<ZeiItem>()
        {
            new ZeiItem(10m, 1.1m, DateTime.Parse("2019/10/1"), DateTime.MaxValue)
        };
    }
}
