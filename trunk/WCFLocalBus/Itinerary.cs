using System;
using System.Net;

namespace WCFLocalBus
{
    public class Itinerary
    {
        private string place;

        public string Place
        {
            get { return place; }
            set { place = value; }
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
