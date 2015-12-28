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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Collections.Generic;
using EZ_Travel.Resources.Class;
using System.Device.Location;
using System.Xml.Linq;

namespace EZ_Travel.Resources.Model
{
    public class PushpinModel
    {
        string pushpinIcon = "";
        Pushpin myPushpin = null;
        public static MapLayer contentLayer = new MapLayer();
        public static MapLayer POIContentLayer = new MapLayer();

        Pushpin contentPushPin = null;
        
       
        public Pushpin createPushPin(string mode, Location loc)
        {
          
            switch (mode)
            {
                case "Location":
                    pushpinIcon = "Resources/Pushpin/Location.png";

                    break;

                case "POI":
                    pushpinIcon = "Resources/Pushpin/POI.png";
                    break;

            }
            myPushpin = new Pushpin();
            myPushpin.Location = loc;
            ImageBrush image = new ImageBrush();
            myPushpin.Template = null;
            image.ImageSource = new BitmapImage(new Uri(pushpinIcon, UriKind.Relative));
           

            myPushpin.Content = new Ellipse()
            {
                Fill = image,
                Opacity = .8,
                Height = 30,
                Width = 30,
             

            };
                       
            return myPushpin;
        }

                
         public Pushpin createPushPin(string mode,Location loc,int sequence,string name) 
        {
            switch (mode)
            {
                case "Bus":
                    pushpinIcon = "Resources/Pushpin/bus.png";

                    break;

                case "Walk":
                    pushpinIcon = "Resources/Pushpin/walk.png";

                    break;

                case "Train":
                    pushpinIcon = "Resources/Pushpin/train.png";

                    break;

            }

            myPushpin = new Pushpin();
            myPushpin.Location = loc;
            ImageBrush image = new ImageBrush();
            myPushpin.Template = null;
            image.ImageSource = new BitmapImage(new Uri(pushpinIcon, UriKind.Relative));


            myPushpin.Content = new Ellipse()
            {
                Fill = image,
                Opacity = .8,
                Height = 30,
                Width = 30,
                
            };

            contentPushPin = new Pushpin();
            contentPushPin.Location = loc;
            contentPushPin.Content = name;
            contentPushPin.Opacity = 0.8;
            contentLayer.Children.Add(contentPushPin);

            
            return myPushpin;
        }

         public void createPOIPushPin(Location loc, string name)
         {
            
             contentPushPin = new Pushpin();
             contentPushPin.Location = loc;
             contentPushPin.Content = name;
             contentPushPin.Opacity = 0.8;
             POIContentLayer.Children.Add(contentPushPin);
            

             
         }

         public Pushpin createItineraryPushPin(Location loc, int content)
         {

            
             myPushpin = new Pushpin();
             myPushpin.Location = loc;
             ImageBrush image = new ImageBrush();
             myPushpin.Template = null;
             image.ImageSource = new BitmapImage(new Uri( "Resources/Pushpin/Itinerary/"+content+".png", UriKind.Relative));

             myPushpin.Content = new Ellipse()
             {
                 Fill = image,
                 Opacity = .8,
                 Height = 30,
                 Width = 30,


             };

             return myPushpin;

         }
               


        }
    }


