using System;
using System.Collections.Generic;
using System.Text;

namespace PF_Xamarin_PM
{
    public class Rubric
    {
        private string Uid { get; set; }
        public string Name { get; private set; }
        public List<RubricCategory> Categories { get; private set; } = new List<RubricCategory>();

        public Rubric(string name, List<RubricCategory> categories)
        {
            Name = name;
            Categories = categories;
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


        public class RubricCategory
        {
            public string Name { get; private set; }
            public float Weigth { get; private set; }
            public List<RubricCategoryElement> Elements { get; private set; }

            public RubricCategory(string name, float weigth, List<RubricCategoryElement> elements)
            {
                Name = name;
                Weigth = weigth;
                Elements = elements;
            }
        }

        public class RubricCategoryElement
        {
            public string Name { get; private set; }
            public float Weigth { get; private set; }
            public List<RubricCategoryElementLevel> Levels { get; private set; }

            public RubricCategoryElement(string name, float weigth, List<RubricCategoryElementLevel> levels)
            {
                Name = name;
                Weigth = weigth;
                Levels = levels;
            }
        }

        public class RubricCategoryElementLevel
        {
            public string Name { get; private set; }
            public float Value { get; private set; }

            public RubricCategoryElementLevel(string name, float value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
