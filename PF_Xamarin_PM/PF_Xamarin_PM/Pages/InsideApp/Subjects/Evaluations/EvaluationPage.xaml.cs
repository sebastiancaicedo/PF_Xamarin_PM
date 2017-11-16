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
	public partial class EvaluationPage : ContentPage, IFinishActivity<Evaluation>
	{
        public event EventHandler<ReturnInfo<Evaluation>> FinishActivity;

        private Evaluation evaluation;
        private IList<Student> students;
        private Rubric rubric;

        public EvaluationPage (Evaluation evaluation, Rubric rubric, IList<Student> students)
		{
            Title = "Evaluation: "+evaluation.Name;
            this.evaluation = evaluation;
            this.students = students;
            this.rubric = rubric;
			InitializeComponent ();
            this.evaluation.SetToolbarInidicator(labelEvaluationStatus);
            layoutMain.Children.Add(evaluation.SetUp(rubric, students));
		}

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!evaluation.CheckIfSaved())
                {
                    var exit = await DisplayAlert("Exit", "Changes won't be save, What will you do?", "Exit", "Continue Evaluation");
                    if (exit)
                    {
                        FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Failed, null));
                        await Navigation.PopModalAsync(); // or anything else
                    }
                }
                else
                {
                    if (evaluation.IsCompleted)
                    {
                        FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Successful, evaluation));
                        await Navigation.PopModalAsync(); // or anything else
                    }
                    else
                    {
                        FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.UnCompleted, evaluation));
                        await Navigation.PopModalAsync(); // or anything else
                    }
                }
            });

            return true;
        }               

        public async void SaveEvaluation(object sender, EventArgs e)
        {
            if (!evaluation.CheckIfSaved())
            {
                evaluation.SaveEvaluationOnDB();
            }
            if (evaluation.IsCompleted)
            {
                if (!evaluation.CheckIfSaved())
                {
                    evaluation.SaveEvaluationOnDB();
                }
                var exit = await DisplayAlert("Save", "Evaluation completed, What will you do?", "Exit", "Edit something");
                if (exit)
                {
                    FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Successful, evaluation));
                    await Navigation.PopModalAsync();
                }

            }
        }
    }
}