using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MySubjectsPage : ContentPage
	{
        private IList<Subject> Subjects { get; set; } = new ObservableCollection<Subject>();
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
                        listviewMySubjects.ItemsSource = null;
                        Subjects = await FirebaseHelper.GetSubjectsByIds(LoginPage.LoggedUser.SubjectsKeys);
                        listviewMySubjects.ItemsSource = Subjects;
                        IsRefreshing = false;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Problema al traer las asignaturas, : " + ex, "OK");
                        //throw;
                    }
                });
            }
        }


        public MySubjectsPage ()
		{
            Title = "Mis Asignaturas";
			InitializeComponent ();
            BindingContext = this;
            IsRefreshing = LoginPage.LoggedUser.SubjectsKeys.Count > 0 ? true : false;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!appeared)
            {
                try
                {
                    Subjects = await FirebaseHelper.GetSubjectsByIds(LoginPage.LoggedUser.SubjectsKeys);
                    listviewMySubjects.ItemsSource = Subjects;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Problema al traer las asignaturas de firebase: "+ex, "OK");
                    //throw;
                }
                IsRefreshing = false;
                appeared = true;
                //await getSubjects(LoginPage.LoggedUser.SubjectsKeys);
            }
        }

        public void CreateNewSubject(object sender, EventArgs e)
        {
            CreateSubjectPage page = new CreateSubjectPage();
            page.FinishActivity += OnCreateSubjectFinish;
            Navigation.PushAsync(page);
        }

        private void OnCreateSubjectFinish(object sender, ReturnInfo<Subject> e)
        {
            if (e.Result == ReturnResult.Successful)
            {
                try
                {
                    LoginPage.LoggedUser.AddSubject(e.Data);
                    Subjects.Add(e.Data);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Problema con agregar asignatura : " + ex, "OK");
                    //throw;
                }
            }
        }

        public void ShowSubjectInfo(object sender, ItemTappedEventArgs e)
        {
            Subject subject = e.Item as Subject;
            SubjectPage page = new SubjectPage(subject);
            Navigation.PushAsync(page);
            (sender as ListView).SelectedItem = null;
        }
    }
}