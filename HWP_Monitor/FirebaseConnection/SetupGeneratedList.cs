using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using HWP_Monitor.Data;

namespace HWP_Monitor.FirebaseConnection
{
    class SetupGeneratedList
    {
        public static List<Company> SetupCompanies()
        {
            List<Company> companies = new List<Company>();

            companies.Add(new Company("Healthy Workplace", "HealthyWorkplace-Logo.jpg", 5));

            return companies;
        }

        private static List<Activity> GenericActivityList()
        {
            List<Activity> newActivities = new List<Activity>();

            List<Color> colors = GetGradients(App.thirdColor, App.secondColor, 5);

            // Individual work activity
            List<ActivityItem> FormIndividual = new List<ActivityItem>();
            FormIndividual.Add(new BoolItem("Concentratie nodig", true, "Laag", "Hoog"));
            FormIndividual.Add(new BoolItem("Hoeveel concentratie had je?", false, "Laag", "Hoog"));
            List<string> locations = new List<string>();
            locations.Add("Zp09");
            locations.Add("Zp11");
            locations.Add("Thuis");
            locations.Add("Anders");
            //FormIndividual.Add(new ListItem("Locatie", true, locations));
            List<String> spots = new List<String>();
            spots.Add("Open Bureau");
            spots.Add("Afgesloten Burea");
            spots.Add("Anders");
            //FormIndividual.Add(new ListItem("Plek", true, spots));

            newActivities.Add(new Activity("Individueel werk", colors[0].ToHex(), "WorkingLaptop.png", FormIndividual));

            // Meeting activity
            List<ActivityItem> FormMeeting = new List<ActivityItem>();
            FormMeeting.Add(new BoolItem("Concentratie nodig", true, "Laag", "Hoog"));
            FormMeeting.Add(new BoolItem("Hoeveel concentratie had je?", false, "Laag", "Hoog"));
            FormMeeting.Add(new BoolItem("Hoe kun je elkaar zien in het overleg?", true, "Online", "Face-to-face"));
            FormMeeting.Add(new BoolItem("Stond het overleg in the agenda?", true, "Gepland", "Ongepland"));
            FormMeeting.Add(new TextItem("Hoeveel personen?", Keyboard.Telephone, true));
            //FormMeeting.Add(new ListItem("Locatie", true, locations));
            List<String> meetingspots = new List<String>();
            meetingspots.Add("Vergaderruimte");
            meetingspots.Add("Vergaderplek");
            meetingspots.Add("Ontmoetingsplek");
            meetingspots.Add("aanlandplek");
            meetingspots.Add("Restaurant");
            meetingspots.Add("Thuis");
            meetingspots.Add("Anders");
            //FormMeeting.Add(new ListItem("Plek", true, meetingspots));

            newActivities.Add(new Activity("Overleg", colors[1].ToHex(), "WorkingLaptop.png", FormMeeting));

            List<ActivityItem> FormBreak = new List<ActivityItem>();
            newActivities.Add(new Activity("Pauze", colors[2].ToHex(), "WorkingLaptop.png", FormBreak));

            List<ActivityItem> FormOther = new List<ActivityItem>();
            FormOther.Add(new TextItem("Naam", true));
            FormOther.Add(new BoolItem("Ik denk dat ik dit vaker ga doen", true, "Ja", "Nee"));
            //FormOther.Add(new ListItem("Locatie", true, locations));
            //FormOther.Add(new ListItem("Plek", true, meetingspots));

            newActivities.Add(new Activity("Anders", colors[3].ToHex(), "WorkingLaptop.png", FormOther));

            return newActivities;
        }

        public async static Task<List<Activity>> GenerateFilledList()
        {
            List<Activity> filledlist = new List<Activity>();

            List<Activity> standardlist = await FirebaseConnection.Firebase_Database.GetActivitiesFromCompany(App.ThisUser.ThisCompany);
            Random rand = new Random();
            DateTime today = DateTime.Today.Date;

            for (int i=0; i<=5; i++)
            {
                DateTime day = today.AddDays(i).Date;
                day = day.AddHours(10);
                Console.WriteLine("Day " + i + " " + day.TimeOfDay);
                foreach(Activity a in standardlist)
                {
                    Activity aNew = new Activity(a.Id, a.Name, a.HexColor, a.Icon, a.ItemList);

                    aNew.StartTime = day;
                    day = day.AddMinutes(rand.Next(15, 180));
                    aNew.EndTime = day;

                    filledlist.Add(aNew);
                }
            }
            return filledlist;
        }

        public static async void AddActivitiesToDatabase()
        {
            int CompanyId = 0;

            List<Company> standardCompanyList = SetupCompanies();
            List<Activity> standardActivities = GenericActivityList();

            FirebaseConnection.Firebase_OutsideDatabase outside = new Firebase_OutsideDatabase();

            foreach (Company comp in standardCompanyList)
            {
                //await outside.AddCompany(comp);
            }

            foreach (Activity a in standardActivities)
            {
                int id = await outside.AddActivity(a);
                await outside.AddCompanyActivityLink(id, CompanyId);
            }
        }

        public static async void AddInputActivitiesToDatabase()
        {
            Console.WriteLine("Reached");
            List<Activity> inputlist = await GenerateFilledList();
            foreach(Activity input in inputlist)
            {
                await FirebaseConnection.Firebase_Database.AddActivityInput(input);
            }
        }

        public static List<Color> GetGradients(Color start, Color end, int steps)
        {
            if (steps <= 0) return null;
            double stepA = ((end.A - start.A) / (steps - 1));
            double stepR = ((end.R - start.R) / (steps - 1));
            double stepG = ((end.G - start.G) / (steps - 1));
            double stepB = ((end.B - start.B) / (steps - 1));

            List<Color> colors = new List<Color>();
            for (int i = 0; i < steps; i++)
            {
                colors.Add(Color.FromRgba(start.R + (stepR * i),
                                            start.G + (stepG * i),
                                            start.B + (stepB * i),
                                            start.A + (stepA * i)));
            }
            return colors;
        }
    }
}
