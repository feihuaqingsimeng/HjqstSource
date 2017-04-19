using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Avatar.Model;
using Logic.Protocol.Model;
using Logic.Hero.Model;
using Logic.Role.Model;
using Logic.Pet.Model;
using Logic.Player.Model;
using Logic.Enums;
using LuaInterface;

namespace Logic.Player.Model
{
    public class PlayerInfo : RoleInfo
    {
        public PlayerData playerData;
        public AvatarData avatarData;
        public uint hairCutIndex;
        public uint hairColorIndex;
        public uint faceIndex;
        public int skinIndex;
        //        public string name;
        //携带的天赋技能
        public int passiveSkillId;//被动
        public int passiveSkillLevel = 1;
        public int summonEffectId;
        public int summonSkillLevel = 1;
        //激活的被动  id,level
        public Dictionary<int, int> normalPassiveSkilIdDic = new Dictionary<int, int>();

        public PlayerInfo(uint instanceID, uint playerID, uint hairCutIndex, uint hairColorIndex, uint faceIndex, int skinIndex, string playerName)
        {
            this.instanceID = instanceID;
            this.playerData = PlayerData.GetPlayerData(playerID);
            this.heroData = HeroData.GetHeroDataByID(playerData.heroId);
            this.avatarData = AvatarData.GetAvatarData(this.playerData.avatarID);
            this.hairCutIndex = hairCutIndex;
            this.hairColorIndex = hairColorIndex;
            this.faceIndex = faceIndex;
            this.skinIndex = skinIndex;
            //            this.name = playerName;
            this.advanceLevel = (int)heroData.starMin;
        }

        public PlayerInfo(uint playerID)
        {
            this.instanceID = playerID;
            this.playerData = PlayerData.GetPlayerData(playerID);
            this.heroData = HeroData.GetHeroDataByID(playerData.heroId);
            this.avatarData = AvatarData.GetAvatarData(this.playerData.avatarID);
            this.hairCutIndex = 0;
            this.hairColorIndex = 0;
            this.faceIndex = 0;
            this.skinIndex = 0;
            this.advanceLevel = (int)heroData.starMin;
        }
        public PlayerInfo(LuaTable table)
        {
            SetPlayerInfo(table);
        }
        public void SetPlayerInfo(LuaTable table)
        {
            instanceID = table["instanceID"].ToString().ToUInt32();
            LuaTable playerDataTable = (LuaTable)table["playerData"];
            if (playerDataTable != null)
            {
                playerData = PlayerData.GetPlayerData(playerDataTable["id"].ToString().ToUInt32());
                heroData = HeroData.GetHeroDataByID(playerData.heroId);
                avatarData = AvatarData.GetAvatarData(playerData.avatarID);
            }
            hairCutIndex = table["hairCutIndex"].ToString().ToUInt32();
            hairColorIndex = table["hairColorIndex"].ToString().ToUInt32();
            faceIndex = table["faceIndex"].ToString().ToUInt32();
            skinIndex = table["skinIndex"].ToString().ToInt32();
            //			name = table["name"].ToString();
            exp = table["exp"].ToString().ToInt32();
            level = table["level"].ToString().ToInt32();
            advanceLevel = table["advanceLevel"].ToString().ToInt32();
            strengthenLevel = table["strengthenLevel"].ToString().ToInt32();
            strengthenExp = table["strengthenExp"].ToString().ToInt32();
            breakthroughLevel = table["breakthroughLevel"].ToString().ToInt32();
            weaponID = table["weaponID"].ToString().ToInt32();
            armorID = table["armorID"].ToString().ToInt32(); ;
            accessoryID = table["accessoryID"].ToString().ToInt32();
        }
        public PlayerInfo GetPlayerInfoCopy()
        {
            //            PlayerInfo playerInfoCopy = new PlayerInfo(this.instanceID, this.playerData.Id, this.hairCutIndex, this.hairColorIndex, this.faceIndex, this.name);
            PlayerInfo playerInfoCopy = new PlayerInfo(this.instanceID, this.playerData.Id, this.hairCutIndex, this.hairColorIndex, this.faceIndex, this.skinIndex, string.Empty);
            return playerInfoCopy;
        }

        public void SetPlayerInfo(PlayerProtoData playerProtoData)
        {
            if (playerProtoData == null)
                return;
            this.instanceID = (uint)playerProtoData.id;
            if (playerProtoData.modelId != 0)
            {
                this.playerData = PlayerData.GetPlayerData((uint)playerProtoData.modelId);
                this.heroData = HeroData.GetHeroDataByID(playerData.heroId);
            }

            this.avatarData = AvatarData.GetAvatarData(this.playerData.avatarID);
            if (playerProtoData.hairCutId != 0)
                this.hairCutIndex = (uint)playerProtoData.hairCutId;
            if (playerProtoData.hairColorId != 0)
                this.hairColorIndex = (uint)playerProtoData.hairColorId;
            if (playerProtoData.faceId != 0)
                this.faceIndex = (uint)playerProtoData.faceId;
            if (playerProtoData.skinId != 0)
                this.skinIndex = playerProtoData.skinId;
            this.exp = (playerProtoData.exp > 0 || this.level != playerProtoData.lv) ? playerProtoData.exp : this.exp;
            this.level = playerProtoData.lv != 1 ? playerProtoData.lv : this.level;
            if (playerProtoData.star != 0)
                this.advanceLevel = playerProtoData.star;
            if (playerProtoData.aggrLv != -1)
                this.strengthenLevel = playerProtoData.aggrLv;
            if (playerProtoData.aggrExp != -1)
                this.strengthenExp = playerProtoData.aggrExp;
            if (playerProtoData.breakLayer != 0)
                this.breakthroughLevel = playerProtoData.breakLayer;
        }
        public void UpdateNormalActiveSkillTalent(Dictionary<int, int> talentDic)
        {
            if (talentDic.Count == 0)
                return;
            normalPassiveSkilIdDic.Clear();
            normalPassiveSkilIdDic.AddRange(talentDic);
        }
        public void UpdateSelectSkillTalent(int passiveSkillId, int passiveSkillLevel, int summonId, int summonSkillLevel)
        {
            this.passiveSkillId = passiveSkillId;
            this.passiveSkillLevel = passiveSkillLevel;
            this.summonEffectId = summonId;
            this.summonSkillLevel = summonSkillLevel;
        }
        public void UpdateSkillTalentByProtocol(List<TalentProto> talents, List<int> selected)
        {
            normalPassiveSkilIdDic.Clear();
            PlayerSkillTalentData data;
            for (int i = 0, count = talents.Count; i < count; i++)
            {
                TalentProto proto = talents[i];
                data = PlayerSkillTalentData.GetSkillTalentDataByID(proto.no);
                if (proto.lv >= 1 && data.groupType == Logic.Enums.PlayerSkillTalentType.PassiveNormal)
                {
                    normalPassiveSkilIdDic[data.effect] = proto.lv;
                }
                if (selected.Contains(proto.no))
                {
                    if (data.groupType == PlayerSkillTalentType.PassiveThreeChoiceOne)
                    {
                        this.passiveSkillId = data.effect;
                        this.passiveSkillLevel = proto.lv;
                    }
                    else if (data.groupType == PlayerSkillTalentType.SummonThreeChoiceOne)
                    {
                        this.summonEffectId = data.effect;
                        this.summonSkillLevel = proto.lv;
                    }
                }
            }
        }
        public PlayerInfo(PlayerProtoData playerProtoData)
        {
            this.playerData = PlayerData.GetPlayerData((uint)playerProtoData.modelId);
            this.heroData = HeroData.GetHeroDataByID(playerData.heroId);
            this.advanceLevel = (int)heroData.starMin;
            //            this.name = PlayerProxy.instance.PlayerName;
            //			this.faceIndex = (uint)playerProtoData.faceId;
            //			this.hairCutIndex = (uint)playerProtoData.hairCutId;
            //			this.hairColorIndex = (uint)playerProtoData.hairColorId;
            SetPlayerInfo(playerProtoData);

        }
        public PlayerInfo(TeamPlayerProtoData data)
        {
            instanceID = (uint)data.id;
            this.playerData = PlayerData.GetPlayerData((uint)data.playerNo);
            this.heroData = HeroData.GetHeroDataByID(playerData.heroId);
            this.avatarData = AvatarData.GetAvatarData(this.playerData.avatarID);
            advanceLevel = data.star;
            if (data.lv != 1)
                level = data.lv;
            if (data.aggrLv != -1)
                strengthenLevel = data.aggrLv;
            if (data.breakLayer != 0)
                breakthroughLevel = data.breakLayer;
        }
        public override int modelDataId
        {
            get
            {
                return (int)playerData.Id;
            }
        }
        public override int MaxAdvanceLevel
        {
            get
            {
                return (int)heroData.starMax;
            }
        }

        public string PetHeadIcon
        {
            get
            {
                PetData data = PetData.GetPetDataByID(playerData.pet_id);
                return Common.ResMgr.ResPath.GetCharacterHeadIconPath(data.head_icon);
            }
        }
    }
}
