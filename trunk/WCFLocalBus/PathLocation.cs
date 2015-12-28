using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WCFLocalBus
{
    public class PathLocation
    {
        private string mode;


        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private double startLatitude;

        public double StartLatitude
        {
            get { return startLatitude; }
            set { startLatitude = value; }
        }
        private double startLongitude;

        public double StartLongitude
        {
            get { return startLongitude; }
            set { startLongitude = value; }
        }

        private double endLatitude;

        public double EndLatitude
        {
            get { return endLatitude; }
            set { endLatitude = value; }
        }
        private double endLongitude;

        public double EndLongitude
        {
            get { return endLongitude; }
            set { endLongitude = value; }
        }

        private int busServiceNum;

        public int BusServiceNum
        {
            get { return busServiceNum; }
            set { busServiceNum = value; }
        }

        private int sequence;

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }
        private string location;

        public string Location
        {
            get { return location; }
            set { location = value; }
        }
    }
}
