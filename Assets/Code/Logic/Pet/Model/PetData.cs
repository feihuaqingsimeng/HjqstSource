using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
namespace Logic.Pet.Model
{
    public class PetData
    {
        private static Dictionary<int, PetData> _petDataDictionary;

        public static Dictionary<int, PetData> GetPetDatas()
        {
            if (_petDataDictionary == null)
            {
                _petDataDictionary = CSVUtil.Parse<int, PetData>("config/csv/pet", "id");
            }
            return _petDataDictionary;
        }


        public static PetData GetPetDataByID(int petId)
        {
            if (_petDataDictionary == null)
                GetPetDatas();
            PetData petData = null;
            if (_petDataDictionary.ContainsKey(petId) && _petDataDictionary[petId] != null)
            {
                petData = _petDataDictionary[petId];
            }
            return petData;
        }

        [CSVElement("id")]
        public int id;

        //public string[] headIcons;
        //[CSVElement("headIcon")]
        //public string headIconStr
        //{
        //    set
        //    {
        //        headIcons = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
        //    }
        //}

        [CSVElement("model")]
        public string modelName;

        public Vector3 scale;
        [CSVElement("scale")]
        public string scaleString
        {
            set
            {
                scale = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("speed")]
        public float speed;

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

        [CSVElement("stay_animation")]
        public string animation;

        public Vector3 homeOffset;
        [CSVElement("home_Offset")]
        public string homeOffsetString
        {
            set
            {
                homeOffset = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Vector3 offset;
        [CSVElement("Offset")]
        public string offsetString
        {
            set
            {
                offset = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
		[CSVElement("head_icon")]
		public string head_icon;
    }
}