local t={}
local name='explorectrl'

local function Start( ... )
	gamemanager.RegisterCtrl(name,t)
	netmanager.RegisterProtocol(MSG.ExploreListResp,t.ExploreListResp)
	netmanager.RegisterProtocol(MSG.ExploreTaskResp,t.ExploreTaskResp)
	netmanager.RegisterProtocol(MSG.ExploreTaskSynResp,t.ExploreTaskSynResp)
	netmanager.RegisterProtocol(MSG.ExploreTaskRewardResp,t.ExploreTaskRewardResp)
	netmanager.RegisterProtocol(MSG.ExploreTaskRefreshResp,t.ExploreTaskRefreshResp)
end

function CallExplore()
	if not gamemanager.GetModel('exploremodel').taskList then
		t.ExploreListReq()
	else
		if not t.view then
			t.view=dofile('ui/explore/view/exploreview')
		end
		t.view.ExploreOpen()
	end
end

function t.OpenExploreView()
	CallExplore()
end

function t.ExploreListRespFromModel()
	t.view=dofile('ui/explore/view/exploreview')
end

function t.ExploreTaskSynResp()
	if not explore_pb then	require 'explore_pb' end
	local pb=explore_pb.ExploreTaskProto()
	pb:ParseFromString(netmanager.GetProtocolData())
	
	gamemanager.GetModel('exploremodel').ExploreTaskSynResp(pb)
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.ExploreListResp()
	if not explore_pb then	require 'explore_pb' end
	local pb=explore_pb.ExploreListResp()
	pb:ParseFromString(netmanager.GetProtocolData())
	
	gamemanager.GetModel('exploremodel').ExploreListResp(pb)
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.ExploreTaskResp()
	gamemanager.GetModel('exploremodel').ExploreTaskResp()
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.ExploreTaskRewardResp()
	gamemanager.GetModel('exploremodel').ExploreTaskRewardResp(t.lastRequestExploreRewardTaskID)
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.ExploreTaskRefreshResp()
	if not explore_pb then	require 'explore_pb' end
	local pb=explore_pb.ExploreTaskRefreshResp()
	pb:ParseFromString(netmanager.GetProtocolData())

	gamemanager.GetModel('exploremodel').ExploreTaskRefreshResp(pb)
  Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.ExploreTaskReq(id,heros)
	if not explore_pb then	require 'explore_pb' end
	local pb=explore_pb.ExploreTaskReq()
	pb.id=id
	for i=1,#heros do
		pb.heros:append(heros[i])
	end
	netmanager.SendProtocol(MSG.ExploreTaskReq,pb)
end

function t.ExploreTaskSynReq(id)
	if not common_pb then	require 'common_pb' end
	local pb=common_pb.IntProto()
	pb.value=id
	
	netmanager.SendProtocol(MSG.ExploreTaskSynReq,pb)
end

t.lastRequestExploreRewardTaskID = -1
function t.ExploreTaskRewardReq(id)
  t.lastRequestExploreRewardTaskID = id
	if not common_pb then	require 'common_pb' end
	local pb=common_pb.IntProto()
	pb.value=id

	netmanager.SendProtocol(MSG.ExploreTaskRewardReq,pb)
end

function t.ExploreListReq()
	netmanager.SendProtocol(MSG.ExploreListReq)
end

function t.ExploreTaskRefreshReq()
	netmanager.SendProtocol(MSG.ExploreTaskRefreshReq)
end

Start()
return t