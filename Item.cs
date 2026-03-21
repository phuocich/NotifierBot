namespace NotifierBot
{
    public class State
    {
        public int LastItem { get; set; } = 0;
        public string LastDate { get; set; } = string.Empty;
    }

    public class Item
    {
        public int Number { get; set; }
        public string Chapter { get; set; } = string.Empty;
        public string Pali { get; set; } = string.Empty;
        public string BhanteThichMinhChau { get; set; } = string.Empty;
        public string BhanteIndacanda { get; set; } = string.Empty;
    }
}
