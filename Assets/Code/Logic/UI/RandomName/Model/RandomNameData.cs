using Common.Util;
using System.Collections;
using System.Collections.Generic;

namespace Logic.UI.RandomName.Model
{
    public class RandomNameData
    {
        private static List<RandomNameData> _randomNameDataList;

        public static RandomNameData GetRandomNameData()
        {
            if (_randomNameDataList == null)
            {
#if UNITY_EDITOR
				string localPath = "config/names";
#else
                string localPath = "config/csv/names";
#endif
                if (Common.ResMgr.ResUtil.ExistsInLocal(localPath + ".csv"))
                    _randomNameDataList = CSVUtil.Parse<RandomNameData>(localPath);
                else
                    _randomNameDataList = CSVUtil.Parse<RandomNameData>("config/names");
            }
            if (_randomNameDataList.Count > 0)
                return _randomNameDataList[0];
            else
                return null;
        }

        public List<string> foreNames;
        [CSVElement("Fore")]
        public string foreName
        {
            set
            {
                foreNames = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<string> postNames;

        [CSVElement("Post")]
        public string postName
        {
            set
            {
                postNames = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<string> boyNames;
        [CSVElement("Boy")]
        public string boyName
        {
            set
            {
                boyNames = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<string> girlNames;
        [CSVElement("Girl")]
        public string girlName
        {
            set
            {
                girlNames = value.ToList(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
    }
}