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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Notification;
using System.IO;
using System.Collections.ObjectModel;
namespace EZ_Travel
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings iSettings = IsolatedStorageSettings.ApplicationSettings;
        public static Resources.Class.Language lan;
        Guid deviceID;
        PushService.Service1Client pushClient = new PushService.Service1Client();
        HttpNotificationChannel pushChannel;
        IsolatedStorageFileStream fileStream = null;
    
        SessionService.Service1Client sessionClient = null;

        public MainPage()
        {
            InitializeComponent();
            if (iSettings.Contains("language"))
            {
                lan = new Resources.Class.Language(iSettings["language"].ToString());
                setLanguage();
            }

            getSubscription();
        }

        private void btnJP_Click(object sender, RoutedEventArgs e)
        {
           NavigationService.Navigate(new Uri("/RoadMode.xaml", UriKind.Relative));
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));  
        }

        private void setLanguage()
        {
            if (lan != null)
            {
                btnJP.Content = lan.Journey_Planner;
                btnSettings.Content = lan.SPageTitle;
                RoadMode.lan = lan;
            }
        }
        void getSubscription()
        {
            
            if (IsolatedStorageSettings.ApplicationSettings.Contains("DeviceId"))
            {
                
                deviceID = (Guid)IsolatedStorageSettings.ApplicationSettings["DeviceId"];
            }
            else
            {
                
                deviceID = Guid.NewGuid();
                IsolatedStorageSettings.ApplicationSettings["DeviceId"] = deviceID;
            }
            pushChannel = HttpNotificationChannel.Find("myChannel");

            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel("myChannel");
                attachHandlerFunctions();
                pushChannel.Open();
               
            }
            else
            {
                attachHandlerFunctions();
                
                pushClient.SubscribeMyPhoneAsync(deviceID, pushChannel.ChannelUri.ToString());
               
            }
        }
          void attachHandlerFunctions()
        {
     

        pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(pushChannel_ErrorOccurred);

        pushClient.SubscribeMyPhoneCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(pushClient_SubscribeMyPhoneCompleted);

        pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(pushChannel_ChannelUriUpdated);
       
        pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(pushChannel_HttpNotificationReceived);
        }

        void pushChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                StreamReader myReader = new StreamReader(e.Notification.Body);
                bool lineExist = false;

                StreamWriter writer = null;
                IsolatedStorageFile favourListStorage = IsolatedStorageFile.GetUserStoreForApplication();
                string readerText = myReader.ReadToEnd();
               

                if (favourListStorage.FileExists("myFavourite.txt"))
                {
                    fileStream = favourListStorage.OpenFile("myFavourite.txt", FileMode.Open, FileAccess.ReadWrite);
                    StreamReader reader = new StreamReader(fileStream);
                    string s = null;
                    
                    while ((s = reader.ReadLine()) != null)
                    {                        
                        if (readerText.Equals(s))
                        {
                            lineExist = true;
                        }
                    }
                    fileStream.Close();
                   reader.Close();
                }
               
               
                fileStream = new IsolatedStorageFileStream("myFavourite.txt", FileMode.Append, favourListStorage);
               
                if (lineExist == false)
                {

                    writer = new StreamWriter(fileStream);
                    writer.WriteLine(readerText);
                    writer.Close();
                                    

                }

                fileStream.Close();
            });
           
        }
    

        void pushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            
            if (pushChannel.IsShellTileBound == false)
            {
                var ListOfAllowedDomains = new Collection<Uri>
                {

                new Uri("http://46.4.195.237/EZTravelPushWCF/Service1.svc")
             
                };

                pushChannel.BindToShellTile(ListOfAllowedDomains);
            }
         
            if (pushChannel.IsShellToastBound == false)
            {
                pushChannel.BindToShellToast();
            }
           
             pushClient.SubscribeMyPhoneAsync(deviceID, e.ChannelUri.ToString());
        }

        void pushClient_SubscribeMyPhoneCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                sessionClient = new SessionService.Service1Client();
                sessionClient.checkExistsAsync(deviceID.ToString(), pushChannel.ChannelUri.ToString());
                RoadMode.deviceID = deviceID.ToString();
                sessionClient.checkExistsCompleted+=new EventHandler<SessionService.checkExistsCompletedEventArgs>(sessionClient_checkExistsCompleted);

             
            }
        
        }

        void pushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
     
            MessageBox.Show(e.Message);
        }

        void sessionClient_setDeviceChannelCompleted(object sender, SessionService.setDeviceChannelCompletedEventArgs e)
        {
            if (e.Result == true)
            {
              
            }
        }

        void sessionClient_checkExistsCompleted(object sender, SessionService.checkExistsCompletedEventArgs e)
        {

            if (e.Result == false)
            {
                sessionClient = new SessionService.Service1Client();
                sessionClient.setDeviceChannelAsync(deviceID.ToString(), pushChannel.ChannelUri.ToString());
                sessionClient.setDeviceChannelCompleted += new EventHandler<SessionService.setDeviceChannelCompletedEventArgs>(sessionClient_setDeviceChannelCompleted);
            }
          
        }
        
     }
    
}