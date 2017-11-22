using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PF_Xamarin_PM
{
    public class Rubric
    {
        private string Uid { get; set; }
        public string Name { get; set; }
        public float MinScore { get; set; }
        public float MaxScore { get; set; }
        public List<RubricCategory> Categories { get; private set; } = new List<RubricCategory>();

        private StackLayout LayoutCategories { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        public Rubric(string name, List<RubricCategory> categories)
        {
            Name = name;
            Categories = categories;
            Uid = FirebaseHelper.GetNewUniqueID();
        }

        public Rubric()
        {
            Uid = FirebaseHelper.GetNewUniqueID();
        }

        public string GetUid()
        {
            return Uid;
        }

        public void SetUid(string key)
        {
            Uid = key;
        }

        public List<string> GetLevelsName(List<RubricCategoryElementLevel> levels)
        {
            List<string> list = new List<string>();
            foreach (var level in levels)
            {
                list.Add(level.Name);
            }
            return list;
        }

        public void AddCategory(RubricCategory category)
        {
            Categories.Add(category);
            LayoutCategories.Children.Add(category.SetUp(Categories.IndexOf(category) + 1));
        }

        public View SetUp()
        {
            LayoutCategories = new StackLayout { Orientation = StackOrientation.Vertical };
            RubricCategory category = new RubricCategory();
            AddCategory(category);

            return LayoutCategories;
        }

        public bool CanBeSaved(out string error)
        {
            for (int index = 0; index < Categories.Count; index++)
            {
                if (!String.IsNullOrEmpty(Categories[index].Name))
                {
                    if (verifCategoriesWeigths())
                    {
                        if (verifElementsNames())
                        {
                            if (verifElementsWeigths())
                            {
                                if (verifLevelsNames())
                                {
                                    if (!verifLevelsValues(MinScore, MaxScore))
                                    {
                                        error = string.Format("Los valores asignados a los niveles deben estar en el rango de {0} y {1}", MinScore, MaxScore);
                                        return false;
                                    }
                                }
                                else
                                {
                                    error = "Todas los Niveles deben tener un nombre";
                                    return false;
                                }
                            }
                            else
                            {
                                error = "La suma de los pesos de los elementos de cada categoria debe sumar 100%, por favor verifique";
                                return false;
                            }
                        }
                        else
                        {
                            error = "Todas los elementos deben tener un nombre";
                            return false;
                        }
                    }
                    else
                    {
                        error = "La suma de los pesos de todas las categorias deben sumar 100%, por favor verifique";
                        return false;
                    }


                }
                else
                {
                    error = "Todas las categorias deben tener un nombre";
                    return false;
                }
            }
            error = null;
            return true;
        }

        private bool verifCategoriesWeigths()
        {
            float sum = 0;
            for (int index = 0; index < Categories.Count; index++)
            {
                RubricCategory category = Categories[index];
                System.Diagnostics.Debug.WriteLine(category.Weigth);
                if(category.Weigth >= 0)
                {
                    sum += category.Weigth;
                }
                else
                {
                    return false;
                }
            }

            if(sum != 100)
            {
                return false;
            }
            return true;
        }

        private bool verifElementsNames()
        {
            for (int catgIndex = 0; catgIndex < Categories.Count; catgIndex++)
            {
                RubricCategory category = Categories[catgIndex];
                for (int elemIndex = 0; elemIndex < category.Elements.Count; elemIndex++)
                {
                    RubricCategoryElement element = category.Elements[elemIndex];
                    if (String.IsNullOrEmpty(element.Name))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool verifElementsWeigths()
        {
            float sum = 0;
            for (int catgIndex = 0; catgIndex < Categories.Count; catgIndex++)
            {
                RubricCategory category = Categories[catgIndex];
                for (int elemIndex = 0; elemIndex < category.Elements.Count; elemIndex++)
                {
                    RubricCategoryElement element = category.Elements[elemIndex];
                    if(element.Weigth >= 0)
                    {
                        sum += element.Weigth;
                    }
                    else
                    {
                        return false;
                    }
                }

                if(sum != 100)
                {
                    return false;
                }
            }
            return true;
        }

        private bool verifLevelsNames()
        {
            for (int catgIndex = 0; catgIndex < Categories.Count; catgIndex++)
            {
                RubricCategory category = Categories[catgIndex];
                for (int elemIndex = 0; elemIndex < category.Elements.Count; elemIndex++)
                {
                    RubricCategoryElement element = category.Elements[elemIndex];
                    for (int lvlIndex = 0; lvlIndex < element.Levels.Count; lvlIndex++)
                    {
                        RubricCategoryElementLevel level = element.Levels[lvlIndex];
                        if (String.IsNullOrEmpty(level.Name))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool verifLevelsValues(float minValue, float maxValue)
        {
            for (int catgIndex = 0; catgIndex < Categories.Count; catgIndex++)
            {
                RubricCategory category = Categories[catgIndex];
                for (int elemIndex = 0; elemIndex < category.Elements.Count; elemIndex++)
                {
                    RubricCategoryElement element = category.Elements[elemIndex];
                    for (int lvlIndex = 0; lvlIndex < element.Levels.Count; lvlIndex++)
                    {
                        RubricCategoryElementLevel level = element.Levels[lvlIndex];
                        if(level.Value < minValue || lvlIndex > maxValue)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        public class RubricCategory
        {
            public string Name { get; private set; }
            public float Weigth { get; private set; }
            public List<RubricCategoryElement> Elements { get; private set; } = new List<RubricCategoryElement>();

            private StackLayout LayoutElements { get; set; }

            [Newtonsoft.Json.JsonConstructor]
            public RubricCategory(string name, float weigth, List<RubricCategoryElement> elements)
            {
                Name = name;
                Weigth = weigth;
                Elements = elements;
            }

            public RubricCategory()
            {
            }

            public View SetUp(int categoryIndex)
            {
                LayoutElements = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(20, 0, 0, 0) };

                StackLayout layoutCategory = new StackLayout { Orientation = StackOrientation.Vertical };

                StackLayout layoutCatgHeader = new StackLayout { Orientation = StackOrientation.Horizontal };
                Label labelCatgTitle = new Label { Text = "Categoria: " + categoryIndex, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"),
                    HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Red
                };
                Button buttonAddElement = new Button { Text = "+ Elemento" };
                layoutCatgHeader.Children.Add(labelCatgTitle);
                layoutCatgHeader.Children.Add(buttonAddElement);
                buttonAddElement.Clicked += (sender, e) =>
                {
                    AddElement(new RubricCategoryElement());
                };

                StackLayout layoutCatgInfo = new StackLayout { Orientation = StackOrientation.Horizontal };
                Entry entryCatgName = new Entry { Placeholder = "Nombre de la Categoria", HorizontalOptions = LayoutOptions.FillAndExpand };
                entryCatgName.TextChanged += (sender, args) =>
                {
                    Name = args.NewTextValue;
                };
                Entry entryCatgWeigth = new Entry { Placeholder = "Peso en %", Keyboard = (Keyboard)new KeyboardTypeConverter().ConvertFromInvariantString("Numeric") };
                entryCatgWeigth.TextChanged += (sender, args) =>
                {
                    float weigth;
                    float.TryParse(args.NewTextValue, out weigth);
                    if(weigth >= 0)
                    {
                        Weigth = weigth;
                    }
                    else
                    {
                        entryCatgWeigth.Text = args.OldTextValue;
                    }
                };
                layoutCatgInfo.Children.Add(entryCatgName);
                layoutCatgInfo.Children.Add(entryCatgWeigth);

                layoutCategory.Children.Add(layoutCatgHeader);
                layoutCategory.Children.Add(layoutCatgInfo);

                layoutCategory.Children.Add(LayoutElements);

                //Agreganos el elemento por defecto
                AddElement(new RubricCategoryElement());


                return layoutCategory;
            }

            public void AddElement(RubricCategoryElement element)
            {
                Elements.Add(element);
                LayoutElements.Children.Add(element.SetUp(Elements.IndexOf(element) + 1));
            }
        }

        public class RubricCategoryElement
        {
            public string Name { get; private set; }
            public float Weigth { get; private set; } = -1;
            public List<RubricCategoryElementLevel> Levels { get; private set; } = new List<RubricCategoryElementLevel>();

            private StackLayout LayoutLevels { get; set; }

            [Newtonsoft.Json.JsonConstructor]
            public RubricCategoryElement(string name, float weigth, List<RubricCategoryElementLevel> levels)
            {
                Name = name;
                Weigth = weigth;
                Levels = levels;
            }

            public RubricCategoryElement()
            {
            }

            public View SetUp(int elementIndex)
            {
                LayoutLevels = new StackLayout { Orientation = StackOrientation.Vertical };

                StackLayout layoutElement = new StackLayout { Orientation = StackOrientation.Vertical };

                StackLayout layoutElemHeader = new StackLayout { Orientation = StackOrientation.Horizontal };
                Label labelElemTitle = new Label { Text = "Elemento: " + elementIndex,
                    FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Medium"),
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.IndianRed
                };
                Button buttonAddLevel = new Button { Text = "+ Nivel" };
                layoutElemHeader.Children.Add(labelElemTitle);
                layoutElemHeader.Children.Add(buttonAddLevel);
                buttonAddLevel.Clicked += (sender, e) =>
                {
                    AddLevel(new RubricCategoryElementLevel());
                };

                StackLayout layoutElemInfo = new StackLayout { Orientation = StackOrientation.Horizontal };
                Entry entryElemName = new Entry { Placeholder = "Nombre del Elemento", HorizontalOptions = LayoutOptions.FillAndExpand };
                entryElemName.TextChanged += (sender, args) =>
                {
                    Name = args.NewTextValue;
                };
                Entry entryElemWeigth = new Entry { Placeholder = "Peso en %", Keyboard = (Keyboard)new KeyboardTypeConverter().ConvertFromInvariantString("Numeric") };
                entryElemWeigth.TextChanged += (sender, args) =>
                {
                    float weigth;
                    float.TryParse(args.NewTextValue, out weigth);
                    if (weigth >= 0)
                    {
                        Weigth = weigth;
                    }
                    else
                    {
                        entryElemWeigth.Text = args.OldTextValue;
                    }
                };
                layoutElemInfo.Children.Add(entryElemName);
                layoutElemInfo.Children.Add(entryElemWeigth);

                layoutElement.Children.Add(layoutElemHeader);
                layoutElement.Children.Add(layoutElemInfo);

                layoutElement.Children.Add(LayoutLevels);

                //Agregamos 2 niveles por default
                AddLevel(new RubricCategoryElementLevel());
                AddLevel(new RubricCategoryElementLevel());

                return layoutElement;
            }

            public void AddLevel(RubricCategoryElementLevel level)
            {
                Levels.Add(level);
                LayoutLevels.Children.Add(level.SetUp(Levels.IndexOf(level) + 1));
            }
        }

        public class RubricCategoryElementLevel
        {
            public string Name { get; private set; }
            public float Value { get; private set; } = -1;

            [Newtonsoft.Json.JsonConstructor]
            public RubricCategoryElementLevel(string name, float value)
            {
                Name = name;
                Value = value;
            }

            public RubricCategoryElementLevel()
            {

            }

            public View SetUp(int levelIndex)
            {
                StackLayout layoutLevel = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(40,0,0,0) };

                Label labelLevelTitle = new Label
                {
                    Text = "Nivel: " + levelIndex,
                    FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Small"),
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.OrangeRed
                };
                StackLayout layoutLevelInfo = new StackLayout { Orientation = StackOrientation.Horizontal };
                Entry entryLevelmName = new Entry { Placeholder = "Nombre del Nivel", HorizontalOptions = LayoutOptions.FillAndExpand };
                entryLevelmName.TextChanged += (sender, args) =>
                {
                    Name = args.NewTextValue;
                };
                Entry entryLevelWeigth = new Entry { Placeholder = "Valor", Keyboard = (Keyboard)new KeyboardTypeConverter().ConvertFromInvariantString("Numeric") };
                entryLevelWeigth.TextChanged += (sender, args) =>
                {
                    float weigth;
                    float.TryParse(args.NewTextValue, out weigth);
                    if (weigth >= 0)
                    {
                        Value = weigth;
                    }
                    else
                    {
                        entryLevelWeigth.Text = args.OldTextValue;
                    }
                };

                layoutLevelInfo.Children.Add(entryLevelmName);
                layoutLevelInfo.Children.Add(entryLevelWeigth);

                layoutLevel.Children.Add(labelLevelTitle);
                layoutLevel.Children.Add(layoutLevelInfo);

                return layoutLevel;
            }
        }
    }
}
