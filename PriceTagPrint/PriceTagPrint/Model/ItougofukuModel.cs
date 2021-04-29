using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class ItougofukuData
    {
        /// <summary>
        /// 得意先コード（JYUCYU.TCODE）
        /// </summary>
        public int TCODE { get; set; }
        /// <summary>
        /// 値札No（TO5001_商品台帳.値札No）
        /// </summary>
        public byte NEFUDANO { get; set; }
        /// <summary>
        /// 仕入週（TO5001_商品台帳.仕入週）
        /// </summary>
        public string SIRESYU { get; set; }
        /// <summary>
        /// 商品摘要コード３（TO5001_商品台帳.商品摘要コード3）
        /// </summary>
        public short TEKIYOCD3 { get; set; }
        /// <summary>
        /// 商品摘要名（TO5007_商品摘要台帳.商品摘要名）
        /// </summary>
        public string SHOHINTEKYONM { get; set; }
        /// <summary>
        /// 品番（T05001_商品台帳.品番）
        /// </summary>
        public string HINBAN { get; set; }
        /// <summary>
        /// JANコード（JYUCYU.JANCD）
        /// </summary>
        public string JANCD { get; set; }
        /// <summary>
        /// 売単価（JYUCYU.HTANKA）
        /// </summary>
        public int HTANKA { get; set; }
        /// <summary>
        /// 仕入原単価（JYUCYU.STANKA）
        /// </summary>
        public int STANKA { get; set; }
        /// <summary>
        /// 税込売価（HTANKA * 1.1 切り捨てで算出）
        /// </summary>
        public double ZTANKA { get; set; }
        /// <summary>
        /// 上代（JYUCYU.JYODAI）
        /// </summary>
        public int JYODAI { get; set; }
        /// <summary>
        /// サイズコード（TO5001_商品台帳.商品摘要コード1）
        /// </summary>
        public short TEKIYOCD1 { get; set; }
        /// <summary>
        /// サイズ名（TO5001_商品台帳.商品摘要コード1）
        /// </summary>
        public string TEKIYONM1 { get; set; }
        /// <summary>
        /// カラーコード（TO5001_商品台帳.商品摘要コード2）
        /// </summary>
        public short TEKIYOCD2 { get; set; }
        /// <summary>
        /// カラー名（TO5001_商品台帳.商品摘要コード2）
        /// </summary>
        public string TEKIYONM2 { get; set; }
        /// <summary>
        /// クラスコード（T05001_商品台帳.分類2コード）
        /// </summary>
        public short BUNRUI2CD { get; set; }
        /// <summary>
        /// 商品名（T05001_商品台帳.商品名）
        /// </summary>
        public string HINNM { get; set; }
        /// <summary>
        /// 発注番号（JYUCYU.HNO）
        /// </summary>
        public int HNO { get; set; }
        // 以下JYUCYUの同名フィールド
        public int LOCTANA_SOKO_CODE { get; set; }
        public int LOCTANA_FLOOR_NO { get; set; }
        public int LOCTANA_TANA_NO { get; set; }
        public int LOCTANA_CASE_NO { get; set; }
        public int BUNRUI { get; set; }
        public string SCODE { get; set; }
        public int SAIZUS { get; set; }
        /// <summary>
        /// ユニットコード（T05001_商品台帳.画像名1）
        /// </summary>
        public string UNITCD { get; set; }
        /// <summary>
        /// 仕入先コード（TO5001_商品台帳.仕入先コード）
        /// </summary>
        public string SIRECD { get; set; }
        /// <summary>
        /// 数量（JYUCYU.TSU）
        /// </summary>
        public int TSU { get; set; }


        /// <summary>
        /// 発注日（JYUCYU.HDATE）
        /// </summary>
        public string HDATE { get; set; }
        /// <summary>
        /// 納品日（JYUCYU.NDATE）
        /// </summary>
        public string NDATE { get; set; }
    }
}
