using UnityEngine;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
using Logic.Enums;
namespace Logic.Transforms.Model
{
    public class HeroTransformData
    {
        private static Dictionary<int, HeroTransformData> _heroTransformDataDictionary;
        public static Dictionary<int, HeroTransformData> GetHeroTransformDataDicionary()
        {
            if (_heroTransformDataDictionary == null)
            {
                _heroTransformDataDictionary = CSVUtil.Parse<int, HeroTransformData>("config/csv/hero_transform", "id");
            }
            return _heroTransformDataDictionary;
        }

        public static Dictionary<int, HeroTransformData> HeroTransformDataDictionary
        {
            get
            {
                return GetHeroTransformDataDicionary();
            }
        }

        public static HeroTransformData GetHeroTransformDataByID(int id)
        {
            if (HeroTransformDataDictionary != null && HeroTransformDataDictionary.ContainsKey(id))
            {
                return HeroTransformDataDictionary[id];
            }
            return null;
        }

        [CSVElement("id")]
        public int id;

        [CSVElement("type")]
        public int heroTransformTypeInt
        {
            set
            {
                heroTransformType = (HeroTransformType)value;
            }
        }
        public HeroTransformType heroTransformType;

        [CSVElement("duration")]
        public float duration;

        [CSVElement("scale")]
        public string scaleStr
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    scale = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
                }
            }
        }
        public Vector3 scale;

        [CSVElement("color")]
        public string colorStr
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    color = value.ToColor(CSVUtil.SYMBOL_SEMICOLON);
                }
            }
        }
        public Color color;

        [CSVElement("change_time")]
        public float changeTime;

        [CSVElement("hero_id")]
        public uint heroId;

        [CSVElement("back_hero_id")]
        public uint backHeroId;

        [CSVElement("animation_name")]
        public string animationName;

        [CSVElement("back_animation_name")]
        public string backAnimationName;

        [CSVElement("skill_id")]
        public string skillIdStr
        {
            set
            {
                skillIds = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
        public uint[] skillIds;

        [CSVElement("back_skill_id")]
        public string backSkillIdStr
        {
            set
            {
                backSkillIds = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
        public uint[] backSkillIds;

        [CSVElement("backable")]
        public bool backable;

    }
}