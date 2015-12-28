using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFLocalBus
{
    public class busStop
    {
        string busStopName;

        public string BusStopName
        {
            get { return busStopName; }
            set { busStopName = value; }
        }

        private double latitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private double longitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
    }
}