using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using HWP_Monitor.Data;

namespace HWP_Monitor.Views
{
    class ResultView : StackLayout
    {
        List<Activity> ResultList;
        public List<DateTime> Dates { get; private set; }
        DateTime EarliestHour, LatestHour;

        private int FullHeight { 
            get {
                return GraphHeight + 50;
            } 
        }
        public int GraphHeight { get; set; } = 300;
        private TimeSpan TimeDisplayed { get; set; }

        public ResultView(List<Activity> results)
        {
            ResultList = results;
            GetDates();
        }

        public void ShowResults()
        {
            Margin = new Thickness(0, 0, 0, 50);

            FindFirstLastTime(out EarliestHour, out LatestHour);

            // Graph Title
            Children.Add(new Label { Text = "Results" });

            StackLayout graph = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = FullHeight
            };

            // Set timespan
            AbsoluteLayout lytHours = new AbsoluteLayout();

            TimeDisplayed = LatestHour.TimeOfDay - EarliestHour.TimeOfDay;

            // Add hours to graph
            double totalhours = TimeDisplayed.TotalHours;
            double spacebetweenhours = GraphHeight / totalhours;
            for (int i = 1; i <= totalhours; i++)
            {
                DateTime current = EarliestHour.AddHours(i);

                Label lblHour = new Label { 
                    Text = current.ToString("t"),
                    Margin = new Thickness(5)
                };
                AbsoluteLayout.SetLayoutFlags(lblHour,
                        AbsoluteLayoutFlags.None);
                AbsoluteLayout.SetLayoutBounds(lblHour,
                    new Rectangle(0f, (GraphHeight - (spacebetweenhours * i)), 50, spacebetweenhours));
                lytHours.Children.Add(lblHour);
            }
            graph.Children.Add(lytHours);

            // YAx
            StackLayout lytAxes = new StackLayout();
            BoxView yAx = new BoxView
            {
                Color = App.secondColor,
                WidthRequest = 5,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            lytAxes.Children.Add(yAx);
            graph.Children.Add(lytAxes);

            // Scrollview
            ScrollView scrollCollections = new ScrollView();
            scrollCollections.Orientation = ScrollOrientation.Horizontal;
            // Collections
            StackLayout lytCollections = new StackLayout();
            lytCollections.Orientation = StackOrientation.Horizontal;
            foreach (DateTime day in Dates)
            {
                lytCollections.Children.Add(CreateDayLayout(day));
            }
            scrollCollections.Content = lytCollections;
            graph.Children.Add(scrollCollections);

            Children.Add(graph);
        }

        private void GetDates()
        {
            Dates = new List<DateTime>();
            foreach (Activity result in ResultList)
            {
                // The first date
                if (Dates.Count < 1) Dates.Add(result.StartTime);
                else
                {
                    // Check if date is already in list
                    bool HasDate = false;
                    foreach (DateTime date in Dates)
                    {
                        if (date.Date == result.StartTime.Date) HasDate = true;
                    }

                    // If not in list add to list
                    if (!HasDate) Dates.Add(result.StartTime);
                }
            }
            foreach(DateTime date in Dates)
            {
                Console.WriteLine(date.Date);
            }
        }

        public int CountDates()
        {
            if (Dates == null) return 0;
            else return Dates.Count;
        }

        private void FindFirstLastTime(out DateTime first, out DateTime last)
        {
            first = ResultList[0].StartTime;
            last = ResultList[0].EndTime;
            foreach (Activity result in ResultList)
            {
                if (first.TimeOfDay > result.StartTime.TimeOfDay) first = result.StartTime;
                if (last.TimeOfDay < result.EndTime.TimeOfDay) last = result.EndTime;
            }

            first = new DateTime(first.Year, first.Month, first.Day, first.Hour, 0, 0);
            last = new DateTime(last.Year, last.Month, last.Day, last.Hour + 1, 0, 0);
        }

        private List<Activity> GetActivitiesFromDate(DateTime day)
        {
            List<Activity> activities = new List<Activity>();
            foreach (Activity result in ResultList)
            {
                if (result.StartTime.Date == day.Date) activities.Add(result);
            }
            return activities;
        }

        private StackLayout CreateDayLayout(DateTime day)
        {
            // Create layout with results
            AbsoluteLayout lytResults = CreateDayResultLayout(day);

            // Set Ax with date
            StackLayout lytAx = new StackLayout();
            BoxView ax = new BoxView
            {
                Color = App.secondColor,
                HeightRequest = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            lytAx.Children.Add(ax);

            Label lblDay = new Label
            {
                Text = day.ToString("d")
            };
            lytAx.Children.Add(lblDay);

            // Create day layout
            StackLayout lytDay = new StackLayout();
            lytDay.Children.Add(lytResults);
            lytDay.Children.Add(lytAx);
            return lytDay;
        }

        private AbsoluteLayout CreateDayResultLayout(DateTime day)
        {
            AbsoluteLayout lytResults = new AbsoluteLayout()
            {
                HeightRequest = GraphHeight
            };

            List<Activity> DayResults = GetActivitiesFromDate(day);
            foreach (Activity a in DayResults)
            {
                BoxView aBox = new BoxView();
                aBox.Color = Color.FromHex(a.HexColor);

                // Find y position
                TimeSpan startspan = LatestHour.TimeOfDay - a.StartTime.TimeOfDay;
                double posY = (GraphHeight / TimeDisplayed.TotalMinutes * startspan.TotalMinutes)-50;

                // Find length
                TimeSpan lengthspan = a.EndTime.TimeOfDay - a.StartTime.TimeOfDay;
                double length = GraphHeight / TimeDisplayed.TotalMinutes * lengthspan.TotalMinutes;

                AbsoluteLayout.SetLayoutFlags(aBox, AbsoluteLayoutFlags.None);
                AbsoluteLayout.SetLayoutBounds(aBox, new Rectangle(0f, posY, 75f, length));

                lytResults.Children.Add(aBox);
            }
            return lytResults;
        }
    }
}
