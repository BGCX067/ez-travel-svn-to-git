using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFLocalBus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
   [ServiceContract]
    public interface IService1
    {
       [OperationContract]
       List<busStop> GetBusStop(int busID, double startLat, double startLong, double endLat, double endLong);

        [OperationContract]
        List<PathLocation> GetPath(int journeyID);

        [OperationContract]
        List<DBMarker> retrieveMarkerInfo(string jid);

        [OperationContract]
        int GetJourney(string from, string to);

        [OperationContract]
        List<string> GetLocation();

        [OperationContract]
        List<Itinerary> retriveItinerary(string deviceID, int journeyID);
    }
    
}
