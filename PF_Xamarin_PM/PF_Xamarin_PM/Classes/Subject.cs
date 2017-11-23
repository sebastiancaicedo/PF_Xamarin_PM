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
        private string UId { get; set; } = null;
        public string Name { get; private set; }
        public string ProfessorId { get; private set; }
        public string ProfessorFullName { get; private set; }
        public List<string> StudentsKeys { get; private set; } = new List<string>();
        public List<string> EvaluationsKeys { get; private set; } = new List<string>();

        public Subject(string name, string professorFullName, string professorId)
        {
            Name = name;
            ProfessorId = professorId;
            ProfessorFullName = professorFullName;
            UId = FirebaseHelper.GetNewUniqueID();
        }

        public string GetUId()
        {
            return UId;
        }

        public void SetUId(string key)
        {
            UId = key;
        }

        public async void SaveSubjectOnDB()
        {
            await FirebaseHelper.SaveSubjectOnDB(this);
        }

        /// <summary>
        /// Metodo para agregar un nuevo estudiante y lo guarda todo en la base de datos.
        /// NO HACERLO DESDE LA PROPIEDAD Students.Add(...).
        /// </summary>
        /// <param name="student">El estudiante a agregar</param>
        public void AddStudent(Student student)
        {
            StudentsKeys.Add(student.GetKey());
            student.SubjectsKeys.Add(UId);
            student.SaveStudentOnDB();
            SaveSubjectOnDB();
            LoginPage.LoggedUser.SaveThisUserOnDB();
        }
    }
}
