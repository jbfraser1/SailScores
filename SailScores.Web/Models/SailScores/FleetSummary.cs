﻿using SailScores.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SailScores.Web.Models.SailScores
{
    public class FleetSummary
    {

        public Guid Id { get; set; }
        public String ShortName { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public bool? IsActive { get; set; }
        public IList<Series> Series { get; set; }
        // todo public IList<Season> Seasons { get; set; }
        //todo: competitor lists, hopefully with seasons active.
    }
}
