using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SubjectPage : TabbedPage
	{
        private Subject SubjectToShow { get; set; }
        private IList<Student> Students { get; set; } = new ObservableCollection<Student>();
        private IList<Evaluation> Evaluations { get; set; } = new ObservableCollection<Evaluation>();
        private bool appeared = false;
        private bool studsIsRefreshing;
        private bool evalsIsRefreshing;

        public bool StudsIsRefreshing
        {
            get
            {
                return studsIsRefreshing;
            }
            set
            {
                studsIsRefreshing = value;
                OnPropertyChanged(nameof(StudsIsRefreshing));
            }
        }

        public bool EvalsIsRefreshing
        {
            get
            {
                return evalsIsRefreshing;
            }
            set
            {
                evalsIsRefreshing = value;
                OnPropertyChanged(nameof(EvalsIsRefreshing));
            }
        }

        public ICommand StudsRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        SubjectToShow = await FirebaseHelper.GetSubjectById(SubjectToShow.GetUId());
                        listviewStudents.ItemsSource = null;
                        Students = await FirebaseHelper.GetStudentsByIds(SubjectToShow.StudentsKeys);
                        listviewStudents.ItemsSource = Students;
                        StudsIsRefreshing = false;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Problema al traer los estudiantes, : " + ex, "OK");
                        //throw;
                    }
                });
            }
        }

        public ICommand EvalsRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        SubjectToShow = await FirebaseHelper.GetSubjectById(SubjectToShow.GetUId());
                        listviewEvaluations.ItemsSource = null;
                        Evaluations = await FirebaseHelper.GetEvaluationsByIds(SubjectToShow.EvaluationsKeys);
                        listviewEvaluations.ItemsSource = Evaluations;
                        EvalsIsRefreshing = false;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", "Problema al traer las evaluaciones, : " + ex, "OK");
                        //throw;
                    }
                    //LoginPage.LoggedUser = await FirebaseHelper.GetProfessorById(LoginPage.Auth.User.LocalId);
                });
            }
        }

        public SubjectPage(Subject subject)
        {
            this.SubjectToShow = subject;
            Title = "Asignatura: " + subject.Name;
            InitializeComponent();
            BindingContext = this;
            StudsIsRefreshing = SubjectToShow.StudentsKeys.Count > 0 ? true : false;
            EvalsIsRefreshing = SubjectToShow.EvaluationsKeys.Count > 0 ? true : false;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!appeared)
            {
                try
                {
                    Students = await FirebaseHelper.GetStudentsByIds(SubjectToShow.StudentsKeys);
                    listviewStudents.ItemsSource = Students;
                    Evaluations = await FirebaseHelper.GetEvaluationsByIds(SubjectToShow.EvaluationsKeys);
                    listviewEvaluations.ItemsSource = Evaluations;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Problema al traer los estudiantes o las evaluaciones, :" + ex, "OK");
                    //throw;
                }
                EvalsIsRefreshing = false;
                StudsIsRefreshing = false;
                appeared = true;
            }
        }

        public void AddNewStudent(object sender, EventArgs e)
        {
            AddStudentPage page = new AddStudentPage();
            page.FinishActivity += OnFinishAddStudent;
            Navigation.PushAsync(page);
        }

        private void OnFinishAddStudent(object sender, ReturnInfo<Student> e)
        {
            if (e.Result == ReturnResult.Successful)
            {
                try
                {
                    Student student = e.Data;
                    SubjectToShow.AddStudent(student);
                    Students.Add(student);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error al agregar studiante, :" + ex, "OK");
                    //throw;
                }
            }
        }

        public void MakeNewEvaluation(object sender, EventArgs e)
        {
            MakeEvaluationPage page = new MakeEvaluationPage(SubjectToShow, Students);
            page.FinishActivity += OnFinishEvaluation;
            Navigation.PushAsync(page);
        }

        private void OnFinishEvaluation(object sender, ReturnInfo<Evaluation> e)
        {
            listviewEvaluations.ItemsSource = null;
            if (e.Result == ReturnResult.Successful || e.Result == ReturnResult.UnCompleted)
            {
                Evaluations.Add(e.Data);
            }
            else
            {
                SubjectToShow.EvaluationsKeys.Remove(e.Data.GetUid());
            }

            try
            {
                SubjectToShow.SaveSubjectOnDB();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Error al agregar la evaluacion, :" + ex, "OK");
                //throw;
            }
            (sender as MakeEvaluationPage).FinishActivity -= OnFinishEvaluation;
            listviewEvaluations.ItemsSource = Evaluations;
        }

        public void ShowEvaluationInfo(object sender, ItemTappedEventArgs e)
        {
            Evaluation evaluation = e.Item as Evaluation;
            CalificationsPage page = new CalificationsPage(evaluation, Students);
            Navigation.PushAsync(page);
            (sender as ListView).SelectedItem = null;
        }
    }
}