﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Interfaces.DOU
{
    public interface IDouRequestStringBuilder : IRequestStringBuilder
    {
        string RequestString { get; }
    }
}
