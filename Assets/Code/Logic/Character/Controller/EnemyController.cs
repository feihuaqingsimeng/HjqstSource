using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Net.Controller;
using Logic.Character.Model;
using Logic.Skill.Model;
using Logic.Fight.Controller;
using Logic.UI.HPBar.View;
using Logic.Team.Model;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Fight.Model;
using Logic.Player.Model;
using Logic.Shaders;
using Common.Animators;
using Logic.Formation.Model;
using LuaInterface;
using Logic.Skill;
using Logic.Effect.Model;
using Logic.Effect.Controller;
using Common.Util;
namespace Logic.Character.Controller
{
    public class EnemyController : SingletonMono<EnemyController>
    {
        private Dictionary<uint, EnemyEntity> _enemyDic = new Dictionary<uint, EnemyEntity>();
        private List<EnemyEntity> _enemies = new List<EnemyEntity>();
        private Dictionary<uint, EnemyEntity> _comboEnemyDic = new Dictionary<uint, EnemyEntity>();
        private Dictionary<uint, EnemyEntity> _deadEnemyDic = new Dictionary<uint, EnemyEntity>();
        private List<uint> _deadEnemyCheckedList = new List<uint>();
        private List<string> _deadHeroTombStones = new List<string>();
        private Dictionary<uint, HPBarView> _hpBarViewDic = new Dictionary<uint, HPBarView>();
        private Queue<HPBarView> _deadHpBarViewQueue = new Queue<HPBarView>();
        private List<SkillInfo> _skillInfos = new List<SkillInfo>();
        private Transform _hpBoxTrans;
        private Dictionary<uint, HeroInfo> _enemyHeroInfoDic = new Dictionary<uint, HeroInfo>();
        private PlayerInfo _enemyPlayerInfo;
        private static Vector3 ROTATE_1 = new Vector3(0, 270, 0);
        private static Vector3 ROTATE_2 = new Vector3(0, 180, 180);
        private static Vector3 SCALE_2 = new Vector3(1, -1, -1);
        private bool _allDead = false;
        private int _deadCount = 0;
        private int _preloadCount;
        private int _preloadTotal;
        private int _enemyCount = 0;
        private float _offset;
        private bool _enemyFloatable = false;
        #region fight imitate
#if UNITY_EDITOR
        [NoToLua]
        public Dictionary<FormationPosition, HeroInfo> imitateEnemyHeroInfoDic = new Dictionary<FormationPosition, HeroInfo>();
        [NoToLua]
        public Dictionary<FormationPosition, PlayerInfo> imitateEnemyPlayerInfoDic = new Dictionary<FormationPosition, PlayerInfo>();

        [NoToLua]
        public void InitImitateData()
        {
            imitateEnemyHeroInfoDic.Clear();
            imitateEnemyPlayerInfoDic.Clear();
        }

        [NoToLua]
        public HeroInfo GetImitateEnemyHeroInfo(int instanceId)
        {
            foreach (var kvp in imitateEnemyHeroInfoDic)
            {
                if (kvp.Value.instanceID == instanceId)
                    return kvp.Value;
            }
            return null;
        }
#endif
        #endregion
        public Dictionary<uint, EnemyEntity> enemyDic
        {
            get
            {
                return _enemyDic;
            }
        }

        public List<EnemyEntity> enemies
        {
            get
            {
                return _enemies;
            }
        }

        public Dictionary<uint, EnemyEntity> deadEnemyDic
        {
            get
            {
                return _deadEnemyDic;
            }
        }

        public List<SkillInfo> skillInfos
        {
            get
            {
                return _skillInfos;
            }
        }

        public HeroInfo GetEnemyHeroInfo(uint id)
        {
            if (_enemyHeroInfoDic.ContainsKey(id))
                return _enemyHeroInfoDic[id];
            return null;
        }

        public PlayerInfo GetEnemyPlayerInfo()
        {
            return _enemyPlayerInfo;
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
                for (int i = 0, count = enemies.Count; i < count; i++)
                {
                    enemies[i].tickCD = value;
                }
            }
        }

        private float _lastTickTime;
        public float lastTickTime
        {
            set
            {
                _lastTickTime = value;
                for (int i = 0, count = enemies.Count; i < count; i++)
                {
                    enemies[i].lastTickTime = value;
                }
            }
            get
            {
                return _lastTickTime;
            }
        }

        public bool enemyFloatable
        {
            get { return _enemyFloatable; }
            set { _enemyFloatable = value; }
        }

        public Dictionary<uint, EnemyEntity> comboEnemyDic
        {
            get
            {
                return _comboEnemyDic;
            }
        }

        public EnemyEntity GetComboEnemy(uint id)
        {
            if (comboEnemyDic.ContainsKey(id))
                return comboEnemyDic[id];
            return null;
        }

        void Awake()
        {
            instance = this;
        }

        private void PreLoadEnemy(bool flag)
        {
            if (flag)
                _preloadCount++;
            if (_preloadCount == _preloadTotal)
            {
                SortSkillInfos();
                DataMessageHandler.DataMessage_InitOK(CharacterType.Enemy);
            }
        }

        #region sort skill
        public void SortSkillInfos()
        {
            _skillInfos.Clear();
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity enemy = enemies[i];
                if (enemy.characterInfo.skillInfo1 != null)
                    _skillInfos.Add(enemy.characterInfo.skillInfo1);
                if (enemy.characterInfo.skillInfo2 != null)
                    _skillInfos.Add(enemy.characterInfo.skillInfo2);
            }
            _skillInfos.Sort(CharacterUtil.SortSkill);
        }

        #endregion

        public void CreateEnemys()
        {
            _preloadCount = 0;
            if (!_hpBoxTrans)
            {
                GameObject go = new GameObject();
                go.name = "enemy_hp_box";
                _hpBoxTrans = go.transform;
                _hpBoxTrans.SetParent(UI.UIMgr.instance.basicCanvas.transform, false);
            }
            _allDead = false;
            StartCoroutine(CreateEnemiesCoroutine());
        }

        private IEnumerator CreateEnemiesCoroutine()
        {
            yield return null;
            _offset = 10f;
            switch (FightController.instance.fightType)
            {
                case FightType.PVE:
                    {
                        TeamData pveTeamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                        Dictionary<FormationPosition, HeroInfo> pveEnemyListDic = Logic.Fight.Model.FightProxy.instance.GetMockTeamHeroInfoDictionary(pveTeamData);
                        if (pveEnemyListDic == null)
                            yield break;
                        _preloadTotal = pveEnemyListDic.Count;
                        _enemyCount = _preloadTotal;
                        float capacityFactor = 1f;
                        try
                        {
                            LuaInterface.LuaTable formationModel = (LuaInterface.LuaTable)LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "formation_model")[0];
                            LuaInterface.LuaTable teamInfo = (LuaInterface.LuaTable)(formationModel.GetLuaFunction("GetCurPveFormationTeam").Call(null)[0]);
                            int ourPower = teamInfo.GetLuaFunction("Power").Call(teamInfo)[0].ToString().ToInt32();
                            Logic.Dungeon.Model.DungeonData dungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
                            int interval = dungeonData.combat - ourPower;
                            if (interval > 0)
                                capacityFactor = 1 + interval * Logic.Game.Model.GlobalData.GetGlobalData().dungeonAtkCombatAdd;
                        }
                        catch (System.Exception e)
                        {
                            Debugger.LogError(e.StackTrace);
                            capacityFactor = 1f;
                        }
                        foreach (var kvp in pveEnemyListDic)
                        {
                            CreateEnemy(kvp, 1f, pveTeamData, PreLoadEnemy, capacityFactor);
                        }
                    }
                    break;
                case FightType.DailyPVE:
                    {
                        TeamData dailyPVETeamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                        Dictionary<FormationPosition, HeroInfo> dailyPVEEnemyListDic = Logic.Fight.Model.FightProxy.instance.GetMockTeamHeroInfoDictionary(dailyPVETeamData);
                        if (dailyPVEEnemyListDic == null)
                            yield break;
                        _preloadTotal = dailyPVEEnemyListDic.Count;
                        _enemyCount = _preloadTotal;
                        foreach (var kvp in dailyPVEEnemyListDic)
                        {
                            CreateEnemy(kvp, 1f, dailyPVETeamData, PreLoadEnemy);
                        }
                    }
                    break;
                case FightType.WorldTree:
                    {
                        TeamData worldTreeTeamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                        Dictionary<FormationPosition, HeroInfo> worldTreeEnemyListDic = Logic.Fight.Model.FightProxy.instance.GetMockTeamHeroInfoDictionary(worldTreeTeamData);
                        if (worldTreeEnemyListDic == null)
                            yield break;
                        _preloadTotal = worldTreeEnemyListDic.Count;
                        _enemyCount = _preloadTotal;
                        foreach (var kvp in worldTreeEnemyListDic)
                        {
                            CreateEnemy(kvp, (1 - Logic.UI.WorldTree.Model.WorldTreeProxy.instance.GetWorldTreeChallengeFailedWeakenValue()), worldTreeTeamData, PreLoadEnemy);
                        }
                    }
                    break;
                case FightType.WorldBoss:
                    {
                        _preloadTotal = 1;
                        _enemyCount = _preloadTotal;
                        // need to implement later
                        HeroData worldBossHeroData = HeroData.GetHeroDataByID(WorldBoss.Model.WorldBossProxy.instance.BossID);
                        HeroInfo worldBossHeroInfo = new HeroInfo(0, worldBossHeroData.id, 0, 0, (int)worldBossHeroData.starMax, 1);
                        KeyValuePair<FormationPosition, HeroInfo> worldBossKeyValuePair = new KeyValuePair<FormationPosition, HeroInfo>(FormationPosition.Enemy_Position_5, worldBossHeroInfo);
                        CreateEnemy(worldBossKeyValuePair, (enemyEntity) =>
                        {
                            if (!enemyEntity)
                                PreLoadEnemy(false);
                            else
                            {
                                WorldBoss.Model.WorldBossProxy.instance.onWorldBossCurrHPChangedDelegate += enemyEntity.OnHPChangedHandler;
                                WorldBoss.Model.WorldBossProxy.instance.onWorldBossKilledByOthersDelegate += enemyEntity.OnForceDeadHandler;
                                Logic.Protocol.Model.WorldBossFightProto worldBossFightProto = WorldBoss.Model.WorldBossProxy.instance.WorldBossFightProto;
                                enemyEntity.SetAttr(worldBossFightProto);
                                PreLoadEnemy(true);
                            }
                        });
                    }
                    break;
                case FightType.Arena:
                case FightType.Expedition:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                case FightType.ConsortiaFight:
                    {
                        List<FightHeroInfo> fightEnemyHeroInfos = FightProxy.instance.enemyFightHeroInfoList;
                        _preloadTotal = fightEnemyHeroInfos.Count;
                        if (FightProxy.instance.enemyFightPlayerInfo != null)
                        {
                            _preloadTotal += 1;
                            CreateEnemyPlayer(FightProxy.instance.enemyFightPlayerInfo, PreLoadEnemy);
                        }
                        _enemyCount = _preloadTotal;
                        for (int i = 0, length = fightEnemyHeroInfos.Count; i < length; i++)
                        {
                            CreateEnemy(fightEnemyHeroInfos[i], PreLoadEnemy);
                        }
                    }
                    break;
                case FightType.SkillDisplay:
                    {
                        _offset = 0f;
                        List<FightHeroInfo> fightEnemyHeroInfos = FightProxy.instance.enemyFightHeroInfoList;
                        _preloadTotal = fightEnemyHeroInfos.Count;
                        if (FightProxy.instance.enemyFightPlayerInfo != null)
                        {
                            _preloadTotal += 1;
                            CreateEnemyPlayer(FightProxy.instance.enemyFightPlayerInfo, PreLoadEnemy);
                        }
                        _enemyCount = _preloadTotal;
                        for (int i = 0, length = fightEnemyHeroInfos.Count; i < length; i++)
                        {
                            CreateEnemy(fightEnemyHeroInfos[i], PreLoadEnemy);
                        }
                    }
                    break;
                case FightType.FirstFight:
                    {
                        List<FightHeroInfo> fightEnemyHeroInfos = FightProxy.instance.enemyFightHeroInfoList;
                        _preloadTotal = fightEnemyHeroInfos.Count;
                        _enemyCount = _preloadTotal;
                        for (int i = 0, length = fightEnemyHeroInfos.Count; i < length; i++)
                        {
                            CreateEnemy(fightEnemyHeroInfos[i], PreLoadEnemy);
                        }
                    }
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    if (imitateEnemyPlayerInfoDic.Count > 0)
                        _preloadTotal = imitateEnemyPlayerInfoDic.Count + imitateEnemyHeroInfoDic.Count + 1;
                    else
                        _preloadTotal = imitateEnemyPlayerInfoDic.Count + imitateEnemyHeroInfoDic.Count;
                    _enemyCount = imitateEnemyPlayerInfoDic.Count + imitateEnemyHeroInfoDic.Count;
                    foreach (var kvp in imitateEnemyPlayerInfoDic)
                    {
                        CreateImitateEnemyPlayer(kvp);
                        yield return null;
                    }
                    foreach (var kvp in imitateEnemyHeroInfoDic)
                    {
                        CreateImimateEnemy(kvp);
                        yield return null;
                    }
                    SortSkillInfos();
                    DataMessageHandler.DataMessage_InitOK(CharacterType.Enemy);
                    break;
#endif
            }
        }

        public void CloneEnemy(uint id)
        {
            StartCoroutine(CloneEnemyCoroutine(id));
        }

        private IEnumerator CloneEnemyCoroutine(uint id)
        {
            while (Common.GameTime.Controller.TimeController.instance.playerPause)
            {
                yield return null;
            }
            List<uint> keys = _enemyDic.GetKeys();
            for (int i = 0, count = keys.Count; i < count; i++)
            {
                uint key = keys[i];
                if (key == id)
                {
                    EnemyEntity enemy = this[key];
                    if (enemy is EnemyPlayerEntity)
                        CloneEnemyPlayer(enemy as EnemyPlayerEntity);
                    else
                        CloneEnemy(enemy);
                }
            }
        }

        public void CreateEnemy(KeyValuePair<FormationPosition, HeroInfo> kvp, float maxHPRate, TeamData teamData, System.Action<bool> callback, float capacityFactor = 1)
        {
            HeroInfo heroInfo = kvp.Value;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)kvp.Key);
            pos = pos + new Vector3(10f, 0, 0);
            EnemyEntity.CreateEnemyEntity(heroInfo, (enemy) =>
            {
                if (enemy == null)
                {
                    if (callback != null)
                        callback(false);
                    return;
                }
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                enemy.characterName = heroInfo.heroData.name;
                Vector3 scale = Vector3.one;
                if (teamData.scaleDictionary[kvp.Key] != 0)
                {
                    scale = Vector3.one * teamData.scaleDictionary[kvp.Key];
                }
                else
                {
                    scale = heroInfo.heroData.scale;
                }
                scale.x *= -1;
                //scale *= -1f;
                enemy.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                Transform root = enemy.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                enemy.positionId = (uint)kvp.Key;
                enemy.pos = pos;
                enemy.eulerAngles = ROTATE_1;
                enemy.scale = scale;
                enemy.anim.transform.localEulerAngles = ROTATE_2;
                enemy.anim.transform.localScale = SCALE_2;
                enemy.height = heroInfo.heroData.height;
                enemy.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                enemy.SetAttr(kvp, maxHPRate, capacityFactor);
                enemyDic.Add(heroInfo.instanceID, enemy);
                enemies.Add(enemy);
                _enemyHeroInfoDic.Add(heroInfo.instanceID, heroInfo);
                #region create hp bar
                HPBarView hpBarView = GetHPBarView(enemy);
                hpBarView.transform.SetParent(_hpBoxTrans, false);
                _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
                enemy.hpBarView = hpBarView;
                #endregion
                if (callback != null)
                    callback(true);
            });
        }

        public void CreateEnemy(FightHeroInfo fightHeroInfo, System.Action<bool> callback)
        {
            if (fightHeroInfo == null)
            {
                if (callback != null)
                    callback(false);
                return;
            }
            if (fightHeroInfo.pveHeroProtoData.attr.hp <= 0) return;//版本限制，后会改为服务器不发送
            HeroInfo heroInfo = fightHeroInfo.heroInfo;
            uint posIndex = (uint)fightHeroInfo.pveHeroProtoData.posIndex + 100;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos(posIndex);
            pos = pos + new Vector3(_offset, 0, 0);
            EnemyEntity.CreateEnemyEntity(heroInfo, (enemy) =>
            {
                if (enemy == null)
                {
                    if (callback != null)
                        callback(false);
                    return;
                }
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                enemy.characterName = heroInfo.heroData.name;
                Vector3 scale = heroInfo.heroData.scale;
                scale.x *= -1;
                enemy.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                Transform root = enemy.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                enemy.positionId = posIndex;
                enemy.pos = pos;
                enemy.eulerAngles = ROTATE_1;
                enemy.scale = scale;
                enemy.anim.transform.localEulerAngles = ROTATE_2;
                enemy.anim.transform.localScale = SCALE_2;
                enemy.height = heroInfo.heroData.height;
                enemy.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                enemy.SetAttr(fightHeroInfo.pveHeroProtoData.attr);
                enemyDic.Add(heroInfo.instanceID, enemy);
                enemies.Add(enemy);
                _enemyHeroInfoDic.Add(heroInfo.instanceID, heroInfo);
                #region create hp bar
                HPBarView hpBarView = GetHPBarView(enemy);
                hpBarView.transform.SetParent(_hpBoxTrans, false);
                _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
                enemy.hpBarView = hpBarView;
                #endregion
                if (callback != null)
                    callback(true);
            });

        }

        public void CreateEnemy(KeyValuePair<FormationPosition, HeroInfo> kvp, System.Action<EnemyEntity> callback)
        {
            HeroInfo heroInfo = kvp.Value;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)kvp.Key);
            pos = pos + new Vector3(_offset, 0, 0);
            EnemyEntity.CreateEnemyEntity(heroInfo, (enemy) =>
            {
                if (enemy != null)
                {
                    ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                    enemy.characterName = heroInfo.heroData.name;
                    Vector3 scale = heroInfo.heroData.scale;
                    scale.x *= -1;
                    enemy.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                    #region shadow
                    Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                    Transform root = enemy.rootNode.transform;
                    Map.Controller.MapController.instance.AddTarget(root, size);
                    #endregion
                    enemy.positionId = (uint)kvp.Key;
                    enemy.pos = pos;
                    enemy.eulerAngles = ROTATE_1;
                    enemy.scale = scale;
                    enemy.anim.transform.localEulerAngles = ROTATE_2;
                    enemy.anim.transform.localScale = SCALE_2;
                    enemy.height = heroInfo.heroData.height;
                    enemy.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                    enemyDic.Add(heroInfo.instanceID, enemy);
                    enemies.Add(enemy);
                    _enemyHeroInfoDic.Add(heroInfo.instanceID, heroInfo);
                    #region create hp bar
                    HPBarView hpBarView = GetHPBarView(enemy);
                    hpBarView.transform.SetParent(_hpBoxTrans, false);
                    _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
                    enemy.hpBarView = hpBarView;
                    #endregion
                }
                if (callback != null)
                    callback(enemy);
            });
        }

        private void CloneEnemy(EnemyEntity enemyEntity)
        {
            HeroInfo heroInfo = null;
#if UNITY_EDITOR
            if (FightController.imitate)
                heroInfo = EnemyController.instance.GetImitateEnemyHeroInfo((int)enemyEntity.characterInfo.instanceID);
            else
                heroInfo = GetEnemyHeroInfo((uint)enemyEntity.characterInfo.instanceID);
#else
                heroInfo = GetEnemyHeroInfo((uint)enemyEntity.characterInfo.instanceID); 
#endif
            if (comboEnemyDic.ContainsKey(heroInfo.instanceID))
            {
                EnemyEntity enemy = comboEnemyDic[heroInfo.instanceID];
                enemy.anim.gameObject.SetActive(true);
                Common.Util.TransformUtil.SwitchLayer(enemy.transform, (int)LayerType.None);
            }
            else
            {
                Vector3 pos = Logic.Position.Model.PositionData.GetPos(enemyEntity.positionId);
                EnemyEntity enemy = EnemyEntity.CreateEnemyEntity(heroInfo);
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                if (enemy == null) return;
                enemy.characterName = heroInfo.heroData.name;
                Vector3 scale = heroInfo.heroData.scale;
                scale.x *= -1;
                enemy.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
                Transform root = enemy.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                enemy.positionId = enemyEntity.positionId;
                enemy.pos = pos;
                enemy.eulerAngles = ROTATE_1;
                enemy.scale = scale;
                enemy.anim.transform.localEulerAngles = ROTATE_2;
                enemy.anim.transform.localScale = SCALE_2;
                enemy.height = heroInfo.heroData.height;
                enemy.transform.SetParent(Map.Controller.MapController.instance.transform, false);
                enemy.CloneAttr(enemyEntity);
                enemy.characterInfo = enemyEntity.characterInfo;
                enemy.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                comboEnemyDic.Add(heroInfo.instanceID, enemy);
                Common.Util.TransformUtil.SwitchLayer(enemy.transform, (int)LayerType.None);
            }
        }

        public void CreateEnemyPlayer(FightPlayerInfo fightPlayerInfo, System.Action<bool> callback)
        {
            if (fightPlayerInfo == null)
            {
                if (callback != null)
                    callback(false);
                return;
            }
            if (fightPlayerInfo.pvePlayerProtoData.attr.hp <= 0) return;//版本限制，后会改为服务器不发送
            PlayerInfo playerInfo = fightPlayerInfo.playerInfo;
            uint posIndex = (uint)fightPlayerInfo.pvePlayerProtoData.posIndex + 100;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos(posIndex);
            pos = pos + new Vector3(_offset, 0, 0);
            PlayerEntity.CreateEnemyPlayerEntity(playerInfo, Map.Controller.MapController.instance.transform, (enemyPlayer) =>
            {
                if (enemyPlayer == null)
                {
                    if (callback != null)
                        callback(false);
                    return;
                }
                ShadersUtil.SetShaderKeyword(enemyPlayer, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                enemyPlayer.characterName = playerInfo.heroData.name;
                Vector3 scale = playerInfo.heroData.scale;
                scale.x *= -1;
                enemyPlayer.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);
                Transform root = enemyPlayer.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                enemyPlayer.positionId = posIndex;
                enemyPlayer.pos = pos;
                enemyPlayer.eulerAngles = ROTATE_1;
                enemyPlayer.scale = scale;
                enemyPlayer.anim.transform.localEulerAngles = ROTATE_2;
                enemyPlayer.anim.transform.localScale = SCALE_2;
                enemyPlayer.height = playerInfo.heroData.height;
                enemyPlayer.SetAttr(fightPlayerInfo.pvePlayerProtoData.attr);
                enemyDic.Add(playerInfo.instanceID, enemyPlayer);
                enemies.Add(enemyPlayer);
                _enemyPlayerInfo = playerInfo;
                #region create hp bar
                HPBarView hpBarView = GetHPBarView(enemyPlayer);
                hpBarView.transform.SetParent(_hpBoxTrans, false);
                _hpBarViewDic.Add(playerInfo.instanceID, hpBarView);
                enemyPlayer.hpBarView = hpBarView;
                #endregion
                if (callback != null)
                    callback(true);
            });
        }

        private void CloneEnemyPlayer(EnemyPlayerEntity playerEntity)
        {
            FightPlayerInfo fightPlayerInfo = FightProxy.instance.enemyFightPlayerInfo;
            PlayerInfo playerInfo = fightPlayerInfo.playerInfo;
            if (comboEnemyDic.ContainsKey(playerInfo.instanceID))
            {
                EnemyEntity player = comboEnemyDic[playerInfo.instanceID];
                player.anim.gameObject.SetActive(true);
                if (player is EnemyPlayerEntity)
                    (player as EnemyPlayerEntity).petEntity.gameObject.SetActive(true);
                Common.Util.TransformUtil.SwitchLayer(player.transform, (int)LayerType.None);
            }
            else
            {
                uint posIndex = playerEntity.positionId;
                Vector3 pos = Logic.Position.Model.PositionData.GetPos(posIndex);
                EnemyPlayerEntity enemyPlayer = EnemyPlayerEntity.CreateEnemyPlayerEntity(playerInfo, Map.Controller.MapController.instance.transform);
                ShadersUtil.SetShaderKeyword(enemyPlayer, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                if (enemyPlayer == null) return;
                enemyPlayer.characterName = playerInfo.heroData.name;
                enemyPlayer.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);
                Vector3 scale = playerInfo.heroData.scale;
                scale.x *= -1;
                enemyPlayer.positionId = posIndex;
                enemyPlayer.pos = pos;
                enemyPlayer.eulerAngles = ROTATE_1;
                enemyPlayer.scale = scale;
                enemyPlayer.anim.transform.localEulerAngles = ROTATE_2;
                enemyPlayer.anim.transform.localScale = SCALE_2;
                enemyPlayer.height = playerInfo.heroData.height;
                #region shadow
                Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);
                Transform root = enemyPlayer.rootNode.transform;
                Map.Controller.MapController.instance.AddTarget(root, size);
                #endregion
                enemyPlayer.CloneAttr(playerEntity);
                enemyPlayer.characterInfo = playerEntity.characterInfo;
                enemyPlayer.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                comboEnemyDic.Add(playerInfo.instanceID, enemyPlayer);
                Common.Util.TransformUtil.SwitchLayer(enemyPlayer.transform, (int)LayerType.None);
            }
        }

        #region fight imitate
#if UNITY_EDITOR
        private void CreateImimateEnemy(KeyValuePair<FormationPosition, HeroInfo> kvp)
        {
            HeroInfo heroInfo = kvp.Value;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos((uint)kvp.Key);
            pos = pos + new Vector3(_offset, 0, 0);
            EnemyEntity enemy = EnemyEntity.CreateEnemyEntity(heroInfo);
            if (enemy == null) return;
            ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            enemy.characterName = heroInfo.heroData.name;
            Vector3 scale = heroInfo.heroData.scale;
            scale.x *= -1;
            enemy.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(heroInfo);
            #region shadow
            Vector2 size = HeroEntity.GetShadowSize(heroInfo.heroData.shadowType);
            Transform root = enemy.rootNode.transform;
            Map.Controller.MapController.instance.AddTarget(root, size);
            #endregion
            enemy.positionId = (uint)kvp.Key;
            enemy.pos = pos;
            enemy.eulerAngles = ROTATE_1;
            enemy.scale = scale;
            enemy.anim.transform.localEulerAngles = ROTATE_2;
            enemy.anim.transform.localScale = SCALE_2;
            enemy.height = heroInfo.heroData.height;
            enemy.transform.SetParent(Map.Controller.MapController.instance.transform, false);
            Dictionary<RoleAttributeType, RoleAttribute> attrDic = Hero.HeroUtil.CalcHeroAttributesDic(heroInfo);
            enemy.SetAttr(attrDic);
            enemyDic.Add(heroInfo.instanceID, enemy);
            enemies.Add(enemy);
            _enemyHeroInfoDic.Add(heroInfo.instanceID, heroInfo);
            #region create hp bar
            HPBarView hpBarView = GetHPBarView(enemy);
            hpBarView.transform.SetParent(_hpBoxTrans, false);
            _hpBarViewDic.Add(heroInfo.instanceID, hpBarView);
            enemy.hpBarView = hpBarView;
            #endregion
        }

        private void CreateImitateEnemyPlayer(KeyValuePair<FormationPosition, PlayerInfo> kvp)
        {
            PlayerInfo playerInfo = kvp.Value;
            uint posIndex = (uint)kvp.Key;
            Vector3 pos = Logic.Position.Model.PositionData.GetPos(posIndex);
            pos = pos + new Vector3(_offset, 0, 0);
            EnemyPlayerEntity enemyPlayer = PlayerEntity.CreateEnemyPlayerEntity(playerInfo, Map.Controller.MapController.instance.transform);
            if (enemyPlayer == null) return;
            ShadersUtil.SetShaderKeyword(enemyPlayer, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            enemyPlayer.characterName = playerInfo.heroData.name;
            Vector3 scale = playerInfo.heroData.scale;
            scale.x *= -1;
            enemyPlayer.characterInfo = CharacterBaseInfo.CreateCharacterBaseInfo(playerInfo);
            #region shadow
            Vector2 size = HeroEntity.GetShadowSize(ShadowType.One);
            Transform root = enemyPlayer.rootNode.transform;
            Map.Controller.MapController.instance.AddTarget(root, size);
            #endregion
            enemyPlayer.positionId = posIndex;
            enemyPlayer.pos = pos;
            enemyPlayer.eulerAngles = ROTATE_1;
            enemyPlayer.scale = scale;
            enemyPlayer.anim.transform.localEulerAngles = ROTATE_2;
            enemyPlayer.anim.transform.localScale = SCALE_2;
            enemyPlayer.height = playerInfo.heroData.height;
            //enemyPlayer.transform.SetParent(Map.Controller.MapController.instance.transform, false);
            enemyPlayer.SetAttr(playerInfo);
            enemyDic.Add(playerInfo.instanceID, enemyPlayer);
            enemies.Add(enemyPlayer);
            _enemyPlayerInfo = playerInfo;
            #region create hp bar
            HPBarView hpBarView = GetHPBarView(enemyPlayer);
            hpBarView.transform.SetParent(_hpBoxTrans, false);
            _hpBarViewDic.Add(playerInfo.instanceID, hpBarView);
            enemyPlayer.hpBarView = hpBarView;
            #endregion
        }
#endif
        #endregion

        private HPBarView GetHPBarView(EnemyEntity enemy)
        {
            HPBarView hpBarView = null;
            if (_deadHpBarViewQueue.Count > 0)
            {
                hpBarView = _deadHpBarViewQueue.Dequeue();
                hpBarView.character = enemy;
            }
            else
                hpBarView = HPBarView.CreateHPBarView(enemy);
            return hpBarView;
        }

        public void Reborn(CharacterEntity character, float hpRate, float delay)
        {
            StartCoroutine(RebornCoroutine(character, hpRate, delay));
        }

        private IEnumerator RebornCoroutine(CharacterEntity character, float hpRate, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            if (character is EnemyEntity)
            {
                EnemyEntity enemy = character as EnemyEntity;
                if (deadEnemyDic.ContainsKey(enemy.characterInfo.instanceID))
                {
                    Effect.Model.EffectData effecData = Effect.Model.EffectData.GetEffectDataById(Effect.Controller.EffectController.TOMBSTONE_EFFECT_ID);
                    if (effecData != null)
                    {
                        string effectName = Effect.Controller.EffectController.instance.GetEffectName(enemy, effecData.effectName);
                        Effect.Controller.EffectController.instance.RemoveEffectByName(effectName);
                        if (_deadHeroTombStones.Contains(effectName))
                            _deadHeroTombStones.Remove(effectName);
                    }
                    enemy.Reborn(hpRate);
                    enemy.anim.gameObject.SetActive(true);
                    if (enemy is EnemyPlayerEntity)
                        (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(true);
                    AnimatorUtil.Play(enemy.anim, AnimatorUtil.IDLE_ID, 0, 0f);
                    //enemy.HP = (int)(hpRate * enemy.maxHP);
                    _deadCount--;
                    deadEnemyDic.Remove(enemy.characterInfo.instanceID);
                    enemyDic.Add(enemy.characterInfo.instanceID, enemy);
                    enemies.Add(enemy);
                    #region shadow
                    Vector2 size = Vector3.one;
                    if (enemy is EnemyPlayerEntity)
                        size = EnemyPlayerEntity.GetShadowSize(ShadowType.One);//主角暂时写死
                    else
                        size = HeroEntity.GetShadowSize(enemy.characterInfo.roleInfo.heroData.shadowType);
                    Transform root = enemy.rootNode.transform;
                    Map.Controller.MapController.instance.AddTarget(root, size);
                    #endregion
                    HPBarView hpBarView = GetHPBarView(enemy);
                    hpBarView.transform.SetParent(_hpBoxTrans, false);
                    _hpBarViewDic.Add(character.characterInfo.instanceID, hpBarView);
                    enemy.hpBarView = hpBarView;
                    PlayBossEffect(enemy);
                }
            }
        }

        private void FriendDead(EnemyEntity enemy)
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity e = enemies[i];
                foreach (var kvp in e.characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.FRIEND_DEAD, kvp.Key);
                    if (func != null)
                    {
                        func.Call(enemy, e, kvp.Value);
                    }
                }
                if (enemy.addedHaloBuff)
                    e.RemoveHaloBuff(enemy);
            }
            enemy.addedHaloBuff = false;
        }

        public void AddHaloBuff()
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity e = enemies[i];
                if (!e.addedHaloBuff)
                    e.AddHaloBuff();
            }
        }

        public void RemovePlayerHaloBuff(HeroEntity hero)
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity e = enemies[i];
                e.RemoveHaloBuff(hero);
            }
        }

        public void AddFormationBuff()
        {
            FormationInfo formationInfo = FightProxy.instance.enemyFormation;
            if (formationInfo == null) return;
            List<FormationAttrData> formationAtts = formationInfo.GetFormationDatas();// FormationAttrData.GetFormationDatas(formationInfo.id, formationInfo.level);
            for (int i = 0, count = formationAtts.Count; i < count; i++)
            {
                FormationAttrData formationAttrData = formationAtts[i];
                bool addPlayer = false;
                bool addEnemy = false;
                switch (formationAttrData.targetType)
                {
                    case TargetType.None:
                        addPlayer = true;
                        addEnemy = true;
                        break;
                    case TargetType.Ally:
                        addEnemy = true;
                        break;
                    case TargetType.Enemy:
                        addPlayer = true;
                        break;
                }
                switch (formationAttrData.effectType)
                {
                    case FormationEffectType.PhysicAtk:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.PhysicsAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.PhysicsAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.MagicAtk:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.MagicAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.MagicAttack, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.PhysicDef:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.PhysicsDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.PhysicsDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.MagicDef:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.MagicDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.MagicDefense, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Hit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Hit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Hit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Dodge:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Dodge, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Dodge, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }

                        }
                        break;
                    case FormationEffectType.Crit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Crit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Crit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.AntiCrit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.AntiCrit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.AntiCrit, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Block:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Block, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Block, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.AntiBlock:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.AntiBlock, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.AntiBlock, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.CritHurtAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.CritHurtAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.CritHurtAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.CritHurtDec:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.CritHurtDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.CritHurtDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.DamageAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.DamageAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.DamageAdd, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.DamageDec:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.DamageDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.DamageDec, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.Weakness:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.Weakness, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.Weakness, SkillLevelBuffAddType.None, BuffAddType.Fixed, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                    case FormationEffectType.HPLimit:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                float hpRate = formationInfo.GetFightFormationAttrValue(formationAttrData.effectType);
                                hero.AddBuff(hero, hero, null, null, BuffType.HPLimit, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, hpRate, 1, 1, true, false);
                                float hpValue = hpRate * hero.characterInfo.maxHP;
                                hero.SetTreatValue((uint)hpValue);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
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
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                BuffInfo buffInfo = hero.AddBuff(hero, hero, null, null, BuffType.TreatPercent, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                                buffInfo.interval = formationAttrData.interval;
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                BuffInfo buffInfo = enemy.AddBuff(enemy, enemy, null, null, BuffType.TreatPercent, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                                buffInfo.interval = formationAttrData.interval;
                            }
                        }
                        break;
                    case FormationEffectType.TreatAdd:
                        if (addPlayer)
                        {
                            for (int j = 0, jCount = PlayerController.instance.heros.Count; j < jCount; j++)
                            {
                                HeroEntity hero = PlayerController.instance.heros[j];
                                hero.AddBuff(hero, hero, null, null, BuffType.TreatAdd, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        if (addEnemy)
                        {
                            for (int j = 0, jCount = enemies.Count; j < jCount; j++)
                            {
                                EnemyEntity enemy = enemies[j];
                                enemy.AddBuff(enemy, enemy, null, null, BuffType.TreatAdd, SkillLevelBuffAddType.None, BuffAddType.Percent, 999, formationInfo.GetFightFormationAttrValue(formationAttrData.effectType), 1, 1, true, false);
                            }
                        }
                        break;
                }
            }
        }

        public void Remove(uint enemyId, float delay)
        {
            if (enemyDic.ContainsKey(enemyId))
            {
                EnemyEntity enemy = instance[enemyId];
                enemy.ClearBuff();
                enemyDic.Remove(enemyId);
                enemies.Remove(enemy);
                StartCoroutine(RemoveEnemyCoroutine(enemy, delay));
            }

            if (_hpBarViewDic.ContainsKey(enemyId))
            {
                HPBarView hpBarView = _hpBarViewDic[enemyId];
                hpBarView.character = null;
                _deadHpBarViewQueue.Enqueue(hpBarView);
                _hpBarViewDic.Remove(enemyId);
            }
        }

        private void CheckEnemyDic()
        {
            if (_allDead) return;
            //Debugger.Log("_deadEnemyDic count:{0},_enemyCount:{1},_deadCount:{2},", _deadEnemyDic.Count, _enemyCount, _deadCount);
            if (_deadEnemyDic.Count == _deadCount && _deadEnemyCheckedList.Count == _enemyCount && _deadCount == _enemyCount)
            {
                _allDead = true;
                ClearDeadEnemy();
                DestroyDeadHeroTombStones();
                switch (FightController.instance.fightType)
                {
                    case FightType.PVE:
                        Dungeon.Model.DungeonInfo dungeonInfo = Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID);
                        if (dungeonInfo.star > 0)
                        {
                            DataMessageHandler.DataMessage_TeamAllDead(CharacterType.Enemy);
                        }
                        else
                        {
                            TeamData pveTeamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                            Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(true);
                            Logic.UI.Dialog.View.DialogView.Open(pveTeamData.endDialogID, () =>
                            {
                                Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(false);
                                DataMessageHandler.DataMessage_TeamAllDead(CharacterType.Enemy);
                            });
                        }
                        break;
                    case FightType.DailyPVE:
                    case FightType.Expedition:
                    case FightType.WorldTree:
                    case FightType.WorldBoss:
                    case FightType.Arena:
                    case FightType.PVP:
                    case FightType.FriendFight:
                    case FightType.ConsortiaFight:
                    case FightType.FirstFight:
                    case FightType.MineFight:
                        DataMessageHandler.DataMessage_TeamAllDead(CharacterType.Enemy);
                        break;
#if UNITY_EDITOR
                    case FightType.Imitate:
                        DataMessageHandler.DataMessage_TeamAllDead(CharacterType.Enemy);
                        break;
#endif
                }
            }
        }

        private IEnumerator RemoveEnemyCoroutine(EnemyEntity enemy, float delay)
        {
            Effect.Controller.EffectController.instance.RemoveEffects(enemy, 0.5f);
            FriendDead(enemy);
            if (enemy.addedHaloBuff)
                PlayerController.instance.RemoveEnemyHaloBuff(enemy);
            enemy.hpBarView = null;
            _deadEnemyDic.Add(enemy.characterInfo.instanceID, enemy);
            int result = 0;
            foreach (var kvp in enemy.characterInfo.passiveIdDic)
            {
                int r = 0;
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.DEAD, kvp.Key);
                if (func != null)
                {
                    object[] rs = func.Call(enemy, kvp.Value);
                    if (rs != null & rs.Length > 0)
                    {
                        int.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
            }
            if (result == 0)
            {
                if (enemies.Count == 0)
                {
                    if (FightProxy.instance.HasNextEnemies())
                        DataMessageHandler.DataMessage_PlayNextFightEffect();
                }
                #region dead effect
                uint effectId = (uint)enemy.characterInfo.roleInfo.heroData.dieEffectId;
                if (effectId != 0)
                {
                    Effect.Model.EffectInfo deadEffectInfo = new Effect.Model.EffectInfo(effectId);
                    if (deadEffectInfo != null)
                    {
                        deadEffectInfo.pos = enemy.pos + deadEffectInfo.effectData.offset;
                        deadEffectInfo.character = enemy;
                        deadEffectInfo.target = enemy;
                        deadEffectInfo.delay = deadEffectInfo.effectData.delay;
                        Effect.Controller.EffectController.instance.PlayNoSkillEffect(deadEffectInfo, true);
                    }
                }
                #endregion
                if (enemies.Count == 0)
                {
                    if (delay > 0)
                        yield return new WaitForSeconds(delay);
                    if (enemy.anim)
                        enemy.anim.gameObject.SetActive(false);
                    if (enemy is EnemyPlayerEntity)
                        (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(false);
                    if (enemy.rootNode)
                        Map.Controller.MapController.instance.RemoveTarget(enemy.rootNode.transform);
                }
                else
                {
                    if (delay > 0)
                        yield return new WaitForSeconds(delay);
                    if (enemy.anim)
                        enemy.anim.gameObject.SetActive(false);
                    if (enemy is EnemyPlayerEntity)
                        (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(false);
                    if (enemy.rootNode)
                        Map.Controller.MapController.instance.RemoveTarget(enemy.rootNode.transform);
                    #region tombstone effect
                    Effect.Model.EffectInfo effectInfo = new Effect.Model.EffectInfo(Effect.Controller.EffectController.TOMBSTONE_EFFECT_ID);
                    effectInfo.pos = enemy.pos + effectInfo.effectData.offset;
                    effectInfo.character = enemy;
                    effectInfo.target = enemy;
                    effectInfo.delay = effectInfo.effectData.delay;
                    effectInfo.rotateAngles = new Vector3(0, 180, 0);
                    Effect.Controller.EffectController.instance.PlayNoSkillEffect(effectInfo);
                    _deadHeroTombStones.Add(Effect.Controller.EffectController.instance.GetEffectName(enemy, effectInfo.effectData.effectName));
                    #endregion
                }
                if (!_deadEnemyCheckedList.Contains(enemy.characterInfo.instanceID))
                    _deadEnemyCheckedList.Add(enemy.characterInfo.instanceID);
                _deadCount++;
                CheckEnemyDic();
            }
            else
                _deadCount++;
        }

        private void ShowDeadHeroTombStones(bool isShow)
        {
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

        public void RemoveAttackSkill(SkillInfo skillInfo)
        {
            //List<EnemyEntity> list = enemies;
            //EnemyEntity enemy = null;
            //for (int i = 0, count = list.Count; i < count; i++)
            //{
            //    enemy = list[i];
            //}
        }

        public void ShowHPBarViews(bool show)
        {
            HPBarView[] hpbarViews = _hpBarViewDic.GetValueArray();
            for (int i = 0, count = hpbarViews.Length; i < count; i++)
            {
                hpbarViews[i].gameObject.SetActive(show);
            }
        }

        public void ClearEnemy()
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity enemy = enemies[i];
                enemy.anim.gameObject.SetActive(true);
                if (enemy is EnemyPlayerEntity)
                    (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(true);
                Pool.Controller.PoolController.instance.Despawn(enemy.name, enemy);
            }
            enemies.Clear();
            enemyDic.Clear();
            ClearComboEnemies(true);

            _skillInfos.Clear();
            ClearDeadEnemy();
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

            _enemyHeroInfoDic.Clear();
            _enemyPlayerInfo = null;
        }

        public void ClearComboEnemies(bool destroy)
        {
            List<EnemyEntity> comboEnemyList = comboEnemyDic.GetValues();
            for (int i = 0, count = comboEnemyList.Count; i < count; i++)
            {
                EnemyEntity enemy = comboEnemyList[i];
                Map.Controller.MapController.instance.RemoveTarget(enemy.rootNode.transform);
                if (destroy)
                {
                    enemy.anim.gameObject.SetActive(true);
                    if (enemy is EnemyPlayerEntity)
                        (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(true);
                    Pool.Controller.PoolController.instance.Despawn(enemy.name, enemy);
                }
                else
                {
                    enemy.anim.gameObject.SetActive(false);
                    if (enemy is EnemyPlayerEntity)
                        (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(false);
                }
                Effect.Controller.EffectController.instance.RemoveEffects(enemy, 0f);
            }
            comboEnemyList.Clear();
            comboEnemyList = null;
            if (destroy)
                comboEnemyDic.Clear();
        }

        private void ClearDeadEnemy()
        {
            _enemyCount = 0;
            _deadCount = 0;
            List<EnemyEntity> deadEnemyList = _deadEnemyDic.GetValues();
            for (int i = 0, count = deadEnemyList.Count; i < count; i++)
            {
                //GameObject.Destroy(deadEnemyList[i].gameObject);
                EnemyEntity enemy = deadEnemyList[i];
                enemy.anim.gameObject.SetActive(true);
                if (enemy is EnemyPlayerEntity)
                    (enemy as EnemyPlayerEntity).petEntity.gameObject.SetActive(true);
                Pool.Controller.PoolController.instance.Despawn(enemy.name, enemy);
                //Debugger.Log("pool name:{0}  {1}", enemy.name, (spawnPool == null));
                //spawnPool.Despawn(enemy.transform);
            }
            deadEnemyList.Clear();
            deadEnemyList = null;
            _deadEnemyDic.Clear();
            _deadEnemyCheckedList.Clear();
            _enemyHeroInfoDic.Clear();
            _enemyPlayerInfo = null;
        }

        public void Run_Scene()
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                FightController.instance.Run_Scene(enemies[i].characterInfo.instanceID, false);
            }
            StartCoroutine(FinishRunCoroutine(FightController.RUN_SCENE_TIME));
        }

        public void ShowHPBar(EnemyEntity ignoreEnemy, bool show)
        {
            List<EnemyEntity> enemyList = enemies;
            for (int i = 0, count = enemyList.Count; i < count; i++)
            {
                EnemyEntity enemy = enemyList[i];
                if (enemy == ignoreEnemy)
                    continue;
                enemy.hpBarView.Show(show);
            }
        }

        private IEnumerator FinishRunCoroutine(float delay)
        {
            yield return new WaitForSeconds(2f);
            delay -= 2f;
            if (delay <= 0)
                delay = 0;
            yield return new WaitForSeconds(delay);
            switch (FightController.instance.fightType)
            {
                case FightType.PVE:
                    Dungeon.Model.DungeonInfo dungeonInfo = Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID);
                    if (dungeonInfo.star > 0)
                    {
                        DataMessageHandler.DataMessage_FinishRun(CharacterType.Enemy);
                    }
                    else
                    {
                        TeamData pveTeamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                        Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(true);
                        Logic.UI.Dialog.View.DialogView.Open(pveTeamData.preDialogID, () =>
                        {
                            Logic.UI.FightTips.Controller.FightTipsController.instance.SetPause(false);
                            DataMessageHandler.DataMessage_FinishRun(CharacterType.Enemy);
                        });
                    }
                    break;
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.Arena:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.ConsortiaFight:
                case FightType.FirstFight:
                case FightType.SkillDisplay:
                case FightType.MineFight:
                    DataMessageHandler.DataMessage_FinishRun(CharacterType.Enemy);
                    break;
#if UNITY_EDITOR
                case FightType.Imitate:
                    DataMessageHandler.DataMessage_FinishRun(CharacterType.Enemy);
                    break;
#endif
            }
        }

        public void PlayBossEffect()
        {
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity enemy = enemies[i];
                PlayBossEffect(enemy);
            }
        }

        private void PlayBossEffect(EnemyEntity enemy)
        {
            if (isBoss(enemy.characterInfo.instanceID))
            {
                EffectInfo effectInfo = new EffectInfo(EffectController.BOSS_EFFECT_ID);
                if (effectInfo.effectData == null) return;
                effectInfo.character = enemy;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.LockPart:
                        effectInfo.delay = effectInfo.effectData.delay;
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, enemy.transform);
                        effectInfo.target = enemy;
                        break;
                }
                EffectController.instance.PlayEffect(effectInfo);
            }
        }

        public bool isBoss(uint id)
        {
#if UNITY_EDITOR
            if (Fight.Controller.FightController.instance.fightType == FightType.Imitate)
            {
                return Fight.Controller.FightController.instance.isBossImitate(id);
            }
#endif
            EnemyEntity enemy = null;
            if (enemyDic.TryGetValue(id, out enemy))
            {
                if (FightProxy.instance.IsBoss((int)enemy.positionId))
                    return true;
            }
            HeroInfo heroInfo = null;
            if (_enemyHeroInfoDic.TryGetValue(id, out heroInfo))
            {
                return heroInfo.heroData.hero_type == 4;//boss
            }
            return false;
        }

        public EnemyEntity this[uint id]
        {
            get
            {
                if (enemyDic.ContainsKey(id))
                    return enemyDic[id];
                return null;
            }
        }

        public EnemyEntity GetDeadHeroById(uint id)
        {
            if (deadEnemyDic.ContainsKey(id))
                return deadEnemyDic[id];
            return null;
        }

        public void SwitchEnemies(int layer, EnemyEntity ignoreEnemy = null)
        {
            List<EnemyEntity> list = EnemyController.instance.enemies;
            for (int i = 0, count = list.Count; i < count; i++)
            {
                EnemyEntity enemy = list[i];
                if (enemy == ignoreEnemy) continue;
                TransformUtil.SwitchLayer(enemy.transform, layer);
                EffectController.instance.SwitchEffect(enemy, layer);
            }
        }

        #region combo
        public void ResetComboSkill(uint userId, uint skillId)
        {
            EnemyEntity enemy = this[userId];
            if (enemy)
                enemy.ResetCD(enemy.GetSkillInfoById(skillId));
        }
        public bool HasFloatEnemies()
        {
            if (!enemyFloatable) return false;
            bool result = false;
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                result |= CanFlaot(enemies[i]);
            }
            return result;
        }

        public bool HasTumbleEnemies()
        {
            if (!enemyFloatable) return false;
            bool result = false;
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                result |= CanTumble(enemies[i]);
            }
            return result;
        }

        public bool CanFlaot(EnemyEntity enemy)
        {
            bool result = false;
            result = CanSkill1Float(enemy) || CanSkill2Float(enemy);
            return result;
        }

        public bool CanSkill1Float(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill1 || enemy.canPlaySkill1)
            {
                result |= SkillUtil.AttackableFloat(enemy.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanSkill2Float(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill2 || enemy.canPlaySkill2)
            {
                result |= SkillUtil.AttackableFloat(enemy.characterInfo.skillInfo2);
            }
            return result;
        }

        public bool CanTumble(EnemyEntity enemy)
        {
            bool result = false;
            result = CanSkill1Tumble(enemy) || CanSkill2Tumble(enemy);
            return result;
        }

        public bool CanSkill1Tumble(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill1 || enemy.canPlaySkill1)
            {
                result |= SkillUtil.AttackableTumble(enemy.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanSkill2Tumble(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill2 || enemy.canPlaySkill2)
            {
                result |= SkillUtil.AttackableTumble(enemy.characterInfo.skillInfo2);
            }
            return result;
        }


        public bool HasCanOrderFloatEnemies()
        {
            bool result = false;
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                result |= CanOrderFlaot(enemies[i]);
            }
            return result;
        }

        public bool HasCanOrderTumbleEnemies()
        {
            bool result = false;
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                result |= CanOrderTumble(enemies[i]);
            }
            return result;
        }

        public bool CanOrderFlaot(EnemyEntity enemy)
        {
            bool result = false;
            result = CanOrderSkill1Float(enemy) || CanOrderSkill2Float(enemy);
            return result;
        }

        public bool CanOrderSkill1Float(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill1)
            {
                result |= SkillUtil.AttackableFloat(enemy.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanOrderSkill2Float(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill2)
            {
                result |= SkillUtil.AttackableFloat(enemy.characterInfo.skillInfo2);
            }
            return result;
        }

        public bool CanOrderTumble(EnemyEntity enemy)
        {
            bool result = false;
            result = CanOrderSkill1Tumble(enemy) || CanOrderSkill2Tumble(enemy);
            return result;
        }

        public bool CanOrderSkill1Tumble(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill1)
            {
                result |= SkillUtil.AttackableTumble(enemy.characterInfo.skillInfo1);
            }
            return result;
        }

        public bool CanOrderSkill2Tumble(EnemyEntity enemy)
        {
            bool result = false;
            if (enemy.canOrderSkill2)
            {
                result |= SkillUtil.AttackableTumble(enemy.characterInfo.skillInfo2);
            }
            return result;
        }
        #endregion


#if UNITY_EDITOR
        void OnGUI()
        {
            if (!Game.GameConfig.instance.showEnemySkillButton) return;
            int j = 0, heigh = 20;
            GUI.Label(new Rect(800, heigh * j, 200, heigh), "敌方列表:");
            j++;
            foreach (var kvp in EnemyController.instance.enemyDic)
            {
                GUI.Label(new Rect(800, heigh * j, 400, heigh), "ID:" + kvp.Key.ToString() + " " + kvp.Value.characterName + " " + kvp.Value.status.ToString());
                j++;
                if (kvp.Value.characterInfo.skillId1 != 0)
                {
                    GUI.Label(new Rect(800, heigh * j, 200, heigh), "skill1");
                    j++;
                    GUI.HorizontalSlider(new Rect(800, heigh * j, 200, heigh), kvp.Value.skill1CD / kvp.Value.characterInfo.skillInfo1.skillData.CD, 0f, 1f);
                    j++;
                    if (kvp.Value.canOrderSkill1)
                    {
                        if ((GUI.Button(new Rect(800, heigh * j, 200, heigh * 2), "order skill1")))
                        {
                            FightController.instance.OrderEnemySkill(kvp.Value.characterInfo.instanceID, kvp.Value.characterInfo.skillInfo1.skillData.skillId, true);
                        }
                    }
                    j++;
                    j++;
                }
                if (kvp.Value.characterInfo.skillId2 != 0)
                {
                    GUI.Label(new Rect(800, heigh * j, 200, heigh), "skill12");
                    j++;
                    GUI.HorizontalSlider(new Rect(800, heigh * j, 200, heigh), kvp.Value.skill2CD / kvp.Value.characterInfo.skillInfo2.skillData.CD, 0f, 1f);
                    j++;
                    if (kvp.Value.canOrderSkill2)
                    {
                        if ((GUI.Button(new Rect(800, heigh * j, 200, heigh * 2), "order skill2")))
                        {
                            FightController.instance.OrderEnemySkill(kvp.Value.characterInfo.instanceID, kvp.Value.characterInfo.skillInfo2.skillData.skillId, true);
                        }
                    }
                    j++;
                    j++;
                }
                j++;
            }
        }
#endif
    }
}