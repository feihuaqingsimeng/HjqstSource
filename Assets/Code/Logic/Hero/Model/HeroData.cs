using UnityEngine;
using System.Collections.Generic;
using Common.Util;
using Common.ResMgr;
using Logic.Enums;

namespace Logic.Hero.Model
{
    public class HeroData
    {
        private static Dictionary<int, HeroData> _heroDataDictionary;

        public static Dictionary<int, HeroData> GetHeroDatas()
        {
            if (_heroDataDictionary == null)
            {
                _heroDataDictionary = CSVUtil.Parse<int, HeroData>("config/csv/hero", "id");
            }
            return _heroDataDictionary;
        }

        public static Dictionary<int, HeroData> HeroDataDictionary
        {
            get
            {
                if (_heroDataDictionary == null)
                {
                    GetHeroDatas();
                }
                return _heroDataDictionary;
            }
        }

        public static HeroData GetHeroDataByID(int heroID)
        {
            HeroData heroData = null;
            if (HeroDataDictionary.ContainsKey(heroID) && HeroDataDictionary[heroID] != null)
            {
                heroData = HeroDataDictionary[heroID];
            }
            return heroData;
        }
		public bool IsPlayer()
		{
			return hero_type == 2;
		}
		
        [CSVElement("id")]
        public int id;

        [CSVElement("hero_type")]
        public int hero_type;

        [CSVElement("hitId")]
        public uint hitId;

        [CSVElement("skillId1")]
        public uint skillId1;

        [CSVElement("skillId2")]
        public uint skillId2;

        [CSVElement("passiveId1")]
        public uint passiveId1;

        [CSVElement("passiveId2")]
        public uint passiveId2;

        [CSVElement("name")]
        public string name;

        [CSVElement("description")]
        public string description;

        public string[] modelNames;
        public string defaultModelName;
        [CSVElement("model")]
        public string model
        {
            set
            {
                modelNames = value.Split(CSVUtil.SYMBOL_SEMICOLON);
                defaultModelName = modelNames[0];
            }
        }

        [CSVElement("height")]
        public float height;

        public ShadowType shadowType;
        [CSVElement("shadows")]
        public int shadowsInt
        {
            set
            {
                shadowType = (ShadowType)value;
            }
        }

        public Vector3 homeRotation;
        [CSVElement("home_rotation")]
        public string homeRotationString
        {
            set
            {
                homeRotation = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Vector3 rotation;
        [CSVElement("rotation")]
        public string rotationString
        {
            set
            {
                rotation = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Vector3 scale;
        [CSVElement("scale")]
        public string scaleString
        {
            set
            {
                scale = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public string[] headIcons;
        [CSVElement("headIcon")]
        public string headIcon
        {
            set
            {
                headIcons = value.Split(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("expMax")]
        public uint expMax;

        [CSVElement("expMax50")]
        public uint expMax50;

        [CSVElement("hp")]
        public uint HP;

        [CSVElement("hp_add")]
        public uint hpAdd;

        [CSVElement("normal_atk")]
        public uint normalAtk;

        [CSVElement("normal_atk_add")]
        public uint normalAtkAdd;

        [CSVElement("normal_def")]
        public uint normalDef;

        [CSVElement("normal_def_add")]
        public uint normalDefAdd;

        [CSVElement("magic_atk")]
        public uint magicAtk;

        [CSVElement("magic_atk_add")]
        public uint magicAtkAdd;

        [CSVElement("magic_def")]
        public uint magicDef;

        [CSVElement("magic_def_add")]
        public uint magicDefAdd;

        [CSVElement("speed")]
        public uint speed;

        [CSVElement("hit")]
        public float hit;

        [CSVElement("dodge")]
        public float dodge;

        [CSVElement("crit")]
        public float crit;

        [CSVElement("anti_crit")]
        public float antiCrit;

        [CSVElement("block")]
        public float block;

        [CSVElement("anti_block")]
        public float antiBlock;

        [CSVElement("counter_atk")]
        public float counterAtk;

        [CSVElement("crit_hurt_add")]
        public float critHurtAdd;

        [CSVElement("crit_hurt_dec")]
        public float critHurtDec;

        [CSVElement("armor")]
        public float armor;

        [CSVElement("damage_dec")]
        public float damageDec;

        [CSVElement("damage_add")]
        public float damageAdd;

        [CSVElement("starMin")]
        public uint starMin;

        [CSVElement("starMax")]
        public uint starMax;

        [CSVElement("correction")]
        public int correction;//战力修正值

        public RoleType roleType;
        [CSVElement("type")]
        public int type
        {
            set
            {
                roleType = (RoleType)value;
            }
        }

        [CSVElement("weaponType")]
        public uint weaponType;

        [CSVElement("floatable")]
        public bool floatable;

        public List<uint[]> bornEffectIds;
        [CSVElement("born_effect")]
        public string bornEffectIdStr
        {
            set
            {
                bornEffectIds = new List<uint[]>();
                string[] strs = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, length = strs.Length; i < length; i++)
                {
                    uint[] effects = strs[i].ToArray<uint>(CSVUtil.SYMBOL_PIPE);
                    bornEffectIds.Add(effects);
                }
            }
        }

        [CSVElement("audio_attack")]
        public int audioAttack;

        [CSVElement("audio_under_attack")]
        public int audioUnderAttack;

        [CSVElement("audio_die")]
        public int audioDie;

        [CSVElement("effect_die")]
        public int dieEffectId;

		public RoleQuality roleQuality = RoleQuality.White;
		[CSVElement("quality")]
		public int quality
		{
			set
			{
				roleQuality = (RoleQuality)value;
			}
		}
		[CSVElement("quality_attr")]
		public float quality_attr;
		public string NameWithQualityColor
		{
			get
			{
				return UI.UIUtil.FormatStringWithinQualityColor(this.roleQuality, Common.Localization.Localization.Get(this.name));
			}
		}
    }
}