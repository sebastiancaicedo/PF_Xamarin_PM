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
	public partial class CreateSubjectPage : ContentPage, IFinishActivity<Subject>
	{
        public event EventHandler<ReturnInfo<Subject>> FinishActivity;

        public CreateSubjectPage ()
		{
            Title = "Create new Subject";
			InitializeComponent ();
		}

        public void AddSubject(object sender, EventArgs e)
        {
            //se agrega la asignatura al usuario y se guarda en db
            string name = entryName.Text;
            if (!String.IsNullOrEmpty(name))
            {
                Subject subject = new Subject(name, LoginPage.auth.User.LocalId);
                FinishActivity(this, new ReturnInfo<Subject>(ReturnResult.Successful, subject));
                Navigation.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "All fields must be full", "OK");
            }
        }

    }
}