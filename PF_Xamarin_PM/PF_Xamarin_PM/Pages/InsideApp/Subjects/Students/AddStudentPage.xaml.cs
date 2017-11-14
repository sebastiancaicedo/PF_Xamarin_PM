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
	public partial class AddStudentPage : ContentPage, IFinishActivity<Student>
	{
        private const string STUDENT_DEFAULT_PASSWORD = "estudiante";
        public event EventHandler<ReturnInfo<Student>> FinishActivity;

        public AddStudentPage ()
		{
            Title = "Add Student";
			InitializeComponent ();
		}

        public void AddStudent(object sender, EventArgs e)
        {
            string name = entryName.Text;
            string lastName = entryLastName.Text;
            string email = entryEmail.Text;
            if(!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(lastName) && !String.IsNullOrEmpty(email))
            {
                string possibleError;
                if(verifEmail(email, out possibleError))
                {
                    Firebase.Xamarin.Auth.FirebaseAuth studentAuthInfo;
                    //verificamos que la cuenta no exista
                    try
                    {
                        studentAuthInfo = FirebaseHelper.authProvider.CreateUserWithEmailAndPasswordAsync(email, STUDENT_DEFAULT_PASSWORD).Result;
                        Student student = new Student(name, lastName, studentAuthInfo.User.Email);
                        student.SetKey(studentAuthInfo.User.LocalId);
                        FinishActivity(this, new ReturnInfo<Student>(ReturnResult.Successful, student));
                        Navigation.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Error", "Email, already exists, use other", "OK");
                        //throw;
                    }
                }
                else
                {
                    DisplayAlert("Error", "Email is not valid, error cause: " + possibleError, "OK");
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

    }
}