local t = {}
local name = 'red_point_model'

local game_model = gamemanager.GetModel('game_model')
local formation_model = gamemanager.GetModel('formation_model')
local item_model = gamemanager.GetModel('item_model')
local player_model = gamemanager.GetModel('player_model')
local hero_model = gamemanager.GetModel('hero_model')

local function Start()
  gamemanager.RegisterModel(name,t)
end
--所有红点刷新
function t.Refresh()
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end
--指定红点刷新
function t.RefreshSpecific(redPointType)

  Observers.Facade.Instance:SendNotification('OnRedPointChange',redPointType)
end

function t.RefreshCountByCSharp(redPointTypes,id)  
  if redPointTypes == nil then
    return false
  end

  local length = redPointTypes.Length
  if length == 0 then
    return false
  end
  local heroModel = gamemanager.GetModel('hero_model')
  for i = 0,length-1 do
    local redPointType = redPointTypes[i]
    local hasRed = false
    if redPointType == RedPointType.RedPoint_Hero_New then --//获得新英雄提示
      hasRed = heroModel.HasNewHero()
    elseif redPointType == RedPointType.RedPoint_Consortia_apply then-- //公会申请提示
      hasRed = gamemanager.GetModel('consortia_model').hasGuildApply
    elseif redPointType == RedPointType.RedPoint_HeroAdvance then--单个英雄进阶提示
      if heroModel.advanceHeroDic:ContainsKey(id) then
        hasRed = true
      end
    elseif redPointType == RedPointType.RedPoint_HeroBreakthrough then--单个英雄突破提示
      if heroModel.breakthroughHeroDic:ContainsKey(id) then
        hasRed = true
      end
    elseif redPointType == RedPointType.RedPoint_All_Hero_advance_breakthrough then--所有英雄进阶和突破提示
      if heroModel.advanceHeroDic.Count > 0 or heroModel.breakthroughHeroDic.Count > 0 then
        hasRed = true
      end
    elseif redPointType == RedPointType.RedPoint_Formation_specific then
      hasRed = gamemanager.GetModel('formation_model').GetNewFormationTipByRedPoint(id)
    elseif redPointType == RedPointType.RedPoint_Formation then --阵型二级界面提示
      hasRed = gamemanager.GetModel('formation_model').GetNewFormationAndPointFullTipByRedPoint()
    elseif redPointType == RedPointType.RedPoint_New_Equip then
      hasRed = gamemanager.GetModel('equip_model').HasNewEquipment()
    elseif redPointType == RedPointType.RedPoint_SignIn then --签到
      hasRed = gamemanager.GetModel('function_open_model').IsFunctionOpen(FunctionOpenType.MainView_SignIn) and not gamemanager.GetModel('game_model').isSignIn
    elseif redPointType == RedPointType.RedPoint_New_Item then --新类型道具
      hasRed = gamemanager.GetModel('item_model').HasNewItem()
    elseif redPointType == RedPointType.RedPoint_HeroHasFitEquipment then
      local roleInfo = nil
      if gamemanager.GetModel('game_model').IsPlayer(id) then
        roleInfo = gamemanager.GetModel('game_model').playerInfo
      else
        roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(id)
      end
      hasRed = gamemanager.GetModel('formation_model').IsHeroInAnyPVETeam(id) and gamemanager.GetModel('equip_model').HasFitEquipment(roleInfo, nil)
    elseif redPointType == RedPointType.RedPoint_AnyInFormationHeroHasFitEquipment then ---上阵英雄有可穿戴装备提示
      hasRed = gamemanager.GetModel('formation_model').IsAnyInFormationHeroHasFitEquipment()
    elseif redPointType == RedPointType.RedPoint_hero_relationship then--单个阵上英雄可羁绊提示
      if heroModel.relationshipHeroDic:ContainsKey(id) then
        hasRed = true
      end
    elseif redPointType == RedPointType.RedPoint_all_hero_relationship then  --所有阵上英雄可羁绊提示
      if heroModel.relationshipHeroDic.Count > 0 then
        hasRed = true
      end
    elseif redPointType == RedPointType.RedPoint_Small_Exp_Potion then
      --[[
      local roleInfo = nil
      if gamemanager.GetModel('game_model').IsPlayer(id) then
        roleInfo = gamemanager.GetModel('game_model').playerInfo
      else
        roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(id)
      end
      
      hasRed = (roleInfo ~= nil)
      and (formation_model.IsHeroInAnyPVETeam(roleInfo.instanceID))
      and (roleInfo.level < roleInfo:MaxLevel())
      and (roleInfo.level < game_model.accountLevel)
      and (item_model.GetItemCountByItemID(10600) > 0)
      ]]--
      hasRed = false
    elseif redPointType == RedPointType.RedPoint_Middle_Exp_Potion then
      --[[
      local roleInfo = nil
      if gamemanager.GetModel('game_model').IsPlayer(id) then
        roleInfo = gamemanager.GetModel('game_model').playerInfo
      else
        roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(id)
      end
      
      hasRed = (roleInfo ~= nil)
      and (formation_model.IsHeroInAnyPVETeam(roleInfo.instanceID))
      and (roleInfo.level < roleInfo:MaxLevel())
      and (roleInfo.level < game_model.accountLevel)
      and (item_model.GetItemCountByItemID(10601) > 0)
      ]]--
      hasRed = false
    elseif redPointType == RedPointType.RedPoint_Big_Exp_Potion then
      --[[
      local roleInfo = nil
      if gamemanager.GetModel('game_model').IsPlayer(id) then
        roleInfo = gamemanager.GetModel('game_model').playerInfo
      else
        roleInfo = gamemanager.GetModel('hero_model').GetHeroInfo(id)
      end
      
      hasRed = (roleInfo ~= nil)
      and (formation_model.IsHeroInAnyPVETeam(roleInfo.instanceID))
      and (roleInfo.level < roleInfo:MaxLevel())
      and (roleInfo.level < game_model.accountLevel)
      and (item_model.GetItemCountByItemID(10602) > 0)
      ]]--
      hasRed = false
    elseif redPointType == RedPointType.RedPoint_MineBattle then 
      hasRed = gamemanager.GetModel('mine_model').m_isHaveLog
    elseif redPointType == RedPointType.RedPoint_Activity then
      hasRed = gamemanager.GetModel('activity_model'):IsActivityCanGetReward()
    elseif redPointType == RedPointType.RedPoint_PVP_Challenge_Reward then    --竞技场挑战奖励提示
      hasRed = gamemanager.GetModel('arena_model').canUseWinTimes >= gamemanager.GetData('global_data').arena_winning_streak_num
    elseif redPointType == RedPointType.RedPoint_Seven_Hilarity then
      if id ~= 0 then
        hasRed = true
        print("-----------------------------RedPoint_Seven_Hilarity---------------------------- ")
      end
    elseif redPointType == RedPointType.RedPoint_In_Pve_Formation_Hero_Can_Use_Exp_Potion then
      --[[
      local inPveTeamHeroIdList = formation_model.GetAnyPveTeamHeroIdList()
      for k, v in pairs(inPveTeamHeroIdList) do
        local roleInfo = nil
        if player_model.IsPlayerInstanceID(v) then
          roleInfo = player_model.GetPlayerInfo(v)
        else
          roleInfo = hero_model.GetHeroInfo(v)
        end
        
        if (roleInfo ~= nil) and (roleInfo.level < roleInfo:MaxLevel()) and (item_model.GetItemCountByItemID(10600) > 0 or item_model.GetItemCountByItemID(10601) > 0 or item_model.GetItemCountByItemID(10602) > 0) then
          hasRed = true
          break
        end
      end
      ]]--
      hasRed = false
    else
      
    end
   -- print('hasRed:',hasRed,'redPointType:',redPointType,'id:',id)
    if hasRed then
      return true
    end
  end
  --Debugger.LogError('[red_point_model][RefreshCountByCSharp]type:'..redPointType..',id:'..id)

  return false
end


------------update by protocol-------------
function t.UpdateRedPointByProtocol(functionOpenType,subIndex)
  print("functionOpenType", functionOpenType)
  if functionOpenType == FunctionOpenType.MainView_Consortia then
    gamemanager.GetModel('consortia_model').hasGuildApply = true
  elseif functionOpenType == FunctionOpenType.FightCenter_MineBattle then
    gamemanager.GetModel('mine_model').UpdateRedPoint(true)
  end
end

Start()
return t