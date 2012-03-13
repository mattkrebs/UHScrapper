using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UhScrapper.Web.Models
{
    public class PlayerLine
    {

        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Assits { get; set; }
        public int PPR { get; set; }
    }
}