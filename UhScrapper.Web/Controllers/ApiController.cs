
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using UhScrapper.Web.Models;
using System.Text.RegularExpressions;
namespace UhScrapper.Web.Controllers
{
    public class ApiController : Controller
    {
        //
        // GET: /Team/
        string BaseRegionUrl = "http://www.uhlife.com/basketball/home.aspx";
        string BaseTeamUrl = "http://www.uhlife.com/basketball/team_list.aspx?RegionID={0}&SeasonDivisionID=428";


        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetRegions()
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(BaseRegionUrl);
            var regiontags = document.DocumentNode.SelectNodes("//ul[@class='subnav']//a");
            List<RegionModel> regions = new List<RegionModel>();
            foreach (var region in regiontags)
            {
                string regionurl = region.GetAttributeValue("href", "");
                int regionId = Convert.ToInt32(regionurl.Replace("home.aspx?RegionID=", ""));
                regions.Add(new RegionModel()
                {
                    RegionId = regionId,
                    Name = region.InnerHtml,
                    Divisions = getLeaguesByRegionId(regionId)
                });
            }
            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        public List<LeagueModel> getLeaguesByRegionId(int id)
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(BaseRegionUrl + "?RegionID=" + id);
            var leaguetags = document.DocumentNode.SelectNodes("//ul[@class='subnav_league']//a");
             List<LeagueModel> leagues = new List<LeagueModel>();
             if (leaguetags != null)
             {
                 foreach (var league in leaguetags)
                 {
                     string leagueUrl = league.GetAttributeValue("href", " ");
                     int leagueId = Convert.ToInt32(leagueUrl.Replace("home.aspx?RegionID=" + id + "&amp;SeasonDivisionID=", ""));
                    
                     HtmlNodeCollection names = league.SelectNodes("div");

                     leagues.Add(new LeagueModel()
                     {
                         LeagueId = leagueId,
                         Name = String.Format("{0} - {1} {2}", names[0].InnerHtml.Replace("&nbsp;", " "), names[1].InnerHtml.Replace("&nbsp;", " "), StripHTML(league.InnerHtml.Replace("&nbsp;", " ")))
                     });
                 }
             }


             return leagues;
        }



        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTeams(string id)
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(string.Format(BaseTeamUrl, id));
            var teamTags = document.DocumentNode.SelectNodes("//table[@class='generic']");
            List<TeamModel> teams = new List<TeamModel>();
            ///  get image '//img[@id]/@src'
            ///  get url '//td/a/@href'
            ///  get name '//td/a[@class='header_bold']
            ///  get record '//td/font[@class='header_bold']'
            
            foreach (var team in teamTags)
            {
                string teamUrl = team.SelectNodes("tr/td/a/@href")[0].GetAttributeValue("href", "NULL");
                teams.Add(new TeamModel()
                {
                    TeamId = Convert.ToInt32(teamUrl.Substring(teamUrl.IndexOf("ClubTeamID=")).Replace("ClubTeamID=", "")),
                    Name = team.SelectNodes("tr/td/a[@class='header_bold']")[0].InnerHtml,
                    Record = team.SelectNodes("tr/td/font[@class='header_bold']")[0].InnerHtml.Replace("&nbsp;", " ")
                });
            }

            return Json(teams, JsonRequestBehavior.AllowGet);

        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSchedule(string regionId, string leagueId)
        {
            string url = "http://www.uhlife.com/basketball/schedules_league.aspx?RegionID={0}&SeasonDivisionID={1}";
            var webGet = new HtmlWeb();
            var document = webGet.Load(string.Format(url, regionId, leagueId));
            var gamesTags = document.DocumentNode.SelectNodes("//table[@class='generic']//tr");
            List<ScheduleModel> games = new List<ScheduleModel>();
            ///  get image '//img[@id]/@src'
            ///  get url '//td/a/@href'
            ///  get name '//td/a[@class='header_bold']
            ///  get record '//td/font[@class='header_bold']'
            string week = "";
            foreach (var game in gamesTags)
            {

                if (game.SelectNodes("td//b") != null)
                {
                    week = game.SelectNodes("td//b")[0].InnerHtml;
                }
                var cells = game.SelectNodes("td");
                if (cells[0].SelectNodes("a") != null){
                    string[] scores = null;
                    if (cells[5].SelectNodes("a[@disabled='disabled']") == null)
                    {
                        scores = cells[5].SelectNodes("a")[0].InnerHtml.Split('-');
                    }
                    else
                    {
                        scores = " - ".Split('-');
                    }
                   
                    if (cells[0].SelectNodes("a")[0].GetAttributeValue("href", "") != "")
                    {
                        string schId = cells[5].SelectNodes("a")[0].GetAttributeValue("href", "0").Replace("boxscore.aspx?RegionID=" + regionId + "&amp;SeasonDivisionID=" + leagueId + "&amp;ScheduleID=", "");
                        if (schId != "0")
                            schId = schId.Substring(0, schId.IndexOf("&amp;TimePeriodID="));
                       
                        games.Add(new ScheduleModel()
                        {
                            ScheduleId = Convert.ToInt32(schId),
                            HomeTeamId = Convert.ToInt32(cells[0].SelectNodes("a")[0].GetAttributeValue("href", "").Replace("schedule.aspx?RegionID=" + regionId + "&amp;SeasonDivisionID=" + leagueId + "&amp;ClubTeamID=", "")),
                            HomeTeamName = cells[0].SelectNodes("a")[0].InnerHtml,
                            AwayTeamName = cells[1].SelectNodes("a")[0].InnerHtml,
                            AwayTeamId = Convert.ToInt32(cells[1].SelectNodes("a")[0].GetAttributeValue("href", "").Replace("schedule.aspx?RegionID=" + regionId + "&amp;SeasonDivisionID=" + leagueId + "&amp;ClubTeamID=", "")),
                            Location = cells[2].InnerHtml,
                            GameTime = DateTime.Parse(cells[4].InnerHtml + " " + cells[3].InnerHtml),
                            HomeScore = scores[0] ?? "",
                            AwayScore = scores[1] ?? "",
                            Week = week
                        });
                    }
                    
                }
              
            }

            return Json(games, JsonRequestBehavior.AllowGet);

        }


        public JsonResult getBoxScore(string scheduleId)
        {
            string url = "http://www.uhlife.com/basketball/boxscore.aspx?ScheduleID={0}";
            var webGet = new HtmlWeb();
            var document = webGet.Load(string.Format(url, scheduleId));
            var tags = document.DocumentNode.SelectNodes("//table[@class='generic_verdana_small']//tr");
            List<BoxScoreModel> games = new List<BoxScoreModel>();

            foreach (var tag in tags)
            {
                
            }
            return Json(games, JsonRequestBehavior.AllowGet);
        }

        private string StripHTML(string htmlString)
        {

            //This pattern Matches everything found inside html tags;

            //(.|\n) - > Look for any character or a new line

            // *?  -> 0 or more occurences, and make a non-greedy search meaning

            //That the match will stop at the first available '>' it sees, and not at the last one

            //(if it stopped at the last one we could have overlooked

            //nested HTML tags inside a bigger HTML tag..)

            // Thanks to Oisin and Hugh Brown for helping on this one...

            string pattern = @"<(.|\n)*?>";



            return Regex.Replace(htmlString, pattern, string.Empty);

        }




    }
}
