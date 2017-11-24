using Firebase.Xamarin.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PF_Xamarin_PM
{
    public class Evaluation
    {
        public event EventHandler EvaluationSaved;

        private string Uid { get; set; } = null;
        public string Name { get; private set; }
        public string SubjectKey { get; private set; }
        public string RubricKey { get; private set; }
        private bool isCompleted;
        public bool IsCompleted { get { CheckForCompletion(); return isCompleted; } private set { isCompleted = value; } }
        private string status;
        public string Status { get { GetEvaluationStatus(); return status; } private set { status = value; } }
        public IList<Calification> Califications { get; private set; } = new ObservableCollection<Calification>();

        private List<Picker> ElementsPickers { get; set; } = new List<Picker>();
        private bool ChangesSaved { get; set; } = false;
        private bool WasSavedOnce { get; set; } = false;
        private ToolbarItem ToolbarItemIndicator { get; set; }

        public Evaluation(string name, string subjectKey, string rubricKey)
        {
            Name = name;
            SubjectKey = subjectKey;
            RubricKey = rubricKey;
            Uid = FirebaseHelper.GetNewUniqueID();
        }

        [Newtonsoft.Json.JsonConstructor]
        public Evaluation(string name, string subjectKey, string rubricKey, bool isCompleted, string status, IList<Calification> califications)
        {
            Name = name;
            SubjectKey = subjectKey;
            RubricKey = rubricKey;
            IsCompleted = isCompleted;
            Status = status;
            Califications = califications;
        }

        public string GetUid()
        {
            return Uid;
        }

        public void SetUid(string key)
        {
            Uid = key;
        }

        private void CheckForCompletion()
        {
            for (int index = 0; index < ElementsPickers.Count; index++)
            {
                if(ElementsPickers[index].SelectedIndex == -1)
                {
                    isCompleted = false;
                }
            }
            isCompleted = true;
        }

        private void GetEvaluationStatus()
        {
            status = isCompleted ? "Completa" : "Incompleta";
        }

        public bool CheckIfSaved()
        {
            return ChangesSaved;
        }

        public void SetToolbarInidicator(ToolbarItem toolbarItem)
        {
            ToolbarItemIndicator = toolbarItem;
        }

        public bool CheckIfSavedAtLeastOnce()
        {
            return WasSavedOnce;
        }

        public async void SaveEvaluationOnDB()
        {
            await FirebaseHelper.SaveEvaluationOnDB(this);

            ChangesSaved = true;
            ToolbarItemIndicator.Text = "Saved";
            if (!WasSavedOnce)
            {
                WasSavedOnce = true;
                EvaluationSaved(this, EventArgs.Empty);
            }
        }

        public View SetUp(Rubric rubric, IList<Student> students)
        {
            StackLayout layoutReturn = new StackLayout { Orientation = StackOrientation.Vertical };
            foreach (var student in students)
            {
                Calification calification = new Calification(student.GetKey(), student.FullName, rubric.Categories.Count);
                Califications.Add(calification);
                StackLayout layoutStudent = new StackLayout { Orientation = StackOrientation.Vertical };

                Label studentName = new Label { Text = student.FullName, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Large"), TextColor = Color.Red };
                StackLayout layoutScore = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                Label labelScoreTitle = new Label { Text = "Parcial Score", HorizontalOptions = LayoutOptions.StartAndExpand };
                Label labelPartialScore = new Label { Text = calification.FinalScore.ToString(), HorizontalOptions = LayoutOptions.CenterAndExpand };
                layoutScore.Children.Add(labelScoreTitle);
                layoutScore.Children.Add(labelPartialScore);

                layoutStudent.Children.Add(studentName);
                layoutStudent.Children.Add(layoutScore);

                foreach (var category in rubric.Categories)
                {
                    calification.CategoriesScores[rubric.Categories.IndexOf(category)] = new ScoreCategory(category.Elements.Count);

                    StackLayout layoutCategory = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(20, 10, 0, 0) };

                    StackLayout layoutCategoryHeader = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                    Label labelCategoryName = new Label { Text = category.Name, HorizontalOptions = LayoutOptions.StartAndExpand };
                    Label labelCategoryWeigth = new Label { Text = string.Format("{0}%", category.Weigth.ToString()), HorizontalOptions = LayoutOptions.CenterAndExpand };
                    layoutCategoryHeader.Children.Add(labelCategoryName);
                    layoutCategoryHeader.Children.Add(labelCategoryWeigth);

                    layoutCategory.Children.Add(layoutCategoryHeader);

                    layoutStudent.Children.Add(layoutCategory);

                    foreach (var element in category.Elements)
                    {
                        StackLayout layoutElement = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(40, 20, 0, 0) };

                        StackLayout layoutElementHeader = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                        Label labelElementName = new Label { Text = element.Name, HorizontalOptions = LayoutOptions.StartAndExpand };
                        Label labelElementWeigth = new Label { Text = string.Format("{0}%", element.Weigth.ToString()), HorizontalOptions = LayoutOptions.CenterAndExpand };
                        Picker pickerLevel = new Picker { Title = "Select Nivel", ItemsSource = rubric.GetLevelsName(element.Levels) };
                        ElementsPickers.Add(pickerLevel);
                        pickerLevel.SelectedIndexChanged += (sender, args) =>
                        {
                            ToolbarItemIndicator.Text = "Unsaved";
                            ChangesSaved = false;
                            int categoryIndex = rubric.Categories.IndexOf(category);
                            int elementIndex = category.Elements.IndexOf(element);
                            int levelSelected = pickerLevel.SelectedIndex;

                            calification.CategoriesScores[categoryIndex].SetElementScore(elementIndex, levelSelected, element.Levels[levelSelected].Value);
                            calculatePartialScore(calification, rubric);
                            labelPartialScore.Text = calification.FinalScore.ToString();

                        };
                        layoutElementHeader.Children.Add(labelElementName);
                        layoutElementHeader.Children.Add(labelElementWeigth);

                        layoutElement.Children.Add(layoutElementHeader);
                        layoutElement.Children.Add(pickerLevel);

                        layoutStudent.Children.Add(layoutElement);
                    }
                }

                layoutReturn.Children.Add(layoutStudent);
            }

            return layoutReturn;
        }

        public View SetUpForEdit(Rubric rubric)
        {
            ElementsPickers = new List<Picker>();
            ChangesSaved = true;
            WasSavedOnce = true;
            ToolbarItemIndicator.Text = "Saved";
            StackLayout layoutReturn = new StackLayout { Orientation = StackOrientation.Vertical };
            foreach (var calification in Califications)
            {
                StackLayout layoutStudent = new StackLayout { Orientation = StackOrientation.Vertical };

                Label studentName = new Label { Text = calification.StudentFullName, FontSize = (double)new FontSizeConverter().ConvertFromInvariantString("Large"), TextColor = Color.Red };
                StackLayout layoutScore = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                Label labelScoreTitle = new Label { Text = "Nota Parcial", HorizontalOptions = LayoutOptions.StartAndExpand };
                Label labelPartialScore = new Label { Text = calification.FinalScore.ToString(), HorizontalOptions = LayoutOptions.CenterAndExpand };
                layoutScore.Children.Add(labelScoreTitle);
                layoutScore.Children.Add(labelPartialScore);

                layoutStudent.Children.Add(studentName);
                layoutStudent.Children.Add(layoutScore);
                foreach (var category in rubric.Categories)
                {
                    int categoryIndex = rubric.Categories.IndexOf(category);
                    //calification.CategoriesScores[rubric.Categories.IndexOf(category)] = new ScoreCategory(category.Elements.Count);

                    StackLayout layoutCategory = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(20, 10, 0, 0) };

                    StackLayout layoutCategoryHeader = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                    Label labelCategoryName = new Label { Text = category.Name, HorizontalOptions = LayoutOptions.StartAndExpand };
                    Label labelCategoryWeigth = new Label { Text = string.Format("{0}%", category.Weigth.ToString()), HorizontalOptions = LayoutOptions.CenterAndExpand };
                    layoutCategoryHeader.Children.Add(labelCategoryName);
                    layoutCategoryHeader.Children.Add(labelCategoryWeigth);

                    layoutCategory.Children.Add(layoutCategoryHeader);

                    layoutStudent.Children.Add(layoutCategory);

                    foreach (var element in category.Elements)
                    {
                        int elementIndex = category.Elements.IndexOf(element);
                        StackLayout layoutElement = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(40, 20, 0, 0) };

                        StackLayout layoutElementHeader = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                        Label labelElementName = new Label { Text = element.Name, HorizontalOptions = LayoutOptions.StartAndExpand };
                        Label labelElementWeigth = new Label { Text = string.Format("{0}%", element.Weigth.ToString()), HorizontalOptions = LayoutOptions.CenterAndExpand };
                        Picker pickerLevel = new Picker { Title = "Seleccione Nivel", ItemsSource = rubric.GetLevelsName(element.Levels) };
                        int elementSelectedLevelIndex = calification.CategoriesScores[categoryIndex].ElementsSelectedLevelsIndex[elementIndex];
                        if(elementSelectedLevelIndex != -1)
                        {
                            pickerLevel.SelectedIndex = elementSelectedLevelIndex;
                        }
                        ElementsPickers.Add(pickerLevel);
                        pickerLevel.SelectedIndexChanged += (sender, args) =>
                        {
                            ToolbarItemIndicator.Text = "Unsaved";
                            ChangesSaved = false;

                            int levelSelected = pickerLevel.SelectedIndex;

                            calification.CategoriesScores[categoryIndex].SetElementScore(elementIndex, levelSelected, element.Levels[levelSelected].Value);
                            calculatePartialScore(calification, rubric);
                            labelPartialScore.Text = calification.FinalScore.ToString();

                        };
                        layoutElementHeader.Children.Add(labelElementName);
                        layoutElementHeader.Children.Add(labelElementWeigth);

                        layoutElement.Children.Add(layoutElementHeader);
                        layoutElement.Children.Add(pickerLevel);

                        layoutStudent.Children.Add(layoutElement);
                    }
                }
                layoutReturn.Children.Add(layoutStudent);
            }
            return layoutReturn;
        }

        private void calculatePartialScore(Calification calification, Rubric rubric)
        {
            float sumElements = 0;
            float sumCategories = 0;
            for (int indexCategory = 0; indexCategory < calification.CategoriesScores.Length; indexCategory++)
            {
                ScoreCategory scoreCategory = calification.CategoriesScores[indexCategory];
                for (int indexElement = 0; indexElement < scoreCategory.ElementScores.Length; indexElement++)
                {
                    float elementScore = scoreCategory.ElementScores[indexElement];
                    float elementWeigth = rubric.Categories[indexCategory].Elements[indexElement].Weigth;
                    if (elementScore > -1)
                    {
                        sumElements += elementScore * (elementWeigth / 100);
                    }
                }
                scoreCategory.CategoryScore = sumElements;
                float categoryWeigth = rubric.Categories[indexCategory].Weigth;
                sumCategories += sumElements * (categoryWeigth / 100);
            }
            calification.FinalScore = sumCategories;
        }

        public class Calification
        {
            public string StudentKey { get; private set; }
            public string StudentFullName { get; private set; }
            public ScoreCategory[] CategoriesScores { get; private set; }
            public float FinalScore { get; set; } = 0;

            [Newtonsoft.Json.JsonConstructor]
            public Calification(string studentKey, string studentFullName, ScoreCategory[] categoriesScores, float finalScore)
            {
                StudentKey = studentKey;
                StudentFullName = studentFullName;
                CategoriesScores = categoriesScores;
                FinalScore = finalScore;
            }

            public Calification(string studentKey, string studentFullName, int numberOfCategories)
            {
                StudentKey = studentKey;
                StudentFullName = studentFullName;
                CategoriesScores = new ScoreCategory[numberOfCategories];
            }
        }

        public class ScoreCategory
        {
            public float CategoryScore { get; set; } = 0;
            public float[] ElementScores { get; private set; }
            public int[] ElementsSelectedLevelsIndex { get; private set; }

            public ScoreCategory(int size)
            {
                ElementScores = new float[size];
                ElementsSelectedLevelsIndex = new int[size];
                for (int index = 0; index < size; index++)
                {
                    ElementScores[index] = -1;
                    ElementsSelectedLevelsIndex[index] = -1;
                }
            }

            [Newtonsoft.Json.JsonConstructor]
            public ScoreCategory(float categoryScore, float[] elementScores, int[] elementsSelectedLevelsIndex)
            {
                CategoryScore = categoryScore;
                ElementScores = elementScores;
                ElementsSelectedLevelsIndex = elementsSelectedLevelsIndex;
            }

            public void SetElementScore(int elementIndex, int selectedLevelIndex, float levelScoreValue)
            {
                ElementScores[elementIndex] = levelScoreValue;
                ElementsSelectedLevelsIndex[elementIndex] = selectedLevelIndex;
            }
        }
    }
}
