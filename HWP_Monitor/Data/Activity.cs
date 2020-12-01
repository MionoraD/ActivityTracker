using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using HWP_Monitor.FirebaseConnection;

namespace HWP_Monitor.Data
{
    public class Activity : DBClasses
    {
        // Variables from database
        public int Id { get; private set; } = 0;
        public string Name { get; private set; }
        public string HexColor { get; private set; }
        public string Icon { get; private set; }
        public List<ActivityItem> ItemList { get; private set; } = null;

        // Variables to database
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Activity()
        {
            Id = 0;
            Name = "Recorded Break";
        }

        // Only for generating input items!!!
        public Activity(int id, string name, string hex, string logo, List<ActivityItem> items)
        {
            Id = id;
            Name = name;
            HexColor = hex;
            ItemList = items;
            Icon = logo;
        }

        public Activity(string name, string hex, string logo, List<ActivityItem> items)
        {
            Name = name;
            HexColor = hex;
            ItemList = items;
            Icon = logo;
        }

        public Activity(DBActivity dbActivity)
        {
            Id = dbActivity.ActivityId;
            Name = dbActivity.Name;
            HexColor = dbActivity.HexColor;
            Icon = dbActivity.Logo;
        }

        public async Task SetItemList()
        {
            ItemList = await Firebase_Database.GetAllActivityItemsActivityId(Id);
        }
    }
}
