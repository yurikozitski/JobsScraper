using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Interfaces.Djinni
{
    public interface IDjinniRequestStringBuilder : IRequestStringBuilder
    {
        string RequestString { get; }
    }
}
