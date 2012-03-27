using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UhScrapper.Web.Models
{
    public class ScheduleModel
    {
        public int ScheduleId { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int AwayTeamId { get; set; }
        public string Location { get; set; }
        public DateTime GameTime { get; set; }
        public string HomeScore { get; set; }
        public string AwayScore { get; set; }
        public string Week { get; set; }

    }
}