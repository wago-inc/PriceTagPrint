using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTagPrint.Common
{    
    public class BunruiCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BunruiCode(int id, string name)
        {
            this.Id = id;
            this.Name = Name;
        }
    }

    public class BunruiCodeList
    {
        public List<BunruiCode> list;
        public BunruiCodeList()
        {
            Create();
        }

        private void Create()
        {
            list = new List<BunruiCode>()
            {
                new BunruiCode(1, "ヤスサキ"),
                new BunruiCode(2, "ヤマナカ"),
                new BunruiCode(3, "マルヨシ"),
                new BunruiCode(4, "沖縄三喜マルエー"),
            };
        }
    }
}
