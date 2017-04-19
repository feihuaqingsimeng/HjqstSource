using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Buff.View;
using PathologicalGames;
using Logic.Pool.Controller;
using Common.ResMgr;
using Logic.Character;
namespace Logic.UI.Buff.Controller
{
    public class BuffTipsController : SingletonMono<BuffTipsController>
    {
        private Transform _buffTipsViewPool;
        private SpawnPool _spawnPool;
        private PrefabPool _prefabPool;
        private GameObject _prefab;
        private const string POOL_NAME = "buffTipsViews";
        private Dictionary<uint, KeyValuePair<float, float>> _buffTipsDic = new Dictionary<uint, KeyValuePair<float, float>>();
        void Awake()
        {
            instance = this;
        }

        #region object pool
        public void InitBuffTipsViewPool()
        {
            _buffTipsDic.Clear();
            if (!_buffTipsViewPool)
            {
                GameObject go = new GameObject(POOL_NAME);
                _buffTipsViewPool = go.transform;
                _buffTipsViewPool.SetParent(UI.UIMgr.instance.basicCanvas.transform, false);
                //create _spawnPoll
                _spawnPool = PoolController.instance.CreatePool(POOL_NAME, BuffTipsView.PREFAB_PATH, true);
                _spawnPool.matchPoolScale = true;

                _prefab = ResMgr.instance.Load<GameObject>(BuffTipsView.PREFAB_PATH);
                _prefabPool = new PrefabPool(_prefab.transform);
                _prefabPool.preloadAmount = 5;//默认初始化5个Prefab
                _prefabPool.limitInstances = true;//开启限制
                _prefabPool.limitFIFO = true;//开启无限取Prefab
                _prefabPool.limitAmount = 20; //限制池子里最大的Prefab数量
                _prefabPool.preloadTime = true;//开启预加载
                _prefabPool.preloadFrames = 2;//每帧加载个数
                _prefabPool.preloadDelay = 2;//延迟几秒开始预加载
                _prefabPool.cullDespawned = true;//开启自动清理
                _prefabPool.cullAbove = 5;//缓存池自动清理，但是始终保存几个对象不清理
                _prefabPool.cullDelay = 10;//每过多久执行一次清理(销毁)，单位秒
                _prefabPool.cullMaxPerPass = 2;//每次自动清理个数
                //初始化内存池
                //_spawnPool._perPrefabPoolOptions.Add(_prefabPool);
                _spawnPool.CreatePrefabPool(_prefabPool);
            }
        }

        public void ResetBuffTipsViewPool()
        {
            _buffTipsDic.Clear();
            foreach (var kvp in _spawnPool.prefabPools)
            {
                for (int i = 0, count = kvp.Value.despawned.Count; i < count; i++)
                {
                    kvp.Value.despawned[i].SetParent(_spawnPool.transform, false);
                }
            }
        }

        private void OnDestroy()
        {
            if (_buffTipsViewPool)
                GameObject.Destroy(_buffTipsViewPool.gameObject);
            _buffTipsViewPool = null;
            _spawnPool = null;
            _prefabPool = null;
            _prefab = null;
            _buffTipsDic.Clear();
            _buffTipsDic = null;
        }

        public void DespawnBuffTipsView(Transform prefab)
        {
            if (_spawnPool)
                _spawnPool.Despawn(prefab);
        }
        #endregion

        public void ShowBuffTipsView(CharacterEntity character, string tips)
        {
            GameObject go = _spawnPool.Spawn(_prefab.transform, _buffTipsViewPool).gameObject;
            BuffTipsView bt = go.GetComponent<BuffTipsView>();
            bt.SetTips(tips);
            KeyValuePair<float, float> last = default(KeyValuePair<float, float>);
            float offset = 0f;
            Vector3 pos = character.pos + new Vector3(0f, character.height, 0f);
            if (_buffTipsDic.ContainsKey(character.characterInfo.instanceID))
                last = _buffTipsDic[character.characterInfo.instanceID];
            if (Time.time - last.Key < 0.2f)
                offset = last.Value - 20;
            else
                offset = 0;
            bt.offset = offset;
            bt.worldPos = pos;
            _buffTipsDic[character.characterInfo.instanceID] = new KeyValuePair<float, float>(Time.time, offset);
        }
    }
}