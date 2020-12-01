using System;
using System.Threading.Tasks;

namespace HWP_Monitor.FirebaseConnection
{
    public interface IFireAuthentication
    {
        Task<string> IsSignIn();
        bool SignOut();
        Task<string> Login(string email, string password);
        Task<string> SignUp(string email, string password);
    }
}
