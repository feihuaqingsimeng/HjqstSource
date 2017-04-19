using UnityEngine;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Util;
using Logic.Enums;
using Logic.Net.Controller;
using Logic.Character.Model;
using Logic.Player;
using Logic.Skill.Model;
using Common.Animators;
using Logic.Hero.Model;
using Logic.Protocol.Model;
using Logic.Shaders;
using Logic.Effect.Model;
using Logic.Effect.Controller;
using Logic.Pet.Model;
using Common.Components.Trans;
using Logic.UI.HPBar.View;
using Logic.Position.Model;
using Common.GameTime.Controller;
using LuaInterface;
using Logic.Fight.Controller;
using Logic.Pool.Controller;
using PathologicalGames;
namespace Logic.Character
{
    public class CharacterEntity : MonoBehaviour
    {
        #region 创建角色
        private static HeroEntity CreateHeroEntity(string modelName)
        {
            string heroModelPath = ResPath.GetHeroModelPath(modelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(modelName))
            {
                Transform trans = PoolController.instance.GetPool(modelName).Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(heroModelPath, modelName, false);
                Transform trans = spawnPool.Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
            }
            //GameObject original = ResMgr.instance.Load<GameObject>(heroModelPath);
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", modelName));
                return null;
            }
            HeroEntity hero = null;
            CharacterEntity character = go.GetComponent<CharacterEntity>();
            if (character)
            {
                hero = go.AddComponent<HeroEntity>();
                hero.anim = character.anim;
                hero.rootNode = character.rootNode;
                UnityEngine.Object.Destroy(character);
                TransformUtil.SwitchLayer(hero.transform, (int)LayerType.Fight);
                if (Game.GameConfig.assetBundle)
                    ShadersUtil.SetShader(hero);
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                ShadersUtil.SetMainColor(hero, ShadersUtil.MAIN_COLOR);
            }
            hero.name = modelName;
            return hero;
        }

        public static HeroEntity CreateHeroEntity(Hero.Model.HeroInfo heroInfo)
        {
            HeroEntity heroEntity = CreateHeroEntity(heroInfo.ModelName);
            if (!heroEntity.bornEffect)
                EffectController.instance.PlayBornEffect(heroEntity, heroInfo.heroData, heroInfo.advanceLevel, false);
            heroEntity.bornEffect = true;
            return heroEntity;
        }

        public static HeroEntity CreateHeroEntityAsUIElement(string modelName, Transform parent, bool canClick, bool canDrag)
        {
            HeroEntity heroEntity = CreateHeroEntity(modelName);
            if (heroEntity == null)
            {
                return null;
            }
            ShadersUtil.SetShaderKeyword(heroEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
            ShadersUtil.SetShaderKeyword(heroEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(heroEntity, ShadersUtil.RIM_MAIN_COLOR);
            ShadersUtil.SetColor(heroEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
            MaterialData materialData = MaterialData.GetMaterialDataByModelName(modelName);
            if (materialData != null)
                ShadersUtil.SetRimPow(heroEntity, materialData.rimPow);

            TransformUtil.SwitchLayer(heroEntity.transform, parent.gameObject.layer);
            heroEntity.transform.SetParent(parent, false);
            heroEntity.transform.localPosition = Vector3.zero;
            heroEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
            heroEntity.transform.localScale = Vector3.one;

            Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = heroEntity.gameObject.GetComponent<Logic.Model.View.ModelRotateAndAnim>();
            if (!modelRotateAndAnim)
                modelRotateAndAnim = heroEntity.gameObject.AddComponent<Logic.Model.View.ModelRotateAndAnim>();
            modelRotateAndAnim.canClick = canClick;
            modelRotateAndAnim.canDrag = canDrag;
            CapsuleCollider capsuleCollider = heroEntity.gameObject.GetComponent<CapsuleCollider>();
            if (!capsuleCollider)
                capsuleCollider = heroEntity.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2.5f;
            capsuleCollider.radius = 1.5f;
            capsuleCollider.center = new Vector3(0f, 1.2f, 0f);
            heroEntity.transform.tag = "Character";

            return heroEntity;
        }
        public static HeroEntity CreateHeroEntityAsUIElementByHeroInfoLuaTable(LuaTable heroInfoLua, Transform parent, bool canClick, bool canDrag)
        {
            HeroInfo info = new HeroInfo(heroInfoLua);
            return CreateHeroEntityAsUIElement(info, parent, canClick, canDrag);
        }
        public static HeroEntity CreateHeroEntityAsUIElement(Hero.Model.HeroInfo heroInfo, Transform parent, bool canClick, bool canDrag)
        {
            HeroEntity heroEntity = CreateHeroEntityAsUIElement(heroInfo.ModelName, parent, canClick, canDrag);
            if (!heroEntity.bornEffect)
                EffectController.instance.PlayBornEffect(heroEntity, heroInfo.heroData, heroInfo.advanceLevel, true, parent.gameObject.layer);
            heroEntity.bornEffect = true;
            return heroEntity;
        }

        public static HeroEntity CreateHeroEntityAs3DUIElementByHeroInfoLuaTable(LuaTable heroInfoLua, Transform parent, bool canClick, bool canDrag)
        {
            HeroInfo heroInfo = new HeroInfo(heroInfoLua);
            return CreateHeroEntityAs3DUIElement(heroInfo, parent, canClick, canDrag);
        }

        public static HeroEntity CreateHeroEntityAs3DUIElement(Hero.Model.HeroInfo heroInfo, Transform parent, bool canClick, bool canDrag)
        {
            HeroEntity heroEntity = CreateHeroEntityAsUIElement(heroInfo.ModelName, parent, canClick, canDrag);
            if (!heroEntity.bornEffect)
                EffectController.instance.PlayBornEffect(heroEntity, heroInfo.heroData, heroInfo.advanceLevel, true, (int)LayerType.UI3D);
            heroEntity.bornEffect = true;
            TransformUtil.SwitchLayer(heroEntity.transform, (int)LayerType.UI3D);
            return heroEntity;
        }

        public static PlayerEntity CreatePlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent)
        {
            string playerModelPath = ResPath.GetPlayerModelPath(playerInfo.ModelName);
            //GameObject original = ResMgr.instance.Load<GameObject>(playerModelPath);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(playerInfo.ModelName).Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(playerModelPath, playerInfo.ModelName, false);
                Transform trans = spawnPool.Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                return null;
            }
            //GameObject go = Instantiate(original) as GameObject;
            CharacterEntity characterEntity = go.GetComponent<CharacterEntity>();
            PlayerEntity playerEntity = go.AddComponent<PlayerEntity>();
            playerEntity.anim = characterEntity.anim;
            playerEntity.rootNode = characterEntity.rootNode;
            UnityEngine.Object.Destroy(characterEntity);
            playerEntity.transform.SetParent(parent, false);
            if (!playerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(playerEntity, playerInfo.heroData, playerInfo.advanceLevel, false);
            playerEntity.bornEffect = true;
            PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerEntity, (int)LayerType.Fight);
            PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerEntity);
            PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, playerEntity);
            PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, playerEntity);
            TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(playerEntity);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            ShadersUtil.SetMainColor(playerEntity, ShadersUtil.MAIN_COLOR);
            #region pet
            PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
            PetEntity petEntity = CreatePetEntiy(petData.modelName);
            petEntity.transform.SetParent(parent, false);
            petEntity.transform.localScale = petData.scale;
            petEntity.transform.localEulerAngles = petData.rotation;
            LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
            if (!lockTransform)
                lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
            lockTransform.trans = playerEntity.rootNode.transform;
            lockTransform.delayRate = petData.speed;
            lockTransform.offset = petData.offset;
            playerEntity.petEntity = petEntity;
            TransformUtil.SwitchLayer(petEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(petEntity);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(petEntity, ShadersUtil.MAIN_COLOR);
            #endregion
            playerEntity.name = playerInfo.ModelName;
            return playerEntity;
        }

        public static EnemyPlayerEntity CreateEnemyPlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent)
        {
            string playerModelPath = ResPath.GetPlayerModelPath(playerInfo.ModelName);
            //GameObject original = ResMgr.instance.Load<GameObject>(playerModelPath);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(playerInfo.ModelName).Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(playerModelPath, playerInfo.ModelName, false);
                Transform trans = spawnPool.Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                return null;
            }
            //GameObject go = Instantiate(original) as GameObject;
            PlayerEntity playerEntity = go.GetComponent<PlayerEntity>();
            EnemyPlayerEntity enemyPlayerEntity = go.AddComponent<EnemyPlayerEntity>();
            enemyPlayerEntity.anim = playerEntity.anim;
            enemyPlayerEntity.characterName = playerEntity.characterName;
            enemyPlayerEntity.rootNode = playerEntity.rootNode;
            UnityEngine.Object.Destroy(playerEntity);
            enemyPlayerEntity.transform.SetParent(parent, false);
            if (!enemyPlayerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(enemyPlayerEntity, playerInfo.heroData, playerInfo.advanceLevel, false);
            enemyPlayerEntity.bornEffect = true;
            PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, enemyPlayerEntity, (int)LayerType.Fight);
            PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, enemyPlayerEntity);
            PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, enemyPlayerEntity);
            PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, enemyPlayerEntity);
            TransformUtil.SwitchLayer(enemyPlayerEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(enemyPlayerEntity);
            ShadersUtil.SetShaderKeyword(enemyPlayerEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(enemyPlayerEntity, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            ShadersUtil.SetMainColor(enemyPlayerEntity, ShadersUtil.MAIN_COLOR);
            #region pet
            PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
            PetEntity petEntity = CreatePetEntiy(petData.modelName);
            petEntity.transform.SetParent(parent, false);
            petEntity.transform.localScale = petData.scale;
            petEntity.transform.localEulerAngles = petData.rotation + new Vector3(0f, 90f, 0f);
            LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
            if (!lockTransform)
                lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
            lockTransform.trans = enemyPlayerEntity.rootNode.transform;
            lockTransform.delayRate = petData.speed;
            lockTransform.offset = petData.offset;
            lockTransform.offset.x *= -1;
            enemyPlayerEntity.petEntity = petEntity;
            TransformUtil.SwitchLayer(petEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(petEntity);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(petEntity, ShadersUtil.MAIN_COLOR);
            #endregion
            enemyPlayerEntity.name = playerInfo.ModelName;
            return enemyPlayerEntity;
        }

        public static PlayerEntity CreatePlayerEntityAsUIElement(Player.Model.PlayerData playerData)
        {
            //GameObject original = ResMgr.instance.Load<GameObject>(playerData.PlayerModelPath);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerData.model))
            {
                Transform trans = PoolController.instance.GetPool(playerData.model).Spawn(playerData.model);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(playerData.PlayerModelPath, playerData.model, false);
                Transform trans = spawnPool.Spawn(playerData.model);
                if (trans)
                    go = trans.gameObject;
            }
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerData.Id, playerData.model));
                return null;
            }
            //GameObject go = Instantiate(original) as GameObject; 
            CharacterEntity characterEntity = go.GetComponent<CharacterEntity>();
            PlayerEntity playerEntity = go.AddComponent<PlayerEntity>();
            playerEntity.anim = characterEntity.anim;
            playerEntity.rootNode = characterEntity.rootNode;
            UnityEngine.Object.Destroy(characterEntity);
            TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.UI);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(playerEntity);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(playerEntity, ShadersUtil.RIM_MAIN_COLOR);
            ShadersUtil.SetColor(playerEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
            MaterialData materialData = MaterialData.GetMaterialDataByModelName(playerData.model);
            if (materialData != null)
                ShadersUtil.SetRimPow(playerEntity, materialData.rimPow);
            playerEntity.name = playerData.model;
            return playerEntity;
        }

        public static PlayerEntity CreatePlayerEntityAsUIElement(int playerInstanceId, Transform parent, bool canClick, bool canDrag)
        {
            return CreatePlayerEntityAsUIElement(Logic.Player.Model.PlayerProxy.instance.GetPlayerInfo(playerInstanceId), parent, canClick, canDrag);
        }

        public static PlayerEntity CreatePlayerEntityAsUIElement(Player.Model.PlayerInfo playerInfo, Transform parent, bool canClick, bool canDrag)
        {
            if (playerInfo == null || playerInfo.playerData == null) return null;
            PlayerEntity playerEntity = CreatePlayerEntityAsUIElement(playerInfo.playerData);
            if (playerEntity == null)
            {
                return null;
            }

            TransformUtil.SwitchLayer(playerEntity.transform, parent.gameObject.layer);
            playerEntity.transform.SetParent(parent, false);
            if (!playerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(playerEntity, playerInfo.heroData, playerInfo.advanceLevel, true, parent.gameObject.layer);
            playerEntity.bornEffect = true;
            playerEntity.transform.localPosition = Vector3.zero;
            playerEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
            playerEntity.transform.localScale = Vector3.one;

            PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerEntity);
            PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerEntity);
            PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, playerEntity);
            PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, playerEntity);

            #region pet
            PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
            PetEntity petEntity = CreatePetEntiy(petData.modelName);
            petEntity.transform.SetParent(parent, false);
            petEntity.transform.localScale = petData.scale;
            petEntity.transform.localEulerAngles = petData.homeRotation;
            LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
            if (!lockTransform)
                lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
            lockTransform.trans = playerEntity.rootNode.transform;
            //            lockTransform.delayRate = petData.speed;
            lockTransform.delayRate = 0;
            lockTransform.offset = petData.homeOffset;
            playerEntity.petEntity = petEntity;
            TransformUtil.SwitchLayer(petEntity.transform, parent.gameObject.layer);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(petEntity);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(petEntity, ShadersUtil.RIM_MAIN_COLOR);
            ShadersUtil.SetColor(petEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
            MaterialData materialData = MaterialData.GetMaterialDataByModelName(petData.modelName);
            if (materialData != null)
                ShadersUtil.SetRimPow(petEntity, materialData.rimPow);
            #endregion

            Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = playerEntity.gameObject.GetComponent<Logic.Model.View.ModelRotateAndAnim>();
            if (!modelRotateAndAnim)
                modelRotateAndAnim = playerEntity.gameObject.AddComponent<Logic.Model.View.ModelRotateAndAnim>();
            modelRotateAndAnim.canClick = canClick;
            modelRotateAndAnim.canDrag = canDrag;
            CapsuleCollider capsuleCollider = playerEntity.gameObject.GetComponent<CapsuleCollider>();
            if (!capsuleCollider)
                capsuleCollider = playerEntity.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2.5f;
            capsuleCollider.radius = 1.5f;
            capsuleCollider.center = new Vector3(0f, 1.2f, 0f);
            playerEntity.transform.tag = "Character";

            return playerEntity;
        }

        public static PlayerEntity CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(LuaTable playerInfoLuaTable, Transform parent, bool canClick, bool canDrag)
        {
            Logic.Player.Model.PlayerInfo playerInfo = new Logic.Player.Model.PlayerInfo(playerInfoLuaTable);
            return CreatePlayerEntityAsUIElement(playerInfo, parent, canClick, canDrag);
        }

        public static PlayerEntity CreatePlayerEntityAs3DUIElement(Player.Model.PlayerInfo playerInfo, Transform parent, bool canClick, bool canDrag)
        {
            PlayerEntity playerEntity = CreatePlayerEntityAsUIElement(playerInfo, parent, canClick, canDrag);
            if (!playerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(playerEntity, playerInfo.heroData, playerInfo.advanceLevel, true, (int)LayerType.UI3D);
            playerEntity.bornEffect = true;
            TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.UI3D);
            return playerEntity;
        }

        public static PlayerEntity CreatePlayerEntityAs3DUIElementByPlayerInfoLuaTable(LuaTable playerInfoLuaTable, Transform parent, bool canClick, bool canDrag)
        {
            Logic.Player.Model.PlayerInfo playerInfo = new Logic.Player.Model.PlayerInfo(playerInfoLuaTable);
            return CreatePlayerEntityAs3DUIElement(playerInfo, parent, canClick, canDrag);
        }

        public static EnemyEntity CreateEnemyEntity(Hero.Model.HeroInfo heroInfo)
        {
            string heroModelPath = ResPath.GetHeroModelPath(heroInfo.ModelName);
            //GameObject original = ResMgr.instance.Load<GameObject>(heroModelPath);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(heroInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(heroInfo.ModelName).Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(heroModelPath, heroInfo.ModelName, false);
                Transform trans = spawnPool.Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
            }
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", heroInfo.ModelName));
                return null;
            }
            //GameObject go = Instantiate(original) as GameObject;
            CharacterEntity character = go.GetComponent<CharacterEntity>();
            EnemyEntity enemy = null;
            if (character)
            {
                //if (character is PlayerEntity || character is HeroEntity)
                //{
                enemy = go.AddComponent<EnemyEntity>();
                enemy.anim = character.anim;
                enemy.rootNode = character.rootNode;
                UnityEngine.Object.Destroy(character);
                //}
                //else if (character is EnemyEntity)
                //    enemy = character as EnemyEntity;
                TransformUtil.SwitchLayer(enemy.transform, (int)LayerType.Fight);
                if (Game.GameConfig.assetBundle)
                    ShadersUtil.SetShader(enemy);
                if (!enemy.bornEffect)
                    EffectController.instance.PlayBornEffect(enemy, heroInfo.heroData, heroInfo.advanceLevel, false);
                enemy.bornEffect = true;
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                ShadersUtil.SetMainColor(enemy, ShadersUtil.MAIN_COLOR);
            }
            enemy.name = heroInfo.ModelName;
            return enemy;
        }

        public static PetEntity CreatePetEntiy(string modelName)
        {
            //GameObject original = ResMgr.instance.Load<GameObject>("character/pet/" + modelName);
            string petModelPath = ResPath.GetPetModelPath(modelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(modelName))
            {
                Transform trans = PoolController.instance.GetPool(modelName).Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
            }
            else
            {
                SpawnPool spawnPool = PoolController.instance.CreateCharacterPool(petModelPath, modelName, false);
                Transform trans = spawnPool.Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
            }
            if (go == null)
            {
                Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", modelName));
                return null;
            }
            //GameObject go = Instantiate(original) as GameObject;
            PetEntity petEntity = go.GetComponent<PetEntity>();
            petEntity.name = modelName;
            return petEntity;
        }

        #region create character asyn
        public static void CreateHeroEntity(Hero.Model.HeroInfo heroInfo, System.Action<HeroEntity> callbackAction, bool isUI = false)
        {
            string heroModelPath = ResPath.GetHeroModelPath(heroInfo.ModelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(heroInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(heroInfo.ModelName).Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    HeroEntity hero = SetHeroEntity(heroInfo, go, isUI);
                    hero.name = heroInfo.ModelName;
                    if (callbackAction != null)
                        callbackAction(hero);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(heroModelPath, heroInfo.ModelName, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", heroInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", heroInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                HeroEntity hero = SetHeroEntity(heroInfo, go, isUI);
                hero.name = heroInfo.ModelName;
                if (callbackAction != null)
                    callbackAction(hero);
            });
        }

        private static HeroEntity SetHeroEntity(Hero.Model.HeroInfo heroInfo, GameObject go, bool isUI)
        {
            HeroEntity hero = null;
            CharacterEntity character = go.GetComponent<CharacterEntity>();
            if (character)
            {
                //if (character is EnemyEntity)
                //{
                hero = go.AddComponent<HeroEntity>();
                hero.anim = character.anim;
                hero.rootNode = character.rootNode;
                UnityEngine.Object.Destroy(character);
                //}
                //else if (character is HeroEntity)
                //    hero = character as HeroEntity;
                TransformUtil.SwitchLayer(hero.transform, (int)LayerType.Fight);
                if (Game.GameConfig.assetBundle)
                    ShadersUtil.SetShader(hero);
                if (!hero.bornEffect)
                    EffectController.instance.PlayBornEffect(hero, heroInfo.heroData, heroInfo.advanceLevel, isUI);
                hero.bornEffect = true;
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
                ShadersUtil.SetShaderKeyword(hero, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                ShadersUtil.SetMainColor(hero, ShadersUtil.MAIN_COLOR);
            }
            return hero;
        }
        public static void CreateHeroEntityAsUIElementAsyn(int heroInstanceId, Transform parent, bool canClick, bool canDrag, System.Action<HeroEntity> callbackAction)
        {
            CreateHeroEntityAsUIElement(HeroProxy.instance.GetHeroInfo((uint)heroInstanceId), parent, canClick, canDrag, callbackAction);
        }
        public static void CreateHeroEntityAsUIElement(Hero.Model.HeroInfo heroInfo, Transform parent, bool canClick, bool canDrag, System.Action<HeroEntity> callbackAction)
        {
            CreateHeroEntity(heroInfo, (heroEntity) =>
            {
                if (heroEntity == null)
                {
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                if (!heroEntity.bornEffect)
                    EffectController.instance.PlayBornEffect(heroEntity, heroInfo.heroData, heroInfo.advanceLevel, true);
                heroEntity.bornEffect = true;
                ShadersUtil.SetShaderKeyword(heroEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
                ShadersUtil.SetShaderKeyword(heroEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
                ShadersUtil.SetMainColor(heroEntity, ShadersUtil.RIM_MAIN_COLOR);
                ShadersUtil.SetColor(heroEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
                MaterialData materialData = MaterialData.GetMaterialDataByModelName(heroInfo.ModelName);
                if (materialData != null)
                    ShadersUtil.SetRimPow(heroEntity, materialData.rimPow);

                TransformUtil.SwitchLayer(heroEntity.transform, parent.gameObject.layer);
                heroEntity.transform.SetParent(parent, false);
                heroEntity.transform.localPosition = Vector3.zero;
                heroEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
                heroEntity.transform.localScale = Vector3.one;

                Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = heroEntity.gameObject.GetComponent<Logic.Model.View.ModelRotateAndAnim>();
                if (!modelRotateAndAnim)
                    modelRotateAndAnim = heroEntity.gameObject.AddComponent<Logic.Model.View.ModelRotateAndAnim>();
                modelRotateAndAnim.canClick = canClick;
                modelRotateAndAnim.canDrag = canDrag;
                CapsuleCollider capsuleCollider = heroEntity.gameObject.GetComponent<CapsuleCollider>();
                if (!capsuleCollider)
                    capsuleCollider = heroEntity.gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 2.5f;
                capsuleCollider.radius = 1.5f;
                capsuleCollider.center = new Vector3(0f, 1.2f, 0f);
                heroEntity.transform.tag = "Character";
                if (callbackAction != null)
                    callbackAction(heroEntity);
            }, true);
        }

        public static void CreateHeroEntityAs3DUIElement(Hero.Model.HeroInfo heroInfo, Transform parent, bool canClick, bool canDrag, System.Action<HeroEntity> callbackAction)
        {
            CreateHeroEntityAsUIElement(heroInfo, parent, canClick, canDrag, (heroEntity) =>
             {
                 TransformUtil.SwitchLayer(heroEntity.transform, (int)LayerType.UI3D);
                 if (callbackAction != null)
                     callbackAction(heroEntity);
             });
        }

        public static void CreatePlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent, System.Action<PlayerEntity> callbackAction, bool isUI = false)
        {
            string playerModelPath = ResPath.GetPlayerModelPath(playerInfo.ModelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(playerInfo.ModelName).Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    PlayerEntity playerEntity = SetPlayerEntity(playerInfo, parent, go, isUI);
                    playerEntity.name = playerInfo.ModelName;
                    if (callbackAction != null)
                        callbackAction(playerEntity);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(playerModelPath, playerInfo.ModelName, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                PlayerEntity playerEntity = SetPlayerEntity(playerInfo, parent, go, isUI);
                playerEntity.name = playerInfo.ModelName;
                if (callbackAction != null)
                    callbackAction(playerEntity);
            });
        }

        private static PlayerEntity SetPlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent, GameObject go, bool isUI)
        {
            CharacterEntity characterEntity = go.GetComponent<CharacterEntity>();
            PlayerEntity playerEntity = go.AddComponent<PlayerEntity>();
            playerEntity.anim = characterEntity.anim;
            playerEntity.rootNode = characterEntity.rootNode;
            UnityEngine.Object.Destroy(characterEntity);
            playerEntity.transform.SetParent(parent, false);
            if (!playerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(playerEntity, playerInfo.heroData, playerInfo.advanceLevel, isUI);
            playerEntity.bornEffect = true;
            PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerEntity, (int)LayerType.Fight);
            PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerEntity);
            PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, playerEntity);
            PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, playerEntity);
            TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(playerEntity);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            ShadersUtil.SetMainColor(playerEntity, ShadersUtil.MAIN_COLOR);
            #region pet
            PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
            PetEntity petEntity = CreatePetEntiy(petData.modelName);
            petEntity.transform.SetParent(parent, false);
            petEntity.transform.localScale = petData.scale;
            petEntity.transform.localEulerAngles = petData.rotation;
            LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
            if (!lockTransform)
                lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
            lockTransform.trans = playerEntity.rootNode.transform;
            lockTransform.delayRate = petData.speed;
            lockTransform.offset = petData.offset;
            playerEntity.petEntity = petEntity;
            TransformUtil.SwitchLayer(petEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(petEntity);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(petEntity, ShadersUtil.MAIN_COLOR);
            #endregion
            return playerEntity;
        }

        public static void CreateEnemyPlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent, System.Action<EnemyPlayerEntity> callbackAction, bool isUI = false)
        {
            string playerModelPath = ResPath.GetPlayerModelPath(playerInfo.ModelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(playerInfo.ModelName).Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    EnemyPlayerEntity enemyPlayerEntity = SetEnemyPlayerEntity(playerInfo, parent, go, isUI);
                    enemyPlayerEntity.name = playerInfo.ModelName;
                    if (callbackAction != null)
                        callbackAction(enemyPlayerEntity);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(playerModelPath, playerInfo.ModelName, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(playerInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerInfo.playerData.Id, playerInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                EnemyPlayerEntity enemyPlayerEntity = SetEnemyPlayerEntity(playerInfo, parent, go, isUI);
                enemyPlayerEntity.name = playerInfo.ModelName;
                if (callbackAction != null)
                    callbackAction(enemyPlayerEntity);
            });
        }

        private static EnemyPlayerEntity SetEnemyPlayerEntity(Player.Model.PlayerInfo playerInfo, Transform parent, GameObject go, bool isUI)
        {
            CharacterEntity characterEntity = go.GetComponent<CharacterEntity>();
            EnemyPlayerEntity enemyPlayerEntity = go.AddComponent<EnemyPlayerEntity>();
            enemyPlayerEntity.anim = characterEntity.anim;
            enemyPlayerEntity.characterName = characterEntity.characterName;
            enemyPlayerEntity.rootNode = characterEntity.rootNode;
            UnityEngine.Object.Destroy(characterEntity);
            enemyPlayerEntity.transform.SetParent(parent, false);
            if (!enemyPlayerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(enemyPlayerEntity, playerInfo.heroData, playerInfo.advanceLevel, isUI);
            enemyPlayerEntity.bornEffect = true;
            PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, enemyPlayerEntity, (int)LayerType.Fight);
            PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, enemyPlayerEntity);
            PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, enemyPlayerEntity);
            PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, enemyPlayerEntity);
            TransformUtil.SwitchLayer(enemyPlayerEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(enemyPlayerEntity);
            ShadersUtil.SetShaderKeyword(enemyPlayerEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(enemyPlayerEntity, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
            ShadersUtil.SetMainColor(enemyPlayerEntity, ShadersUtil.MAIN_COLOR);
            #region pet
            PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
            PetEntity petEntity = CreatePetEntiy(petData.modelName);
            petEntity.transform.SetParent(parent, false);
            petEntity.transform.localScale = petData.scale;
            petEntity.transform.localEulerAngles = petData.rotation + new Vector3(0f, 90f, 0f);
            LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
            if (!lockTransform)
                lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
            lockTransform.trans = enemyPlayerEntity.rootNode.transform;
            lockTransform.delayRate = petData.speed;
            lockTransform.offset = petData.offset;
            lockTransform.offset.x *= -1;
            enemyPlayerEntity.petEntity = petEntity;
            TransformUtil.SwitchLayer(petEntity.transform, (int)LayerType.Fight);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(petEntity);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(petEntity, ShadersUtil.MAIN_COLOR);
            #endregion
            return enemyPlayerEntity;
        }

        public static void CreatePlayerEntityAsUIElement(Player.Model.PlayerData playerData, System.Action<PlayerEntity> callbackAction)
        {
            GameObject go = null;
            if (PoolController.instance.ContainsPool(playerData.model))
            {
                Transform trans = PoolController.instance.GetPool(playerData.model).Spawn(playerData.model);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    PlayerEntity playerEntity = SetPlayerEntity(playerData, go, true);
                    playerEntity.name = playerData.model;
                    if (callbackAction != null)
                        callbackAction(playerEntity);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(playerData.PlayerModelPath, playerData.model, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerData.Id, playerData.model));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(playerData.model);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->PlayerModelName:{0}, PlayerModelName:{1}", playerData.Id, playerData.model));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                PlayerEntity playerEntity = SetPlayerEntity(playerData, go, true);
                playerEntity.name = playerData.model;
                if (callbackAction != null)
                    callbackAction(playerEntity);
            });
        }

        private static PlayerEntity SetPlayerEntity(Player.Model.PlayerData playerData, GameObject go, bool isUI)
        {
            CharacterEntity characterEntity = go.GetComponent<CharacterEntity>();
            PlayerEntity playerEntity = go.AddComponent<PlayerEntity>();
            playerEntity.anim = characterEntity.anim;
            playerEntity.characterName = characterEntity.characterName;
            playerEntity.rootNode = characterEntity.rootNode;
            UnityEngine.Object.Destroy(characterEntity);
            TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.UI);
            if (Game.GameConfig.assetBundle)
                ShadersUtil.SetShader(playerEntity);
            if (!playerEntity.bornEffect)
                EffectController.instance.PlayBornEffect(playerEntity, playerData.heroData, (int)playerData.heroData.starMin, isUI);
            playerEntity.bornEffect = true;
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
            ShadersUtil.SetShaderKeyword(playerEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
            ShadersUtil.SetMainColor(playerEntity, ShadersUtil.RIM_MAIN_COLOR);
            ShadersUtil.SetColor(playerEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
            MaterialData materialData = MaterialData.GetMaterialDataByModelName(playerData.model);
            if (materialData != null)
                ShadersUtil.SetRimPow(playerEntity, materialData.rimPow);
            return playerEntity;
        }
        public static void CreatePlayerEntityAsUIElementAsyn(int playerInstanceId, Transform parent, bool canClick, bool canDrag, System.Action<PlayerEntity> callbackAction, System.Action<PetEntity> petEntityAction = null)
        {
            CreatePlayerEntityAsUIElement(Logic.Player.Model.PlayerProxy.instance.GetPlayerInfo(playerInstanceId), parent, canClick, canDrag, callbackAction, petEntityAction);
        }
        public static void CreatePlayerEntityAsUIElement(Player.Model.PlayerInfo playerInfo, Transform parent, bool canClick, bool canDrag, System.Action<PlayerEntity> callbackAction, System.Action<PetEntity> petEntityAction = null)
        {
            if (playerInfo == null || playerInfo.playerData == null)
            {
                if (callbackAction != null)
                    callbackAction(null);
                if (petEntityAction != null)
                    petEntityAction(null);
                return;
            };
            CreatePlayerEntityAsUIElement(playerInfo.playerData, (playerEntity) =>
            {
                if (playerEntity == null)
                {
                    if (callbackAction != null)
                        callbackAction(null);
                    if (petEntityAction != null)
                        petEntityAction(null);
                    return;
                }

                TransformUtil.SwitchLayer(playerEntity.transform, parent.gameObject.layer);
                playerEntity.transform.SetParent(parent, false);
                playerEntity.transform.localPosition = Vector3.zero;
                playerEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
                playerEntity.transform.localScale = Vector3.one;

                PlayerEntityUtil.ChangeHair(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerEntity);
                PlayerEntityUtil.ChangeHairColor(playerInfo.playerData.Id, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerEntity);
                PlayerEntityUtil.ChangeFace(playerInfo.playerData.Id, playerInfo.faceIndex, playerEntity);
                PlayerEntityUtil.ChangeSkin(playerInfo.playerData.Id, playerInfo.skinIndex, playerEntity);

                #region pet
                PetData petData = PetData.GetPetDataByID(playerInfo.playerData.pet_id);
                PetEntity petEntity = CreatePetEntiy(petData.modelName);
                petEntity.transform.SetParent(parent, false);
                petEntity.transform.localScale = petData.scale;
                petEntity.transform.localEulerAngles = petData.homeRotation;
                LockTransform lockTransform = petEntity.gameObject.GetComponent<LockTransform>();
                if (!lockTransform)
                    lockTransform = petEntity.gameObject.AddComponent<LockTransform>();
                lockTransform.trans = playerEntity.rootNode.transform;
                //            lockTransform.delayRate = petData.speed;
                lockTransform.delayRate = 0;
                lockTransform.offset = petData.homeOffset;
                playerEntity.petEntity = petEntity;
                TransformUtil.SwitchLayer(petEntity.transform, parent.gameObject.layer);
                if (Game.GameConfig.assetBundle)
                    ShadersUtil.SetShader(petEntity);
                ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.RIMLIGHT_ON, ShadersUtil.RIMLIGHT_OFF);
                ShadersUtil.SetShaderKeyword(petEntity, ShadersUtil.CLIP_POSITION_OFF, ShadersUtil.CLIP_POSITION_ON);
                ShadersUtil.SetMainColor(petEntity, ShadersUtil.RIM_MAIN_COLOR);
                ShadersUtil.SetColor(petEntity, ShadersUtil.RIM_COLOR_ID, ShadersUtil.RIM_COLOR);
                MaterialData materialData = MaterialData.GetMaterialDataByModelName(petData.modelName);
                if (materialData != null)
                    ShadersUtil.SetRimPow(petEntity, materialData.rimPow);
                if (petEntityAction != null)
                    petEntityAction(petEntity);
                #endregion

                Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = playerEntity.gameObject.GetComponent<Logic.Model.View.ModelRotateAndAnim>();
                if (!modelRotateAndAnim)
                    modelRotateAndAnim = playerEntity.gameObject.AddComponent<Logic.Model.View.ModelRotateAndAnim>();
                modelRotateAndAnim.canClick = canClick;
                modelRotateAndAnim.canDrag = canDrag;
                CapsuleCollider capsuleCollider = playerEntity.gameObject.GetComponent<CapsuleCollider>();
                if (!capsuleCollider)
                    capsuleCollider = playerEntity.gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 2.5f;
                capsuleCollider.radius = 1.5f;
                capsuleCollider.center = new Vector3(0f, 1.2f, 0f);
                playerEntity.transform.tag = "Character";
                if (callbackAction != null)
                    callbackAction(playerEntity);
            });
        }

        public static void CreatePlayerEntityAs3DUIElement(Player.Model.PlayerInfo playerInfo, Transform parent, bool canClick, bool canDrag, System.Action<PlayerEntity> callbackAction)
        {
            CreatePlayerEntityAsUIElement(playerInfo, parent, canClick, canDrag, (playerEntity) =>
            {
                TransformUtil.SwitchLayer(playerEntity.transform, (int)LayerType.UI3D);
                if (callbackAction != null)
                    callbackAction(playerEntity);
            });
        }

        public static void CreateEnemyEntity(Hero.Model.HeroInfo heroInfo, System.Action<EnemyEntity> callbackAction, bool isUI = false)
        {
            string heroModelPath = ResPath.GetHeroModelPath(heroInfo.ModelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(heroInfo.ModelName))
            {
                Transform trans = PoolController.instance.GetPool(heroInfo.ModelName).Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    EnemyEntity enemy = SetEnemyEntity(heroInfo, go, isUI);
                    enemy.name = heroInfo.ModelName;
                    if (callbackAction != null)
                        callbackAction(enemy);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(heroModelPath, heroInfo.ModelName, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", heroInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(heroInfo.ModelName);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", heroInfo.ModelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                EnemyEntity enemy = SetEnemyEntity(heroInfo, go, isUI);
                enemy.name = heroInfo.ModelName;
                if (callbackAction != null)
                    callbackAction(enemy);
            });
        }

        private static EnemyEntity SetEnemyEntity(Hero.Model.HeroInfo heroInfo, GameObject go, bool isUI)
        {
            CharacterEntity character = go.GetComponent<CharacterEntity>();
            EnemyEntity enemy = null;
            if (character)
            {
                //if (character is PlayerEntity || character is HeroEntity)
                //{
                enemy = go.AddComponent<EnemyEntity>();
                enemy.anim = character.anim;
                enemy.rootNode = character.rootNode;
                UnityEngine.Object.Destroy(character);
                //}
                //else if (character is EnemyEntity)
                //    enemy = character as EnemyEntity;
                TransformUtil.SwitchLayer(enemy.transform, (int)LayerType.Fight);
                if (Game.GameConfig.assetBundle)
                    ShadersUtil.SetShader(enemy);
                if (!enemy.bornEffect)
                    EffectController.instance.PlayBornEffect(enemy, heroInfo.heroData, heroInfo.advanceLevel, isUI);
                enemy.bornEffect = true;
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
                ShadersUtil.SetShaderKeyword(enemy, ShadersUtil.CLIP_POSITION_ON, ShadersUtil.CLIP_POSITION_OFF);
                ShadersUtil.SetMainColor(enemy, ShadersUtil.MAIN_COLOR);
            }
            return enemy;
        }

        public static void CreatePetEntiy(string modelName, System.Action<PetEntity> callbackAction)
        {
            string petModelPath = ResPath.GetPetModelPath(modelName);
            GameObject go = null;
            if (PoolController.instance.ContainsPool(modelName))
            {
                Transform trans = PoolController.instance.GetPool(modelName).Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
                if (go)
                {
                    PetEntity petEntity = go.GetComponent<PetEntity>();
                    petEntity.name = modelName;
                    if (callbackAction != null)
                        callbackAction(petEntity);
                    return;
                }
            }
            PoolController.instance.CreateCharacterPool(petModelPath, modelName, false, (spawnPool) =>
            {
                if (!spawnPool)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", modelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                Transform trans = spawnPool.Spawn(modelName);
                if (trans)
                    go = trans.gameObject;
                if (go == null)
                {
                    Debugger.LogError(string.Format("Can't find model----->ModelName:{0}", modelName));
                    if (callbackAction != null)
                        callbackAction(null);
                    return;
                }
                PetEntity petEntity = go.GetComponent<PetEntity>();
                petEntity.name = modelName;
                if (callbackAction != null)
                    callbackAction(petEntity);
            });
        }

        #endregion
        #endregion
        #region 阴影
        public static Vector2 GetShadowSize(ShadowType shadowType)
        {
            Vector3 size = Vector3.one;
            switch (shadowType)
            {
                case ShadowType.One:
                    size = new Vector2(1.5f, 1.5f);
                    break;
                case ShadowType.Two:
                    size = new Vector3(2f, 2f);
                    break;
                case ShadowType.Three:
                    size = new Vector3(3f, 3f);
                    break;
                case ShadowType.Four:
                    size = new Vector3(5f, 5f);
                    break;
                default:
                    size = new Vector3(1.5f, 1.5f);
                    break;
            }
            return size;
        }
        #endregion
        #region character entity
        [HideInInspector]
        public HPBarView hpBarView;
        public Animator anim;
        public string characterName;
        private Transform _trans;
        protected Status _status = Status.Idle;
        public System.Action OnHPChange;
        public GameObject rootNode;
        public bool isPlayer = false;
        public bool isRole = false;
        public bool moveBroken = false;
        public bool bornEffect;
        public PositionData positionData;
        private bool _addedHaloBuff;
        private float _height;
        private Dictionary<uint, uint> _skillCountDic = new Dictionary<uint, uint>();
        private uint _currentSkillId;
        private const float ANIMATION_DELAY = 1f;
        private float _lastSkillOverTime;
        protected virtual void Awake()
        {
            _trans = this.transform;
            status = Status.Idle;
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public bool addedHaloBuff
        {
            get
            {
                return _addedHaloBuff;
            }
            set
            {
                _addedHaloBuff = value;
            }
        }

        private float _lastTickTime;
        public float lastTickTime
        {
            set
            {
                _lastTickTime = value;
            }
            get
            {
                return _lastTickTime;
            }
        }

        private bool _tickCD = false;
        public bool tickCD
        {
            get
            {
                return _tickCD;
            }
            set
            {
                if (_tickCD && value) return;
                _tickCD = value;
                //if (value)
                //{
                //    float interval = Time.time - _lastTickTime;
                //    _lastHitTime += interval;
                //    _lastSkill1CDTime += interval;
                //    _lastSkill2CDTime += interval;
                //    _lastSkill1CDTime = _lastSkill2CDTime = TimeController.instance.fightSkillTime;
                //}
            }
        }

        #region 属性
        #region 模型数据
        public virtual uint positionId
        {
            set
            {
                _characterInfo.positionId = value;
                positionData = Logic.Position.Model.PositionData.GetPostionDataById(value);
                _original = positionData.position;
                pos = _original;
            }
            get
            {
                return _characterInfo.positionId;
            }
        }

        private Vector3 _original;
        public Vector3 pos
        {
            get
            {
                if (_characterInfo == null || !_trans)
                    return Vector3.zero;
                _characterInfo.pos = _trans.localPosition;//同步一次坐标？？？
                return _characterInfo.pos;
            }
            set
            {
                _trans.localPosition = value;
                _characterInfo.pos = value;
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                if (_characterInfo == null)
                    return Vector3.zero;
                _characterInfo.eulerAngles = _trans.eulerAngles;
                return _characterInfo.eulerAngles;
            }
            set
            {
                _trans.eulerAngles = value;
                _characterInfo.eulerAngles = value;
            }
        }

        public Vector3 scale
        {
            get
            {
                if (_characterInfo == null)
                    return Vector3.zero;
                _characterInfo.scale = _trans.localScale;
                return _characterInfo.scale;
            }
            set
            {
                _trans.localScale = value;
                _characterInfo.scale = value;
            }
        }

        public virtual float height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        #endregion
        /// <summary>
        /// 角色基本数据
        /// </summary>
        private CharacterBaseInfo _characterInfo;
        public CharacterBaseInfo characterInfo
        {
            get { return _characterInfo; }
            set
            {
                _characterInfo = value;
            }
        }

        public virtual Status status
        {
            get
            {
                return _status;
            }
            protected set
            {
                _status = value;
                switch (value)
                {
                    case Status.Idle:
                        _lastSkillOverTime = Time.realtimeSinceStartup;
                        Action.Controller.ActionController.instance.SetAnimBoolean(this, AnimatorUtil.DEAD, false);
                        break;
                    case Status.Skill:
                        break;
                    case Status.GetHit:
                        break;
                    case Status.Dead:
                        Action.Controller.ActionController.instance.SetAnimBoolean(this, AnimatorUtil.DEAD, true);
                        break;
                }
            }
        }

        #region buff && debuff
        private Dictionary<BuffType, List<BuffInfo>> _buffDic = new Dictionary<BuffType, List<BuffInfo>>();
        private Dictionary<BuffType, List<string>> _buffEffectDic = new Dictionary<BuffType, List<string>>();
        private Dictionary<string, float> _buffIconDic = new Dictionary<string, float>();
        private List<string> _buffIcons = new List<string>();
        //private List<BuffType> _immuneBuffs;
        public Dictionary<BuffType, List<BuffInfo>> buffDic
        {
            get
            {
                return _buffDic;
            }
            protected set
            {
                _buffDic = value;
            }
        }

        [NoToLua]
        public Dictionary<BuffType, List<string>> buffEffectDic
        {
            get
            {
                return _buffEffectDic;
            }
        }

#if UNITY_EDITOR
        [NoToLua]
        public Dictionary<string, float> buffIconDic
        {
            get
            {
                return _buffIconDic;
            }
        }
#endif

        public List<string> GetBuffIcons()
        {
            _buffIcons.Clear();
            foreach (var kvp in _buffIconDic)
            {
                if (kvp.Value >= TimeController.instance.fightSkillTime)
                    _buffIcons.Add(kvp.Key);
            }
            return _buffIcons;
        }

        public void AddBuffIcon(string path, float time)
        {
            //if (!isPlayer)
            //    return;
            float end = TimeController.instance.fightSkillTime + time;
            AddBuffIconEndTime(path, end);
        }

        public void AddBuffIconEndTime(string path, float end)
        {
            //if (!isPlayer)
            //    return;
            if (_buffIconDic.ContainsKey(path))
            {
                float oldEnd = _buffIconDic[path];
                if (end > oldEnd)
                    _buffIconDic[path] = end;
            }
            else
                _buffIconDic.Add(path, end);
        }

        public void RemoveBuffIcon(string path)
        {
            //if (!isPlayer)
            //    return;
            if (_buffIconDic.ContainsKey(path))
                _buffIconDic.Remove(path);
        }

        public void ClearBuff()
        {
            buffDic.Clear();

            Dictionary<BuffType, List<string>>.Enumerator e = buffEffectDic.GetEnumerator();
            while (e.MoveNext())
            {
                for (int i = 0, count = e.Current.Value.Count; i < count; i++)
                {
                    EffectController.instance.RemoveEffectByName(e.Current.Value[i]);
                }
            }
            e.Dispose();
            buffEffectDic.Clear();
            if (_buffIconDic != null)
                _buffIconDic.Clear();
            if (_buffIcons != null)
                _buffIcons.Clear();
        }

        public void PlayBuffEffect(MechanicsData mechanicsData, BuffType buffType)
        {
            if (!Logic.Game.GameSetting.instance.effectable) return;
            for (int i = 0, count = mechanicsData.effectIds.Length; i < count; i++)
            {
                EffectInfo effectInfo = new EffectInfo(mechanicsData.effectIds[i]);
                if (effectInfo.effectData == null) return;
                //Debugger.Log(effectInfo.effectData.effectType);
                effectInfo.character = this;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        effectInfo.pos = Logic.Position.Model.PositionData.GetPostionDataById(this.positionId).position + effectInfo.effectData.offset;
                        effectInfo.target = this;
                        break;
                    case EffectType.LockTarget:
                        effectInfo.target = this;
                        break;
                    case EffectType.ChangeColor:
                        effectInfo.target = this;
                        break;
                    case EffectType.LockPart:
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, this.transform);
                        effectInfo.target = this;
                        break;
                }
                effectInfo.delay = effectInfo.effectData.delay;
                string effectName = EffectController.instance.PlayContinuousEffect(effectInfo);
                if (!_buffEffectDic.ContainsKey(buffType))
                    _buffEffectDic.Add(buffType, new List<string>());
                _buffEffectDic[buffType].Add(effectName);
            }
        }

        protected virtual void BuffStart(MechanicsData mechanicsData, BuffInfo buffInfo)
        {
            if (_buffEffectDic.ContainsKey(buffInfo.buffType)) return;
            if (mechanicsData != null)
                PlayBuffEffect(mechanicsData, buffInfo.buffType);
            #region effect id by buff type and play motion
            uint effectId = CharacterUtil.GetEffectIdByBuffType(buffInfo);
            switch (buffInfo.buffType)
            {
                case BuffType.Swimmy:
                    //effectId = EffectController.SWIMMY_EFFECT_ID;
                    AnimatorUtil.SetBool(anim, AnimatorUtil.SWIMMY, true);
                    break;
                case BuffType.Invincible:
                    break;
                case BuffType.Silence:
                    break;
                case BuffType.Blind:
                    break;
                case BuffType.Frozen:
                    if (anim)
                        anim.speed = 0f;
                    break;
                case BuffType.Sleep:
                    if (anim)
                        anim.speed = 0f;
                    break;
                case BuffType.Landification:
                    if (anim)
                        anim.speed = 0f;
                    break;
                case BuffType.Tieup:
                    if (anim)
                        anim.speed = 0f;
                    break;
                case BuffType.Rebound:
                    break;
                case BuffType.DamageImmuneTime:
                    break;
                case BuffType.DamageImmuneCount:
                    break;
                case BuffType.Immune:
                    break;
                case BuffType.Poisoning:
                    //if (mechanicsData.effectIds.Length > 0)
                    //    effectId = mechanicsData.effectIds[0];
                    break;
                case BuffType.Ignite:
                    break;
                case BuffType.Bleed:
                    break;
                case BuffType.Treat:
                    //if (mechanicsData.effectIds.Length > 0)
                    //    effectId = mechanicsData.effectIds[0];
                    break;
                case BuffType.TreatPercent:
                    break;
                case BuffType.Speed:
                    break;
                case BuffType.Shield:
                    //effectId = EffectController.SHIELD_EFFECT_ID;
                    break;
                case BuffType.Drain:
                    break;
                case BuffType.PhysicsDefense:
                    break;
                case BuffType.MagicDefense:
                    break;
                case BuffType.PhysicsAttack:
                    break;
                case BuffType.MagicAttack:
                    break;
                case BuffType.HPLimit:
                    break;
                case BuffType.Hit:
                    break;
                case BuffType.Dodge:
                    break;
                case BuffType.Crit:
                    break;
                case BuffType.AntiCrit:
                    break;
                case BuffType.Block:
                    break;
                case BuffType.AntiBlock:
                    break;
                case BuffType.CounterAtk:
                    break;
                case BuffType.CritHurtAdd:
                    break;
                case BuffType.CritHurtDec:
                    break;
                case BuffType.Armor:
                    break;
                case BuffType.DamageDec:
                    break;
                case BuffType.DamageAdd:
                    break;
                case BuffType.Weakness:
                    break;
                case BuffType.TreatAdd:
                    break;
                case BuffType.ImmunePhysicsAttack:
                    break;
                case BuffType.ImmuneMagicAttack:
                    break;
                case BuffType.Tag:
                    break;
                case BuffType.GeneralSkillHit:
                    break;
                case BuffType.GeneralSkillCrit:
                    break;
                case BuffType.GeneralSkillPhysicsAttack:
                    break;
                case BuffType.GeneralSkillMagicAttack:
                    break;
                case BuffType.TargetSkillPhysicsAttack:
                    break;
                case BuffType.TargetSkillMagicAttack:
                    break;
                case BuffType.AccumulatorTag:
                    break;
            }
            #endregion
            EffectInfo effectInfo = new EffectInfo(effectId);
            if (effectInfo.effectData == null) return;
            effectInfo.character = this;
            switch (effectInfo.effectData.effectType)
            {
                case EffectType.LockTarget:
                    effectInfo.target = this;
                    break;
                case EffectType.LockPart:
                    effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, this.transform);
                    effectInfo.target = this;
                    break;
            }
            effectInfo.delay = effectInfo.effectData.delay;
            string effectName = Effect.Controller.EffectController.instance.PlayContinuousEffect(effectInfo);
            //Debugger.Log("add buff type:" + buffType.ToString() + "   name:" + effectName);
            if (buffInfo.buffType != BuffType.Immune)
            {
                if (!_buffEffectDic.ContainsKey(buffInfo.buffType))
                    _buffEffectDic.Add(buffInfo.buffType, new List<string>());
                _buffEffectDic[buffInfo.buffType].Add(effectName);
            }
        }

        protected virtual void RemoveBuffEffect(BuffType buffType)
        {
            switch (buffType)
            {
                case BuffType.Swimmy:
                    AnimatorUtil.SetBool(anim, AnimatorUtil.SWIMMY, false);
                    break;
                case BuffType.Frozen:
                    if (anim)
                        anim.speed = 1f;
                    break;
                case BuffType.Sleep:
                    if (anim)
                        anim.speed = 1f;
                    break;
                case BuffType.Landification:
                    if (anim)
                        anim.speed = 1f;
                    break;
                case BuffType.Tieup:
                    if (anim)
                        anim.speed = 1f;
                    break;
            }
            List<string> effectNames;
            if (_buffEffectDic.TryGetValue(buffType, out effectNames))
            {
                //Debugger.Log("remove buff type:" + buffType.ToString() + "   name:" + effectName);
                for (int i = 0, count = effectNames.Count; i < count; i++)
                {
                    EffectController.instance.RemoveEffectByName(effectNames[i]);
                }
                _buffEffectDic.Remove(buffType);
            }
        }

        //public virtual BuffInfo AddBuff(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, BuffType buffType, SkillLevelBuffAddType skillLevelBuffAddType, BuffAddType buffAddType, float time, float value, uint level, int judgeType)
        //{
        //    if (PreAddBuff(mechanicsData, buffType))
        //    {
        //        List<BuffInfo> list = _buffDic[buffType];
        //        BuffInfo buffInfo = new BuffInfo(character, target, skillInfo, mechanicsData, buffType, skillLevelBuffAddType, buffAddType, time, value, level, judgeType);
        //        list.Add(buffInfo);
        //        PostAddBuff(buffInfo);
        //        return buffInfo;
        //    }
        //    return null;
        //}

        public virtual BuffInfo AddBuff(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, BuffType buffType, SkillLevelBuffAddType skillLevelBuffAddType, BuffAddType buffAddType, float time, float value, uint level, int judgeType, bool forever = false, bool disperseable = true)
        {
            if (PreAddBuff(mechanicsData, buffType))
            {
                List<BuffInfo> list = _buffDic[buffType];
                BuffInfo buffInfo = new BuffInfo(character, target, skillInfo, mechanicsData, buffType, skillLevelBuffAddType, buffAddType, time, value, level, judgeType);
                buffInfo.forever = forever;
                buffInfo.disperseable = disperseable;
                list.Add(buffInfo);
                if (Logic.Game.GameSetting.instance.effectable)
                    BuffStart(mechanicsData, buffInfo);
                string key = string.Format("buff.{0}.{1}", buffInfo.buffType.ToString(), (buffInfo.kindness ? 1 : 0));
                string tips = Common.Localization.Localization.Get(key);
                if (key != tips)
                    Logic.UI.Buff.Controller.BuffTipsController.instance.ShowBuffTipsView(this, tips);
#if UNITY_EDITOR
                else
                    Debugger.LogError("key:" + key);
#endif
                PostAddBuff(buffInfo);
                return buffInfo;
            }
            return null;
        }

        public virtual BuffInfo AddBuff(CharacterEntity character, CharacterEntity target, SkillInfo skillInfo, MechanicsData mechanicsData, BuffType buffType, SkillLevelBuffAddType skillLevelBuffAddType, BuffAddType buffAddType, int count, uint targetSkill, float value, uint level, int judgeType, bool forever = false, bool disperseable = true)
        {
            if (PreAddBuff(mechanicsData, buffType))
            {
                List<BuffInfo> list = _buffDic[buffType];
                BuffInfo buffInfo = new BuffInfo(character, target, skillInfo, mechanicsData, buffType, skillLevelBuffAddType, buffAddType, count, targetSkill, value, level, judgeType);
                buffInfo.forever = forever;
                buffInfo.disperseable = disperseable;
                list.Add(buffInfo);
                if (Logic.Game.GameSetting.instance.effectable)
                    BuffStart(mechanicsData, buffInfo);
                string key = string.Format("buff.{0}.{1}", buffInfo.buffType.ToString(), (buffInfo.kindness ? 1 : 0));
                string tips = Common.Localization.Localization.Get(key);
                if (key != tips)
                    Logic.UI.Buff.Controller.BuffTipsController.instance.ShowBuffTipsView(this, tips);
#if UNITY_EDITOR
                else
                    Debugger.LogError("key:" + key);
#endif
                PostAddBuff(buffInfo);
                return buffInfo;
            }
            return null;
        }

        public virtual BuffInfo AddBuff(CharacterEntity target, MechanicsData mechanicsData, BuffType buffType, float time, float intervalTime, int count, float value)
        {
            if (!_buffDic.ContainsKey(buffType))
                _buffDic.Add(buffType, new List<BuffInfo>());
            List<BuffInfo> list = _buffDic[buffType];
            BuffInfo buffInfo = new BuffInfo(target, mechanicsData, buffType, time, intervalTime, count, value);
            buffInfo.forever = false;
            buffInfo.disperseable = true;
            list.Add(buffInfo);
            if (Logic.Game.GameSetting.instance.effectable)
                BuffStart(mechanicsData, buffInfo);
            string key = string.Format("buff.{0}.{1}", buffInfo.buffType.ToString(), (buffInfo.kindness ? 1 : 0));
            string tips = Common.Localization.Localization.Get(key);
            if (key != tips)
                Logic.UI.Buff.Controller.BuffTipsController.instance.ShowBuffTipsView(this, tips);
#if UNITY_EDITOR
            else
                Debugger.LogError("key:" + key);
#endif
            PostAddBuff(buffInfo);
            return buffInfo;
        }

        private bool PreAddBuff(MechanicsData mechanicsData, BuffType buffType)
        {
            List<BuffInfo> immuneBuffs = GetBuffs(BuffType.Immune);
            if (immuneBuffs != null && immuneBuffs.Count > 0)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_IMMUNE_BUFFS);
                if (func != null)
                {
                    for (int j = 0; j < immuneBuffs.Count; j++)
                    {
                        BuffInfo buffInfo = immuneBuffs[j];
                        CharacterEntity adder = buffInfo.character;
                        if (adder == null) continue;
                        object[] rs = func.Call(adder);
                        if (rs != null)
                        {
                            bool exist = true;
                            for (int i = 0, count = rs.Length; i < count; i++)
                            {
                                BuffType bt = (BuffType)rs[i];
                                if (bt == buffType)
                                    exist = false;
                            }
                            if (!exist)
                                return exist;
                        }
                    }
                }
            }
            if (_characterInfo != null && _characterInfo.passiveIdDic.Count > 0)
            {
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ADD_BUFF, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(this, buffType, kvp.Value);
                        int r = 0;
                        int.TryParse(rs[0].ToString(), out r);
                        if (r <= 0)
                            return false;
                    }
                }
            }
            if (!_buffDic.ContainsKey(buffType))
                _buffDic.Add(buffType, new List<BuffInfo>());
            if (mechanicsData == null)
            {
                List<BuffInfo> buffs = GetBuffs(buffType);
                if (buffs != null)
                {
                    for (int i = 0, iCount = buffs.Count; i < iCount; i++)
                    {
                        BuffInfo buffInfo = buffs[i];
                        if (buffInfo.mechanics == null)//效果为null时，清除同类型buff                    
                        {
                            buffInfo.Set2OutOfTime();
                            buffInfo.forever = false;//永久性buff过期
                        }
                    }
                }
            }
            else
            {
                List<BuffInfo> mechanicsBuffs = GetBuffs(buffType, mechanicsData.mechanicsId);
                if (mechanicsBuffs.Count >= mechanicsData.maxLayer && mechanicsBuffs.Count > 0)
                {
                    BuffInfo buffInfo = mechanicsBuffs.First();
                    buffInfo.Set2OutOfTime();
                    buffInfo.forever = false;
                }
            }
            return true;
        }

        private void PostAddBuff(BuffInfo buffInfo)
        {
            if (_characterInfo.passiveIdDic.Count > 0)
            {
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ADDED_BUFF, kvp.Key);
                    if (func != null)
                        func.Call(this, buffInfo, kvp.Value);
                }
            }
            //if (isPlayer)
            //{
            if (buffInfo.buffType != BuffType.Immune)
            {
                string path = Fight.Model.FightProxy.instance.GetIconPath(buffInfo.buffType, buffInfo.kindness);
                if (!string.IsNullOrEmpty(path))
                {
                    if (buffInfo.count > 0)
                        AddBuffIconEndTime(path, 999);
                    else
                        AddBuffIconEndTime(path, buffInfo.forever ? 999 : buffInfo.time);
                }
            }
            else
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.GET_IMMUNE_BUFF_ICON);
                if (func != null)
                {
                    object[] rs = func.Call(this);
                    if (rs != null && rs.Length > 0)
                    {
                        string path = rs[0].ToString();
                        AddBuffIconEndTime(path, buffInfo.forever ? 999 : buffInfo.time);
                    }
                }
            }
            //}
        }

        public void RemoveBuff(BuffType buffType)
        {
            List<BuffInfo> buffs = GetBuffs(buffType);
            if (buffs != null)
            {
                buffs.Clear();
                RemoveBuffEffect(buffType);
            }
        }

        public bool ExistBuff(BuffType buffType, bool kindness)
        {
            bool result = false;
            List<BuffInfo> list = GetBuffs(buffType);
            if (list != null)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    if (list[i].kindness == kindness)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public bool ExistBuff(BuffType buffType)
        {
            if (_buffDic == null) return false;
            bool result = false;
            List<BuffInfo> list = null;
            if (_buffDic.TryGetValue(buffType, out list) && list.Count > 0)
            {
                List<BuffInfo> stillBuffs = new List<BuffInfo>();
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    BuffInfo buffInfo = list[i];
                    #region buff type
                    switch (buffInfo.buffType)
                    {
                        case BuffType.Swimmy:
                        case BuffType.Invincible:
                        case BuffType.Silence:
                        case BuffType.Blind:
                        case BuffType.Float:
                        case BuffType.Tumble:
                        case BuffType.Poisoning:
                        case BuffType.Treat:
                        case BuffType.Speed:
                        case BuffType.Shield:
                        case BuffType.Drain:
                        case BuffType.PhysicsDefense:
                        case BuffType.MagicDefense:
                        case BuffType.PhysicsAttack:
                        case BuffType.MagicAttack:
                        case BuffType.HPLimit:
                        case BuffType.Hit:
                        case BuffType.Dodge:
                        case BuffType.Crit:
                        case BuffType.AntiCrit:
                        case BuffType.Block:
                        case BuffType.AntiBlock:
                        case BuffType.CounterAtk:
                        case BuffType.CritHurtAdd:
                        case BuffType.CritHurtDec:
                        case BuffType.Armor:
                        case BuffType.DamageDec:
                        case BuffType.DamageAdd:
                        case BuffType.Frozen:
                        case BuffType.TreatPercent:
                        case BuffType.Ignite:
                        case BuffType.Bleed:
                        case BuffType.Sleep:
                        case BuffType.Landification:
                        case BuffType.Tieup:
                        case BuffType.Immune:
                        case BuffType.Rebound:
                        case BuffType.DamageImmuneTime:
                        case BuffType.TreatAdd:
                        case BuffType.Weakness:
                        case BuffType.ImmunePhysicsAttack:
                        case BuffType.ImmuneMagicAttack:
                        case BuffType.Tag:
                        case BuffType.AccumulatorTag:
                            if (!buffInfo.outOfTime || buffInfo.forever)//更新buff
                            {
                                result = true;
                                stillBuffs.Add(buffInfo);
                            }
                            break;
                        case BuffType.GeneralSkillPhysicsAttack:
                        case BuffType.GeneralSkillMagicAttack:
                        case BuffType.TargetSkillPhysicsAttack:
                        case BuffType.TargetSkillMagicAttack:
                        case BuffType.DamageImmuneCount:
                        case BuffType.GeneralSkillHit:
                        case BuffType.GeneralSkillCrit:
                            if (buffInfo.count > 0)
                            {
                                result = true;
                                stillBuffs.Add(buffInfo);
                            }
                            break;
                    }
                    #endregion
                }
                list.Clear();
                if (stillBuffs.Count == 0)
                {
                    RemoveBuffEffect(buffType);
                }
                _buffDic[buffType] = stillBuffs;
            }
            return result;
        }

        public List<BuffInfo> GetBuffs(BuffType buffType, uint mechanicsId)
        {
            if (_buffDic == null) return null;
            List<BuffInfo> list = GetBuffs(buffType);
            List<BuffInfo> result = new List<BuffInfo>();
            if (list != null)
            {
                for (int i = 0, iCount = list.Count; i < iCount; i++)
                {
                    BuffInfo buffInfo = list[i];
                    if (buffInfo.mechanics == null)
                        continue;
                    if (buffInfo.mechanics.mechanicsId == mechanicsId)
                        result.Add(buffInfo);
                }
            }
            return result;
        }

        public List<BuffInfo> GetBuffs(BuffType buffType)
        {
            if (_buffDic == null) return null;
            if (_buffDic.Count == 0) return null;
            List<BuffInfo> list = null;
            List<BuffInfo> result = null;
            if (_buffDic.TryGetValue(buffType, out list))
            {
                if (list.Count > 0)
                {
                    result = new List<BuffInfo>();
                    for (int i = 0, count = list.Count; i < count; i++)
                    {
                        BuffInfo buffInfo = list[i];
                        #region buff type
                        switch (buffInfo.buffType)
                        {
                            case BuffType.Swimmy:
                            case BuffType.Invincible:
                            case BuffType.Silence:
                            case BuffType.Blind:
                            case BuffType.Float:
                            case BuffType.Tumble:
                            case BuffType.Poisoning:
                            case BuffType.Treat:
                            case BuffType.Speed:
                            case BuffType.Shield:
                            case BuffType.Drain:
                            case BuffType.PhysicsDefense:
                            case BuffType.MagicDefense:
                            case BuffType.PhysicsAttack:
                            case BuffType.MagicAttack:
                            case BuffType.HPLimit:
                            case BuffType.Hit:
                            case BuffType.Dodge:
                            case BuffType.Crit:
                            case BuffType.AntiCrit:
                            case BuffType.Block:
                            case BuffType.AntiBlock:
                            case BuffType.CounterAtk:
                            case BuffType.CritHurtAdd:
                            case BuffType.CritHurtDec:
                            case BuffType.Armor:
                            case BuffType.DamageDec:
                            case BuffType.DamageAdd:
                            case BuffType.Frozen:
                            case BuffType.TreatPercent:
                            case BuffType.Ignite:
                            case BuffType.Bleed:
                            case BuffType.Sleep:
                            case BuffType.Landification:
                            case BuffType.Tieup:
                            case BuffType.Immune:
                            case BuffType.Rebound:
                            case BuffType.DamageImmuneTime:
                            case BuffType.TreatAdd:
                            case BuffType.Weakness:
                            case BuffType.ImmunePhysicsAttack:
                            case BuffType.ImmuneMagicAttack:
                            case BuffType.Tag:
                            case BuffType.AccumulatorTag:
                                if (!buffInfo.outOfTime || buffInfo.forever)//更新buff
                                    result.Add(buffInfo);
                                break;
                            case BuffType.GeneralSkillPhysicsAttack:
                            case BuffType.GeneralSkillMagicAttack:
                            case BuffType.TargetSkillPhysicsAttack:
                            case BuffType.TargetSkillMagicAttack:
                            case BuffType.DamageImmuneCount:
                            case BuffType.GeneralSkillHit:
                            case BuffType.GeneralSkillCrit:
                                if (buffInfo.count > 0)
                                    result.Add(buffInfo);
                                break;
                        }
                        #endregion
                    }
                    list.Clear();
                    if (result.Count == 0)
                    {
                        RemoveBuffEffect(buffType);
                    }
                    _buffDic[buffType] = result;
                }
            }
            return result;
        }

        public float GetBuffsValue(BuffType buffType, float original)
        {
            if (_buffDic == null) return 0f;
            float result = 0f;
            if (_buffDic.Count == 0) return result;
            List<BuffInfo> list = null;
            if (_buffDic.TryGetValue(buffType, out list))
            {
                if (list.Count > 0)
                {
                    List<BuffInfo> stillBuffs = new List<BuffInfo>();
                    for (int i = 0, count = list.Count; i < count; i++)
                    {
                        BuffInfo buffInfo = list[i];
                        #region buff type
                        switch (buffInfo.buffType)
                        {
                            case BuffType.Swimmy:
                            case BuffType.Invincible:
                            case BuffType.Silence:
                            case BuffType.Blind:
                            case BuffType.Float:
                            case BuffType.Tumble:
                            case BuffType.Poisoning:
                            case BuffType.Treat:
                            case BuffType.Speed:
                            case BuffType.Shield:
                            case BuffType.Drain:
                            case BuffType.PhysicsDefense:
                            case BuffType.MagicDefense:
                            case BuffType.PhysicsAttack:
                            case BuffType.MagicAttack:
                            case BuffType.HPLimit:
                            case BuffType.Hit:
                            case BuffType.Dodge:
                            case BuffType.Crit:
                            case BuffType.AntiCrit:
                            case BuffType.Block:
                            case BuffType.AntiBlock:
                            case BuffType.CounterAtk:
                            case BuffType.CritHurtAdd:
                            case BuffType.CritHurtDec:
                            case BuffType.Armor:
                            case BuffType.DamageDec:
                            case BuffType.DamageAdd:
                            case BuffType.Frozen:
                            case BuffType.TreatPercent:
                            case BuffType.Ignite:
                            case BuffType.Bleed:
                            case BuffType.Sleep:
                            case BuffType.Landification:
                            case BuffType.Tieup:
                            case BuffType.Immune:
                            case BuffType.Rebound:
                            case BuffType.DamageImmuneTime:
                            case BuffType.TreatAdd:
                            case BuffType.Weakness:
                            case BuffType.ImmunePhysicsAttack:
                            case BuffType.ImmuneMagicAttack:
                            case BuffType.Tag:
                            case BuffType.AccumulatorTag:
                                if (!buffInfo.outOfTime || buffInfo.forever)//更新buff
                                {
                                    switch (buffInfo.buffAddType)
                                    {
                                        case BuffAddType.Fixed:
                                            result += buffInfo.value;
                                            break;
                                        case BuffAddType.Percent:
                                            result += buffInfo.value * original;
                                            break;
                                    }
                                    stillBuffs.Add(buffInfo);
                                }
                                break;
                            case BuffType.GeneralSkillPhysicsAttack:
                            case BuffType.GeneralSkillMagicAttack:
                            case BuffType.DamageImmuneCount:
                            case BuffType.GeneralSkillHit:
                            case BuffType.GeneralSkillCrit:
                                if (buffInfo.count > 0)
                                {
                                    switch (buffInfo.buffAddType)
                                    {
                                        case BuffAddType.Fixed:
                                            result += buffInfo.value;
                                            break;
                                        case BuffAddType.Percent:
                                            result += buffInfo.value * original;
                                            break;
                                    }
                                    stillBuffs.Add(buffInfo);
                                }
                                break;
                            case BuffType.TargetSkillPhysicsAttack:
                            case BuffType.TargetSkillMagicAttack:
                                if (buffInfo.count > 0)
                                {
                                    if (buffInfo.targetSkillId == _currentSkillId)
                                    {
                                        switch (buffInfo.buffAddType)
                                        {
                                            case BuffAddType.Fixed:
                                                result += buffInfo.value;
                                                break;
                                            case BuffAddType.Percent:
                                                result += buffInfo.value * original;
                                                break;
                                        }
                                    }
                                    stillBuffs.Add(buffInfo);
                                }
                                break;
                        }
                        #endregion
                    }
                    list.Clear();
                    if (stillBuffs.Count == 0)
                    {
                        RemoveBuffEffect(buffType);
                    }
                    _buffDic[buffType] = stillBuffs;
                }
            }
            return result;
        }

        public void DisperseBuff(BuffType buffType, bool kindness)
        {
            //Debugger.Log(buffType.ToString());
            List<BuffInfo> list = GetBuffs(buffType);
            if (list != null)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    BuffInfo buffInfo = list[i];
                    if (!buffInfo.disperseable)
                        continue;
                    if (buffInfo.kindness == kindness)
                        buffInfo.Set2OutOfTime();
                }
                if (!ExistBuff(buffType, kindness))
                {
                    RemoveBuffEffect(buffType);
                    RemoveBuffIcon(Fight.Model.FightProxy.instance.GetIconPath(buffType, kindness));
                }
            }
        }

        public void UpdateDamageBuffs()
        {
            List<BuffInfo> damageImmuneCount = GetBuffs(BuffType.DamageImmuneCount);
            if (damageImmuneCount != null)
            {
                int result = 0;
                for (int i = 0, count = damageImmuneCount.Count; i < count; i++)
                {
                    BuffInfo buffInfo = damageImmuneCount[i];
                    buffInfo.count--;
                    result += buffInfo.count;
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.DamageImmuneCount, true);
                    RemoveBuffIcon(path);
                }
            }
        }

        public void UpdateAttackBuffs()
        {
            List<BuffInfo> generalSkillPhysicsAttacks = GetBuffs(BuffType.GeneralSkillPhysicsAttack);
            if (generalSkillPhysicsAttacks != null)
            {
                int result = 0;
                for (int i = 0, count = generalSkillPhysicsAttacks.Count; i < count; i++)
                {
                    BuffInfo buffInfo = generalSkillPhysicsAttacks[i];
                    buffInfo.count--;
                    result += buffInfo.count;
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.GeneralSkillPhysicsAttack, true);
                    RemoveBuffIcon(path);
                }
            }
            List<BuffInfo> generalSkillMagicAttacks = GetBuffs(BuffType.GeneralSkillMagicAttack);
            if (generalSkillMagicAttacks != null)
            {
                int result = 0;
                for (int i = 0, count = generalSkillMagicAttacks.Count; i < count; i++)
                {
                    BuffInfo buffInfo = generalSkillMagicAttacks[i];
                    buffInfo.count--;
                    result += buffInfo.count;
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.GeneralSkillMagicAttack, true);
                    RemoveBuffIcon(path);
                }
            }
            List<BuffInfo> generalSkillHits = GetBuffs(BuffType.GeneralSkillHit);
            if (generalSkillHits != null)
            {
                int result = 0;
                for (int i = 0, count = generalSkillHits.Count; i < count; i++)
                {
                    BuffInfo buffInfo = generalSkillHits[i];
                    buffInfo.count--;
                    result += buffInfo.count;
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.GeneralSkillHit, true);
                    RemoveBuffIcon(path);
                }
            }
            List<BuffInfo> generalSkillCrits = GetBuffs(BuffType.GeneralSkillCrit);
            if (generalSkillCrits != null)
            {
                int result = 0;
                for (int i = 0, count = generalSkillCrits.Count; i < count; i++)
                {
                    BuffInfo buffInfo = generalSkillCrits[i];
                    buffInfo.count--;
                    result += buffInfo.count;
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.GeneralSkillCrit, true);
                    RemoveBuffIcon(path);
                }
            }
            List<BuffInfo> targetSkillPhysicsAttacks = GetBuffs(BuffType.TargetSkillPhysicsAttack);
            if (targetSkillPhysicsAttacks != null)
            {
                int result = 0;
                for (int i = 0, count = targetSkillPhysicsAttacks.Count; i < count; i++)
                {
                    BuffInfo buffInfo = targetSkillPhysicsAttacks[i];
                    if (buffInfo.targetSkillId == _currentSkillId)
                    {
                        buffInfo.count--;
                        result += buffInfo.count;
                    }
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.TargetSkillPhysicsAttack, true);
                    RemoveBuffIcon(path);
                }
            }
            List<BuffInfo> targetSkillMagicAttacks = GetBuffs(BuffType.TargetSkillMagicAttack);
            if (targetSkillMagicAttacks != null)
            {
                int result = 0;
                for (int i = 0, count = targetSkillMagicAttacks.Count; i < count; i++)
                {
                    BuffInfo buffInfo = targetSkillMagicAttacks[i];
                    if (buffInfo.targetSkillId == _currentSkillId)
                    {
                        buffInfo.count--;
                        result += buffInfo.count;
                    }
                }
                if (result == 0 && isPlayer)
                {
                    string path = Fight.Model.FightProxy.instance.GetIconPath(BuffType.TargetSkillMagicAttack, true);
                    RemoveBuffIcon(path);
                }
            }
        }

        [ContextMenu("AddHaloBuff")]
        public void AddHaloBuff()
        {
            foreach (var kvp in _characterInfo.passiveIdDic)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ADD_HALO_BUFF, kvp.Key);
                if (func != null)
                {
                    func.Call(this, kvp.Value);
                    _addedHaloBuff = true;
                }
            }
        }

        public void RemoveHaloBuff(CharacterEntity character)
        {
            foreach (var kvp in buffDic)
            {
                List<BuffInfo> list = kvp.Value;
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    BuffInfo buffInfo = list[i];
                    if (buffInfo.forever && buffInfo.character == character)
                    {
                        buffInfo.forever = false;
                        buffInfo.Set2OutOfTime();
                    }
                }
            }
        }
        #endregion

        #region 角色被控制状态
        public bool Swimmy
        {
            get
            {
                if (isDead)
                {
                    //if (anim.speed == 0f)
                    //    anim.speed = 1f;
                    AnimatorUtil.SetBool(anim, AnimatorUtil.SWIMMY, false);
                    return false;
                }
                bool swimmy = ExistBuff(BuffType.Swimmy);
                if (swimmy)
                {
                    //if (anim.speed != 0f)
                    //    anim.speed = 0f;
                    AnimatorUtil.SetBool(anim, AnimatorUtil.SWIMMY, true);
                }
                else
                {
                    //if (anim.speed == 0f)
                    //    anim.speed = 1f;
                    AnimatorUtil.SetBool(anim, AnimatorUtil.SWIMMY, false);
                }
                return swimmy;
            }
        }

        public bool Frozen
        {
            get
            {
                if (isDead)
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                    return false;
                }
                bool frozen = ExistBuff(BuffType.Frozen);
                if (frozen)
                {
                    if (anim && anim.speed != 0f)
                        anim.speed = 0f;
                }
                else
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                }
                return frozen;
            }
        }

        public bool Sleep
        {
            get
            {
                if (isDead)
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                    return false;
                }
                bool sleep = ExistBuff(BuffType.Sleep);
                if (sleep)
                {
                    if (anim && anim.speed != 0f)
                        anim.speed = 0f;
                }
                else
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                }
                return sleep;
            }
        }

        public bool Landification
        {
            get
            {
                if (isDead)
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                    return false;
                }
                bool landification = ExistBuff(BuffType.Landification);
                if (landification)
                {
                    if (anim && anim.speed != 0f)
                        anim.speed = 0f;
                }
                else
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                }
                return landification;
            }
        }

        public bool Tieup
        {
            get
            {
                if (isDead)
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                    return false;
                }
                bool tieup = ExistBuff(BuffType.Tieup);
                if (tieup)
                {
                    if (anim && anim.speed != 0f)
                        anim.speed = 0f;
                }
                else
                {
                    if (anim && anim.speed == 0f)
                        anim.speed = 1f;
                }
                return tieup;
            }
        }

        public bool Invincible
        {
            get
            {
                return ExistBuff(BuffType.Invincible);
            }
        }

        public bool ImmunePhysicsAttack
        {
            get
            {
                return ExistBuff(BuffType.ImmunePhysicsAttack);
            }
        }

        public bool ImmuneMagicAttack
        {
            get
            {
                return ExistBuff(BuffType.ImmuneMagicAttack);
            }
        }

        public bool Tag
        {
            get
            {
                return ExistBuff(BuffType.Tag);
            }
        }

        public bool Silence
        {
            get
            {
                return ExistBuff(BuffType.Silence);
            }
        }

        public bool Blind
        {
            get
            {
                return ExistBuff(BuffType.Blind);
            }
        }

        public bool Float
        {
            get
            {
                return ExistBuff(BuffType.Float);
            }
        }

        public bool Tumble
        {
            get
            {
                return ExistBuff(BuffType.Tumble);
            }
        }

        public bool controled
        {
            get
            {
                bool result = Swimmy || Frozen || Sleep || Landification || Tieup;
                return result;
            }
        }

        public bool damageImmuneTime
        {
            get
            {
                bool result = ExistBuff(BuffType.DamageImmuneTime);
                return result;
            }
        }

        public bool damageImmuneCount
        {
            get
            {
                bool result = ExistBuff(BuffType.DamageImmuneCount);
                return result;
            }
        }
        #endregion

        #region 角色属性

        private float _lastHitTime, _lastSkill1CDTime, _lastSkill2CDTime;

        public int shieldValue
        {
            get
            {
                int result = (int)GetBuffsValue(BuffType.Shield, 0);
                if (result < 0)
                    return 0;
                return result;
            }
        }

        public int HP
        {
            set
            {
                _characterInfo.HP = value;
            }
            get
            {
                return _characterInfo.HP;
            }
        }

        public uint maxHP
        {
            protected set
            {
                _characterInfo.maxHP = value;
            }
            get
            {
                uint buffsValue = (uint)GetBuffsValue(BuffType.HPLimit, _characterInfo.maxHP);
                uint result = buffsValue + _characterInfo.maxHP;
                return result;
            }
        }

        public bool floatable
        {
            get
            {
                bool result = characterInfo.floatable;
                if (characterInfo.passiveIdDic.Count > 0)
                {
                    foreach (var kvp in characterInfo.passiveIdDic)
                    {
                        LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.ADD_BUFF, kvp.Key);
                        if (func != null)
                        {
                            object[] rs = func.Call(this, BuffType.Float, kvp.Value);
                            int r = 0;
                            int.TryParse(rs[0].ToString(), out r);
                            if (r <= 0)
                                return false;
                        }
                    }
                }
                return result;
            }
        }

        // 物理攻击力
        [SerializeField]
        private int _physicsAttack;
        public int physicsAttack
        {
            protected set
            {
                _physicsAttack = value;
            }
            get
            {
                float result = 0;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.PHYSICS_ATTACK, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(this, kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                result = _physicsAttack * (1 + result);
                int buffsValue = (int)GetBuffsValue(BuffType.PhysicsAttack, result);
                buffsValue += (int)GetBuffsValue(BuffType.GeneralSkillPhysicsAttack, result);
                buffsValue += (int)GetBuffsValue(BuffType.TargetSkillPhysicsAttack, result);
                result += buffsValue;

                if (result < 0)
                    return 0;
                return (int)result;
            }
        }

        // 魔法攻击力
        [SerializeField]
        private int _magicAttack;
        public int magicAttack
        {
            protected set
            {
                _magicAttack = value;
            }
            get
            {
                float result = 0;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.MAGIC_ATTACK, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(this, kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                result = _magicAttack * (1 + result);
                int buffsValue = (int)GetBuffsValue(BuffType.MagicAttack, result);
                buffsValue += (int)GetBuffsValue(BuffType.GeneralSkillMagicAttack, result);
                buffsValue += (int)GetBuffsValue(BuffType.TargetSkillMagicAttack, result);
                result += buffsValue;
                if (result < 0)
                    return 0;
                return (int)result;
            }
        }

        // 物理防御
        [SerializeField]
        private int _physicsDefense;
        public int physicsDefense
        {
            protected set
            {
                _physicsDefense = value;
            }
            get
            {
                float result = 0;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.PHYSICS_DEFENSE, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(this, kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                result = _physicsDefense * (1 + result); ;
                int buffsValue = (int)GetBuffsValue(BuffType.PhysicsDefense, result);
                result += buffsValue;

                if (result < 0)
                    return 0;
                return (int)result;
            }
        }

        // 魔法防御
        [SerializeField]
        private int _magicDefense;
        public int magicDefense
        {
            protected set
            {
                _magicDefense = value;
            }
            get
            {
                float result = 0;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.MAGIC_DEFENSE, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(this, kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                result = _magicDefense * (1 + result); ;
                int buffsValue = (int)GetBuffsValue(BuffType.MagicDefense, result);
                result += buffsValue;
                if (result < 0)
                    return 0;
                return (int)result;
            }
        }

        // 行动力
        public int speed
        {
            protected set
            {
                characterInfo.speed = value;
            }
            get
            {
                return characterInfo.speed;
            }
        }

        // 命中
        [SerializeField]
        private float _hit;
        public float hit
        {
            protected set
            {
                _hit = value;
            }
            get
            {
                if (ExistBuff(BuffType.GeneralSkillHit))
                    return 1f;
                float buffsValue = GetBuffsValue(BuffType.Hit, _hit);
                float result = buffsValue + _hit;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 闪避
        [SerializeField]
        private float _dodge;
        public float dodge
        {
            protected set
            {
                _dodge = value;
            }
            get
            {
                float result = _dodge;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.DODGE, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                float buffsValue = GetBuffsValue(BuffType.Dodge, result);
                result += buffsValue;

                if (result < 0)
                    return 0;
                if (result > 0.75f)
                    return 0.75f;
                return result;
            }
        }

        // 暴击
        [SerializeField]
        private float _crit;
        public float crit
        {
            protected set
            {
                _crit = value;
            }
            get
            {
                if (ExistBuff(BuffType.GeneralSkillCrit))
                    return 1f;
                float result = _crit;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.CRIT, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                float buffsValue = GetBuffsValue(BuffType.Crit, result);
                result += buffsValue;
                //float buffsValue = GetBuffsValue(BuffType.Crit, _crit);
                //float result = buffsValue + _crit;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 防爆
        [SerializeField]
        private float _antiCrit;
        public float antiCrit
        {
            protected set
            {
                _antiCrit = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.AntiCrit, _antiCrit);
                float result = buffsValue + _antiCrit;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 格挡
        [SerializeField]
        private float _block;
        public float block
        {
            protected set
            {
                _block = value;
            }
            get
            {
                float result = _block;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.BLOCK, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                float buffsValue = GetBuffsValue(BuffType.Block, result);
                result += buffsValue;
                //float result = buffsValue + _block;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 破击
        [SerializeField]
        private float _antiBlock;
        public float antiBlock
        {
            protected set
            {
                _antiBlock = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.AntiBlock, _antiBlock);
                float result = buffsValue + _antiBlock;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 反击
        [SerializeField]
        private float _counterAtk;
        public float counterAtk
        {
            protected set
            {
                _counterAtk = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.CounterAtk, _counterAtk);
                float result = buffsValue + _counterAtk;
                if (result < 0)
                    return 0;
                return result;
            }
        }

        // 暴击伤害加成
        [SerializeField]
        private float _critHurtAdd;
        public float critHurtAdd
        {
            protected set
            {
                _critHurtAdd = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.CritHurtAdd, _critHurtAdd);
                float result = buffsValue + _critHurtAdd;
                if (result < 0f)
                    return 0f;
                return result;
            }
        }

        // 暴击伤害减免
        [SerializeField]
        private float _critHurtDec;
        public float critHurtDec
        {
            protected set
            {
                _critHurtDec = value;
            }
            get
            {
                float result = _critHurtDec;
                foreach (var kvp in _characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.CRIT_HURT_DEC, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(kvp.Value);
                        float r = 0;
                        float.TryParse(rs[0].ToString(), out r);
                        result += r;
                    }
                }
                float buffsValue = GetBuffsValue(BuffType.CritHurtDec, result);
                result += buffsValue;
                if (result < 0f)
                    return 0f;
                return result;
            }
        }

        // 破甲
        [SerializeField]
        private float _armor;
        public float armor
        {
            protected set
            {
                _armor = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.Armor, _armor);
                float result = buffsValue + _armor;
                if (result < 0f)
                    return 0f;
                if (result > 1f)
                    return 1f;
                return result;
            }
        }

        // 伤害减免
        [SerializeField]
        private float _damageDec;
        public float damageDec
        {
            protected set
            {
                _damageDec = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.DamageDec, _damageDec);
                float result = buffsValue + _damageDec;
                if (result < -4f)
                    return -4f;
                if (result > 0.75f)
                    return 0.75f;
                return result;
            }
        }

        // 伤害加成
        [SerializeField]
        private float _damageAdd;
        public float damageAdd
        {
            protected set
            {
                _damageAdd = value;
            }
            get
            {
                float buffsValue = GetBuffsValue(BuffType.DamageAdd, _damageAdd);
                float result = buffsValue + _damageAdd;
                if (result < -0.75f)
                    return -0.75f;
                return result;
            }
        }


        // 行动力
        //public float hitCD
        //{
        //    get
        //    {
        //        if (tickCD)
        //        {
        //            float time = Time.time;
        //            characterInfo.hitCD += (time - _lastHitTime);
        //            _lastHitTime = time;
        //        }
        //        return characterInfo.hitCD;
        //    }
        //    set
        //    {
        //        characterInfo.hitCD = value;
        //        if (value == 0)
        //        {
        //            canOrderHit = true;
        //            _lastHitTime = Time.time;
        //        }
        //    }
        //}

        public float skill1CD
        {
            get
            {
                float time = TimeController.instance.fightSkillTime;
                characterInfo.skill1CD += (time - _lastSkill1CDTime);
                _lastSkill1CDTime = time;
                return characterInfo.skill1CD;
            }
            set
            {
                characterInfo.skill1CD = value;
                if (value == 0)
                {
                    canOrderSkill1 = true;
                    _lastSkill1CDTime = TimeController.instance.fightSkillTime;
                }
            }
        }

        public float skill2CD
        {
            get
            {
                float time = TimeController.instance.fightSkillTime;
                characterInfo.skill2CD += (time - _lastSkill2CDTime);
                _lastSkill2CDTime = time;
                return characterInfo.skill2CD;
            }
            set
            {
                characterInfo.skill2CD = value;
                if (value == 0)
                {
                    canOrderSkill2 = true;
                    _lastSkill2CDTime = TimeController.instance.fightSkillTime;
                }
            }
        }
        #endregion

        public virtual bool isInPlace
        {
            get
            {
                return Vector3.Distance(_original, pos) <= 0.01f;
            }
        }

        //protected bool _canOrderHit = true;
        //public virtual bool canOrderHit
        //{
        //    get
        //    {
        //        if (characterInfo.hitSkillInfo == null || isDead) return false;
        //        return _canOrderHit && hitCD > (Const.Model.ConstData.GetConstData().speedMax / speed) && !controled;
        //    }
        //    set
        //    {
        //        _canOrderHit = value;
        //    }
        //}

        protected float _canOrderTime;
        public float canOrderTime
        {
            set
            {
                _canOrderTime = value;
            }
            get
            {
                return _canOrderTime;
            }
        }

        public virtual bool canOrderSkill
        {
            get
            {
                return Time.time >= _canOrderTime;
            }
        }

        protected bool _canOrderSkill1 = false;
        public virtual bool canOrderSkill1
        {
            get
            {
                if (characterInfo.skillInfo1 == null || isDead) return false;
                return _canOrderSkill1 && skill1CD >= characterInfo.skillInfo1.skillData.CD && !controled && !Silence;
            }
            set
            {
                _canOrderSkill1 = value;
            }
        }

        protected bool _canOrderSkill2 = false;
        public virtual bool canOrderSkill2
        {
            get
            {
                if (characterInfo.skillInfo2 == null || isDead) return false;
                return _canOrderSkill2 && skill2CD >= characterInfo.skillInfo2.skillData.CD && !controled && !Silence;
            }
            set
            {
                _canOrderSkill2 = value;
            }
        }

        public virtual bool canPlayHit
        {
            get
            {
                if (characterInfo.hitSkillInfo == null || isDead) return false;
                if (!canPlayAnimator || Swimmy) return false;
#if UNITY_EDITOR
                if (VirtualServerController.instance.fightEidtor)
                    return status == Status.Idle && isInPlace;
#endif
                //return hitCD > (Const.Model.ConstData.GetConstData().speedMax / speed) && (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled;
                return (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled;
            }
        }

        public virtual bool canPlaySkill1
        {
            get
            {
                if (characterInfo.skillInfo1 == null || isDead) return false;
                if (!canPlayAnimator || Swimmy) return false;
#if UNITY_EDITOR
                if (VirtualServerController.instance.fightEidtor)
                    return status == Status.Idle && isInPlace;
#endif
                return skill1CD >= characterInfo.skillInfo1.skillData.CD && (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled && !Silence;
            }
        }

        public virtual bool canPlaySkill2
        {
            get
            {
                if (characterInfo.skillInfo2 == null || isDead) return false;
                if (!canPlayAnimator || Swimmy) return false;
#if UNITY_EDITOR
                if (VirtualServerController.instance.fightEidtor)
                    return status == Status.Idle && isInPlace;
#endif
                return skill2CD >= characterInfo.skillInfo2.skillData.CD && (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled && !Silence;
            }
        }

        public bool canPlayAnimator
        {
            get
            {
                if (/*Swimmy || */Frozen || Sleep || Landification || Tieup)
                    return false;
                if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal)
                    if (AnimatorUtil.isTargetState(anim, AnimatorUtil.GETUP_ID) || AnimatorUtil.isTargetState(anim, AnimatorUtil.TUMBLEGETHIT_ID) ||
                        AnimatorUtil.isTargetState(anim, AnimatorUtil.FLOATGETHIT_ID) || AnimatorUtil.isTargetState(anim, AnimatorUtil.FLOATDOWN_ID))
                        return false;
                return true;
            }
        }

        public virtual bool canAttack
        {
            get
            {
                if (isDead || HP <= 0) return false;
                return (_status == Status.Idle || _status == Status.GetHit || _status == Status.BootSkill) && isInPlace;
            }
        }

        public bool isDead
        {
            get
            {
                return _status == Status.Dead;
            }
            set
            {
                if (value)
                {
                    status = Status.Dead;
                    //if (Swimmy)
                    //    anim.speed = 1f;
                    //OnHPChange = null;
                }
            }
        }
        #endregion

        public void SetOrderable()
        {
            //canOrderHit = true;
            canOrderSkill1 = true;
            canOrderSkill2 = true;
            _canOrderTime = Time.time;
        }

        public virtual void Reborn(float hpRate)
        {
            status = Status.Idle;
            //hitCD = skill1CD = skill2CD = 0f;
            skill1CD = skill2CD = 0f;
            HP = (int)(hpRate * maxHP);
            AddHaloBuff();
        }

        public virtual void ResetSkillOrder(uint skillID)
        {
            if (skillID == characterInfo.skillId1)
            {
                canOrderSkill1 = true;
                _canOrderTime = Time.time;
            }
            else if (skillID == characterInfo.skillId2)
            {
                canOrderSkill2 = true;
                _canOrderTime = Time.time;
            }
            else if (skillID == characterInfo.aeonSkill)
                _canOrderTime = Time.time;
        }

        public virtual bool ExistSkill(uint skillID)
        {
            if (skillID == characterInfo.hitId)
                return true;
            else if (skillID == characterInfo.skillId1)
                return true;
            else if (skillID == characterInfo.skillId2)
                return true;
            return false;
        }

        public virtual bool CanPlaySkill(uint skillID)
        {
            if (Time.realtimeSinceStartup < _lastSkillOverTime + ANIMATION_DELAY) return false;
            if (skillID == characterInfo.hitId)
                return canPlayHit;
            else if (skillID == characterInfo.skillId1)
                return canPlaySkill1;
            else if (skillID == characterInfo.skillId2)
                return canPlaySkill2;
            return false;
        }

        public SkillInfo GetSkillInfoById(uint skillID)
        {
            if (characterInfo.hitSkillInfo != null && characterInfo.hitSkillInfo.skillData.skillId == skillID)
                return characterInfo.hitSkillInfo;
            else if (characterInfo.skillInfo1 != null && characterInfo.skillInfo1.skillData.skillId == skillID)
                return characterInfo.skillInfo1;
            else if (characterInfo.skillInfo2 != null && characterInfo.skillInfo2.skillData.skillId == skillID)
                return characterInfo.skillInfo2;
            else if (characterInfo.aeonSkillInfo != null && characterInfo.aeonSkillInfo.skillData.skillId == skillID)
                return characterInfo.aeonSkillInfo;
            return null;
        }

        public virtual void PlaySkill(SkillInfo skillInfo)
        {
            _currentSkillId = skillInfo.skillData.skillId;
            if (!_skillCountDic.ContainsKey(_currentSkillId))
            {
                _skillCountDic.Add(_currentSkillId, 1);
            }
            else
                _skillCountDic[_currentSkillId]++;
        }

        [NoToLua]
        public virtual uint SetDamageValue(CharacterEntity character, SkillInfo skillInfo, uint hitValue)
        {
            float damgeRateDec = 0f;
            if (character)
            {
                RoleAttackAttributeType roleAttackAttributeType = CharacterUtil.GetRoleAttackAttributeType(character.characterInfo.roleType);
                foreach (var kvp in character.characterInfo.passiveIdDic)
                {
                    LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.DAMAGE, kvp.Key);
                    if (func != null)
                    {
                        object[] rs = func.Call(character, this, roleAttackAttributeType, skillInfo, kvp.Value);
                        if (rs != null && rs.Length > 0)
                            float.TryParse(rs[0].ToString(), out damgeRateDec);
                    }
                }
            }

            hitValue = (uint)((1 - damgeRateDec) * hitValue);
            int shield = shieldValue;
            int originalHP = HP;
            if (character)
            {
                if (HP > 0 && (HP + shield) <= hitValue)
                {
                    foreach (var kvp in character.characterInfo.passiveIdDic)
                    {
                        LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.KILLER_BUFF, kvp.Key);
                        if (func != null)
                            func.Call(character, skillInfo, kvp.Value);
                    }
                }
            }
            if (shield > 0)
            {
                #region calc shield value
                if (shield < hitValue)
                    HP -= ((int)hitValue - shield);
                List<BuffInfo> buffs = GetBuffs(BuffType.Shield);
                int r = (int)hitValue;
                for (int i = 0, count = buffs.Count; i < count; i++)
                {
                    BuffInfo bi = buffs[i];
                    if (bi.value <= r)
                    {
                        r -= (int)bi.value;
                        bi.value = 0;
                    }
                    else
                    {
                        bi.value -= r;
                        r = 0;
                    }
                    if (r <= 0)
                        break;
                }
                #endregion
            }
            else
                HP -= (int)hitValue;
            if (HP <= 0)
                HP = 0;
            if (OnHPChange != null)
                OnHPChange();
            if (HP <= 0)
                isDead = true;
            if (hpBarView)
                hpBarView.UpdateHPValue(true);
            uint result = (uint)(originalHP - HP);
            if (HP == 0)
                result = hitValue;
            return result;
            //Debugger.LogError("character {0} hurt target {1} damge {2} originalHP {3} currentHP {4}", character.characterInfo.instanceID, characterInfo.instanceID, hitValue, originalHP, HP);
        }

        [NoToLua]
        public virtual void SetTreatValue(uint hpValue)
        {
            if (HP <= 0) return;
            HP += (int)hpValue;
            if (HP > maxHP)
                HP = (int)maxHP;
            if (OnHPChange != null)
                OnHPChange();
            if (hpBarView)
                hpBarView.UpdateHPValue(false);
        }


        public void ResetCD(SkillInfo skillInfo)
        {
            if (skillInfo == null) return;
            //if (characterInfo.hitId == skillInfo.skillData.skillId)
            if (characterInfo.skillId1 == skillInfo.skillData.skillId)
            {
                _canOrderTime = Time.time + FightController.COMMON_CD_TIME;
                skill1CD = 0;
            }
            else if (characterInfo.skillId2 == skillInfo.skillData.skillId)
            {
                _canOrderTime = Time.time + FightController.COMMON_CD_TIME;
                skill2CD = 0;
            }
            //else if (characterInfo.aeonSkill == skillInfo.skillData.skillId)
            //{ 
            //}
            skillInfo.mechanicsIndex = 0;
        }

        public virtual void SetStatus(Status status)
        {
            if (this.status != Status.Dead)
            {
                this.status = status;
            }
        }

        public virtual void ResetStatus()
        {
            if (this.status != Status.Dead)
                status = Status.Idle;
        }

        #region set attr
        //public virtual void SetAttr(HeroAttrProtoData pvePlayerAttrProtoData)
        //{
        //    HP = pvePlayerAttrProtoData.hp;
        //    maxHP = (uint)pvePlayerAttrProtoData.hpUp;
        //    physicsAttack = pvePlayerAttrProtoData.normal_atk;
        //    magicAttack = pvePlayerAttrProtoData.magic_atk;
        //    physicsDefense = pvePlayerAttrProtoData.normal_def;
        //    magicDefense = pvePlayerAttrProtoData.magic_def;
        //    speed = pvePlayerAttrProtoData.speed;
        //    hit = pvePlayerAttrProtoData.hit;
        //    dodge = pvePlayerAttrProtoData.dodge;
        //    crit = pvePlayerAttrProtoData.crit;
        //    antiCrit = pvePlayerAttrProtoData.anti_crit;
        //    block = pvePlayerAttrProtoData.block;
        //    antiBlock = pvePlayerAttrProtoData.anti_block;
        //    counterAtk = pvePlayerAttrProtoData.counter_atk;
        //    critHurtAdd = pvePlayerAttrProtoData.crit_hurt_add;
        //    critHurtDec = pvePlayerAttrProtoData.crit_hurt_dec;
        //    armor = pvePlayerAttrProtoData.armor;
        //    damageAdd = pvePlayerAttrProtoData.damage_add;
        //    damageDec = pvePlayerAttrProtoData.damage_dec;
        //}

        [NoToLua]
        public virtual void SetAttr(HeroAttrProtoData pveHeroAttrProtoData)
        {
            HP = pveHeroAttrProtoData.hp;
            maxHP = (uint)pveHeroAttrProtoData.hpUp;
            physicsAttack = pveHeroAttrProtoData.normal_atk;
            magicAttack = pveHeroAttrProtoData.magic_atk;
            physicsDefense = pveHeroAttrProtoData.normal_def;
            magicDefense = pveHeroAttrProtoData.magic_def;
            speed = pveHeroAttrProtoData.speed;
            hit = pveHeroAttrProtoData.hit;
            dodge = pveHeroAttrProtoData.dodge;
            crit = pveHeroAttrProtoData.crit;
            antiCrit = pveHeroAttrProtoData.anti_crit;
            block = pveHeroAttrProtoData.block;
            antiBlock = pveHeroAttrProtoData.anti_block;
            counterAtk = pveHeroAttrProtoData.counter_atk;
            critHurtAdd = pveHeroAttrProtoData.crit_hurt_add;
            critHurtDec = pveHeroAttrProtoData.crit_hurt_dec;
            armor = pveHeroAttrProtoData.armor;
            damageAdd = pveHeroAttrProtoData.damage_add;
            damageDec = pveHeroAttrProtoData.damage_dec;
        }

        [NoToLua]
        public void SetAttr(WorldBossFightProto worldBossFightProto)
        {
            HP = worldBossFightProto.currHp;
            maxHP = (uint)worldBossFightProto.hpUpperLimit;
            physicsAttack = worldBossFightProto.atk;
            magicAttack = worldBossFightProto.atk;
            physicsDefense = worldBossFightProto.def;
            magicDefense = worldBossFightProto.def;
            damageAdd = worldBossFightProto.damage_add;


            HeroData worldBossData = HeroData.GetHeroDataByID(worldBossFightProto.id);
            //			maxHP = (uint)heroInfo.heroData.HP;
            //			HP = (int)maxHP;
            //			physicsAttack = (int)heroInfo.heroData.normalAtk;
            //			magicAttack = (int)heroInfo.heroData.magicAtk;
            //			physicsDefense = (int)worldBossData.normalDef;
            //			magicDefense = (int)worldBossData.magicDef;
            speed = (int)worldBossData.speed;
            hit = worldBossData.hit;
            dodge = worldBossData.dodge;
            crit = worldBossData.crit;
            antiCrit = worldBossData.antiCrit;
            block = worldBossData.block;
            antiBlock = worldBossData.antiBlock;
            counterAtk = worldBossData.counterAtk;
            critHurtAdd = worldBossData.critHurtAdd;
            critHurtDec = worldBossData.critHurtDec;
            armor = worldBossData.armor;
            //			damageAdd = heroInfo.heroData.damageAdd;
            damageDec = worldBossData.damageDec;
        }

        #region fight imitate
#if UNITY_EDITOR
        [NoToLua]
        public virtual void SetAttr(Dictionary<RoleAttributeType, RoleAttribute> attrDic)
        {
            maxHP = (uint)attrDic[RoleAttributeType.HP].value;
            HP = (int)maxHP;
            physicsAttack = (int)attrDic[RoleAttributeType.NormalAtk].value;
            magicAttack = (int)attrDic[RoleAttributeType.MagicAtk].value;
            physicsDefense = (int)attrDic[RoleAttributeType.Normal_Def].value;
            magicDefense = (int)attrDic[RoleAttributeType.Normal_Def].value;
            speed = (int)attrDic[RoleAttributeType.Speed].value;
            hit = attrDic[RoleAttributeType.Hit].value;
            dodge = attrDic[RoleAttributeType.Dodge].value;
            crit = attrDic[RoleAttributeType.Crit].value;
            antiCrit = attrDic[RoleAttributeType.AntiCrit].value;
            block = attrDic[RoleAttributeType.Block].value;
            antiBlock = attrDic[RoleAttributeType.AntiBlock].value;
            counterAtk = attrDic[RoleAttributeType.CounterAtk].value;
            critHurtAdd = attrDic[RoleAttributeType.CritHurtAdd].value;
            critHurtDec = attrDic[RoleAttributeType.CritHurtDec].value;
            armor = attrDic[RoleAttributeType.Armor].value;
            damageAdd = attrDic[RoleAttributeType.DamageAdd].value;
            damageDec = attrDic[RoleAttributeType.CritHurtDec].value;
        }

        [NoToLua]
        public virtual void SetAttr(Player.Model.PlayerInfo playerInfo)
        {
            float factor = 48.4f;
            maxHP = (uint)(playerInfo.heroData.HP * factor);
            HP = (int)maxHP;
            physicsAttack = (int)(playerInfo.heroData.normalAtk * factor);
            magicAttack = (int)(playerInfo.heroData.magicAtk * factor);
            physicsDefense = (int)(playerInfo.heroData.normalDef * factor);
            magicDefense = (int)(playerInfo.heroData.magicDef * factor);
            speed = (int)playerInfo.heroData.speed;
            hit = playerInfo.heroData.hit;
            dodge = playerInfo.heroData.dodge;
            crit = playerInfo.heroData.crit;
            antiCrit = playerInfo.heroData.antiCrit;
            block = playerInfo.heroData.block;
            antiBlock = playerInfo.heroData.antiBlock;
            counterAtk = playerInfo.heroData.counterAtk;
            critHurtAdd = playerInfo.heroData.critHurtAdd;
            critHurtDec = playerInfo.heroData.critHurtDec;
            armor = playerInfo.heroData.armor;
            damageAdd = playerInfo.heroData.damageAdd;
            damageDec = playerInfo.heroData.damageDec;
        }
#endif
        #endregion

        [NoToLua]
        public virtual void SetAttr(HeroInfo heroInfo)
        {
            maxHP = (uint)heroInfo.heroData.HP;
            HP = (int)maxHP;
            physicsAttack = (int)heroInfo.heroData.normalAtk;
            magicAttack = (int)heroInfo.heroData.magicAtk;
            physicsDefense = (int)heroInfo.heroData.normalDef;
            magicDefense = (int)heroInfo.heroData.magicDef;
            speed = (int)heroInfo.heroData.speed;
            hit = heroInfo.heroData.hit;
            dodge = heroInfo.heroData.dodge;
            crit = heroInfo.heroData.crit;
            antiCrit = heroInfo.heroData.antiCrit;
            block = heroInfo.heroData.block;
            antiBlock = heroInfo.heroData.antiBlock;
            counterAtk = heroInfo.heroData.counterAtk;
            critHurtAdd = heroInfo.heroData.critHurtAdd;
            critHurtDec = heroInfo.heroData.critHurtDec;
            armor = heroInfo.heroData.armor;
            damageAdd = heroInfo.heroData.damageAdd;
            damageDec = heroInfo.heroData.damageDec;
        }

        [NoToLua]
        public virtual void SetAttr(KeyValuePair<FormationPosition, HeroInfo> kvp, float maxHPRate, float capacityFactor)
        {
            Logic.Dungeon.Model.DungeonData dungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
            float powerFix = dungeonData.powerFix;
            if (dungeonData.dungeonType == DungeonType.BossSubspecies)
                powerFix = Logic.Dungeon.Model.SpecialBossData.GetBossPowerByLevel((uint)Game.Model.GameProxy.instance.AccountLevel);
            Team.Model.TeamData teamData = dungeonData.teamDataList[Logic.Fight.Model.FightProxy.instance.CurrentTeamIndex];
            Team.Model.Rate rate = default(Team.Model.Rate);
            if (teamData.rateDictionary.ContainsKey(kvp.Key))
                rate = teamData.rateDictionary[kvp.Key];
            HeroInfo heroInfo = kvp.Value;
            maxHP = (uint)((heroInfo.heroData.HP * powerFix * rate.HPRate) * maxHPRate);
            HP = (int)maxHP;
            physicsAttack = (int)(heroInfo.heroData.normalAtk * powerFix * rate.attackRate);
            magicAttack = (int)(heroInfo.heroData.magicAtk * powerFix * rate.attackRate);
            if (capacityFactor > 1)
            {
                physicsAttack = (int)(physicsAttack * capacityFactor);
                magicAttack = (int)(magicAttack * capacityFactor);
            }
            physicsDefense = (int)(heroInfo.heroData.normalDef * powerFix * rate.defenseRate);
            magicDefense = (int)(heroInfo.heroData.magicDef * powerFix * rate.defenseRate);
            speed = (int)heroInfo.heroData.speed;
            hit = heroInfo.heroData.hit;
            dodge = heroInfo.heroData.dodge;
            crit = heroInfo.heroData.crit;
            antiCrit = heroInfo.heroData.antiCrit;
            block = heroInfo.heroData.block;
            antiBlock = heroInfo.heroData.antiBlock;
            counterAtk = heroInfo.heroData.counterAtk;
            critHurtAdd = heroInfo.heroData.critHurtAdd;
            critHurtDec = heroInfo.heroData.critHurtDec;
            armor = heroInfo.heroData.armor;
            damageAdd = heroInfo.heroData.damageAdd;
            damageDec = heroInfo.heroData.damageDec;
        }

        [NoToLua]
        public virtual void CloneAttr(CharacterEntity character)
        {
            physicsAttack = character.physicsAttack;
            magicAttack = character.magicAttack;
            physicsDefense = character.physicsDefense;
            magicDefense = character.magicDefense;
            speed = character.speed;
            hit = character.hit;
            dodge = character.dodge;
            crit = character.crit;
            antiCrit = character.antiCrit;
            block = character.block;
            antiBlock = character.antiBlock;
            counterAtk = character.counterAtk;
            critHurtAdd = character.critHurtAdd;
            critHurtDec = character.critHurtDec;
            armor = character.armor;
            damageAdd = character.damageAdd;
            damageDec = character.damageDec;
            buffDic = character.buffDic;
        }
        #endregion

        protected virtual void OnDestroy()
        {
            _buffDic.Clear();
            _buffDic = null;
            _buffEffectDic.Clear();
            _buffEffectDic = null;
            _skillCountDic.Clear();
            _skillCountDic = null;
        }
        #endregion
    }
}

