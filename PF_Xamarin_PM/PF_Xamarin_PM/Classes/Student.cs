using Firebase.Xamarin.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace PF_Xamarin_PM
{
    public class Student
    {

        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string FullName { get { return string.Format("{0} {1}", Name, LastName); } }
        public List<string> SubjectsKeys { get; private set; } = new List<string>();

        private string authId;

        public Student(string name, string lastName, string email)
        {
            Name = name;
            LastName = lastName;
            Email = email;
        }

        public string GetKey()
        {
            return authId;
        }

        public void SetKey(string key)
        {
            authId = key;
        }

        public void AddToSubject(string subjectKey)
        {
            SubjectsKeys.Add(subjectKey);
        }

        public async void SaveStudentOnDB()
        {
            await FirebaseHelper.SaveStudentOnDB(this);
        }

        /*public async Task<bool> SaveThisUserOnDB(BackTrackInfo subjectToAdd, bool newStudent = false)
        {
            try
            {
                if (newStudent)
                {
                    SubjectsInfo.Add(subjectToAdd);
                    await FirebaseHelper.firebaseDBClient
                    .Child("students")//tabla estudiantes
                    .Child(authId)//se usa el id creado por la autenticacion como key tambien
                            //.WithAuth(token)
                    .PutAsync<Student>(this);

                    return true;
                }
                else
                {
                    return false;
                    //Todo here
                }
            }
            catch (Exception ex)
            {
                return false;
                //throw ex;
            }
        }*/
    }
}
