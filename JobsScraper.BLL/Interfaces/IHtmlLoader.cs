using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Interfaces
{
    public interface IHtmlLoader
    {
        Task<string> LoadJobBoardHTML(JobSearchModel jobSearchModel);
    }
}
