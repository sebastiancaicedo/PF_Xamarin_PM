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
	public partial class CreateRubricPage : TabbedPage, IFinishActivity<Rubric>
	{

        public event EventHandler<ReturnInfo<Rubric>> FinishActivity;

        private Rubric rubricToCreate { get; set; }

        public CreateRubricPage ()
		{
            Title = "Crear Rubrica";
			InitializeComponent ();
            entryMinScore.Text = "0";
            entryMaxScore.Text = "5";
            rubricToCreate = new Rubric();
            layoutMain.Children.Add(rubricToCreate.SetUp());
		}

        /*public void Crear(object sender, EventArgs e)
        {
            List<Rubric.RubricCategoryElementLevel> niveles = new List<Rubric.RubricCategoryElementLevel>();
            niveles.Add(new Rubric.RubricCategoryElementLevel("Nivel 1", 5));
            niveles.Add(new Rubric.RubricCategoryElementLevel("Nivel 2", 3));
            niveles.Add(new Rubric.RubricCategoryElementLevel("Nivel 3", 1));
            Rubric.RubricCategoryElement element = new Rubric.RubricCategoryElement("Elemento 1", 100, niveles);
            List<Rubric.RubricCategoryElement> elements = new List<Rubric.RubricCategoryElement>();
            elements.Add(element);
            Rubric.RubricCategory category = new Rubric.RubricCategory("Categoria 1", 100, elements);
            List<Rubric.RubricCategory> categories = new List<Rubric.RubricCategory>();
            categories.Add(category);
            Rubric rubric = new Rubric("Test Rubric", categories);

            FinishActivity(this, new ReturnInfo<Rubric>(ReturnResult.Successful, rubric));
            Navigation.PopAsync();
        }*/
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool exit = await DisplayAlert("Salir", "Desea salir y descartar todo", "Salir y descartar", "Continuar Rubrica");
                if (exit)
                {
                    FinishActivity(this, new ReturnInfo<Rubric>(ReturnResult.UnCompleted, rubricToCreate));
                    await Navigation.PopModalAsync();
                }
            });

            return true;
        }

        public async void SaveRubric(object sender, EventArgs e)
        {
            string rubricName = entryRubricName.Text;
            float minScore;
            float.TryParse(entryMinScore.Text, out minScore);
            float maxScore;
            float.TryParse(entryMaxScore.Text, out maxScore);
            if (!String.IsNullOrEmpty(rubricName))
            {
                if (!String.IsNullOrEmpty(entryMinScore.Text) && !String.IsNullOrEmpty(entryMaxScore.Text))
                {
                    System.Diagnostics.Debug.WriteLine(minScore.ToString());
                    if (minScore >= 0 && maxScore > minScore)
                    {
                        rubricToCreate.MinScore = minScore;
                        rubricToCreate.MaxScore = maxScore;
                        //string error;
                        //if (rubricToCreate.CanBeSaved(out error))
                        //{
                            try
                            {
                                rubricToCreate.Name = rubricName;
                                LoginPage.LoggedUser.AddRubric(rubricToCreate);
                                await DisplayAlert("Completo", "Rubrica creada exitosamente", "Salir");
                                FinishActivity(this, new ReturnInfo<Rubric>(ReturnResult.Successful, rubricToCreate));
                                await Navigation.PopModalAsync();
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Error", "Problema al agregar la asignatura, : " + ex, "OK");
                                //throw;
                            }
                        //}
                        //else
                        //{
                          //  await DisplayAlert("Error", error, "OK");
                        //}
                    }
                    else
                    {
                        await DisplayAlert("Error", "Por favor verifique que el menor valor del rango sea >= 0 y que el mayor valor sea > el menor valor", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Por favor verifique que la rubrica tenga un valor minimo y uno maximo asignado", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Debe asignarle un nombre a a rubrica", "OK");
            }
        }

        public void AddCategory(object sender, EventArgs e)
        {
            Rubric.RubricCategory category = new Rubric.RubricCategory();
            rubricToCreate.AddCategory(category);
        }
    }
}