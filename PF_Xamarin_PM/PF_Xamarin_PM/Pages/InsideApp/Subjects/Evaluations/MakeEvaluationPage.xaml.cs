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

        private Subject Subject { get; set; }
        //private string subjectKey;
        private IList<Student> Students { get; set; }
        private List<Rubric> Rubrics = new List<Rubric>();
        private bool appeared = false;

        public MakeEvaluationPage (Subject subject, IList<Student> students)
		{
            Title = "Nueva Evaluación";
            this.Subject = subject;
            this.Students = students;
            //subjectKey = subject.GetUId();
            InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!appeared)
            {
                await getRubrics();
                appeared = true;
            }
            
        }

        private async Task getRubrics()
        {
            try
            {
                var rubrics = await FirebaseHelper.FirebaseDBClient
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
                await DisplayAlert("Error", "Problema al traer las trubricas, : "+ex, "OK");
                //throw ex;
            }
        }

        public void StartEvaluating(object sender, EventArgs e)
        {
            string name = entryName.Text;
            int rubricIndex = pickerRubrics.SelectedIndex;
            if (!String.IsNullOrEmpty(name) && rubricIndex > -1)
            {
                Evaluation evaluation = new Evaluation(name, Subject.GetUId(), Rubrics[rubricIndex].GetUid());
                Subject.EvaluationsKeys.Add(evaluation.GetUid());
                evaluation.EvaluationSaved += OnEvaluationSaved;
                EvaluationPage page = new EvaluationPage(evaluation, Rubrics[rubricIndex], Students);
                page.FinishActivity += OnFinishEvaluation;
                Navigation.PushModalAsync(new NavigationPage(page));
            }
            else
            {
                DisplayAlert("Error", "Todos los campos debes ser completados", "OK");
            }
        }

        private void OnEvaluationSaved(object sender, EventArgs e)
        {
            try
            {
                Subject.SaveSubjectOnDB();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Problema al guardar la evaluacion, : " + ex, "OK");
                //throw;
            }
        }

        private void OnFinishEvaluation(object sender, ReturnInfo<Evaluation> e)
        {
            FinishActivity(this, new ReturnInfo<Evaluation>(e.Result, e.Data));
            Navigation.PopAsync();
            (sender as EvaluationPage).FinishActivity -= OnFinishEvaluation;
            e.Data.EvaluationSaved -= OnEvaluationSaved;
        }
    }
}