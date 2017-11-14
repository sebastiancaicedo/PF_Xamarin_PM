using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Firebase.Xamarin.Database.Query;

namespace PF_Xamarin_PM
{
    public class Subject
    {
        public string Name { get; private set; }
        public string ProfessorId { get; private set; } 
        public List<string> StudentsKeys { get; private set; } = new List<string>();
        //private IList<Evaluation> Evaluations { get; set; } = new ObservableCollection<Evaluation>();

        private string authKey = null;

        public Subject(string name, string professorId)
        {
            Name = name;
            ProfessorId = professorId;
            authKey = FirebaseHelper.GetNewUniqueID();
        }

        public string GetAuthId()
        {
            return authKey;
        }

        public void SetAuthId(string key)
        {
            authKey = key;
        }

        private void SaveSubjectOnDB()
        {
            try
            {
                FirebaseHelper.firebaseDBClient
                    .Child("subjects")
                    .Child(authKey)
                    .PutAsync<Subject>(this);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo para agregar un nuevo estudiante y lo guarda todo en la base de datos.
        /// NO HACERLO DESDE LA PROPIEDAD Students.Add(...).
        /// </summary>
        /// <param name="student">El estudiante a agregar</param>
        public void AddStudent(Student student)
        {
            try
            {
                string studentKey = student.GetKey();
                StudentsKeys.Add(studentKey);
                student.SubjectsKeys.Add(authKey);
                FirebaseHelper.firebaseDBClient
                    .Child("students")
                    .Child(studentKey)
                    .PutAsync(student);

                SaveSubjectOnDB();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            LoginPage.LoggedUser.SaveThisUserOnDB();
        }
    }
}
