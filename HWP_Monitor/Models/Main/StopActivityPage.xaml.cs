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
    public partial class StopActivityPage : ContentPage
    {
        ManagerPage ManagerP;
        Activity ThisActivity;

        public StopActivityPage(ManagerPage parent, Activity a, bool NextIsKnown)
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            ManagerP = parent;
            ThisActivity = a;

            LytIndicator.IsVisible = false;

            ActivityView aView = new ActivityView(ThisActivity);
            aView.AsListView(false);

            if (aView.ItemLayout.Children.Count < 1)
            {
                ButtonToQuit.IsVisible = true;
            }
            else
            {
                aView.OnAllInputItemsFilled += (s, e) =>
                {
                    ButtonToQuit.IsVisible = true;
                };
            }
            
            LytActivity.Children.Add(aView);
        }

        async void OnSendActivityToQuit(object sender, EventArgs args)
        {
            LayoutActivityForm.IsVisible = false;
            LytIndicator.IsVisible = true;

            await FirebaseConnection.Firebase_Database.AddActivityInput(ThisActivity);
            await ManagerP.SetActivity(null);

            await App.Navigation.PopAsync();
        }
    }
}