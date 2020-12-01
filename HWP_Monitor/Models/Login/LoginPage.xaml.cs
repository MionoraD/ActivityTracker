using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HWP_Monitor.Data;

namespace HWP_Monitor.Models.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        FirebaseConnection.IFireAuthentication auth;

        public LoginPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<FirebaseConnection.IFireAuthentication>();

            // Title
            App.AddTitle(TitleLayout);
            // Logo Healthy Workplace
            SetCompanyLogo();
            // Register label
            AddRegisterLabelFunction();
        }

        public string Token { get; private set; } = "";

        public async Task<bool> SignIn()
        {
            if (auth != null)
            {
                Token = await auth.IsSignIn();
                App.ThisUser = await FirebaseConnection.Firebase_Database.FindUser(Token);
            }

            if (Token != "") return true;
            else return false;
        }

        public void SignOut()
        {
            auth.SignOut();
        }

        async private void LoginClicked(object sender, EventArgs e)
        {
            string Token = await auth.Login(EmailInput.Text, PasswordInput.Text);
            if (Token != "")
            {
                await App.Navigation.PopToRootAsync();
            }
            else
            {
                ShowError();
            }
        }

        async private void ShowError()
        {
            await DisplayAlert("Authentication Failed", "E-mail or password are incorrect. Try again!", "OK");
        }

        private async void SetCompanyLogo()
        {
            Company comp = await FirebaseConnection.Firebase_Database.FindCompany(1);
            if (comp != null)
            {
                ImgCompanyLogo.Source = comp.Logo;
            }
            else
            {
                ImgCompanyLogo.Source = await (FirebaseConnection.Firebase_Storage.GetCompanyLogo("HealthyWorkplace-Logo.jpg"));
            }
        }

        private void AddRegisterLabelFunction()
        {
            var generateList_tap = new TapGestureRecognizer();
            generateList_tap.Tapped += async (s, e) =>
            {
                await App.Navigation.PushAsync(new RegisterPage());
            };
            LblRegister.GestureRecognizers.Add(generateList_tap);
        }
    }
}