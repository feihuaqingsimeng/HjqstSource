using UnityEngine;
using System.IO;
using System.Collections;

namespace Common.ResMgr
{
    public class ResPath
    {
        public const string CONFIG_FILE_PATH = "config/csv/";
        public const string NPC_TEXTURE_PATH = "ui_textures/npc_images/";
		public const string CHAPTER_BG_PATH = "ui_textures/chapter_bg_textures/";
        public const string PLAYER_MODEL_PATH = "character/player/";
        public const string FIGURE_IMAGE_PATH = "role_figure_image/";
        public const string PLAYER_PORTRAIT_PATH = "sprite/player_portrait/";
        public const string HERO_MODEL_PATH = "character/hero/";
        public const string PET_MODEL_PATH = "character/pet/";
        public const string PLAYER_ANIMATOR_CONTROLLER_PATH = "animcontroller/player/";
        public const string HERO_ANIMATOR_CONTROLLER_PATH = "animcontroller/hero/";
        public const string CHARACTER_HEAD_ICON_PATH = "sprite/head_icon/";
        public const string EQUIPMENT_ICON_PATH = "sprite/equipment_icon/";
        public const string SHOP_ITEM_ICON_PATH = "sprite/item_icon/";
        public const string ITEM_ICON_PATH = "sprite/item_icon/";
        public const string ACTIVITY_ICON_PATH = "sprite/daily_dungeon_cards/";
        public const string SKILL_ICON_PATH = "sprite/skill/";
        public const string TASK_ICON_PATH = "sprite/task_icon/";
        public const string ROLE_SKILL_HEAD_ICON_PATH = "sprite/skill_head/";


        public static string GetConfigFilePath(string configFileName)
        {
            return Path.Combine(CONFIG_FILE_PATH, configFileName);
        }

        public static string GetNPCTexturePath(string npcTextureName)
        {
            return Path.Combine(NPC_TEXTURE_PATH, npcTextureName);
        }

        public static string GetChapterBGPath(string chapterBGName)
        {
            return Path.Combine(CHAPTER_BG_PATH, chapterBGName);
        }

        public static string GetPlayerModelPath(string modelName)
        {
            return Path.Combine(PLAYER_MODEL_PATH, modelName);
        }

        public static string GetFigureImagePath(string figureImageName)
        {
            return Path.Combine(FIGURE_IMAGE_PATH, figureImageName);
        }

        public static string GetPlayerPortraitPath(string playerPortraitName)
        {
            return Path.Combine(PLAYER_PORTRAIT_PATH, playerPortraitName);
        }

        public static string GetHeroModelPath(string heroModelName)
        {
            return Path.Combine(HERO_MODEL_PATH, heroModelName);
        }

        public static string GetPetModelPath(string petModelName)
        {
            return Path.Combine(PET_MODEL_PATH, petModelName);
        }

        public static string GetPlayerAnimatorControllerPath(string animatorControllerName)
        {
            return Path.Combine(PLAYER_ANIMATOR_CONTROLLER_PATH, animatorControllerName);
        }

        public static string GetHeroAnimatorControllerPath(string animatorControllerName)
        {
            return Path.Combine(HERO_ANIMATOR_CONTROLLER_PATH, animatorControllerName);
        }

        public static string GetCharacterHeadIconPath(string roleHeadIconName)
        {
            return Path.Combine(CHARACTER_HEAD_ICON_PATH, roleHeadIconName);
        }

        public static string GetEquipmentIconPath(string equipmentIconName)
        {
            return Path.Combine(EQUIPMENT_ICON_PATH, equipmentIconName);
        }

        public static string GetItemIconPath(string itemIconName)
        {
            return Path.Combine(ITEM_ICON_PATH, itemIconName);
        }

        public static string GetShopItemIconPath(string shopItemIconName)
        {
            return Path.Combine(SHOP_ITEM_ICON_PATH, shopItemIconName);
        }

        public static string GetActivityIconPath(string activityIconName)
        {
            return Path.Combine(ACTIVITY_ICON_PATH, activityIconName);
        }
        public static string GetSkillIconPath(string skillIconName)
        {
            return Path.Combine(SKILL_ICON_PATH, skillIconName);
        }
        public static string GetTaskIconPath(string iconName)
        {
            return Path.Combine(TASK_ICON_PATH, iconName);
        }

        public static string GetTutorialIllustrateImagePath(string illustrateImageName)
        {
            return Path.Combine(NPC_TEXTURE_PATH, illustrateImageName);
        }

        public static string GetRoleSkillHeadIconPath(string roleHeadIconName)
        {
            return Path.Combine(ROLE_SKILL_HEAD_ICON_PATH, roleHeadIconName);
        }
    }
}
