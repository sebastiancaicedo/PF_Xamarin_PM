using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PF_Xamarin_PM
{
    public class FirebaseHelper
    {
        //Auth Config Parameters
        public const string APP_API_KEY = "AIzaSyCV8rNgC4yWFirF911_lOgE9vUjlzmLlC0";
        public static FirebaseAuthProvider authProvider { get; } = new FirebaseAuthProvider(new FirebaseConfig(APP_API_KEY));

        //DB Parameters
        public static FirebaseClient firebaseDBClient { get; } = new FirebaseClient("https://proyectofinal-xamarin.firebaseio.com/");

        /// <summary>
        /// Devuelve un id nuevo y unico
        /// </summary>
        /// <returns></returns>
        public static string GetNewUniqueID()
        {
            return FirebaseKeyGenerator.Next();
        }

        /// <summary>
        /// Trae la rubrica con el id, desde firebase
        /// </summary>
        /// <param name="rubriId"></param>
        /// <returns></returns>
        public static async Task<Rubric> GetRubricById(string rubrickey)
        {
            var rubric = await firebaseDBClient
                    .Child("rubrics")
                    .Child(rubrickey)
                    .OnceSingleAsync<Rubric>();

            rubric.SetUid(rubrickey);

            return rubric;
        }

        /// <summary>
        /// Trae al profesor con el id, desde firebase
        /// </summary>
        /// <param name="professorId"></param>
        /// <returns></returns>
        public static async Task<Professor> GetProfessorById(string professorId)
        {
            var professor = await firebaseDBClient
                    .Child("professors")
                    .Child(professorId)
                    .OnceSingleAsync<Professor>();

            return professor;
        }

        /// <summary>
        /// Guarda el profesor logeado
        /// </summary>
        /// <param name="professor"></param>
        public static async Task SaveProfessorOnDB(Professor professor)
        {
            await firebaseDBClient
                .Child("professors")//tabla profesores
                .Child(LoginPage.Auth.User.LocalId)//se usa el id creado por la autenticacion como key tambien
                .PutAsync(professor);
        }

        /// <summary>
        /// Trae las asignaturas dados su ids, desde firebase
        /// </summary>
        /// <param name="subjectsKeys"></param>
        /// <returns></returns>
        public static async Task<IList<Subject>> GetSubjectsByIds(List<string> subjectsKeys)
        {
            IList<Subject> list = new ObservableCollection<Subject>();
            if (subjectsKeys.Count > 0)
            {
                foreach (var subjectKey in subjectsKeys)
                {
                    Subject subject = await firebaseDBClient
                        .Child("subjects")
                        .Child(subjectKey)
                        .OnceSingleAsync<Subject>();

                    subject.SetUId(subjectKey);

                    list.Add(subject);
                }
            }

            return list;
        }

        public static async Task SaveSubjectOnDB(Subject subject)
        {
            await firebaseDBClient
                .Child("subjects")
                .Child(subject.GetUId())
                .PutAsync(subject);
        }

        public static async Task<IList<Student>> GetStudentsByIds(List<string> studentsKeys)
        {
            IList<Student> list = new ObservableCollection<Student>();
            if(studentsKeys.Count > 0)
            {
                foreach (var studentKey in studentsKeys)
                {
                    var student = await firebaseDBClient
                        .Child("students")
                        .Child(studentKey)
                        .OnceSingleAsync<Student>();

                    student.SetKey(studentKey);

                    list.Add(student);
                }
            }
            return list;
        }

        public static async Task<IList<Evaluation>> GetEvaluationsByIds(List<string> evaluationsKeys)
        {
            IList<Evaluation> list = new ObservableCollection<Evaluation>();
            if (evaluationsKeys.Count > 0)
            {
                foreach (var evaluationKey in evaluationsKeys)
                {
                    var evaluation = await firebaseDBClient
                        .Child("evaluations")
                        .Child(evaluationKey)
                        .OnceSingleAsync<Evaluation>();

                    evaluation.SetUid(evaluationKey);

                    list.Add(evaluation);
                }
            }
            return list;
        }

        public static async Task SaveStudentOnDB(Student student)
        {
            await firebaseDBClient
                .Child("students")
                .Child(student.GetKey())
                .PutAsync(student);
        }

        public static async Task<Subject> GetSubjectById(string subjectKey)
        {
            Subject subject = await firebaseDBClient
                .Child("subjects")
                .Child(subjectKey)
                .OnceSingleAsync<Subject>();

            subject.SetUId(subjectKey);

            return subject;
        }

        public static async Task<Student> GetStudentById(string studentKey)
        {
            Student student = await firebaseDBClient
                .Child("students")
                .Child(studentKey)
                .OnceSingleAsync<Student>();

            student.SetKey(studentKey);

            return student;
        }
    }
}
