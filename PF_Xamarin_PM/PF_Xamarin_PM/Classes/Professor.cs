using Firebase.Xamarin.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Firebase.Xamarin.Database;
using System.Threading.Tasks;
using System.Diagnostics;

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
        //public IList<Subject> Subjects { get; private set; } = new ObservableCollection<Subject>();
        //private IList<Rubric> Rubrics { get; set; } = new ObservableCollection<Rubric>();

        public Professor(string name, string lastName, string email)
        {
            Name = name;
            LastName = lastName;
            Email = email;
        }

        public void SaveThisUserOnDB()
        {
            try
            {
                FirebaseHelper.firebaseDBClient
                .Child("professors")//tabla profesores
                .Child(LoginPage.auth.User.LocalId)//se usa el id creado por la autenticacion como key tambien
                //.WithAuth(token)
                .PutAsync(this);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// Metodo para agregar una nueva asignatura y la guarda todo en la base de datos.
        /// NO HACERLO DESDE LA PROPIEDAD Subjects.Add(...).
        /// </summary>
        /// <param name="subject">La asignatura a agregar</param>
        public void AddSubject(Subject subject)
        {
            //Subjects.Add(subject);
            //Creamos una nueva asignatura
            try
            {
                string subjectKey = subject.GetAuthId();
                SubjectsKeys.Add(subjectKey);
                SaveThisUserOnDB();
                FirebaseHelper.firebaseDBClient
                    .Child("subjects")
                    .Child(subjectKey)
                    .PutAsync<Subject>(subject);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
