res_path = {}

res_path.CONFIG_FILE_PATH = "config/csv/"
res_path.NPC_TEXTURE_PATH = "ui_textures/npc_images/"
res_path.CHAPTER_BG_PATH = "ui_textures/background_images/"
res_path.PLAYER_MODEL_PATH = "character/player/"
res_path.FIGURE_IMAGE_PATH = "role_figure_image/"
res_path.PLAYER_PORTRAIT_PATH = "sprite/daily_dungeon_cards/"
res_path.HERO_MODEL_PATH = "character/hero/"
res_path.PET_MODEL_PATH = "character/pet/"
res_path.PLAYER_ANIMATOR_CONTROLLER_PATH = "animcontroller/player/"
res_path.HERO_ANIMATOR_CONTROLLER_PATH = "animcontroller/hero/"
res_path.CHARACTER_HEAD_ICON_PATH = "sprite/head_icon/"
res_path.EQUIPMENT_ICON_PATH = "sprite/equipment_icon/"
res_path.SHOP_ITEM_ICON_PATH = "sprite/item_icon/"
res_path.ITEM_ICON_PATH = "sprite/item_icon/"
res_path.ACTIVITY_ICON_PATH = "sprite/daily_dungeon_cards/"
res_path.SKILL_ICON_PATH = "sprite/skill/"
res_path.TASK_ICON_PATH = "sprite/task_icon/"
res_path.ROLE_SKILL_HEAD_ICON_PATH = "sprite/skill_head/"


function res_path.GetConfigFilePath(configFileName)
    return res_path.CONFIG_FILE_PATH..configFileName
end

function res_path.GetNPCTexturePath(npcTextureName)
    return res_path.NPC_TEXTURE_PATH..npcTextureName
end

function res_path.GetChapterBGPath(chapterBGName)
    return res_path.CHAPTER_BG_PATH..chapterBGName
end

function res_path.GetPlayerModelPath(modelName)
    return res_path.PLAYER_MODEL_PATH..modelName
end

function res_path.GetFigureImagePath(figureImageName)
    return res_path.FIGURE_IMAGE_PATH..figureImageName
end

function res_path.GetPlayerPortraitPath(playerPortraitName)
    return res_path.PLAYER_PORTRAIT_PATH..playerPortraitName
end

function res_path.GetHeroModelPath(heroModelName)
    return res_path.HERO_MODEL_PATH..heroModelName
end

function res_path.GetPetModelPath(petModelName)
    return res_path.PET_MODEL_PATH..petModelName
end

function res_path.GetPlayerAnimatorControllerPath(animatorControllerName)
    return res_path.PLAYER_ANIMATOR_CONTROLLER_PATH..animatorControllerName
end

function res_path.GetHeroAnimatorControllerPath(animatorControllerName)
    return res_path.HERO_ANIMATOR_CONTROLLER_PATH..animatorControllerName
end

function res_path.GetCharacterHeadIconPath(roleHeadIconName)
    return res_path.CHARACTER_HEAD_ICON_PATH..roleHeadIconName
end

function res_path.GetEquipmentIconPath(equipmentIconName)
    return res_path.EQUIPMENT_ICON_PATH..equipmentIconName
end

function res_path.GetItemIconPath(itemIconName)
    return res_path.ITEM_ICON_PATH..itemIconName
end

function res_path.GetShopItemIconPath(shopItemIconName)
    return res_path.SHOP_ITEM_ICON_PATH..shopItemIconName
end

function res_path.GetActivityIconPath(activityIconName)
    return res_path.ACTIVITY_ICON_PATH..activityIconName
end

function res_path.GetSkillIconPath(skillIconName)
    return res_path.SKILL_ICON_PATH..skillIconName
end

function res_path.GetTaskIconPath(iconName)
    return res_path.TASK_ICON_PATH..iconName
end

function res_path.GetTutorialIllustrateImagePath(illustrateImageName)
    return res_path.NPC_TEXTURE_PATH..illustrateImageName
end

function res_path.GetRoleSkillHeadIconPath(roleHeadIconName)
    return res_path.ROLE_SKILL_HEAD_ICON_PATH..roleHeadIconName
end