using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFLocalBus
{
    public class JourneyPlan
    {
        private int journeyID;

        public int JourneyID
        {
            get { return journeyID; }
            set { journeyID = value; }
        }

        private string from;

        public string From
        {
            get { return from; }
            set { from = value; }
        }
        private string to;

        public string To
        {
            get { return to; }
            set { to = value; }
        }
    }
}