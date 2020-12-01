using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using HWP_Monitor.Data;

namespace HWP_Monitor.FirebaseConnection
{
    class Firebase_Database : DBClasses
    {
        static private FirebaseClient Firebase = new FirebaseClient("https://hwpmonitor.firebaseio.com/");

        // This is to list all the companies 
        // The user then chooses the one they are working for
        public static async Task<List<Company>> GetAllCompanies()
        {
            List<DBCompany> dbCompanyList = (await Firebase
              .Child("Companies")
              .OnceAsync<DBCompany>()).Select(item => new DBCompany
              {
                  Name = item.Object.Name,
                  CompanyId = item.Object.CompanyId,
                  Logo = item.Object.Logo
              }).ToList();
            if (dbCompanyList == null || dbCompanyList.Count == 0) return null;

            List<Company> ListCompany = new List<Company>();
            foreach (DBCompany dbCompany in dbCompanyList)
            {
                Company comp = new Company(dbCompany);
                ListCompany.Add(comp);
            }

            return ListCompany;
        }

        // To find the company where the user works
        public static async Task<Company> FindCompany(int id)
        {
            List<Company> companies = await GetAllCompanies();

            foreach (Company comp in companies)
            {
                if (comp.Id == id) return comp;
            }

            return null;
        }

        // Needed to make a connection between user and company
        public static async Task AddUserToDatabase(string token, int idCompany)
        {
            // Get a list of all users
            List<DBUser> KnownUsers = (await Firebase
              .Child("Users")
              .OnceAsync<DBUser>()).Select(item => new DBUser
              {
                  UserId = item.Object.UserId
              }).ToList();

            int id = 0;
            if (KnownUsers != null) id = KnownUsers.Count + 1;

            token = CutTokenString(token);

            // Add the next user
            await Firebase
              .Child("Users")
              .PostAsync(new DBUser() { UserId = id, UserToken = token });

            // Create link user and company
            await Firebase
              .Child("UserCompanyLinks")
              .PostAsync(new DBCompanyUserLink() { UserId = id, CompanyId = idCompany });
        }

        public static async Task<User> FindUser(string token)
        {
            if (token.Equals("")) return null;
            token = CutTokenString(token);

            List<DBUser> KnownUsers = (await Firebase
              .Child("Users")
              .OnceAsync<DBUser>()).Select(item => new DBUser
              {
                  UserId = item.Object.UserId,
                  UserToken = item.Object.UserToken,
                  DaysMeasured = item.Object.DaysMeasured,
                  Gender = item.Object.Gender,
                  Age = item.Object.Age,
                  Function = item.Object.Function
              }).ToList();
            if (KnownUsers == null || KnownUsers.Count == 0) return null;

            foreach (DBUser user in KnownUsers)
            {
                if (user.UserToken.Equals(token))
                {
                    Company company = await FindCompanyFromUser(user);
                    return new User(user, company);
                }
            }

            return null;
        }

        public static async Task UpdateUser()
        {
            var toUpdate = (await Firebase
              .Child("Users")
              .OnceAsync<DBUser>())
              .Where(a=> a.Object.UserId == App.ThisUser.Id)
              .FirstOrDefault();

            await Firebase
              .Child("Users")
              .Child(toUpdate.Key)
              .PutAsync(new DBUser
              {
                  UserId = toUpdate.Object.UserId,
                  UserToken = toUpdate.Object.UserToken,
                  DaysMeasured = App.ThisUser.DaysMeasured,
                  Gender = App.ThisUser.Gender,
                  Age = App.ThisUser.Age,
                  Function = App.ThisUser.Function
              });
        }

        // Token is too big for the database, 
        // This function makes sure that it is cut at the same spot every time
        private static string CutTokenString(string token)
        {
            return token.Substring(0, 26);
        }

        // Needed to find the connection between company and user
        private static async Task<Company> FindCompanyFromUser(DBUser user)
        {
            List<DBCompanyUserLink> links = (await Firebase
              .Child("UserCompanyLinks")
              .OnceAsync<DBCompanyUserLink>()).Select(item => new DBCompanyUserLink
              {
                  UserId = item.Object.UserId,
                  CompanyId = item.Object.CompanyId
              }).ToList();

            if (links.Count == 0) return null;
            foreach (DBCompanyUserLink link in links)
            {
                if (link.UserId == user.UserId) return await FindCompany(link.CompanyId);
            }
            return null;
        }

        public static async Task<List<Activity>> GetActivitiesFromCompany(Company company)
        {
            // Find all company/activity links
            List<DBCompanyActivityLink> companyactivitylinks = (await Firebase
              .Child("ActivityLists")
              .Child("CompanyActivityLink")
              .OnceAsync<DBCompanyActivityLink>()).Select(item => new DBCompanyActivityLink
              {
                  ActivityId = item.Object.ActivityId,
                  CompanyId = item.Object.CompanyId
              }).ToList();
            if (companyactivitylinks.Count == 0) return null;

            // Find all the activities
            List<DBActivity> activitylist = (await Firebase
              .Child("ActivityLists")
              .Child("Activities")
              .OnceAsync<DBActivity>()).Select(item => new DBActivity
              {
                  ActivityId = item.Object.ActivityId,
                  Name = item.Object.Name,
                  HexColor = item.Object.HexColor,
                  Logo = item.Object.Logo
              }).ToList();
            if (activitylist.Count == 0) return null;

            // Search for the activities that match with one of the links
            List<Activity> list = new List<Activity>();
            foreach (DBActivity dbA in activitylist)
            {
                foreach (DBCompanyActivityLink link in companyactivitylinks)
                {
                    if (link.CompanyId == company.Id && link.ActivityId == dbA.ActivityId)
                    {
                        list.Add(new Activity(dbA));
                    }
                }
            }

            return list;
        }

        private static async Task<List<DBActivityItems>> GetAllActivityItems()
        {
            return (await Firebase
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
        }

        public static async Task<List<ActivityItem>> GetAllActivityItemsActivityId(int activityId)
        {
            List<DBActivityItems> allActivityItems = await GetAllActivityItems();
            if (allActivityItems.Count == 0) return null;

            List<ActivityItem> ItemList = new List<ActivityItem>();
            foreach (DBActivityItems dbItem in allActivityItems)
            {
                if (dbItem.ActivityId == activityId)
                {
                    if (dbItem.Type.Contains("Text"))
                    {
                        TextItem aItem = new TextItem(dbItem);
                        await GetTextOption(dbItem.ActivityItemId, aItem);
                        ItemList.Add(aItem);
                    }
                    else if (dbItem.Type.Contains("Bool"))
                    {
                        BoolItem aItem = new BoolItem(dbItem);
                        await GetBoolOptions(dbItem.ActivityItemId, aItem);
                        ItemList.Add(aItem);
                    }
                    else if (dbItem.Type.Contains("List"))
                    {
                        ListItem aItem = new ListItem(dbItem);
                        await GetListOptions(dbItem.ActivityItemId, aItem);
                        ItemList.Add(aItem);
                    }
                    else
                    {
                        ActivityItem aItem = new ActivityItem(dbItem);
                        ItemList.Add(aItem);
                    }
                }
            }

            return ItemList;
        }

        public static async Task GetTextOption(int aItemId, TextItem tItem)
        {
            List<DBTextItem> allTextItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("TextItems")
              .OnceAsync<DBTextItem>()).Select(item => new DBTextItem
              {
                  ItemTextId = item.Object.ItemTextId,
                  ActivityItemId = item.Object.ActivityItemId,
                  Keyboard = item.Object.Keyboard
              }).ToList();
            if (allTextItems.Count == 0)
            {
                return;
            }

            foreach (DBTextItem item in allTextItems)
            {
                if (aItemId == item.ActivityItemId)
                {
                    tItem.SetKeyboard(item.Keyboard);
                    return;
                }
            }
        }

        private static async Task GetBoolOptions(int aItemId, BoolItem bItem)
        {
            List<DBItemBool> allBoolItems = (await Firebase
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
            if (allBoolItems.Count == 0)
            {
                bItem.SetOptions("False", "True");
            }

            foreach (DBItemBool item in allBoolItems)
            {
                if (aItemId == item.ActivityItemId)
                {
                    bItem.SetOptions(item.OptionOne, item.OptionTwo);
                    return;
                }
            }
        }

        private static async Task GetListOptions(int aItemId, ListItem lItem)
        {
            List<DBItemList> allBoolItems = (await Firebase
              .Child("ActivityLists")
              .Child("ActivityItemExtras")
              .Child("ListItems")
              .OnceAsync<DBItemList>()).Select(item => new DBItemList
              {
                  ItemListId = item.Object.ItemListId,
                  ActivityItemId = item.Object.ActivityItemId,
                  Option = item.Object.Option
              }).ToList();
            if (allBoolItems.Count == 0)
            {
                lItem.SetOptions(null);
            }

            List<string> newList = new List<string>();
            foreach (DBItemList item in allBoolItems)
            {
                if (aItemId == item.ActivityItemId)
                {
                    newList.Add(item.Option);
                }
            }

            lItem.SetOptions(newList);
        }

        private static string GetFolder()
        {
            return "User:" + App.ThisUser.UserToken;
        }

        private static async Task<List<DBActivityInput>> GetKnownInputList()
        {
            string folder = GetFolder();

            return (await Firebase
              .Child("Input")
              .Child(folder)
              .Child("ActivityInput")
              .OnceAsync<DBActivityInput>()).Select(item => new DBActivityInput
              {
                  ActivityInputId = item.Object.ActivityInputId,
                  ActivityId = item.Object.ActivityId,
                  UserId = item.Object.UserId,
                  Start = item.Object.Start,
                  End = item.Object.End
              }).ToList();
        }

        public static async Task AddActivityInput(Activity a)
        {
            string folder = GetFolder();

            List<DBActivityInput> KnownInput = await GetKnownInputList();

            int id = 0;
            if (KnownInput != null) id = KnownInput.Count + 1;

            await Firebase
              .Child("Input")
              .Child(folder)
              .Child("ActivityInput")
              .PostAsync(new DBActivityInput()
              {
                  ActivityInputId = id,
                  ActivityId = a.Id,
                  UserId = App.ThisUser.Id,
                  Start = a.StartTime,
                  End = a.EndTime
              });

            if (a.ItemList == null || a.ItemList.Count < 1) return;
            foreach (ActivityItem item in a.ItemList)
            {
                await AddItemInput(id, item);
            }
        }

        private static async Task<List<DBItemInput>> GetKnownInputItemList()
        {
            string folder = GetFolder();

            return (await Firebase
              .Child("Input")
              .Child(folder)
              .Child("ActivityItemInput")
              .OnceAsync<DBItemInput>()).Select(item => new DBItemInput
              {
                  ItemInputId = item.Object.ItemInputId,
                  ActivityItemId = item.Object.ActivityItemId,
                  ActivityInputId = item.Object.ActivityInputId,
                  Input = item.Object.Input
              }).ToList();
        }

        public static async Task AddItemInput(int ActivityInputId, ActivityItem aItem)
        {
            string folder = GetFolder();

            List<DBItemInput> KnownInput = await GetKnownInputItemList();

            int id = 0;
            if (KnownInput != null) id = KnownInput.Count + 1;

            await Firebase
              .Child("Input")
              .Child(folder)
              .Child("ActivityItemInput")
              .PostAsync(new DBItemInput()
              {
                  ItemInputId = aItem.Id,
                  ActivityInputId = ActivityInputId,
                  ActivityItemId = aItem.Id,
                  Input = aItem.Input
              });
        }

        public static async Task<List<Activity>> GetResults(List<Activity> ActivitiesList)
        {
            string folder = GetFolder();
            List<DBActivityInput> KnownInput = await GetKnownInputList();
            List<DBItemInput> KnownInputItems = await GetKnownInputItemList();

            if (KnownInput == null || KnownInput.Count < 0) return null;

            List<Activity> listResults = new List<Activity>();
            foreach(DBActivityInput input in KnownInput)
            {
                // Find the activity
                Activity a = FindActivity(input.ActivityId, ActivitiesList);
                if(a != null)
                {
                    Activity result = new Activity(a.Name, a.HexColor, a.Icon, a.ItemList);
                    result.StartTime = input.Start;
                    result.EndTime = input.End;

                    // find input of each item in itemlist
                    foreach (ActivityItem item in result.ItemList)
                    {
                        DBItemInput itemresult = FindItem(input.ActivityInputId, item.Id, KnownInputItems);
                        if(itemresult != null)
                        {
                            item.Input = itemresult.Input;
                        }
                    }

                    listResults.Add(result);
                }
            }
            return listResults;
        }

        private static Activity FindActivity(int inputId, List<Activity> activitylist)
        {
            if (activitylist == null || activitylist.Count < 0) return null;
            foreach (Activity a in activitylist)
            {
                if (a.Id == inputId)
                {
                    return a;
                }
            }
            return null;
        }

        private static DBItemInput FindItem(int inputId, int itemid, List<DBItemInput> listInput)
        {
            if (listInput == null || listInput.Count < 0) return null;
            foreach (DBItemInput item in listInput)
            {
                if(item.ActivityInputId == inputId && item.ActivityItemId == itemid)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
