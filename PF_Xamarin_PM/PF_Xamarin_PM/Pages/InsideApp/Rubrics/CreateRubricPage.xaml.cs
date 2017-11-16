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
	public partial class CreateRubricPage : ContentPage, IFinishActivity<Rubric>
	{

        public event EventHandler<ReturnInfo<Rubric>> FinishActivity;

        public CreateRubricPage ()
		{
			InitializeComponent ();
		}

        public void Crear(object sender, EventArgs e)
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
        }
    }
}