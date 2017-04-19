local datapath={}

local function Start( ... )
  AddDataPath('equip_data','ui/equip/model/')
  AddDataPath('equip_attr_data','ui/equip/model/')
  AddDataPath('equip_strengthen_data','ui/equip/model/')
  AddDataPath('illustration_data','ui/illustration/model/') 
  
  AddDataPath('formation_data','ui/formation/model/')
  AddDataPath('formation_attr_data','ui/formation/model/')
 
  AddDataPath('account_exp_data','ui/game/model/')
  AddDataPath('player_data','ui/player/model/')
  AddDataPath('pet_data','ui/player/model/')
  AddDataPath('pet_talent_data','ui/player/model/')
  
  AddDataPath('hero_data','ui/hero/model/')
  AddDataPath('breakthrough_data','ui/hero/model/')
  AddDataPath('hero_strengthen_data','ui/hero/model/')
  AddDataPath('hero_strengthen_need_data','ui/hero/model/')
  AddDataPath('hero_strengthen_provide_data','ui/hero/model/')
  AddDataPath('hero_advance_data','ui/hero/model/')
  AddDataPath('hero_exp_data','ui/hero/model/')
  AddDataPath('hero_compose_data','ui/hero/model/')
  AddDataPath('hero_decomposition_data', 'ui/hero/model/')
  AddDataPath('hero_relationship_data','ui/hero/model/')
  AddDataPath('aggr_extra_exp_data','ui/hero/model/')

  AddDataPath('skill_data','ui/skill/model/')
  AddDataPath('mechanics_data','ui/skill/model/')
  
  AddDataPath('global_data','ui/game/model/')
  
  AddDataPath('item_data','ui/item/model/')
  AddDataPath('piece_data','ui/item/model/')
  
  AddDataPath('package_data','ui/pack/model/')
  AddDataPath('gem_attr_data','ui/item/model/')
  AddDataPath('gem_synthesis_data','ui/item/model/')
  
  AddDataPath('shop_card_random_data', 'ui/shop/model/data/')
  AddDataPath('shop_diamond_data', 'ui/shop/model/data/')
  AddDataPath('shop_goods_data', 'ui/shop/model/data/')
  AddDataPath('shop_limit_item_data', 'ui/shop/model/data/')
  
  AddDataPath('vip_data', 'ui/vip/model/')
  
  AddDataPath('function_open_data', 'ui/function_open/model/')
  
  AddDataPath('black_market_data', 'ui/black_market/model/')
  AddDataPath('black_market_rule_data', 'ui/black_market/model/')
 
  AddDataPath('consume_tip_data', 'ui/consume_tip/model/')

  AddDataPath('team_data', 'ui/team/model/')
  AddDataPath('chapter_data', 'ui/chapter/model/')
  AddDataPath('dungeon_star_data', 'ui/chapter/model/')
  AddDataPath('dungeon_data', 'ui/dungeon/model/')
  
  --公会
  AddDataPath('consortia_data', 'ui/consortia/model/')
  AddDataPath('consortia_shop_data', 'ui/consortia/model/')
  AddDataPath('consortia_rank_reward_data','ui/consortia/model/')
  --远征
  AddDataPath('expedition_data', 'ui/expedition/model/')
  --任务
  AddDataPath('task_condition_data', 'ui/task/model/')
  AddDataPath('task_data', 'ui/task/model/')
  
  --event
  AddDataPath('event_data', 'ui/activity/model/')
  AddDataPath('seven_hilarity_data', 'ui/activity/model/')
  
  AddDataPath('online_gift_data','ui/online_gift/model/')
  
  AddDataPath('drop_message_data', 'ui/goods_jump/model/')
  --矿战
  AddDataPath('mine_data','ui/mine/model/')
  --装备合成
  AddDataPath('equip_compose_data','ui/compose/model/')
  AddDataPath('const_data','ui/game/model/')
  --装备分解
  AddDataPath('equip_decompose_data','ui/equip/model/')
  --点金手
  AddDataPath('golden_touch_data','ui/golden_touch/model/')
  --英雄点击音效
  AddDataPath('daily_audio_data','ui/audio/model/')
  --转盘
  AddDataPath('turntable_data','ui/activity/model/turntable/')
  --广播
  AddDataPath('broadcast_data','ui/chat/model/')
end

function AddDataPath(name,path)
    if not datapath[name] then
        datapath[name]=path..name
    else
        print('repeat datapath:'..name)
    end
end

Start()
return datapath