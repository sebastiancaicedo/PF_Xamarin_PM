using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CalificationsPage : ContentPage
	{
        private Evaluation Evaluation { get; set; }
        private IList<Student> Students { get; set; }

		public CalificationsPage (Evaluation evaluation, IList<Student> students)
		{
            this.Evaluation = evaluation;
            Students = students;
            Title = evaluation.Name;
			InitializeComponent ();
            listviewCalifications.ItemsSource = evaluation.Califications;            
		}

        public async void EditEvaluation (object sender, EventArgs e)
        {
            Rubric rubric = await FirebaseHelper.GetRubricById(Evaluation.RubricKey);
            EvaluationPage page = new EvaluationPage(Evaluation, rubric, Students, true);
            page.FinishActivity += OnFinishEdition;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        private void OnFinishEdition(object sender, ReturnInfo<Evaluation> e)
        {
            listviewCalifications.ItemsSource = null;
            (sender as EvaluationPage).FinishActivity -= OnFinishEdition;
            listviewCalifications.ItemsSource = Evaluation.Califications;
        }
    }
}