using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Enums
{
    [Flags]
    public enum Grades
    {
        TraineeIntern = 1,
        Junior = 2,
        Middle = 4,
        Senior = 8,
        TeamLead = 16,
        HeadChief = 32,
    }
}
