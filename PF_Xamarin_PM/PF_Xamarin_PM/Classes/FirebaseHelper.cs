using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;

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
    }
}
