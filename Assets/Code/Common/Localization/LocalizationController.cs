using UnityEngine;
using System.Collections;
using System.IO;

namespace Common.Localization
{
    public class LocalizationController : SingletonMono<LocalizationController>
    {
        public const string DEFAULT_LANGUAGE = "Chinese";

        void Awake()
        {
            instance = this;
            Localization.loadFunction = LoadFunction;
            Localization.language = DEFAULT_LANGUAGE;
        }

        private byte[] LoadFunction(string path)
        {
            if (path == "Localization") return null;
            byte[] bytes = null;
            //if (Logic.Game.GameConfig.instance.loadCSVRemote)
            //{
            string localPath = Path.Combine("config/csv/", path) + ".csv";
            if (Common.ResMgr.ResUtil.ExistsInLocal(localPath))
            {
                bytes = ResMgr.ResMgr.instance.LoadBytes(localPath);
            }
            else
            {
                TextAsset textAsset = Common.ResMgr.ResMgr.instance.Load<TextAsset>(Path.Combine("languages/", path));
                if (textAsset != null)
                {
                    bytes = textAsset.bytes;
                }
            }
            return bytes;
        }

        public string Get(string key)
        {
            return Localization.Get(key);
        }
    }
}
