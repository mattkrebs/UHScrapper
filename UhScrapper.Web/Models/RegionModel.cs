using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UhScrapper.Web.Models
{
    public class RegionModel
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public List<LeagueModel> Divisions { get; set; }

    }
}