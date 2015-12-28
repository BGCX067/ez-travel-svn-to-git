using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFLocalBus
{
    public class DBMarker
    {
        private string jID;

        public string JID
        {
            get { return jID; }
            set { jID = value; }
        }

        private string markerID;

        public string MarkerID
        {
            get { return markerID; }
            set { markerID = value; }
        }

        private string direction;

        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
    }
}