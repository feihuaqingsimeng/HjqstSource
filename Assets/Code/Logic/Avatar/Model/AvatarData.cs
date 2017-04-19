using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;

namespace Logic.Avatar.Model
{
    public class AvatarData
    {
        private static Dictionary<uint, AvatarData> _avatarDataDictionary;

        public static Dictionary<uint, AvatarData> GetAvatarDatas()
        {
            if (_avatarDataDictionary == null)
            {
                _avatarDataDictionary = CSVUtil.Parse<uint, AvatarData>("config/csv/avatar", "Id");
            }
            return _avatarDataDictionary;
        }

        public static Dictionary<uint, AvatarData> AvatarDataDictionary
        {
            get
            {
                if (_avatarDataDictionary == null)
                {
                    GetAvatarDatas();
                }
                return _avatarDataDictionary;
            }
        }

        public static AvatarData GetAvatarData(uint avatarID)
        {
            AvatarData avatarData = null;
            if (AvatarDataDictionary.ContainsKey(avatarID))
            {
                avatarData = AvatarDataDictionary[avatarID];
            }
            return avatarData;
        }

        public string GetHairPathByIndex(uint hairIndex)
        {
            if (hairIndex >= hairPaths.Length)
                return string.Empty;
            return hairPaths[hairIndex];
        }

        public string GetHairColorPathByIndex(uint hairColorIndex)
        {
            if (hairColorIndex >= hairColorPaths.Length)
                return string.Empty;
            return hairColorPaths[hairColorIndex];
        }

        public string GetFacePathByIndex(uint faceIndex)
        {
            if (faceIndex >= facePaths.Length)
                return string.Empty;
            return facePaths[faceIndex];
        }

        public string GetSkinPathByIndex(int skinIndex)
        {
            if (skinIndex >= skinPaths.Length)
                return string.Empty;
            return skinPaths[skinIndex];
        }

        public string GetWPPathByIndex(int skinIndex)
        {
            if (skinIndex >= skinPaths.Length)
                return string.Empty;
            return wpPaths[skinIndex];
        }
        [CSVElement("Id")]
        public uint ID;

        public string[] hairPaths;
        [CSVElement("Hair")]
        public string hair
        {
            set
            {
                hairPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);// value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] hairIconPaths;
        [CSVElement("HairIcon")]
        public string hariIcon
        {
            set
            {
                hairIconPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] hairColorPaths;
        [CSVElement("HairColor")]
        public string hairColor
        {
            set
            {
                hairColorPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] hairColorIconPaths;
        [CSVElement("HairColorIcon")]
        public string hairColorIcon
        {
            set
            {
                hairColorIconPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] facePaths;
        [CSVElement("Face")]
        public string face
        {
            set
            {
                facePaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);// value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] faceIconPaths;
        [CSVElement("FaceIcon")]
        public string faceIcon
        {
            set
            {
                faceIconPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] skinPaths;
        [CSVElement("Skin")]
        public string skin
        {
            set
            {
                skinPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }

		public string[] skinIconPaths;
		[CSVElement("SkinIcon")]
		public string skinIcon
		{
			set
			{
				skinIconPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
			}
		}
        
        public string[] wpPaths;
        [CSVElement("WP")]
        public string wp
        {
            set
            {
                wpPaths = value.ToArray(CSVUtil.SYMBOL_SEMICOLON); //value.Split(CSVUtil.SYMBOL_FIRST, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
