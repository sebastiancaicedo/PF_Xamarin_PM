
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PF_Xamarin_PM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RubricPage : TabbedPage
	{
        private Rubric Rubric { get; set; }

		public RubricPage (Rubric rubric)
		{
            this.Rubric = rubric;
            Title = "Rubrica: " + rubric.Name;
			InitializeComponent ();
            BindingContext = this.Rubric;
            SetUpRubric();
		}

        private void SetUpRubric()
        {
            StackLayout layoutCategories = new StackLayout { Orientation = StackOrientation.Vertical };
            foreach (var category in Rubric.Categories)
            {
                int categoryIndex = Rubric.Categories.IndexOf(category) + 1;
                StackLayout layoutCategory = new StackLayout { Orientation = StackOrientation.Vertical };

                Label labelCatgTitle = new Label
                {
                    Text = "Categoria: " + categoryIndex,
                    FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"),
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Red
                };
                layoutCategory.Children.Add(labelCatgTitle);
                StackLayout layoutCatgHeaderTable = new StackLayout { Orientation = StackOrientation.Horizontal };

                StackLayout layoutCatgColumName = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.StartAndExpand };
                Label labelCatgNameTitle = new Label { Text = "Nombre de la categoria", HorizontalTextAlignment = TextAlignment.Center };
                Label labelCatgName = new Label { Text = category.Name, FontAttributes = FontAttributes.Bold, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium") };
                layoutCatgColumName.Children.Add(labelCatgNameTitle);
                layoutCatgColumName.Children.Add(labelCatgName);

                layoutCatgHeaderTable.Children.Add(layoutCatgColumName);

                StackLayout layoutCatgColumWeigth = new StackLayout { Orientation = StackOrientation.Vertical };
                Label labelCatgWeigthTitle = new Label { Text = "Peso", HorizontalTextAlignment = TextAlignment.Center };
                Label labeCatgWeigth = new Label { Text = string.Format("{0}%", category.Weigth), FontAttributes = FontAttributes.Bold, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium") };
                layoutCatgColumWeigth.Children.Add(labelCatgWeigthTitle);
                layoutCatgColumWeigth.Children.Add(labeCatgWeigth);

                layoutCatgHeaderTable.Children.Add(layoutCatgColumWeigth);

                layoutCategory.Children.Add(layoutCatgHeaderTable);

                StackLayout layoutElements = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(20, 10, 0, 0) };
                foreach (var element in category.Elements)
                {
                    int elementIndex = category.Elements.IndexOf(element) + 1;
                    StackLayout layoutElement = new StackLayout { Orientation = StackOrientation.Vertical };
                    Label labelElemTitle = new Label
                    {
                        Text = "Elemento: " + elementIndex,
                        FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Red
                    };
                    layoutElement.Children.Add(labelElemTitle);
                    StackLayout layoutElemHeaderTable = new StackLayout { Orientation = StackOrientation.Horizontal };

                    StackLayout layoutElemColumName = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.StartAndExpand };
                    Label labelElemNameTitle = new Label { Text = "Nombre del Elemento", HorizontalTextAlignment = TextAlignment.Center };
                    Label labelElemName = new Label { Text = element.Name, FontAttributes = FontAttributes.Bold, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium") };
                    layoutElemColumName.Children.Add(labelElemNameTitle);
                    layoutElemColumName.Children.Add(labelElemName);

                    layoutElemHeaderTable.Children.Add(layoutElemColumName);

                    StackLayout layoutElemColumWeigth = new StackLayout { Orientation = StackOrientation.Vertical };
                    Label labelElemWeigthTitle = new Label { Text = "Peso", HorizontalTextAlignment = TextAlignment.Center };
                    Label labeElemWeigth = new Label { Text = string.Format("{0}%", element.Weigth), FontAttributes = FontAttributes.Bold, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium") };
                    layoutElemColumWeigth.Children.Add(labelElemWeigthTitle);
                    layoutElemColumWeigth.Children.Add(labeElemWeigth);

                    layoutElemHeaderTable.Children.Add(layoutElemColumWeigth);

                    layoutElement.Children.Add(layoutElemHeaderTable);

                    StackLayout layoutLevels = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(40,10,0,0) };
                    Label labelLevelsTitle = new Label
                    {
                        Text = "Niveles",
                        FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Red
                    };
                    layoutLevels.Children.Add(labelLevelsTitle);

                    StackLayout layoutLevelsHeader = new StackLayout { Orientation = StackOrientation.Horizontal };
                    Label labelLevelNameTitle = new Label { Text = "Nombre del Nivel", HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.StartAndExpand };
                    Label labelLevelValueTitle = new Label { Text = "Valor", HorizontalTextAlignment = TextAlignment.Center };
                    layoutLevelsHeader.Children.Add(labelLevelNameTitle);
                    layoutLevelsHeader.Children.Add(labelLevelValueTitle);

                    layoutLevels.Children.Add(layoutLevelsHeader);
                    foreach (var level in element.Levels)
                    {
                        int levelIndex = element.Levels.IndexOf(level);
                        StackLayout layoutLevel = new StackLayout { Orientation = StackOrientation.Horizontal };
                        Label labelLevelName = new Label { Text = level.Name, FontAttributes = FontAttributes.Bold, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"), HorizontalOptions = LayoutOptions.StartAndExpand };
                        Label labelLevelScoreValue = new Label { Text = level.Value.ToString(), FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium") };
                        layoutLevel.Children.Add(labelLevelName);
                        layoutLevel.Children.Add(labelLevelScoreValue);

                        layoutLevels.Children.Add(layoutLevel);
                    }
                    layoutElement.Children.Add(layoutLevels);

                    layoutElements.Children.Add(layoutElement);
                }
                layoutCategory.Children.Add(layoutElements);

                layoutCategories.Children.Add(layoutCategory);
            }

            layoutMain.Children.Add(layoutCategories);
        }
	}
}