using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Effect.Model;
using Logic.Enums;
using Common.Components.Effect;
using Common.Components.Trans;
using Common.ResMgr;
using Common.Util;
using Logic.Game.Controller;
using Logic.Game;
using Logic.Cameras.Controller;
using Logic.Character;
using Logic.Fight.Controller;
using Common.Animators;
using Common.Effect;
using Common.GameTime.Controller;
using Logic.Hero.Model;
using Logic.Player.Model;
using Common.Components;

namespace Logic.Effect.Controller
{
    public class EffectController : SingletonMono<EffectController>
    {
        #region effect id
        //ui
        public const string UI_EFFECT_01 = "ui_effect_01";
        public const string UI_EFFECT_07 = "ui_effect_07";
        public const string UI_EFFECT_08 = "ui_effect_08";
        public const string UI_EFFECT_NEXT_FIGHT = "ui_effect_next_fight";
        public const string UI_EFFECT_WARNING_FIGHT = "ui_effect_warning_fight";
        public const string EFFECT_CLICKSKILL = "effect_click_skill";
        public const string UI_EFFECT_28 = "ui_effect_28";
        public const string UI_ICON_FLOAT = "ui_icon_shang";
        public const string UI_ICON_TUMBLE = "ui_icon_shang";
        //skill


        //public const uint SLEEP_EFFECT_ID = 690023; //易伤
        //public const uint SLEEP_EFFECT_ID = 690022; //诅咒
        //public const uint SLEEP_EFFECT_ID = 690003; //物理攻击增强

        //public const uint SLEEP_EFFECT_ID = 690038; //伤害减免
        //public const uint SLEEP_EFFECT_ID = 690039; //伤害加成
        public const uint PHYSICS_DEFENSE_ADD_EFFECT_ID = 690025; //物理防御增强
        public const uint PHYSICS_DEFENSE_REDUCE_EFFECT_ID = 690026; //物理防御降低
        public const uint MAGIC_DEFENSE_ADD_EFFECT_ID = 690027; //魔法防御增强
        public const uint MAGIC_DEFENSE_REDUCE_EFFECT_ID = 690028; //魔法防御降低
        public const uint PHYSICS_ATTACK_ADD_EFFECT_ID = 690003; //物理攻击增强
        public const uint PHYSICS_ATTACK_REDUCE_EFFECT_ID = 690030; //物理攻击降低
        public const uint MAGIC_ATTACK_REDUCE_EFFECT_ID = 690029; //魔法攻击降低
        public const uint DAMAGE_DESC_ADD_EFFECT_ID = 690040; //伤害减免（正值,攻击方）
        public const uint DAMAGE_DESC_DESC_EFFECT_ID = 690040; //伤害减免（负值,攻击方）
        public const uint DAMAGE_ADD_ADD_EFFECT_ID = 690039; //伤害加成（正,受击方）
        public const uint DAMAGE_ADD_DESC_EFFECT_ID = 690039; //伤害加成（负值,受击方）

        public const uint SILENCE_EFFECT_ID = 99942; //沉默
        public const uint FORCE_KILL_ID = 690016; //即死	
        public const uint INVINCIBLE_ID = 690024; //无敌
        public const uint SLEEP_EFFECT_ID = 690010; //睡觉
        public const uint SHIELD_EFFECT_ID = 690004;
        public const uint SWIMMY_EFFECT_ID = 99978;
        public const uint POISONING_EFFECT_ID = 690001;
        public const uint BLEED_EFFECT_ID = 690000;
        public const uint IGNITE_EFFECT_ID = 690008;
        public const uint GOLD_EFFECT_ID = 99976;
        public const uint DODGE_EFFECT_ID = 690012;
        public const uint BLOCK_EFFECT_ID = 690002;
        public const uint TOMBSTONE_EFFECT_ID = 96000;
        public const uint BLACK_SCREEN_EFFECT_ID = 98000;
        public const string SPEED_LINE = "suduxian";
        public const uint BOSS_EFFECT_ID = 690041;
        public const string RED_BACKGROUND = "hongsebeijing";
        #endregion

        private const string _COLOR = "_Color";
        private const string _HIT = "_Hit";
        //private Dictionary<uint, GameObject> _tipEffectDic = new Dictionary<uint, GameObject>();
        //private Dictionary<uint, bool> _isTipEffectDic = new Dictionary<uint, bool>();
        private Dictionary<string, GameObject> _effectDic = new Dictionary<string, GameObject>();//key:character's type+userId+effectName+Time.realtimeSinceStartup
        private Dictionary<string, GameObject> _ignoreDelEffectDic = new Dictionary<string, GameObject>();
        private List<GameObject> _comboMakeEffects = new List<GameObject>();
        private List<GameObject> _attackRootTips = new List<GameObject>();
        private List<GameObject> _targetRangeTips = new List<GameObject>();
        private Dictionary<uint, List<GameObject>> _characterTargetRangeTips = new Dictionary<uint, List<GameObject>>();
        private Transform _parentTrans;
        private Transform _UIParentTrans;
        private GameObject _Red_Background_Go;
        private GameObject _BgEffectGo;
        public bool isPlayEffect = true;
#if UNITY_EDITOR
        public Dictionary<string, GameObject> effectDic
        {
            get
            {
                return _effectDic;
            }
        }

        public Dictionary<string, GameObject> ignoreDelEffectDic
        {
            get
            {
                return _ignoreDelEffectDic;
            }
        }

        public List<GameObject> comboMakeEffects
        {
            get
            {
                return _comboMakeEffects;
            }
        }

        public List<GameObject> attackRootTips
        {
            get
            {
                return _attackRootTips;
            }
        }

        public List<GameObject> targetRangeTips
        {
            get
            {
                return _targetRangeTips;
            }
        }
#endif
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            _parentTrans = Map.Controller.MapController.instance.effectsParent;
            _UIParentTrans = UI.UIMgr.instance.transform;
        }

        #region switch effects layer
        public void SwitchEffectAfterWaitCombo()
        {
            if (!GameSetting.instance.effectable) return;
            ClearComboMakeEffects();
        }

        public void SwitchEffect(CharacterEntity character, int layer)
        {
            if (!character) return;
            if (!GameSetting.instance.effectable) return;
            string prefix = character.gameObject.name + character.characterInfo.instanceID;
            List<string> effectKeys = _effectDic.GetKeys();
            for (int i = 0, count = effectKeys.Count; i < count; i++)
            {
                string key = effectKeys[i];
                if (key.StartsWith(prefix))
                {
                    GameObject effect = _effectDic[key];
                    if (effect)
                        TransformUtil.SwitchLayer(effect.transform, layer);
                }
            }
            effectKeys.Clear();
            effectKeys = null;
        }

        public void SwitchEffectByPrefabName(string prefabName, int layer)
        {
            if (!GameSetting.instance.effectable) return;
            List<string> effectNames = _effectDic.GetKeys();
            for (int i = 0, count = effectNames.Count; i < count; i++)
            {
                string effectName = effectNames[i];
                if (effectName.StartsWith(prefabName))
                {
                    GameObject go = _effectDic[effectName];
                    TransformUtil.SwitchLayer(go.transform, layer);
                }
            }
            effectNames.Clear();
            effectNames = null;
        }
        #endregion

        #region remove effects
        private void ClearComboMakeEffects()
        {
            for (int i = 0, count = _comboMakeEffects.Count; i < count; i++)
            {
                GameObject go = _comboMakeEffects[i];
                if (go)
                    UnityEngine.Object.Destroy(go);
            }
            _comboMakeEffects.Clear();
        }

        public void RemoveEffects(CharacterEntity character, float delay)
        {
            StartCoroutine(RemoveEffectsCoroutine(character, delay));
        }

        private IEnumerator RemoveEffectsCoroutine(CharacterEntity character, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            string prefix = character.gameObject.name + character.characterInfo.instanceID;
            List<string> effectNames = _effectDic.GetKeys();
            for (int i = 0, count = effectNames.Count; i < count; i++)
            {
                string effectName = effectNames[i];
                if (effectName.StartsWith(prefix))
                {
                    RemoveEffectByName(effectName);
                }
            }
            effectNames.Clear();
            effectNames = null;
        }

        public bool RemoveEffects(CharacterEntity character, int layer)
        {
            string prefix = character.gameObject.name + character.characterInfo.instanceID;
            List<string> effectNames = _effectDic.GetKeys();
            for (int i = 0, count = effectNames.Count; i < count; i++)
            {
                string effectName = effectNames[i];
                if (effectName.StartsWith(prefix))
                {
                    RemoveEffectByName(effectName, layer);
                }
            }
            effectNames.Clear();
            effectNames = null;
            return true;
        }

        public bool RemoveEffectByPrefabName(string prefabName)
        {
            List<string> effectNames = _effectDic.GetKeys();
            for (int i = 0, count = effectNames.Count; i < count; i++)
            {
                string effectName = effectNames[i];
                if (effectName.StartsWith(prefabName))
                {
                    RemoveEffectByName(effectName);
                }
            }
            effectNames.Clear();
            effectNames = null;
            return true;
        }

        public bool RemoveEffectByName(string effectName)
        {
            GameObject target = null;
            if (_effectDic.TryGetValue(effectName, out target))
            {
                _effectDic.Remove(effectName);
                UnityEngine.Object.Destroy(target);
            }
            return true;
        }

        public bool RemoveEffectByName(string effectName, int layer)
        {
            GameObject target = null;
            if (_effectDic.TryGetValue(effectName, out target))
            {
                if (!target)
                    _effectDic.Remove(effectName);
                else if (target.layer == layer)
                {
                    _effectDic.Remove(effectName);
                    UnityEngine.Object.Destroy(target);
                }
            }
            return true;
        }

        public void ClearEffectInFight()
        {
            List<GameObject> list = _effectDic.GetValues();
            GameObject go = null;
            for (int i = 0, count = list.Count; i < count; i++)
            {
                go = list[i];
                if (go)
                    UnityEngine.Object.Destroy(go);
            }
            _effectDic.Clear();
            go = null;
            List<GameObject> ignoreList = _ignoreDelEffectDic.GetValues();
            for (int i = 0, count = ignoreList.Count; i < count; i++)
            {
                go = list[i];
                if (go)
                    UnityEngine.Object.Destroy(go);
            }
            _ignoreDelEffectDic.Clear();
            ClearComboMakeEffects();
            ClearAttackRootTips();
            ClearTargetRangeTips();
            TransformUtil.ClearChildren(_parentTrans, false);
        }

        private IEnumerator DestroyEffectCoroutine(float delay, GameObject effectGO)
        {
            yield return new WaitForSeconds(delay);
            if (effectGO)
            {
                _effectDic.TryDelete(effectGO.name);
                UnityEngine.Object.Destroy(effectGO);
            }
        }

        private void OnDestroyEffect(GameObject effectGO)
        {
            _effectDic.TryDelete(effectGO.name);
        }
        #endregion

        #region change color

        public void SetColor(CharacterEntity character, Color color)
        {
            if (!GameSetting.instance.effectable) return;
            Renderer[] tempRenderers = character.GetComponentsInChildren<Renderer>();
            List<Renderer> renderers = new List<Renderer>();
            for (int i = 0, count = tempRenderers.Length; i < count; i++)
            {
                Renderer r = tempRenderers[i];
                if (r is MeshRenderer || r is SkinnedMeshRenderer)
                    renderers.Add(r);
            }

            for (int i = 0, count = renderers.Count; i < count; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer) continue;
                //#if UNITY_EDITOR
                Material[] materials = renderer.materials;
                //#else
                //                    Material[] materials = renderer.sharedMaterials;
                //#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    if (materials[j].HasProperty(_COLOR))
                    {
                        materials[j].SetColor(_COLOR, color);
                    }
                }
            }
        }

        private void SetColor(CharacterEntity character, Color color, float duration)
        {
            StartCoroutine(SetMaterialColorCoroutine(duration, color));
        }

        private IEnumerator SetMaterialColorCoroutine(float duration, Color color)
        {
            List<Color> originalColor = new List<Color>();
            Renderer[] tempRenderers = GetComponentsInChildren<Renderer>();
            List<Renderer> renderers = new List<Renderer>();
            for (int i = 0, count = tempRenderers.Length; i < count; i++)
            {
                Renderer r = tempRenderers[i];
                if (r is MeshRenderer || r is SkinnedMeshRenderer)
                    renderers.Add(r);
            }
            for (int i = 0, count = renderers.Count; i < count; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer) continue;
                //#if UNITY_EDITOR
                Material[] materials = renderer.materials;
                //#else
                //                    Material[] materials = renderer.sharedMaterials;
                //#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    if (materials[j].HasProperty(_COLOR))
                        originalColor.Add(materials[j].GetColor(_COLOR));
                }
            }

            for (int i = 0, count = renderers.Count; i < count; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer) continue;
                //#if UNITY_EDITOR
                Material[] materials = renderer.materials;
                //#else
                //                    Material[] materials = renderer.sharedMaterials;
                //#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    if (materials[j].HasProperty(_COLOR))
                    {
                        materials[j].SetColor(_COLOR, color);
                        materials[j].SetFloat(_HIT, 1);
                    }
                }
            }
            duration /= Logic.Game.GameSetting.instance.speed;
            float time = Time.realtimeSinceStartup;
            float currentTime = time;
            while (Time.realtimeSinceStartup - time <= duration)
            {
                yield return null;
                if (TimeController.instance.playerPause)
                    time += (Time.realtimeSinceStartup - currentTime);
                currentTime = Time.realtimeSinceStartup;
            }
            int index = 0;
            for (int i = 0, count = renderers.Count; i < count; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer) continue;
                //#if UNITY_EDITOR
                Material[] materials = renderer.materials;
                //#else
                //                    Material[] materials = renderer.sharedMaterials;
                //#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    if (materials[j].HasProperty(_COLOR))
                    {
                        materials[j].SetColor(_COLOR, originalColor[index]);
                        materials[j].SetFloat(_HIT, 0);
                        index++;
                    }
                }
            }
            renderers.Clear();
            renderers = null;
            originalColor.Clear();
            originalColor = null;
            tempRenderers = null;
        }
        #endregion

        #region play effects
        private void ShowRedBackgroundEffect(float duration)
        {
            return;
            //改为读配置
            if (!_Red_Background_Go)
            {
                Debug.Log("effects/prefabs/" + RED_BACKGROUND);
                GameObject particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + RED_BACKGROUND);
                if (!particle) return;
                _Red_Background_Go = GameObject.Instantiate(particle, new Vector3(0, 0, -18f), Quaternion.identity) as GameObject;
                _Red_Background_Go.transform.SetParent(_parentTrans);
                TransformUtil.SwitchLayer(_Red_Background_Go.transform, (int)LayerType.Scene);
            }
            StartCoroutine(ShowRedBackgroundEffectCoroutine(duration));
        }
      
        private IEnumerator ShowRedBackgroundEffectCoroutine(float duration)
        {
            if (!_Red_Background_Go) yield break;
            _Red_Background_Go.SetActive(true);
            yield return new WaitForSeconds(duration);
            if (_Red_Background_Go)
                _Red_Background_Go.SetActive(false);
        }

        //private void ShowBgEffect(float duration, string effectName)
        //{
        //    if (!_BgEffectGo)
        //    {
        //        Debug.Log("effects/prefabs/" + effectName);
        //        GameObject particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectName);
        //        if (!particle) return;
        //        _BgEffectGo = GameObject.Instantiate(particle, new Vector3(0, 0, -18f), Quaternion.identity) as GameObject;
        //        _BgEffectGo.transform.SetParent(_parentTrans);
        //        TransformUtil.SwitchLayer(_BgEffectGo.transform, (int)LayerType.Scene);
        //    }
        //    StartCoroutine(ShowBgEffectCoroutine(duration));
        //}

        //private IEnumerator ShowBgEffectCoroutine(float duration)
        //{
        //    if (!_BgEffectGo) yield break;
        //    _BgEffectGo.SetActive(true);
        //    yield return new WaitForSeconds(duration);
        //    if (_BgEffectGo)
        //        _BgEffectGo.SetActive(false);
        //}
        public void PlayInvincibleEffect(CharacterEntity target)
        {

        }

        public void PlayForceKillEffect(CharacterEntity target)
        {
            if (!GameSetting.instance.effectable) return;
            EffectInfo effectInfo = new EffectInfo(FORCE_KILL_ID);
            if (effectInfo.effectData == null) return;
            //Debugger.Log(effectInfo.effectData.effectType);
            effectInfo.character = target;
            switch (effectInfo.effectData.effectType)
            {
                //case EffectType.Root:
                //    effectInfo.pos = Logic.Position.Model.PositionData.GetPostionDataById(target.positionId).position + effectInfo.effectData.offset;
                //    effectInfo.target = target;
                //    break;
                //case EffectType.LockTarget:
                //    effectInfo.target = target;
                //    break;
                //case EffectType.ChangeColor:
                //    effectInfo.target = target;
                //    break;
                case EffectType.LockPart:
                    effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, target.transform);
                    effectInfo.target = target;
                    break;
            }
            effectInfo.delay = effectInfo.effectData.delay;
            EffectController.instance.PlayEffect(effectInfo);
        }

        public string PlayContinuousEffect(EffectInfo effectInfo, int layer = (int)LayerType.Fight)
        {
            string effectName = GetEffectNameContactTime(effectInfo.target, effectInfo.effectData);
            if (effectInfo.delay > 0)
                StartCoroutine(PlayContinuousEffectCoroutine(effectInfo, effectName, layer));
            else
                PlayContinuousEffectImmediate(effectInfo, effectName, layer);
            return effectName;
        }

        private IEnumerator PlayContinuousEffectCoroutine(EffectInfo effectInfo, string effectName, int layer)
        {
            yield return new WaitForSeconds(effectInfo.delay);
            PlayContinuousEffectImmediate(effectInfo, effectName, layer);
        }

        private GameObject PlayContinuousEffectImmediate(EffectInfo effectInfo, string effectName, int layer)
        {
            if (effectInfo.effectData.effectType == EffectType.ChangeColor || effectInfo.effectData.effectType == EffectType.ShakeScreen)
            {
                PlayOtherEffect(effectInfo);
                return null;
            }
            GameObject particle = null;// ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectInfo.effectData.effectName);
            bool mirror = true;
            if (!effectInfo.target.isPlayer)
                particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectInfo.effectData.effectName + "_enemy");
            if (!particle)
                particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectInfo.effectData.effectName);
            else
                mirror = false;
            GameObject go = null;
            if (particle)
            {
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        if (isPlayEffect)
                        {
                            Quaternion qua = Quaternion.identity;
                            if (effectInfo.effectData.randomAngles.Count > 0)
                            {
                                int index = Random.Range(0, effectInfo.effectData.randomAngles.Count);
                                qua.eulerAngles = effectInfo.effectData.randomAngles[index];
                            }
                            go = GameObject.Instantiate(particle, effectInfo.pos, qua) as GameObject;
                            go.transform.SetParent(_parentTrans, false);
                        }
                        break;
                    case EffectType.LockTarget:
                        {
                            Quaternion qua = Quaternion.identity;
                            go = GameObject.Instantiate(particle, effectInfo.pos, qua) as GameObject;
                            go.name = effectName;
                            go.transform.SetParent(_parentTrans, false);
                            if (effectInfo.effectData.randomAngles.Count > 0)
                            {
                                int index = Random.Range(0, effectInfo.effectData.randomAngles.Count);
                                qua.eulerAngles = effectInfo.effectData.randomAngles[index];
                            }
                            else
                            {
                                #region rotate
                                if (effectInfo.target.isPlayer)
                                {
                                    if (effectInfo.effectData.isRotate)
                                    {
                                        qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                    }
                                }
                                else
                                {
                                    if (mirror)
                                    {
                                        List<ParticleMirror> mirros = GameObjectUtil.AddComponent2GameObject<ParticleMirror, Renderer>(go);
                                        for (int i = 0, length = mirros.Count; i < length; i++)
                                        {
                                            mirros[i].mirror = -1;
                                        }
                                        go.transform.localScale = new Vector3(-1, 1, 1);

                                        if (effectInfo.effectData.isRotate)
                                            qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                    }
                                    else
                                    {
                                        if (effectInfo.effectData.isRotate)
                                        {
                                            qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                        }
                                    }
                                }
                                #endregion
                            }
                            go.transform.localRotation = qua;
                            LockTransform lt = go.AddComponent<LockTransform>();
                            Vector3 offset = effectInfo.effectData.offset;
                            if (!effectInfo.target.isPlayer)
                                offset.x *= -1;
                            lt.offset = offset;
                            lt.trans = effectInfo.target.rootNode.transform;
                            lt.ignoreYaxis = true;
                        }
                        break;
                    case EffectType.LockPart:
                        {
                            go = GameObject.Instantiate(particle) as GameObject;
                            go.name = effectName;
                            Vector3 offset = effectInfo.effectData.offset;

                            if (!effectInfo.effectData.isRotate)
                            {
                                if (!effectInfo.target.isPlayer)
                                    offset.x *= -1;
                                go.transform.SetParent(_parentTrans, false);
                                LockTransform lt = go.AddComponent<LockTransform>();
                                lt.offset = offset;
                                lt.trans = effectInfo.lockTrans;
                                lt.ignoreYaxis = false;
                            }
                            else
                            {
                                go.transform.SetParent(effectInfo.lockTrans, false);
                                go.transform.localPosition = offset;
                                if (!effectInfo.target.isPlayer)
                                {
                                    go.transform.localScale = effectInfo.effectData.scale;
                                    go.transform.localEulerAngles = go.transform.localEulerAngles + effectInfo.effectData.rotate;
                                }
                            }
                        }
                        break;
                }
                if (go)
                {
                    TransformUtil.SwitchLayer(go.transform, layer);
                    _effectDic[effectName] = go;
                    switch (FightController.instance.fightStatus)//为了方便策划连击时，通过buff系统来实现播放攻击特效（比如瞄准）
                    {
                        case FightStatus.FloatComboing:
                        case FightStatus.TumbleComboing:
                            TransformUtil.SwitchLayer(go.transform, (int)LayerType.FightCombo);
                            if (effectInfo.effectData.ignoreTimeScale)
                            {
                                ParticlePlayer pp = go.AddComponent<ParticlePlayer>();
                                pp.duration = effectInfo.effectData.length / GameSetting.instance.speed;
                                pp.speed = GameSetting.instance.speed;
                                pp.character = effectInfo.target;
                            }
                            break;
                    }
                }
            }
            return go;
        }

        public void PlayUIEffect(string effectName, Vector3 pos, Quaternion rotation, Vector3 scale, float time, int sortingOrder, Transform parent = null, float delay = 0f, System.Action callback = null, int layer = (int)LayerType.UI)
        {
            if (delay > 0)
                StartCoroutine(PlayUIEffectCoroutine(effectName, pos, rotation, scale, time, sortingOrder, parent, delay, callback, layer));
            else
                PlayUIEffect(effectName, pos, rotation, scale, time, sortingOrder, parent, callback, layer);
        }

        private IEnumerator PlayUIEffectCoroutine(string effectName, Vector3 pos, Quaternion rotation, Vector3 scale, float time, int sortingOrder, Transform parent, float delay, System.Action callback, int layer)
        {
            yield return new WaitForSeconds(delay);
            PlayUIEffect(effectName, pos, rotation, scale, time, sortingOrder, parent, callback, layer);
        }

        private void PlayUIEffect(string effectName, Vector3 pos, Quaternion rotation, Vector3 scale, float time, int sortingOrder, Transform parent, System.Action callback, int layer)
        {
            GameObject particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectName);
            if (particle)
            {
                GameObject go = GameObject.Instantiate(particle, pos, rotation) as GameObject;
                if (parent)
                    go.transform.SetParent(parent, false);
                else
                    go.transform.SetParent(_UIParentTrans, false);
                TransformUtil.SwitchLayer(go.transform, layer);
                ParticlePlayer pp = go.AddComponent<ParticlePlayer>();
                go.AddComponent<Common.Components.SortingOrderChanger>().sortingOrder = sortingOrder + 1;
                pp.duration = time / GameSetting.instance.speed;
                pp.isUI = true;
                pp.onDestroyEvent += OnDestroyEffect;
                _effectDic[effectName] = go;
            }
        }

        public void PlayNoSkillEffect(EffectInfo effectInfo, bool ignoreDel = false, int layer = (int)LayerType.Fight)
        {
            if (effectInfo.delay > 0)
                StartCoroutine(PlayNoSkillEffectCoroutine(effectInfo, ignoreDel, layer));
            else
            {
                GameObject go = PlayEffectImmediate(effectInfo, layer);
                if (go)
                {
                    if (ignoreDel)
                        _ignoreDelEffectDic[go.name] = go;
                    else
                        _effectDic[go.name] = go;
                }
            }
        }

        private IEnumerator PlayNoSkillEffectCoroutine(EffectInfo effectInfo, bool ignoreDel, int layer)
        {
            yield return new WaitForSeconds(effectInfo.delay);
            GameObject go = PlayEffectImmediate(effectInfo, layer);
            if (go)
            {
                if (ignoreDel)
                    _ignoreDelEffectDic[go.name] = go;
                else
                    _effectDic[go.name] = go;
            }
        }

        public void PlayBornEffect(CharacterEntity character, HeroData heroData, int advanceLevel, bool isUI, int layer = (int)LayerType.Fight)
        {
            if (!GameSetting.instance.effectable) return;
            if (heroData.bornEffectIds.Count == 0) return;
            uint[] effects = heroData.bornEffectIds[advanceLevel - 1];
            for (int i = 0, length = effects.Length; i < length; i++)
            {
                EffectInfo effectInfo = new EffectInfo(effects[i]);
                if (effectInfo.effectData == null) continue;
                effectInfo.character = character;
                //Vector3 scale = Vector3.one;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.LockPart:
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, character.transform);
                        if (effectInfo.lockTrans == null) continue;
                        TransformUtil.ClearChildren(effectInfo.lockTrans, effectInfo.effectData.effectName);
                        effectInfo.target = character;
                        //scale = effectInfo.lockTrans.transform.lossyScale;
                        //Debugger.Log("scale" + scale);
                        break;
                }
                effectInfo.delay = effectInfo.effectData.delay;
                StartCoroutine(PlayBornEffect(effectInfo, isUI, layer));
            }
        }

        private IEnumerator PlayBornEffect(EffectInfo effectInfo, bool isUI, int layer)
        {
            GameObject go = PlayEffectImmediate(effectInfo, layer);
            if (go)
            {
                _effectDic[go.name] = go;
                yield return null;
                if (!go)
                    yield break;
                List<ParticleScaler> scales = GameObjectUtil.AddComponent2GameObject<ParticleScaler, Renderer>(go);
                for (int i = 0, length = scales.Count; i < length; i++)
                {
                    scales[i].isUI = isUI;
                }
                if (isUI)
                {
                    Canvas canvas = go.GetComponentInParent<Canvas>();
                    if (canvas)
                    {
                        SortingOrderChanger sortingOrderChanger = go.AddComponent<SortingOrderChanger>();
                        sortingOrderChanger.UISortingLayer = UI.EUISortingLayer.MainUI;
                        sortingOrderChanger.isPlus = true;
                        sortingOrderChanger.sortingOrder = canvas.sortingOrder;
                    }
                }
            }
        }


        public void PlayEffect(EffectInfo effectInfo, int layer = (int)LayerType.Fight)
        {
            if (effectInfo.delay > 0)
                StartCoroutine(PlayEffectCoroutine(effectInfo, layer));
            else
            {
                GameObject go = PlayEffectImmediate(effectInfo, layer);
                if (go)
                    _effectDic[go.name] = go;
            }
        }

        private IEnumerator PlayEffectCoroutine(EffectInfo effectInfo, int layer)
        {
            float puaseTime = 0f;
            if (effectInfo.skillInfo != null)
                puaseTime = effectInfo.skillInfo.skillData.pauseTime;
            if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal && puaseTime <= 0)
                yield return new WaitForSeconds(effectInfo.delay);
            else
            {
                float time = Time.realtimeSinceStartup;
                float delay = effectInfo.delay;
                delay /= GameSetting.instance.speed;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (!TimeController.instance.IgnorePause(effectInfo.character))
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            if (effectInfo.effectData != null)
            {
                GameObject go = PlayEffectImmediate(effectInfo, layer);
                if (go)
                    _effectDic[go.name] = go;
            }
        }

        private void PlayOtherEffect(EffectInfo effectInfo)
        {
            //float time = Time.realtimeSinceStartup;
            //float delay = effectInfo.effectData.delay;
            //while (Time.realtimeSinceStartup - time < delay)
            //    yield return null;
            switch (effectInfo.effectData.effectType)
            {
                case EffectType.ChangeColor:
                    SetColor(effectInfo.target, effectInfo.effectData.color, effectInfo.effectData.length);
                    break;
                case EffectType.ShakeScreen:
                    CameraController.instance.Shake();
                    break;
                //case EffectType.BlackScreen:
                //    float blackTime = effectInfo.length;
                //    switch (Fight.Controller.FightController.instance.fightStatus)
                //    {
                //        case FightStatus.Normal:
                //        case FightStatus.FloatWaiting:
                //        case FightStatus.TumbleWaiting:
                //            if (effectInfo.skillInfo != null)
                //                blackTime -= effectInfo.skillInfo.skillData.pauseTime;
                //            if (blackTime < 0)
                //                blackTime = GameSetting.instance.deltaTimeFight;
                //            Map.Controller.MapController.instance.MapTrigger(blackTime, effectInfo.effectData.color.a);
                //            break;
                //        case FightStatus.FloatComboing:
                //        case FightStatus.TumbleComboing:
                //            //Map.Controller.MapController.instance.MapTrigger(blackTime, effectInfo.effectData.color.a);
                //            break;
                //    }
                //break;
            }
        }

        private GameObject PlayEffectImmediate(EffectInfo effectInfo, int layer)
        {
            if (effectInfo.effectData.effectType == EffectType.ChangeColor || effectInfo.effectData.effectType == EffectType.ShakeScreen/* || effectInfo.effectData.effectType == EffectType.BlackScreen*/)
            {
                PlayOtherEffect(effectInfo);
                return null;
            }
            else
            {
                GameObject particle = null;
                bool mirror = true;
                if (!effectInfo.target.isPlayer)
                    particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectInfo.effectData.effectName + "_enemy");
                if (!particle)
                    particle = ResMgr.instance.Load<GameObject>("effects/prefabs/" + effectInfo.effectData.effectName);
                else
                    mirror = false;
                GameObject go = null;
                if (particle)
                {
                    switch (effectInfo.effectData.effectType)
                    {
                        case EffectType.Root:
                            if (isPlayEffect)
                            {
                                Quaternion qua = Quaternion.identity;
                                if (effectInfo.effectData.randomAngles.Count > 0)
                                {
                                    int index = Random.Range(0, effectInfo.effectData.randomAngles.Count);
                                    qua.eulerAngles = effectInfo.effectData.randomAngles[index];
                                }
                                else
                                    qua.eulerAngles = effectInfo.rotateAngles;
                                go = GameObject.Instantiate(particle, effectInfo.pos, qua) as GameObject;
                                go.transform.SetParent(_parentTrans, false);
                            }
                            break;
                        case EffectType.Trace:
                        case EffectType.CurveTrace:
                            if (isPlayEffect)
                            {
                                //释放技能角色转向目标，z轴作相应的偏移
                                float offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                if (!effectInfo.target.isPlayer)
                                    offsetZ *= -1;
                                effectInfo.pos += new Vector3(0f, 0f, offsetZ);
                                go = GameObject.Instantiate(particle, effectInfo.pos, Quaternion.identity) as GameObject;
                                go.transform.SetParent(_parentTrans, false);
                            }
                            break;
                        case EffectType.TargetArea:
                            if (isPlayEffect)
                            {
                                go = GameObject.Instantiate(particle, effectInfo.pos, Quaternion.identity) as GameObject;
                                go.transform.SetParent(_parentTrans, false);
                            }
                            break;
                        case EffectType.LockPart:
                            if (isPlayEffect)
                            {
                                go = GameObject.Instantiate(particle) as GameObject;
                                Vector3 offset = effectInfo.effectData.offset;
                                if (!effectInfo.effectData.isRotate)
                                {
                                    if (!effectInfo.target.isPlayer)
                                        offset.x *= -1;
                                    go.transform.SetParent(_parentTrans, false);
                                    LockTransform lt = go.AddComponent<LockTransform>();
                                    lt.offset = offset;
                                    lt.trans = effectInfo.lockTrans;
                                    lt.ignoreYaxis = false;
                                }
                                else
                                {
                                    go.transform.SetParent(effectInfo.lockTrans, false);
                                    go.transform.localPosition = offset;
                                }

                                if (effectInfo.effectData.randomAngles.Count > 0)
                                {
                                    Quaternion qua = Quaternion.identity;
                                    int index = Random.Range(0, effectInfo.effectData.randomAngles.Count);
                                    qua.eulerAngles = effectInfo.effectData.randomAngles[index];
                                    go.transform.localRotation = qua;
                                }
                                if (!effectInfo.target.isPlayer && effectInfo.effectData.isRotate)
                                {
                                    go.transform.localScale = effectInfo.effectData.scale;
                                    go.transform.localEulerAngles = go.transform.localEulerAngles + effectInfo.effectData.rotate;
                                }
                            }
                            break;
                        case EffectType.LockTarget:
                            if (isPlayEffect)
                            {
                                Quaternion qua = Quaternion.identity;
                                go = GameObject.Instantiate(particle, effectInfo.target.pos, qua) as GameObject;
                                go.transform.SetParent(_parentTrans, false);
                                float offsetZ = 0f;
                                if (effectInfo.effectData.randomAngles.Count > 0)
                                {
                                    int index = Random.Range(0, effectInfo.effectData.randomAngles.Count);
                                    qua.eulerAngles = effectInfo.effectData.randomAngles[index];
                                }
                                else
                                {
                                    #region rotate
                                    if (effectInfo.target.isPlayer)
                                    {
                                        if (effectInfo.effectData.isRotate)
                                        {
                                            offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                            qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                        }
                                    }
                                    else
                                    {
                                        if (mirror)
                                        {
                                            List<ParticleMirror> mirros = GameObjectUtil.AddComponent2GameObject<ParticleMirror, Renderer>(go);
                                            for (int i = 0, length = mirros.Count; i < length; i++)
                                            {
                                                mirros[i].mirror = -1;
                                            }
                                            go.transform.localScale = new Vector3(-1, 1, 1);

                                            if (effectInfo.effectData.isRotate)
                                            {
                                                offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                                offsetZ *= -1;
                                                qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                            }
                                        }
                                        else
                                        {
                                            if (effectInfo.effectData.isRotate)
                                            {
                                                offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                                offsetZ *= -1;
                                                qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                go.transform.localRotation = qua;
                                LockTransform lt = go.AddComponent<LockTransform>();
                                Vector3 offset = effectInfo.effectData.offset;
                                if (!effectInfo.target.isPlayer)
                                    offset.x *= -1;
                                lt.offset = offset + new Vector3(0, 0, offsetZ);
                                lt.trans = effectInfo.target.rootNode.transform;
                                lt.ignoreYaxis = true;
                            }
                            break;
                        case EffectType.MoveTargetPos:
                            {
                                if (isPlayEffect)
                                {
                                    go = GameObject.Instantiate(particle, effectInfo.pos, Quaternion.identity) as GameObject;
                                    go.transform.SetParent(_parentTrans, false);
                                    #region rotate
                                    if (effectInfo.effectData.isRotate)
                                    {
                                        float offsetZ = 0f;
                                        Quaternion qua = Quaternion.identity;
                                        if (effectInfo.target.isPlayer)
                                        {
                                            offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                            qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                        }
                                        else
                                        {
                                            if (mirror)
                                            {
                                                List<ParticleMirror> mirros = GameObjectUtil.AddComponent2GameObject<ParticleMirror, Renderer>(go);
                                                for (int i = 0, length = mirros.Count; i < length; i++)
                                                {
                                                    mirros[i].mirror = -1;
                                                }
                                                go.transform.localScale = new Vector3(-1, 1, 1);
                                                offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                                offsetZ *= -1;
                                                qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                            }
                                            else//有问题的，要测试才通过
                                            {
                                                offsetZ = Mathf.Sin(effectInfo.rotateAngles.y * Mathf.PI / 180) * effectInfo.effectData.offset.x;
                                                offsetZ *= -1;
                                                qua.eulerAngles = new Vector3(effectInfo.rotateAngles.x, -effectInfo.rotateAngles.y, effectInfo.rotateAngles.z);
                                            }
                                        }
                                        go.transform.localRotation = qua;
                                        Vector3 offset = effectInfo.effectData.offset;
                                        if (!effectInfo.target.isPlayer)
                                            offset.x *= -1;
                                        go.transform.localPosition = go.transform.localPosition + offset + new Vector3(0, 0, offsetZ);
                                    }
                                    #endregion
                                }
                            }
                            break;
                        case EffectType.FullScreen:
                            if (isPlayEffect)
                            {
                                go = GameObject.Instantiate(particle, effectInfo.pos, Quaternion.identity) as GameObject;
                                go.transform.SetParent(_parentTrans, false);
                            }
                            break;
                        case EffectType.BlackScreen:
                            float blackTime = effectInfo.length;
                            switch (Fight.Controller.FightController.instance.fightStatus)
                            {
                                case FightStatus.Normal:
                                case FightStatus.FloatWaiting:
                                case FightStatus.TumbleWaiting:
                                    if (effectInfo.skillInfo != null)
                                        blackTime -= effectInfo.skillInfo.skillData.pauseTime;
                                    if (blackTime < 0)
                                        blackTime = GameSetting.instance.deltaTimeFight;
                                    ShowRedBackgroundEffect(blackTime);
                                    Map.Controller.MapController.instance.MapTrigger(blackTime, effectInfo.effectData.color.a);
                                    go = GameObject.Instantiate(particle, Vector3.zero, Quaternion.identity) as GameObject;
                                    go.transform.SetParent(_parentTrans, false);
                                    go.transform.localPosition = new Vector3(0f, 0f, -15f);
                                    break;
                                case FightStatus.FloatComboing:
                                case FightStatus.TumbleComboing:
                                    return null;
                                    break;
                            }
                            break;

                        case EffectType.BGEffect:
                            float effectTime = effectInfo.length;
                            switch (Fight.Controller.FightController.instance.fightStatus)
                            {
                                case FightStatus.Normal:
                                case FightStatus.FloatWaiting:
                                case FightStatus.TumbleWaiting:
                                    if (effectInfo.skillInfo != null)
                                        effectTime -= effectInfo.skillInfo.skillData.pauseTime;
                                    if (effectTime < 0)
                                        effectTime = GameSetting.instance.deltaTimeFight;
                                    //ShowBgEffect(effectTime, effectInfo.effectData.effectName);
                                    Map.Controller.MapController.instance.MapTrigger(effectTime, effectInfo.effectData.color.a);
                                    go = GameObject.Instantiate(particle, Vector3.zero, Quaternion.identity) as GameObject;
                                    go.transform.SetParent(_parentTrans, false);
                                    go.transform.localPosition = new Vector3(0f, 0f, -15f);
                                    break;
                                case FightStatus.FloatComboing:
                                case FightStatus.TumbleComboing:
                                    return null;
                                    break;
                            }
                            break;
                    }
                    if (isPlayEffect)
                    {
                        if (go)
                        {
                            TransformUtil.SwitchLayer(go.transform, layer);
                            go.name = GetEffectNameContactTime(effectInfo.target, effectInfo.effectData);
                            switch (FightController.instance.fightStatus)
                            {
                                case FightStatus.Normal:
                                case FightStatus.FloatWaiting:
                                case FightStatus.TumbleWaiting:
                                    if (layer == (int)LayerType.FightCombo)
                                        _comboMakeEffects.Add(go);
                                    if (effectInfo.effectData.ignoreTimeScale)//忽略暂停
                                    {
                                        float length = 0f;
                                        if (effectInfo.effectData.effectType == EffectType.FullScreen)
                                            length = effectInfo.length;
                                        else
                                            length = effectInfo.effectData.length;
                                        ParticlePlayer pp = go.AddComponent<ParticlePlayer>();
                                        pp.duration = length / GameSetting.instance.speed;
                                        pp.speed = GameSetting.instance.speed;
                                        pp.character = effectInfo.target;
                                        pp.onDestroyEvent += OnDestroyEffect;
                                    }
                                    else
                                    {
                                        if (effectInfo.effectData.effectType == EffectType.Trace || effectInfo.effectData.effectType == EffectType.CurveTrace)//移动特效到达目标点销毁
                                            Move(go, effectInfo);
                                        else
                                        {
                                            float length = 0f;
                                            if (effectInfo.effectData.effectType == EffectType.FullScreen || effectInfo.effectData.effectType == EffectType.BlackScreen||effectInfo.effectData.effectType == EffectType.BGEffect)
                                                length = effectInfo.length;
                                            else
                                                length = effectInfo.effectData.length;
                                            StartCoroutine(DestroyEffectCoroutine(length, go));//其他特效到时间销毁
                                        }
                                    }
                                    break;
                                case FightStatus.FloatComboing:
                                case FightStatus.TumbleComboing:
                                    TransformUtil.SwitchLayer(go.transform, (int)LayerType.FightCombo);
                                    ParticlePlayer particlePlayer = go.AddComponent<ParticlePlayer>();
                                    particlePlayer.speed = GameSetting.instance.speed;
                                    particlePlayer.character = effectInfo.target;
                                    if (effectInfo.effectData.effectType == EffectType.Trace || effectInfo.effectData.effectType == EffectType.CurveTrace)//移动特效到达目标点销毁
                                    {
                                        particlePlayer.isLoop = true;
                                        MoveCombo(go, effectInfo);
                                    }
                                    else//其他特效到时间销毁
                                    {
                                        float length = 0f;
                                        if (effectInfo.effectData.effectType == EffectType.FullScreen)
                                            length = effectInfo.length;
                                        else
                                            length = effectInfo.effectData.length;
                                        particlePlayer.duration = length / GameSetting.instance.speed;
                                        particlePlayer.onDestroyEvent += OnDestroyEffect;
                                    }
                                    break;
                            }
                        }
                    }
                }
                return go;
            }
        }
        #endregion

        #region effects move
        private void Move(GameObject go, EffectInfo effectInfo)
        {
            switch (effectInfo.effectData.effectType)
            {
                case EffectType.Trace:
                    StartCoroutine(MoveCoroutine(go, effectInfo));
                    break;
                case EffectType.CurveTrace:
                    StartCoroutine(CurveMoveCoroutine(go, effectInfo));
                    break;
            }
        }

        private void MoveCombo(GameObject go, EffectInfo effectInfo)
        {
            switch (effectInfo.effectData.effectType)
            {
                case EffectType.Trace:
                    StartCoroutine(MoveComboCoroutine(go, effectInfo));
                    break;
                case EffectType.CurveTrace:
                    StartCoroutine(CurveMoveComboCoroutine(go, effectInfo));
                    break;
            }
        }

        private IEnumerator MoveComboCoroutine(GameObject go, EffectInfo effectInfo)
        {
            Vector3 startPos = effectInfo.pos;
            Vector3 endPos = effectInfo.endPos;
            Vector3 normal = (endPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, endPos);
            float deltaTime = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            float lastTime = Time.realtimeSinceStartup;
            float time = effectInfo.time;
            float speed = distance / (time / deltaTime) * 2;
            if (isPlayEffect)
                go.transform.LookAt(endPos);
            normal *= speed;
            Vector3 curPos = startPos;
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < deltaTime)
                    {
                        yield return null;
                        continue;
                    }
                    else
                        lastTime = Time.realtimeSinceStartup;
                    if (TimeController.instance.playerPause) continue;
                    curPos += normal;
                    if (isPlayEffect)
                    {
                        if (go)
                            go.transform.localPosition = curPos;
                        else
                            break;
                    }
                }
            }

            else
            {
                while (curPos.x > endPos.x)
                {
                    if (Time.realtimeSinceStartup - lastTime < deltaTime)
                    {
                        yield return null;
                        continue;
                    }
                    else
                        lastTime = Time.realtimeSinceStartup;
                    if (TimeController.instance.playerPause) continue;
                    curPos += normal;
                    if (isPlayEffect)
                    {
                        if (go)
                            go.transform.localPosition = curPos;
                        else
                            break;
                    }
                }
            }
            if (isPlayEffect)
            {
                _effectDic.TryDelete(go.name);
                UnityEngine.Object.Destroy(go);
            }
            go = null;
        }

        private IEnumerator MoveCoroutine(GameObject go, EffectInfo effectInfo)
        {
            Vector3 startPos = effectInfo.pos;
            Vector3 endPos = effectInfo.endPos;
            Vector3 normal = (endPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, endPos);
            float deltaTime = GameSetting.instance.deltaTimeFight;
            WaitForSeconds waitForSeconds = new WaitForSeconds(deltaTime);
            float time = effectInfo.time;
            //Debugger.LogError(effectInfo.positionRow + "  " + distance + "   " + time);
            float speed = distance / (time / deltaTime) * 2;//乘以2就对了
            //Debugger.LogError("distance:" + distance + "  " + effectBase.duration + "  " + speed);
            if (isPlayEffect)
                go.transform.LookAt(endPos);
            normal *= speed;
            //normal.y = 0f;
            //Debugger.LogError("normal:" + normal);
            Vector3 curPos = startPos;
            if (startPos.x < endPos.x)
            {
                while (curPos.x < endPos.x)
                {
                    curPos += normal;
                    if (isPlayEffect)
                    {
                        if (go)
                            go.transform.localPosition = curPos;
                        else
                            break;
                    }
                    yield return waitForSeconds;
                }
            }
            else
            {
                while (curPos.x > endPos.x)
                {
                    curPos += normal;
                    if (isPlayEffect)
                    {
                        if (go)
                            go.transform.localPosition = curPos;
                        else
                            break;
                    }
                    yield return waitForSeconds;
                }
            }
            if (isPlayEffect)
            {
                if (go)
                {
                    _effectDic.TryDelete(go.name);
                    UnityEngine.Object.Destroy(go);
                }
            }
            go = null;
        }

        private IEnumerator CurveMoveComboCoroutine(GameObject go, EffectInfo effectInfo)
        {
            Vector3 startPos = effectInfo.pos;
            Vector3 endPos = effectInfo.endPos;
            float time = effectInfo.time / GameSetting.instance.speed;
            float deltaTime = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            go.transform.LookAt(endPos);

            Vector3 oneDivideFour = Vector3.Slerp(startPos, endPos, 1f / 4f);
            Vector3 twoDivideFour = Vector3.Slerp(startPos, endPos, 2f / 4f);
            Vector3 threeDivideFour = Vector3.Slerp(startPos, endPos, 3f / 4f);
            Vector3 point1 = effectInfo.effectData.curvePoint1.Key + oneDivideFour;
            Vector2 point1YZ = VectorUtil.RotateAngle(new Vector2(oneDivideFour.y, oneDivideFour.z), new Vector2(point1.y, point1.z), effectInfo.effectData.curvePoint1.Value);
            Vector3 point1Target = new Vector3(point1.x, point1YZ.x, point1YZ.y);

            Vector3 point2 = effectInfo.effectData.curvePoint2.Key + twoDivideFour;
            Vector2 point2YZ = VectorUtil.RotateAngle(new Vector2(twoDivideFour.y, twoDivideFour.z), new Vector2(point2.y, point2.z), effectInfo.effectData.curvePoint2.Value);
            Vector3 point2Target = new Vector3(point2.x, point2YZ.x, point2YZ.y);

            Vector3 point3 = effectInfo.effectData.curvePoint3.Key + threeDivideFour;
            Vector2 point3YZ = VectorUtil.RotateAngle(new Vector2(threeDivideFour.y, threeDivideFour.z), new Vector2(point3.y, point3.z), effectInfo.effectData.curvePoint3.Value);
            Vector3 point3Target = new Vector3(point3.x, point3YZ.x, point3YZ.y);

            List<float> times = new List<float>() { 0, 1 / 4f, 2 / 4f, 3 / 4f, 1 };
            List<float> valuesX = new List<float> { startPos.x, point1Target.x, point2Target.x, point3Target.x, endPos.x };
            List<float> valuesY = new List<float> { startPos.y, point1Target.y, point2Target.y, point3Target.y, endPos.y };
            List<float> valuesZ = new List<float> { startPos.z, point1Target.z, point2Target.z, point3Target.z, endPos.z };

            AnimationCurve curveX = AnimationUtil.CreateAnimationCurve(times, valuesX);
            AnimationCurve curveY = AnimationUtil.CreateAnimationCurve(times, valuesY);
            AnimationCurve curveZ = AnimationUtil.CreateAnimationCurve(times, valuesZ);

#if UNITY_EDITOR
            oneDivideFourEditor = oneDivideFour;
            twoDivideFourEditor = twoDivideFour;
            threeDivideFourEditor = threeDivideFour;
            point1TargetEditor = point1Target;
            point2TargetEditor = point2Target;
            point3TargetEditor = point3Target;
            curveXEditor = curveX;
            curveYEditor = curveY;
            curveZEditor = curveZ;
#endif
            float startTime = Time.realtimeSinceStartup;
            float costTime = 0;
            float lastTime = startTime;
            float currentTime = lastTime;
            while (Time.realtimeSinceStartup - startTime < time)
            {
                if (Time.realtimeSinceStartup - lastTime < deltaTime)
                {
                    yield return null;
                    continue;
                }
                else
                    lastTime = Time.realtimeSinceStartup;
                if (go)
                    go.transform.localPosition = AnimationUtil.GetVector3FromCurves(curveX, curveY, curveZ, costTime / time * 2);//不知道为何要乘以2
                else
                    break;
                if (TimeController.instance.playerPause)
                    startTime += (Time.realtimeSinceStartup - currentTime);
                else
                    costTime += deltaTime;
                currentTime = Time.realtimeSinceStartup;
            }
            if (isPlayEffect)
            {
                if (go)
                {
                    _effectDic.TryDelete(go.name);
                    UnityEngine.Object.Destroy(go);
                }
            }
            go = null;
        }

        private IEnumerator CurveMoveCoroutine(GameObject go, EffectInfo effectInfo)
        {
            Vector3 startPos = effectInfo.pos;
            Vector3 endPos = effectInfo.endPos;
            float time = effectInfo.time;
            float deltaTime = GameSetting.instance.deltaTimeFight;
            go.transform.LookAt(endPos);

            Vector3 oneDivideFour = Vector3.Slerp(startPos, endPos, 1f / 4f);
            Vector3 twoDivideFour = Vector3.Slerp(startPos, endPos, 2f / 4f);
            Vector3 threeDivideFour = Vector3.Slerp(startPos, endPos, 3f / 4f);
            Vector3 point1 = effectInfo.effectData.curvePoint1.Key + oneDivideFour;
            Vector2 point1YZ = VectorUtil.RotateAngle(new Vector2(oneDivideFour.y, oneDivideFour.z), new Vector2(point1.y, point1.z), effectInfo.effectData.curvePoint1.Value);
            Vector3 point1Target = new Vector3(point1.x, point1YZ.x, point1YZ.y);

            Vector3 point2 = effectInfo.effectData.curvePoint2.Key + twoDivideFour;
            Vector2 point2YZ = VectorUtil.RotateAngle(new Vector2(twoDivideFour.y, twoDivideFour.z), new Vector2(point2.y, point2.z), effectInfo.effectData.curvePoint2.Value);
            Vector3 point2Target = new Vector3(point2.x, point2YZ.x, point2YZ.y);

            Vector3 point3 = effectInfo.effectData.curvePoint3.Key + threeDivideFour;
            Vector2 point3YZ = VectorUtil.RotateAngle(new Vector2(threeDivideFour.y, threeDivideFour.z), new Vector2(point3.y, point3.z), effectInfo.effectData.curvePoint3.Value);
            Vector3 point3Target = new Vector3(point3.x, point3YZ.x, point3YZ.y);

            List<float> times = new List<float>() { 0, 1 / 4f, 2 / 4f, 3 / 4f, 1 };
            List<float> valuesX = new List<float> { startPos.x, point1Target.x, point2Target.x, point3Target.x, endPos.x };
            List<float> valuesY = new List<float> { startPos.y, point1Target.y, point2Target.y, point3Target.y, endPos.y };
            List<float> valuesZ = new List<float> { startPos.z, point1Target.z, point2Target.z, point3Target.z, endPos.z };

            AnimationCurve curveX = AnimationUtil.CreateAnimationCurve(times, valuesX);
            AnimationCurve curveY = AnimationUtil.CreateAnimationCurve(times, valuesY);
            AnimationCurve curveZ = AnimationUtil.CreateAnimationCurve(times, valuesZ);

#if UNITY_EDITOR
            oneDivideFourEditor = oneDivideFour;
            twoDivideFourEditor = twoDivideFour;
            threeDivideFourEditor = threeDivideFour;
            point1TargetEditor = point1Target;
            point2TargetEditor = point2Target;
            point3TargetEditor = point3Target;
            curveXEditor = curveX;
            curveYEditor = curveY;
            curveZEditor = curveZ;
#endif
            WaitForSeconds waitForSeconds = new WaitForSeconds(deltaTime);
            float startTime = Time.time;
            float currentTime = 0;
            while (Time.time - startTime <= time)
            {
                if (go)
                    go.transform.localPosition = AnimationUtil.GetVector3FromCurves(curveX, curveY, curveZ, currentTime / time * 2);//不知道为何要乘以2
                else
                    break;
                currentTime += deltaTime;
                yield return waitForSeconds;
            }
            //Debugger.Log("originalTime:" + time + "costTime:" + (Time.time - lastTime));
            if (isPlayEffect)
            {
                if (go)
                {
                    _effectDic.TryDelete(go.name);
                    UnityEngine.Object.Destroy(go);
                }
            }
            go = null;
        }

#if UNITY_EDITOR
        Vector3 oneDivideFourEditor = Vector3.zero;
        Vector3 twoDivideFourEditor = Vector3.zero;
        Vector3 threeDivideFourEditor = Vector3.zero;
        Vector3 point1TargetEditor = Vector3.zero;
        Vector3 point2TargetEditor = Vector3.zero;
        Vector3 point3TargetEditor = Vector3.zero;
        AnimationCurve curveXEditor;
        AnimationCurve curveYEditor;
        AnimationCurve curveZEditor;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(oneDivideFourEditor, 0.1f);
            Gizmos.DrawSphere(twoDivideFourEditor, 0.1f);
            Gizmos.DrawSphere(threeDivideFourEditor, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(point1TargetEditor, 0.1f);
            Gizmos.DrawSphere(point2TargetEditor, 0.1f);
            Gizmos.DrawSphere(point3TargetEditor, 0.1f);
            Gizmos.color = Color.red;
            if (curveXEditor != null && curveYEditor != null && curveZEditor != null)
            {
                AnimationUtil.gizmoDraw(curveXEditor, curveYEditor, curveZEditor);
            }

        }
#endif
        #endregion

        #region generate effect name
        public string GetEffectName(CharacterEntity character, string effectName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (character)
            {
                sb.Append(character.gameObject.name);
                if (character.characterInfo != null)
                    sb.Append(character.characterInfo.instanceID);
            }
            sb.Append(effectName);
            return sb.ToString();
        }

        private string GetEffectNameContactTime(CharacterEntity character, EffectData effectData)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (character)
            {
                sb.Append(character.gameObject.name);
                if (character.characterInfo != null)
                    sb.Append(character.characterInfo.instanceID);
            }
            sb.Append(effectData.effectName);
            sb.Append(effectData.effectId);
            sb.Append(Time.realtimeSinceStartup.ToString());
            return sb.ToString();
        }
        #endregion

        #region attack tips
        private GameObject GetAttackRootTips()
        {
            GameObject result = null;
            if (_attackRootTips.Count > 0)
            {
                result = _attackRootTips.First();
                if (result)
                    result.SetActive(true);
                _attackRootTips.RemoveAt(0);
            }
            else
            {
                result = GameObject.Instantiate(ResMgr.instance.Load<GameObject>("effects/prefabs/attak_root_tip")) as GameObject;
                if (result)
                {
                    result.transform.localEulerAngles = new Vector3(85, 0, 0);
                    result.transform.localScale = Vector3.one;
                    result.transform.SetParent(_parentTrans, false);
                    TransformUtil.SwitchLayer(result.transform, (int)LayerType.Fight);
                }
            }
            return result;
        }

        private void ResetAttackRootTips(GameObject go)
        {
            if (!go) return;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
            go.transform.localEulerAngles = new Vector3(85, 0, 0);
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            _attackRootTips.Add(go);
        }

        private void ClearAttackRootTips()
        {
            for (int i = 0, count = _attackRootTips.Count; i < count; i++)
            {
                GameObject go = _attackRootTips[i];
                UnityEngine.Object.Destroy(go);
            }
            _attackRootTips.Clear();
        }

        /// <summary>
        /// 设置受击时脚下方圈
        /// </summary>
        /// <param name="pid">位置id</param>
        public void SetAttackRootTips(uint positionId)
        {
            Vector3 pos = Logic.Position.Model.PositionData.GetPos(positionId);
            GameObject go = GetAttackRootTips();
            if (go)
            {
                go.transform.localPosition = pos;
                if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal)
                    StartCoroutine(SetAttackRootTipsCoroutine(go));
                else
                    StartCoroutine(SetComboAttackRootTipsCoroutine(go));
            }
        }

        private IEnumerator SetAttackRootTipsCoroutine(GameObject go)
        {
            float time = 0.3f;
            float _time = time;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Color curColor = sr.color;
            float deltaTime = GameSetting.instance.deltaTimeFight;
            while (_time > 0)
            {
                if (Time.timeScale == 0)
                {
                    yield return null;
                    continue;
                }

                yield return deltaTime;
                _time -= deltaTime;
                if (_time <= time / 2)
                    curColor.a *= 0.85f;
                if (sr)
                    sr.color = curColor;
                if (go)
                    go.transform.localScale *= (1 + 0.04f * GameSetting.instance.speed);
            }
            //UnityEngine.Object.Destroy(go);
            //go = null;
            ResetAttackRootTips(go);
        }

        private IEnumerator SetComboAttackRootTipsCoroutine(GameObject go)
        {
            float time = 0.3f;
            float _time = time;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Color curColor = sr.color;
            float deltaTime = GameSetting.instance.deltaTimeFight / GameSetting.instance.speed;
            float lastTime = Time.realtimeSinceStartup;
            float delay = deltaTime;
            while (_time > 0)
            {
                if (Time.realtimeSinceStartup - lastTime < delay)
                {
                    yield return null;
                    continue;
                }
                else
                    lastTime = Time.realtimeSinceStartup;
                if (TimeController.instance.playerPause)
                    continue;
                _time -= deltaTime;
                if (_time <= time / 2)
                    curColor.a *= 0.85f;
                if (sr)
                    sr.color = curColor;
                if (go)
                    go.transform.localScale *= (1 + 0.04f);
            }
            //UnityEngine.Object.Destroy(go);
            //go = null;
            ResetAttackRootTips(go);
        }
        #endregion

        #region target range tips
        private GameObject GetTargetRangeTips()
        {
            GameObject result = null;
            if (_targetRangeTips.Count > 0)
            {
                result = _targetRangeTips.First();
                result.SetActive(true);
                _targetRangeTips.RemoveAt(0);
            }
            else
            {
                result = GameObject.Instantiate(ResMgr.instance.Load<GameObject>("effects/prefabs/target_range_tip")) as GameObject;
                result.transform.localEulerAngles = new Vector3(85, 0, 0);
                result.transform.SetParent(_parentTrans, false);
                TransformUtil.SwitchLayer(result.transform, (int)LayerType.Fight);
            }
            return result;
        }

        private void ResetTargetRangeTips(GameObject go)
        {
            go.transform.localEulerAngles = new Vector3(85, 0, 0);
            go.SetActive(false);
            _targetRangeTips.Add(go);
        }

        private void ClearTargetRangeTips()
        {
            for (int i = 0, count = _targetRangeTips.Count; i < count; i++)
            {
                GameObject go = _targetRangeTips[i];
                UnityEngine.Object.Destroy(go);
            }
            _targetRangeTips.Clear();
            _characterTargetRangeTips.Clear();
        }

        public void ShowTargetRangeTips(CharacterEntity character, List<uint> positionIds)
        {
            if (positionIds == null || positionIds.Count == 0) return;
            List<GameObject> tips = new List<GameObject>();
            for (int i = 0, count = positionIds.Count; i < count; i++)
            {
                uint positionId = positionIds[i];
                Vector3 pos = Logic.Position.Model.PositionData.GetPos(positionId);
                GameObject go = GetTargetRangeTips();
                go.transform.localPosition = pos;
                tips.Add(go);
            }
            _characterTargetRangeTips[character.characterInfo.instanceID] = tips;
        }

        public void HideTargetRangeTips(CharacterEntity character)
        {
            List<GameObject> tips = null;
            if (_characterTargetRangeTips.TryGetValue(character.characterInfo.instanceID, out tips))
            {
                for (int i = 0, count = tips.Count; i < count; i++)
                {
                    ResetTargetRangeTips(tips[i]);
                }
                _characterTargetRangeTips.Remove(character.characterInfo.instanceID);
            }
        }
        #endregion
    }
}