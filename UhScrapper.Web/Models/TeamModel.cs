using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UhScrapper.Web.Models
{
    public class TeamModel
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string TeamPhotoUrl { get; set; }
        public string Record { get; set; }
        public string LogoUrl { get; set; }

    }
}