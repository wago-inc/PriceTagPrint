using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class YasusakiModel
    {
        public int 発行枚数 { get; set; }
        public string 売切月 { get; set; }
        public string 品番 { get; set; }
        public short JAN { get; set; }
        public string 商品コード { get; set; }
        public string 商品名 { get; set; }
        public int 売価 { get; set; }
        public int 単価 { get; set; }
        public string 値札番号 { get; set; }
    }
}
