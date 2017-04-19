gamemanager={}

local function Start()
	require('enum/enum')
  Dictionary = require('system/Dictionary')
  ArrayList = require('system/ArrayList')
  MList = require('system/MList')
  lua_json = require('system/lua_json')
	require('res_path')
	ui_util = require('util/ui_util')
	particle_util = require('util/particle_util')
	void_delegate = require('delegate/void_delegate')
	gamemanager.path=dofile('common/datapath')

	dofile('ui/function_open/model/function_open_model')

	dofile('message')

	dofile('ui/game/model/game_model')
	dofile('ui/game/controller/game_controller')  

	dofile('ui/hero/controller/hero_controller')
	dofile('ui/hero/model/hero_model')

	dofile('ui/equip/controller/equip_controller')
	dofile('ui/equip/model/equip_model')

	dofile('ui/explore/controller/explorectrl')
	dofile('ui/explore/model/exploremodel')  

	dofile('ui/illustration/controller/illustration_ctrl')
	dofile('ui/illustration/model/illustration_model')

	dofile('ui/arena/controller/arena_controller')
	dofile('ui/arena/model/arena_model')

	dofile('ui/formation/model/formation_model')
	dofile('ui/formation/controller/formation_controller')
  

  dofile('ui/player/model/player_model')
	dofile('ui/player/controller/player_controller')

  dofile('ui/consortia/model/consortia_model')
	dofile('ui/consortia/controller/consortia_controller')

	dofile('ui/role/model/role_model')
	dofile('ui/item/controller/item_controller')
	dofile('ui/item/model/item_model')
	
	dofile('ui/ranking/controller/ranking_controller')
	dofile('ui/ranking/model/ranking_model')

	dofile('ui/shop/model/shop_model')
	dofile('ui/shop/controller/shop_controller')

	dofile('ui/vip/model/vip_model')
	dofile('ui/vip/controller/vip_controller')

	dofile('ui/chat/model/chat_model')
	dofile('ui/chat/controller/chat_controller')
  
  dofile('ui/activity/model/activity_model')
	dofile('ui/activity/controller/activity_controller')	
  
	dofile('ui/black_market/controller/black_market_controller')
	dofile('ui/black_market/model/black_market_model')


	dofile('ui/consume_tip/model/consume_tip_model')
	--friend
	dofile('ui/friend/controller/friend_controller')
	dofile('ui/friend/model/friend_model')
  
  -- [[ dungeon ]] --
  dofile('ui/dungeon/model/dungeon_model')
  dofile('ui/dungeon/controller/dungeon_controller')

	-- [[ chapter ]] --
	dofile('ui/chapter/model/chapter_model')
	dofile('ui/chapter/controller/chapter_controller')
  
  dofile('ui/fight_result/model/fight_result_model')
  
  dofile('ui/red_point/model/red_point_model')
  
  --login
  dofile('ui/login/controller/login_controller')
  --expedition
  dofile('ui/expedition/model/expedition_model')
  dofile('ui/expedition/controller/expedition_controller')
  --pve embattle
  dofile('ui/pve_embattle/controller/pve_embattle_controller')
  --task
  dofile('ui/task/model/task_model')
  
  --pack
  dofile('ui/pack/controller/pack_controller')
  --世界boss
  dofile('ui/world_boss/model/world_boss_model')
  --在线礼包
  dofile('ui/online_gift/model/online_gift_model')
  dofile('ui/online_gift/controller/online_gift_controller')
  
  --矿战
  dofile('ui/mine/model/mine_model')
  dofile('ui/mine/controller/mine_controller')
  
  --装备合成
  dofile('ui/compose/model/compose_model')
  dofile('ui/compose/controller/compose_controller')
 --sign in
  dofile('ui/signIn/model/sign_in_model')
 --tip model
 dofile('ui/tips/model/tips_model')
 --点金手
 dofile('ui/golden_touch/controller/golden_touch_controller')
 dofile('ui/golden_touch/model/golden_touch_model')
 --声音
 dofile('ui/audio/model/audio_model')
 --转盘
 dofile('ui/activity/model/turntable/turntable_model')
 --玩家手册
 dofile('ui/player_manual/controller/player_manual_controller')
  --preload
  gamemanager.GetData('hero_data')
  gamemanager.GetData('item_data')
  gamemanager.GetData('formation_data')
  gamemanager.GetData('equip_data')
  gamemanager.GetData('player_data')
  gamemanager.GetData('global_data')
  gamemanager.GetData('function_open_data')
  gamemanager.GetData('team_data')
  gamemanager.GetData('dungeon_data')
end

function gamemanager.GetData(name)
	if not gamemanager[name] then 
		if not gamemanager.path[name] then
			print('not find datapath:'..name)
		end
		gamemanager[name]=dofile(gamemanager.path[name])
	end
	return gamemanager[name]
end


function gamemanager.GetModel(name)
	if gamemanager[name] then
		return gamemanager[name]
	else
		Debugger.LogError('can not get model: '..name)
		return nil
	end
end

function gamemanager.RegisterModel(name,model)
	if not gamemanager[name] then
		gamemanager[name]=model
    print('RegisterModel',name)
	else
		print('repeat register:'..name) 
	end
  
end

function gamemanager.GetCtrl(name)
	if gamemanager[name] then
		return gamemanager[name]
	else
		Debugger.LogError('can not get ctrl: '..name)
		return nil
	end
end

function gamemanager.RegisterCtrl(name,ctrl)
	if not gamemanager[name] then
		gamemanager[name]=ctrl
    print('RegisterCtrl:'..name) 
	else
		print('repeat register:'..name) 
	end
end

function gamemanager.gc()
  local size1 = collectgarbage("count")
  print('current momery size:'..tostring(size1))
  collectgarbage("collect")
  local size2 = collectgarbage("count")
  print('after gc momery size:'..tostring(size2),'diff:'..tostring(size1 - size2))
end

function gamemanager.ZeroInvoke()  
  print('zero invoke')
  --请求在线礼包
   local online_gift_controller = gamemanager.GetCtrl('online_gift_controller')
   online_gift_controller.OnlineGiftSynReq()
end
Start()
