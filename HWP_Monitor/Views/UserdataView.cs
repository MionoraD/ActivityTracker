using System;
using System.Collections.Generic;
using System.Text;

using HWP_Monitor.Data;
using Xamarin.Forms;

namespace HWP_Monitor.Views
{
    class UserdataView : Viewform
    {
        List<ActivityItem> UserDataList = new List<ActivityItem>();
        public UserdataView()
        {
            // What is needed?
            UserDataList.Add(new BoolItem("Ben je een man of vrouw?", true, "Vrouw", "Man"));
            UserDataList.Add(new TextItem("Hoe oud ben je?", Keyboard.Telephone, true));
            UserDataList.Add(new TextItem("Welke functie heb je binnen jouw bedrijf?", true));

            ItemLayout = new StackLayout();
            foreach (ActivityItem item in UserDataList)
            {
                CreateActivityItem(item);
            }

            StackLayout content = new StackLayout();
            content.Children.Add(ItemLayout);

            AddPlayButton(content);
            OnFormHasFilled += OnSendInData;

            Content = content;
        }

        private void AddPlayButton(StackLayout parent)
        {
            // Create button
            Frame button = App.CreateButton("Opslaan", "", 40, 100, FormHasFilled);
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

        public async void OnSendInData(object sender, EventArgs e)
        {
            string gender = UserDataList[0].Input;
            int age = 0;
            int.TryParse(UserDataList[1].Input, out age);
            string function = UserDataList[2].Input;

            await App.ThisUser.SetUserData(gender, age, function);
        }
    }
}
