using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyRubricsPage : ContentPage
	{
        private IList<Rubric> Rubrics { get; set; } = new ObservableCollection<Rubric>();
        private bool appeared = false;
        private bool isRefreshing;

        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        LoginPage.LoggedUser = await FirebaseHelper.GetProfessorById(LoginPage.Auth.User.LocalId);
                        listviewRubrics.ItemsSource = null;
                        Rubrics = await FirebaseHelper.GetRubricsByIds(LoginPage.LoggedUser.RubricsKeys);
                        listviewRubrics.ItemsSource = Rubrics;
                        IsRefreshing = false;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Problema al traer las rubricas, :" + ex, "OK");
                        //throw;
                    }                    
                });
            }
        }

        public MyRubricsPage ()
		{
            Title = "Mis Rubricas";
            BindingContext = this;
			InitializeComponent ();
            IsRefreshing = LoginPage.LoggedUser.RubricsKeys.Count > 0 ? true : false;
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!appeared)
            {
                try
                {
                    Rubrics = await FirebaseHelper.GetRubricsByIds(LoginPage.LoggedUser.RubricsKeys);
                    listviewRubrics.ItemsSource = Rubrics;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "No se pudo traer las rubricas, : " + ex, "OK");
                    //throw;
                }
                appeared = true;
                IsRefreshing = false;
            }
        }

        public void CreateNewRubric(object sender, EventArgs e)
        {
            CreateRubricPage page = new CreateRubricPage();
            page.FinishActivity += OnFinishCreateRubric;
            Navigation.PushModalAsync(new NavigationPage(page));
        }

        private void OnFinishCreateRubric(object sender, ReturnInfo<Rubric> e)
        {
            if(e.Result == ReturnResult.Successful)
            {
                Rubrics.Add(e.Data);
            }
        }

        public void ShowRubricInfo(object sender, ItemTappedEventArgs e)
        {
            Rubric rubric = e.Item as Rubric;
            RubricPage page = new RubricPage(rubric);
            Navigation.PushAsync(page);
            (sender as ListView).SelectedItem = null;
        }
    }
}