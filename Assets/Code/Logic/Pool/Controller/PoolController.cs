using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Common.ResMgr;
using Logic.Character;
using LuaInterface;
using Common.Components.Trans;
namespace Logic.Pool.Controller
{
    public class PoolController : SingletonMono<PoolController>
    {
        private Dictionary<string, bool> _spawnPoolDic = new Dictionary<string, bool>();
        private Dictionary<string, string> _pathDic = new Dictionary<string, string>();
#if UNITY_EDITOR
        [NoToLua]
        public Dictionary<string, bool> spawnPoolDic
        {
            get
            {
                return _spawnPoolDic;
            }
        }

        [NoToLua]
        public Dictionary<string, string> pathDic
        {
            get
            {
                return _pathDic;
            }
        }
#endif
        void Awake()
        {
            instance = this;
        }

        public SpawnPool CreatePool(string path, string poolName, bool forever = false)
        {
            GameObject poolGO = new GameObject(poolName);
            poolGO.transform.SetParent(transform, false);
            SpawnPool spawnPool = PoolManager.Pools.Create(poolName, poolGO);
            if (spawnPool)
            {
                _spawnPoolDic.Add(poolName, forever);
                _pathDic.Add(poolName, path);
            }
            return spawnPool;
        }

        public SpawnPool CreateCharacterPool(string path, string poolName, bool forever)
        {
            GameObject poolGO = new GameObject(poolName);
            poolGO.transform.SetParent(transform, false);
            SpawnPool spawnPool = PoolManager.Pools.Create(poolName, poolGO);
            spawnPool.matchPoolScale = true;

            GameObject _prefab = ResMgr.instance.Load<GameObject>(path);
            PrefabPool _prefabPool = new PrefabPool(_prefab.transform);
            if (path.Contains("player/") || path.Contains("pet/"))
            {
                _prefabPool.preloadAmount = 2;//默认初始化5个Prefab
                _prefabPool.limitInstances = true;//开启限制
                _prefabPool.limitFIFO = true;//开启无限取Prefab
                _prefabPool.limitAmount = 15; //限制池子里最大的Prefab数量
                _prefabPool.cullAbove = 2;//缓存池自动清理，但是始终保存几个对象不清理
            }
            else
            {
                _prefabPool.preloadAmount = 1;//默认初始化5个Prefab
                _prefabPool.limitInstances = true;//开启限制
                _prefabPool.limitFIFO = true;//开启无限取Prefab
                _prefabPool.limitAmount = 10; //限制池子里最大的Prefab数量
                _prefabPool.cullAbove = 1;//缓存池自动清理，但是始终保存几个对象不清理
            }
            _prefabPool.preloadTime = true;//开启预加载
            _prefabPool.preloadFrames = 1;//每帧加载个数
            _prefabPool.preloadDelay = 0;//延迟几秒开始预加载
            _prefabPool.cullDespawned = true;//开启自动清理
            _prefabPool.cullDelay = 60;//每过多久执行一次清理(销毁)，单位秒
            _prefabPool.cullMaxPerPass = 1;//每次自动清理个数
            //初始化内存池
            //_spawnPool._perPrefabPoolOptions.Add(_prefabPool);
            spawnPool.CreatePrefabPool(_prefabPool);
            _spawnPoolDic.Add(poolName, forever);
            _pathDic.Add(poolName, path);
            return spawnPool;
        }

        public void CreateCharacterPool(string path, string poolName, bool forever, System.Action<SpawnPool> callBack)
        {
            ResMgr.instance.Load<GameObject>(path, (_prefab) =>
            {
                if (!_prefab)
                {
                    if (callBack != null)
                        callBack(null);
                    return;
                }
                if (ContainsPool(poolName))
                {
                    if (callBack != null)
                        callBack(GetPool(poolName));
                    return;
                }

                GameObject poolGO = new GameObject(poolName);
                poolGO.transform.SetParent(transform, false);
                SpawnPool spawnPool = PoolManager.Pools.Create(poolName, poolGO);
                spawnPool.matchPoolScale = true;
                PrefabPool _prefabPool = new PrefabPool(_prefab.transform);
                if (path.Contains("player/") || path.Contains("pet/"))
                {
                    _prefabPool.preloadAmount = 2;//默认初始化5个Prefab
                    _prefabPool.limitInstances = true;//开启限制
                    _prefabPool.limitFIFO = true;//开启无限取Prefab
                    _prefabPool.limitAmount = 15; //限制池子里最大的Prefab数量
                    _prefabPool.cullAbove = 2;//缓存池自动清理，但是始终保存几个对象不清理
                }
                else
                {
                    _prefabPool.preloadAmount = 1;//默认初始化5个Prefab
                    _prefabPool.limitInstances = true;//开启限制
                    _prefabPool.limitFIFO = true;//开启无限取Prefab
                    _prefabPool.limitAmount = 10; //限制池子里最大的Prefab数量
                    _prefabPool.cullAbove = 1;//缓存池自动清理，但是始终保存几个对象不清理
                }
                _prefabPool.preloadTime = true;//开启预加载
                _prefabPool.preloadFrames = 1;//每帧加载个数
                _prefabPool.preloadDelay = 0;//延迟几秒开始预加载
                _prefabPool.cullDespawned = true;//开启自动清理
                _prefabPool.cullDelay = 60;//每过多久执行一次清理(销毁)，单位秒
                _prefabPool.cullMaxPerPass = 1;//每次自动清理个数
                //初始化内存池
                //_spawnPool._perPrefabPoolOptions.Add(_prefabPool);
                spawnPool.CreatePrefabPool(_prefabPool);
                _spawnPoolDic.Add(poolName, forever);
                _pathDic.Add(poolName, path);
                if (callBack != null)
                    callBack(spawnPool);
            }, 0);
        }

        public void Despawn(string poolName, Transform trans)
        {
            SpawnPool spawnPool = Pool.Controller.PoolController.instance.GetPool(poolName);
            if (spawnPool)
            {
                trans.SetParent(spawnPool.transform, false);
                spawnPool.Despawn(trans);
            }
        }

        public void Despawn(string poolName, Logic.Character.CharacterEntity character)
        {
            SpawnPool spawnPool = Pool.Controller.PoolController.instance.GetPool(poolName);
            if (spawnPool)
            {
                if (character is Logic.Character.PlayerEntity)
                {
                    Logic.Character.PlayerEntity playerEntity = character as Logic.Character.PlayerEntity;
                    if (playerEntity.petEntity != null)
                    {
                        LockTransform lockTransform = playerEntity.petEntity.gameObject.GetComponent<LockTransform>();
                        if (lockTransform)
                            lockTransform.trans = null;
                        Despawn(playerEntity.petEntity.name, playerEntity.petEntity.transform);
                    }
                    playerEntity.petEntity = null;
                }
                else if (character is Logic.Character.EnemyPlayerEntity)
                {
                    Logic.Character.EnemyPlayerEntity enemyPlayerEntity = character as Logic.Character.EnemyPlayerEntity;
                    if (enemyPlayerEntity.petEntity != null)
                    {
                        LockTransform lockTransform = enemyPlayerEntity.petEntity.gameObject.GetComponent<LockTransform>();
                        if (lockTransform)
                            lockTransform.trans = null;
                        Despawn(enemyPlayerEntity.petEntity.name, enemyPlayerEntity.petEntity.transform);
                    }
                    enemyPlayerEntity.petEntity = null;
                }
                character.transform.SetParent(spawnPool.transform, false);
                spawnPool.Despawn(character.transform);
            }
        }

        public SpawnPool GetPool(string poolName)
        {
            SpawnPool result = null;
            PoolManager.Pools.TryGetValue(poolName, out result);
            return result;
        }

        public bool ContainsPool(string poolName)
        {
            return PoolManager.Pools.ContainsKey(poolName);
        }

        public void ClearTemporaryPools()
        {
            List<string> temporarys = new List<string>();
            foreach (var kvp in _spawnPoolDic)
            {
                if (kvp.Value) continue;
                temporarys.Add(kvp.Key);
            }

            for (int i = 0, count = temporarys.Count; i < count; i++)
            {
                string poolName = temporarys[i];
                _spawnPoolDic.Remove(poolName);
                if (_pathDic.ContainsKey(poolName))
                    _pathDic.Remove(poolName);
                SpawnPool spawnPool = GetPool(poolName);
                if (spawnPool)
                    GameObject.Destroy(spawnPool.gameObject);
            }
            temporarys.Clear();
            temporarys = null;
        }

        public bool ExsitInPool(string path)
        {
            foreach (var kvp in _pathDic)
            {
                if (path.Contains(kvp.Value))
                    return true;
            }
            return false;
        }

        public void SetPoolForever(string poolName, bool forever)
        {
            if (_spawnPoolDic.ContainsKey(poolName))
                _spawnPoolDic[poolName] = forever;
            else
                _spawnPoolDic.Add(poolName, forever);
        }

        void OnDestroy()
        {
            _spawnPoolDic.Clear();
        }
    }
}