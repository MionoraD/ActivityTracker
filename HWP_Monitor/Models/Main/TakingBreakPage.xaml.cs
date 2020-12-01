using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HWP_Monitor.Views;
using HWP_Monitor.Data;

namespace HWP_Monitor.Models.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TakingBreakPage : ContentPage
    {
        private ManagerPage ManagerP;
        // private TimerView BreakTimer;

        private Activity BreakActivity;

        public TakingBreakPage(ManagerPage mPage, Activity currentactivity)
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            AddIcon(LayoutIcon);

            ManagerP = mPage;

            BreakActivity = new Activity();
            BreakActivity.StartTime = DateTime.Now;

            Label_CurrentActivity.Text = currentactivity.Name;

            //BreakTimer = new TimerView(lblTime);
            //BreakTimer.Start();
        }

        private async void AddIcon(StackLayout parent)
        {
            string imgSource = await FirebaseConnection.Firebase_Storage.GetIcon("002-TakingABreakvs2.png");
            parent.Children.Add(new Image
            {
                Source = imgSource,
                WidthRequest = 200
            });
        }

        async void OnReturnToWork(object sender, EventArgs args)
        {
            await SaveActivity();
            await App.Navigation.PopAsync();
        }

        async void OnStopActivity(object sender, EventArgs args)
        {
            await SaveActivity();
            ManagerP.HasStoppedActivity = true;
            await App.Navigation.PopAsync();
        }

        async void OnStopWorkday(object sender, EventArgs args)
        {
            await SaveActivity();
            ManagerP.IsStopWorkday = true;
            await App.Navigation.PopAsync();
        }

        private async Task SaveActivity()
        {
            LytBreak.IsVisible = false;
            LytIndicator.IsVisible = true;

            BreakActivity.EndTime = DateTime.Now;
            await FirebaseConnection.Firebase_Database.AddActivityInput(BreakActivity);
        }
    }
}