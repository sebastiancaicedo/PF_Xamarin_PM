using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyRubricsPage : ContentPage
	{
        public IList<Rubric> rubrics = new ObservableCollection<Rubric>();

		public MyRubricsPage ()
		{
            Title = "My Rubrics";
            BindingContext = rubrics;
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await getRubrics(LoginPage.LoggedUser.RubricsKeys);
        }

        private async Task<bool> getRubrics(List<string> rubricKeys)
        {
            if (rubricKeys.Count > 0)
            {
                try
                {
                    var items = await FirebaseHelper.firebaseDBClient
                    .Child("rubrics")
                    .OnceAsync<Rubric>();

                    rubrics.Clear();

                    foreach (var item in items)
                    {
                        if (rubricKeys.Contains(item.Key))
                        {
                            //la asignatura pertenece a el profesor
                            Rubric rubric = item.Object;
                            rubric.SetUid(item.Key);
                            rubrics.Add(rubric);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return true;
        }

        public void CreateNewRubric(object sender, EventArgs e)
        {
            CreateRubricPage page = new CreateRubricPage();
            page.FinishActivity += OnFinishCreateRubric;
            Navigation.PushAsync(page);
        }

        private void OnFinishCreateRubric(object sender, ReturnInfo<Rubric> e)
        {
            if(e.Result == ReturnResult.Successful)
            {
                LoginPage.LoggedUser.AddRubric(e.Data);
            }
        }
    }
}