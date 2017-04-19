-- AutoGenerate, Do not Edit!
local t = {}
local indexs={
'id','name','function_name','player_level','dungeon_pass','task_get','task_finish','notice','vip','is_show_light','is_show_animation','show_main_position','show_sheet_position','show_new_pic','show_new_des'
}

t.t = {
	['100']={'100','任务按钮','function_name.100','0','10101','0','0','0','0','1','0','ui/main/main_view#core/top_left_anchor/left/btn_task',nil,'sprite/main_ui/btn_task_1','newfunction_des.100'},
	['101']={'101','任务领取奖励',nil,'0','10101','0','0','0','0','1','0',nil,nil,nil,nil},
	['200']={'200','邮件按钮','function_name.200','0','10105','0','0','0','0','1','0','ui/main/main_view#core/top_left_anchor/left/btn_mail',nil,'sprite/main_ui/btn_mail_1','newfunction_des.200'},
	['201']={'201','邮件内所有功能',nil,'0','10105','0','0','0','0','1','0',nil,nil,nil,nil},
	['300']={'300','好友按钮','function_name.300','8','0','0','0','0','0','1','0','ui/main/main_view#core/top_left_anchor/left/btn_friend',nil,'sprite/main_ui/btn_friend_1','newfunction_des.300'},
	['301']={'301','好友内所有功能',nil,'8','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['400']={'400','签到按钮','function_name.400','10','0','0','0','0','0','1','0','ui/main/main_view#core/top_left_anchor/left/btn_sign',nil,'sprite/main_ui/btn_signin_1','newfunction_des.400'},
	['401']={'401','签到领取',nil,'10','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['500']={'500','活动按钮','function_name.500','8','0','0','0','0','0','1','2','ui/main/main_view#core/top_right_anchor/btn_activity',nil,'sprite/main_ui/btn_activity_1','newfunction_des.500'},
	['600']={'600','排行榜按钮','function_name.600','8','0','0','0','0','0','1','0','ui/main/main_view#core/top_left_anchor/left/btn_ranking',nil,'sprite/main_ui/btn_rank_1','newfunction_des.600'},
	['700']={'700','主角按钮','function_name.700','1','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_player',nil,'sprite/main_ui/btn_hero','newfunction_des.700'},
	['701']={'701','使魔选择按钮','function_name.701','25','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_select_profession',nil,nil,nil},
	['702']={'702','使魔激活',nil,'25','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['703']={'703','使魔强化按钮','function_name.703','0','10102','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_player_strengthen',nil,nil,nil},
	['704']={'704','使魔强化',nil,'0','10102','0','0','0','0','1','0',nil,nil,nil,nil},
	['705']={'705','使魔培养按钮','function_name.705','99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['706']={'706','使魔技能激活',nil,'99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['707']={'707','使魔突破按钮','function_name.707','99','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_player_breakthrough',nil,nil,nil},
	['708']={'708','使魔突破',nil,'99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['709']={'709','使魔穿戴按钮','function_name.709','0','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_role_equipment',nil,nil,nil},
	['710']={'710','使魔升星','function_name.803','21','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_player_advance',nil,nil,nil},
	['800']={'800','培养按钮','function_name.800','0','10102','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_hero',nil,'sprite/main_ui/btn_train_1','newfunction_des.800'},
	['801']={'801','英雄强化按钮','function_name.801','0','10102','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_hero_strengthen',nil,nil,nil},
	['802']={'802','英雄强化',nil,'0','10102','0','0','0','0','1','0',nil,nil,nil,nil},
	['803']={'803','英雄升星按钮','function_name.803','21','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_hero_advance',nil,nil,nil},
	['804']={'804','英雄升星',nil,'21','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['805']={'805','英雄突破按钮','function_name.805','99','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/role_function_buttons_root/btn_hero_breakthrough',nil,nil,nil},
	['806']={'806','英雄突破',nil,'99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['807']={'807','英雄分解','function_name.807','0','0','0','0','0','0','1','0','ui/role/role_info_view#core/role_info_panel/hero_attr_root/btn_decompose',nil,nil,nil},
	['900']={'900','阵形按钮','function_name.900','0','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_pve_embattle',nil,'sprite/main_ui/btn_team_1','newfunction_des.900'},
	['901']={'901','换阵按钮','function_name.901','0','0','0','0','0','0','1','0','ui/pve_embattle/pve_embattle_view_lua#core/formation_team_view(Clone)/btn_formation',nil,nil,nil},
	['902']={'902','阵形培养',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['903']={'903','两队切换',nil,'16','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['904']={'904','三队切换',nil,'27','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['905']={'905','两队按钮','function_name.905','16','0','0','0','0','0','1','0','ui/pve_embattle/pve_embattle_view_lua#core/team_toggles_root/toggle_team_2',nil,nil,nil},
	['906']={'906','三队按钮','function_name.906','27','0','0','0','0','0','1','0','ui/pve_embattle/pve_embattle_view_lua#core/team_toggles_root/toggle_team_3',nil,nil,nil},
	['1000']={'1000','装备按钮','function_name.1000','0','10105','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_equipments',nil,'sprite/main_ui/btn_equipments_1','newfunction_des.1000'},
	['1001']={'1001','装备培养按钮','function_name.1001','0','10105','0','0','0','0','1','0','ui/equipments/role_equipments_view#core/left_part/img_selected_equipment_info_frame/btn_train',nil,nil,nil},
	['1002']={'1002','装备强化',nil,'0','10105','0','0','0','0','1','0',nil,nil,nil,nil},
	['1003']={'1003','装备进阶',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1004']={'1004','装备重铸按钮','function_name.1004','24','0','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_recast',nil,nil,nil},
	['1005']={'1005','装备重铸',nil,'24','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1006']={'1006','装备附魔按钮','function_name.1006','28','0','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_enchant',nil,nil,nil},
	['1007']={'1007','装备附魔',nil,'28','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1008']={'1008','装备升星按钮','function_name.1008','0','10206','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_upStar',nil,nil,nil},
	['1009']={'1009','装备升星',nil,'0','10206','0','0','0','0','1','0',nil,nil,nil,nil},
	['1010']={'1010','宝石镶嵌按钮','function_name.1010','35','0','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_diamond_insert',nil,nil,nil},
	['1011']={'1011','宝石镶嵌',nil,'35','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1012']={'1012','装备进阶按钮','function_name.1012','0','0','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_advance',nil,nil,nil},
	['1013']={'1013','装备继承按钮','function_name.1013','24','0','0','0','0','0','1','0','ui/equipments/equipment_training_view#core/right/toggle_group/toggle_inherit',nil,nil,nil},
	['1014']={'1014','装备继承',nil,'24','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1100']={'1100','商店按钮','function_name.1100','0','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_shop',nil,'sprite/main_ui/btn_shop_1','newfunction_des.1100'},
	['1200']={'1200','黑市按钮','function_name.1200','10','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_black_market',nil,'sprite/main_ui/btn_blackmarket_1','newfunction_des.1200'},
	['1201']={'1201','黑市功能',nil,'10','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1202']={'1202','竞技场黑市按钮','function_name.1202','0','10301','0','0','0','0','1','0',nil,nil,nil,nil},
	['1203']={'1203','远征黑市按钮','function_name.1203','0','10308','0','0','0','0','1','0',nil,nil,nil,nil},
	['1204']={'1204','世界BOSS黑市按钮','function_name.1204','30','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1205']={'1205','积分赛黑市按钮','function_name.1205','35','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1300']={'1300','图鉴按钮','function_name.1300','0','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_illustration',nil,'sprite/main_ui/btn_book_1','newfunction_des.1300'},
	['1301']={'1301','图鉴功能',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1400']={'1400','公会按钮','function_name.1400','25','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_consortia',nil,'sprite/main_ui/btn_consortia','newfunction_des.1400'},
	['1401']={'1401','公会开启',nil,'25','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1500']={'1500','探险按钮','function_name.1500','32','0','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_explore',nil,'sprite/main_ui/btn_dispatch_1','newfunction_des.1500'},
	['1501']={'1501','探险委派',nil,'32','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1600']={'1600','战斗中心按钮','function_name.1600','0','10301','0','0','0','0','1','2','ui/main/main_view#core/bottom_right_anchor/btn_multiple',nil,'sprite/main_ui/btn_pvp_1','newfunction_des.1600'},
	['1601']={'1601','竞技场按钮','function_name.1601','0','10301','0','0','0','0','1','0',nil,nil,nil,nil},
	['1602']={'1602','竞技场所有功能',nil,'0','10301','0','0','0','0','1','0',nil,nil,nil,nil},
	['1604']={'1604','世界树按钮','function_name.1604','26','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1605']={'1605','世界树所有功能',nil,'26','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1606']={'1606','远征按钮','function_name.1606','0','10308','0','0','0','0','1','0',nil,nil,nil,nil},
	['1607']={'1607','远征所有功能',nil,'0','10308','0','0','0','0','1','0',nil,nil,nil,nil},
	['1608']={'1608','PVP积分赛按钮','function_name.1608','35','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1609']={'1609','PVP积分赛',nil,'35','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1610']={'1610','矿战按钮','function_name.1610','28','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1611']={'1611','矿战功能',nil,'28','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1700']={'1700','每日副本按钮','function_name.1700','0','10204','0','0','0','0','1','1','ui/main/main_view#core/bottom_right_anchor/btn_daily_activity',nil,'sprite/main_ui/btn_day','newfunction_des.1700'},
	['1701']={'1701','每日副本挑战',nil,'0','10204','0','0','0','0','1','0',nil,nil,nil,nil},
	['1800']={'1800','战役副本按钮','function_name.1800','0','0','0','0','0','0','1','1','ui/main/main_view#core/bottom_right_anchor/btn_start_fight',nil,nil,nil},
	['1801']={'1801','普通难度按钮','function_name.1801','0','10207','0','0','0','0','1','0','ui/chapter/select_chapter_view#core/difficulty_buttons_root/btn_normal',nil,nil,nil},
	['1802']={'1802','困难难度按钮','function_name.1802','0','20207','0','0','0','0','1','0','ui/chapter/select_chapter_view#core/difficulty_buttons_root/btn_hard',nil,nil,nil},
	['1803']={'1803','单次扫荡按钮','function_name.1803','0','10105','0','0','0','0','1','0',nil,nil,nil,nil},
	['1804']={'1804','单次扫荡功能',nil,'0','10105','0','0','0','0','1','0',nil,nil,nil,nil},
	['1805']={'1805','十次扫荡按钮','function_name.1805','0','0','0','0','0','4','1','0',nil,nil,nil,nil},
	['1806']={'1806','十次扫荡功能',nil,'0','0','0','0','0','4','1','0',nil,nil,nil,nil},
	['1807']={'1807','两倍速按钮','function_name.1807','0','10101','0','0','0','0','1','0','ui/fight_tips/fight_tips_view#core/top_right_anchor/btn_set_speed',nil,nil,nil},
	['1808']={'1808','自动战斗按钮','function_name.1808','0','10101','0','0','0','0','1','0','ui/fight_tips/fight_tips_view#core/top_right_anchor/btn_auto_fight',nil,nil,nil},
	['1809']={'1809','暂停按钮','function_name.1809','0','10105','0','0','0','0','0','0','ui/fight_tips/fight_tips_view#core/top_right_anchor/btn_pause_fight',nil,nil,nil},
	['1900']={'1900','世界BOSS按钮','function_name.1900','30','0','0','0','0','0','1','0','ui/main/main_view#core/btn_world_boss',nil,'sprite/main_ui/icon_map_boss','newfunction_des.1900'},
	['1901']={'1901','世界BOSS挑战',nil,'30','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['1902']={'1902','世界BOSS鼓舞按钮','function_name.1902','30','0','0','0','0','3','1','0',nil,nil,nil,nil},
	['1903']={'1903','世界BOSS鼓舞',nil,'30','0','0','0','0','3','1','0',nil,nil,nil,nil},
	['2000']={'2000','聊天按钮','function_name.2000','9','0','0','0','0','0','1','1','ui/main/main_view#core/common_top_bar(Clone)/btn_chat',nil,'sprite/main_ui/btn_talk','newfunction_des.2000'},
	['2001']={'2001','聊天功能',nil,'9','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2100']={'2100','背包','function_name.2100','0','10202','0','0','0','0','1','2','ui/main/main_view#core/bottom_left_anchor/bottom/btn_pack',nil,'sprite/main_ui/btn_bag','newfunction_des.2100'},
	['2101']={'2101','背包',nil,'0','10202','0','0','0','0','1','0',nil,nil,nil,nil},
	['2200']={'2200','boss列表','function_name.2200','29','0','0','0','0','0','1','1','ui/chapter/select_chapter_view#core/btn_boss_dungeon_list',nil,'sprite/main_ui/icon_map_boss','newfunction_des.2200'},
	['2201']={'2201','boss列表',nil,'29','0','0','0','0','0','1','1',nil,nil,nil,nil},
	['2300']={'2300','每日任务','function_name.2300','0','10204','0','0','0','0','1','1','ui/main/main_view#core/bottom_right_anchor/btn_daily_task',nil,'sprite/main_ui/btn_task_1','newfunction_des.2300'},
	['2400']={'2400','合成按钮','function_name.2400','99','0','0','0','0','0','1','1','ui/main/main_view#core/bottom_left_anchor/bottom/btn_hero_combine',nil,'sprite/main_ui/icon_anvil','newfunction_des.2400'},
	['2401']={'2401','装备合成','function_name.2401','99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2402']={'2402','装备合成',nil,'99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2403']={'2403','英雄合成','function_name.2402','99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2404']={'2404','英雄合成',nil,'99','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2500']={'2500','点金手按钮','function_name.2500','0','0','0','0','0','0','1','0',nil,nil,nil,'newfunction_des.2500'},
	['2600']={'2600','七日狂欢','function_name.2600','0','0','0','0','0','0','1','0',nil,nil,nil,'newfunction_des.2500'},
	['2601']={'2601','七日狂欢',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,'newfunction_des.2500'},
	['2700']={'2700','首充礼包','function_name.2700','0','0','0','0','0','0','1','0',nil,nil,nil,'newfunction_des.2500'},
	['2701']={'2701','首充礼包',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,'newfunction_des.2500'},
	['2800']={'2800','转盘','function_name.2800','0','0','0','0','0','0','1','0',nil,nil,nil,nil},
	['2801']={'2801','转盘',nil,'0','0','0','0','0','0','1','0',nil,nil,nil,nil}
}

function t.ForEach(func)
	if not func then return end
	local ky = nil
	local v = nil
	for i,j in pairs(t.t) do
		local r={}
		ky=tonumber(i)
		for k=1,#indexs do
			v=indexs[k]
			if v and v ~= '' then
				r[v]=j[k]
			end
		end

		if ky then func(ky,r) 
		else func(i,r) end
	end
end

function t.GetItem(id)
	id=tostring(id)
    local item=t[id]
	if item then return item end
	item=t.t[id]
	local result = {}
	local v = nil
	for i=1,#indexs do
		v=indexs[i]
		if v and v ~= '' then
			result[v]=item[i]
		end
	end
	t[id]=result
	return result
end

return t
