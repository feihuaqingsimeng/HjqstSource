local mine_model={}
local name = 'mine_model'
mine_model.mineData = gamemanager.GetData('mine_data')
mine_model.m_selfMineInfo = dofile('ui/mine/model/self_mine_info')
mine_model.m_aryMineItems ={}
mine_model.m_aryMineRoles ={}
mine_model.m_playerIconId = nil
mine_model.m_curMineInfo = nil
mine_model.m_curRolePlayerInfo =nil
mine_model.m_curAryRoleHeroInfo =nil
mine_model.m_curRoleProtectTime =nil
mine_model.m_curRoleGold =nil
mine_model.m_curFormationNo =nil
mine_model.m_curEnemyRoleInfo = nil
mine_model.m_enemyTeamInfo = {}
mine_model.m_selfTeamInfo = {}
mine_model.m_isHaveLog = false

mine_model.OnMapUpdateDelegate = void_delegate.New()
mine_model.OnOtherMineUpdataDelegate = void_delegate.New()
mine_model.OnUpdateSelfMineDelegate = void_delegate.New()
mine_model.OnSelfMineCloseDelegate = void_delegate.New()
mine_model.OnOtherMineCloseDelegate = void_delegate.New()
mine_model.OnRoleMineUpdateDelegate = void_delegate.New()
mine_model.OnRedPointUpdateDelegate = void_delegate.New()

local function Start ()  
  gamemanager.RegisterModel(name,mine_model)
end

function mine_model.SetOwnMineInfo(occTime,plunderTime,ownMineNo,endTime,inCome)
  print("ownMineNo=============",ownMineNo)
  mine_model.m_selfMineInfo.occTime = occTime
  mine_model.m_selfMineInfo.plunderTime = plunderTime
  mine_model.m_selfMineInfo.ownMineNo = ownMineNo
  mine_model.m_selfMineInfo.endTime =math.floor(endTime/1000)
  mine_model.m_selfMineInfo.award = inCome
end

function mine_model.AddMineItemList(mineItemInfo)
  if mineItemInfo== nil then
      do return end
  end
  mine_model.m_aryMineItems[mineItemInfo.mineNo] = mineItemInfo
end
function mine_model.AddMineRoleList(roleInfo)
  if roleInfo== nil then
      do return end
  end
  mine_model.m_aryMineRoles[roleInfo.roleId] = roleInfo
end

function mine_model.SetPlayerIcon(iconId)
  mine_model.m_playerIconId = iconId
end

function mine_model.RefreshMineMap()
  mine_model.OnMapUpdateDelegate:Invoke()
  
end

function mine_model.RefreshOtherMine()
  mine_model.OnOtherMineUpdataDelegate:Invoke() 
end

function mine_model.RefreshSelfMine()
  mine_model.OnUpdateSelfMineDelegate:Invoke()
end
function mine_model.CloseSelfMine()
  mine_model.OnSelfMineCloseDelegate:Invoke()
end
function mine_model.CloseOtherMine()
  mine_model.OnOtherMineCloseDelegate:Invoke()
end
function mine_model.RefreshOtherRole()
  mine_model.OnRoleMineUpdateDelegate:Invoke()
end
function mine_model.UpdateRedPoint(state)
  mine_model.m_isHaveLog = state
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_MineBattle)
  if not state then
      do return end
  end
  mine_model.OnRedPointUpdateDelegate:Invoke()
end


function mine_model.MineConpare(a,b)
   return a.mineNo> b.mineNo
end
function mine_model.GetMineData(mineId)
    return mine_model.mineData.GetDataById(mineId)
end

--点击查看取到当前数据
function mine_model.GetCurMineInfo(mineInfo)
  print("mineInfo==============",mineInfo.ownMineNo)
  return mine_model.m_aryMineItems[mineInfo.ownMineNo]
end
Start()
return mine_model
