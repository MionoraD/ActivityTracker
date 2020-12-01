using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using HWP_Monitor.Data;

namespace HWP_Monitor.Views
{
    class InputItem : StackLayout
    {
        protected Viewform ThisView { get; private set; }
        protected ActivityItem ThisItem { get; private set; }
        public bool Filled { get; protected set; } = false;

        protected void SetInput(string value)
        {
            if (value == null) Filled = false;
            else if (value.Equals("")) Filled = false;
            else
            {
                if (ThisItem != null) ThisItem.Input = value;
                Filled = true;
                if (ThisView != null) ThisView.CheckAllItemsFilled(ThisItem.AskBeforeActivity);
            }
        }

        public InputItem(Viewform view, ActivityItem item)
        {
            ThisView = view;
            ThisItem = item;
        }

        public StackLayout AsResult()
        {
            StackLayout layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            Label labelName = new Label {
                FontAttributes = FontAttributes.Bold,
                Text = ThisItem.Name,
                WidthRequest = 150
            };
            layout.Children.Add(labelName);

            Label labelResult = new Label { Text = ThisItem.Input };
            layout.Children.Add(labelResult);

            return layout;
        }
    }

    class EntryInput : InputItem
    {
        private Keyboard limit;

        public EntryInput(Viewform view, TextItem item) : base(view, item) 
        {
            limit = item.board;

            CreateEntry();
        }

        private void CreateEntry()
        {
            Entry ItemEntry = new Entry { Placeholder = ThisItem.Name };
            if (limit != null) ItemEntry.Keyboard = limit;
            if (ThisItem.Name.Contains("Pass")) ItemEntry.IsPassword = true;
            ItemEntry.Completed += OnEntryCompleted;
            ItemEntry.TextChanged += OnEntryCompleted;
            this.Children.Add(ItemEntry);
        }

        void OnEntryCompleted(object sender, EventArgs args)
        {
            SetInput(((Entry)sender).Text);
        }
    }

    class SwitchInput : InputItem
    {
        public SwitchInput(Viewform view, BoolItem item) : base(view, item) 
        {
            Style = (Style)Application.Current.Resources["SwitchLayout"];

            // Add the switch title
            if (ThisItem.Name != null && !ThisItem.Name.Equals(""))
            {
                Label labelTitle = new Label
                {
                    Text = ThisItem.Name,
                    Style = (Style)Application.Current.Resources["SwitchTitle"]
                };
                this.Children.Add(labelTitle);
            }

            // Create Layout for the switch buttons
            StackLayout switchButtons = new StackLayout
            {
                Style = (Style)Application.Current.Resources["SwitchButtons"]
            };

            // Create buttons
            Label labelOption1;
            Frame fOne = CreateButton(item.OptionOne, out labelOption1);

            Label labelOption2;
            Frame fTwo = CreateButton(item.OptionTwo, out labelOption2);

            // Make the buttons tappable
            AddFunctionToButton(fOne, labelOption1, fTwo, labelOption2, item.OptionOne);
            AddFunctionToButton(fTwo, labelOption2, fOne, labelOption1, item.OptionTwo);

            switchButtons.Children.Add(fOne);
            switchButtons.Children.Add(fTwo);

            this.Children.Add(switchButtons);
        }

        private Frame CreateButton(string option, out Label lblOption)
        {
            Frame frame = new Frame
            {
                Style = (Style)Application.Current.Resources["SwitchFrame"]
            };
            lblOption = new Label
            {
                Text = option,
                Style = (Style)Application.Current.Resources["SwitchLabel"]
            };
            frame.Content = lblOption;

            return frame;
        }

        private void AddFunctionToButton(Frame fSelected, Label lblSelected, Frame fUnselected, Label lblUnselected, string option)
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                SetInput(option);

                // Select the button
                fSelected.BackgroundColor = App.secondColor;
                lblSelected.TextColor = App.firstColor;

                // Unselect the other button
                fUnselected.BackgroundColor = App.firstColor;
                lblUnselected.TextColor = App.secondColor;
            };
            fSelected.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }

    class PickerInput : InputItem
    {
        public PickerInput(Viewform view, ListItem item) : base(view, item) 
        {
            Picker picker = new Picker
            {
                Title = ThisItem.Name,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Use boolitem instead of ActivityItem
            foreach (string option in item.Options)
            {
                picker.Items.Add(option);
            }

            picker.SelectedIndexChanged += (sender, args) =>
            {
                if (picker.SelectedIndex == -1)
                {
                    SetInput(null);
                }
                else
                {
                    SetInput(picker.Items[picker.SelectedIndex]);
                }
            };

            this.Children.Add(picker);
        }
    }
}
