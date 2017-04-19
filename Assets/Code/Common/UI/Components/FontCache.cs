using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

namespace Common.UI.Components
{
    public class FontCache : SingletonMono<FontCache>
    {
        private const string FONT_1 = "fonts/FZY3JW_Dynamic";
        private List<Font> _cacheFonts;
        private StringBuilder _stringBuilder = new StringBuilder();

        void Awake()
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            _cacheFonts = new List<Font>();
        }

        public void LoadFonts()
        {
            Font font1 = ResMgr.ResMgr.instance.Load<Font>(FONT_1);
            _cacheFonts.Add(font1);
        }
    }
}
