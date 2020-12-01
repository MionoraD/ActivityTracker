using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using HWP_Monitor.FirebaseConnection;

namespace HWP_Monitor.Data
{
    public class ActivityItem : DBClasses
    {
        public int Id { get; private set; } = 0;
        public string Name { get; private set; }
        public bool AskBeforeActivity { get; private set; }
        public string Type { get; protected set; } = "none";
        public string Input { get; set; }

        public ActivityItem (string name, bool askBefore)
        {
            Name = name;
            AskBeforeActivity = askBefore;
        }

        public ActivityItem(DBActivityItems dbItem)
        {
            Id = dbItem.ActivityId;
            Name = dbItem.Name;
            AskBeforeActivity = dbItem.AskBeforeActivity;
            Type = dbItem.Type;
        }
    }

    class TextItem : ActivityItem
    {
        public Keyboard board { get; private set; }

        public TextItem(string name, bool askbefore) : base(name, askbefore) 
        {
            Type = "Text";
            board = Keyboard.Default;
        }

        public TextItem(string name, Keyboard keyboard, bool askbefore) : base(name, askbefore)
        {
            Type = "Text";
            board = keyboard;
        }

        public TextItem(DBActivityItems dbItem) : base(dbItem) { }

        public void SetKeyboard(string keyboard)
        {
            if (keyboard.Contains("Chat"))
                board = Keyboard.Chat;
            else if (keyboard.Contains("Email"))
                board = Keyboard.Email;
            else if (keyboard.Contains("Numeric"))
                board = Keyboard.Numeric;
            else if (keyboard.Contains("Plain"))
                board = Keyboard.Plain;
            else if (keyboard.Contains("Telephone"))
                board = Keyboard.Telephone;
            else if (keyboard.Contains("Text"))
                board = Keyboard.Text;
            else if (keyboard.Contains("Url"))
                board = Keyboard.Url;
            else
                board = Keyboard.Default;

            Console.WriteLine(keyboard);
        }
    }

    class BoolItem : ActivityItem
    {
        public string OptionOne { get; private set; }
        public string OptionTwo { get; private set; }

        public BoolItem(string name, bool askbefore, string option1, string option2) : base(name, askbefore) 
        {
            Type = "Bool";

            OptionOne = option1;
            OptionTwo = option2;
        }

        public BoolItem(DBActivityItems dbItem) : base(dbItem) { }

        public void SetOptions(string option1, string option2)
        {
            OptionOne = option1;
            OptionTwo = option2;
        }
    }

    class ListItem : ActivityItem
    {
        public List<string> Options { get; private set; }

        public ListItem(string name, bool askbefore, List<string> optionList) : base(name, askbefore) 
        {
            Type = "List";

            Options = optionList;
        }

        public ListItem(DBActivityItems dbItem) : base(dbItem) { }

        public void SetOptions(List<string> options)
        {
            Options = options;
        }
    }
}
