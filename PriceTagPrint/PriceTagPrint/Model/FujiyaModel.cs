using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class FujiyaData
    {
        /// <summary>
        /// 出荷数
        /// </summary>
        public int TSU { get; set; }
        /// <summary>
        /// 識別番号　※ヘッダ入力値
        /// </summary>
        public string SHIKIBETSU { get; set; }
        /// <summary>
        /// 取引先コード　※886固定
        /// </summary>
        public string TORIHIKICD { get; set; }
        /// <summary>
        /// 部門コード　※JYUCYUのNETUKE_BUNRUIの"-"splitのindex0
        /// </summary>
        public string BUMONCD { get; set; }
        /// <summary>
        /// 中分類　※JYUCYUのNETUKE_BUNRUIの"-"splitのindex1
        /// </summary>
        public string CHUBUNRUI { get; set; }
        /// <summary>
        /// 仕入週　※FujiyaSyuuSuListで定義している
        /// </summary>
        public string SIRESYU { get; set; }
        /// <summary>
        /// 相手先品番
        /// </summary>
        public string AITE_HINBAN { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// 品番
        /// </summary>
        public string HINBAN { get; set; }
        /// <summary>
        /// 枝番
        /// </summary>
        public string EDABAN { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string HINNMA { get; set; }
        /// <summary>
        /// 原単価
        /// </summary>
        public int STANKA { get; set; }
        /// <summary>
        /// 本体価格
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// 税込売価
        /// </summary>
        public int ZBAIKA { get; set; }
        /// <summary>
        /// 備考
        /// </summary>
        public string BIKOU { get; set; }
    }
}
