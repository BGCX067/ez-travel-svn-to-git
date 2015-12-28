using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using EZ_Travel.Resources.Model;
using System.Xml.Linq;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Controls.Maps;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using EZ_Travel.localService;
using Microsoft.Devices;
using EZ_Travel.Resources.Class;
using System.IO;
using System.IO.IsolatedStorage;



namespace EZ_Travel
{
    public partial class RoadMode : PhoneApplicationPage
    {
        #region const

        private SoundEffect alarmSound;
        public static int journeyID=0;
        public GeoCoordinate geoLocation = null;
        public bool startSelectionIndex = false;
        public bool endSelectionIndex = false;
        bool favouriteIndex = false;
        public List<string> Dictionary=new List<string>();
        List<int> journeyList;
        PushpinModel ppM ;
        private GeoCoordinateWatcher geoWatcher;
        private double previousLat = 0.0;
        private double previousLong = 0.0;
        private const string conString = "";
        private List<string> busList ;
        List<localService.busStop> busStopList;
        List<busStop> pendingBusStopList;
        List<localService.PathLocation> pathList;
        Location previous = null;
        Location current = null;
        MapLayer myRouteLayer ;
        MapLayer pinLayer;
        MapLayer GPSLayer;
        MapLayer busLayer;
        MapLayer POILayer;
        MapLayer ItineraryLayer;
        MapPolyline routeLine;
        int count = 0;
        public static Resources.Class.Language lan;
       Location startLocation;
       Location endLocation;
        List<GeoCoordinate> simulateLocation;
        DispatcherTimer simulateTimer;
        DispatcherTimer vibrateTimer;
        bool editText =false;
        bool busStopCount = false;
        Pushpin p = null;
        int poiIndex = 0;
        public static string deviceID="";
        int itineraryFaveIndex = 0;
    
        public static  IsolatedStorageFileStream fileStream = null;
        public static List<Favourite> favouriteList = new List<Favourite>();
        string currentLocation = "City Hall MRT Station";


        #endregion

        #region pageload
        public RoadMode()
        {
            InitializeComponent();
            geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            geoWatcher.MovementThreshold = 10.0f;
            clearLayer();
            loadDictionary();
            tbxStatus.Visibility = Visibility.Collapsed;
            new Thread(startLocServInBackground).Start();
            Location location = new Location();
            LoadSound("Resources/Sound/Alight.wav", out alarmSound);
            DispatcherTimer XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);
            XnaDispatchTimer.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            XnaDispatchTimer.Start();
            setLang();
            lbxStart.Visibility = Visibility.Collapsed;
            lbxDestination.Visibility = Visibility.Collapsed;
            lbxIternary.Visibility = Visibility.Collapsed;
            gdShow.Visibility = Visibility.Collapsed;
                       

        }

      
        #endregion

        #region retrieveDictionary
        void loadDictionary()
        {
            localService.Service1Client dictionaryClient = new Service1Client();
            dictionaryClient.GetLocationAsync();
            dictionaryClient.GetLocationCompleted+=new EventHandler<GetLocationCompletedEventArgs>(dictionaryClient_GetLocationCompleted);
        }

        void dictionaryClient_GetLocationCompleted(object sender, GetLocationCompletedEventArgs e)
        {
            Dictionary = e.Result;
        }

        #endregion


        #region setSimulateCord
        void setCordList()
        {
            string destination = tbxDestination.Text;

            switch (destination)
            {

                case "Grand Copthorne Hotel":

                    simulateLocation.Add(new GeoCoordinate(1.2936, 103.8523));
                    simulateLocation.Add(new GeoCoordinate(1.2957, 103.8506));
                    simulateLocation.Add(new GeoCoordinate(1.2965, 103.8493));
                    simulateLocation.Add(new GeoCoordinate(1.2984, 103.8472));
                    simulateLocation.Add(new GeoCoordinate(1.2988, 103.8432));
                    simulateLocation.Add(new GeoCoordinate(1.2999, 103.8405));
                    simulateLocation.Add(new GeoCoordinate(1.301, 103.8356));
                    simulateLocation.Add(new GeoCoordinate(1.3045, 103.8327));
                    simulateLocation.Add(new GeoCoordinate(1.3017, 103.8296));
                    simulateLocation.Add(new GeoCoordinate(1.2988, 103.8302));
                    simulateLocation.Add(new GeoCoordinate(1.2952, 103.8319));
                    simulateLocation.Add(new GeoCoordinate(1.2964, 103.8312));
                    simulateLocation.Add(new GeoCoordinate(1.2917, 103.8355));
                    break;
                
                case "Plaza Singapura Shopping Centre":

                    simulateLocation.Add(new GeoCoordinate(1.2899, 103.8337));
                    simulateLocation.Add(new GeoCoordinate(1.2892, 103.8364));
                    simulateLocation.Add(new GeoCoordinate(1.2894, 103.84));
                    simulateLocation.Add(new GeoCoordinate(1.2880, 103.8427));
                    simulateLocation.Add(new GeoCoordinate(1.2865, 103.8451));
                    simulateLocation.Add(new GeoCoordinate(1.2879, 103.8463));
                    break;

                case "National Museum of Singapore":

                    simulateLocation.Add(new GeoCoordinate(1.2936, 103.8523));
                    simulateLocation.Add(new GeoCoordinate(1.2957, 103.8506));

                    break;
            }
        }
        private void stimulatedGeoWatcher(GeoCoordinate e)
        {           
            p = ppM.createPushPin("Location", e);
            geoLocation = e;

            if (GPSLayer.Children.Contains(p) == false)
            {
                GPSLayer.Children.Add(p);

            };

            previousLat = e.Latitude;
            previousLong = e.Longitude;

            getNearbyBusStop();
            lbxBusStop.Items.Clear();
            foreach (busStop b in busStopList)
            {
                lbxBusStop.Items.Add(b.BusStopName);
            }

            if (lbxBusStop.Items.Count ==2)
            {
                alarmSound.Play();
                TimeSpan seconds = alarmSound.Duration;
                vibrate(seconds);
            }

            var distance = geoLocation.GetDistanceTo(endLocation);

            if (distance < 200)
            {
                MessageBox.Show("You have reached your destination");
            }
            if (busStopCount == true )
            {
                if (lbxBusStop.Items.Count == 0 )
                {
                    MessageBox.Show("You have to alight here");
                }
            }
       
            getNearby();
        }
        void simulateTimer_Tick(object sender, EventArgs e)
        {
            stimulatedGeoWatcher(simulateLocation[count]);
            count++;

            if (lbxBusStop.Items.Count == 0)
            {
                simulateTimer.Stop();
            }
        }
      
        #endregion

        #region GPS System
        void geoWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:

                    if (geoWatcher.Permission == GeoPositionPermission.Denied)
                    {
                        tbxStatus.Text = "You have disabled location service";
                        visible();
                    }
                    else
                    {
                        tbxStatus.Text = "Location Service is not functioning on this device";
                        visible();
                    }
                    break;
                case GeoPositionStatus.Initializing:
                    tbxStatus.Text = "Location service is retrieving data...";
                    visible();
                    break;

                case GeoPositionStatus.NoData:

                    tbxStatus.Text = "Location data is unavailable";
                    MessageBoxResult res = MessageBox.Show("The system is unable to detect GPS location. Do you want to swtich to AR mode?", "AR Mode", MessageBoxButton.OKCancel);
                    if (res==MessageBoxResult.OK)
                    {
                        NavigationService.Navigate(new Uri("/AR.xaml", UriKind.Relative));
                    }
                    visible();
                    break;

                case GeoPositionStatus.Ready:
                    tbxStatus.Text = "Location data is available";
                    visible();
                    tbxStatus.Visibility = Visibility.Collapsed;

                    break;
            }

        }
        void startLocServInBackground()
        {
            geoWatcher.TryStart(true, TimeSpan.FromMilliseconds(60000));

        }
        void geoWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Pushpin p = null;
            p = new Pushpin();
            p = ppM.createPushPin("Location", e.Position.Location);
            geoLocation = e.Position.Location;

            if (GPSLayer.Children.Contains(p) == false)
            {
                GPSLayer.Children.Add(p);

            };

            previousLat = e.Position.Location.Latitude;
            previousLong = e.Position.Location.Longitude;


            getNearbyBusStop();
            lbxBusStop.Items.Clear();
            foreach (busStop b in busStopList)
            {
                lbxBusStop.Items.Add(b.BusStopName);
            }

            if (lbxBusStop.Items.Count==2)
            {
                alarmSound.Play();
                TimeSpan seconds = alarmSound.Duration;
                vibrate(seconds);
            }
            var distance = geoLocation.GetDistanceTo(endLocation);

            if (distance < 200)
            {
                MessageBox.Show("You have reached your destination");
            }

            if (busStopCount == true )
            {
                if (lbxBusStop.Items.Count == 0)
                {
                    MessageBox.Show("You have to alight here");
                }
            }
            getNearby();


        }
        void vibrate(TimeSpan seconds)
        {
            vibrateTimer.Interval = seconds;
            vibrateTimer.Tick += new EventHandler(vibrateTimer_Tick);
            vibrateTimer.Start();
        }

        void vibrateTimer_Tick(object sender, EventArgs e)
        {
            VibrateController vibrate = VibrateController.Default;
            vibrate.Start(TimeSpan.FromMilliseconds(5000));
            vibrateTimer.Stop();
        }

      
        internal GeocodeService.GeocodeResult[] geocodeResults;
        #endregion

        void visible()
        {
            tbxStatus.Visibility = Visibility.Visible;
        }

        #region Zoom Mode
        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            myMap.ZoomLevel -= 1;
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            myMap.ZoomLevel += 1;

        }
        #endregion
                           
        void setMapPath(int journeyID)
        {
            localService.Service1Client pathService = new localService.Service1Client();
            pathService.GetPathAsync(journeyID);
            pathService.GetPathCompleted+=new EventHandler<localService.GetPathCompletedEventArgs>(busService_GetPathCompleted);
                           
        }

        #region WCFMethod Call


        void getNearby()
        {
            WebClient client = new WebClient();
            client.Headers["AccountKey"] = "g53o8YQKLimhdxPjRRBaSw==";
            client.Headers["UniqueUserID"] = "50da7f8f-22b4-4c52-ba3f-fa35a2973a64";
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("http://api.projectnimbus.org/stbodataservice.svc/PlaceSet?Latitude=" + previousLat + "&Longitude=" + previousLong + "&Distance=1000"));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string resStr = e.Result;


            XNamespace atomNS = "http://www.w3.org/2005/Atom";
            XNamespace dNS = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace mNS = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

            var results = (
            from item in XElement.Parse(resStr).Descendants(atomNS + "entry")
            let rest = item.Element(atomNS + "content").Element(mNS + "properties")
            select new Resources.Class.POI
            {
                PlaceName = rest.Element(dNS + "Name").Value,
                Latitude = Convert.ToDouble(rest.Element(dNS + "Latitude").Value),
                Longitude = Convert.ToDouble(rest.Element(dNS + "Longitude").Value),


            }
             );


            foreach (var r in results)
            {
                Location x = new Location();
                x.Latitude = r.Latitude;
                x.Longitude = r.Longitude;
               POILayer.Children.Add( ppM.createPushPin("POI", x));
               ppM.createPOIPushPin(x, r.PlaceName);
                
               
            }
        }


        private void busService_GetPathCompleted(object sender, localService.GetPathCompletedEventArgs e)
        {
            pathList = e.Result;
            startLocation.Latitude = pathList[0].StartLatitude;
            startLocation.Longitude = pathList[0].StartLongitude;
            endLocation.Latitude = pathList[pathList.Count-1].EndLatitude;
            endLocation.Longitude = pathList[pathList.Count-1].EndLongitude;

            for (int i = 0; i < pathList.Count; i++)
            {
                
               
                if (pathList[i].BusServiceNum != 0)
                {
                    localService.Service1Client transitPath = new localService.Service1Client();
                    transitPath.GetBusStopAsync(Convert.ToInt32(pathList[i].BusServiceNum),pathList[i].StartLatitude,pathList[i].StartLongitude,pathList[i].EndLatitude,pathList[i].EndLongitude);
                    transitPath.GetBusStopCompleted+=new EventHandler<localService.GetBusStopCompletedEventArgs>(transitPath_GetBusStopCompleted);

                    previous = new Location();
                    previous.Latitude = pathList[i].StartLatitude;
                    previous.Longitude = pathList[i].StartLongitude;

                    pinLayer.Children.Add(ppM.createPushPin(pathList[i].Mode, previous, pathList[i].Sequence,pathList[i].Location));

                    current = new Location();
                    current.Latitude = pathList[i].EndLatitude;
                    current.Longitude = pathList[i].EndLongitude;
                    
                    pinLayer.Children.Add(ppM.createPushPin(pathList[i].Mode, current, pathList[i].Sequence, pathList[i].Location));

                

                }
                               
                else
                {
                    if (pathList[i].Mode.Equals("Walk"))
                    {
                      
                        routeLine = new MapPolyline();
                       System.Windows.Media.Color routeColor = Colors.Red;
                        routeLine.Locations = new LocationCollection();
                        SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
                        routeLine.Stroke = routeBrush;
                        routeLine.Opacity = 0.65;
                        routeLine.StrokeThickness = 5.0;
                        myRouteLayer.Children.Add(routeLine);
                     
                    }
                    else
                    {
                        routeLine = new MapPolyline();
                        System.Windows.Media.Color routeColor = Colors.Green;
                        routeLine.Locations = new LocationCollection();
                        SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
                        routeLine.Stroke = routeBrush;
                        routeLine.Opacity = 0.65;
                        routeLine.StrokeThickness = 5.0;
                        myRouteLayer.Children.Add(routeLine);
                    }

                    previous = new Location();
                    previous.Latitude = pathList[i].StartLatitude;
                    previous.Longitude = pathList[i].StartLongitude;
                    routeLine.Locations.Add(previous);
                    pinLayer.Children.Add(ppM.createPushPin(pathList[i].Mode, previous, pathList[i].Sequence, pathList[i].Location));

                    current = new Location();
                    current.Latitude = pathList[i].EndLatitude;
                    current.Longitude = pathList[i].EndLongitude;
                    routeLine.Locations.Add(current);

                                      
                }
                if (i == pathList.Count - 1)
                {
                    var rect = LocationRect.CreateLocationRect(startLocation, endLocation);
                    myMap.SetView(rect);
                   
                } 
 
            }
        }

        private void transitPath_GetBusStopCompleted(object sender, localService.GetBusStopCompletedEventArgs e)
        {
            busStopList = e.Result;
          
            geocodeResults = new GeocodeService.GeocodeResult[busStopList.Count];
          

            for (int a = 0; a < busStopList.Count; a++)
            {
                
                Geocode((busStopList[a].Latitude+","+busStopList[a].Longitude).ToString(),a);
                lbxBusStop.Items.Add(busStopList[a].BusStopName);
                busStopCount = true;
            }
          
          }

      

        private void Geocode(string strAddress, int wayPointIndex)
        {
            GeocodeService.GeocodeServiceClient geocodeService = new GeocodeService.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            geocodeService.GeocodeCompleted += new EventHandler<GeocodeService.GeocodeCompletedEventArgs>(geoCodeService_GeocodeCompleted);

            GeocodeService.GeocodeRequest geocodeRequest = new GeocodeService.GeocodeRequest();
            geocodeRequest.Credentials = new Credentials();

            geocodeRequest.Credentials.ApplicationId = ((ApplicationIdCredentialsProvider)myMap.CredentialsProvider).ApplicationId;
            geocodeRequest.Query = strAddress;

            geocodeService.GeocodeAsync(geocodeRequest, wayPointIndex);
        }

        private void geoCodeService_GeocodeCompleted(object sender, GeocodeService.GeocodeCompletedEventArgs e)
        {
            int waypointIndex = Convert.ToInt32(e.UserState);
            geocodeResults[waypointIndex] = e.Result.Results[0];
            
            bool doneGeocoding = true;

            foreach (GeocodeService.GeocodeResult gr in geocodeResults)
            {
                if (gr == null)
                {
                    doneGeocoding = false;
                }
            }

            if (doneGeocoding)
                CalculateRoute(geocodeResults);
        }
        private void CalculateRoute(GeocodeService.GeocodeResult[] results)
        {
            RouteService.RouteServiceClient routeService = new RouteService.RouteServiceClient("BasicHttpBinding_IRouteService");

            routeService.CalculateRouteCompleted += new EventHandler<RouteService.CalculateRouteCompletedEventArgs>(routeService_CalculateRouteCompleted);

            RouteService.RouteRequest routeRequest = new RouteService.RouteRequest();
            routeRequest.Credentials = new Credentials();
            routeRequest.Credentials.ApplicationId = ((ApplicationIdCredentialsProvider)myMap.CredentialsProvider).ApplicationId;

            routeRequest.Options = new RouteService.RouteOptions();
            routeRequest.Options.RoutePathType = RouteService.RoutePathType.Points;

            routeRequest.Waypoints = new System.Collections.ObjectModel.ObservableCollection<RouteService.Waypoint>();

            foreach (GeocodeService.GeocodeResult result in results)
            {
                routeRequest.Waypoints.Add(GeoCodeResultToWaypoint(result));
            }
            routeService.CalculateRouteAsync(routeRequest);
        }

        private RouteService.Waypoint GeoCodeResultToWaypoint(GeocodeService.GeocodeResult result)
        {
            RouteService.Waypoint waypoint = new RouteService.Waypoint();
            waypoint.Description = result.DisplayName;
            waypoint.Location = new Location();
            waypoint.Location.Latitude = result.Locations[0].Latitude;
            waypoint.Location.Longitude = result.Locations[0].Longitude;
            return waypoint;
        }

        private void routeService_CalculateRouteCompleted(object sender, RouteService.CalculateRouteCompletedEventArgs e)
        {
            if ((e.Result.ResponseSummary.StatusCode == RouteService.ResponseStatusCode.Success) && (e.Result.Result.Legs.Count != 0))
            {
               System.Windows.Media.Color routeColor = Colors.Blue;
                SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();

                routeLine.Stroke = routeBrush;
                routeLine.Opacity = 0.65;
                routeLine.StrokeThickness = 5.0;
                
                foreach (Location p in e.Result.Result.RoutePath.Points)
                {
                    Location x = new Location();
                    x.Latitude = p.Latitude;
                    x.Longitude = p.Longitude;

                    routeLine.Locations.Add(x);

                }

               
                myRouteLayer.Children.Add(routeLine);

            }
        }


        private void journeyClient_GetJourneyCompleted(object sender, GetJourneyCompletedEventArgs e)
        {
            journeyID = e.Result;
            setMapPath(journeyID);
            RoadPanaroma.DefaultItem = RoadPanaroma.Items[1];
            if ( ItineraryLayer.Children.Count==0)
            {
                localService.Service1Client retrieveItinerary = new Service1Client();
                retrieveItinerary.retriveItineraryAsync(deviceID, journeyID);
                retrieveItinerary.retriveItineraryCompleted += new EventHandler<retriveItineraryCompletedEventArgs>(retrieveItinerary_retriveItineraryCompleted);
                
            }

        }
        private void LoadSound(String SoundFilePath, out SoundEffect Sound)
        {
            // For error checking, assume we'll fail to load the file.
            Sound = null;

            try
            {
                
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri(SoundFilePath, UriKind.Relative));
 
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Couldn't load sound " + SoundFilePath);
            }
        }
        private void getNearbyBusStop()
        {
            pendingBusStopList.Clear();
          

            for (int i = 0; i < busStopList.Count; i++)
            {

                var distanceBetweenPinNextBusStop = (geoLocation.GetDistanceTo(new GeoCoordinate(busStopList[i].Latitude, busStopList[i].Longitude)));

                if (distanceBetweenPinNextBusStop < 300)
                {

                    tbxStop.Text = busStopList[i].BusStopName;
                    busStopList.RemoveAt(i);
                }

            }
        }
    
        #endregion
     

        #region AutoComplete

        private void tbxStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            List <string> startList =new List <string>();
         
                                   
                int wordCount = tbxStart.Text.Length;

                if (wordCount == 0)
                {
                   
                    lbxStart.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (editText == true)
                    {
                        lbxStart.Visibility = Visibility.Visible;
                        foreach (string s in Dictionary)
                        {
                            if (s.Length >= wordCount)
                            {
                                string subString = s.Substring(0, wordCount);

                                if (tbxStart.Text.Equals(subString, StringComparison.OrdinalIgnoreCase))
                                {
                                    startList.Add(s);
                                }
                            }
                        }

                        lbxStart.ItemsSource = startList;
                    }
                }
               
               
        }

        private void tbxDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> endList = new List<string>();
           
                int wordCount = tbxDestination.Text.Length;

                if (wordCount == 0)
                {
                    lbxDestination.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (editText == true)
                    {
                        lbxDestination.Visibility = Visibility.Visible;
                        foreach (string s in Dictionary)
                        {
                            if (s.Length >= wordCount)
                            {

                                string subString = s.Substring(0, wordCount);
                                if (tbxDestination.Text.Equals(subString, StringComparison.OrdinalIgnoreCase))
                                {
                                    endList.Add(s);
                                }
                            }
                        }

                        lbxDestination.ItemsSource = endList;
                    }
                }
            
        }

        private void lbxStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxStart.SelectedItem != null)
            {
                tbxStart.Text = lbxStart.SelectedItem.ToString();
                lbxStart.Visibility = Visibility.Collapsed;
                editText = false;
             
            }
        }

        private void lbxDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxDestination.SelectedItem != null)
            {
                tbxDestination.Text = lbxDestination.SelectedItem.ToString();
                lbxDestination.Visibility = Visibility.Collapsed;
                
                editText = false;
            }
        }

        #endregion

        #region Events Argument
        private void btnPlan_Click(object sender, RoutedEventArgs e)
        {
            busStopCount = false;
            itineraryFaveIndex=0;
            if (tbxStart.Text!= "" && tbxDestination.Text !="")
            {
                string item=tbxStart.Text +  "," + tbxDestination.Text;
                            
                    bool check = false;

                               StreamWriter writer = null;
                        IsolatedStorageFile favourListStorage = IsolatedStorageFile.GetUserStoreForApplication();
                        fileStream = new IsolatedStorageFileStream("myFavourite.txt", FileMode.Append, favourListStorage);

                        writer = new StreamWriter(RoadMode.fileStream);

                        foreach (Favourite s in favouriteList)
                        {
                            string checkString = tbxStart.Text + " To " + tbxDestination.Text;
                            if (s.Name.Equals(checkString))
                            {
                                check = true;
                                break;
                            }

                        }
                        if (check == false)
                        {
                            writer.WriteLine(item);
                        }

                        writer.Flush();
                        writer.Close();
                        fileStream.Close();
                       clearLayer();
                journeyList = new List<int>();
                ppM = new PushpinModel();
                busList = new List<string>();
                busStopList = new List<localService.busStop>();
                pendingBusStopList = new List<busStop>();
                pathList = new List<localService.PathLocation>();
                myRouteLayer = new MapLayer();
                pinLayer = new MapLayer();
                GPSLayer = new MapLayer();
                busLayer = new MapLayer();
                POILayer = new MapLayer();
                ItineraryLayer = new MapLayer();
                myMap.Children.Add(ItineraryLayer);
                p = new Pushpin();
                count = 0;
                simulateLocation = new List<GeoCoordinate>();
                simulateTimer = new DispatcherTimer();
                vibrateTimer = new DispatcherTimer();
                startLocation = new Location();
                endLocation = new Location();
                myMap.Children.Add(busLayer);
                myMap.Children.Add(GPSLayer);
                myMap.Children.Add(PushpinModel.contentLayer);
                myMap.Children.Add(pinLayer);
                myMap.Children.Add(myRouteLayer);
                myMap.Children.Add(POILayer);
                myMap.Children.Add(PushpinModel.POIContentLayer);
              
                POILayer.Visibility = Visibility.Collapsed;
                PushpinModel.POIContentLayer.Visibility = Visibility.Collapsed;

                setCordList();

             
                geoWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(geoWatcher_StatusChanged);
                geoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_PositionChanged);
               // stimulatedGeoWatcher(geoLocation);

                localService.Service1Client journeyClient = new localService.Service1Client();
                journeyClient.GetJourneyAsync(tbxStart.Text, tbxDestination.Text);
                journeyClient.GetJourneyCompleted += new EventHandler<GetJourneyCompletedEventArgs>(journeyClient_GetJourneyCompleted);


                
            }

            if (tbxStart.Text.Equals(""))
            {
                MessageBox.Show("Please enter your starting location");

            }
            else if (tbxDestination.Text.Equals(""))
            {
                MessageBox.Show("Please enter your destination");
            }
            favouriteIndex = false;
            retrieveHistory();
          }
        void retrieveItinerary_retriveItineraryCompleted(object sender, retriveItineraryCompletedEventArgs e)
        {
            int count=1;
            List<localService.Itinerary> iList = e.Result;
            List<ItineraryClass> collections = new List<ItineraryClass>();

            if (iList.Count==0)
            {
                cbxItinary.IsEnabled = false;
                if (cbxItinary.IsChecked == true)
                {
                    cbxItinary.IsChecked = false;
                }
                lbxIternary.Visibility = Visibility.Collapsed;
            }
            else
            {
                cbxItinary.IsEnabled = true;
            }
           
            foreach (localService.Itinerary i in iList)
            {
                collections.Add(new ItineraryClass { Number = count, Name = i.Place, Latitude = i.Latitude, Longitude = i.Longitude });
                Location x = new Location() { Latitude = i.Latitude, Longitude = i.Longitude };
                ItineraryLayer.Children.Add (ppM.createItineraryPushPin(x,count));
                count++;
                
            }
            lbxIternary.ItemsSource = collections;
          
        }
        
        private void setLang()
        {
            if (lan != null)
            {
                tbStart.Text = lan.StartingPt;
                tbDestination.Text = lan.Destination;
                Pana_Journey.Header = lan.Journey;
                Pana_routeplan.Header = lan.Route_Plan;
                btnPlan.Content = lan.Plan;
                btnNGPS.Content = lan.NGPS;
                pana_bus.Header = lan.Bus_Stop;
                tbNextStop.Text = lan.NextStop;
                tbList.Text = lan.ListOfStops;
                btnStimulate.Content = lan.Start;
            }
        }
        
        private void ButtonLocate_Click(object sender, RoutedEventArgs e)
        {
            myMap.SetView(geoLocation, 18);
        }

        private void btnStimulate_Click(object sender, RoutedEventArgs e)
        {

            simulateTimer.Interval = TimeSpan.FromMilliseconds(3000);
            simulateTimer.Tick += new EventHandler(simulateTimer_Tick);
            simulateTimer.Start();
            btnStimulate.Visibility = Visibility.Collapsed;
           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
                       MessageBoxResult res = MessageBox.Show("The system is unable to detect GPS location. Do you want to switch to AR mode?", "AR Mode", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/AR.xaml", UriKind.Relative));
            }
        }
        #endregion

        private void tbxStart_GotFocus(object sender, RoutedEventArgs e)
        {
            editText = true;
        }

        private void tbxDestination_GotFocus(object sender, RoutedEventArgs e)
        {
            editText = true; 
        }

        private void cbxCurrent_Checked(object sender, RoutedEventArgs e)
        {
            WebClient getAddress = new WebClient();
            getAddress.DownloadStringAsync(new Uri("http://dev.virtualearth.net/REST/v1/Locations/" + geoLocation.Latitude + "," + geoLocation.Longitude + "?o=xml&key=Akxg4L1TJmWpzXIsT1jbpBfFF1TEcfrrog2cytX_mCZ8PqrCfHvFx1mJ5D_Wa4Ix"));
            getAddress.DownloadStringCompleted+=new DownloadStringCompletedEventHandler(getAddress_DownloadStringCompleted);

        }

        void getAddress_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string resStr = e.Result;
            XNamespace xn = "http://schemas.microsoft.com/search/local/ws/rest/v1";

            var results = (
           from item in XElement.Parse(resStr).Descendants(xn + "Location")
           select item.Descendants(xn + "Name").First().Value
          
           );


            foreach (var r in results)
            {
                tbxStart.Text = r.ToString();
                startSelectionIndex = false;
            }
        }

       public void clearLayer()
        {

            if (myRouteLayer != null)
            {
                myRouteLayer.Children.Clear();
                busLayer.Children.Clear();
                pinLayer.Children.Clear();
                POILayer.Children.Clear();
                if (itineraryFaveIndex == 0)
                {
                    ItineraryLayer.Children.Clear();
                    myMap.Children.Remove(ItineraryLayer);
                }
                PushpinModel.POIContentLayer.Children.Clear();
                PushpinModel.contentLayer.Children.Clear();
                myMap.Children.Remove(PushpinModel.contentLayer);
                myMap.Children.Remove(PushpinModel.POIContentLayer);
                PushpinModel.contentLayer.Children.Clear();
                PushpinModel.POIContentLayer.Children.Clear();
                GPSLayer.Children.Clear();
                myMap.Children.Remove(GPSLayer);
                myMap.Children.Remove(busLayer);

                btnStimulate.Visibility = Visibility.Visible;
                tbxStop.Text = "";
            }
        }

     

     
       void retrieveHistory()
       {
           favouriteList.Clear();
           lbxFave.ItemsSource = null;
               IsolatedStorageFile favourStorage = IsolatedStorageFile.GetUserStoreForApplication();
               lbxFave.ItemsSource = null;

               if (favourStorage.FileExists("myFavourite.txt"))
               {
                   fileStream = favourStorage.OpenFile("myFavourite.txt", FileMode.Open, FileAccess.Read);
                   StreamReader reader = new StreamReader(fileStream);
                   string s = null;

                   while ((s = reader.ReadLine()) != null)
                   {
                       string[] journeyName = s.Split(',');
                      
                       favouriteList.Add(new Favourite { From = journeyName[0], To = journeyName[1], Name = journeyName[0] + " To " + journeyName[1] });
                   }

                   reader.Close();
                   fileStream.Close();
               }
           
               lbxFave.ItemsSource = favouriteList;
           
       }

       private void RoadPanaroma_Loaded(object sender, RoutedEventArgs e)
       {
           retrieveHistory();
       }

       private void btnClearFave_Click(object sender, RoutedEventArgs e)
       {
            IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForApplication();

            Storage.DeleteFile("myFavourite.txt");
            retrieveHistory();

             
       }

       private void MenuItem_Click(object sender, RoutedEventArgs e)
       {
           string header = (sender as MenuItem).Header.ToString();
           ListBoxItem selectedListBoxItem = this.lbxFave.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
           Favourite content = (Favourite)selectedListBoxItem.Content;

           if (selectedListBoxItem == null)
           {
               return;
           }
           if (header == "Delete")
           {
               
               string check = content.Name;
               

               for (int i = 0; i < favouriteList.Count; i++)
               {
                   if (favouriteList[i].Name.Equals(check))
                   {
                       favouriteList.RemoveAt(i);
                   }
               }

               bool res = false;

              
               Favourite fave=null;

               foreach (Favourite s in favouriteList)
               {
                   fave = s;
                   if (s.Name.Equals(tbxStart.Text + " To " + tbxDestination.Text))
                       res = true;


               }

               if (favouriteList.Count != 0)
               {
                   btnClearFave_Click(sender, e);
                   StreamWriter writer = null;
                   IsolatedStorageFile favourListStorage = IsolatedStorageFile.GetUserStoreForApplication();
                   fileStream = new IsolatedStorageFileStream("myFavourite.txt", FileMode.Append, favourListStorage);

                   writer = new StreamWriter(RoadMode.fileStream);
                   if (res == false )
                   {
                       writer.WriteLine(fave.From + " , " +fave.To);
                   }
                   writer.Close();
               }
               else 
               {
                  
                   btnClearFave_Click(sender, e);
               }

                 retrieveHistory();
           }
           else
           {
              
                   if (favouriteIndex == false)
                   {
                                             
                       string selectedFave = content.Name;
                       itineraryFaveIndex = 1;
                       string[] seperator = new string[] { "To" };
                       string[] wordSplit = selectedFave.Split(seperator, StringSplitOptions.None);

                       int count = 0;

                       foreach (string s in wordSplit)
                       {
                           if (count == 0)
                           {
                               tbxStart.Text = s.Trim();
                               count = 1;

                           }
                           else
                           {
                               tbxDestination.Text = s.Trim();

                           }
                       }
                       favouriteIndex = true;
                       btnPlan_Click(sender, e);
                   }
              
           } 

       }

       private void lbxIternary_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
         
               ItineraryClass fave = (ItineraryClass)lbxIternary.SelectedItem;

               if (fave != null)
               {
                   localService.Service1Client journeyClient = new localService.Service1Client();
                   journeyClient.GetJourneyAsync(currentLocation,fave.Name);
                   journeyClient.GetJourneyCompleted += new EventHandler<GetJourneyCompletedEventArgs>(journeyClient_GetJourneyCompleted);
                   itineraryFaveIndex = 1;
                   clearLayer();

                 
               }
       }

       private void ButtonPOI_Click(object sender, RoutedEventArgs e)
       {
           if (poiIndex == 0)
           {
               gdShow.Visibility = Visibility.Visible;
               poiIndex = 1;
           }
           else
           {
               gdShow.Visibility = Visibility.Collapsed;
               poiIndex = 0;
           }

       }

       private void cbxPOI_Checked(object sender, RoutedEventArgs e)
       {
           POILayer.Visibility = Visibility.Visible;
       }

       private void cbxPOI_Unchecked(object sender, RoutedEventArgs e)
       {
           POILayer.Visibility = Visibility.Collapsed;
       }

       private void cbxItinary_Checked(object sender, RoutedEventArgs e)
       {
           ItineraryLayer.Visibility = Visibility.Visible;
           lbxIternary.Visibility = Visibility.Visible;
           
       }

       private void cbxItinary_Unchecked(object sender, RoutedEventArgs e)
       {
           ItineraryLayer.Visibility = Visibility.Collapsed;
           lbxIternary.Visibility = Visibility.Collapsed;
           
       }



          
        
         


       }

    
     

      
}
        