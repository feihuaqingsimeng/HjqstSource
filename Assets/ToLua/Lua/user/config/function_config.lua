-- AutoGenerate Do not Edit!
local t = {}
local indexs={
'id','name','player_level','dungeon_pass','task_get','task_finish','notice','vip','show_main_position','show_sheet_position'
}

t.t = {
	[10]={'10','商店按钮','1','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_shop',nil},
	[11]={'11','商店-英雄','1','0','0','0','0','0',nil,nil},
	[12]={'12','商店-装备','1','0','0','0','0','0',nil,nil},
	[13]={'13','商店-钻石','1','0','0','0','0','0',nil,nil},
	[14]={'14','商店-行动力','1','0','0','0','0','0',nil,nil},
	[15]={'15','商店-金币','1','0','0','0','0','0',nil,nil},
	[16]={'16','商店-其他','1','0','0','0','0','0',nil,nil},
	[20]={'20','装备按钮','5','1','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_equipments',nil},
	[21]={'21','装备-强化','3','0','0','0','0','0',nil,nil},
	[22]={'22','装备-卸下','3','0','0','0','0','0',nil,nil},
	[23]={'23','装备-出售','3','0','0','0','0','0',nil,nil},
	[24]={'24','装备-背包购买','3','0','0','0','0','0',nil,nil},
	[30]={'30','主角按钮','1','0','0','0','0','0',nil,nil},
	[31]={'31','主角-召唤兽','0','2','0','0','0','0',nil,nil},
	[32]={'32','主角-转职与强化','5','0','0','0','0','0',nil,nil},
	[33]={'33','主角-穿戴位置解锁','5','0','0','0','0','0',nil,nil},
	[34]={'34','主角-转职','15','0','0','0','0','0',nil,nil},
	[35]={'35','主角-职业强化','5','0','0','0','0','0',nil,nil},
	[40]={'40','英雄按钮','1','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_pve_embattle',nil},
	[41]={'41','英雄1队','1','0','0','0','0','0',nil,nil},
	[42]={'42','英雄2队','10','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_pve_embattle','ui/pve_embattle/pve_embattle_view#core/team_toggles_root/toggle_team_2'},
	[43]={'43','英雄3队','30','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_pve_embattle','ui/pve_embattle/pve_embattle_view#core/team_toggles_root/toggle_team_3'},
	[44]={'44','英雄-背包购买','1','0','0','0','0','0',nil,nil},
	[45]={'45','英雄-突破','15','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_hero','ui/role/role_info_view#core/role_function_buttons_root/btn_hero_breakthrough'},
	[46]={'46','英雄-强化','1','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_hero','ui/role/role_info_view#core/role_function_buttons_root/btn_pet_strengthen'},
	[47]={'47','英雄-进阶','15','0','0','0','0','0',nil,nil},
	[50]={'50','副本按钮','1','0','0','0','0','0',nil,nil},
	[51]={'51','副本难度2','0','12','0','0','0','0',nil,nil},
	[52]={'52','副本难度3','0','10012','0','0','0','0',nil,nil},
	[53]={'53','副本-扫荡','5','0','0','0','0','0',nil,nil},
	[54]={'54','副本-2倍速','10','0','0','0','0','1',nil,nil},
	[55]={'55','副本-自动战斗','0','6','0','0','0','1',nil,nil},
	[56]={'56','副本-扫荡10次','5','0','0','0','0','4',nil,nil},
	[60]={'60','聊天','1','0','0','0','0','0',nil,nil},
	[70]={'70','邮件','5','0','0','0','0','0',nil,nil},
	[80]={'80','设置功能','30','0','0','0','0','0',nil,nil},
	[90]={'90','竞技场','18','0','0','0','0','0','ui/main/main_view#core/bottom_right_anchor/btn_multiple','ui/multiple_fight/multiple_fight_view#core/Scroll View/Viewport/Content/pvp'},
	[100]={'100','每日副本开启等级','10','0','0','0','0','0','ui/main/main_view#core/bottom_right_anchor/btn_daily_activity',nil},
	[110]={'110','远征开启','21','0','0','0','0','0','ui/main/main_view#core/bottom_right_anchor/btn_multiple','ui/multiple_fight/multiple_fight_view#core/Scroll View/Viewport/Content/expedition'},
	[120]={'120','黑市','8','0','0','0','0','0','ui/main/main_view#core/bottom_left_anchor/bottom/btn_black_market',nil},
	[130]={'130','世界树开启','19','0','0','0','0','0','ui/main/main_view#core/bottom_right_anchor/btn_multiple','ui/multiple_fight/multiple_fight_view#core/Scroll View/Viewport/Content/world_tree'},
	[140]={'140','世界boss开启','20','0','0','0','0','0',nil,nil},
	[141]={'141','世界BOSS鼓舞','20','0','0','0','0','3',nil,nil},
	[150]={'150','图鉴开启','6','0','0','0','0','0',nil,nil},
	[160]={'160','阵型开启','10','0','0','0','0','0',nil,nil},
	[170]={'170','好友功能','19','0','0','0','0','0',nil,nil},
	[180]={'180','签到功能','5','0','0','0','0','0',nil,nil},
	[190]={'190','使魔成长功能','35','0','0','0','0','0',nil,nil}
}

function t.ForEach(func)
	if not func then return end
	for i,j in pairs(t.t) do
		local r={}
		for k,v in ipairs(indexs) do
			if v ~= nil and v ~= '' then
				r[v]=j[k]
			end
		end
		func(i,r)
	end
end

function t.GetItem(id)
    local item=t[id]
	if item then return item end
	item=t.t[id]
	local result = {}
	for k,v in ipairs(indexs) do
		if v ~= nil and v ~= '' then
			result[v]=item[k]
		end
	end
	t[id]=result
	return result
end

return t
