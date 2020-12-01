using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HWP_Monitor.FirebaseConnection;

namespace HWP_Monitor.Data
{
    public class User : DBClasses
    {
        public int Id { get; private set; }
        public string UserToken { get; private set; }
        public int DaysMeasured { get; private set; }

        public string Gender { get; private set; } = "";
        public int Age { get; private set; } = 0;
        public string Function { get; private set; } = "";

        public Company ThisCompany { get; set; }

        public bool HasUserData
        {
            get
            {
                if (Gender == null || Gender.Equals("")) return false;
                if (Age < 1) return false;
                if (Function == null || Function.Equals("")) return false;
                return true;
            }
        }

        public User(int id, string token, int measuredays, Company company)
        {
            Id = id;
            UserToken = token;
            DaysMeasured = measuredays;
            ThisCompany = company;
        }

        public User(DBUser user, Company company)
        {
            Id = user.UserId;
            UserToken = user.UserToken;
            Gender = user.Gender;
            Age = user.Age;
            Function = user.Function;
            DaysMeasured = user.DaysMeasured;
            ThisCompany = company;
        }

        public async Task SetDaysMeasured(int days)
        {
            DaysMeasured = days;
            await FirebaseConnection.Firebase_Database.UpdateUser();
        }

        public async Task SetUserData(string gender, int age, string function)
        {
            Gender = gender;
            Age = age;
            Function = function;
            await FirebaseConnection.Firebase_Database.UpdateUser();
        }
    }
}
