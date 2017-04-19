using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Character.Controller;
using Logic.Character;
using Common.Util;
using Logic.Skill.Model;
using Logic.Fight.Controller;
namespace Logic.Net.Controller
{
    public struct SkillStruct
    {
        public uint id;
        public uint skillId;
        public float time;
        public bool isPlayer;

        public SkillStruct(uint id, uint skillId, bool isPlayer, float time)
        {
            this.id = id;
            this.skillId = skillId;
            this.isPlayer = isPlayer;
            this.time = time;
        }

        public SkillStruct(uint id, uint skillId, bool isPlayer)
        {
            this.id = id;
            this.skillId = skillId;
            this.isPlayer = isPlayer;
            this.time = 0;
        }

        public SkillStruct(bool isPlayer, float time)
        {
            this.id = 0;
            this.skillId = 0;
            this.isPlayer = isPlayer;
            this.time = 0;
        }
    }

    public class VirtualServer : SingletonMono<VirtualServer>
    {
        //角色ID+"_"+技能ID做key
        public Queue<KeyValuePair<uint, uint>> skillWaitingOrders = new Queue<KeyValuePair<uint, uint>>();
        public Dictionary<string, uint> skillWaitingDic = new Dictionary<string, uint>();
        public Queue<string> skillWaitingQueue = new Queue<string>();
        public Queue<KeyValuePair<uint, uint>> hitSkillQueue = new Queue<KeyValuePair<uint, uint>>();
        public Dictionary<string, SkillStruct> bootSkillWaitingDic = new Dictionary<string, SkillStruct>();
        public Dictionary<string, uint> skillingDic = new Dictionary<string, uint>();
        public Dictionary<string, List<KeyValuePair<uint, uint>>> beLcokedDic = new Dictionary<string, List<KeyValuePair<uint, uint>>>();
        public Dictionary<uint, uint> aeonSkillDic = new Dictionary<uint, uint>();//召唤兽
        public Dictionary<uint, uint> aeonSkillingDic = new Dictionary<uint, uint>();//召唤兽
        private Dictionary<uint, float> _deadDic = new Dictionary<uint, float>();//死亡列表
        public Dictionary<string, float> skillWaitFinishDic = new Dictionary<string, float>();//等待完成列表
        //public Dictionary<uint, long> releaseTimeDic = new Dictionary<uint, long>();
#if UNITY_EDITOR
        //public Dictionary<uint, long> orderTimeDic = new Dictionary<uint, long>();
        //public Dictionary<uint, long> playSkillTimeDic = new Dictionary<uint, long>();
        //public Dictionary<uint, long> calcTimeDic = new Dictionary<uint, long>();
        public List<uint> orderSkills = new List<uint>();
        public List<uint> finishSkills = new List<uint>();
#endif
        public const float FORCE_CLEAR_TIME = 4f;
        public const int SKILL_DELAY_MAX_TIME = 1000;//ms
        #region imitate
#if UNITY_EDITOR
        private int _imitateWave = 1;
#endif
        #endregion
        void Awake()
        {
            instance = this;
        }

        #region bool field
        private bool _initPlayer = false;
        private bool _initEnemy = false;
        private bool _isPlayerInPlace = false;
        private bool _isEnemyInPlace = false;
        private bool _victory = false;
        private bool _enemyAutoOrder = true;
        private bool _hangup = false;
        private bool _hanguped = false;
        private bool _interlude = false;//过场
        public bool victory
        {
            private set
            {
                _victory = value;
            }
            get
            {
                return _victory;
            }
        }

        public bool canFight
        {
            get
            {
                return playerReady && enemyReady;
            }
        }

        public bool playerReady
        {
            get
            {
                return _initPlayer && _isPlayerInPlace;
            }
            set
            {
                _initPlayer = _isPlayerInPlace = value;
            }
        }

        public bool enemyReady
        {
            get
            {
                return _initEnemy && _isEnemyInPlace;
            }
            set
            {
                _initEnemy = _isEnemyInPlace = value;
            }
        }

        public bool enemyAutoOrder
        {
            get
            {
                return _enemyAutoOrder;
            }
        }

        public bool hangup
        {
            get
            {
                return _hangup;
            }
        }

        public bool interlude
        {
            get
            {
                return _interlude;
            }
        }
        #endregion

        #region public function
        public void ReadyFight()
        {
            #region imitate
#if UNITY_EDITOR
            if (Fight.Controller.FightController.imitate)
                _imitateWave = 1;
#endif
            #endregion
            ClearData();
            VirtualServerController.instance.runnig = true;
            _initPlayer = false;
            _initEnemy = false;
            victory = false;
            DataMessageHandler.DataMessage_ReadyScene();
            _orderCount = 0;
        }

        public void ReadySceneSuccess()
        {
            InitPlayer();
            InitEnemy();
        }

        public void FightHangupOrder()
        {
            _hangup = true;
            _hanguped = false;
        }

        public void FightHangup()
        {
            if (_hanguped) return;
            _hanguped = true;
            TickCD(false);
            DataMessageHandler.DataMessage_FightHangup();
        }

        public void FightRegainOrder()
        {
            SortHitSkill(PlayerController.instance.heros, EnemyController.instance.enemies);
            PlayerController.instance.SortSkillInfos();
            EnemyController.instance.SortSkillInfos();
            _hangup = false;
            _hanguped = false;
            TickCD(true);
        }

        //public void CalcPVPSkillDelay(uint skillId)
        //{
        //    if (VirtualServer.instance.releaseTimeDic.ContainsKey(skillId))
        //    {
        //        long interval = Common.GameTime.Controller.TimeController.instance.ServerTimeTicksMillisecond - VirtualServer.instance.releaseTimeDic[skillId];
        //        //Debugger.Log("releaseTime:{0},real releaseTime:{1},interval:{2}", VirtualServer.instance.releaseTimeDic[skillId], Common.GameTime.Controller.TimeController.instance.ServerTimeTicksMillisecond, interval);
        //        if (interval >= SKILL_DELAY_MAX_TIME)
        //            PVPFightController.instance.CLIENT2LOBBY_FIGHT_CMD_SYN_REVISE_REQ();
        //    }
        //}

        public void ClearConsortiaSkills()
        {
            skillWaitingOrders.Clear();
        }

        public void OrderComboSkill(uint id, uint skillId, bool isPlayer)
        {
            DataMessageHandler.OrderComboSkill(id, skillId, isPlayer);
        }

        int _orderCount = 0;
        public void OrderConsortiaSkill(uint characterId, uint skillId, long releaseTime, bool forceFirst)
        {
            //if (skillWaitingDic.Count == 0 && skillingDic.Count == 0)
            //{
            //    _orderCount++;
            //    OrderSkill(characterId, skillId, forceFirst);
            //    Debugger.LogError("order skill:{0} {1} ", characterId, skillId);
            //}
            //else
            skillWaitingOrders.Enqueue(new KeyValuePair<uint, uint>(characterId, skillId));
            //releaseTimeDic[skillId] = releaseTime;
        }

        private void OrderConsortiaSkill()
        {
            if (skillWaitingOrders.Count > 0)
            {
                KeyValuePair<uint, uint> kvp = skillWaitingOrders.Dequeue();
                _orderCount++;
                if (!DataMessageHandler.DataMessage_OrderConsortiaSkill(kvp.Key, kvp.Value))
                    OrderSkill(kvp.Key, kvp.Value, false);
                Debugger.LogError("order skill:{0} {1} ", kvp.Key, kvp.Value);
                //if (skillWaitingOrders.Count > 0)
                //{
                //    MechanicsType mt = Skill.SkillUtil.GetSkillMechanicsType(kvp.Value);
                //    if (mt != MechanicsType.None)
                //    {
                //        List<KeyValuePair<uint, uint>> list = new List<KeyValuePair<uint, uint>>();
                //        for (int i = 0, count = skillWaitingOrders.Count; i < count; i++)
                //        {
                //            KeyValuePair<uint, uint> k = skillWaitingOrders[i];
                //            SkillData skillData = SkillData.GetSkillDataById(k.Value);
                //            if (skillData == null) continue;
                //            switch (mt)
                //            {
                //                case MechanicsType.Float:
                //                    if (Logic.Skill.SkillUtil.AttackableFloat(skillData))
                //                        list.Add(k);
                //                    break;
                //                case MechanicsType.Tumble:
                //                    if (Logic.Skill.SkillUtil.AttackableTumble(skillData))
                //                        list.Add(k);
                //                    break;
                //            }
                //        }
                //    }
                //}
            }
        }

        public void OrderSkill(uint characterId, uint skillId, bool forceFirst)
        {
#if UNITY_EDITOR
            if (FightController.instance.fightType == FightType.ConsortiaFight)
                Debugger.LogError("client order skill count:{0}", _orderCount);
#endif
            string key = StringUtil.ConcatNumber(characterId, skillId);
            skillWaitingDic.TryAdd(key, characterId);
            lock (skillWaitingQueue)
            {
                if (forceFirst)
                {
                    if (skillWaitingQueue.Count > 0)
                    {
                        Queue<string> tempQueue = new Queue<string>();
                        while (skillWaitingQueue.Count > 0)
                        {
                            tempQueue.Enqueue(skillWaitingQueue.Dequeue());
                        }
                        skillWaitingQueue.Enqueue(key);
                        while (tempQueue.Count > 0)
                        {
                            skillWaitingQueue.Enqueue(tempQueue.Dequeue());
                        }
                    }
                    else
                        skillWaitingQueue.Enqueue(key);
                }
                else
                    skillWaitingQueue.Enqueue(key);
            }
            //orderTimeDic[skillId] = Common.Util.TimeUtil.GetTimeStamp();
#if UNITY_EDITOR
            orderSkills.Add(skillId);
#endif
            //VirtualServerController.instance.PlaySkillCalc();
        }

        public void PlayAeonSkill(uint id, uint skillId)
        {
            //if (_interlude)
            //{
            //    ResetSkillOrder(skillId);
            //    return;
            //}
            //TickCD(false);
            aeonSkillDic.Add(skillId, id);
        }

        public void PlayBootSkill(uint characterId, uint skillId, bool isPlayer)
        {
            DataMessageHandler.DataMessage_PlayBootSkill(characterId, skillId, isPlayer);
        }

        //取消技能预约
        public void BreakSkill(uint characterId)
        {
            List<string> keys = skillWaitingDic.GetKeys();
            for (int i = 0, length = keys.Count; i < length; i++)
            {
                string key = keys[i];
                if (key.StartsWith(characterId.ToString() + "_"))
                    skillWaitingDic.TryDelete(key);
            }
        }

        public void BreakBootSkill(uint characterId)
        {
            //Debugger.Log(characterId + " has been breaken");
            List<string> keys = bootSkillWaitingDic.GetKeys();
            for (int i = 0, length = keys.Count; i < length; i++)
            {
                string key = keys[i];
                if (key.StartsWith(characterId.ToString() + "_"))
                    bootSkillWaitingDic.TryDelete(key);
            }
        }

        public void PlayPlayerSkill(uint id, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillId)
        {
            SkillData skillData = SkillData.GetSkillDataById(skillId);
            if (skillData.skillType == SkillType.Skill || skillData.skillType == SkillType.Aeon)
                TickCD(false);
            DataMessageHandler.DataMessage_PlayPlayerSkill(id, timelineList, skillId);
        }

        public void PlayEnemySkill(uint id, List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList, uint skillId)
        {
            SkillData skillData = SkillData.GetSkillDataById(skillId);
            if (skillData.skillType == SkillType.Skill || skillData.skillType == SkillType.Aeon)
                TickCD(false);
            DataMessageHandler.DataMessage_PlayEnemySkill(id, timelineList, skillId);
        }

        public void InitOK(CharacterType characterType)
        {
            if (characterType == CharacterType.Player)
            {
                _initPlayer = true;
            }
            else if (characterType == CharacterType.Enemy)
            {
                _initEnemy = true;
            }
            if (FightController.instance.fightType == FightType.SkillDisplay)
            {
                playerReady = _initPlayer;
                enemyReady = _initEnemy;
                if (canFight)
                    StartFight();
            }
            else
                DataMessageHandler.DataMessage_Run(characterType);
        }

        public void FinishRun(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Player:
                    _isPlayerInPlace = true;
                    DataMessageHandler.DataMessage_EndRun(type);
                    if (Fight.Model.FightProxy.instance.CurrentTeamIndex > 0)
                        InitEnemy();
                    break;
                case CharacterType.Enemy:
                    //if (Fight.Model.FightProxy.instance.CurrentTeamIndex == 0)
                    //    DataMessageHandler.DataMessage_PlayFightStartEffect();
                    _isEnemyInPlace = true;
                    break;
            }
            if (canFight)
            {
                switch (Fight.Controller.FightController.instance.fightType)
                {
                    case FightType.PVE:
                    case FightType.Arena:
                    case FightType.DailyPVE:
                    case FightType.Expedition:
                    case FightType.WorldTree:
                    case FightType.WorldBoss:
                    case FightType.SkillDisplay:
                    case FightType.PVP:
                    case FightType.FriendFight:
                    case FightType.MineFight:
#if UNITY_EDITOR
                    case FightType.Imitate:
#endif
                        if (Fight.Model.FightProxy.instance.CurrentTeamIndex == 0)
                            DataMessageHandler.DataMessage_PlayFightStartEffect();
                        break;
                    case FightType.FirstFight:
                        break;
                    case FightType.ConsortiaFight:
                        //ConsortiaFightController.instance.CLIENT2LOBBY_FIGHT_START_REQ(Fight.Model.FightProxy.instance.fightId);
                        break;
                }
                StartFight();
            }
        }

        public void TeamAllDead(CharacterType characterType)
        {
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.FirstFight:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    switch (characterType)
                    {
                        case CharacterType.Enemy:
                            enemyReady = false;
                            _interlude = true;
                            StartCoroutine(FinishLevelCoroutine());
                            break;
                        case CharacterType.Player:
                            playerReady = false;
                            PlayerFail();
                            Debugger.Log("fight fail ------------------");
                            break;
                    }
                    break;
                case FightType.ConsortiaFight:
                    StopCoroutine("ConsortiaResultCoroutine");
                    StartCoroutine("ConsortiaResultCoroutine");
                    break;
            }
        }

        private IEnumerator ConsortiaResultCoroutine()
        {
            while (true)
            {
                if (Fight.Model.FightProxy.instance.consortiaOver)
                {
                    if (Fight.Model.FightProxy.instance.consortiaResult)
                        PlayerVictory();
                    else
                        PlayerFail();
                    break;
                }
                yield return null;
            }
        }

        public void PlayerVictory(FightOverType fightOverType = FightOverType.Normal)
        {
            VirtualServerController.instance.runnig = false;
            DataMessageHandler.DataMessage_FightResult(true, fightOverType);
        }

        public void PlayerFail(FightOverType fightOverType = FightOverType.Normal)
        {
            VirtualServerController.instance.runnig = false;
            DataMessageHandler.DataMessage_FightResult(false, fightOverType);
        }

        public void ForceFinishSkill(uint characterId, uint skillId, bool isPlayer)
        {
            DataMessageHandler.DataMessage_ForceFinishedSkill(characterId, skillId, isPlayer);
        }

#if UNITY_EDITOR
        [ContextMenu("calc unplay skills")]
        private void CalcUnplaySkills()
        {
            for (int i = 0, iCount = finishSkills.Count; i < iCount; i++)
            {
                for (int j = 0, jCount = orderSkills.Count; j < jCount; j++)
                {
                    if (finishSkills[i] == orderSkills[j])
                    {
                        orderSkills.RemoveAt(j);
                        break;
                    }
                }
            }
            for (int i = 0, count = orderSkills.Count; i < count; i++)
            {
                Debugger.Log("unplay skill:{0}", orderSkills[i]);
            }
        }
#endif

        public void FinishSkill(uint skillId, uint characterId, bool isPlayer)
        {
            //Debugger.LogError("finish skillID:" + skillId.ToString());            
#if UNITY_EDITOR
            finishSkills.Add(skillId);
#endif
            if (aeonSkillingDic.ContainsKey(skillId))//超级技能
            {
                aeonSkillingDic.Remove(skillId);
                TickCD(true);
            }
            string key = StringUtil.ConcatNumber(characterId, skillId);
            beLcokedDic.TryDelete(key);
            skillWaitFinishDic.TryDelete(key);
            skillingDic.TryDelete(key);


            CharacterEntity character = null;
            //检测自己是否死亡
            if (isPlayer)
                character = PlayerController.instance[characterId];
            else
                character = EnemyController.instance[characterId];
            if (character && character.isDead)
                VerifyDead(character, isPlayer);

            bool needTick = true;
            if (aeonSkillingDic.Count > 0)
                needTick = false;
            else
            {
                string[] keys = skillingDic.GetKeyArray();
                for (int i = 0, count = keys.Length; i < count; i++)
                {
                    KeyValuePair<uint, uint> kvp = keys[i].SplitToKeyValuePair<uint, uint>();
                    SkillData skillData = SkillData.GetSkillDataById(kvp.Value);
                    if (skillData.skillType == SkillType.Skill)
                        needTick &= false;
                }
            }
            if (needTick && !_hanguped)
                TickCD(true);
            switch (FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.FirstFight:
                case FightType.SkillDisplay:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
                    break;
                case FightType.ConsortiaFight:
                    List<int> deadHeroList = Fight.Model.FightProxy.instance.GetConsortiaDeadHeroList((int)characterId, (int)skillId);
                    if (deadHeroList != null && deadHeroList.Count > 0)
                    {
                        for (int i = 0, count = deadHeroList.Count; i < count; i++)
                        {
                            CharacterEntity c = CharacterUtil.FindTarget((uint)deadHeroList[i]);
                            if (c != null && !c.isDead)
                                DataMessageHandler.DataMessage_Dead(c.characterInfo.instanceID, c.isPlayer);
                        }
                    }
                    OrderConsortiaSkill();
                    Fight.Model.FightProxy.instance.RemoveConsortiaFightData((int)characterId, (int)skillId);
                    break;
            }
            //VirtualServerController.instance.PlaySkillCalc();
        }

        public void FinishedSkillMechanics(uint skillId, uint characterId, bool isPlayer)
        {
            string key = StringUtil.ConcatNumber(characterId, skillId);
            if (beLcokedDic.ContainsKey(key))
            {
                List<KeyValuePair<uint, uint>> lockedList = beLcokedDic[key];
                List<KeyValuePair<uint, uint>> targets = new List<KeyValuePair<uint, uint>>(lockedList);
                lockedList.Clear();
                FinishedMechanicsed(skillId, targets, isPlayer);//重置受击者状态
                for (int i = 0, count = targets.Count; i < count; i++)
                {
                    KeyValuePair<uint, uint> kvp = targets[i];
                    uint tid = kvp.Key;
                    CharacterEntity target = null;
                    if (isPlayer)
                        target = EnemyController.instance[tid];
                    else
                        target = PlayerController.instance[tid];
                    if (target && target.isDead)
                    {
                        VerifyDead(target, !isPlayer);
                    }
                }
            }
            lock (hitSkillQueue)
            {
                if (hitSkillQueue.Count > 0)
                {
                    KeyValuePair<uint, uint> hit = hitSkillQueue.Peek();
                    if (hit.Key == characterId && hit.Value == skillId)
                    {
                        hitSkillQueue.Dequeue();
                        hitSkillQueue.Enqueue(hit);
                        beLcokedDic.TryDelete(key);
                        skillWaitFinishDic.TryDelete(key);
                        skillingDic.TryDelete(key);
                    }
                }
            }
            //VirtualServerController.instance.PlaySkillCalc();
        }


        private void FinishedMechanicsed(uint skillId, List<KeyValuePair<uint, uint>> targets, bool isPlayer)
        {
            SkillData skillData = SkillData.GetSkillDataById(skillId);
            MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(skillData.timeline.First().Value.First());//第一个作用效果
            switch (mechanicsData.targetType)
            {
                case TargetType.Ally://己方暂不用更新状态，因为己方给己方使用技能，状态未改变
                    break;
                case TargetType.Enemy:
                    {
                        for (int i = 0, iCount = targets.Count; i < iCount; i++)
                        {
                            KeyValuePair<uint, uint> kvp = targets[i];
                            uint characterId = kvp.Key;

                            if (!VirtualServerControllerUtil.ExistInDic(characterId, beLcokedDic))
                            {
                                FinishedMechanicsed(characterId, !isPlayer);
                            }
                        }
                    }
                    break;
            }
        }

        public void FinishedMechanicsed(uint characterId, bool isPlayer)
        {
            DataMessageHandler.DataMessage_FinishedMechanicsed(characterId, isPlayer);
        }

        public void VerifyDead(CharacterEntity character, bool isPlayer)
        {
            if (character && character.isDead)
            {
                #region 超时强制从锁定列表移除
                if (_deadDic.ContainsKey(character.characterInfo.instanceID))
                {
                    float deadTime = _deadDic[character.characterInfo.instanceID];
                    if (Time.time - deadTime > FORCE_CLEAR_TIME)
                    {
                        VirtualServerControllerUtil.RemoveFromDic(character.characterInfo.instanceID, beLcokedDic);
                        _deadDic.Remove(character.characterInfo.instanceID);
                    }
                }
                else
                {
                    _deadDic.Add(character.characterInfo.instanceID, Time.time);
                }
                #endregion
                ClearSkiller(character.characterInfo.instanceID, isPlayer);
                if (!VirtualServerControllerUtil.ExistInDic(character.characterInfo.instanceID, beLcokedDic))
                {
                    DataMessageHandler.DataMessage_Dead(character.characterInfo.instanceID, isPlayer);
                }
            }
        }

        //清除角色技能释放信息和被锁定信息
        private void ClearSkiller(uint characterId, bool isPlayer)
        {
            List<uint> skillIds = new List<uint>();
            KeyValuePair<uint, uint> kvp = default(KeyValuePair<uint, uint>);
            List<string> list = skillWaitingDic.GetKeys();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                kvp = list[i].SplitToKeyValuePair<uint, uint>();
                if (kvp.Key == characterId)
                    skillIds.Add(kvp.Value);
            }
            for (int i = 0, count = skillIds.Count; i < count; i++)//有时角色会预约两个技能
            {
                string key = StringUtil.ConcatNumber(characterId, skillIds[i]);
                skillWaitingDic.TryDelete(key);
            }
            list.Clear();
            skillIds.Clear();

            list = bootSkillWaitingDic.GetKeys();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                kvp = list[i].SplitToKeyValuePair<uint, uint>();
                if (kvp.Key == characterId)
                    skillIds.Add(kvp.Value);
            }
            for (int i = 0, count = skillIds.Count; i < count; i++)//有时角色会预约两个技能
            {
                string key = StringUtil.ConcatNumber(characterId, skillIds[i]);
                bootSkillWaitingDic.TryDelete(key);
            }
            list.Clear();
            skillIds.Clear();
        }

        public void RemoveCharacterFromHitSkillQueue(uint characterId)
        {
            lock (hitSkillQueue)
            {
                Queue<KeyValuePair<uint, uint>> tempQueue = new Queue<KeyValuePair<uint, uint>>();
                while (hitSkillQueue.Count > 0)
                {
                    KeyValuePair<uint, uint> hit = hitSkillQueue.Dequeue();
                    if (hit.Key != characterId)
                        tempQueue.Enqueue(hit);
                }
                while (tempQueue.Count > 0)
                {
                    hitSkillQueue.Enqueue(tempQueue.Dequeue());
                }
                tempQueue = null;
            }
        }

        public void ForceFightFinished(bool result, FightOverType fightOverType)
        {
            victory = result;
            enemyReady = false;
            ClearData();
            ResetSkillOrder();
            TickCD(false);
            if (victory)
                PlayerVictory(fightOverType);
            else
                PlayerFail(fightOverType);
        }

        private void ClearData()
        {
            bootSkillWaitingDic.Clear();
            skillWaitingDic.Clear();
            skillWaitingQueue.Clear();
            skillWaitFinishDic.Clear();
            hitSkillQueue.Clear();
            skillingDic.Clear();
            beLcokedDic.Clear();
            _deadDic.Clear();
            aeonSkillingDic.Clear();
            aeonSkillDic.Clear();
        }
        #endregion

        #region private function
        private void InitPlayer()
        {
            _isPlayerInPlace = false;
            DataMessageHandler.DataMessage_InitPlayers();
        }

        private void InitEnemy()
        {
            _isEnemyInPlace = false;
            DataMessageHandler.DataMessage_InitEnemys();
        }

        private void StartFight()
        {
            _interlude = false;
            _hangup = false;
            List<HeroEntity> heroList = PlayerController.instance.heros;
            List<EnemyEntity> enemyList = EnemyController.instance.enemies;
            switch (Fight.Controller.FightController.instance.fightType)
            {
                case FightType.PVE:
                case FightType.Arena:
                case FightType.DailyPVE:
                case FightType.Expedition:
                case FightType.WorldTree:
                case FightType.WorldBoss:
                case FightType.PVP:
                case FightType.FriendFight:
                case FightType.MineFight:
#if UNITY_EDITOR
                case FightType.Imitate:
#endif
                    SortHitSkill(heroList, enemyList);
                    _enemyAutoOrder = true;
                    DataMessageHandler.DataMessage_TickCD(true);
                    break;
                case FightType.ConsortiaFight:
#if UNITY_EDITOR
                    orderSkills.Clear();
                    finishSkills.Clear();
#endif
                    _enemyAutoOrder = false;
                    DataMessageHandler.DataMessage_TickCD(true);
                    //StopCoroutine("PVPResultCoroutine");
                    OrderConsortiaSkill();
                    break;
                case FightType.FirstFight:
                case FightType.SkillDisplay:
                    _enemyAutoOrder = false;
                    break;
            }
            DataMessageHandler.DataMessage_StartFight();
            HeroEntity hero = null;
            for (int i = 0, count = heroList.Count; i < count; i++)
            {
                hero = heroList[i];
                if (Fight.Model.FightProxy.instance.CurrentTeamIndex == 0)
                {
                    if (Fight.Controller.FightController.instance.fightType == FightType.FirstFight)
                    {
                        FirstFightCharacterData ffcd = MockFightController.instance.GetCharacterDataById((int)hero.characterInfo.instanceID);
                        hero.skill1CD = ffcd.cd1;
                        hero.skill2CD = ffcd.cd2;
                    }
                    else
                    {
                        hero.skill1CD = 0;
                        hero.skill2CD = 0;
                    }
                }
                hero.SetOrderable();
                //Debugger.Log("hero {0},hp {1} ", hero.characterInfo.instanceID, hero.HP);
            }

            EnemyEntity enemy = null;
            for (int i = 0, count = enemyList.Count; i < count; i++)
            {
                enemy = enemyList[i];
                enemy.skill1CD = 0;
                enemy.skill2CD = 0;
                switch (Fight.Controller.FightController.instance.fightType)
                {
                    case FightType.PVE:
                    case FightType.DailyPVE:
                    case FightType.WorldTree:
                        try
                        {
                            if (Fight.Model.FightProxy.instance.CurrentTeamIndex > 0)
                            {
                                Logic.Team.Model.TeamData teamData = Logic.Fight.Model.FightProxy.instance.GetCurrentTeamData();
                                if (enemy.characterInfo.skillId1 > 0)
                                {
                                    enemy.skill1CD = teamData.cdReduceRate * enemy.characterInfo.skillInfo1.skillData.CD;
                                }
                                else
                                    enemy.skill1CD = 0;
                                if (enemy.characterInfo.skillId2 > 0)
                                {
                                    enemy.skill2CD = teamData.cdReduceRate * enemy.characterInfo.skillInfo2.skillData.CD;
                                }
                                else
                                    enemy.skill2CD = 0;
                            }
                            else
                            {
                                enemy.skill1CD = 0;
                                enemy.skill2CD = 0;
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debugger.Log("set cd fail:" + e.StackTrace);
                            enemy.skill1CD = 0;
                            enemy.skill2CD = 0;
                        }
                        break;
                    case FightType.FirstFight:
                        FirstFightCharacterData ffcd = MockFightController.instance.GetCharacterDataById((int)enemy.characterInfo.instanceID);
                        enemy.skill1CD = ffcd.cd1;
                        enemy.skill2CD = ffcd.cd2;
                        break;
                }
                //if (Fight.Controller.FightController.instance.fightType == FightType.FirstFight)
                //{
                //    FirstFightCharacterData ffcd = MockFightController.instance.GetCharacterDataById((int)enemy.characterInfo.instanceID);
                //    enemy.skill1CD = ffcd.cd1;
                //    enemy.skill2CD = ffcd.cd2;
                //}
                //else
                //{
                //    enemy.skill1CD = 0;
                //    enemy.skill2CD = 0;
                //}
                enemy.SetOrderable();
                //Debugger.Log("enemy {0},hp {1} ", enemy.characterInfo.instanceID, enemy.HP);
            }
        }

        private void SortHitSkill(List<HeroEntity> heros, List<EnemyEntity> enemies)
        {
            List<CharacterEntity> list = new List<CharacterEntity>();
            Dictionary<long, List<CharacterEntity>> result = new Dictionary<long, List<CharacterEntity>>();
            List<int> speeds = new List<int>();
            for (int i = 0, count = heros.Count; i < count; i++)
            {
                HeroEntity hero = heros[i];
                if (hero.characterInfo.hitId == 0)
                    continue;
                list.Add(hero);
                speeds.Add(hero.speed);
            }
            for (int i = 0, count = enemies.Count; i < count; i++)
            {
                EnemyEntity enmey = enemies[i];
                if (enmey.characterInfo.hitId == 0)
                    continue;
                list.Add(enmey);
                speeds.Add(enmey.speed);
            }
            long leastCommonMutiple = Common.Util.MathUtil.LeastCommonMultiple(speeds);
            int min = speeds.MinValue();
            if (min > 0)
            {
                List<long> nums = CalcNumbers(leastCommonMutiple, speeds);
                for (int i = 0, iCount = nums.Count; i < iCount; i++)
                {
                    long num = nums[i];
                    for (int j = 0, count = list.Count; j < count; j++)
                    {
                        CharacterEntity c = list[j];
                        long interval = leastCommonMutiple / c.speed;
                        if (num % interval == 0)
                        {
                            if (!result.ContainsKey(num))
                                result.Add(num, new List<CharacterEntity>());
                            result[num].Add(c);
                        }
                    }
                }
                lock (hitSkillQueue)
                {
                    hitSkillQueue.Clear();
                    Dictionary<long, List<CharacterEntity>>.Enumerator e = result.GetEnumerator();
                    while (e.MoveNext())
                    {
                        KeyValuePair<long, List<CharacterEntity>> kvp = e.Current;
                        List<CharacterEntity> cs = SortedCross(kvp.Value);
                        for (int i = 0, count = cs.Count; i < count; i++)
                        {
                            CharacterEntity c = cs[i];
                            KeyValuePair<uint, uint> hit = new KeyValuePair<uint, uint>(c.characterInfo.instanceID, c.characterInfo.hitId);
                            hitSkillQueue.Enqueue(hit);
                        }
                    }
                    e.Dispose();
                }
            }
            result.Clear();
            result = null;
            list.Clear();
            list = null;
        }

        private List<long> CalcNumbers(long leastCommonMultiple, List<int> speeds)
        {
            List<long> result = new List<long>();
            for (int i = 0, count = speeds.Count; i < count; i++)
            {
                int num = speeds[i];
                long interval = leastCommonMultiple / num;
                if (!result.Contains(interval))
                    result.Add(interval);
                for (int j = 2; j <= num; j++)
                {
                    long r = interval * j;
                    if (!result.Contains(r))
                        result.Add(r);
                }
            }
            result.Sort(SortType.Asc);
            return result;
        }

        private List<CharacterEntity> SortedCross(List<CharacterEntity> list)
        {
            bool player = RandomUtil.GetRandom(0.5f);
            List<CharacterEntity> result = new List<CharacterEntity>();
            Queue<CharacterEntity> heroQueue = new Queue<CharacterEntity>();
            Queue<CharacterEntity> enemyQueue = new Queue<CharacterEntity>();

            for (int i = 0, count = list.Count; i < count; i++)
            {
                CharacterEntity c = list[i];
                if (c.isPlayer)
                    heroQueue.Enqueue(c);
                else
                    enemyQueue.Enqueue(c);
            }
            while (heroQueue.Count > 0 || enemyQueue.Count > 0)
            {
                if (player)
                {
                    if (heroQueue.Count > 0)
                        result.Add(heroQueue.Dequeue());
                }
                else
                {
                    if (enemyQueue.Count > 0)
                        result.Add(enemyQueue.Dequeue());
                }
                player = !player;
            }
            return result;
        }

        public bool CanCancelSkillOrder(CharacterEntity character, uint skillId)
        {
            string key = StringUtil.ConcatNumber(character.characterInfo.instanceID, skillId);
            if (skillingDic.ContainsKey(key))
                return false;
            return true;
        }

        public void ResetSkillOrder(CharacterEntity character, uint skillId, bool isPlayer)
        {
            string key = StringUtil.ConcatNumber(character.characterInfo.instanceID, skillId);
            skillWaitingDic.TryDelete(key);
            character.ResetSkillOrder(skillId);
            if (isPlayer)
                DataMessageHandler.DataMessage_ResetSkillOrder(character.characterInfo.instanceID, skillId);
        }

        private void ResetSkillOrder()
        {
            DataMessageHandler.DataMessage_ResetSkillOrder();
        }

        private void RunWithoutMove()
        {
            DataMessageHandler.DataMessage_RunWithoutMove();
        }

        private void NextLevel()
        {
            RunWithoutMove();
        }

        private IEnumerator FinishLevelCoroutine()
        {
            while (true)
            {
                if (skillingDic.Count == 0 && beLcokedDic.Count == 0 && aeonSkillingDic.Count == 0)
                    break;
                yield return null;
            }

            DataMessageHandler.DataMessage_TickCD(false);
            ClearData();
            ResetSkillOrder();
            #region imitate
#if UNITY_EDITOR
            if (Fight.Controller.FightController.imitate)
            {
                if (++_imitateWave <= Fight.Controller.FightController.instance.enemyWave)
                {
                    //DataMessageHandler.DataMessage_PlayNextFightEffect();
                    //yield return new WaitForSeconds(2f);
                    //if (Fight.Model.FightProxy.instance.hasBoss)
                    //    DataMessageHandler.DataMessage_PlayBossAppearEffect();
                    NextLevel();
                }
                else
                {
                    //yield return new WaitForSeconds(2f);
                    PlayerVictory();
                }
            }
            else
            {
#endif
            #endregion
                if (Fight.Model.FightProxy.instance.HasNextEnemies())
                {
                    Fight.Model.FightProxy.instance.Next();
                    //yield return new WaitForSeconds(2f);
                    if (Fight.Model.FightProxy.instance.hasBoss)
                        DataMessageHandler.DataMessage_PlayBossAppearEffect();
                    NextLevel();
                }
                else
                {
                    //yield return new WaitForSeconds(2f);
                    PlayerVictory();
                }
#if UNITY_EDITOR
            }
#endif
        }

        private void TickCD(bool isTick)
        {
            float time = Time.time;
            DataMessageHandler.DataMessage_TickCD(isTick);
            if (!isTick)
                DataMessageHandler.DataMessage_LastTickTime(time);
        }
        #endregion
    }

    public enum CharacterType
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }
}