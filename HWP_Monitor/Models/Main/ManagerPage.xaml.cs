using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HWP_Monitor.Data;
using HWP_Monitor.Views;

namespace HWP_Monitor.Models.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManagerPage : ContentPage
    {
        private int UserId { get; set; }
        private List<Activity> ActivityList;

        public bool HasDataLoaded = false;
        private Activity CurrentActivity { get; set; }

        private TimerView ActivityTimer { get; set; }

        // Switching between pages
        public bool HasStoppedActivity { get; set; } = false;
        public bool IsStopWorkday { get; set; } = false;
        public async Task SetActivity(Activity newActivity)
        {
            CurrentActivity = newActivity;

            if (CurrentActivity == null) ActivityTimer.Reset();
            else CurrentActivity.StartTime = DateTime.Now;
        }

        public ManagerPage(int idUser)
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            LytTimer.IsVisible = false;
            ActivityTimer = new TimerView(lblTime);

            // Load in data
            UserId = idUser;
            LoadData(UserId);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(IsStopWorkday)
            {   // When the user stops working
                OpenStopWorkdayPage();
            }
            else if (HasStoppedActivity)
            {   // When the user stops doing the activity
                OpenStopActivityPage();
                HasStoppedActivity = false;
            }
            else if (CurrentActivity == null)
            {   // When the current activity is not known
                OpenActivityListPage();
            }
            else
            {   // When activity is known start counting
                StartTimingActivity();
            }
        }

        // Load in the activity list from the database
        public async void LoadData(int userid)
        {
            ActivityList = await FirebaseConnection.Firebase_Database.GetActivitiesFromCompany(App.ThisUser.ThisCompany);
            if (ActivityList == null || ActivityList.Count == 0) 
                HasDataLoaded = false;
            else
            {
                foreach(Activity a in ActivityList)
                {
                    await a.SetItemList();
                }

                // Setup to choose activity after the list is loaded
                HasDataLoaded = true;
                CurrentActivity = null;
                OpenActivityListPage();
            }
        }

        private async void OpenActivityListPage()
        {
            if(HasDataLoaded && CurrentActivity == null)
            {
                await Task.Delay(50); // Not opening page right after Manager is loaded, prevents activitylist from replacing managerpage in stack
                await App.Navigation.PushAsync(new ActivityListPage(this, ActivityList));
            }
            else if (!HasDataLoaded)
            {
                // If data was not loaded send message to user
            }
        }

        private void StartTimingActivity()
        {
            LytCurrentActivity.Children.Clear();
            ActivityView aView = new ActivityView(CurrentActivity);
            aView.AsTimerView();
            LytCurrentActivity.Children.Add(aView);

            LytIndicator.IsVisible = false;
            LytTimer.IsVisible = true;
            
            ActivityTimer.Start();
        }

        async void OnTakeABreak(object sender, EventArgs args)
        {
            ActivityTimer.Stop();

            OnSwitchingPages();
            await App.Navigation.PushAsync(new TakingBreakPage(this, CurrentActivity));
        }

        // For the button on the manager page
        public void OnStopActivity(object sender, EventArgs args)
        {
            OpenStopActivityPage();
        }

        private async void OpenStopActivityPage()
        {
            CurrentActivity.EndTime = DateTime.Now;
            ActivityTimer.Stop();

            OnSwitchingPages();

            bool next = false;
            if (HasStoppedActivity || IsStopWorkday) next = true;
            await App.Navigation.PushAsync(new StopActivityPage(this, CurrentActivity, next));
        }

        private void OnSwitchingPages()
        {
            // Show indicator
            LytTimer.IsVisible = false;
            LytIndicator.IsVisible = true;
        }

        public void OnStopWorkday(object sender, EventArgs args)
        {
            OpenStopWorkdayPage();
        }

        public async void OpenStopWorkdayPage()
        {
            if (CurrentActivity != null)
            {
                IsStopWorkday = true;
                OpenStopActivityPage();
            }
            else
            {
                await App.Navigation.PushAsync(new ResultPage(ActivityList));
            }
        }

        // Overriding software backbutton to not return to last page from here
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}