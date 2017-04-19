local t = {}
local name = 'illustration_model'

local illustration_data = gamemanager.GetData('illustration_data')
local hero_data = gamemanager.GetData('hero_data')
local player_info = require('ui/player/model/player_info')
local hero_info = require('ui/hero/model/hero_info')
local equip_info = require('ui/equip/model/equip_info')
local item_info = require('ui/item/model/item_info')
local global_data = gamemanager.GetData('global_data')
local illustration_info = require('ui/illustration/model/illustration_info')
local scroll_item_data = require('ui/illustration/model/scroll_item_data')
local role_attr = require('ui/role/model/role_attr')


  --已获得的图鉴英雄、装备、道具 key string:("type,modelid")   value bool: true
t.IllustrationDictionary = {}
t.power = 0 --图鉴英雄战力加成
--图鉴收集附加属性
t.IllustrationAddAttrDictionary = Dictionary.New('number','number')
t.IllustrationOldDictionary = {}--已获得的图鉴英雄(缓存的旧数据 用于查看界面)
 --图鉴英雄表格数据 
t.IllustrationRoleDataDictionary = Dictionary.New('number','Mlist<illustration_info>>')
--图鉴装备表格数据 
t.IllustrationEquipDataDictionary = Dictionary.New('number','Mlist<illustration_info>>')
--图鉴道具表格数据 
t.IllustrationItemDataDictionary = Dictionary.New('number','Mlist<illustration_info>>')

  --大页签下的子页签
t.smallTitleStringDictionary = Dictionary.New('number','Dictionary<number,string>')

t.selectDropdownIndex = 0 -- 界面销毁时记录当前选中的页签
t.scrollPercent = 1 -- 界面销毁时记录滚动的位置
t.selectRoleInfo = nil --界面销毁时记录当前选中的英雄
t.selectIllustrationType = IllustrationType.hero--当前查看的类型

t.currentSelectList = MList.New('scroll_item_data') --scroll view 中要取的数据



local function Start()

  gamemanager.RegisterModel(name,t)

end

function t.GetIllustrationDictionary()
  return t.IllustrationDictionary
end

function t.Clear()
  t.IllustrationDictionary = {}
  t.IllustrationOldDictionary()
end

--初始化表格数据
function t.InitDataDictionary()
  if t.IllustrationRoleDataDictionary.Count ~= 0 then
    return
  end
  local heroData = nil
  --local temp_big_title_dic = nil
  local temp_small_title_dic = nil
  for k,v in pairs(illustration_data.data) do
    
    if t.smallTitleStringDictionary:Get(v.illustration_type) == nil then
      t.smallTitleStringDictionary:Add(v.illustration_type,Dictionary.New('number','string'))
    end
    temp_small_title_dic = t.smallTitleStringDictionary:Get(v.illustration_type)
    
    if v.illustration_type == IllustrationType.hero then
      --英雄图鉴
      if t.IllustrationRoleDataDictionary:Get(v.sheet) == nil then 
        t.IllustrationRoleDataDictionary:Add(v.sheet,MList.New('illustration_info'))
        temp_small_title_dic:Add(v.sheet,LocalizationController.instance:Get( v.sheet_name))
        --print('英雄图鉴sheet',temp_small_title_dic:Get(v.sheet))
      end
      local illus_info_list = t.IllustrationRoleDataDictionary:Get(v.sheet)
      heroData = hero_data.GetDataById(v.resData.id)
      if heroData.hero_type == 2 then --主角
        local playerInfo = player_info:New( 0,  v.resData.id, 0,  0, 0, '')
        playerInfo.advanceLevel = v.resData.star
        playerInfo.level = 1
        illus_info_list:Add(illustration_info.NewByRoleInfo(v.id,playerInfo))
      else--英雄
        local heroInfo = hero_info:New( 0,  v.resData.id,  1,  0,  v.resData.star,  1)
        illus_info_list:Add(illustration_info.NewByRoleInfo(v.id,heroInfo))
      end
     -- print('-------info',illus_info_list:Get(illus_info_list.Count-1).roleInfo.heroData.id)
    elseif v.illustration_type == IllustrationType.equip then
      --装备图鉴
      if t.IllustrationEquipDataDictionary:Get(v.sheet) == nil then 
        t.IllustrationEquipDataDictionary:Add(v.sheet,MList.New('illustration_info'))
        temp_small_title_dic:Add(v.sheet,LocalizationController.instance:Get( v.sheet_name))
      end
      local illus_info_list = t.IllustrationEquipDataDictionary:Get(v.sheet)
      local equipInfo = equip_info.New(0,v.resData.id)
      illus_info_list:Add(illustration_info.NewByEquipInfo(v.id,equipInfo))
    else
      --道具图鉴
      if t.IllustrationItemDataDictionary:Get(v.sheet) == nil then 
        t.IllustrationItemDataDictionary:Add(v.sheet,MList.New('illustration_info'))
        temp_small_title_dic:Add(v.sheet,LocalizationController.instance:Get( v.sheet_name))
      end
      local illus_info_list = t.IllustrationItemDataDictionary:Get(v.sheet)
      local itemInfo = item_info.New(0,v.resData.id,0)
      illus_info_list:Add(illustration_info.NewByItemInfo(v.id,itemInfo))
    end
  end
 
end
--切换
function t.UpdateSelectDropdownDic(illustration_type,id)
  local limitCount = 0
  local dic = Dictionary.New('number','Mlist<illustration_info>>')
  if illustration_type == IllustrationType.hero then
    limitCount = 8
    if id == 0 then--全部
      dic = t.IllustrationRoleDataDictionary
    else
      for k,v in pairs(t.IllustrationRoleDataDictionary:GetDatas()) do --按类型
        if dic:Get(k) == nil then
          dic:Add(k,MList.New('illustration_info'))
        end
        local tempList = dic:Get(k)
        for k1,info in pairs (v:GetDatas()) do
         if info.roleInfo.heroData.roleType == id then
            tempList:Add(info)
          end
        end
      end
    end
    
  elseif illustration_type == IllustrationType.equip then
    limitCount = 5
    if id == 0 then--全部
      dic = t.IllustrationEquipDataDictionary
    else
      for k,v in pairs(t.IllustrationEquipDataDictionary:GetDatas()) do--按类型
        if dic:Get(k) == nil then
          dic:Add(k,MList.New('illustration_info'))
        end
        local tempList = dic:Get(k)
        for k1,info in pairs (v:GetDatas()) do
         if info.equipInfo.data.equipmentRoleType == id then
            tempList:Add(info)
          end
        end
        
      end
    end
    
  else
    limitCount = 5
    dic = t.IllustrationItemDataDictionary
  end
  
    t.currentSelectList:Clear()
    local useCount = 0
    local k,v
    for index,key in pairs(dic:GetSortKeysList()) do
      k = key
      v = dic:Get(key)
      
      useCount = 0
      local tempCount = 0
      local count = v.Count
      local innerSubList = nil
      if count ~= 0 then --有元素哟
        local smallTitle = t.GetSmallTitleStringDictionaryByIllustrationType(illustration_type,k)
        t.currentSelectList:Add(scroll_item_data.New( smallTitle,nil,nil,nil))
        local collectNum = 0 -- 图鉴中收集的个数
        local roleAttrDictionary = Dictionary.New('number','role_attr')--图鉴收集后加的属性
        local tempIllusInfo = nil
        local tempRoleAttr = nil
        
        while(true) do
          innerSubList = MList.New('illustration_info')
          tempCount = useCount + limitCount-1 
          if tempCount >= count then
            tempCount = count-1
          end
          for i = useCount,tempCount do
            tempIllusInfo = v:Get(i)
            innerSubList:Add(v:Get(i))
            
            if illustration_type == IllustrationType.hero then ---蛋疼的策划要加 收集英雄属性 
              if t.IsHeroGotInIllustration(tempIllusInfo.id) then
                collectNum = collectNum + 1
                local illusData = illustration_data.GetDataById(tempIllusInfo.illustrationDataId)
                for k,v in pairs(illusData.roleAttrDic) do
                  local tempRoleAttr = roleAttrDictionary:Get(k)
                  if tempRoleAttr then
                    tempRoleAttr.value = tempRoleAttr.value + v.value
                  else
                    roleAttrDictionary:Add(k,role_attr.New(k,v.value))
                  end
                end
              end
            end
            
          end
          useCount = useCount + limitCount
          if innerSubList.Count == 0 then
            break
          else
            t.currentSelectList:Add(scroll_item_data.New(nil, innerSubList,nil,nil))
          end
        end
        if illustration_type == IllustrationType.hero then ---蛋疼的策划要加 收集英雄属性 
          roleAttrDictionary:Remove(RoleAttributeType.MagicAtk)
          t.currentSelectList:Add(scroll_item_data.New( nil,nil,collectNum,roleAttrDictionary))
        end
      end
      
    end
end
--获取大页签(dropdown中)
function t.GetBigTitleStringDictionaryByIllustrationType(illustrationType)
  local titleDic = Dictionary.New('number','string')
  if illustrationType == IllustrationType.hero then
    titleDic:Add(0,'全部英雄')
    titleDic:Add(RoleType.Defence,'防御型')
    titleDic:Add(RoleType.Offence,'攻击型')
    titleDic:Add(RoleType.Mage,'魔法型')
    titleDic:Add(RoleType.Support,'辅助型')
    titleDic:Add(RoleType.Mighty,'全能型')
  elseif illustrationType == IllustrationType.equip then
    titleDic:Add(0,'全部类型')
    titleDic:Add(RoleType.Defence,'防御型')
    titleDic:Add(RoleType.Offence,'攻击型')
    titleDic:Add(RoleType.Mage,'魔法型')
    titleDic:Add(RoleType.Support,'辅助型')
    titleDic:Add(RoleType.Mighty,'全能型')
  else
    titleDic:Add(0,'全部类型')
  end
  return titleDic
  --return t.bigTitleStringDictionary:Get(illustrationType)
end
--获取小页签
function t.GetSmallTitleStringDictionaryByIllustrationType(illustrationType,id)
  return t.smallTitleStringDictionary:Get(illustrationType):Get(id)
end

function t.UpdateIllustrationAttr(heroId)
  local illusData = illustration_data.GetDataByHeroId(heroId)
  if illusData then
    t.power = t.power + illusData.comat
    for k,v in pairs(illusData.roleAttrDic) do
      local attr = t.IllustrationAddAttrDictionary:Get(k)
      if attr then
        attr.value = attr.value + v.value
      else
        t.IllustrationAddAttrDictionary:Add(k,role_attr.New(k,v.value))
      end
    end
  end
end

--------------------------------update by protcol--------------------------
function t.UpdateIllustrationList(heros,equips,items,clear)
  if clear then 
    t.IllustrationDictionary = {}
    t.IllustrationOldDictionary = {}
    t.IllustrationAddAttrDictionary:Clear()
    t.power = 0
  end
  local s = nil
  for k,v in ipairs(heros) do
    
    if not t.IsHeroGotInIllustration(v.heroNo) then
      t.UpdateIllustrationAttr(v.heroNo)
    end
    s = string.format('%d,%d',IllustrationType.hero,v.heroNo)
    t.IllustrationDictionary[s] = true
    if clear then 
       t.IllustrationOldDictionary[s] = true
    end
  end
  --print('装备图鉴total',#equips)
  for k,v in ipairs(equips) do
     s = string.format('%d,%d',IllustrationType.equip,v)
     
      t.IllustrationDictionary[s] = true
      --print('装备图鉴id',v)
  end
  --print('道具图鉴total',#items)
  for k,v in ipairs(items) do
     s = string.format('%d,%d',IllustrationType.item,v)
      t.IllustrationDictionary[s] = true
      --print('道具图鉴id',v)
  end

end
--是否在图鉴中
function t.IsGotInIllustration(illustrationType,modelId)
  if illustrationType == IllustrationType.hero then
    return t.IsHeroGotInIllustration(modelId)
  elseif illustrationType == IllustrationType.equip then
    return t.IsEquipGotInIllustration(modelId)
  else
    return t.IsItemGotInIllustration(modelId)
  end
end
--判断英雄是否再图鉴中
function t.IsHeroGotInIllustration(heroModelId)
  local k = string.format('%d,%d',IllustrationType.hero,heroModelId)
  
  return t.IllustrationDictionary[k]
end
function t.IsEquipGotInIllustration(equipId)
  local k = string.format('%d,%d',IllustrationType.equip,equipId,0)
  return t.IllustrationDictionary[k]
end
function t.IsItemGotInIllustration(itemId)
  local k = string.format('%d,%d',IllustrationType.item,itemId,0)
  return t.IllustrationDictionary[k]
end
--判断新英雄是否查看过
function t.IsHeroCheck(heroModelId)
  local k = string.format('%d,%d',IllustrationType.hero,heroModelId)
  local isCheck = t.IllustrationOldDictionary[k]
  if t.IsHeroGotInIllustration(heroModelId) then
    t.IllustrationOldDictionary[k] = true
  end
  if isCheck == nil then
    isCheck = false
  end
  return isCheck
end
---------------------from server update---------------------------

Start()
return t