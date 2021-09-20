using System.Collections.Generic;
public class OptionFormTypeConfig
{
    public class Item
    {
        public string icon { get; set; }
        public string text { get; set; }
        public string key { get; set; }
        public string tabs { get; set; }
        public string fields { get; set; }
        public string svg { get; set; }
        public object svgTrusted { get; set; }
        public string css { get; set; }
        public List<Item> items { get; set; }
    }

    public class Items
    {
        public List<Item> items { get; set; }
    }

    public class Root
    {
        public string hideFields { get; set; }
        public Items items { get; set; }
    }
}