using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EvaluationPage : ContentPage, IFinishActivity<Evaluation>
	{
        public event EventHandler<ReturnInfo<Evaluation>> FinishActivity;

        private Evaluation Evaluation { get; set; }
        private IList<Student> Students { get; set; }
        private Rubric Rubric { get; set; }

        public EvaluationPage(Evaluation evaluation, Rubric rubric, IList<Student> students, bool editEvaluation = false)
		{
            Title = "Evaluación: "+evaluation.Name;
            this.Evaluation = evaluation;
            this.Students = students;
            this.Rubric = rubric;
            InitializeComponent();
            this.Evaluation.SetToolbarInidicator(labelEvaluationStatus);
            if (!editEvaluation)
            {
                layoutMain.Children.Add(evaluation.SetUp(rubric, students));
            }
            else
            {
                layoutMain.Children.Add(evaluation.SetUpForEdit(rubric));
            }
		}

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Evaluation.CheckIfSavedAtLeastOnce())
                {
                    if (!Evaluation.CheckIfSaved())
                    {
                        var exit = await DisplayAlert("Salir", "Los cambios no se guardarán?", "Salir", "Continuar Evaluación");
                        if (exit)
                        {
                            FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.UnCompleted, Evaluation));
                            await Navigation.PopModalAsync(); // or anything else
                        }
                    }
                    else
                    {
                        if (Evaluation.IsCompleted)
                        {
                            FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Successful, Evaluation));
                            await Navigation.PopModalAsync(); // or anything else
                        }
                        else
                        {
                            var exit = await DisplayAlert("Salir", "La evaluación no está completa, que desea hacer?", "Salir", "Continue Evaluation");
                            if (exit)
                            {
                                FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.UnCompleted, Evaluation));
                                await Navigation.PopModalAsync(); // or anything else
                            }
                        }
                    }
                }
                else
                {
                    var discardAll = await DisplayAlert("Salir", "Quiere descartar esta evaluación?", "Descartar y Salir", "Continuar Evaluación");
                    if (discardAll)
                    {
                        FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Failed, Evaluation));
                        await Navigation.PopModalAsync(); // or anything else
                    }
                }
            });

            return true;
        }               

        public async void SaveEvaluation(object sender, EventArgs e)
        {
            if (!Evaluation.CheckIfSaved())
            {
                Evaluation.SaveEvaluationOnDB();
            }
            if (Evaluation.IsCompleted)
            {
                if (!Evaluation.CheckIfSaved())
                {
                    try
                    {
                        Evaluation.SaveEvaluationOnDB();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Problema al guardar la evaluacion, : "+ex, "OK");
                        //throw;
                    }
                }
                var exit = await DisplayAlert("Save", "Evaluacion Completa, Que desea hacer ahora?", "Salir", "Editar");
                if (exit)
                {
                    FinishActivity(this, new ReturnInfo<Evaluation>(ReturnResult.Successful, Evaluation));
                    await Navigation.PopModalAsync();
                }

            }
        }
    }
}