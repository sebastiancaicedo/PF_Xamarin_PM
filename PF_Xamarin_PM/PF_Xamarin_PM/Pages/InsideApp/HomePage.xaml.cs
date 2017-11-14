using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace PF_Xamarin_PM
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : MasterDetailPage
	{
		public HomePage ()
		{
            //ellabel.Text = LoginPage.auth.User.FederatedId;
			InitializeComponent ();
            Detail = new NavigationPage(new MySubjectsPage());
            IsPresented = false;
		}
            

        public void ShowMySubjects(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MySubjectsPage());
            IsPresented = false;
        }

        public void ShowMyRubrics(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MyRubricsPage());
            IsPresented = false;
        }
	}
}