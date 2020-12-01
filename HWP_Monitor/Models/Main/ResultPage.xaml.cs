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
    public partial class ResultPage : ContentPage
    {
        ResultView vResults;

        List<Activity> ActivityCompanyList;
        List<Activity> ActivityResultList;

        private int NeedDays = 0;
        private int CollectedDays = 0;
        private int MissingDays = 0;

        public ResultPage(List<Activity> compactivitieslist)
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            ActivityCompanyList = compactivitieslist;

            LayoutNotDone.IsVisible = false;
            LayoutDone.IsVisible = false;

            SetNeedDays(App.ThisUser.ThisCompany.NeedMeasureDays);

            LoadResults();
        }

        private async void LoadResults()
        {
            IsLoading(true);
            ActivityResultList = await FirebaseConnection.Firebase_Database.GetResults(ActivityCompanyList);

            if(ActivityResultList.Count == 0)
            {
                SetCollectedDays(0);
                return;
            }

            // Count collected days
            vResults = new ResultView(ActivityResultList);
            SetCollectedDays(vResults.CountDates());

            // Has collected enough for results?
            if (!HasMissingDays()) SetupDone();
            else SetupNotDone();
        }

        private void SetNeedDays(int daysneeded)
        {
            NeedDays = daysneeded;
            Span_DaysNeeded.Text = "" + NeedDays;

            if (CollectedDays == 1 || CollectedDays == -1) Span_DaysNeededWords.Text = "dag";
            else Span_DaysNeededWords.Text = "dagen";
        }
        
        private async void SetCollectedDays(int dayscollected)
        {
            CollectedDays = dayscollected;
            Span_DaysCollected.Text = "" + CollectedDays;

            if (CollectedDays == 1 || CollectedDays == -1) Span_DaysCollectedWords.Text = "dag";
            else Span_DaysCollectedWords.Text = "dagen";

            await App.ThisUser.SetDaysMeasured(CollectedDays);
        }

        private bool HasMissingDays()
        {
            MissingDays = NeedDays - CollectedDays;
            Span_DaysLeftToGo.Text = "" + MissingDays;

            if (MissingDays == 1 || MissingDays == -1) Span_DaysLeftToGoWords.Text = "dag";
            else Span_DaysLeftToGoWords.Text = "dagen";

            if (MissingDays > 0) return true;
            return false;
        }

        private void SetupDone()
        {
            IsLoading(false);
            LayoutDone.IsVisible = true;

            if (!App.ThisUser.HasUserData)
            {
                UserdataView vUserdata = new UserdataView();
                LayoutDone.Children.Add(vUserdata);
                vUserdata.OnFormHasFilled += (s, e) =>
                {
                    LayoutDone.Children.Remove(vUserdata);
                    ShowResult();
                };
            }
            else ShowResult();
        }

        private void ShowResult()
        {
            vResults.ShowResults();
            LayoutDone.Children.Add(vResults);
        }

        private void SetupNotDone()
        {
            IsLoading(false);
            LayoutNotDone.IsVisible = true;
        }

        private void IsLoading(bool loading)
        {
            LytIndicator.IsVisible = loading;
            LytContent.IsVisible = !loading;
        }

        public async void OnReturnToHome(object sender, EventArgs args)
        {
            await App.Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}