using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : MasterDetailPage
	{
        private IList<MenuItem> MenuItems { get; set; } = new ObservableCollection<MenuItem>();

        MenuItem mySubjectsPageItem = new MenuItem { IconSource = "ic_custom_asignaturas.png", Title = "Mis Asignaturas", Page = new MySubjectsPage() };
        MenuItem myRubricsPageItem = new MenuItem { IconSource = "ic_custom_rubricas.png", Title = "Mis Rubricas", Page = new MyRubricsPage() };

		public HomePage ()
		{
            InitializeComponent();
            BindingContext = LoginPage.LoggedUser;
            MenuItems.Add(mySubjectsPageItem);
            MenuItems.Add(myRubricsPageItem);
            listviewMenuItems.ItemsSource = this.MenuItems;
            Detail = new NavigationPage(new MySubjectsPage());
            IsPresented = false;
		}

        public void ItemMenuTapped(object sender, ItemTappedEventArgs e)
        {
            MenuItem menuItem = e.Item as MenuItem;
            Detail = new NavigationPage(menuItem.Page);
            IsPresented = false;
            (sender as ListView).SelectedItem = null;
        }

    }
}