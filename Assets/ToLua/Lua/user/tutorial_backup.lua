-- Tutorial Test

-- [[ chapter parameters ]] --
-- [[ 开启条件参数 ]] --
--["player_level"]:玩家等级
--["pass_dungeon"]:通关副本ID
--["task_id"]:完成任务ID
--["dungeon_total_star_count"]:主线副本评星总数
--["at_ui_view_path"]:打开UI界面路径
--["function_open_id"]:功能开启ID

-- [[ 其它参数 ]] --
--["complete_step_id"]:结束步骤ID
--["is_skippable"]:是否可跳过  [可跳过 = true], [不可跳过 = false] 如果不填为不可跳过
--["can_ignore"]:是否可被忽略  [可忽略 = true], [不可忽略 = false] 如果不填为不可忽略
-- [[ chapter parameters ]] --

--------------------
-- ["id"]: 新手教程步骤id
-- ["next_id"]: 新手教程下一步id
-- ["show_mask"]: 是否显示屏蔽层(功能屏蔽）
-- ["enable_next_step_button"]: 是否开启全屏下一步按钮
-- ["dialog"]: 教程对话,其中
--    ["anchor"]: 锚点,其值可以有四种，分别为1. TopLeft, 2. TopRight, 3. BottomRight, 4. BottomLeft
--    ["npc_head_icon"]: npc图片名
--    ["dialog_content_id"]: 对话内容的语言包ID
-- ["illustrate_image"]: 新手指引配图(没有则不显示新手指引配图)
--    ["image"]: 图片名
--    ["position"]: 图片位置，若图片位置为（0,0），则图片显示在左下角
-- ["hand_indicate_ui_path"]: 手指指示UI控件路径(没有则不显示手指)
-- ["arrow_indicate_ui_path"]: 箭头指引UI控件路径(没有则不显示箭头)
-- ["highlight_ui_path_list"]: 需要高亮的UI控件路径列表(可以同时配置多个高亮UI路径)
-- ["masked_ui_path_list"]: 需要遮挡的UI控件路径列表
-- 	["delay_time"] = 1
-- ["wait_msg_id_list"]: 等待消息列表
-- ["force_complete_msg_id_list"]: 强制结束当前步骤消息列表
-- ["on_complete_msg_list"]: 当前步骤完成消息列表（当前步骤完成时通知已注册该消息列表中消息的模块）
--------------------

tutorial_data = {}
tutorial_data.IsTutorialOpen = true

backup_tutorial_chapter = {} 


backup_tutorial_chapter[5] = { --1-1后返回主界面
  ["pass_dungeon"] = 10101,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,
  
  ["steps"] = {
      ["t5.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
    ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA1"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    },
  }
}   

  
--1-2结束上阵亏刚
backup_tutorial_chapter[8] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,

  ["steps"] = {
      ["t8.1"] = {   --对话，女仆出来说一下
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA2"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      
    },
    
    ["t8.2"] = {  --阵型按钮
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}}
    },

    ["t8.3"] = {  --选中英雄
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA21"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
    
    ["t8.4"] = {  --放入位置
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["arrow_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}}
    },
  
  }
}


--1-2奎钢上阵后引导经验药水
backup_tutorial_chapter[50] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {
    ["t50.1"] = {  --进入培养
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
    },

        ["t50.2"] = {  --对话，选择狼
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}},
    },

    ["t50.3"] = {   --对话，经验药水
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}},
	},
  }
}











--1-2奎钢上阵后返回战斗
backup_tutorial_chapter[51] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,

  ["steps"] = {
    ["t51.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "BPA3"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"}, 
    },
    
    
    ["t51.2"] = {  --对话，选择副本
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10103"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10103"}},
    },
  
    ["t51.3"] = {
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    }, 
  }
}



backup_tutorial_chapter[9] = { --1-1后返回主界面
  ["pass_dungeon"] = 10103,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,
   
  ["steps"] = {
      ["t9.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA4"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    }, 
  }
}





--1-4打完去打1-5
backup_tutorial_chapter[12] = {     --亏刚上阵
  ["pass_dungeon"] = 10104,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,

  ["steps"] = {
    ["t12.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "BPA5"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"}, 
    },
    
    
    ["t12.2"] = {  --对话，选择副本
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10105"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10105"}},
    },
  
    ["t12.3"] = {
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    }, 
  }
}




--1-5结束返回主城



backup_tutorial_chapter[13] = {   --穿装备
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,

  ["steps"] = {
      ["t13.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA6"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    }, 
  }
}




backup_tutorial_chapter[15] = {   --强化装备
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 4,

  ["steps"] = {
 
    ["t15.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA7"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
    ["t15.2"] = {   --对话，选择英雄装备
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA34"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  

    ["t15.3"] = {   --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA37"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      },
    
    ["t15.4"] = {   --对话
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA38"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
      },
  }
}



--2-2结束返回主城合成英雄


 
backup_tutorial_chapter[16]  = {    --合成英雄
  ["pass_dungeon"] = 10202,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,
  ["steps"] = {
      ["t16.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA8"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    }, 
  }
}




--2-4结束返回主城打每日副本



backup_tutorial_chapter[19] = {    --返回主界面
  ["pass_dungeon"] = 10204,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,
  ["steps"] = {
      ["t19.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA9"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    }, 
  }
}

--打完2-6之后装备升星


backup_tutorial_chapter[22] = {
  ["pass_dungeon"] = 10206,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 1,
  
  ["steps"] = {
      ["t19.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "BPA10"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    }, 
  }
}



backup_tutorial_chapter[24] = {--升星
  ["pass_dungeon"] = 10206,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,
  
 ["steps"] = {

 ["t24.1"] = {   --对话，选择英雄
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA59"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },

    ["t24.2"] = {   --对话，选择装备穿戴
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA60"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

      ["t24.3"] = {   --对话，选择防具
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA61"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    

        ["t24.4"] = {   --对话，peiyang
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA64"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    


    ["t24.5"] = {   --对话，选择升星
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA65"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },


    ["t24.6"] = {   --对话，升星
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA66"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}




--打完2-7引导普通难度


backup_tutorial_chapter[25] = {
  ["pass_dungeon"] = 10207,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 2,
  

    ["steps"] = {
    ["t25.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA67"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
    },
    
  
    ["t25.2"] = {  --普通关卡按钮
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/normal_root/btn_normal"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/normal_root/btn_normal"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA68"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

        ["t25.3"] = {   --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA69"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
  }
}




--打完3-1开竞技场
backup_tutorial_chapter[26] = {

  ["pass_dungeon"] = 10301,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 2,
  
  ["steps"] = {


    ["t26.1"] = {   --对话，选择战斗中心
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
    ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA71"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"},
    },

    ["t26.2"] = {   --对话，选择竞技场
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA72"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t26.3"] = {   --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA73"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face3"}},
    },
 
    ["t26.4"] = {   --对话，确认成功
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA74"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},

    },
  }
}
