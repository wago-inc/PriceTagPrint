using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Model
{
    public class YasusakiData
    {
        public int HNO { get; set; }
        public string TOKCD { get; set; }
        public string SYOHINCD { get; set; }
        public string JANCD { get; set; }
        public int BUNRUI { get; set; }
        public string SCODE { get; set; }
        public int SAIZUS { get; set; }
        public string HINCD { get; set; }
        public DateTime HATYUBI { get; set; }
        public DateTime NOUHINBI { get; set; }
        public int NSU { get; set; }
        public int BAIKA { get; set; }
        public string EOS_SYOHINNM { get; set; }
        public int GENKA { get; set; }
        public string SKBN { get; set; }
        public string NEFUDA_KBN { get; set; }
        public string NETUKE_BUNRUI { get; set; }
        public string BIKOU1 { get; set; }
        public string BIKOU2 { get; set; }
    }

    public class YasusakiModel
    {
        public int 発行枚数 { get; set; }
        public string 売切月 { get; set; }
        public string 品番 { get; set; }
        public string JAN { get; set; }
        public string 商品コード { get; set; }
        public string 商品名 { get; set; }
        public int 売価 { get; set; }
        public int 単価 { get; set; }
        public string 値札番号 { get; set; }
        public YasusakiModel(int 発行枚数, string 売切月, string 品番, string JAN, string 商品コード, 
                             string 商品名, int 売価, int 単価, string 値札番号)
        {
            this.発行枚数 = 発行枚数;
            this.売切月 = 売切月;
            this.品番 = 品番;
            this.JAN = JAN;
            this.商品コード = 商品コード;
            this.商品名 = 商品名;
            this.売価 = 売価;
            this.単価 = 単価;
            this.値札番号 = 値札番号;
        }
    }

    public class YasusakiModelList
    {
        public IEnumerable<YasusakiModel> ConvertYasusakiDataToModel(List<YasusakiData> datas)
        {
            var result = new List<YasusakiModel>();
            datas.ForEach(data =>
            {
                result.Add(
                    new YasusakiModel(data.NSU, "41", data.SCODE, data.JANCD, data.SYOHINCD, 
                            data.EOS_SYOHINNM, data.BAIKA, data.GENKA, data.NEFUDA_KBN));
            });
            return result;
        }
    }
}
