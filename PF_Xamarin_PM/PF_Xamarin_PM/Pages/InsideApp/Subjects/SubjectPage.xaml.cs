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

namespace PF_Xamarin_PM
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SubjectPage : TabbedPage
	{
        private Subject subjectToShow;
        private IList<Student> students = new ObservableCollection<Student>();
        private IList<Evaluation> evaluations = new ObservableCollection<Evaluation>();

        public SubjectPage(Subject subject)
        {
            this.subjectToShow = subject;
            Title = subject.Name;
            InitializeComponent();
            listviewStudents.ItemsSource = students;
            listviewEvaluations.ItemsSource = evaluations;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await getStudentsAsync(subjectToShow.StudentsKeys);
            await getEvaluations(subjectToShow.EvaluationsKeys);
        }

        public async Task<int> getStudentsAsync(List<string> studentsKeys)
        {
            if (studentsKeys.Count > 0)
            {
                try
                {
                    students.Clear();
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
                                students.Add(student);
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
        }

        public async Task<int> getEvaluations(List<string> evaluationKeys)
        {
            if(subjectToShow.EvaluationsKeys.Count > 0)
            {
                try
                {
                    evaluations.Clear();
                    for (int index = 0; index < subjectToShow.EvaluationsKeys.Count; index++)
                    {
                        //System.Diagnostics.Debug.WriteLine("STUDENT KEY: " + studentsKeys[index]);
                        var items = await FirebaseHelper.firebaseDBClient
                            .Child("evaluations")
                            .OnceAsync<Evaluation>();

                        foreach (var item in items)
                        {
                            if (subjectToShow.EvaluationsKeys[index] == item.Key)
                            {
                                Evaluation evaluation = item.Object;
                                evaluation.SetUid(item.Key);
                                evaluations.Add(evaluation);
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
        }

        public void AddNewStudent(object sender, EventArgs e)
        {
            AddStudentPage page = new AddStudentPage();
            page.FinishActivity += OnFinishAddStudent;
            Navigation.PushAsync(page);
        }

        private void OnFinishAddStudent(object sender, ReturnInfo<Student> e)
        {
            Student student = e.Data;
            subjectToShow.AddStudent(student);
            //students.Add(student);
            //subjectToShow.AddStudent(student);
        }

        public void MakeNewEvaluation(object sender, EventArgs e)
        {
            MakeEvaluationPage page = new MakeEvaluationPage(subjectToShow, students);
            page.FinishActivity += OnFinishEvaluation;
            Navigation.PushAsync(page);
        }

        private void OnFinishEvaluation(object sender, ReturnInfo<Evaluation> e)
        {
            if (e.Result == ReturnResult.UnCompleted)
            {
                subjectToShow.SaveSubjectOnDB();
                evaluations.Add(e.Data);
            }
            else
                if (e.Result == ReturnResult.Successful)
            {
                subjectToShow.SaveSubjectOnDB();
                evaluations.Add(e.Data);
            }
            else
            {
                subjectToShow.EvaluationsKeys.Remove(e.Data.GetUid());
                subjectToShow.SaveSubjectOnDB();
            }
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
            CalificationsPage page = new CalificationsPage(evaluation, students);
            Navigation.PushAsync(page);
            (sender as ListView).SelectedItem = null;
        }
    }
}