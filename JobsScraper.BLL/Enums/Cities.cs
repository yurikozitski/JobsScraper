namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum Cities
    {
        Kyiv = 1,
        Vinnytsia = 2,
        Dnipro = 4,
        IvanoFrankivsk = 8,
        Zhytomyr = 16,
        Zaporizhzhia = 32,
        Lviv = 64,
        Mykolaiv = 128,
        Odesa = 256,
        Ternopil = 512,
        Kharkiv = 1024,
        Khmelnytskyi = 2048,
        Cherkasy = 4096,
        Chernihiv = 8192,
        Chernivtsi = 16384,
        Uzhhorod = 32768,
    }
}
