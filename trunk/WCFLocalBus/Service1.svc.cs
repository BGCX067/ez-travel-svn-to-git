using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;

namespace WCFLocalBus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {

        private string conString = "Server=46.4.195.237; Initial Catalog=EZTravel; User Id=sa; Password=u1!sa;";
        SqlConnection conn = new SqlConnection();
        SqlCommand sqlComm;

        public List<busStop> GetBusStop(int busID, double startLat, double startLong, double endLat,double endLong)
        {
            List<busStop> BusStopList = new List<busStop>();

            try
            {
                conn.ConnectionString = conString;
                conn.Open();
                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                sqlComm.CommandText = "SELECT busStop,Latitude,Longitude from BusDirection where serviceNum=@busID and busStopID between  (select busStopID from BusDirection where Latitude=@startLat and Longitude=@startLong) and (select busStopID from BusDirection where Latitude= @endLat  and Longitude=@endLong)";
                sqlComm.Parameters.AddWithValue("@busID", busID);
                sqlComm.Parameters.AddWithValue("@startLat", startLat);
                sqlComm.Parameters.AddWithValue("@startLong", startLong);
                sqlComm.Parameters.AddWithValue("@endLat",endLat);
                sqlComm.Parameters.AddWithValue("@endLong", endLong);
                SqlDataReader dr = sqlComm.ExecuteReader();

                while (dr.Read())
                {
                    busStop bs = new busStop();
                    bs.BusStopName = dr["busStop"].ToString();
                    bs.Latitude =Convert.ToDouble(dr["Latitude"].ToString());
                    bs.Longitude = Convert.ToDouble(dr["Longitude"].ToString());
                    BusStopList.Add(bs);
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return BusStopList;
        }

        public List<PathLocation> GetPath(int journeyID)
        {
            List<PathLocation> pathList = new List<PathLocation>();

            try
            {
                conn.ConnectionString = conString;
                conn.Open();

                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                int count = 0;

                string[] modeList=new string[]{"Walk","Train","Bus"};

                for (int i = 0; i < modeList.Length; i++)
                {
                    
                    sqlComm.CommandText = "SELECT Sequence,Location,startLat,startLong,endLat,endLong from "+modeList[i]+" where JourneyID=@jID"+count;

                    if (modeList[i].Equals("Bus"))
                    {
                        sqlComm.CommandText = "SELECT Sequence,serviceNum,startLat,startLong,endLat,endLong from "+modeList[i]+" where JourneyID=@jID"+count;
                    }

                    sqlComm.Parameters.AddWithValue("@jID"+count, journeyID);

                    SqlDataReader dr = sqlComm.ExecuteReader();

                    while (dr.Read())
                    {
                        PathLocation p = new PathLocation();
                        p.Mode = modeList[i];

                        if (modeList[i].Equals("Bus"))
                        {
                            p.BusServiceNum = Convert.ToInt32(dr["serviceNum"].ToString());
                            p.Location = p.BusServiceNum.ToString();
                        }
                        else
                        {
                            p.Location = dr["Location"].ToString();
                        }
                        p.Sequence = Convert.ToInt32(dr["Sequence"].ToString());
                       
                        p.StartLatitude = Convert.ToDouble(dr["startLat"].ToString());
                        p.StartLongitude = Convert.ToDouble(dr["startLong"].ToString());
                        p.EndLatitude = Convert.ToDouble(dr["endLat"].ToString());
                        p.EndLongitude = Convert.ToDouble(dr["endLong"].ToString());
                        pathList.Add(p);
                        
                    }
                    count++;
                    dr.Close();

    
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            pathList = arrangeInSequence(pathList);

            return pathList;
        }

        List<PathLocation> arrangeInSequence(List<PathLocation> pathList)
        {
            List<PathLocation> arrangedPathList = new List<PathLocation>();
            int count = 1;

            while (count <= pathList.Count)
            {
                for (int j = 0; j < pathList.Count; j++)
                {
                    PathLocation p = pathList[j];

                    if (p.Sequence == count)
                    {
                        arrangedPathList.Add(p);
                       
                    }
                }
                count++;
            }

            return arrangedPathList;
        }

        public List<DBMarker> retrieveMarkerInfo(string jid)
        {
            List<DBMarker> dbmList = new List<DBMarker>();

            try
            {
                conn.ConnectionString = conString;
                conn.Open();
                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                sqlComm.CommandText = "Select * from Marker where JID = @jid ";
                sqlComm.Parameters.AddWithValue("@jid", jid);
                SqlDataReader dr = sqlComm.ExecuteReader();

                while (dr.Read())
                {
                    DBMarker dbm = new DBMarker();
                    dbm.JID = dr[0].ToString();
                    dbm.MarkerID = dr[1].ToString();
                    dbm.Direction = dr[2].ToString();
                    dbmList.Add(dbm);
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dbmList;
        }

        public int GetJourney(string from, string to)
        {
           int jID = 0;
            try
            {
                
                conn.ConnectionString = conString;
                conn.Open();
                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                sqlComm.CommandText = "SELECT JourneyID FROM JourneyPlan WHERE FromLocation=@fromLoc and ToLocation=@toLoc";
                sqlComm.Parameters.AddWithValue("@fromLoc", from);
                sqlComm.Parameters.AddWithValue("@toLoc", to);
                SqlDataReader dr = sqlComm.ExecuteReader();

                while (dr.Read())
                {
                    jID= Convert.ToInt32(dr["JourneyID"].ToString());
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return jID;
        }

        public List<string> GetLocation()
        {
            List<string> journeyList = new List<string>();

            try
            {
                conn.ConnectionString = conString;
                conn.Open();
                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                sqlComm.CommandText = "Select FromLocation,ToLocation from JourneyPlan ";
                SqlDataReader dr = sqlComm.ExecuteReader();
               
                while (dr.Read())
                {
                                                      
                    journeyList.Add(dr["FromLocation"].ToString());
                    journeyList.Add(dr["ToLocation"].ToString());
                }
                dr.Close();

                List<string> previous = new List<string>();
                for (int i = 0; i < journeyList.Count; i++)
                {
                    foreach (string s in previous)
                    {
                        if (journeyList[i].ToString().Equals(s))
                        {
                            journeyList.RemoveAt(i);
                        }
                    }
                    previous.Add(journeyList[i].ToString());
                   


                }
               
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return journeyList;
        }
        public List<Itinerary> retriveItinerary(string deviceID, int journeyID)
        {
            List<Itinerary> iList = new List<Itinerary>(); 

            try
            {
                conn.ConnectionString = conString;
                conn.Open();
                sqlComm = new SqlCommand();
                sqlComm.Connection = conn;
                sqlComm.CommandText = "Select place_Name, latitude, longitude from Itinerary where deviceID=@deviceID AND JID=@jid ";
                sqlComm.Parameters.AddWithValue("@deviceID", deviceID);
                sqlComm.Parameters.AddWithValue("@jid", journeyID);
                SqlDataReader dr = sqlComm.ExecuteReader();

                while (dr.Read())
                {

                    iList.Add(new Itinerary {Place=dr["place_Name"].ToString(), Latitude=Convert.ToDouble(dr["latitude"].ToString()), Longitude=Convert.ToDouble(dr["longitude"].ToString()) });
                }
                dr.Close();

              
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return iList;
        }
    }
}
