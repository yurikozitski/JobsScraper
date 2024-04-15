﻿using JobsScraper.BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Models
{
    public class Vacancy
    {
        public string WebSite { get; set; } = default!;

        public string Link { get; set; } = default!;

        public string JobTitle { get; set; } = default!;

        public string Company { get; set; } = default!;

        public string? Salary { get; set; }

        public string? JobType { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public DateOnly? PublicationDate { get; set; }
    }
}
