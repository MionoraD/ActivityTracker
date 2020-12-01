using System;
using System.Collections.Generic;
using System.Text;

namespace HWP_Monitor.FirebaseConnection
{
    public class DBClasses
    {
        public class DBUser
        {
            public int UserId { get; set; }
            public string UserToken { get; set; }
            public string Gender { get; set; }
            public int Age { get; set; }
            public string Function { get; set; }
            public int DaysMeasured { get; set; } = 0;
        }

        public class DBCompany
        {
            public DBCompany()
            {
                CompanyId = 0;
            }
            public int CompanyId { get; set; }
            public string Name { get; set; }
            public string Logo { get; set; }
            public int NeedsDaysMeasured { get; set; } = 5;
        }

        public class DBCompanyUserLink
        {
            public int UserId { get; set; }
            public int CompanyId { get; set; }
        }

        public class DBCompanyActivityLink
        {
            public int CompanyId { get; set; }
            public int ActivityId { get; set; }
        }

        public class DBActivity
        {
            public int ActivityId { get; set; }
            public string Name { get; set; }
            public string HexColor { get; set; }
            public string Logo { get; set; }
        }

        public class DBActivityItems
        {
            public int ActivityItemId { get; set; }
            public int ActivityId { get; set; }
            public string Name { get; set; }
            public bool AskBeforeActivity { get; set; }
            public string Type { get; set; }
        }

        public class DBTextItem
        {
            public int ItemTextId { get; set; }
            public int ActivityItemId { get; set; }
            public string Keyboard { get; set; }
        }

        public class DBItemBool
        {
            public int ItemBoolId { get; set; }
            public int ActivityItemId { get; set; }
            public string OptionOne { get; set; }
            public string OptionTwo { get; set; }
        }

        public class DBItemList
        {
            public int ItemListId { get; set; }
            public int ActivityItemId { get; set; }
            public string Option { get; set; }
        }

        public class DBActivityInput
        {
            public int ActivityInputId { get; set; }
            public int ActivityId { get; set; }
            public int UserId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        public class DBItemInput
        {
            public int ItemInputId { get; set; }
            public int ActivityInputId { get; set; }
            public int ActivityItemId { get; set; }
            public string Input { get; set; }
        }
    }
}
