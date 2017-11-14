using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage, IFinishActivity<RegisterPage.ReturnData>
	{

        public event EventHandler<ReturnInfo<ReturnData>> FinishActivity;

        public RegisterPage ()
		{
            Title = "Register";
			InitializeComponent ();
		}

        /// <summary>
        /// Evento del click del boton register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Register(object sender, EventArgs e)
        {
            string name = entryName.Text;
            string lastName = entryLastName.Text;
            string email = entryEmail.Text;
            string password = entryPassword.Text;
            string confirmPass = entryConfirmPassword.Text;

            if(!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(lastName) && !String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password) && !String.IsNullOrEmpty(confirmPass)){
                string possibleError;
                if (verifEmail(email, out possibleError))
                {
                    if (password.Length >= 6)
                    {
                        if (password == confirmPass)
                        {
                            Professor professor = new Professor(name, lastName, email);
                            FinishActivity(this, new ReturnInfo<ReturnData>(ReturnResult.Successful, new ReturnData(professor, password)));
                        }
                        else
                        {
                            DisplayAlert("Error", "Confirmation password doesn't match", "OK");
                        }
                    }
                    else
                    {
                        DisplayAlert("Error", "Password must have 6 or more characters", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", "Email is not valid, error cause: "+possibleError, "OK");
                }
            }
            else
            { 
                DisplayAlert("Error", "All fields must be full", "OK");
            }
        }

        /// <summary>
        /// Verifica si el email ingresado es valido para el registro en la aplicación
        /// </summary>
        /// <param name="email">El email ingresado</param>
        /// <param name="errorCause">la posible causa de la invalidez</param>
        /// <returns></returns>
        private bool verifEmail(string email, out string errorCause)
        {
            if (email.Contains("@uninorte.edu.co"))
            {
                if (email.Last() == 'o')
                {
                    if (email.Split('@').Length == 2)
                    {
                        if (email.Split('.').Length == 3)
                        {
                            errorCause = null;
                            return true;
                        }
                        errorCause = "points '.' are only allowed on domain name";
                    }
                    errorCause = "email not valid";
                }
                errorCause = "email doesn't belong to @uninorte.edu.co domain, must finish in 'o'";
            }
            errorCause = "email doesn't belong to @uninorte.edu.co domain";
            return false;
        }

        public struct ReturnData
        {
            public Professor ProfessorInfo { get; private set; }
            public string Password { get; private set; }

            public ReturnData(Professor professor, string password)
            {
                ProfessorInfo = professor;
                Password = password;
            }
        }
    }
}