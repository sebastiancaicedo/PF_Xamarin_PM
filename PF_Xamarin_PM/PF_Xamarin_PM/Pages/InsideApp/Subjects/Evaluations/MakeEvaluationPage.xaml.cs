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
	public partial class MakeEvaluationPage : ContentPage, IFinishActivity<Evaluation>
	{
        public event EventHandler<ReturnInfo<Evaluation>> FinishActivity;

        private Subject subject;
        private string subjectKey;
        private IList<Student> students;
        private List<Rubric> Rubrics = new List<Rubric>();

        public MakeEvaluationPage (string subjectKey, IList<Student> students)
		{
            Title = "New Evaluation";
            this.students = students;
            this.subjectKey = subjectKey;
            InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await getRubrics();
            
        }

        private async Task<bool> getRubrics()
        {
            try
            {
                var rubrics = await FirebaseHelper.firebaseDBClient
                    .Child("rubrics")
                    .OnceAsync<Rubric>();

                foreach (var item in rubrics)
                {
                    Rubric rubric = item.Object;
                    rubric.SetUid(item.Key);
                    Rubrics.Add(rubric);
                    pickerRubrics.Items.Add(rubric.Name);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return true;
        }

        public void StartEvaluating(object sender, EventArgs e)
        {
            string name = entryName.Text;
            int rubricIndex = pickerRubrics.SelectedIndex;
            if (!String.IsNullOrEmpty(name) && rubricIndex > -1)
            {
                Evaluation evaluation = new Evaluation(name, subjectKey, Rubrics[rubricIndex].GetUid());
                EvaluationPage page = new EvaluationPage(evaluation, Rubrics[rubricIndex], students);
                page.FinishActivity += OnFinishEvaluation;
                Navigation.PushModalAsync(new NavigationPage(page));
            }
            else
            {
                DisplayAlert("Error", "All fields must be full", "OK");
            }
        }

        private void OnFinishEvaluation(object sender, ReturnInfo<Evaluation> e)
        {
            if (e.Result == ReturnResult.Successful || e.Result == ReturnResult.UnCompleted)
            {
                FinishActivity(this, new ReturnInfo<Evaluation>(e.Result, e.Data));
            }
            Navigation.PopAsync();
        }
    }
}