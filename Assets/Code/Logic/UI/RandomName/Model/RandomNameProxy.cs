using UnityEngine;
using System.Collections;
using Logic.Enums;
using System.Collections.Generic;
namespace Logic.UI.RandomName.Model
{
    public class RandomNameProxy : SingletonMono<RandomNameProxy>
    {
        void Awake()
        {
            instance = this;
        }

        public string RandomName(GenderType gender)
        {
            RandomNameData randomNameData = RandomNameData.GetRandomNameData();
            string result = string.Empty;
            int randomType = Random.Range(0, 10);
            switch (gender)
            {
                case GenderType.Male:
                    if (randomType % 2 == 0)
                        result = RandomStringFromList(randomNameData.foreNames) + RandomStringFromList(randomNameData.boyNames);
                    else
                        result = RandomStringFromList(randomNameData.boyNames) + RandomStringFromList(randomNameData.postNames);
                    break;
                case GenderType.Female:
                    if (randomType % 2 == 0)
                        result = RandomStringFromList(randomNameData.foreNames) + RandomStringFromList(randomNameData.girlNames);
                    else
                        result = RandomStringFromList(randomNameData.girlNames) + RandomStringFromList(randomNameData.postNames);
                    break;
                case GenderType.Child:
                    switch (randomType % 4)
                    {
                        case 0:
                            result = RandomStringFromList(randomNameData.foreNames) + RandomStringFromList(randomNameData.boyNames);
                            break;
                        case 1:
                            result = RandomStringFromList(randomNameData.foreNames) + RandomStringFromList(randomNameData.girlNames);
                            break;
                        case 2:
                            result = RandomStringFromList(randomNameData.boyNames) + RandomStringFromList(randomNameData.postNames);
                            break;
                        case 3:
                            result = RandomStringFromList(randomNameData.girlNames) + RandomStringFromList(randomNameData.postNames);
                            break;
                    }
                    break;
            }
            return result;
        }

        private string RandomStringFromList(List<string> strs)
        {
            int length = strs.Count;
            if (length == 0)
                return string.Empty;
            int index = Random.Range(0, length);
            return strs[index];
        }
    }
}