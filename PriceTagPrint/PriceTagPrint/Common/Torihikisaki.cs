using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{
    public enum HakkouKind
    {
        None,
        Auto,
        Input,
        Both
    }

    public static class TidNum
    {
        public const int YASUSAKI = 112;
        public const int YAMANAKA = 127;
        public const int MARUYOSI = 102;
        public const int OKINAWA_SANKI = 122;
        public const int WATASEI = 7858;
        public const int AJU = 170;
        public const int ABUABU = 8104;
        public const int ITOGOFUKU = 7705;
        public const int OKADA = 7501;
        public const int KANETA = 7500;
        public const int KYOEI = 101;
        public const int COSMOMATUOKA = 7883;
        public const int SANEI = 117;
        public const int TAIFUKUTOKYO = 7846;
        public const int TENMAYA = 109;
        public const int DOMMY = 129;
        public const int BIGA = 8108;
        public const int FUJI = 103;
        public const int FUJIYA = 0;
        public const int HOKKAIDO_SANKI = 121;
        public const int HONTENTAKAHASI = 8103;
        public const int MAXVALUE = 110;
        public const int MARBURU = 8110;
        public const int MIYAMA = 8102;
        public const int YANAGIYA = 7840;
        public const int WORKWAY = 0;
        public const int SANKI = 118;
    }

    public static class Tid
    {
        public const string YASUSAKI = "0112";
        public const string YAMANAKA = "0127";
        public const string MARUYOSI = "0102";
        public const string OKINAWA_SANKI = "0122";
        public const string WATASEI = "7858";
        public const string AJU = "0170";
        public const string ABUABU = "8104";
        public const string ITOGOFUKU = "7705";
        public const string OKADA = "7501";
        public const string KANETA = "7500";
        public const string KYOEI = "0101";
        public const string COSMOMATUOKA = "7883";
        public const string SANEI = "0117";
        public const string TAIFUKUTOKYO = "7846";
        public const string TENMAYA = "0109";
        public const string DOMMY = "0129";
        public const string BIGA = "8108";
        public const string FUJI = "0103";
        public const string FUJIYA = "";
        public const string HOKKAIDO_SANKI = "0121";
        public const string HONTENTAKAHASI = "8103";
        public const string MAXVALUE = "0110";
        public const string MARBURU = "8110";
        public const string MIYAMA = "8102";
        public const string YANAGIYA = "7840";
        public const string WORKWAY = "";
        public const string SANKI = "0118";
    }

    public static class Tnm
    {
        public const string YASUSAKI = "ヤスサキ";
        public const string YAMANAKA = "ヤマナカ";
        public const string MARUYOSI = "マルヨシ";
        public const string OKINAWA_SANKI = "沖縄三喜マルエー";
        public const string WATASEI = "わたせい";
        public const string AJU = "アージュ";
        public const string ABUABU = "アブアブ赤札堂";
        public const string ITOGOFUKU = "イトウゴフク";
        public const string OKADA = "おかだ";
        public const string KANETA = "カネタ";
        public const string KYOEI = "キョーエイ";
        public const string COSMOMATUOKA = "コスモマツオカ";
        public const string SANEI = "サンエー";
        public const string TAIFUKUTOKYO = "タイフク東京";
        public const string TENMAYA = "天満屋ストア";
        public const string DOMMY = "ドミー";
        public const string BIGA = "ビッグエー";
        public const string FUJI = "フジ";
        public const string FUJIYA = "フジヤ";
        public const string HOKKAIDO_SANKI = "北海道三喜";
        public const string HONTENTAKAHASI = "本店タカハシ";
        public const string MAXVALUE = "マックスバリュ西日本";
        public const string MARBURU = "マーブル";
        public const string MIYAMA = "ミヤマ";
        public const string YANAGIYA = "ヤスサキ";
        public const string WORKWAY = "ワークウェイ";
        public const string SANKI = "三喜";
    }
    public class Torihikisaki
    {
        public int Id { get; set; }
        public string Tcode { get; set; }
        public string Name { get; set; }
        public HakkouKind Kind { get; set; }
        public Torihikisaki(int id, string tcode, string name, HakkouKind kind)
        {
            this.Id = id;
            this.Tcode = tcode;
            this.Name = name;
            this.Kind = kind;
        }
    }

    public class TorihikisakiList
    {
        public List<Torihikisaki> list;
        public TorihikisakiList()
        {
            Create();
        }

        private void Create()
        {
            list = new List<Torihikisaki>()
            {
                new Torihikisaki(1, Tid.YASUSAKI,        Tnm.YASUSAKI,       HakkouKind.Both),
                new Torihikisaki(2, Tid.YAMANAKA,        Tnm.YAMANAKA,       HakkouKind.Auto),
                new Torihikisaki(3, Tid.MARUYOSI,        Tnm.MARUYOSI,       HakkouKind.Both),
                new Torihikisaki(4, Tid.OKINAWA_SANKI,   Tnm.OKINAWA_SANKI,  HakkouKind.Auto),
                new Torihikisaki(5, Tid.WATASEI,         Tnm.WATASEI,        HakkouKind.Both),
                new Torihikisaki(6, Tid.AJU,             Tnm.AJU,            HakkouKind.Input),
                new Torihikisaki(7, Tid.ABUABU,          Tnm.ABUABU,         HakkouKind.Input),
                new Torihikisaki(8, Tid.ITOGOFUKU,       Tnm.ITOGOFUKU,      HakkouKind.Input),
                new Torihikisaki(9, Tid.OKADA,           Tnm.OKADA,          HakkouKind.Input),
                new Torihikisaki(10, Tid.KANETA,         Tnm.KANETA,         HakkouKind.Input),
                new Torihikisaki(11, Tid.KYOEI,          Tnm.KYOEI,          HakkouKind.Input),
                new Torihikisaki(12, Tid.COSMOMATUOKA,   Tnm.COSMOMATUOKA,   HakkouKind.Input),
                new Torihikisaki(13, Tid.SANEI,          Tnm.SANEI,          HakkouKind.Input),
                new Torihikisaki(14, Tid.TAIFUKUTOKYO,   Tnm.TAIFUKUTOKYO,   HakkouKind.Input),
                new Torihikisaki(15, Tid.TENMAYA,        Tnm.TENMAYA,        HakkouKind.Input),
                new Torihikisaki(16, Tid.DOMMY,          Tnm.DOMMY,          HakkouKind.Input),
                new Torihikisaki(17, Tid.BIGA,           Tnm.BIGA,           HakkouKind.Input),
                new Torihikisaki(18, Tid.FUJI,           Tnm.FUJI,           HakkouKind.Input),
                new Torihikisaki(19, Tid.FUJIYA,         Tnm.FUJIYA,         HakkouKind.Input),
                new Torihikisaki(20, Tid.HOKKAIDO_SANKI, Tnm.HOKKAIDO_SANKI, HakkouKind.Input),
                new Torihikisaki(21, Tid.HONTENTAKAHASI, Tnm.HONTENTAKAHASI, HakkouKind.Input),
                new Torihikisaki(22, Tid.MAXVALUE,       Tnm.MAXVALUE,       HakkouKind.Input),
                new Torihikisaki(23, Tid.MARBURU,        Tnm.MARBURU,        HakkouKind.Input),
                new Torihikisaki(24, Tid.MIYAMA,         Tnm.MIYAMA,         HakkouKind.Input),
                new Torihikisaki(25, Tid.WORKWAY,        Tnm.WORKWAY,        HakkouKind.Input),
                new Torihikisaki(26, Tid.SANKI,          Tnm.SANKI,          HakkouKind.None),
            };
        }
    }
}
