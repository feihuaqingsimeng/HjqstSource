local t = {}
local name = 'sign_in_model'

t.lastSignInId = 0 --上次签到id
t.curSignInId = 0 -- 当前可签到id
t.isSignInToday = false --今天是否已签到
--签到更新
t.onSignInUpdateDelegate = void_delegate.New()

local function Start ()
  gamemanager.RegisterModel(name, t)
end

--是否签到
function t.SetIsSign(lastSignInNo,curSignInId,isSign)
  t.isSignInToday = isSign
  t.lastSignInId = lastSignInNo
  t.curSignInId = curSignInId
  t.onSignInUpdateDelegate:Invoke()
  print('---------签到---------------',t.isSignInToday,t.lastSignInId)
  gamemanager.GetModel('red_point_model').RefreshSpecific(RedPointType.RedPoint_SignIn)
end

Start()
return t