using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HWP_Monitor.FirebaseConnection;
using HWP_Monitor.Models.Login;
using HWP_Monitor.Models.Main;

namespace HWP_Monitor.Models
{
    public partial class MainPage : ContentPage
    {
        LoginPage login = new LoginPage();

        public MainPage()
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            // Setup the list 
            // SetupGeneratedList.AddActivitiesToDatabase();
            ButtonStartWorkday.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            OpenLoginPage();
        }

        private async void OpenLoginPage()
        {
            LayoutProcess.IsVisible = false;
            LayoutLoading.IsVisible = true;
            ButtonStartWorkday.IsVisible = false;

            bool signIn = await login.SignIn();
            if (signIn)
            {   // If signed in find user id
                App.ThisUser = await Firebase_Database.FindUser(login.Token);
                // SetupGeneratedList.AddInputActivitiesToDatabase(); // Needs App.ThisUser

                LayoutProcess.IsVisible = true;
                LayoutLoading.IsVisible = false;

                // Show process to the user
                Span_NrWorkdaysFinished.Text = "" + App.ThisUser.DaysMeasured;
                Span_NrWorkdaysTotal.Text = "" + App.ThisUser.ThisCompany.NeedMeasureDays;

                // Make workdaybtn visible
                ButtonStartWorkday.IsVisible = true;
            }
            else
            {   // If not signed in
                await Task.Delay(100);
                await App.Navigation.PushAsync(login);
            }
        }

        async void OnStartWorkdayTapped(object sender, EventArgs e)
        {
            await App.Navigation.PushAsync(new ManagerPage(App.ThisUser.Id));
        }

        void OnButtonLogoutClicked(object sender, EventArgs e)
        {
            login.SignOut();
            OpenLoginPage();
        }
    }
}
