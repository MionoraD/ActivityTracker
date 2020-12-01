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
    public partial class ActivityListPage : ContentPage
    {
        // Data used
        List<Activity> ListActivity;
        ManagerPage ManagerP;

        // Data collected
        private static ActivityView CurrentActivity;

        
        public ActivityListPage(ManagerPage parent, List<Activity> activityList)
        {
            InitializeComponent();

            // Set data
            ManagerP = parent;
            ListActivity = activityList;

            // Show title and activity list on page
            App.AddTitle(TitleLayout);
            CreateActivityList();
        }

        public void CreateActivityList()
        {
            // When activities are loaded
            foreach (Activity a in ListActivity)
            {
                // Show on screen
                ActivityView aView = new ActivityView(a);
                aView.AsListView(true);
                aView.OnFormHasFilled += OnStartingActivity;

                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    UpdateForm(aView);
                };
                aView.GestureRecognizers.Add(tapGestureRecognizer);

                Activities.Children.Add(aView);
            }
        }

        private void UpdateForm(ActivityView current)
        {
            if (CurrentActivity != null) CurrentActivity.UntapActivity();

            CurrentActivity = current;

            CurrentActivity.TapActivity();
        }

        public async void OnStartingActivity(object sender, EventArgs e)
        {
            await ManagerP.SetActivity(CurrentActivity.ThisActivity);
            await App.Navigation.PopAsync();
        }

        public async void OnButtonQuitWorkdayClicked(object sender, EventArgs e)
        {
            ManagerP.IsStopWorkday = true;
            await App.Navigation.PopAsync();
        }
    }
}