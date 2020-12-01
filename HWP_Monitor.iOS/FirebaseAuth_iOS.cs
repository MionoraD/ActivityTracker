using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Auth;
using HWP_Monitor.FirebaseConnection;

[assembly: Dependency(typeof(HWP_Monitor.iOS.FirebaseAuth_iOS))]
namespace HWP_Monitor.iOS
{
    class FirebaseAuth_iOS : IFireAuthentication
    {
        public async Task<string> IsSignIn()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            if (user == null) return string.Empty;
            return await user.GetIdTokenAsync();
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
            return await user.User.GetIdTokenAsync();
        }

        public bool SignOut()
        {
            try
            {
                _ = Auth.DefaultInstance.SignOut(out Foundation.NSError error);
                return error == null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> SignUp(string email, string password)
        {
            var user = await Firebase.Auth.Auth.DefaultInstance.CreateUserAsync(email, password);
            return await user.User.GetIdTokenAsync();
        }
    }
}