using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace EZ_Travel.Resources.Class
{
    public class Language
    {
        private string lan;

        public string Lan
        {
            get { return lan; }
        }
        private string journey_Planner;

        public string Journey_Planner
        {
            get { return journey_Planner; }
        }

        private string journey;

        public string Journey
        {
            get { return journey; }
        }

        private string route;

        public string Route
        {
            get { return route; }
        }

      
        private string bus_Stop;

        public string Bus_Stop
        {
            get { return bus_Stop; }
        }

        private string startingPt;

        public string StartingPt
        {
            get { return startingPt; }
        }

        private string destination;

        public string Destination
        {
            get { return destination; }
        }

        private string plan;

        public string Plan
        {
            get { return plan; }
        }

        private string nGPS;

        public string NGPS
        {
            get { return nGPS; }
        }

        private string sPageTitle;

        public string SPageTitle
        {
            get { return sPageTitle; }
        }

        private string tLang;

        public string TLang
        {
            get { return tLang; }
        }

        private string bSet;

        public string BSet
        {
            get { return bSet; }
        }

        private string nextStop;

        public string NextStop
        {
            get { return nextStop; }
        }

        private string listOfStops;

        public string ListOfStops
        {
            get { return listOfStops; }
        }

        private string route_Plan;

        public string Route_Plan
        {
            get { return route_Plan; }
        }

        private string start;

        public string Start
        {
            get { return start; }
        }

        private string resetGuide;

        public string ResetGuide
        {
            get { return resetGuide; }
            set { resetGuide = value; }
        }


        public Language(string chooseLang)
        {
            switch (chooseLang)
            {
                case "English":

                    this.journey = "Journey";
                    this.route_Plan = "Route Plan";
                    this.route = "Route";
                    this.bus_Stop = "Bus Stop";
                    this.startingPt = "Starting Point";
                    this.destination = "Destination";
                    this.plan = "Plan";
                    this.nGPS = "No GPS";
                    this.sPageTitle = "Settings";
                    this.tLang = "Language";
                    this.bSet = "Set";
                    this.nextStop = "Next Stop:";
                    this.listOfStops = "List of Bus Stops";
                    this.journey_Planner = "Journey Planner";
                    this.start = "Start";
                    this.resetGuide = "Reset Guide";

                    break;
                
                case "Chinese":

                    this.journey = "旅程";
                    this.route_Plan = "路线计划";
                    this.route = "路线";
                    this.bus_Stop = "巴士站";
                    this.startingPt = "起点";
                    this.destination = "目的地";
                    this.plan = "计划";
                    this.nGPS = "没有GPS";
                    this.sPageTitle = "设置";
                    this.tLang = "语言";
                    this.bSet = "设置";
                    this.nextStop = "下一站：";
                    this.listOfStops = "巴士站列表";
                    this.journey_Planner = "旅程规划";
                    this.start = "开始";
                    this.resetGuide = "重置指南";

                    break;
                
                case "French":

                    this.journey = "voyage";
                    this.route_Plan = "plan d'itinéraire";
                    this.route = "Route";
                    this.bus_Stop = "Arrêtez de bus";
                    this.startingPt = "point de départ";
                    this.destination = "de destination";
                    this.plan = "plan de";
                    this.nGPS = "Pas de GPS";
                    this.sPageTitle = "réglages";
                    this.tLang = "Langue";
                    this.bSet = "Set";
                    this.nextStop = "Prochaine étape:";
                    this.listOfStops = "Liste des arrêts d'autobus";
                    this.journey_Planner = "Planificateur de voyage";
                    this.start = "démarrage";
                    this.resetGuide = "réinitialiser le guide";

                    break;

            }
        }
    }
}
