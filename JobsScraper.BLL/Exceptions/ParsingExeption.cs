using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Exceptions
{
    public class ParsingExeption : Exception
    {
        public string JobBoard { get; }

        public string XPath { get; }

        public ParsingExeption(string message, string jobBoard, string xPath) : base(message)
        {
            this.JobBoard = jobBoard;
            this.XPath = xPath;
        }
    }
}
