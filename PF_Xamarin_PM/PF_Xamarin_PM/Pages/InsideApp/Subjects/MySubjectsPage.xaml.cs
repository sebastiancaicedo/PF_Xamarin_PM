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
	public partial class MySubjectsPage : ContentPage
	{
        private IList<Subject> subjects = new ObservableCollection<Subject>();

		public MySubjectsPage ()
		{
            Title = "My Subjects";
			InitializeComponent ();
            BindingContext = subjects;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await getSubjects(LoginPage.LoggedUser.SubjectsKeys);
        }

        private async Task<bool> getSubjects(List<string> subjectsKeys)
        {
            if (subjectsKeys.Count > 0)
            {
                try
                {
                    var items = await FirebaseHelper.firebaseDBClient
                    .Child("subjects")
                    .OnceAsync<Subject>();

                    subjects.Clear();

                    foreach (var item in items)
                    {
                        if (subjectsKeys.Contains(item.Key))
                        {
                            //la asignatura pertenece a el profesor
                            Subject subject = item.Object;
                            subject.SetAuthId(item.Key);
                            subjects.Add(subject);
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
                LoginPage.LoggedUser.AddSubject(e.Data);
                //subjects.Add(e.Data);
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