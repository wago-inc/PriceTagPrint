using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class MaruyoshiData
    {
        public string RPTCLTID { get; set; }
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
        /// 納品日付
        /// </summary>
        public string VNOHINDT { get; set; }
        /// <summary>
        /// 分類コード（EOSJUTRA.VBUNCD）
        /// </summary>
        public string VBUNCD { get; set; }
        /// <summary>
        /// 伝票管理NO.
        /// </summary>
        public string DATNO { get; set; }
        /// <summary>
        /// 行№
        /// </summary>
        public string VROWNO { get; set; }
        /// <summary>
        /// クラスコード（EOSJUTRA.VHINCDの先頭４桁）
        /// </summary>
        public string NEFCMA { get; set; }
        /// <summary>
        /// 当社品番（EOSJUTRA.VCYOBI7）
        /// </summary>
        public string NEFCMB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NEFCMB2 { get; set; }
        /// <summary>
        /// 品名（EOSJUTRA.VHINNMA）
        /// </summary>
        public string NEFCMC { get; set; }
        /// <summary>
        /// カラーコード + カラー名（EOSJUTRA.VCOLNM）
        /// </summary>
        public string NEFCMD { get; set; }
        /// <summary>
        /// カラーコード（EOSJUTRA.VCOLNMの下２桁）
        /// </summary>
        public string NEFCMD2 { get; set; }
        /// <summary>
        /// ｻｲｽﾞｺｰﾄﾞ（EOSJUTRA.VSIZNMの下２桁）
        /// </summary>
        public string NEFCME { get; set; }
        /// <summary>
        /// サイズ名（EOSJUTRA.VSIZNMの先頭５桁）
        /// </summary>
        public string NEFCMF { get; set; }
        /// <summary>
        /// 単品コード（EOSJUTRA.VHINCDの６桁目から４桁取得）
        /// </summary>
        public string NEFCMG { get; set; }
        /// <summary>
        /// 組（EOSJUTRA.VHINCDの下２桁）
        /// </summary>
        public string NEFCMH { get; set; }
        /// <summary>
        /// FLG（8 or 2）（ＤＢ登録なし 値札発行には特に必要ない項目）
        /// </summary>
        public string NEFCMI { get; set; }
        /// <summary>
        /// 追加（T or Y or X）（ＤＢ登録なし 値札発行には特に必要ない項目）
        /// </summary>
        public string NEFCMJ { get; set; }
        /// <summary>
        /// タグ（ＤＢ登録なし ※値札発行に必要な項目）
        /// </summary>
        public string NEFCMK { get; set; }
        /// <summary>
        /// 消し売価（ＤＢ登録なし 値札発行には特に必要ない項目）
        /// </summary>
        public decimal NEFTKA { get; set; }
        /// <summary>
        /// 売価（EOSJUTRA.VURITK）
        /// </summary>
        public decimal NEFTKB { get; set; }
        /// <summary>
        /// 売価と同じものがセットされている※CSVにはこちらを渡す（EOSJUTRA.VURITK）
        /// </summary>
        public decimal NEFTKB2 { get; set; }
        /// <summary>
        /// 発行枚数（EOSJUTRA.VSURYO）
        /// </summary>
        public decimal NEFSUA { get; set; }
        /// <summary>
        /// シーズンコード（EOSJUTRA.VHINNMAの下２桁）
        /// </summary>
        public string NEFSEZ { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string VHINCD { get; set; }
        /// <summary>
        /// 商品コード
        /// </summary>
        public string HINCD { get; set; }
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCD { get; set; }
        public string WRTTM { get; set; }
        public string WRTDT { get; set; }
    }
}
