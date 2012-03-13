using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UhScrapper.Web.Models
{
    public class BoxScoreModel
    {
        public int ScheduleId { get; set; }
        public string HomeTeamName { get; set; }
        public string TeamId { get; set; }
        public List<PlayerLine> HomePlayers { get; set; }
        public List<PlayerLine> AwayPlayers { get; set; }
      
    }
}