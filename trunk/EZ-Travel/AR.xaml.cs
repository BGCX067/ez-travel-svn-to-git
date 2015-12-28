using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using SLARToolKit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Linq;
using EZ_Travel.localService;
using EZ_Travel;
using System.IO.IsolatedStorage;

namespace SLARWP7
{

    public partial class MainPage : PhoneApplicationPage
    {

        private VideoCamera camera;//requires videocamera to take over camera function
        private List<Marker> markerList;
        private BitmapMarkerDetector bmdList;
        private List<DBMarker> dbmList;
        Service1Client wcfService;
        IsolatedStorageSettings iSettings = IsolatedStorageSettings.ApplicationSettings;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            wcfService = new Service1Client();
            wcfService.retrieveMarkerInfoCompleted +=new EventHandler<retrieveMarkerInfoCompletedEventArgs>(wcfService_retrieveMarkerInfoCompleted);
            InitializeARDetector();
            InitializedCamera();
            wcfService.retrieveMarkerInfoAsync(RoadMode.journeyID.ToString());
            if (iSettings.Contains("guide"))
            {
                if (iSettings["guide"].ToString().Equals("true"))
                {
                    popupInfo.IsOpen = true;
                    iSettings["guide"] = false;
                }
            }
        }
      
        private void InitializedCamera()
        {
            camera = new VideoCamera();
            camera.Initialized += OnCameraInitialized;
            cameraVisualizer.SetSource(camera);
        }

        void OnCameraInitialized(object sender, EventArgs e)
        {
            camera.Initialized -= OnCameraInitialized;
            Dispatcher.BeginInvoke(() => { GrabFrame(); });
            
        }
        private void InitializeARDetector()
        {
            //  Initialize the Detector

            // Load the marker pattern. It has 16x16 segments and a width of 80 millimeters
            //creating a marker list
            markerList = new List<Marker>();
            for (int i = 0; i <= 2; i++)
            {
                Marker marker = new Marker();
                string url = "Resources/Marker/marker" + i + ".pat";
                marker = Marker.LoadFromResource(url, 16, 16, 80);
                markerList.Add(marker);
            }
            // The perspective projection has the near plane at 1 and the far plane at 4000

            bmdList = new BitmapMarkerDetector();
            bmdList.Initialize(640, 480, 1, 4000, markerList);

        }


        private void GrabFrame()
        {

            if (camera != null)
            {
                WriteableBitmap wb = new WriteableBitmap(640, 480);

                camera.GetCurrentFrame(wb);

                wb.Invalidate();


                var sw = new Stopwatch();
                sw.Start();

                var results = bmdList.DetectAllMarkers(wb);

                if (results.HasResults)
                {
                    string direction = displayAR(results);

                    if (direction == "Left")
                    {
                        rightarrow.Visibility = Visibility.Collapsed;
                        okay.Visibility = Visibility.Collapsed;
                        wrong.Visibility = Visibility.Collapsed;
                        leftarrow.Visibility = Visibility.Visible;
                    }

                    else if (direction == "Right")
                    {
                        leftarrow.Visibility = Visibility.Collapsed;
                        okay.Visibility = Visibility.Collapsed;
                        wrong.Visibility = Visibility.Collapsed;
                        rightarrow.Visibility = Visibility.Visible;
                    }

                    else if (direction == "Okay")
                    {
                        rightarrow.Visibility = Visibility.Collapsed;
                        leftarrow.Visibility = Visibility.Collapsed;
                        wrong.Visibility = Visibility.Collapsed;
                        okay.Visibility = Visibility.Visible;
                    }

                    else if (direction == "Wrong")
                    {
                        rightarrow.Visibility = Visibility.Collapsed;
                        leftarrow.Visibility = Visibility.Collapsed;
                        okay.Visibility = Visibility.Collapsed;
                        wrong.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    rightarrow.Visibility = Visibility.Collapsed;
                    leftarrow.Visibility = Visibility.Collapsed;
                    okay.Visibility = Visibility.Collapsed;
                    wrong.Visibility = Visibility.Collapsed;
                }
                sw.Stop();
            }
            Dispatcher.BeginInvoke(() => { GrabFrame();  });
        }

        private string displayAR(DetectionResults detectedResults)
        {
            string result = "";
            for (int i = 0; i < markerList.Count; i++)
            {
                if (detectedResults.Where(r => r.Marker == markerList[i]).FirstOrDefault() != null)
                {
                    result = dbmList[i].Direction;
                }
            }
            return result;
        }

        void wcfService_retrieveMarkerInfoCompleted(object sender,retrieveMarkerInfoCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                dbmList = new List<DBMarker>();
                dbmList = e.Result;
            }
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            camera = null;
            cameraVisualizer.SetSource(null);
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            popupInfo.IsOpen = false;
        }

      

    }
}    