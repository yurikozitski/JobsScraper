using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum JobTypes
    {
        OnSite = 1,
        Hybrid = 2,
        Remote = 4,
    }
}
