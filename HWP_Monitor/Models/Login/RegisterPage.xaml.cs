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
    public partial class RegisterPage : ContentPage
    {
        Company UserCompany = null;
        List<Company> companies;

        bool HasEmail = false;
        bool HasPass = false;

        public RegisterPage()
        {
            InitializeComponent();
            App.AddTitle(TitleLayout);

            Back.Clicked += async (sender, args) => await App.Navigation.PopAsync();
            Forward.IsVisible = false;
            Forward.Clicked += OnButtonNextClicked;

            SetupCompanyPicker();
        }

        private void CheckInput()
        {
            if (HasEmail && HasPass && UserCompany != null)
            {
                Forward.IsVisible = true;
            }
        }

        private async void SetupCompanyPicker()
        {
            companies = await FirebaseConnection.Firebase_Database.GetAllCompanies();

            CompanyPicker.Items.Add("Gebruik standaard activiteiten lijst");

            List<string> CompanyNames = new List<string>();
            foreach (Company comp in companies)
            {
                CompanyPicker.Items.Add(comp.Name);
            }

            CompanyPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (CompanyPicker.SelectedIndex == -1)
                {
                    UserCompany = null;
                }
                else
                {
                    int index = CompanyPicker.SelectedIndex;
                    if (index == 0)
                    {
                        UserCompany = companies[0];
                    }
                    else UserCompany = companies[index - 1];

                    CheckInput();
                }
            };
        }

        public void EntryEmail_Completed(object sender, EventArgs args)
        {
            // Email cannot be added to the database before
            HasEmail = true;
            CheckInput();
        }

        public void EntryPass_Completed(object sender, EventArgs args)
        {
            // Password has to be longer than 6 characters
            if (PasswordInput.Text.Length < 7) return;
            HasPass = true;
            CheckInput();
        }

        async void OnButtonNextClicked(object sender, EventArgs args)
        {
            // Create user (email, pass)
            FirebaseConnection.IFireAuthentication auth = DependencyService.Get<FirebaseConnection.IFireAuthentication>();
            string Token = await auth.SignUp(EmailInput.Text, PasswordInput.Text);
            if (Token != "")
            {
                // Add user company combination to database (user, company)
                Console.WriteLine(Token);

                await FirebaseConnection.Firebase_Database.AddUserToDatabase(Token, UserCompany.Id);

                await App.Navigation.PopToRootAsync();
            }
            else
            {
                ShowError();
            }
        }

        async private void ShowError()
        {
            await DisplayAlert("Registration Failed", "Something went wrong when registering. Try again!", "OK");
        }
    }
}