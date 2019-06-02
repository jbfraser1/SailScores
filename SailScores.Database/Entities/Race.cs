﻿using SailScores.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SailScores.Database.Entities
{
    public class Race
    {
        public Guid Id { get; set; }
        
        public Guid ClubId { get; set; }
        public Club Club { get; set; }
        [StringLength(200)]
        public String Name { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(30)]
        public RaceState? State { get; set; }

        // Typically the order of the race for a given date, but may not be.
        // used for display order after date. 
        public int Order { get; set; }
        [StringLength(1000)]
        public String Description { get; set; }

        [StringLength(500)]
        public String TrackingUrl { get; set; }

        public Fleet Fleet { get; set; }
        public IList<Score> Scores { get; set; }
        
        public IList<SeriesRace> SeriesRaces { get; set; }
    }
}
