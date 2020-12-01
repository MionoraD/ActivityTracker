using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using HWP_Monitor.Data;

namespace HWP_Monitor.FirebaseConnection
{
    class Firebase_OutsideDatabase : DBClasses
    {
        FirebaseClient Firebase = new FirebaseClient("https://hwpmonitor.firebaseio.com/");

        // Add company to database
        // Will be done by Healthy Workers outside of app
        public async Task AddCompany(Company comp)
        {
            // Find next index
            List<Company> KnownCompanies = await Firebase_Database.GetAllCompanies();
            int id = 0;
            if (KnownCompanies != null) id = KnownCompanies.Count + 1;

            await Firebase
              .Child("Companies")
              .PostAsync(new DBCompany() { 
                  CompanyId = id, 
                  Name = comp.Name, 
                  Logo = comp.Logo,
                  NeedsDaysMeasured = comp.NeedMeasureDays
              });
        }

        public async Task AddCompanyActivityLink(int actId, int compId)
        {
            await Firebase
                .Child("ActivityLists")
                .Child("CompanyActivityLink")
                .PostAsync(new DBCompanyActivityLink() { ActivityId = actId, CompanyId = compId });
        }

        // Done by a Healthy Workplace employee
        public async Task<int> AddActivity(Activity a)
        {
            List<DBActivity> KnownActivities = (await Firebase
              .Child("ActivityLists")
              .Child("Activities")
              .OnceAsync<DBActivity>()).Select(item => new DBActivity
              {
                  ActivityId = item.Object.ActivityId,
                  Name = item.Object.Name,
                  HexColor = item.Object.HexColor,
                  Logo = item.Object.Logo
              }).ToList();

            int id = 0;
            if (KnownActivities != null) id = KnownActivities.Count + 1;

            await Firebase
              .Child("ActivityLists")
              .Child("Activities")
              .PostAsync(new DBActivity() { 
                  ActivityId = id, 
                  Name = a.Name,
                  HexColor = a.HexColor,
                  Logo = a.Icon });

            foreach (ActivityItem aItem in a.ItemList)
            {
                await AddActivityItem(id, aItem);
            }

            return id;
        }

        public async Task AddActivityItem(int parentId, ActivityItem aItem)
        {
            List<DBActivityItems> KnownActivityItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItems")
              .OnceAsync<DBActivityItems>()).Select(item => new DBActivityItems
              {
                  ActivityItemId = item.Object.ActivityItemId,
                  ActivityId = item.Object.ActivityId,
                  Name = item.Object.Name,
                  AskBeforeActivity = item.Object.AskBeforeActivity,
                  Type = item.Object.Type
              }).ToList();

            int id = 0;
            if (KnownActivityItems != null) id = KnownActivityItems.Count + 1;

            await Firebase
              .Child("ActivityLists")
              .Child("ActivityItems")
              .PostAsync(new DBActivityItems()
              {
                  ActivityItemId = id,
                  ActivityId = parentId,
                  Name = aItem.Name,
                  AskBeforeActivity = aItem.AskBeforeActivity,
                  Type = aItem.Type
              });

            if(aItem.Type.Contains("Text"))
            {
                var itemText = aItem as TextItem;
                if (itemText != null) await AddTextItem(id, itemText); 
            }
            else if (aItem.Type.Contains("Bool"))
            {
                var itemBool = aItem as BoolItem;
                if (itemBool != null) await AddBoolItem(id, itemBool);
            }
            else if (aItem.Type.Contains("List"))
            {
                var itemList = aItem as ListItem;
                if (itemList != null) await AddListItems(id, itemList);
            }
        }

        public async Task AddTextItem(int parentId, TextItem aItem)
        {
            List<DBTextItem> KnownTextItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("TextItems")
              .OnceAsync<DBTextItem>()).Select(item => new DBTextItem
              {
                  ItemTextId = item.Object.ItemTextId,
                  ActivityItemId = item.Object.ActivityItemId,
                  Keyboard = item.Object.Keyboard
              }).ToList();

            int id = 0;
            if (KnownTextItems != null) id = KnownTextItems.Count + 1;

            string keyboard = aItem.board.ToString();

            await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("TextItems")
              .PostAsync(new DBTextItem()
              {
                  ItemTextId = id,
                  ActivityItemId = parentId,
                  Keyboard = keyboard
              });
        }

        public async Task AddBoolItem(int parentId, BoolItem aItem)
        {
            List<DBItemBool> KnownBoolItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("BoolItems")
              .OnceAsync<DBItemBool>()).Select(item => new DBItemBool
              {
                  ItemBoolId = item.Object.ItemBoolId,
                  ActivityItemId = item.Object.ActivityItemId,
                  OptionOne = item.Object.OptionOne,
                  OptionTwo = item.Object.OptionTwo
              }).ToList();

            int id = 0;
            if (KnownBoolItems != null) id = KnownBoolItems.Count + 1;

            await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("BoolItems")
              .PostAsync(new DBItemBool()
              {
                  ItemBoolId = id,
                  ActivityItemId = parentId,
                  OptionOne = aItem.OptionOne,
                  OptionTwo = aItem.OptionTwo
              });
        }

        public async Task AddListItems(int parentId, ListItem aItem)
        {
            List<DBItemList> KnownActivityItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("ListItems")
              .OnceAsync<DBItemList>()).Select(item => new DBItemList
              {
                  ItemListId = item.Object.ItemListId,
                  ActivityItemId = item.Object.ActivityItemId,
                  Option = item.Object.Option
              }).ToList();

            int id = 0;
            if (KnownActivityItems != null) id = KnownActivityItems.Count + 1;

            foreach (string option in aItem.Options)
            {
                await Firebase
                  .Child("ActivityLists")
                  .Child("ActivityItemExtras")
                  .Child("ListItems")
                  .PostAsync(new DBItemList()
                  {
                      ItemListId = id,
                      ActivityItemId = parentId,
                      Option = option
                  });

                id++;
            }
        }
    }
}
