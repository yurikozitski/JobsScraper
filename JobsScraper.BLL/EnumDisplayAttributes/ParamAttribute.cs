using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.EnumDisplayAttributes
{
    public abstract class ParamAttribute : Attribute
    {
        protected ParamAttribute(string paramText)
        {
            this.Text = paramText;
        }

        public string Text { get; set; }
    }
}
