using Firebase.Xamarin.Database.Query;
using System;
using System.Collections.Generic;

namespace PF_Xamarin_PM
{
    public class Professor
    {
        //El Id se maneja con el UID que se genera cuando se crea un usuario para autenticacion, se guarda con ese mismo
        //public string Uid { get; set; }
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string FullName { get { return string.Format("{0} {1}", Name, LastName); } }
        public List<string> SubjectsKeys { get; private set; } = new List<string>();
        public List<string> RubricsKeys { get; private set; } = new List<string>();

        public Professor(string name, string lastName, string email)
        {
            Name = name;
            LastName = lastName;
            Email = email;
        }

        public async void SaveThisUserOnDB()
        {
            await FirebaseHelper.SaveProfessorOnDB(this);
        }

        /// <summary>
        /// Metodo para agregar una nueva asignatura y la guarda todo en la base de datos.
        /// NO HACERLO DESDE LA PROPIEDAD Subjects.Add(...).
        /// </summary>
        /// <param name="subject">La asignatura a agregar</param>
        public void AddSubject(Subject subject)
        {
            SubjectsKeys.Add(subject.GetUId());
            SaveThisUserOnDB();
            subject.SaveSubjectOnDB();
        }

        public void AddRubric(Rubric rubric)
        {
            RubricsKeys.Add(rubric.GetUid());
            SaveThisUserOnDB();
            rubric.SaveRubricOnDB();
        }

    }
}
