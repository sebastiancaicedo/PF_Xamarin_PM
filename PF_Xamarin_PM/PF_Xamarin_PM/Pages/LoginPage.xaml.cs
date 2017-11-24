using Firebase.Xamarin.Auth;
using System;
using Xamarin.Forms;

namespace PF_Xamarin_PM
{
    public partial class LoginPage : ContentPage
	{
        public static FirebaseAuth Auth { get; private set; }
        public static Professor LoggedUser { get; set; }

		public LoginPage()
		{
            Title = "Iniciar Sesión";
			InitializeComponent();
		}

        /// <summary>
        /// Evento del click del boton show register page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowRegisterPage(object sender, EventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            registerPage.FinishActivity += OnFinishRegisterActivity;
            Navigation.PushAsync(registerPage);
        }

        /// <summary>
        /// Callback que se ejecuta cuando ocurre el evento FinishActivity de la pagina de registro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">La ReturnInfo que contiene el result y el return data del register page que contiene
        /// el professor creado y su contraseña.</param>
        private async void OnFinishRegisterActivity(object sender, ReturnInfo<RegisterPage.ReturnData> e)
        {
            if (e.Result == ReturnResult.Successful)//Todo bien
            {
                RegisterPage.ReturnData data = e.Data;
                try
                {
                    Auth = await FirebaseHelper.AuthProvider.CreateUserWithEmailAndPasswordAsync(data.ProfessorInfo.Email, data.Password);
                    //guardamos el usuario en la base de datos
                    data.ProfessorInfo.SaveThisUserOnDB();
                    LoggedUser = data.ProfessorInfo;
                    ShowHomePage();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "El email ya existe, :"+ex, "OK");
                    //throw ex;
                }
            }
        }

        /// <summary>
        /// Evento del click del boton LogIn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void LogIn(object sender, EventArgs e)
        {
            StartLoadingIndicator();
            if (String.IsNullOrEmpty(entryEmail.Text) && String.IsNullOrEmpty(entryPassword.Text))
            {
                entryEmail.Text = "admin@uninorte.edu.co";
                entryPassword.Text = "123456";
            }
            if (!String.IsNullOrEmpty(entryEmail.Text) && !String.IsNullOrEmpty(entryPassword.Text))
            {
                try
                {
                    Auth = await FirebaseHelper.AuthProvider.SignInWithEmailAndPasswordAsync(entryEmail.Text, entryPassword.Text);
                    LoggedUser = await FirebaseHelper.GetProfessorById(Auth.User.LocalId);
                    if (LoggedUser == null)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        ShowHomePage();
                    }

                }
                catch (Exception ex)
                {
                    StopLoadingIndicator();
                    await DisplayAlert("Error", "No es posible iniciar sesión :"+ex, "OK");
                    //throw ex;
                }
            }
            else
            {
                StopLoadingIndicator();
                await DisplayAlert("Error", "Uno o mas campos vacios", "OK");
            }
        }

        /// <summary>
        /// Muestra la ventana principal y la coloca como la main page de la aplicacion
        /// </summary>
        private void ShowHomePage()
        {
            StopLoadingIndicator();
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }

        /// <summary>
        /// Muestra el indicador de carga y oculta los demas Views
        /// </summary>
        private void StartLoadingIndicator()
        {
            loadIndicator.IsRunning = true;
            layoutTop.IsVisible = false;
            layoutBottom.IsVisible = false;
        }

        /// <summary>
        /// Detiene el indicador de carga y muestra los demas Views
        /// </summary>
        private void StopLoadingIndicator()
        {
            if (loadIndicator.IsRunning)
            {
                loadIndicator.IsRunning = false;
                layoutTop.IsVisible = true;
                layoutBottom.IsVisible = true;
            }
        }
    }
}
