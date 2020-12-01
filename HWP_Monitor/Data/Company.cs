using System;
using System.Collections.Generic;
using System.Text;
using HWP_Monitor.FirebaseConnection;

namespace HWP_Monitor.Data 
{
    public class Company : DBClasses
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Logo { get; private set; }

        public int NeedMeasureDays { get; private set; }

        public Company(string name, string logo, int measuredays)
        {
            Name = name;
            Logo = logo;
            NeedMeasureDays = measuredays;
        }

        public Company(DBCompany comp)
        {
            Id = comp.CompanyId;
            Name = comp.Name;
            NeedMeasureDays = comp.NeedsDaysMeasured;

            FindLogo(comp.Logo);
        }

        public async void FindLogo(string logo)
        {
            Logo = await (Firebase_Storage.GetCompanyLogo(logo));
        }
    }
}
