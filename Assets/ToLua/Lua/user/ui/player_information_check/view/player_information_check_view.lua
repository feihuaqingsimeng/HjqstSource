local t = {}
local PREFAB_PATH = 'ui/player_information_check/player_information_check_view'
local name = PREFAB_PATH


local global_data = gamemanager.GetData('global_data')
local hero_data = gamemanager.GetData('hero_data')

--参数playerInfomationInfo:玩家信息[参考player_infomation_info.lua]
function t.Open(playerInfomationInfo)
  
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Replace)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.characters = {}
  t.playerInfomationInfo = playerInfomationInfo
  t.heroInstanceIDList = {} --界面站位index,id(注意：主角位置已调整到正中间，所以阵型站位请不要使用me)
  t.InitComponent()
  t.Refresh()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
  t.DespawnCharacter()
end

function t.InitComponent()
  local formationRoot = t.transform:Find('core/formation')
  t.combatCapabilityText = t.transform:Find('core/power_root/text_power'):GetComponent(typeof(Text))
  t.formationPositionTable = {}
  for i = 1,5 do
    local table = {}
    table.root = formationRoot:Find(i)
    table.modelRoot = table.root:Find('hero_model_root')
    table.titleRoot = table.root:Find('common_role_title')
    table.textLevel = table.titleRoot:Find('text_level'):GetComponent(typeof(Text))
    table.textName = table.titleRoot:Find('text_name'):GetComponent(typeof(Text))
    table.imgRoleType = table.titleRoot:Find('img_role_type_icon'):GetComponent(typeof(Image))
    table.goStarTable = {}
    for k = 1,6 do
      table.goStarTable[k] = table.titleRoot:Find('stars_root/img_star_'..k).gameObject
    end
    table.triggerCheckInfo = table.root:Find('btn_check'):GetComponent(typeof(EventTriggerDelegate))
    table.triggerCheckInfo.onClick:AddListener(t.ClickCheckRoleInfoHandler)
    t.formationPositionTable[i] = table
    
  end
  --pos
  t.positionTable = {}
  for i = 1,5 do
    t.positionTable[i] = t.transform:Find('core/pos_root/pos'..i).localPosition
  end
  local btnClose = t.transform:Find("core/btn_back"):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.ClickCloseBtnHandler)
end

t.RefreshHeroModelsDelayCoroutine = nil
function t.Refresh()
  
  t.combatCapabilityText.text = t.playerInfomationInfo:Power()
  
  coroutine.stop(t.RefreshHeroModelsDelayCoroutine)
  t.RefreshHeroModelsDelayCoroutine = coroutine.start(t.RefreshHeroModelsDelay)
end
--刷新英雄模型
function t.RefreshHeroModelsDelay()
  coroutine.wait(0.1)
  t.DespawnCharacter()
  local teamPosDictionary = t.playerInfomationInfo:GetTeamPosDictionary()
  t.heroInstanceIDList = {}
  local index = 1
  local playerIndex = 0
  for k,v in pairs(teamPosDictionary:GetDatas()) do
    t.heroInstanceIDList[index] = v
    if t.playerInfomationInfo:GetRoleCheckInfo(v):IsPlayer() then
      playerIndex = index
    end
    index = index + 1
  end
 -- 主角要一直站中间
  if index ~= 0 then
    local temp = t.heroInstanceIDList[1]
    t.heroInstanceIDList[1] = t.heroInstanceIDList[playerIndex]
    t.heroInstanceIDList[playerIndex] = temp
  end
  local id
  for k,v in pairs(t.formationPositionTable) do
    
    id = t.heroInstanceIDList[k]
    if id and id ~= 0 then
      v.root.gameObject:SetActive(true)
      v.titleRoot.gameObject:SetActive(true)
      local roleInfo = t.playerInfomationInfo:GetRoleCheckInfo(id).roleInfo
      if roleInfo.heroData:IsPlayer() then
        t.CreatePlayerEntityAsUIElement(roleInfo,v.modelRoot)
        v.modelRoot.localRotation = Quaternion.Euler(roleInfo.heroData.home_rotation)
        t.RefreshModelInfo(roleInfo,v)
      else
        t.CreateHeroEntityAsUIElement(roleInfo,v.modelRoot)
        v.modelRoot.localRotation = Quaternion.Euler(roleInfo.heroData.home_rotation)
        t.RefreshModelInfo(roleInfo,v)
      end
    else
      v.root.gameObject:SetActive(false)
    end
  end
  
end

function t.CreatePlayerEntityAsUIElement(playerInfo,parent)
  local playerEntity = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElementByPlayerInfoLuaTable(playerInfo, parent,false,false)
  t.characters[playerInfo.instanceID] = playerEntity
end
--
function t.CreateHeroEntityAsUIElement(heroInfo,parent)
  local heroEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(heroInfo,parent,false,false)
  t.characters[heroInfo.instanceID] = heroEntity
end
--刷新模型属性
function t.RefreshModelInfo(roleInfo,formationPosRoot)
  local table = formationPosRoot
    table.textLevel.text = roleInfo.level
    table.textName.text = roleInfo.heroData:GetNameWithQualityColor()
    table.imgRoleType.sprite = ui_util.GetRoleTypeBigIconSprite(roleInfo.heroData.roleType)
    for k ,v in pairs(table.goStarTable) do
        v:SetActive(k <= roleInfo.advanceLevel)
    end
end

function t.DespawnCharacter()
  for k,v in pairs(t.characters) do
    if(v ~= nil) then
      PoolController.instance:Despawn(v.name,v)
    end
  end
  t.characters = {}
end
----------------------------click event-------------------------
function t.ClickCloseBtnHandler()
  t.Close()
end
--英雄信息面板
function t.ClickCheckRoleInfoHandler(go)
  local index = -1
  for k, v in pairs(t.formationPositionTable) do
    if v.triggerCheckInfo.gameObject == go then
      index = k
      break
    end
  end
  if index ~= -1 then
    local id = t.heroInstanceIDList[index]
    local roleCheckInfo = t.playerInfomationInfo:GetRoleCheckInfo(id) 
    AnimatorUtil.CrossFade(t.characters[id].anim,AnimatorUtil.VICOTRY_ID,0.3)
    local view = dofile('ui/player_information_check/view/role_check_info_view')
    view.Open(roleCheckInfo,t.positionTable[index])
  end
  
end


return t