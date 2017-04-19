local t = {}
local name = 'illustration_ctrl'

t.InitIllustrationDelegate = void_delegate.New()
t.UpdateIllustrationDelegate = void_delegate.New()
t.UpdatePowerDelegate = void_delegate.New()

local function Start()
  dofile('ui/illustration/protocol/illustration_protocol')
  gamemanager.RegisterCtrl(name,t)
end

--req 请求图鉴信息
function t.IllustrationReq()
  if not pack_pb then require('pack_pb')
  end
  local pb = pack_pb.IllustrationReq()
  netmanager.SendProtocol(MSG.IllustrationReq,pb)
end


--resp 响应图鉴信息
function t.IllustrationRespHandler(pb)
  t.GetIllustrationModel().UpdateIllustrationList(pb.heros,pb.equips,pb.items,true)
  t.InitIllustrationDelegate:Invoke()
  t.UpdatePowerDelegate:Invoke()
  Observers.Facade.Instance:SendNotification('IllustrationResp')
end
--同步图鉴信息
function t.IllustrationSynRespHandler(pb)
  t.GetIllustrationModel().UpdateIllustrationList(pb.newHeros,pb.equips,pb.items,false)
  t.UpdateIllustrationDelegate:Invoke()
  t.UpdatePowerDelegate:Invoke()
end

function t.GetIllustrationModel()
  return gamemanager.GetModel('illustration_model')
end
-------------------------open view------------------------
--打开图鉴界面
function t.OpenIllustrationView(useSaveState)
  --LuaCsTransfer.OpenIllustrationView()
  local view = dofile('ui/illustration/view/illustration_view')
  view.Open(useSaveState)
end
--打开英雄图鉴详细界面(如果roleInfo为nil 则打开前次打开过的英雄)
function t.OpenIllustrationHeroDetailView(roleInfo)
  local view = dofile('ui/illustration/view/illustration_role_detail_view')
  if roleInfo == nil then
    roleInfo = gamemanager.GetModel('illustration_model').selectRoleInfo
  end
  view.Open(roleInfo)
end

------打开图鉴高形态展示
function t.OpenIllustrationHeroHighStatusView(roleInfo)
  local view = dofile('ui/illustration/view/high_status/illustration_highstatus_view')
  view.Open(roleInfo)
end
---------打开图鉴羁绊
function t.OpenRelationShipView(heroDataId)
  local view = dofile('ui/illustration/view/relationship/relationship_view')
  view.Open(heroDataId)
end
Start()
return t
