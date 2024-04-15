using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum Countries
    {
        Ukraine = 1,
        Poland = 2,
        EU = 4,
        Other  = 8,
    }
}
