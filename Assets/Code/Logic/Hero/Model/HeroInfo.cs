using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Equipment.Model;
using Logic.Role.Model;
using LuaInterface;
using Logic.Game.Model;

namespace Logic.Hero.Model
{
    public class HeroInfo : RoleInfo
    {
        //public uint instanceID;
        //public HeroData heroData;
        public List<RoleAttribute> heroAttributes;

        //public int breakthroughLevel = 0;
        //public int strengthenLevel = 0;
        //public int advanceLevel = 0;
        //public int level = 1;

        //public int exp = 0;
        //public int strengthenExp = 0;
        //public int weaponID = 0;
        //public int armorID = 0;
        //public int accessoryID = 0;  

        public HeroInfo(uint instanceID, int heroDataID, int breakthroughLevel, int strengthenLevel, int advanceLevel, int level = 1)
        {
            this.instanceID = instanceID;
            this.heroData = HeroData.GetHeroDataByID(heroDataID);
            if (this.heroData == null)
                Debugger.LogError("heroData is null,can't find id :" + heroDataID);
            this.breakthroughLevel = breakthroughLevel;
            this.strengthenLevel = strengthenLevel;
            this.advanceLevel = advanceLevel;
            this.level = level;
            UpdateHeroAttribute();

        }

        public HeroInfo GetHeroInfoCopy()
        {
            HeroInfo heroInfoCopy = new HeroInfo(this.instanceID, this.heroData.id, this.breakthroughLevel, this.strengthenLevel, this.advanceLevel, this.level);
            return heroInfoCopy;
        }

        public HeroInfo(HeroProtoData heroProtoData)
        {
            SetHeroInfo(heroProtoData);
        }

		public HeroInfo (LuaTable heroInfoLuaTable)
		{
			this.instanceID = heroInfoLuaTable["instanceID"].ToString().ToUInt32();
			LuaTable heroDataLuaTable = (LuaTable)heroInfoLuaTable["heroData"];
			if(heroDataLuaTable == null)
				Debugger.LogError("heroData is null,can't find ");
			this.heroData = HeroData.GetHeroDataByID(heroDataLuaTable["id"].ToString().ToInt32());
			this.breakthroughLevel = heroInfoLuaTable["breakthroughLevel"].ToString().ToInt32();
			this.strengthenLevel = heroInfoLuaTable["strengthenLevel"].ToString().ToInt32();
			this.advanceLevel = heroInfoLuaTable["advanceLevel"].ToString().ToInt32();
			this.level = heroInfoLuaTable["level"].ToString().ToInt32();

			this.exp = heroInfoLuaTable["exp"].ToString().ToInt32();
			this.strengthenExp = heroInfoLuaTable["strengthenExp"].ToString().ToInt32();

			this.weaponID = heroInfoLuaTable["weaponID"].ToString().ToInt32();
			this.armorID = heroInfoLuaTable["armorID"].ToString().ToInt32();
			this.accessoryID = heroInfoLuaTable["accessoryID"].ToString().ToInt32();
		}

        public HeroInfo(DrawCardDropProto drawDropCardProto)
        {
            this.instanceID = 0;
            this.heroData = HeroData.GetHeroDataByID(drawDropCardProto.no);
            this.breakthroughLevel = 0;
            this.strengthenLevel = 0;
            this.advanceLevel = drawDropCardProto.star;
            this.level = 1;
        }
        public HeroInfo(TeamHeroProtoData data)
        {
            this.instanceID = (uint)data.id;
            heroData = HeroData.GetHeroDataByID(data.heroNo);
            if (data.lv != 1)
                level = data.lv;
            if (data.star != 0)
                advanceLevel = data.star;
            if (data.aggrLv != -1)
                strengthenLevel = data.aggrLv;
            if (data.breakLayer != 0)
                breakthroughLevel = data.breakLayer;
        }
        //		public HeroInfo (BuyHeroInfo buyHeroInfo)
        //		{
        //			this.instanceID = 0;
        //			this.heroData = HeroData.GetHeroDataByID(buyHeroInfo.modelId);
        //			this.advanceLevel = buyHeroInfo.star;
        //			UpdateHeroAttribute();
        //		}

        public HeroInfo(int heroId)
        {
            this.instanceID = (uint)heroId;
            heroData = HeroData.GetHeroDataByID(heroId);
            this.advanceLevel = 1;
            UpdateHeroAttribute();
        }

        public HeroInfo(HeroData heroData)
        {
            this.heroData = heroData;
            this.advanceLevel = 1;//召唤兽默认用1阶模型
            UpdateHeroAttribute();
        }

		public HeroInfo (GameResData gameResData)
		{
			this.heroData = HeroData.GetHeroDataByID(gameResData.id);
			this.advanceLevel = gameResData.star;
			UpdateHeroAttribute();
		}

        public void SetHeroInfo(HeroProtoData heroProtoData)
        {
            this.instanceID = (uint)heroProtoData.id;
            if (heroProtoData.modelId != 0)
                this.heroData = HeroData.GetHeroDataByID(heroProtoData.modelId);
            if (heroProtoData.breakLayer != 0)
            {
                this.breakthroughLevel = heroProtoData.breakLayer;
            }
            if (heroProtoData.aggrLv != -1)
            {
                this.strengthenLevel = heroProtoData.aggrLv;
            }
            if (heroProtoData.star != 0)
            {
                this.advanceLevel = heroProtoData.star;
            }
            if (heroProtoData.exp != -1)
            {
                this.exp = heroProtoData.exp;
            }
            if (heroProtoData.lv != 0)
            {
                this.level = heroProtoData.lv;
            }
            if (heroProtoData.aggrExp != -1)
            {
                this.strengthenExp = heroProtoData.aggrExp;
            }
            UpdateHeroAttribute();

        }

        private void UpdateHeroAttribute()
        {
            heroAttributes = HeroUtil.CalcHeroAttributes(this);
        }
        public override int modelDataId
        {
            get
            {
                return heroData.id;
            }
        }

        public override int MaxAdvanceLevel
        {
            get
            {
                return (int)heroData.starMax;
            }
        }

    }
}