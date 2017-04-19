using UnityEngine;
using System.Collections;
using Common.Util;
#if UNITY_EDITOR
using System.IO;
using System.Text;
#endif

namespace Logic.Game.Model
{
    public class InfoData
    {

        private static InfoData _infoData;

        public static InfoData GetInfoData()
        {
            if (_infoData == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    _infoData = CSVUtil.ParseClass<InfoData>("config/info");
                else
                {
#endif
                    string localPath = "config/csv/info";
                    if (Common.ResMgr.ResUtil.ExistsInLocal(localPath + ".csv"))
                        _infoData = CSVUtil.ParseClass<InfoData>(localPath);
                    else
                        _infoData = CSVUtil.ParseClass<InfoData>("config/info");
#if UNITY_EDITOR
                }
#endif
            }
            return _infoData;
        }

        #region field
        [CSVElement("version")]
        public string versionStr
        {
            set
            {
                version = value;
                //#if !UNITY_EDITOR
                //                GameConfig.instance.version = value;
                //#endif
            }
        }

        public string version;

        [CSVElement("ableDebug")]
        public bool ableDebug;

        [CSVElement("outLog")]
        public bool outLog;

        [CSVElement("assetBundle")]
        public bool assetBundle;
        #endregion

#if UNITY_EDITOR
        public static bool SaveCSV()
        {
            string path = Application.dataPath + "/Res/Resources/config/info.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("name,");
            sb.Append("infoConfig");
            sb.Append(CSVUtil.SYMBOL_LINE[0]);
            sb.Append("version,");
            sb.Append(_infoData.version);
            sb.Append(CSVUtil.SYMBOL_LINE[0]);
            sb.Append("ableDebug,");
            sb.Append(_infoData.ableDebug ? 1 : 0);
            sb.Append(CSVUtil.SYMBOL_LINE[0]);
            sb.Append("outLog,");
            sb.Append(_infoData.outLog ? 1 : 0);
            sb.Append(CSVUtil.SYMBOL_LINE[0]);
            sb.Append("assetBundle,");
            sb.Append(_infoData.assetBundle ? 1 : 0);
            sb.Append(CSVUtil.SYMBOL_LINE[0]);
            sw.WriteLine(sb.ToString());
            #endregion
            sw.Close();
            fs.Close();
            return true;
        }
#endif
    }
}