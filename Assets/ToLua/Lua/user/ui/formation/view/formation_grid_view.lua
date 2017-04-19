local t = {}

local PREFAB_PATH = 'ui/formation/formation_grid_view'

--阵型选择英雄后回调，接受参数为阵型上的英雄id,formationIndex
t.selectRoleCallbackDelegate = void_delegate.New()
t.characters = {}

--根节点,队伍类型，主角是否可下阵
function t.Create(transformParent,formationTeamType,playerCanLeaveTeam)
  
  t.formationNormalSprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/bg_unit_02')
  t.formationOccupiedSprite = Common.ResMgr.ResMgr.instance:LoadSprite('sprite/main_ui/bg_unit_01')
  
  t.playerCanLeaveTeam = playerCanLeaveTeam
  t.formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam( formationTeamType)
  
  t.BindDelegate()
  
  local gameObject = Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH)
  t.transform = GameObject.Instantiate(gameObject):GetComponent(typeof(Transform))
  t.transform:SetParent(transformParent,false)
  
  t.InitComponent()
  t.RefreshView(formationTeamType)
end
---界面销毁时请手动调用 释放模型缓存
function t.Close()
  t.DespawnCharacter()
  t.UnbindDelegate()
end

function t.BindDelegate()
  gamemanager.GetModel('formation_model').FormationChangeDelegate:AddListener(t.Refresh)
end
function t.UnbindDelegate()
  gamemanager.GetModel('formation_model').FormationChangeDelegate:RemoveListener(t.Refresh)  
end

function t.InitComponent()
  t.btnFormationTable = {}
  t.transformModelRootTable = {}
  t.goIndicatorTable = {}
  t.goShadowTable = {}
  
  t.goSelectedIndicator = t.transform:Find('img_select_indicator').gameObject
  
  
  local root 
  for i = 1 , 9  do
    root = t.transform:Find('formation_'..i)
    t.btnFormationTable[i] = root:Find('formation_base_button'):GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    t.btnFormationTable[i].onClick:AddListener( t.ClickFormationBaseButtonHandler)
    t.transformModelRootTable[i] = root:Find('role_model_root')
    t.goIndicatorTable[i] = root:Find('indicator').gameObject
    t.goShadowTable[i] = root:Find('img_shadow').gameObject
  end
end   

function t.InitHeroModel()
  local gameModel = gamemanager.GetModel('game_model')
  local heroModel = gamemanager.GetModel('hero_model')
  t.DespawnCharacter()
  for i = 1,9 do
    local roleId = t.formationTeamInfo:GetInstacneIdByPosition(i)
      
    --ui_util.ClearChildren(t.transformModelRootTable[i],true)
    
    if roleId == 0 then  -- 无英雄
      t.goShadowTable[i]:SetActive(false)
      local sprite = t.btnFormationTable[i]:GetComponent(typeof(Image))
      if t.formationTeamInfo.formationInfo.formationData:GetPosEnable(i) then
        sprite.sprite = t.formationOccupiedSprite
      else 
        sprite.sprite = t.formationNormalSprite
      end
    else  -- 有英雄
      if gameModel.IsPlayer(roleId) then
        --need add player model
      local playerEntity = Logic.Character.CharacterEntity.CreatePlayerEntityAsUIElement(gameModel.playerInfo.instanceID, t.transformModelRootTable[i],false,false)
        t.characters[roleId] = playerEntity
      else 
        --need add hero model
        local heroInfo = heroModel.GetHeroInfo(roleId)
        local heroEntity = Logic.Character.CharacterEntity.CreateHeroEntityAsUIElementByHeroInfoLuaTable(heroInfo,t.transformModelRootTable[i],false,false)
        t.characters[roleId] = heroEntity
      end
        t.goShadowTable[i]:SetActive(true)
        t.btnFormationTable[i]:GetComponent(typeof(Image)).sprite = t.formationOccupiedSprite
    end
  end
end

function t.DespawnCharacter()
  for k,v in pairs(t.characters) do
    if(v ~= nil) then
      Logic.Pool.Controller.PoolController.instance:Despawn(v.name,v)
    end
  end
  t.characters = {}
end

function t.Refresh()
  t.SelectRole(0)
  t.InitHeroModel()
end
function t.RefreshView(formationTeamType)
  t.selectedRoleId = 0
  t.formationTeamInfo = gamemanager.GetModel('formation_model').GetFormationTeam( formationTeamType)
  --print('[RefreshView]formationTeamType:',formationTeamType)
  t.InitHeroModel()
  t.SelectRole(0)
end
function t.SelectRole(selectRoleId)
  
  local formationIndex = 0
  t.selectedRoleId = selectRoleId
  if selectRoleId == 0 then
    t:HideAllIndicators()
  else 
    t.ShowIndicators()
    if t.formationTeamInfo:IsHeroInFormation(selectRoleId) then
      formationIndex = t.formationTeamInfo:GetHeroFormationPosition(selectRoleId)
      local btnPosition = t.GetGridPosition(formationIndex)
      
      local selectIndicatorPos = t.goSelectedIndicator.transform.parent:InverseTransformPoint(btnPosition)
      selectIndicatorPos = Vector3(selectIndicatorPos.x,selectIndicatorPos.y + 230,-800)
      t.goSelectedIndicator.transform.localPosition = selectIndicatorPos
      t.goSelectedIndicator:SetActive(true)
      LeanTween.cancel(t.goSelectedIndicator)
      LeanTween.moveLocalY(t.goSelectedIndicator,selectIndicatorPos.y+30,0.6):setLoopPingPong()
    else 
      LeanTween.cancel(t.goSelectedIndicator)
      t.goSelectedIndicator:SetActive(false)
    end
  end
 -- print('[formation_grid_view]SelectRole selectRoleId:',selectRoleId,'formationIndex:',formationIndex)
    t.selectRoleCallbackDelegate:InvokeTwoParam(selectRoleId,formationIndex)
end
function t.GetGridPosition(formationIndex)
  return t.btnFormationTable[formationIndex].transform.position
end

function t.HideAllIndicators()
  local count = #t.goIndicatorTable
  for i = 1,count do
    t.goIndicatorTable[i]:SetActive(false)
  end
  t.goSelectedIndicator:SetActive(false)
end

function t.ShowIndicators()
  local count = #t.goIndicatorTable
  for i = 1,count do
    local canAdd = t.formationTeamInfo:CanAddToFormationPosition(i,t.selectedRoleId,t.playerCanLeaveTeam)
    t.goIndicatorTable[i]:SetActive(canAdd)
  end
end

---------------------click event-------------------
function t.ClickFormationBaseButtonHandler(gameObject)
  local pos = 0
  for k,v in ipairs(t.btnFormationTable) do
    if v.gameObject == gameObject then 
        pos = k
        break
    end
  end
  local isEmpty = t.formationTeamInfo:IsPositionEmpty(pos)
  local roleId = t.formationTeamInfo:GetInstacneIdByPosition(pos)
  if t.selectedRoleId ~= 0 then
    if isEmpty == false and roleId == t.selectedRoleId then
        t.SelectRole(0)
    else 
      if t.formationTeamInfo:CanAddToFormationPosition(pos,t.selectedRoleId,t.playerCanLeaveTeam) then
        t.formationTeamInfo:AddHeroToFormaiton(pos,t.selectedRoleId)
        ----need send Protocol to save team-------------------
        gamemanager.GetCtrl('formation_controller').TeamChangeReq(t.formationTeamInfo.teamType)
        --t.Refresh()
        
      end
    end
    
  else
    if isEmpty == false then t.SelectRole(roleId) end
    
  end

end

return t