using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HWP_Monitor.Models;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using HWP_Monitor.Data;

namespace HWP_Monitor
{
    public partial class App : Application
    {
        public static User ThisUser { get; set; }

        public static INavigation Navigation { get; private set; }

        public static Color firstColor = Color.White;
        public static Color secondColor = Color.FromHex("#950149");
        public static Color thirdColor = Color.FromHex("#01888B");

        public App()
        {
            InitializeComponent();
            Xamarin.Forms.Device.SetFlags(new string[] { "Brush_Experimental" });

            MainPage = GetMainPage();

            BindingContext = this;
        }

        private static Page GetMainPage()
        {
            var rootPage = new NavigationPage(new MainPage());
            Navigation = rootPage.Navigation;
            return rootPage;
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=ac8b2b34-6d2c-4116-af0a-ff37df3d93b9;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static void AddTitle(StackLayout titleLayout)
        {
            Label mainTitleLabel = new Label
            {
                Text = "Work Activity Tracker",
                Style = (Style)Application.Current.Resources["title"]
            };
            titleLayout.Children.Add(mainTitleLabel);

            Label subTitleLabel = new Label
            {
                Text = DateTime.Now.ToString("D"),
                Style = (Style)Application.Current.Resources["subtitle"]
            };
            titleLayout.Children.Add(subTitleLabel);
        }

        public static Frame CreateButton(string btnName, string imgUrl, int height, int width, EventHandler pressButton)
        {
            Frame frameButton = new Frame
            {
                Style = (Style)Application.Current.Resources["buttonFrame"],
                HeightRequest = height,
                WidthRequest = width
            };

            StackLayout framecontent = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            if(btnName != null || !btnName.Equals(""))
            {
                Label lblButton = new Label
                {
                    Text = btnName,
                    TextColor = App.firstColor,
                    FontSize = (height/5*2.5),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                framecontent.Children.Add(lblButton);
            }

            if(imgUrl != null || !imgUrl.Equals(""))
            {
                Image imgButton = new Image
                {
                    Source = imgUrl,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HeightRequest = (height/5*4),
                    Margin = new Thickness(0, 0, 0, 0)
                };
                framecontent.Children.Add(imgButton);
            }

            frameButton.Content = framecontent;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += pressButton;
            frameButton.GestureRecognizers.Add(tapGestureRecognizer);

            return frameButton;
        }
    }
}
