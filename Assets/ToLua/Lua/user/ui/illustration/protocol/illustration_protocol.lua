local t = {}
local name = 'illustration_protocol'

local function Start()
  netmanager.RegisterProtocol(MSG.IllustrationResp,t.IllustrationRespHandler)
  netmanager.RegisterProtocol(MSG.IllustrationSynResp,t.IllustrationSynRespHandler)
end


function t.IllustrationRespHandler()
  print('IllustrationRespHandler')
  t.CheckPb()
  local pb = pack_pb.IllustrationResp()
  pb:ParseFromString(netmanager.GetProtocolData())
  t.GetIllustrationCtrl().IllustrationRespHandler(pb)
end

function t.IllustrationSynRespHandler()
  print('IllustrationSynRespHandler')
  t.CheckPb()
  local pb = pack_pb.IllustrationSynResp()
  pb:ParseFromString(netmanager.GetProtocolData())
  t.GetIllustrationCtrl().IllustrationSynRespHandler(pb)
end

function t.CheckPb()
  if not pack_pb then 
    require('pack_pb')
  end
end
function t.GetIllustrationCtrl()
  return gamemanager.GetCtrl('illustration_ctrl')
end


Start()
return t