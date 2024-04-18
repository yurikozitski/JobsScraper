namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum ExperienceLevels
    {
        NoExperience = 1,
        BelowOneYear = 2,
        OneYear = 4,
        TwoYears = 8,
        ThreeYears = 16,
        FourYear = 32,
        FiveYearsAndAbove = 64,
    }
}
