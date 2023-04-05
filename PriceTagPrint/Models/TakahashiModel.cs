using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Models
{
    public class TakahashiData
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public int TCODE { get; set; }
        /// <summary>
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        public string BUMON { get; set; }
        /// <summary>
        /// 相手先JANコード
        /// </summary>
        public string JANCD { get; set; }
        /// <summary>
        /// 分類コード
        /// </summary>
        public int BUNRUI { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string SCODE { get; set; }
        /// <summary>
        /// 社内サイズ・カラーコード
        /// </summary>
        public string SAIZUS { get; set; }
        /// <summary>
        /// 社内商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string HINMEI { get; set; }
        /// <summary>
        /// サイズ名
        /// </summary>
        public string SAIZUN { get; set; }
        /// <summary>
        /// 仕入原価
        /// </summary>
        public int STANKA { get; set; }
        /// <summary>
        /// 販売単価
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// ロケーション倉庫コード
        /// </summary>
        public int LOCTANA_SOKO_CODE { get; set; }
        /// <summary>
        /// ロケーションフロアNo
        /// </summary>
        public int LOCTANA_FLOOR_NO { get; set; }
        /// <summary>
        /// ロケーション棚No
        /// </summary>
        public int LOCTANA_TANA_NO { get; set; }
        /// <summary>
        /// ロケーションケースNo
        /// </summary>
        public int LOCTANA_CASE_NO { get; set; }
    }
}
