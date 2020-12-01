using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

using HWP_Monitor.Data;

namespace HWP_Monitor.Views
{
    public class Viewform : Frame
    {
        public StackLayout ItemLayout = new StackLayout();

        public event EventHandler OnFormHasFilled;
        public event EventHandler OnAllInputItemsFilled;

        // For this to work only InputItems can be added to ItemLayout!!!
        public void CheckAllItemsFilled(bool beforeActivity)
        {
            foreach (InputItem item in ItemLayout.Children)
            {
                if (!item.Filled) return;
            }

            // If all InputItems are Filled
            if (OnAllInputItemsFilled != null)
            {
                OnAllInputItemsFilled(this, EventArgs.Empty);
            }
        }

        public void FormHasFilled(object s, EventArgs a)
        {
            if (OnFormHasFilled != null)
            {
                OnFormHasFilled(this, EventArgs.Empty);
            }
        }

        protected void CreateActivityItem(ActivityItem item)
        {
            if (item.Type.Contains("Text"))
            {
                var txtInput = item as TextItem;
                ItemLayout.Children.Add(new EntryInput(this, txtInput));
            }
            else if (item.Type.Contains("Bool"))
            {
                var bInput = item as BoolItem;
                ItemLayout.Children.Add(new SwitchInput(this, bInput));
            }
            else if (item.Type.Contains("List"))
            {
                var listInput = item as ListItem;
                ItemLayout.Children.Add(new PickerInput(this, listInput));
            }
            else
            {
                ItemLayout.Children.Add(new InputItem(this, item));
            }
        }
    }
}
