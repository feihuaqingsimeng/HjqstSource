using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Net.Controller;
using Logic.Character.Model;
using Logic.Hero.Model;
using Logic.Skill.Model;
using Logic.Fight.Controller;
using Logic.Enums;
using Logic.Player.Model;
using Common.Animators;
using Logic.Fight.Model;
using Logic.Shaders;
using Logic.UI.HPBar.View;
using Logic.Formation.Model;
using LuaInterface;
using Logic.Skill;
namespace Logic.Character.Controller
{
    public class PlayerController : SingletonMono<PlayerController>
    {
        private Dictionary<uint, HeroEntity> _heroDic = new Dictionary<uint, HeroEntity>();
        private List<HeroEntity> _heros = new List<HeroEntity>();
        private Dictionary<uint, HeroEntity> _comboHeroDic = new Dictionary<uint, HeroEntity>();
        private Dictionary<uint, HeroEntity> _deadHeroDic = new Dictionary<uint, HeroEntity>();
        private List<uint> _deadHeroCheckedList = new List<uint>();
        private List<string> _deadHeroTombStones = new List<string>();
        private Dictionary<uint, HPBarView> _hpBarViewDic = new Dictionary<uint, HPBarView>();
        private Queue<HPBarView> _deadHpBarViewQueue = new Queue<HPBarView>();
        private List<HeroEntity> _aeonList = new List<HeroEntity>();
        private Dictionary<uint, HeroInfo> _heroInfoDic = new Dictionary<uint, HeroInfo>();
        private List<BuffType> _limitBuffs = new List<BuffType>();
        private List<SkillInfo> _skillInfos = new List<SkillInfo>();
        private PlayerInfo _playerInfo;
        private Transform _hpBoxTrans;
        private static Vector3 ROTATE = new Vector3(0, 90, 0);
        private bool _allDead = false;
        private int _deadCount = 0;
        private int _heroCount = 0;
        private bool _addedPlayerFormationBuff;
        private int _preloadCount;
        private int _preloadTotal;
        private float _offset;
        private bool _playerDead;
        public bool playerDead { get { return _playerDead; } }
        private int _fightCount = 0;

        #region fight imitate
#if UNITY_EDITOR
        [NoToLua]
        public Dictionary<FormationPosition, HeroInfo> imitateHeroInfoDic = new Dictionary<FormationPosition, HeroInfo>();
        [NoToLua]
        public Dictionary<FormationPosition, PlayerInfo> imitatePlayerInfoDic = new Dictionary<FormationPosition, PlayerInfo>();

        [NoToLua]
        public void InitImitateData()
        {
            imitateHeroInfoDic.Clear();
            imitatePlayerInfoDic.Clear();
        }

        [NoToLua]
        public HeroInfo GetImitateHeroInfo(int instanceId)
        {
            foreach (var kvp in imitateHeroInfoDic)
            {
                if (kvp.Value.instanceID == instanceId)
                    return kvp.Value;
            }
            return null;
        }
#endif
        #endregion

        #region attr
        public Dictionary<uint, HeroEntity> heroDic
        {
            get
            {
                return _heroDic;
            }
        }

        public List<HeroEntity> heros
        {
            get
            {
                return _heros;
            }
        }

        public Dictionary<uint, HeroEntity> deadHeroDic
        {
            get
            {
                return _deadHeroDic;
            }
        }

        public List<int> DeadHeroIdList
        {
            get
            {
                List<int> deadHeroIDList = new List<int>();
                List<uint> originalDeadHeroIDList = deadHeroDic.GetKeys();
                int deadHeroCount = originalDeadHeroIDList.Count;
                for (int i = 0; i < deadHeroCount; i++)
                {
                    deadHeroIDList.Add((int)originalDeadHeroIDList[i]);
                }
                return deadHeroIDList;
            }
        }

        public List<SkillInfo> skillInfos
        {
            get
            {
                return _skillInfos;
            }
        }

        private bool _tickCD;
        public bool tickCD
        {
            get
            {
                return _tickCD;
            }
            set
            {
                _tickCD = value;
                for (int i = 0, count = heros.Count; i < count; i++)
                {
                    heros[i].tickCD = value;
                }
            }
        }

        private float _lastTickTime;
        public float lastTickTime
        {
            set
            {
                _lastTickTime = value;
                for (int i = 0, count = heros.Count; i < count; i++)
                {
                    heros[i].lastTickTime = value;
                }
            }
            get
            {
                return _lastTickTime;
            }
        }

#if UNITY_EDITOR
        [NoToLua]
        public List<string> deadHeroTombStones
        {
            get
            {
                return _deadHeroTombStones;
            }
        }
#endif

        public Dictionary<uint, HeroEntity> comboHeroDic
        {
            get
            {
                return _comboHeroDic;
            }
        }
        #endregion

        public HeroEntity GetComboHero(uint id)
        {
            if (comboHeroDic.ContainsKey(id))
                return comboHeroDic[id];
            return null;
        }

        public HeroInfo GetHeroInfo(uint id)
        {
            if (_heroInfoDic.ContainsKey(id))
                return _heroInfoDic[id];
            return null;
        }

        public PlayerInfo GetPlayerInfo()
        {
            return _playerInfo;
        }

        void Awake()
        {
            instance = this;
            _limitBuffs.Add(BuffType.Swimmy);
            _limitBuffs.Add(BuffType.Frozen);
            _limitBuffs.Add(BuffType.Sleep);
            _limitBuffs.Add(BuffType.Landification);
            _limitBuffs.Add(BuffType.Tieup);
            _limitBuffs.Add(BuffType.Tag);
        }

        private void PreLoadHero(bool flag)
        {
            if (flag)
                _preloadCount++;
            if (_preloadCount == _preloadTotal)
            {
                switch (FightController.instance.fightType)
                {
                    case FightType.PVE:
                    case FightType.DailyPVE:
                    case FightType.Expedition:
                    case FightType.WorldTree:
                    case FightType.WorldBoss:
                    case FightType.PVP:
                    case FightType.FirstFight:
#if UNITY_EDITOR
                    case FightType.Imitate:
#endif
                        {
                            Logic.UI.SkillBar.View.SkillBarView skillBarView = Logic.UI.SkillBar.View.SkillBarView.Open();
                            skillBarView.showMask = false;
                        }
                        break;
                    case FightType.ConsortiaFight:
                    case FightType.FriendFight:
                    case FightType.Arena:
                    case FightType.MineFight:
                        {
                            Logic.UI.SkillBar.View.SkillBarView skillBarView = Logic.UI.SkillBar.View.SkillBarView.Open();
                            skillBarView.showMask = true;
                        }
                        break;
                    case FightType.SkillDisplay:
                        break;
                }
                SortSkillInfos();
                DataMessageHandler.DataMessage_InitOK(CharacterType.Player);
            }
        }

        #region sort skill
        public void SortSkillInfos()
        {
            _skillInfos.Clear();
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                if (hero.characterInfo.skillInfo1 != null)
                    _skillInfos.Add(hero.characterInfo.skillInfo1);
                if (hero.characterInfo.skillInfo2 != null)
                    _skillInfos.Add(hero.characterInfo.skillInfo2);
            }
            _skillInfos.Sort(CharacterUtil.SortSkill);
            //for (int i = 0, count = _skillInfos.Count; i < count; i++)
            //{
            //    SkillInfo skillInfo = _skillInfos[i];
            //    MechanicsType mechanicsType = Logic.Skill.SkillUtil.GetSkillMechanicsType(skillInfo);
            //Debugger.Log("make an appointment skill order:{0} and attackableType:{1}", mechanicsType.ToString(), skillInfo.skillData.attackableType.ToString());
            //}
        }

        #endregion

        public void CreateHeros()
        {
            _preloadCount = 0;
            if (!_hpBoxTrans)
            {
                GameObject go = new GameObject();
                go.name = "hero_hp_box";
                _hpBoxTrans = go.transform;
                _hpBoxTrans.SetParent(UI.UIMgr.instance.basicCanvas.transform, false);
            }
            _allDead = false;
            _playerDead = false;
            _heroCount = 0;
            _deadCount = 0;
            _offset = 10f;
            switch (FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.FirstFight:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.ConsortiaFight:
                case FightType.MineFight:
                    {
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        _preloadTotal = fightHeroInfos.Count + 2;
                        _heroCount = fightHeroInfos.Count + 1;
                        CreatePlayer(FightProxy.instance.fightPlayerInfo, PreLoadHero);
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            CreateHero(fightHeroInfos[i], PreLoadHero);
                        }
                    }
                    break;
                case FightType.Expedition:
                    {
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        _preloadTotal = fightHeroInfos.Count;
                        _heroCount = fightHeroInfos.Count;
                        if (FightProxy.instance.fightPlayerInfo != null)
                        {
                            _preloadTotal += 2;
                            _heroCount += 1;
                            CreatePlayer(FightProxy.instance.fightPlayerInfo, PreLoadHero);
                        }
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            CreateHero(fightHeroInfos[i], PreLoadHero);
                        }
                    }
                    break;
                case FightType.SkillDisplay:
                    {
                        _offset = 0f;
                        List<FightHeroInfo> fightHeroInfos = FightProxy.instance.fightHeroInfoList;
                        _preloadTotal = fightHeroInfos.Count;
                        _heroCount = fightHeroInfos.Count;
                        if (FightProxy.instance.fightPlayerInfo != null)
                        {
                            _preloadTotal += 2;
                            _heroCount += 1;
                            CreatePlayer(FightProxy.instance.fightPlayerInfo, PreLoadHero);
                        }
                        for (int i = 0, count = fightHeroInfos.Count; i < count; i++)
                        {
                            CreateHero(fightHeroInfos[i], PreLoadHero);
                        }
                    }
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    _heroCount = imitatePlayerInfoDic.Count + imitateHeroInfoDic.Count;
                    StartCoroutine(InitHerosCoroutine());
                    break;
#endif
            }
        }

#if UNITY_EDITOR
        private IEnumerator InitHerosCoroutine()
        {
            #region fight imitate
            foreach (var kvp in imitatePlayerInfoDic)
            {
                CreateImitatePlayer(kvp);
                PreCreateAeon(kvp.Value);
                yield return null;
            }
            foreach (var kvp in imitateHeroInfoDic)
            {
                CreateImitateHero(kvp);
                yield return null;
            }
            Logic.UI.SkillBar.View.SkillBarView.Open();
            SortSkillInfos();
            DataMessageHandler.DataMessage_InitOK(CharacterType.Player);
            #endregion
        }
#endif

        public void CloneHero(uint id)
        {
            StartCoroutine(CloneHeroCoroutine(id));
        }

        private IEnumerator CloneHeroCoroutine(uint id)
        {
            while (Common.GameTime.Controller.TimeController.instance.playerPause)
            {
                yield return null;
            }
            List<uint> keys = _heroDic.GetKeys();
            for (int i = 0, count = keys.Count; i < count; i++)
            {
                uint key = keys[i];
                if (key == id)
                {
                    HeroEntity hero = this[key];
                    if (hero is PlayerEntity)
                        ClonePlayer(hero as PlayerEntity);
                    else
                        CloneHero(hero);
                }
            }
        }

        private void CreatePlayer(FightPlayerInfo fightPlayerInfo, System.Action<bool> callback)
        {
            if (fightPlayerInfo == null)
            {
                if (callback != null)
                    callback(false);
                return;
            }
            PlayerInfo playerInfo = fightPlayerInfo.playerInfo;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)fightPlayerInfo.pvePlayerProtoData.posIndex);
            if (FightController.instance.fightStatus == FightStatus.Normal)
                pos = pos - new Vector3(_offset, 0, 0);
            PlayerEntity.CreatePlayerEntity(playerInfo, Map.Controller.MapController.instance.transform, (player) =>
            {
                if (player == null)
                {
                    if (callback != null)
                        callback(false);
                    return;
                }
                ShadersUtil.SetShaderKeyword(player, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                player.characterName = playerInfo.heroData.name;
                player.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);

                player.positionId = (uint)fightPlayerInfo.pvePlayerProtoData.posIndex;
                player.pos = pos;
                player.eulerAngles = playerInfo.heroData.rotation;
                player.scale = playerInfo.heroData.scale;
                player.anim.transform.localPosition = Vector3.zero;
                player.anim.transform.localEulerAngles = Vector3.zero;
                player.anim.transform.localScale = Vector3.one;
                player.height = playerInfo.heroData.height;
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);//Ö÷½ÇÔÝÊ±Ð´ËÀ
                Transform root = player.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                player.SetAttr(fightPlayerInfo.pvePlayerProtoData.attr);
                heroDic.Add(playerInfo.instanceID, player);
                heros.Add(player);
                _playerInfo = playerInfo;
                #region create hp bar
                HPBarView hpBarView = GetHPBarView(player);
                hpBarView.transform.SetParent(_hpBoxTrans, false);
                _hpBarViewDic.Add(playerInfo.instanceID, hpBarView);
                player.hpBarView = hpBarView;
                if (callback != null)
                    callback(true);
                #endregion
                PreCreateAeon(player.characterInfo, PreLoadHero);
            });
        }

        private void ClonePlayer(PlayerEntity playerEntity)
        {
            FightPlayerInfo fightPlayerInfo = FightProxy.instance.fightPlayerInfo;
            PlayerInfo playerInfo = fightPlayerInfo.playerInfo;
            if (comboHeroDic.ContainsKey(playerInfo.instanceID))
            {
                HeroEntity player = comboHeroDic[playerInfo.instanceID];
                player.anim.gameObject.SetActive(true);
                if (player is PlayerEntity)
                    (player as PlayerEntity).petEntity.gameObject.SetActive(true);
                Common.Util.TransformUtil.SwitchLayer(player.transform, (int)LayerType.None);
            }
            else
            {
                Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)fightPlayerInfo.pvePlayerProtoData.posIndex);
                PlayerEntity player = PlayerEntity.CreatePlayerEntity(playerInfo, Map.Controller.MapController.instance.transform);
                ShadersUtil.SetShaderKeyword(player, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                if (player == null) return;
                player.characterName = playerInfo.heroData.name;
                player.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);
                player.positionId = (uint)fightPlayerInfo.pvePlayerProtoData.posIndex;
                player.pos = pos;
                player.eulerAngles = playerInfo.heroData.rotation;
                player.scale = playerInfo.heroData.scale;
                player.anim.transform.localPosition = Vector3.zero;
                player.anim.transform.localEulerAngles = Vector3.zero;
                player.anim.transform.localScale = Vector3.one;
                player.height = playerInfo.heroData.height;
                //player.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);
                Transform root = player.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                player.CloneAttr(playerEntity);
                player.characterInfo = playerEntity.characterInfo;
                player.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                comboHeroDic.Add(playerInfo.instanceID, player);
                Common.Util.TransformUtil.SwitchLayer(player.transform, (int)LayerType.None);
            }
        }

        private void CreateHero(FightHeroInfo fightHeroInfo, System.Action<bool> callback)
        {
            if (fightHeroInfo == null)
            {
                if (callback != null)
                    callback(false);
                return;
            }
            HeroInfo heroInfo = fightHeroInfo.heroInfo;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)fightHeroInfo.pveHeroProtoData.posIndex);
            pos = pos - new Vector3(_offset, 0, 0);
            HeroEntity.CreateHeroEntity(heroInfo, (hero) =>
            {
                if (hero == null)
                {
                    if (callback != null)
                        callback(false);
                    return;
                }
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                hero.characterName = heroInfo.heroData.name;
                hero.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                Transform root = hero.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                hero.positionId = (uint)fightHeroInfo.pveHeroProtoData.posIndex;
                hero.pos = pos;
                hero.eulerAngles = ROTATE;
                hero.scale = heroInfo.heroData.scale;
                hero.anim.transform.localPosition = Vector3.zero;
                hero.anim.transform.localEulerAngles = Vector3.zero;
                hero.anim.transform.localScale = Vector3.one;
                hero.height = heroInfo.heroData.height;
                hero.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                hero.SetAttr(fightHeroInfo.pveHeroProtoData.attr);
                heroDic.Add(heroInfo.instanceID, hero);
                heros.Add(hero);
                _heroInfoDic.Add(heroInfo.instanceID, heroInfo);
                #region create hp bar
                HPBarView hpBarView = GetHPBarView(hero);
                hpBarView.transform.SetParent(_hpBoxTrans, false);
                _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
                hero.hpBarView = hpBarView;
                #endregion
                if (callback != null)
                    callback(true);
            });
        }

        private void CloneHero(HeroEntity heroEntity)
        {
            FightHeroInfo fightHeroInfo = FightProxy.instance.GetFightHeroInfoById(heroEntity.characterInfo.instanceID);
#if UNITY_EDITOR
            HeroInfo heroInfo = null;
            if (fightHeroInfo != null)
            {
                heroInfo = fightHeroInfo.heroInfo;
            }
            else
            {
                if (FightController.imitate)
                    heroInfo = PlayerController.instance.GetImitateHeroInfo((int)heroEntity.characterInfo.instanceID);
            }
#else
            HeroInfo heroInfo = fightHeroInfo.heroInfo;
#endif
            if (comboHeroDic.ContainsKey(heroInfo.instanceID))
            {
                HeroEntity hero = comboHeroDic[heroInfo.instanceID];
                hero.anim.gameObject.SetActive(true);
                Common.Util.TransformUtil.SwitchLayer(hero.transform, (int)LayerType.None);
            }
            else
            {
                Vector3 pos = Logic.Position.Model.PositionData.GetPos(heroEntity.positionId);
                HeroEntity hero = HeroEntity.CreateHeroEntity(heroInfo);
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                if (hero == null) return;
                hero.characterName = heroInfo.heroData.name;
                hero.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                Transform root = hero.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                hero.positionId = heroEntity.positionId;
                hero.pos = pos;
                hero.eulerAngles = ROTATE;
                hero.scale = heroInfo.heroData.scale;
                hero.anim.transform.localPosition = Vector3.zero;
                hero.anim.transform.localEulerAngles = Vector3.zero;
                hero.anim.transform.localScale = Vector3.one;
                hero.height = heroInfo.heroData.height;
                hero.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                hero.CloneAttr(heroEntity);
                hero.characterInfo = heroEntity.characterInfo;
                hero.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                comboHeroDic.Add(heroInfo.instanceID, hero);
                Common.Util.TransformUtil.SwitchLayer(hero.transform, (int)LayerType.None);
            }
        }

        #region fight imitate
#if UNITY_EDITOR
        private void CreateImitatePlayer(KeyValuePair<FormationPosition, PlayerInfo> kvp)
        {
            PlayerInfo playerInfo = kvp.Value;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)kvp.Key);
            if (FightController.instance.fightStatus == FightStatus.Normal)
                pos = pos - new Vector3(10, 0, 0);
            PlayerEntity player = PlayerEntity.CreatePlayerEntity(playerInfo, Map.Controller.MapController.instance.transform);
            ShadersUtil.SetShaderKeyword(player, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            if (player == null) return;
            player.characterName = playerInfo.heroData.name;
            player.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);

            player.positionId = (uint)kvp.Key;
            player.pos = pos;
            player.eulerAngles = playerInfo.heroData.rotation;
            player.scale = playerInfo.heroData.scale;
            player.anim.transform.localPosition = Vector3.zero;
            player.anim.transform.localEulerAngles = Vector3.zero;
            player.anim.transform.localScale = Vector3.one;
            player.height = playerInfo.heroData.height;
            //player.transform.SetParent(Map.Controller.MapController.instance.transform, false);
            #region shadow
            Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);
            Transform root = player.rootNode.transform;
            Map.Controller.MapController.instance.AddTarget(root, size);
            #endregion
            player.SetAttr(playerInfo);
            heroDic.Add(playerInfo.instanceID, player);
            heros.Add(player);
            #region create hp bar
            HPBarView hpBarView = GetHPBarView(player);
            hpBarView.transform.SetParent(_hpBoxTrans, false);
            _hpBarViewDic.Add(playerInfo.instanceID, hpBarView);
            _playerInfo = playerInfo;
            player.hpBarView = hpBarView;
            #endregion
        }

        private void CreateImitateHero(KeyValuePair<FormationPosition, HeroInfo> kvp)
        {
            HeroInfo heroInfo = kvp.Value;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)kvp.Key);
            pos = pos - new Vector3(10, 0, 0);
            HeroEntity hero = HeroEntity.CreateHeroEntity(heroInfo);
            ShadersUtil.SetShaderKeyword(hero, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            if (hero == null) return;
            hero.characterName = heroInfo.heroData.name;
            hero.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
            #region shadow
            Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
            Transform root = hero.rootNode.transform;
            Map.Controller.MapController.instance.AddTarget(root, size);
            #endregion
            hero.positionId = (uint)kvp.Key;
            hero.pos = pos;
            hero.eulerAngles = ROTATE;
            hero.scale = heroInfo.heroData.scale;
            hero.anim.transform.localPosition = Vector3.zero;
            hero.anim.transform.localEulerAngles = Vector3.zero;
            hero.anim.transform.localScale = Vector3.one;
            hero.height = heroInfo.heroData.height;
            hero.transform.SetParent(Map.Controller.MapController.instance.transform, false);
            Dictionary<RoleAttributeType, RoleAttribute> attrDic = Hero.HeroUtil.CalcHeroAttributesDic(heroInfo);
            hero.SetAttr(attrDic);
            heroDic.Add(heroInfo.instanceID, hero);
            heros.Add(hero);
            #region create hp bar
            HPBarView hpBarView = GetHPBarView(hero);
            hpBarView.transform.SetParent(_hpBoxTrans, false);
            _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
            _heroInfoDic.Add(heroInfo.instanceID, heroInfo);
            hero.hpBarView = hpBarView;
            #endregion
        }

        private void PreCreateAeon(PlayerInfo playerInfo)
        {
            Hero.Model.HeroInfo heroInfo = new Hero.Model.HeroInfo(Hero.Model.HeroData.GetHeroDataByID((int)playerInfo.playerData.summonID));
            HeroEntity.CreateHeroEntity(heroInfo, (aeon) =>
            {
                if (aeon != null)
                {
                    ShadersUtil.SetShaderKeyword(aeon, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                    aeon.anim.gameObject.SetActive(false);
                    aeon.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                    aeon.characterInfo = Logic.Character.Model.CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                    aeon.characterInfo.instanceID = playerInfo.instanceID;
                    aeon.positionId = (int)FormationPosition.Player_Position_5;
                    aeon.pos = Logic.Position.Model.PositionData.GetPos(aeon.positionId);
                    aeon.eulerAngles = ROTATE;
                    aeon.scale = heroInfo.heroData.scale;
                    aeon.anim.transform.localPosition = Vector3.zero;
                    aeon.anim.transform.localEulerAngles = Vector3.zero;
                    aeon.anim.transform.localScale = Vector3.one;
                    _aeonList.Add(aeon);
                }
            });
        }
#endif
        #endregion

        private void PreCreateAeon(CharacterBaseInfo characterInfo, System.Action<bool> callback)
        {
            if (characterInfo.aeonId == 0)
            {
                if (callback != null)
                    callback(true);
                return;
            }
            Hero.Model.HeroInfo heroInfo = new Hero.Model.HeroInfo(Hero.Model.HeroData.GetHeroDataByID((int)characterInfo.aeonId));
            HeroEntity.CreateHeroEntity(heroInfo, (aeon) =>
            {
                if (aeon != null)
                {
                    ShadersUtil.SetShaderKeyword(aeon, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                    aeon.anim.gameObject.SetActive(false);
                    aeon.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                    aeon.characterInfo = Logic.Character.Model.CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                    aeon.characterInfo.instanceID = characterInfo.instanceID;
                    aeon.positionId = (int)FormationPosition.Player_Position_5;
                    aeon.pos = Logic.Position.Model.PositionData.GetPos(aeon.positionId);
                    aeon.eulerAngles = ROTATE;
                    aeon.scale = heroInfo.heroData.scale;
                    aeon.anim.transform.localPosition = Vector3.zero;
                    aeon.anim.transform.localEulerAngles = Vector3.zero;
                    aeon.anim.transform.localScale = Vector3.one;
                    _aeonList.Add(aeon);
                    if (callback != null)
                        callback(true);
                }
                else
                {

                    if (callback != null)
                        callback(false);
                }
            });
        }

        public HeroEntity CreateAeon(CharacterEntity character)
        {
            if (_aeonList.Count <= 0) return null;
            HeroEntity aeon = _aeonList.First();
            aeon.anim.gameObject.SetActive(true);
            Hero.Model.HeroInfo heroInfo = new Hero.Model.HeroInfo(Hero.Model.HeroData.GetHeroDataByID((int)character.characterInfo.aeonId));
            #region shadow
            Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
            Transform root = aeon.rootNode.transform;
            Map.Controller.MapController.instance.AddTarget(root, size);
            #endregion
            aeon.CloneAttr(character);
            return aeon;
        }

        private HPBarView GetHPBarView(HeroEntity hero)
        {
            HPBarView hpBarView = null;
            if (_deadHpBarViewQueue.Count > 0)
            {
                hpBarView = _deadHpBarViewQueue.Dequeue();
                hpBarView.character = hero;
            }
            else
                hpBarView = HPBarView.CreateHPBarView(hero);
            return hpBarView;
        }

        public void ClearHeros()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                hero.anim.gameObject.SetActive(true);
                if (hero is PlayerEntity)
                    (hero as PlayerEntity).petEntity.gameObject.SetActive(true);
                CheckedAnimatorController(hero);
                Pool.Controller.PoolController.instance.Despawn(hero.name, hero);
            }
            heros.Clear();
            heroDic.Clear();

            ClearComboHeros(true);
            ClearAeons(true);
            List<HeroEntity> deadHeroList = _deadHeroDic.GetValues();
            for (int i = 0, count = deadHeroList.Count; i < count; i++)
            {
                //GameObject.Destroy(deadHeroList[i].gameObject);
                HeroEntity hero = deadHeroList[i];
                hero.anim.gameObject.SetActive(true);
                if (hero is PlayerEntity)
                    (hero as PlayerEntity).petEntity.gameObject.SetActive(true);
                CheckedAnimatorController(hero);
                Pool.Controller.PoolController.instance.Despawn(hero.name, hero);
            }
            _skillInfos.Clear();
            deadHeroList.Clear();
            _deadHeroDic.Clear();
            _deadHeroCheckedList.Clear();
            _deadHeroTombStones.Clear();

            List<HPBarView> hpbarViewList = _hpBarViewDic.GetValues();
            for (int i = 0, count = hpbarViewList.Count; i < count; i++)
            {
                GameObject.Destroy(hpbarViewList[i].gameObject);
            }
            hpbarViewList.Clear();
            _hpBarViewDic.Clear();

            while (_deadHpBarViewQueue.Count > 0)
            {
                UnityEngine.GameObject.Destroy(_deadHpBarViewQueue.Dequeue().gameObject);
            }

            //if (_hpBoxTrans)
            //    GameObject.Destroy(_hpBoxTrans.gameObject);
            //_hpBoxTrans = null;

            _heroInfoDic.Clear();
            _playerInfo = null;

            _addedPlayerFormationBuff = false;
        }

        public void ClearComboHeros(bool destroy)
        {
            List<HeroEntity> comboHeroList = comboHeroDic.GetValues();
            for (int i = 0, count = comboHeroList.Count; i < count; i++)
            {
                HeroEntity hero = comboHeroList[i];
                Map.Controller.MapController.instance.RemoveTarget(hero.rootNode.transform);
                if (destroy)
                {
                    hero.anim.gameObject.SetActive(true);
                    if (hero is PlayerEntity)
                        (hero as PlayerEntity).petEntity.gameObject.SetActive(true);
                    CheckedAnimatorController(hero);
                    Pool.Controller.PoolController.instance.Despawn(hero.name, hero);
                }
                else
                {
                    hero.anim.gameObject.SetActive(false);
                    if (hero is PlayerEntity)
                        (hero as PlayerEntity).petEntity.gameObject.SetActive(false);
                }
                Effect.Controller.EffectController.instance.RemoveEffects(hero, 0f);
            }
            comboHeroList.Clear();
            comboHeroList = null;
            if (destroy)
                comboHeroDic.Clear();
        }

        private void CheckedAnimatorController(HeroEntity hero)
        {
            if (!hero.anim.name.Contains(hero.anim.runtimeAnimatorController.name))
            {
                string path = string.Empty;
                if (hero is PlayerEntity)
                    path = Common.ResMgr.ResPath.GetPlayerAnimatorControllerPath(hero.anim.name);
                else
                    path = Common.ResMgr.ResPath.GetHeroAnimatorControllerPath(hero.anim.name);
                hero.anim.runtimeAnimatorController = Common.ResMgr.ResMgr.instance.Load<Object>(path) as RuntimeAnimatorController;
            }
        }

        public void ClearAeons(bool destroy)
        {
            for (int i = 0, count = _aeonList.Count; i < count; i++)
            {
                HeroEntity aeon = _aeonList[i];
                Map.Controller.MapController.instance.RemoveTarget(aeon.rootNode.transform);
                if (destroy)
                {
                    //GameObject.Destroy(aeon.gameObject);
                    aeon.anim.gameObject.SetActive(true);
                    Pool.Controller.PoolController.instance.Despawn(aeon.name, aeon);
                    //Debugger.Log("pool name:{0}  {1}", aeon.name, (spawnPool == null));
                    //spawnPool.Despawn(aeon.transform);
                }
                else
                    aeon.anim.gameObject.SetActive(false);
            }
            if (destroy)
                _aeonList.Clear();
        }

        public void SetHerosCD2Zero()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                if (hero.characterInfo.skillInfo1 != null)
                    hero.skill1CD = hero.characterInfo.skillInfo1.skillData.CD;
                if (hero.characterInfo.skillInfo2 != null)
                    hero.skill2CD = hero.characterInfo.skillInfo2.skillData.CD;
                //if (hero.characterInfo.aeonSkillInfo != null)
                //    hero. = hero.characterInfo.skillInfo1.skillData.CD;
            }
        }

        public void Reborn(CharacterEntity character, float hpRate, float delay)
        {
            StartCoroutine(RebornCoroutine(character, hpRate, delay));
        }

        private IEnumerator RebornCoroutine(CharacterEntity character, float hpRate, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            if (character is HeroEntity)
            {
                HeroEntity hero = character as HeroEntity;
                if (deadHeroDic.ContainsKey(hero.characterInfo.instanceID))
                {
                    Effect.Model.EffectData effecData = Effect.Model.EffectData.GetEffectDataById(Effect.Controller.EffectController.TOMBSTONE_EFFECT_ID);
                    if (effecData != null)
                    {
                        string effectName = Effect.Controller.EffectController.instance.GetEffectName(hero, effecData.effectName);
                        Effect.Controller.EffectController.instance.RemoveEffectByName(effectName);
                        if (_deadHeroTombStones.Contains(effectName))
                            _deadHeroTombStones.Remove(effectName);
                    }
                    hero.Reborn(hpRate);
                    hero.anim.gameObject.SetActive(true);
                    if (hero is PlayerEntity)
                        (hero as PlayerEntity).petEntity.gameObject.SetActive(true);
                    AnimatorUtil.Play(hero.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                    _deadCount--;
                    deadHeroDic.Remove(hero.characterInfo.instanceID);
                    heroDic.Add(hero.characterInfo.instanceID, hero);
                    heros.Add(hero);
                    #region shadow
                    Vector2 size = Vector3.one;
                    if (hero is PlayerEntity)
                        size = PlayerEntity.GetShadowSize(ShadowType.One);//Ö÷½ÇÔÝÊ±Ð´ËÀ
                    else
                        size = HeroEntity.GetShadowSize(hero.characterInfo.roleInfo.heroData.shadowType);
                    Transform root = hero.rootNode.transform;
                    Map.Controller.MapController.instance.AddTarget(root, size);
                    #endregion

                    HPBarView hpBarView = GetHPBarView(hero);
                    hpBarView.transform.SetParent(_hpBoxTrans, false);
                    _hpBarViewDic.Add(character.characterInfo.instanceID, hpBarView);
                    hero.hpBarView = hpBarView;
                    hero.skillItemBoxView.ResetSkillItem();
                }
                if (character is PlayerEntity)
                    _playerDead = false;
            }
        }

        public void Run_Scene()
        {
            FightController.instance.PreLoadNumFont();
            if (_fightCount == 0)
                Logic.Audio.Controller.AudioController.instance.PlayAudio(Logic.Audio.Controller.AudioController.BATTLE_START_AUDIO, false, 1f);
            else
            {
                if (Random.Range(0f, 1f) <= 0.3)
                    Logic.Audio.Controller.AudioController.instance.PlayAudio(Logic.Audio.Controller.AudioController.BATTLE_START_AUDIO, false, 1f);
            }
            _fightCount++;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                FightController.instance.Run_Scene(heros[i].characterInfo.instanceID);
            }
            StartCoroutine(FinishRunCoroutine(FightController.RUN_SCENE_TIME));
        }

        public void Victory_Scene()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                FightController.instance.Victory_Scene(heros[i].characterInfo.instanceID);
            }
        }

        public void RunWithoutMove_Scene()
        {
            RemoveLimitBuffs();
            DestroyDeadHeroTombStones();
            StartCoroutine(RunWithoutMove_SceneCoroutine());
        }

        private IEnumerator RunWithoutMove_SceneCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                FightController.instance.RunWithoutMove_Scene(heros[i].characterInfo.instanceID);
            }
            Logic.UI.FightTips.Controller.FightTipsController.instance.NextTeam();
        }

        private IEnumerator FinishRunCoroutine(float time)
        {
            yield return new WaitForSeconds(0.2f);
            Logic.UI.FightTips.Controller.FightTipsController.instance.NextTeam();
            yield return new WaitForSeconds(time - 0.2f);
            DataMessageHandler.DataMessage_FinishRun(CharacterType.Player);
        }

        public void EndRun_Scene()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                FightController.instance.EndRun_Scene(heros[i].characterInfo.instanceID);
            }
        }

        public void HeroTrigger(float delay)
        {
            StartCoroutine(HeroTriggerCoroutine(delay));
        }

        private IEnumerator HeroTriggerCoroutine(float delay)
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                hero.anim.gameObject.SetActive(false);
                if (hero is PlayerEntity)
                    (hero as PlayerEntity).petEntity.gameObject.SetActive(false);
                Effect.Controller.EffectController.instance.SwitchEffect(hero, (int)LayerType.None);
            }
            ShowDeadHeroTombStones(false);
            ShowHPBarViews(false);
            yield return new WaitForSeconds(delay);
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                hero.anim.gameObject.SetActive(true);
                if (hero is PlayerEntity)
                    (hero as PlayerEntity).petEntity.gameObject.SetActive(true);
                Effect.Controller.EffectController.instance.SwitchEffect(hero, (int)LayerType.Fight);
            }
            ShowDeadHeroTombStones(true);
            ShowHPBarViews(true);
        }

        public void ShowHPBarViews(bool show)
        {
            HPBarView[] hpbarViews = _hpBarViewDic.GetValueArray();
            for (int i = 0, count = hpbarViews.Length; i < count; i++)
            {
                hpbarViews[i].gameObject.SetActive(show);
            }
        }

        public HeroEntity this[uint id]
        {
            get
            {
                if (heroDic.ContainsKey(id))
                    return heroDic[id];
                return null;
            }
        }

        public HeroEntity GetDeadHeroById(uint id)
        {
            if (deadHeroDic.ContainsKey(id))
                return deadHeroDic[id];
            return null;
        }

        private void FriendDead(HeroEntity hero)
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity h = heros[i];
                foreach (var kvp in h.characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.FRIEND_DEAD, kvp.Key);
                    if (func != null)
                    {
                        func.Call(hero, h, kvp.Value);
                    }
                }
                if (hero.addedHaloBuff)
                    h.RemoveHaloBuff(hero);
            }
            hero.addedHaloBuff = false;
        }

        public void RemoveLimitBuffs()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity h = heros[i];
                for (int j = 0, jCount = _limitBuffs.Count; j < jCount; j++)
                {
                    BuffType buffType = _limitBuffs[j];
                    h.RemoveBuff(buffType);
                    string path = Fight.Model.FightProxy.instance.GetIconPath(buffType, false);
                    h.RemoveBuffIcon(path);
                }
            }
        }

        public void AddHaloBuff()
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity h = heros[i];
                if (!h.addedHaloBuff)
                    h.AddHaloBuff();

                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_FOREVER_BUFF_ICON);
                if (func != null)
                {
                    object[] rs = func.Call(h);
                    if (rs != null && rs.Length > 0)
                    {
                        string path = rs[0].ToString();
                        float lastTime = 0;
                        float.TryParse(rs[1].ToString(), out lastTime);
                        h.AddBuffIcon(path, lastTime);
                    }
                }
            }
        }

        public void RemoveEnemyHaloBuff(EnemyEntity enemy)
        {
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity h = heros[i];
                h.RemoveHaloBuff(enemy);
            }
        }

        public void AddFormationBuff()
        {
            FormationInfo formationInfo = FightProxy.instance.ourFormation;
            if (formationInfo == null)
                return;
            List<FormationAttrData> formationAtts = formationInfo.GetFormationDatas();// FormationAttrData.GetFormationDatas(formationInfo.id, formationInfo.level);
            for (int i = 0, count = formationAtts.Count; i < count; i++)
            {
                FormationAttrData formationAttrData = formationAtts[i];
                bool addPlayer = false;
                bool addEnemy = false;
                switch (formationAttrData.targetType)
                {
                    case TargetType.None:
                        if (!_addedPlayerFormationBuff)
                            addPlayer = true;
                        addEnemy = true;
                        break;
                    case TargetType.Ally:
                        if (!_addedPlayerFormationBuff)
                            addPlayer = true;
                        break;
                    case TargetType.Enemy:
                        addEnemy = true;
                        break;
                }
                switch (formationAttrData.effectType)
                {
                    case FormationEffectType.PhysicAtk:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.PhysicsAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.PhysicsAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.MagicAtk:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.MagicAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.MagicAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.PhysicDef:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.PhysicsDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.PhysicsDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.MagicDef:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.MagicDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.MagicDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Hit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Hit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Hit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Dodge:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Dodge, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Dodge, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }

                        }
                        break;
                    case FormationEffectType.Crit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Crit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Crit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.AntiCrit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.AntiCrit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.AntiCrit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Block:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Block, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Block, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.AntiBlock:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.AntiBlock, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.AntiBlock, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.CritHurtAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.CritHurtAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.CritHurtAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.CritHurtDec:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.CritHurtDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.CritHurtDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.DamageAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.DamageAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.DamageAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.DamageDec:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.DamageDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.DamageDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Weakness:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Weakness, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Weakness, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.HPLimit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                float hpRate = formationInfo.GetFightFormationAttrValue(formationAttrData.effectType);
                                hero.AddBuff(hero, hero, null, null, BuffType.HPLimit, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, hpRate, 1, 1, true, false);
                                float hpValue = hpRate * hero.characterInfo.maxHP;
                                hero.SetTreatValue((uint)hpValue);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                float hpRate = formationInfo.GetFightFormationAttrValue(formationAttrData.effectType);
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.HPLimit, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, hpRate, 1, 1, true, false);
                                float hpValue = hpRate * enemy.characterInfo.maxHP;
                                enemy.SetTreatValue((uint)hpValue);
                            }
                        }
                        break;
                    case FormationEffectType.TreatPercent:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                BuffInfo buffInfo = hero.AddBuff(hero, hero, null, null, BuffType.TreatPercent, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                                buffInfo.interval = formationAttrData.interval;
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                BuffInfo buffInfo = enemy.AddBuff(enemy, enemy, null, null, BuffType.TreatPercent, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                                buffInfo.interval = formationAttrData.interval;
                            }
                        }
                        break;
                    case FormationEffectType.TreatAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.TreatAdd, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = EnemyController.instance.enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = EnemyController.instance.enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.TreatAdd, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                }
            }
            _addedPlayerFormationBuff = true;
        }

        public void Remove(uint heroId, float delay)
        {
            if (heroDic.ContainsKey(heroId))
            {
                HeroEntity hero = instance[heroId];
                hero.ClearBuff();
                heroDic.Remove(heroId);
                heros.Remove(hero);
                StartCoroutine(RemoveHeroCoroutine(hero, delay));
            }

            if (_hpBarViewDic.ContainsKey(heroId))
            {
                HPBarView hpBarView = _hpBarViewDic[heroId];
                hpBarView.character = null;
                _deadHpBarViewQueue.Enqueue(hpBarView);
                _hpBarViewDic.Remove(heroId);
            }
        }

        private void CheckHeroDic()
        {
            if (_allDead) return;
            if (_deadHeroDic.Count == _deadCount && _deadHeroCheckedList.Count == _heroCount && _deadCount == _heroCount)
            {
                _allDead = true;
                DataMessageHandler.DataMessage_TeamAllDead(CharacterType.Player);
            }
        }

        private IEnumerator RemoveHeroCoroutine(HeroEntity hero, float delay)
        {
            Effect.Controller.EffectController.instance.RemoveEffects(hero, 0.5f);
            FriendDead(hero);
            if (hero.addedHaloBuff)
                EnemyController.instance.RemovePlayerHaloBuff(hero);
            _deadHeroDic.Add(hero.characterInfo.instanceID, hero);
            int result = 0;
            foreach (var kvp in hero.characterInfo.passiveIdDic)
            {
                int r = 0;
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.DEAD, kvp.Key);
                if (func != null)
                {
                    object[] rs = func.Call(hero, kvp.Value);
                    if (rs != null && rs.Length > 0)
                    {
                        int.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
            }
            if (result == 0)
            {
                #region dead effect
                uint effectId = (uint)hero.characterInfo.roleInfo.heroData.dieEffectId;
                if (effectId != 0)
                {
                    Effect.Model.EffectInfo deadEffectInfo = new Effect.Model.EffectInfo(effectId);
                    if (deadEffectInfo != null)
                    {
                        deadEffectInfo.pos = hero.pos + deadEffectInfo.effectData.offset;
                        deadEffectInfo.character = hero;
                        deadEffectInfo.target = hero;
                        deadEffectInfo.delay = deadEffectInfo.effectData.delay;
                        Effect.Controller.EffectController.instance.PlayNoSkillEffect(deadEffectInfo, true);
                    }
                }
                #endregion
                if (delay > 0)
                    yield return new WaitForSeconds(delay);
                if (hero.anim)
                    hero.anim.gameObject.SetActive(false);
                if (hero is PlayerEntity)
                {
                    _playerDead = true;
                    (hero as PlayerEntity).petEntity.gameObject.SetActive(false);
                }
                if (hero.rootNode)
                    Map.Controller.MapController.instance.RemoveTarget(hero.rootNode.transform);
                #region tombstone effect
                Effect.Model.EffectInfo effectInfo = new Effect.Model.EffectInfo(Effect.Controller.EffectController.TOMBSTONE_EFFECT_ID);
                effectInfo.pos = hero.pos + effectInfo.effectData.offset;
                effectInfo.character = hero;
                effectInfo.target = hero;
                effectInfo.delay = effectInfo.effectData.delay;
                Effect.Controller.EffectController.instance.PlayNoSkillEffect(effectInfo);
                _deadHeroTombStones.Add(Effect.Controller.EffectController.instance.GetEffectName(hero, effectInfo.effectData.effectName));
                #endregion

                //DataMessageHandler.DataMessage_RemoveCharacterFromHitSkillQueue(hero.characterInfo.instanceID);
                if (!_deadHeroCheckedList.Contains(hero.characterInfo.instanceID))
                    _deadHeroCheckedList.Add(hero.characterInfo.instanceID);
                _deadCount++;
                CheckHeroDic();
            }
            else
                _deadCount++;
        }

        private void ShowDeadHeroTombStones(bool isShow)
        {
            //List<HeroEntity> list = _deadHeroDic.GetValues();
            //HeroEntity hero = null;
            //for (int i = 0, count = list.Count; i < count; i++)
            //{
            //    hero = list[i];
            //    hero.gameObject.SetActive(isShow);
            //    if (isShow)
            //        Action.Controller.ActionController.instance.PlayerAnimAction(hero, AnimatorUtil.DEAD, 1f);
            //}
            for (int i = 0, count = _deadHeroTombStones.Count; i < count; i++)
            {
                Effect.Controller.EffectController.instance.SwitchEffectByPrefabName(_deadHeroTombStones[i], (int)(isShow ? LayerType.Fight : LayerType.None));
            }
        }

        private void DestroyDeadHeroTombStones()
        {
            for (int i = 0, count = _deadHeroTombStones.Count; i < count; i++)
            {
                Effect.Controller.EffectController.instance.RemoveEffectByPrefabName(_deadHeroTombStones[i]);
            }
            _deadHeroTombStones.Clear();
        }

        public void SwitchHeros(int layer, HeroEntity ignoreHero = null)
        {
            List<HeroEntity> list = PlayerController.instance.heros;
            for (int i = 0, count = list.Count; i < count; i++)
            {
                HeroEntity hero = list[i];
                if (hero == ignoreHero) continue;
                Common.Util.TransformUtil.SwitchLayer(hero.transform, layer);
                Logic.Effect.Controller.EffectController.instance.SwitchEffect(hero, layer);
            }
        }

        #region combo
        public void ResetComboSkill(uint userId, uint skillId)
        {
            HeroEntity hero = this[userId];
            if (hero)
                hero.ResetCD(hero.GetSkillInfoById(skillId));
        }

        public bool HasFloatHeros()
        {
            bool result = false;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                result |= CanFlaot(heros[i]);
            }
            return result;
        }

        public bool HasTumbleHeros()
        {
            bool result = false;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                result |= CanTumble(heros[i]);
            }
            return result;
        }

        public bool CanFlaot(HeroEntity hero)
        {
            bool result = false;
            result = CanSkill1Float(hero) || CanSkill2Float(hero);
            return result;
        }

        public bool CanSkill1Float(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill1 || hero.canPlaySkill1)
            {
                result |= SkillUtil.AttackableFloat(hero.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanSkill2Float(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill2 || hero.canPlaySkill2)
            {
                result |= SkillUtil.AttackableFloat(hero.characterInfo.skillInfo2);
            }
            return result;
        }

        public bool CanTumble(HeroEntity hero)
        {
            bool result = false;
            result = CanSkill1Tumble(hero) || CanSkill2Tumble(hero);
            return result;
        }

        public bool CanSkill1Tumble(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill1 || hero.canPlaySkill1)
            {
                result |= SkillUtil.AttackableTumble(hero.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanSkill2Tumble(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill2 || hero.canPlaySkill2)
            {
                result |= SkillUtil.AttackableTumble(hero.characterInfo.skillInfo2);
            }
            return result;
        }


        public bool HasCanOrderFloatHeros()
        {
            bool result = false;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                result |= CanOrderFlaot(heros[i]);
            }
            return result;
        }

        public bool HasCanOrderTumbleHeros()
        {
            bool result = false;
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                result |= CanOrderTumble(heros[i]);
            }
            return result;
        }

        public bool CanOrderFlaot(HeroEntity hero)
        {
            bool result = false;
            result = CanOrderSkill1Float(hero) || CanOrderSkill2Float(hero);
            return result;
        }

        public bool CanOrderSkill1Float(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill1)
            {
                result |= SkillUtil.AttackableFloat(hero.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanOrderSkill2Float(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill2)
            {
                result |= SkillUtil.AttackableFloat(hero.characterInfo.skillInfo2);
            }
            return result;
        }

        public bool CanOrderTumble(HeroEntity hero)
        {
            bool result = false;
            result = CanOrderSkill1Tumble(hero) || CanOrderSkill2Tumble(hero);
            return result;
        }

        public bool CanOrderSkill1Tumble(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill1)
            {
                result |= SkillUtil.AttackableTumble(hero.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanOrderSkill2Tumble(HeroEntity hero)
        {
            bool result = false;
            if (hero.canOrderSkill2)
            {
                result |= SkillUtil.AttackableTumble(hero.characterInfo.skillInfo2);
            }
            return result;
        }
        #endregion
    }
}