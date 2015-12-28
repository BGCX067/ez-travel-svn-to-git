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
    public class ItineraryClass
    {
        private int number;


        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
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
