using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using HWP_Monitor.Data;

namespace HWP_Monitor.Views
{
    public class ActivityView : Viewform
    {
        public Activity ThisActivity { get; private set; } = null;

        // Contains the whole activity (+title)
        StackLayout content = new StackLayout();
        // Contains the items that need to be invisible when Activity not active
        StackLayout ItemContent = new StackLayout();

        public ActivityView(Activity a)
        {
            ThisActivity = a;

            BackgroundColor = Color.FromHex(a.HexColor);
            BorderColor = App.firstColor;
            CornerRadius = 30;
            HasShadow = true;
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            WidthRequest = 250;

            // Add title
            StackLayout lytTitle = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            // Add pic to title
            AddPicture(ThisActivity.Icon, lytTitle);

            // Add label to title
            lytTitle.Children.Add(new Label
            {
                Text = ThisActivity.Name,
                Style = (Style)Application.Current.Resources["activityTitle"]
            });

            // Add the title to content
            content.Children.Add(lytTitle);
        }

        public void AsListView(bool isBeforeActivity)
        {
            ItemLayout = new StackLayout();
            StackLayout resultLayout = new StackLayout();

            foreach (ActivityItem item in ThisActivity.ItemList)
            {
                if (isBeforeActivity == item.AskBeforeActivity)
                {
                    CreateActivityItem(item);
                }
                else if (!isBeforeActivity)
                {
                    InputItem input = new InputItem(this, item);
                    StackLayout result = input.AsResult();
                    resultLayout.Children.Add(result);
                }
            }

            if (!isBeforeActivity)
            {
                content.Children.Add(resultLayout);
            }

            ItemContent.Children.Add(ItemLayout);
            if(isBeforeActivity) AddPlayButton(ItemContent);

            ItemContent.IsVisible = !isBeforeActivity;
            content.Children.Add(ItemContent);

            this.Content = content;
        }

        public void AsTimerView()
        {
            ItemLayout = new StackLayout();

            foreach (ActivityItem item in ThisActivity.ItemList)
            {
                if (item.AskBeforeActivity)
                {
                    InputItem input = new InputItem(this, item);
                    StackLayout result = input.AsResult();
                    ItemLayout.Children.Add(result);
                }
            }

            content.Children.Add(ItemLayout);
            this.Content = content;
        }

        public void AsBreakView()
        {
            content.Children.Add(ItemLayout);
            this.Content = content;
        }

        private async void AddPicture(string file, StackLayout parent)
        {
            string imgSource = await FirebaseConnection.Firebase_Storage.GetActivityLogo(file);
            parent.Children.Add(new Image
            {
                Source = imgSource,
                WidthRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand
            });
        }

        private async void AddPlayButton(StackLayout parent)
        {
            // Get img from storage
            string PlayIcon = await FirebaseConnection.Firebase_Storage.GetIcon("Play_Icon.png");

            // Create button
            Frame button = App.CreateButton("Start ", PlayIcon, 40, 100, FormHasFilled);
            button.HorizontalOptions = LayoutOptions.EndAndExpand;

            // If the activity has input
            if (ItemLayout.Children.Count > 0)
            {
                button.IsVisible = false;

                // Handler for when the user can start using the button
                OnAllInputItemsFilled += (s, e) =>
                {
                    button.IsVisible = true;
                };
            }

            // Add button to parent layout
            parent.Children.Add(button);
        }

        public void TapActivity()
        {
            ItemContent.IsVisible = true;
        }
        public void UntapActivity()
        {
            ItemContent.IsVisible = false;
        }
    }
}
