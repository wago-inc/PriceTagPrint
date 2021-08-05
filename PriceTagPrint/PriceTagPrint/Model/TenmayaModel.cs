using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class TenmayaData
    {
        /// <summary>
        /// 量販店コード
        /// </summary>
        public string VRYOHNCD { get; set; }
        /// <summary>
        /// 量販店名
        /// </summary>
        public string VRYOHNNM { get; set; }
        /// <summary>
        /// 受信日付
        /// </summary>
        public string VRCVDT { get; set; }        
        /// <summary>
        /// 分類コード
        /// </summary>
        public string VBUNCD { get; set; }
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string QOLTORID { get; set; }
        /// <summary>
        /// 納品日付
        /// </summary>
        public string VNOHINDT { get; set; }        
        /// <summary>
        /// JANコード
        /// </summary>
        public string VHINCD { get; set; }
        /// <summary>
        /// カラーコード
        /// </summary>
        public string VCOLCD { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string VSIZCD { get; set; }
        /// <summary>
        /// カラーコード
        /// </summary>
        public string VCOLNM { get; set; }
        /// <summary>
        /// サイズコード
        /// </summary>
        public string VSIZNM { get; set; }
        /// <summary>
        /// 発行枚数
        /// </summary>
        public decimal VSURYO { get; set; }
        /// <summary>
        /// 売価
        /// </summary>
        public decimal VURITK { get; set; }
        /// <summary>
        /// 表示用商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// フリー入力（品番 + 枝番）
        /// </summary>
        public string VCYOBI7 { get; set; }
        /// <summary>
        /// 表示用商品コード
        /// </summary>
        public string HINNM { get; set; }
        /// <summary>
        /// 棚番
        /// </summary>
        public string TNANM { get; set; }
        /// <summary>
        /// 品番のみ
        /// </summary>
        public string HINBAN { get; set; }
    }
}
