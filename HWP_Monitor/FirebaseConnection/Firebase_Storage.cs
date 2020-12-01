using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Storage;
using System.Threading.Tasks;

namespace HWP_Monitor.FirebaseConnection
{
    class Firebase_Storage
    {
        private static FirebaseStorage FirebaseStorage = new FirebaseStorage("hwpmonitor.appspot.com");

        public static async Task<string> GetCompanyLogo(string filestring)
        {
            return await FirebaseStorage
                .Child("CompanyLogos")
                .Child(filestring)
                .GetDownloadUrlAsync();
        }

        public static async Task<string> GetActivityLogo(string filestring)
        {
            return await FirebaseStorage
                .Child("ActivityLogos")
                .Child(filestring)
                .GetDownloadUrlAsync();
        }

        public static async Task<string> GetIcon(string filestring)
        {
            return await FirebaseStorage
                .Child("Icons")
                .Child(filestring)
                .GetDownloadUrlAsync();
        }
    }
}
