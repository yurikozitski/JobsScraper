using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum EnglishLevels
    {
        NoEnglish = 1,
        Elementary = 2,
        PreIntermediate = 4,
        Intermediate = 8,
        UpperIntermediate = 16,
        Fluent = 32,
    }
}
