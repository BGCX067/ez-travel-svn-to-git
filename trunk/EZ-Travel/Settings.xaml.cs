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
using EZ_Travel.Resources.Class;
using System.IO.IsolatedStorage;


namespace EZ_Travel
{
    public partial class Settings : PhoneApplicationPage
    {
        IsolatedStorageSettings iSettings = IsolatedStorageSettings.ApplicationSettings;
        public static Resources.Class.Language lan;
        public List<string> languages = new List<string>() { "Arabic", "Burmese", "Chinese", "Dutch", "English", "French", "German", "Hindi", 
            "Indonesian", "Japanese", "Korean", "Latin", "Malay", "Norwegian", "Portugese", "Russian"};
     
        public Settings()
        {
            InitializeComponent();

            if (iSettings.Contains("language"))
            {
                lan = new Resources.Class.Language(iSettings["language"].ToString());
                setLanguage();
            }

            for (int i = 0; i < languages.Count; i++)
            {
                lbxLanguage.Items.Add(languages[i]);
            }
             
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

            if (lbxLanguage.SelectedIndex!=-1)
            {
            string chosenLang = lbxLanguage.SelectedItem.ToString();

                RoadMode.lan = new Language(chosenLang);
                if (!iSettings.Contains("language"))
                {
                    iSettings.Add("language", chosenLang);
                }
                else
                {
                    iSettings["language"] = chosenLang;
                }

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Choose a language or press the back button");
            }
        }


        private void setLanguage()
        {
            if (lan != null)
            {
                PageTitle.Text = lan.SPageTitle;
                tbLanguage.Text = lan.TLang;
                btnSet.Content = lan.BSet;
                btnReset.Content = lan.ResetGuide;

            }
        }
      
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            iSettings["guide"] = "true";
            MessageBox.Show("Augmented Reality Guide has been reset!");
        }
    }
}