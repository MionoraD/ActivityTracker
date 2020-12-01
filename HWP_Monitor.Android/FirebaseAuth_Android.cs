using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Forms;
using Android.Gms.Extensions;
using HWP_Monitor.FirebaseConnection;

[assembly: Dependency(typeof(HWP_Monitor.Droid.FirebaseAuth_Android))]
namespace HWP_Monitor.Droid
{
    class FirebaseAuth_Android : IFireAuthentication
    {
        public async Task<string> IsSignIn()
        {
            var user = Firebase.Auth.FirebaseAuth.Instance.CurrentUser;
            if (user == null) return string.Empty;
            GetTokenResult tokenResult = await (user.GetIdToken(true).AsAsync<GetTokenResult>());
            if (tokenResult == null) return string.Empty;
            return tokenResult.Token;
        }

        public bool SignOut()
        {
            try
            {
                Firebase.Auth.FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
            GetTokenResult tokenResult = await (user.User.GetIdToken(true).AsAsync<GetTokenResult>());
            return tokenResult.Token;
        }

        public async Task<string> SignUp(string email, string password)
        {
            var user = await Firebase.Auth.FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
            GetTokenResult tokenResult = await (user.User.GetIdToken(true).AsAsync<GetTokenResult>());
            return tokenResult.Token;
        }
    }
}