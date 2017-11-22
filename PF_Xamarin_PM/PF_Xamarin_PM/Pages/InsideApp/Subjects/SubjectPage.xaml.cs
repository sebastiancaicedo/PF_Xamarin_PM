using Firebase.Xamarin.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;

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
                    SubjectToShow = await FirebaseHelper.GetSubjectById(SubjectToShow.GetUId());
                    listviewStudents.ItemsSource = null;
                    Students = await FirebaseHelper.GetStudentsByIds(SubjectToShow.StudentsKeys);
                    listviewStudents.ItemsSource = Students;
                    StudsIsRefreshing = false;
                });
            }
        }

        public ICommand EvalsRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    //LoginPage.LoggedUser = await FirebaseHelper.GetProfessorById(LoginPage.Auth.User.LocalId);
                    SubjectToShow = await FirebaseHelper.GetSubjectById(SubjectToShow.GetUId());
                    listviewEvaluations.ItemsSource = null;
                    Evaluations = await FirebaseHelper.GetEvaluationsByIds(SubjectToShow.EvaluationsKeys);
                    listviewEvaluations.ItemsSource = Evaluations;
                    EvalsIsRefreshing = false;
                });
            }
        }

        public SubjectPage(Subject subject)
        {
            this.SubjectToShow = subject;
            Title = subject.Name;
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

        /*public async Task<int> getStudentsAsync(List<string> studentsKeys)
        {
            if (studentsKeys.Count > 0)
            {
                try
                {
                    Students.Clear();
                    for (int index = 0; index < studentsKeys.Count; index++)
                    {
                        //System.Diagnostics.Debug.WriteLine("STUDENT KEY: " + studentsKeys[index]);
                        var items = await FirebaseHelper.firebaseDBClient
                            .Child("students")
                            .OnceAsync<Student>();

                        foreach (var item in items)
                        {
                            if (studentsKeys[index] == item.Key)
                            {
                                Student student = item.Object;
                                student.SetKey(item.Key);
                                Students.Add(student);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    //return false;
                    //throw ex;
                    await DisplayAlert("Err", "Error aqui taryendo a los estudiantes "+ex, "asd");
                }
            }
            return 0;      
        }*/

        /*public async Task<int> getEvaluations(List<string> evaluationKeys)
        {
            if(SubjectToShow.EvaluationsKeys.Count > 0)
            {
                try
                {
                    Evaluations.Clear();
                    for (int index = 0; index < SubjectToShow.EvaluationsKeys.Count; index++)
                    {
                        //System.Diagnostics.Debug.WriteLine("STUDENT KEY: " + studentsKeys[index]);
                        var items = await FirebaseHelper.firebaseDBClient
                            .Child("evaluations")
                            .OnceAsync<Evaluation>();

                        foreach (var item in items)
                        {
                            if (SubjectToShow.EvaluationsKeys[index] == item.Key)
                            {
                                Evaluation evaluation = item.Object;
                                evaluation.SetUid(item.Key);
                                Evaluations.Add(evaluation);
                            }
                        }

                    }
                }catch(Exception ex)
                {
                    await DisplayAlert("error", "error trayendo evaluaciones " + ex, "OK");
                    //throw ex;
                }
            }
            return 0;
        }*/

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
            //subjectToShow.AddStudent(student);
        }

        public void MakeNewEvaluation(object sender, EventArgs e)
        {
            MakeEvaluationPage page = new MakeEvaluationPage(SubjectToShow, Students);
            page.FinishActivity += OnFinishEvaluation;
            Navigation.PushAsync(page);
        }

        private void OnFinishEvaluation(object sender, ReturnInfo<Evaluation> e)
        {
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
        }

        public void ShowStudentInfo(object sender, ItemTappedEventArgs e)
        {
            Student student = e.Item as Student;
            StudentPage page = new StudentPage(student);
            Navigation.PushAsync(page);
            (sender as ListView).SelectedItem = null;
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