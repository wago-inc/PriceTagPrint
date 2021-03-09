using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PriceTagPrint.ViewModel
{
    public class HakkouType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BunruiCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HachuNumber
    {
        public string Name { get; set; }
    }

    public class NefudaBangou
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class YasusakiViewModel
    {
        public ObservableCollection<HakkouType> HakkouTypeItems { get; set; }
        public ObservableCollection<BunruiCode> BunruiCodeItems { get; set; }
        public ObservableCollection<HachuNumber> HachuNumberItems { get; set; }
        public ObservableCollection<NefudaBangou> NefudaBangouItems { get; set; }

        public YasusakiViewModel()
        {

        }
    }
}
