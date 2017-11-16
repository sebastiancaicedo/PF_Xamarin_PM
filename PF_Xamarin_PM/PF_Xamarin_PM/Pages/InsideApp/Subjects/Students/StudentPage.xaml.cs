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
	public partial class StudentPage : ContentPage
	{
        private Student StudentToShow { get; set; }

		public StudentPage (Student student)
		{
            StudentToShow = student;
            Title = student.Name;
            BindingContext = StudentToShow;
			InitializeComponent ();
		}
	}
}