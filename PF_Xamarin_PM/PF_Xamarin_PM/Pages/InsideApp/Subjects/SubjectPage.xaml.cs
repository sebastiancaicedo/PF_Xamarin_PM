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
        public IList<Student> students = new ObservableCollection<Student>();

        public SubjectPage(Subject subject)
        {
            this.subjectToShow = subject;
            Title = subject.Name;
            InitializeComponent();
            listviewStudents.ItemsSource = students;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await getStudentsAsync(subjectToShow.StudentsKeys);
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
    }
}