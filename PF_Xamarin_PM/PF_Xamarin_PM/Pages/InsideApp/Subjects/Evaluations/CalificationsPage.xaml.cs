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
	public partial class CalificationsPage : ContentPage
	{
        Evaluation evaluation;

		public CalificationsPage (Evaluation evaluation, IList<Student> students)
		{
            this.evaluation = evaluation;
            Title = evaluation.Name;
			InitializeComponent ();
            listviewCalifications.ItemsSource = evaluation.Califications;            
		}
	}
}