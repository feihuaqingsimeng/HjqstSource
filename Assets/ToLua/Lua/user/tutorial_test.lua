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
--  ["delay_time"] = 1
-- ["wait_msg_id_list"]: 等待消息列表
-- ["force_complete_msg_id_list"]: 强制结束当前步骤消息列表
-- ["on_complete_msg_list"]: 当前步骤完成消息列表（当前步骤完成时通知已注册该消息列表中消息的模块）
--------------------

tutorial_data = {}
tutorial_data.IsTutorialOpen = true

tutorial_cahpter_id_list = {1, 2, 3, 4, 5, 6, 7, 8, 50, 51, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 100}

tutorial_chapter = {} 

tutorial_chapter[1] = { --第一场战斗


  ["complete_step_id"] = 21,
    ["at_ui_view_path"] = 'impossible',
  ["is_skippable"] = false,
   ["steps"] = {
    ["t1.1"] = {   
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false}, 
    ["wait_msg_id_list"] = {"donot wait me!"},
    --["expand_data"] = {["isSkip"] = 1},--跳过该章
    },
  
   ["t1.2"] = {
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["delay_time"] = 2,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP1"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"200", "face3"}},
      },
      
   ["t1.3"] = {
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopRight", ["offset"] = "-200, -2", ["dialog_content_id"] = "OP3"},
      ["npc"] = {["anchor"] = "TopRight", ["offset"] = "0,0",  ["npc_show"] =  {"52", "face3"}},
      },
    

   ["t1.4"] = {
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP2"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"27", "face3"}},
      --   退出战斗["on_complete_msg_list"] = {"Fight::ForceFightFinishedHandler"},
      },
 
         
    ["t1.5"] = {--27
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 27 ,
      ["skill_id"] = 272,
      ["damage"] = 16683,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
      ["wait_msg_id_list"] = {"donot wait me!"},
      },
  
  
  
       ["t1.6"] = { --
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = false},
      --["damage"] = 8130,
      ["enable_next_step_button"] = true,
      ["delay_time"] = 2,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP4"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"138", "face3"}},
      },
  
  
  
  
       ["t1.7"] = {--138指引
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 138,
      ["skill_id"] = 1382,
      ["damage"] = 19391,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
      ["wait_msg_id_list"] = {"donot wait me!"},
      },
  
  
  
    ["t1.8"] = {--boss
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopRight", ["offset"] = "-200, -2", ["dialog_content_id"] = "OP5"},
      ["npc"] = {["anchor"] = "TopRight", ["offset"] = "0,0",  ["npc_show"] =  {"52", "face3"}},
     },
   
    ["t1.9"] = { 
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 51,
      ["skill_id"] = 510,
      ["damage"] = 18663,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
      ["wait_msg_id_list"] = {"donot wait me!"},
    },
    
    
    






  









  

 
     ["t1.10"] = { --
      ["id"] = 10,
      ["next_id"] = 11,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP6"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"104", "face3"}},
      },
  
   
    
     ["t1.11"] = {--104浮空
      ["id"] = 11,
      ["next_id"] = 12,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 104,
      ["skill_id"] = 1041,
      ["damage"] = 7361,
      ["expand_data"] = {["MoveNextSkilled"] = "1",["comboWait"] = "0"},--combowait不用等待，自动继续，没有就会等待
      ["wait_msg_id_list"] = {"donot wait me!"},
      },
  
  
  
     ["t1.12"] = {--27追击1
      ["id"] = 12,
      ["next_id"] = 15,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 129,
      ["skill_id"] = 1291,
      ["damage"] = 7918,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
      ["wait_msg_id_list"] = {"donot wait me!"},
      },
  
  
    --   ["t1.13"] = {--203技能1
    --   ["id"] = 13,
    --   ["next_id"] = 14,
    --   ["mask"] = {["enable"] = true},
    --   ["hero_instance_id"] = 203,
    --   ["skill_id"] = 2031,
    --   ["damage"] = 21458,
    --   ["expand_data"] = {["MoveNextSkilled"] = "1",["comboWait"] = "0"},--combowait不用等待，自动继续，没有就会等待
    --   ["wait_msg_id_list"] = {"donot wait me!"},
    --   },
  
  
    -- ["t1.14"] = { --121
    --   ["id"] = 14,
    --   ["next_id"] = 15,
    --   ["mask"] = {["enable"] = true},
    --   ["hero_instance_id"] = 129,
    --   ["skill_id"] = 1291,
    --   ["damage"] = 8971,
    --   ["expand_data"] = {["MoveNextSkilled"] = "1"},
    --   ["wait_msg_id_list"] = {"donot wait me!"},
    --   },
  
    ["t1.15"] = { --125
      ["id"] = 15,
      ["next_id"] = 16,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP7"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"129", "face3"}},
     },
   
    ["t1.16"] = {--boss
      ["id"] = 16,
      ["next_id"] = 17,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopRight", ["offset"] = "-200, -2", ["dialog_content_id"] = "OP8"},
      ["npc"] = {["anchor"] = "TopRight", ["offset"] = "0,0",  ["npc_show"] =  {"52", "face3"}},
     },
   
    ["t1.17"] = { 
      ["id"] = 17,
      ["next_id"] = 18,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 51,
      ["skill_id"] = 511,
      ["damage"] = 37663,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
      ["wait_msg_id_list"] = {"donot wait me!"},
    },
    
          
   
    ["t1.18"] = {--boss
      ["id"] = 18,
      ["next_id"] = 19,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopRight", ["offset"] = "-200, -2", ["dialog_content_id"] = "OP9"},
      ["npc"] = {["anchor"] = "TopRight", ["offset"] = "0,0",  ["npc_show"] =  {"52", "face3"}},
    },
    
    ["t1.19"] = { --变身
      ["id"] = 19,
      ["next_id"] = 20,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 51,
      ["delay_time"] = 6.7,
      ["expand_data"] = {["Transform"] = "1053",["Mechanics"] = "5155"},
      ["wait_msg_id_list"] = {"donot wait me!"},
    },
    
    ["t1.20"] = {--boss
      ["id"] = 20,
      ["next_id"] = 21,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopRight", ["offset"] = "-200, -2", ["dialog_content_id"] = "OP10"},
      ["npc"] = {["anchor"] = "TopRight", ["offset"] = "0,0",  ["npc_show"] =  {"52", "face3"}},
    },
    

    ["t1.21"] = { --125
      ["id"] = 21,
      ["next_id"] = 22,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP11"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"129", "face3"}},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"},
     },
 
     ["t1.22"] = {
      ["id"] = 22,
      ["next_id"] = 23,
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
       ["skill_id"] = 51011,
     },


    ["t1.23"] = {
      ["id"] = 23,
      ["next_id"] = 24,
      ["mask"] = {["enable"] = true},
      ["hero_instance_id"] = 51,
      ["skill_id"] = 51011,
      ["damage"] = 42671,
      ["delay_time"] = 0.5,
      ["expand_data"] = {["MoveNextSkilled"] = "1"},
 --     ["wait_msg_id_list"] = {"donot wait me!"},
  },
  
  ["t1.24"] = {
      ["id"] = 24,
      ["next_id"] = 25,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "TopLeft", ["offset"] = "200, -2", ["dialog_content_id"] = "OP12"},
      ["npc"] = {["anchor"] = "TopLeft", ["offset"] = "0,0",  ["npc_show"] =  {"200", "face3"}},

      },


        ["t1.25"] = {
      ["id"] = 25,
      ["next_id"] = 26,
      ["expand_data"] = {["FightOver"] = "1"},
     },


   }
}



--上面是第一场战斗




tutorial_chapter[2] = { --抽卡，对话
  ["at_ui_view_path"] = "ui/main/main_view",
  ["function_open_id"] = 1100,
  ["complete_step_id"] = 4,

  ["steps"] = {
    ["t2.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA1"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
      },


        ["t2.2"] = {--商店
      ["id"] = 2,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},

      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA2"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
          ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_shop"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_shop"}}
     },


    --["t2.3"] = {  --对话
      --["id"] = 3,
      --["next_id"] = 4,
      --["mask"] = {["enable"] = false},
      --["enable_next_step_button"] = true,
      --["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA3"},
      --["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face3"}},
    --},
    
    ["t2.4"] = {  --单抽
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/shop_view", "core/img_frame/normal_shop/scrollview/viewport/content/0/img_bottom_bar/btn_buy"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/shop_view", "core/img_frame/normal_shop/scrollview/viewport/content/0/img_bottom_bar/btn_buy"}},
    },


    ["t2.5"] = {  --返回1
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["wait_msg_id_list"] = {"ui/shop/single_draw_card_result_view::OnViewStay"},
    },




    ["t2.6"] = {  --对话
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA4"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/single_draw_card_result_view", "core/bottom_bar/buttons_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/single_draw_card_result_view", "core/bottom_bar/buttons_root/btn_back"}},
  
    },


   ["t2.7"] = {  --返回
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/shop_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/shop_view", "core/common_top_bar(Clone)/btn_back"}},
    },

  }
}



--上面是引导第1次抽卡



tutorial_chapter[3] = { --对话，上阵谢拉尔

  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,  
  ["steps"] = {
      ["t3.1"] = {   --对话，女仆出来说一下
      ["id"] = 1,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA5"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}}   
    },
    
    


    --["t3.2"] = {  --对话
      --["id"] = 2,
      --["next_id"] = 3,
      --["mask"] = {["enable"] = false},
      --["enable_next_step_button"] = true,
      --["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA6"},
      --["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    --},


    ["t3.3"] = {  --选中英雄
      ["id"] = 3,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/0"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/0"}}
    },
      

    -- ["t3.4"] = {  --对话
    --   ["id"] = 4,
    --   ["next_id"] = 5,
    --   ["mask"] = {["enable"] = false},
    --   ["enable_next_step_button"] = true,
    --   ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA7"},
    --   ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    -- },


    ["t3.5"] = {  --放入位置
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["arrow_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_8/formation_base_button"}},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_8/formation_base_button"}}
    },


    ["t3.6"] = {  --对话
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA8"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}},
    },

  }
}



--上面是引导第一次上阵






--下面是引导打第1个副本



tutorial_chapter[4] = { --前往副本，对话，指引，打1-1

  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 13,
  
  ["steps"] = {
    ["t4.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA9"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
    },
    
    
    ["t4.2"] = {  --对话，选择副本
      ["id"] = 2,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA10"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10101"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10101"}},
    },



    -- ["t4.3"] = {  --对话
    --   ["id"] = 3,
    --   ["next_id"] = 4,
    --   ["mask"] = {["enable"] = false},
    --   ["enable_next_step_button"] = true,
    --   ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA11"},
    --   ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face1"}},
    --  },

  
    ["t4.4"] = {
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
      ["wait_msg_id_list"] = {"1030"},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA12"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face1"}},
    },
    
    ["t4.5"] = {
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["wait_msg_id_list"] = {"Fight::OnSecondFightStart"},
      ["on_complete_msg_list"] = {"Fight::FightHangup"}
    },
    
    ["t4.6"] = {  --
      ["id"] = 6,
      ["next_id"] = 7,
      ["delay_time"] = 2,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFightHangupHandler"},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"}
    },

    ["t4.7"] = {  --
      ["id"] = 7,
      ["next_id"] = 8,
      ["skill_id"] = 2001,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"}
    },
    
  
    ["t4.8"] = {  --
      ["id"] = 8,
      ["next_id"] = 9,
      ["skill_id"] = 1191,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
      ["on_complete_msg_list"] = {"Fight::StopFightTime"}--战斗时间暂停
    },
    
    

    ["t4.9"] = {  --教程图
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = true}, 
      ["enable_next_step_button"] = true,
      ["illustrate_image"] = {["image"] = "tutorial_01", ["position"] = "250,130"},
    },
  
  
    ["t4.10"] = {   --play skill 
      ["id"] = 10,
      ["next_id"] = 11,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_player.name.tyro/skillitem_view_1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_player.name.tyro/skillitem_view_1"}},
      ["on_complete_msg_list"] = {"Fight::SetNeedComboPause"}
    },
  

    ["t4.11"] = {   --等待协议
      ["id"] = 11,
      ["next_id"] = 12,
      ["delay_time"] = 1,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["wait_msg_id_list"] = {"Fight::OnComboWaitSettingHandler"},
    },
  
    ["t4.12"] = {   --play skill ----暂停后继续
      ["id"] = 12,
      ["next_id"] = 13,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.lasser/skillitem_view_1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.lasser/skillitem_view_1"}},
      ["on_complete_msg_list"] = {"Fight::OnComboWaitingHandler","Fight::ReSetNeedComboPause","Fight::FightRegainOrder","Fight::ReStartFightTime"}  --恢复战斗时间
    },
    
    ["t4.13"] = {   --start combo 
      ["id"] = 13,
      ["next_id"] = 14,
      ["expand_data"] = {["startCombo"] = 1},
      ["wait_msg_id_list"] = {"Fight::OnFightRegainOrder"},
    },
   }
}




tutorial_chapter[5] = {    -- 1-1后返回主界面
  ["pass_dungeon"] = 10101,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 2,
  
  ["steps"] = {
    
  
    ["t5.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t5.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA13"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    },
  
    ["t5.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t5.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}






tutorial_chapter[6] = { --主界面任务
  ["at_ui_view_path"] = "ui/main/main_view",
  ["task_id"] = 30301, --完成第一个副本任务
  ["function_open_id"] = 100,
  ["complete_step_id"] = 3,

  ["steps"] = {
    ["t6.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA14"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/top_left_anchor/left/btn_task"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/top_left_anchor/left/btn_task"}}
       },


    ["t6.2"] = {   --等待协议
       ["id"] = 2,
       ["next_id"] = 3,
       ["mask"] = {["enable"] = true, ["show"] = false},   
       ["wait_msg_id_list"] = {"ui/task/task_view::OnScrollReady"},
   },

    ["t6.3"] = {   --对话，点击任务
      ["id"] = 3,
       ["next_id"] = 4,
       ["mask"] = {["enable"] = true, ["show"] = false},
       ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/task/task_view", "core/right_root/scroll_view/Viewport/Content/30301/btn_complete"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
       ["highlight_ui_path_list"] = {["ui_path"] = {"ui/task/task_view", "core/right_root/scroll_view/Viewport/Content/30301/btn_complete"}},
       ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA15"},   
    },
    
   --     ["t6.4"] = {   --对话，提示任务完成
   --     ["id"] = 4,
   --     ["next_id"] = 5,
   --     ["mask"] = {["enable"] = false},
   --     ["enable_next_step_button"] = true,
   --     ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA16"},
   -- },

 }
}


  
--做完任务前往第二关

tutorial_chapter[7] = { --主界面任务
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,

  ["steps"] = {
    ["t7.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA17"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"},   
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
    },

  
    ["t7.2"] = {  --对话，选择副本
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA18"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10102"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10102"}},
    },



  
    ["t7.3"] = {
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },


    ["t7.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
  
   ["t7.5"] = {  --返回大地图
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA19"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},  
    },
  }
}


--1-2结束上阵亏刚

tutorial_chapter[8] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {
     ["t8.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = { "ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA20"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"},  
    },
    
    ["t8.2"] = {  --选中英雄
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA21"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
    
    ["t8.3"] = {  --放入位置
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["arrow_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}}
    },

  }
}


--1-2奎钢上阵后引导经验药水
tutorial_chapter[50] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/pve_embattle/pve_embattle_view_lua",
  ["complete_step_id"] = 3,
  
  ["steps"] = {
    ["t50.1"] = {  --返回战斗界面
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/right_panel_root/right_root/right_panel/btn_roles_info"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/right_panel_root/right_root/right_panel/btn_roles_info"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA22"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}}, 
    },

        ["t50.2"] = {  --对话，选择副本
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
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA47"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}







--1-2奎钢上阵经验药水后返回战斗
tutorial_chapter[51] = {     --亏刚上阵
  ["pass_dungeon"] = 10102,
  ["at_ui_view_path"] = "ui/role/role_info_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {

    ["t51.1"] = {   --对话，经验药水
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}},
   },

    ["t51.2"] = {  --返回战斗界面
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA22"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}}, 
    },

        ["t51.3"] = {  --对话，选择副本
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },
  }
}



--1-3结束返回英雄强化

tutorial_chapter[9] = {     
  ["pass_dungeon"] = 10103,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 2,
  
  ["steps"] = {
      
    ["t9.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },



     ["t9.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA23"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"},["npc_sound"] = "4"},
    },
  

    ["t9.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },



  
    ["t9.4"] = {  --返回主城
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}



--狼强化

tutorial_chapter[10] = {    
  ["pass_dungeon"] = 10103,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,
  
  ["steps"] = {
  

    ["t10.1"] = {   --对话，选择培养
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA24"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
        ["t10.2"] = {   --对话，选择阵型中的英雄培养
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}},
    -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA25"},
    --   ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  
  
    
        ["t10.3"] = {   --对话，选择狼
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_hero_strengthen"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_hero_strengthen"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA26"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
  
    ["t10.4"] = {   --对话，选择材料
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthenAutoPut"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthenAutoPut"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA27"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
  
    ["t10.5"] = {   --开始强化
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthen"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthen"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA28"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
     
    ["t10.6"] = {   --开始强化
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA29"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/common_top_bar(Clone)/btn_back"}},
    },
        
    
        ["t10.7"] = {   --返回
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}





--强化完狼去打1-4

tutorial_chapter[11] = { --主界面任务
  ["at_ui_view_path"] = "ui/main/main_view",
  ["pass_dungeon"] = 10103,
  ["complete_step_id"] = 3,

  ["steps"] = {
    ["t11.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA30"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"},["npc_sound"] = "4"},   
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
    },


  
    ["t11.2"] = {  --对话，选择副本
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10104"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_10104"}},
    },


  
    ["t11.3"] = {
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },


    ["t11.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
  
   ["t11.5"] = {  --返回大地图
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA31"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},   
    },
  }
}





--1-4打完去打1-5

tutorial_chapter[12] = {     --亏刚上阵
  ["pass_dungeon"] = 10104,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 1,
  
  ["steps"] = {
     
        ["t12.1"] = {  --对话，开始战斗
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },
  }
}

























--1-5结束返回主城



tutorial_chapter[13] = {    --穿装备
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 4,
  ["steps"] = {
  

    ["t13.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t13.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA32"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t13.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t13.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}






tutorial_chapter[14] = {    --穿装备
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 4,
  ["steps"] = {
  

    ["t14.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA33"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
    ["t14.2"] = {   --对话，选择英雄装备
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA34"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
  
    ["t14.3"] = {   --对话，选择英雄装备
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA35"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
      
    
    ["t14.4"] = {   --对话，选择穿戴
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA36"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}














tutorial_chapter[15] = { --装备强化
 
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/equipments/role_equipments_view",
  ["complete_step_id"] = 2,
  
  ["steps"] = {

    ["t15.1"] = {   --对话
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA37"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      },
    
    ["t15.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA38"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
      },
  }
}









--2-2结束返回主城合成英雄


 
tutorial_chapter[16] = {    --合成英雄
  ["pass_dungeon"] = 10202,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 4,
  ["steps"] = {
  

    ["t16.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t16.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA39"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t16.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t16.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
   }
}








 
tutorial_chapter[17] = {    --合成英雄
  ["pass_dungeon"] = 10202,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 4,
  ["steps"] = {
  
    ["t17.1"] = {  
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pack"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pack"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA40"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
    ["t17.2"] = {   --对话，选择英雄碎片
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pack/pack_view", "core/scroll_view/viewport/content/toggle_item2"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pack/pack_view", "core/scroll_view/viewport/content/toggle_item2"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA41"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
  
    ["t17.3"] = {   --对话，选择第一个碎片
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pack/pack_view", "core/img_list_frame/scroll_view_items/viewport/content/50102"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pack/pack_view", "core/img_list_frame/scroll_view_items/viewport/content/50102"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA42"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
      
    
    ["t17.4"] = {   --对话，选择合成
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pack/pack_view", "core/des_frame/item_des_root/btns_root/btn_compose"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pack/pack_view", "core/des_frame/item_des_root/btns_root/btn_compose"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA43"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    


    ["t17.5"] = {   --对话，选择关闭
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pack/pack_view", "core/img_title/btn_close"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pack/pack_view", "core/img_title/btn_close"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA44"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
   }
}








--打完2-2英雄上阵后使用经验药水


tutorial_chapter[18] = { --主界面任务
  ["at_ui_view_path"] = "ui/main/main_view",
  ["pass_dungeon"] = 10202,
  ["complete_step_id"] = 3,

  ["steps"] = {
   
    ["t18.1"] = {  --
      ["id"] = 1,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA45"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
    },

    -- ["t18.2"] = {  --对话
    --   ["id"] = 2,
    --   ["next_id"] = 3,
    --   ["mask"] = {["enable"] = false},
    --   ["enable_next_step_button"] = true,
    --   ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA46"},
    --   ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    -- },

    ["t18.3"] = {   --对话，经验药水
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA47"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}











--2-4结束返回主城打每日副本



tutorial_chapter[19] = {    --返回主界面
  ["pass_dungeon"] = 10204,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 2,
  ["steps"] = {
  

    ["t19.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t19.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA48"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t19.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t19.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}





   

--打完2-4去每日
tutorial_chapter[20] = { --主界面去每日副本
  ["function_open_id"] = 1700,
  ["pass_dungeon"] = 10204,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,
  
  
  ["steps"] = {
  
  
  
  

    ["t20.1"] = {   --对话，每日副本
      ["id"] = 1,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_activity"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_activity"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2",  ["dialog_content_id"] = "PA49"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
    -- ["t20.2"] = {   --对话
    --   ["id"] = 2,
    --   ["next_id"] = 3,
    --   ["mask"] = {["enable"] = true, ["show"] = false},
    --   ["enable_next_step_button"] = true,
    --   ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA50"},
    --   ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    -- },
  
    ["t20.3"] = {   --对话，指示开启的副本
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_view", "core/Scroll View/Viewport/Content/open_dungeon"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_view", "core/Scroll View/Viewport/Content/open_dungeon"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA51"},
     
    },
  
  
       ["t20.4"] = {   --对话
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA52"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t20.5"] = {   --对话，指示开启的副本
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_info_view", "core/img_right_part_bg/bottom_buttons_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_info_view", "core/img_right_part_bg/bottom_buttons_root/btn_next"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA53"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
  }
}












--打完每日去完成每日任务
tutorial_chapter[21] = { --主界面去每日任务
  ["function_open_id"] = 1700,
  ["pass_dungeon"] = 10204,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,
  
  
  ["steps"] = {
  
    ["t21.1"] = {   --对话，每日副本
      ["id"] = 1,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_task"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_task"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2",  ["dialog_content_id"] = "PA54"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    
    
    -- ["t21.2"] = {   --对话
    --   ["id"] = 2,
    --   ["next_id"] = 3,
    --   ["mask"] = {["enable"] = true, ["show"] = false},
    --   ["enable_next_step_button"] = true,
    --   ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA55"},
    --   ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    -- },
  

    ["t21.3"] = {   --对话，点击任务
      ["id"] = 3,
       ["next_id"] = 4,
       ["mask"] = {["enable"] = true, ["show"] = false},
       ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/task/task_daily_view", "core/right_root/scroll_view/Viewport/Content/20105/btn_complete"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
       ["highlight_ui_path_list"] = {["ui_path"] = {"ui/task/task_daily_view", "core/right_root/scroll_view/Viewport/Content/20105/btn_complete"}},
       -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA56"},
       -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},   
    },
    
   --     ["t21.4"] = {   --对话，提示任务完成
   --     ["id"] = 4,
   --     ["next_id"] = 5,
   --     ["mask"] = {["enable"] = false},
   --     ["enable_next_step_button"] = true,
   --     ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA57"},
   --     ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
   -- },
 }
}






--打完2-6之后装备升星


tutorial_chapter[22] = {
  ["pass_dungeon"] = 10206,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 9,
  
  ["steps"] = {
  
    ["t22.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t22.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA58"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t22.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t22.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  
  }
}





tutorial_chapter[23] = {
  ["pass_dungeon"] = 10206,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,
  
  ["steps"] = {



    ["t23.1"] = {   --对话，选择英雄
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA59"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },

    ["t23.2"] = {   --对话，选择装备穿戴
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA60"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

      ["t23.3"] = {   --对话，选择防具
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA61"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    

    
    ["t23.4"] = {   --对话，选择第一个培养
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA62"},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}},
    },
    
      ["t23.5"] = {   --对话，选择穿戴
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}},
      -- ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      -- ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA63"},
    }, 
  }
}









tutorial_chapter[24] = {--升星
  ["pass_dungeon"] = 10206,
  ["at_ui_view_path"] = "ui/equipments/role_equipments_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {

        ["t24.1"] = {   --对话，peiyang
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA64"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
    


    ["t24.2"] = {   --对话，选择升星
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA65"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },


    ["t24.3"] = {   --对话，升星
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}},
      -- ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA66"},
      -- ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}






--打完2-7引导普通难度


tutorial_chapter[25] = {
  ["pass_dungeon"] = 10207,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 4,
  
  ["steps"] = {
  

    ["t25.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },


    ["t25.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "PA67"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
  
    ["t25.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },

    ["t25.4"] = {  --普通关卡按钮
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/normal_root/btn_normal"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/normal_root/btn_normal"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA68"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

        ["t25.5"] = {   --对话
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA69"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
  }
}



--打完3-1开竞技场
tutorial_chapter[26] = {

  ["pass_dungeon"] = 10301,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 5,
  
  ["steps"] = {

    ["t25.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t26.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA70"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"},["npc_sound"] = "4"},
    },
  
    ["t26.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t26.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },

    ["t26.5"] = {   --对话，选择战斗中心
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
    ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA71"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

    ["t26.6"] = {   --对话，选择竞技场
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA72"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t26.7"] = {   --对话
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA73"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face3"}},
    },
 
    ["t26.8"] = {   --对话，确认成功
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA74"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}







tutorial_chapter[27] = { --打完3-8远征
  ["pass_dungeon"] = 10308,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 7,


  ["steps"] = {
    ["t27.1"] = {   --对话
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA75"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face2"},["npc_sound"] = "4"},
    },
    

  ["t27.2"] = {   --对话，指战斗中心
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA76"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t27.3"] = {   --对话，指远征
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/expedition"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/expedition"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA77"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },


      ["t27.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/expedition/expedition_view::OnViewReady"}, 
    },


   ["t27.5"] = {  --返回大地图
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/expedition/expedition_view", "core/map_root/map_root/ahead_root/dungeon_root/0/normal_Root"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/expedition/expedition_view", "core/map_root/map_root/ahead_root/dungeon_root/0/normal_Root"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA78"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t27.6"] = {  --返回大地图
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA79"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t27.7"] = {  --返回大地图
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA80"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face2"}},
    },

   ["t27.8"] = {  --返回大地图
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA81"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}







tutorial_chapter[28] = { --世界树
    ["function_open_id"] = 1604,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 6,


  ["steps"] = {
    ["t28.1"] = {   --对话
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA82"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face2"},["npc_sound"] = "4"},
    },
    

  ["t28.2"] = {   --对话，指战斗中心
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA83"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t28.3"] = {   --对话，指世界树
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/world_tree"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/world_tree"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "PA84"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },


      ["t28.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/world_tree/world_tree_preview_view::OnViewReady"}, 
    },


   ["t28.5"] = {  --说话
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA85"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t28.6"] = {  --指引打
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "PA86"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/world_tree/world_tree_preview_view", "core/world_tree_content_root/img_world_tree_dungeon_info_panel/btn_challenge"},  ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/world_tree/world_tree_preview_view", "core/world_tree_content_root/img_world_tree_dungeon_info_panel/btn_challenge"}},
    },

  }
}






























































































--[[

--装备强化完成后去竞技场
tutorial_chapter[30] = { --主界面去每日副本
  ["function_open_id"] = 1700,
  ["pass_dungeon"] = 10105,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 4,
  
  

  ["steps"] = {
   
    ["t15.1"] = {   --对话，选择战斗中心
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
    ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE71"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

    ["t15.2"] = {   --对话，选择竞技场
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE72"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t15.3"] = {   --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE73"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
 
    ["t15.4"] = {   --对话，确认成功
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE74"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pvp/pvp_view", "core/pvp_fighter_root"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pvp/pvp_view", "core/pvp_fighter_root"}},
    },
  }
}








tutorial_chapter[4] = { --前往副本，对话，指引，打1-2
  ["pass_dungeon"] = 1,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 11,
  
  ["steps"] = {
  
    ["t4.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
      ["wait_msg_id_list"] = {"1030"}
    },
    
    ["t4.2"] = {
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = false},
      ["wait_msg_id_list"] = {"Fight::OnSecondFightStart"},
      ["on_complete_msg_list"] = {"Fight::FightHangup"}
    },
    
    ["t4.3"] = {  --
      ["id"] = 3,
      ["next_id"] = 4,
      ["delay_time"] = 2,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFightHangupHandler"},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"}
    },
  
  
  
  
    ["t4.4"] = {  --
      ["id"] = 4,
      ["next_id"] = 5,
      ["skill_id"] = 2001,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"}
    },
    
  
    ["t4.5"] = {  --
      ["id"] = 5,
      ["next_id"] = 6,
      ["skill_id"] = 1362,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
      ["on_complete_msg_list"] = {"Fight::StopFightTime"}--战斗时间暂停
    },
    
    

    ["t4.6"] = {  --教程图
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = true}, 
      ["enable_next_step_button"] = true,
      ["illustrate_image"] = {["image"] = "tutorial_01", ["position"] = "250,130"},
    },
  
  
    ["t4.7"] = {   --play skill 
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_player.name.tyro/skillitem_view_1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_player.name.tyro/skillitem_view_1"}},
      ["on_complete_msg_list"] = {"Fight::SetNeedComboPause"}
    },
  

    ["t4.8"] = {   --等待协议
      ["id"] = 8,
      ["next_id"] = 9,
      ["delay_time"] = 1,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["wait_msg_id_list"] = {"Fight::OnComboWaitSettingHandler"},
    },
  
    ["t4.9"] = {   --play skill ----暂停后继续
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.yingli/skillitem_view_2"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.yingli/skillitem_view_2"}},
      ["on_complete_msg_list"] = {"Fight::OnComboWaitingHandler","Fight::ReSetNeedComboPause","Fight::FightRegainOrder","Fight::ReStartFightTime"}  --恢复战斗时间
    },
    
    ["t4.10"] = {   --start combo 
      ["id"] = 10,
      ["next_id"] = 11,
      ["expand_data"] = {["startCombo"] = 1},
      ["wait_msg_id_list"] = {"Fight::OnFightRegainOrder"},
    },
        
    ["t4.11"] = {   --等待协议
      ["id"] = 11,
      ["next_id"] = 12,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
  
    ["t4.12"] = {  --返回大地图
      ["id"] = 12,
      ["next_id"] = 13,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}},
    }
  }
}













tutorial_chapter[5] = {     --亏刚上阵
  ["pass_dungeon"] = 2,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {
  
    ["t5.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = { "ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}},
    },
    
    ["t5.2"] = {  --选中英雄
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/1"}}
    },
    
    ["t5.3"] = {  --放入位置
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["arrow_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_2/formation_base_button"}}
    },
  
    ["t5.4"] = {  --返回战斗界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}},
    }
  }
}













tutorial_chapter[6] = {    -- 1-3
  ["pass_dungeon"] = 2,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 6,
  
  ["steps"] = {
    ["t6.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0, 0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
      ["wait_msg_id_list"] = {"1030"}
    },
    
    ["t6.2"] = {   --战斗暂停
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = false},
      ["wait_msg_id_list"] = {"Fight::OnSecondFightStart"},
      ["on_complete_msg_list"] = {"Fight::FightHangup","Fight::StopFightTime"}   
    },
  
    ["t6.3"] = {   --满cd
      ["id"] = 3,
      ["next_id"] = 4,
      ["delay_time"] = 2,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFightHangupHandler"},
      ["on_complete_msg_list"] = {"Fight::FullSkillCD"}
    },
  
    ["t6.4"] = {   --开始引导
      ["id"] = 4,
      ["next_id"] = 5,
      ["skill_id"] = 31,
      ["mask"] = {["enable"] = true},
      ["wait_msg_id_list"] = {"Fight::OnFullSkillCDHandler"},
    },
    
    
    ["t6.5"] = {   --play skill 
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.wolf3/skillitem_view_1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/skillbar/skill_bar_view", "core/skill_root/sheet_hero.name.wolf3/skillitem_view_1"}},
      ["on_complete_msg_list"] = {"Fight::FightRegainOrder","Fight::ReStartFightTime"},
    },

    ["t6.6"] = {   --等待协议
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t6.7"] = {  --下一关
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}},
    },
  }
}













tutorial_chapter[7] = {    -- 1-4，开始战斗，出来引导强化
  ["player_level"] = 4,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 1,
  
  ["steps"] = {
    
    ["t7.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = { "ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },
  

    ["t7.2"] = {   --等待协议
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
  
    ["t7.3"] = {  --返回大地图
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_next"}},
    },


  }
}













tutorial_chapter[8] = {    -- 强化引导 --
  ["player_level"] = 1,
  ["pass_dungeon"] = 4,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 6,
  
  ["steps"] = {
  

    ["t8.1"] = {   --对话，选择培养
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/btn_embattle"}},
          ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE24"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
    
        ["t8.2"] = {   --对话，选择阵型中的英雄培养
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/right_panel_root/right_root/right_panel/btn_roles_info"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/right_panel_root/right_root/right_panel/btn_roles_info"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE25"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
  
    
        ["t8.3"] = {   --对话，选择狼
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/3"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE26"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
    
    ["t8.4"] = {   --对话，选择英雄强化
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_hero_strengthen"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_hero_strengthen"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE27"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t8.5"] = {   --对话，选择材料
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthenAutoPut"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthenAutoPut"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE28"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
  
    ["t8.6"] = {   --开始强化
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthen"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/left_part/img_attributes_frame/btn_strengthen"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE29"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
     
    ["t8.7"] = {   --开始强化
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE30"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },
        
    ["t8.8"] = {   --返回
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_strengthen/hero_strengthen_view", "core/common_top_bar(Clone)/btn_back"}},
    },
    
        ["t8.9"] = {   --返回
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/common_top_bar(Clone)/btn_back"}},
    },
         
         
    ["t8.10"] = {   --返回
      ["id"] = 10,
      ["next_id"] = 11,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}},
    },

    ["t8.11"] = {   --开始战斗
      ["id"] = 11,
      ["next_id"] = 12,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },
  }
}




















tutorial_chapter[9] = {    -- 1-6
  ["player_level"] = 4,
  ["pass_dungeon"] = 6,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 1,
  
  ["steps"] = {
    
  
    ["t9.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t9.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
    },
  
    ["t9.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t9.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    }
  }
}













tutorial_chapter[10] = { --抽卡，对话
  ["pass_dungeon"] = 6,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["function_open_id"] = 1100,
  ["complete_step_id"] = 4,

  ["steps"] = {
    ["t10.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE31"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},   
    },

    ["t10.2"] = {  --商店
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_shop"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_shop"}}
    },

    ["t10.3"] = {  --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE32"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70", ["npc_show"] = {"nv_pu", "face3"}},
    },
    
    ["t10.4"] = {  --单抽
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/shop_view", "core/img_frame/normal_shop/scrollview/viewport/content/0/img_bottom_bar/btn_buy"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/shop_view", "core/img_frame/normal_shop/scrollview/viewport/content/0/img_bottom_bar/btn_buy"}},
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_hero_display_view::OnViewReady"},
    },


    ["t10.5"] = {  --对话
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE33"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },


    ["t10.6"] = {  --返回1
      ["id"] = 6,
      ["next_id"] = 7,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_hero_display_view::OnViewClose"},

    },


    ["t10.7"] = {  --返回2
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/single_draw_card_result_view", "core/bottom_bar/buttons_root/btn_back"},["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/single_draw_card_result_view", "core/bottom_bar/buttons_root/btn_back"}},
    },
  
    ["t10.8"] = {  --返回主界面
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/shop/shop_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/shop/shop_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  }
}



--上面是引导第1次抽卡










tutorial_chapter[11] = {    -- 这里是吃经验药水引
  ["player_level"] = 4,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {
    
    ["t11.1"] = {   --对话，选择培养
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE34"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  
  
    ["t11.2"] = {   --对话，选择第四个人
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/119"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/119"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE35"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
     
    
    ["t11.3"] = {   --对话，经验药水
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE36"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  
  
    ["t11.4"] = {   --对话，选择经验药水
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/exp_Medicine_root/small"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE37"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}



--上面是引导谢拉尔吃经验药水





--下面是引导谢拉尔上阵



tutorial_chapter[12] = { --上阵，对话
  ["pass_dungeon"] = 6,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 7,
  
  ["steps"] = {
    ["t12.1"] = {
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE38"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },
    
    
    ["t12.2"] = {  --阵型按钮
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}}
    },


    ["t12.3"] = {  --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "EE39"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },


  
        ["t12.4"] = {  --教程图
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = true}, 
      ["enable_next_step_button"] = true,
      ["illustrate_image"] = {["image"] = "tutorial_02", ["position"] = "250,130"},
    },
  
  
  
    ["t12.5"] = {  --选中英雄
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/2"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/Scroll View/Viewport/Content/2"}}
    },


    ["t12.6"] = {  --对话
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE40"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t12.7"] = {  --放入位置
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["arrow_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_6/formation_base_button"}},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/formation_root/formation_grid_view(Clone)/formation_6/formation_base_button"}}
    },


    ["t12.8"] = {  --对话，图片教学换位置
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE41"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t12.9"] = {  --返回主界面
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}},
    },
  
  }
}



--上面是引导谢拉尔上阵




--下面是引导1-7



tutorial_chapter[13] = {    -- 1-7
  ["pass_dungeon"] = 6,
  ["at_ui_view_path"] = "ui/chapter/select_chapter_view",
  ["complete_step_id"] = 4,
  
  ["steps"] = {
    
    ["t13.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_7"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_7"}},
    },
    
    ["t13.2"] = {
      ["id"] = 2,
      ["next_id"] = 3,  
      ["mask"] = {["enable"] = false, ["show"] = false},
      ["wait_msg_id_list"] = {"ui/fight_tips/fight_tips_view::OnViewReady"},  
    },

    ["t13.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["wait_msg_id_list"] = {"Fight::OnFirstFightStart"},
      ["masked_ui_path_list"] = {{"ui/fight_tips/fight_tips_view", "core/top_right_anchor/btn_auto_fight"}}, 
      ["on_complete_msg_list"] = {"Fight::FightHangup","Fight::StopFightTime"},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE42"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
    ["t13.4"] = {
      ["id"] = 4,
      ["next_id"] = 5,  
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["wait_msg_id_list"] = {"Fight::OnFightHangupHandler"},   
    },
    
    ["t13.5"] = {   --指引自动战斗
      ["id"] = 5,
      ["next_id"] = 6,  
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_tips/fight_tips_view", "core/top_right_anchor/btn_auto_fight"}}, --btn_set_speed
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE43"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
      ["force_complete_msg_id_list"] = {"ui/fight_tips/fight_tips_view::OnViewClose"},
      ["on_complete_msg_list"] = {"Fight::FightRegainOrder","Fight::ReStartFightTime"}
    },
    
    ["t13.6"] = {   --start combo 
      ["id"] = 6,
      ["next_id"] = 7,
      ["wait_msg_id_list"] = {"Fight::OnFightRegainOrder"},
    },
    
  }
}









tutorial_chapter[14] = {    -- 1-8战斗加速引导
  ["player_level"] = 4,
  ["pass_dungeon"] = 7,
  ["at_ui_view_path"] = "ui/dungeon_detail/dungeon_detail_view",
  ["complete_step_id"] = 4,
  
    ["steps"] = {
    
    ["t14.1"] = {  
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },


    ["t14.2"] = {
      ["id"] = 2,
      ["next_id"] = 3,  
      ["mask"] = {["enable"] = false, ["show"] = false},
      ["wait_msg_id_list"] = {"ui/fight_tips/fight_tips_view::OnViewReady"},  
    },

    ["t14.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["wait_msg_id_list"] = {"Fight::OnFirstFightStart"},
      ["masked_ui_path_list"] = {{"ui/fight_tips/fight_tips_view", "core/top_right_anchor/btn_auto_fight"}}, 
      ["on_complete_msg_list"] = {"Fight::FightHangup","Fight::StopFightTime"},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE44"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
    ["t14.4"] = {
      ["id"] = 4,
      ["next_id"] = 5,  
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["wait_msg_id_list"] = {"Fight::OnFightHangupHandler"},   
    },
    
    ["t14.5"] = {   --指引自动战斗
      ["id"] = 5,
      ["next_id"] = 6,  
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_tips/fight_tips_view", "core/top_right_anchor/btn_set_speed"}}, --btn_set_speed
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE45"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
      ["force_complete_msg_id_list"] = {"ui/fight_tips/fight_tips_view::OnViewClose"}, --有问题
      ["on_complete_msg_list"] = {"Fight::FightRegainOrder","Fight::ReStartFightTime"}
    },
    
    ["t14.6"] = {   --start combo 
      ["id"] = 6,
      ["next_id"] = 7,
      ["wait_msg_id_list"] = {"Fight::OnFightRegainOrder"},
    },
  }
}













tutorial_chapter[15] = {    -- 1-11
  ["player_level"] = 4,
  ["pass_dungeon"] = 11,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 9,

  ["steps"] = {
  
    ["t15.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t15.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
    },
  
    ["t15.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t15.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },  
    
    ["t15.5"] = {   --对话，选择升星
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE46"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t15.6"] = {   --对话，选择第3个位置
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/200"},  ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_list_root/Scroll View/Viewport/Content/200"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE47"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face4"}},
    },

    ["t15.7"] = {   --对话，选择英雄强化
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_player_advance"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_player_advance"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE48"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t15.8"] = {   --对话，选择材料
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE49"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
    ["t15.9"] = {   --对话
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/hero_advance/hero_advance_view", "core/center_part_root/img_attributes_frame/img_material_bg/advance_operation_root/btn_advance"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/hero_advance/hero_advance_view", "core/center_part_root/img_attributes_frame/img_material_bg/advance_operation_root/btn_advance"}},
    },
  }
}
    
    
    
    
    
    
    
    





tutorial_chapter[16] = {    -- 1-12
  ["player_level"] = 4,
  ["pass_dungeon"] = 12,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 12,
  
  
  
  ["steps"] = {
  
    ["t16.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t16.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
    },
  
    ["t16.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t16.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },  
    
    ["t16.5"] = {
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE50"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },
    
    
    ["t16.6"] = {  --阵型按钮
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_pve_embattle"}}
    },


    ["t16.7"] = {  --对话
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "EE51"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t16.8"] = {  --选中换阵
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/btn_formation"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/formation_team_view(Clone)/btn_formation"}},
      ["wait_msg_id_list"] = {"ui/train_formation/train_formation_view::OnViewReady"},
    },


    ["t16.9"] = {  --对话
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "EE52"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
      },


    ["t16.10"] = {  --选择阵型
      ["id"] = 10,
      ["next_id"] = 11,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/formation_list_frame/img_inner_frame/Scroll View/Viewport/Content/2"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/formation_list_frame/img_inner_frame/Scroll View/Viewport/Content/2"}}
    },


    ["t16.11"] = {  --对话
      ["id"] = 11,
      ["next_id"] = 12,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE53"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },
  
    ["t16.12"] = {  --对话,培养
      ["id"] = 12,
      ["next_id"] = 13,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/img_right_frame/btn_use"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/img_right_frame/btn_use"}}
    },

    ["t16.13"] = {  --使用阵型
      ["id"] = 13,
      ["next_id"] = 14,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE54"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},   
    },
    
    ["t16.14"] = {  --对话,培养
      ["id"] = 14,
      ["next_id"] = 15,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/img_right_frame/btn_train"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/img_right_frame/btn_train"}},
    },
  
    ["t16.15"] = {
      ["id"] = 15,
      ["next_id"] = 16,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft" , ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/train_formation/train_formation_view", "core/common_top_bar(Clone)/btn_back"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "AB37"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    
    },
    
    ["t16.16"] = {
      ["id"] = 16,
      ["next_id"] = 17,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}, ["direction"] = "UpLeft" , ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pve_embattle/pve_embattle_view_lua", "core/common_top_bar(Clone)/btn_back"}}
    }
  }
}













tutorial_chapter[17] = {
  ["player_level"] = 4,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 4,
  
  ["steps"] = {
  
  
    ["t17.1"] = {  --点击关卡开始按钮
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_start_fight"}},
    },
    
    ["t17.2"] = {  --普通关卡按钮
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/btn_normal"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/difficulty_buttons_root/btn_normal"}},
    },
    
    ["t17.3"] = {  --点击第一关
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_1"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/scroll_view/viewport/content/dungeon_buttons_root/dungeon_button_1"}},
    },
    
    ["t17.4"] = {  
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = { "ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight"}},
    },
  }
}


















tutorial_chapter[18] = {    --穿装备

  ["player_level"] = 2,
  ["pass_dungeon"] = 21,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 8,
  ["steps"] = {
  

    ["t18.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t18.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE60"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t18.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t18.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
    
    ["t18.5"] = {   --对话，选择培养
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE61"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    
    
    ["t18.6"] = {   --对话，选择英雄装备
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE62"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
  
    ["t18.7"] = {   --对话，选择英雄装备
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE63"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
      
    
    ["t18.8"] = {   --对话，选择穿戴
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE64"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    }, 
    
  }
}













tutorial_chapter[19] = { --装备强化
 
  ["pass_dungeon"] = 21,
  ["at_ui_view_path"] = "ui/equipments/role_equipments_view",
  ["complete_step_id"] = 2,
  
  ["steps"] = {

    ["t19.1"] = {   --对话
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE66"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      },
    
    ["t19.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/strengthen_panel(Clone)/root/btn_strengthen_more"}},
      },


    ["t19.3"] = {   --对话
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE80"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}











tutorial_chapter[20] = { --主界面去每日副本
  ["function_open_id"] = 1700,
  ["pass_dungeon"] = 21,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 5,
  
  
  ["steps"] = {
  
  
  
  

    ["t20.1"] = {   --对话，每日副本
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_activity"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_daily_activity"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2",  ["dialog_content_id"] = "EE55"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },
    
    
    ["t20.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE56"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t20.3"] = {   --对话，指示开启的副本
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_view", "core/Scroll View/Viewport/Content/open_dungeon"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_view", "core/Scroll View/Viewport/Content/open_dungeon"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE57"},
     
    },
  
  
       ["t20.4"] = {   --对话
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE58"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t20.5"] = {   --对话，指示开启的副本
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_info_view", "core/img_right_part_bg/bottom_buttons_root/btn_next"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/daily_dungeon/daily_dungeon_info_view", "core/img_right_part_bg/bottom_buttons_root/btn_next"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE59"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
    
  
  }
}










tutorial_chapter[21] = {
  ["pass_dungeon"] = 23,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 9,
  
  ["steps"] = {
  
    ["t21.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"},
      ["mask"] = {["enable"] = false, ["show"] = false},   
    },

    ["t21.2"] = {   --对话
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},   
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE67"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      },
   
    ["t21.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t21.4"] = {  --对话，点击返回
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false}, 
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },
  

    ["t21.5"] = {   --对话，选择英雄
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_left_anchor/bottom/btn_hero"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "AB23"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t21.6"] = {   --对话，选择装备穿戴
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/role/role_info_view", "core/role_info_panel/role_function_buttons_root/btn_role_equipment"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "AB24"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

      ["t21.7"] = {   --对话，选择防具
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/armor_frame"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "AB30"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    

    
    ["t21.8"] = {   --对话，选择第一个培养
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "AB25"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/img_equipments_frame/scroll_rect/content/0"}},
    },
    
      ["t21.9"] = {   --对话，选择穿戴
      ["id"] = 9,
      ["next_id"] = 10,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_put_on"}},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    }, 
  }
}






tutorial_chapter[22] = {--升星
  ["pass_dungeon"] = 23,
  ["at_ui_view_path"] = "ui/equipments/role_equipments_view",
  ["complete_step_id"] = 3,
  
  ["steps"] = {

        ["t22.1"] = {   --对话，peiyang
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/role_equipments_view", "core/left_part/img_selected_equipment_info_frame/btn_train"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] =  "-205, -2", ["dialog_content_id"] = "EE77"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },
    


    ["t22.2"] = {   --对话，选择升星
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}, ["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/toggle_group/toggle_upStar"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE78"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },


    ["t22.3"] = {   --对话，升星
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}, ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/equipments/equipment_training_view", "core/right/training_function_root/up_star_panel(Clone)/attr/btn/btn_up_star"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE79"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}



























tutorial_chapter[23] = {

  ["pass_dungeon"] = 25,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 6,
  
  ["steps"] = {

    ["t23.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t23.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
    },
  
    ["t23.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },
  
    ["t23.4"] = {  --返回主界面
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"},["direction"] = "UpLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/common_top_bar(Clone)/btn_back"}},
    },

    ["t23.5"] = {   --对话，选择战斗中心
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
    ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE71"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },

    ["t23.6"] = {   --对话，选择竞技场
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/pvp"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE72"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
    },

    ["t23.7"] = {   --对话
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE73"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
 
    ["t23.8"] = {   --对话，确认成功
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE74"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70",["flip"] = "true", ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/pvp/pvp_view", "core/pvp_fighter_root"}, ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/pvp/pvp_view", "core/pvp_fighter_root"}},
    }
  }
}








tutorial_chapter[24] = { --Boss列表
  ["complete_step_id"] = 3,
  ["pass_dungeon"] = 32,
  ["at_ui_view_path"] = "ui/fight_result/fight_result_view",
  ["complete_step_id"] = 6,

  ["steps"] = {
  ["t24.1"] = {   --等待协议
      ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false, ["show"] = false},  
      ["wait_msg_id_list"] = {"ui/fight_result/fight_result_view::OnViewStay"}, 
    },
    
    ["t24.2"] = {  --返回大地图
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/fight_result/fight_result_view", "core/win/win_extra_root/btn_back"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE90"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },
  
    ["t24.3"] = {   --等待协议
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/chapter/select_chapter_view::OnViewReady"}, 
    },


    ["t24.4"] = {   --对话，指Boss列表
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/btn_boss_dungeon_list"},  ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/chapter/select_chapter_view", "core/btn_boss_dungeon_list"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE86"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
    ["t24.5"] = {   --对话
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE87"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },
  
      ["t24.6"] = {   --对话，指挑战
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE88"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/boss_dungeon_list/boss_dungeon_list_view", "core/frame/scroll_view/viewport/content/0/btn_challenge"},  ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/boss_dungeon_list/boss_dungeon_list_view", "core/frame/scroll_view/viewport/content/0/btn_challenge"}},
    },
      

       ["t24.7"] = {   --对话，指
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE89"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight_boss_species_dungeon"},  ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/dungeon_detail/dungeon_detail_view", "core/right/challenge_button_and_remain_times_root/btn_start_fight_boss_species_dungeon"}},
    },
  
  
  }
}












tutorial_chapter[25] = { --远征
  ["function_open_id"] = 1606,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 7,


  ["steps"] = {
    ["t25.1"] = {   --对话
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE81"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face2"}},
    },
    

  ["t25.2"] = {   --对话，指战斗中心
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE82"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t25.3"] = {   --对话，指远征
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/expedition"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/expedition"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE83"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face3"}},
    },


      ["t25.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/expedition/expedition_view::OnViewReady"}, 
    },


   ["t25.5"] = {  --返回大地图
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/expedition/expedition_view", "core/map_root/map_root/ahead_root/dungeon_root/0/normal_Root"}, ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/expedition/expedition_view", "core/map_root/map_root/ahead_root/dungeon_root/0/normal_Root"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE92"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t25.6"] = {  --返回大地图
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE93"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t25.7"] = {  --返回大地图
      ["id"] = 7,
      ["next_id"] = 8,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE94"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face2"}},
    },

   ["t25.8"] = {  --返回大地图
      ["id"] = 8,
      ["next_id"] = 9,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE95"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face3"}},
    },

  }
}







tutorial_chapter[26] = { --世界树
  ["function_open_id"] = 1604,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 6,


  ["steps"] = {
    ["t26.1"] = {   --对话
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE96"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face2"}},
    },
    

  ["t26.2"] = {   --对话，指战斗中心
      ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/main/main_view", "core/bottom_right_anchor/btn_multiple"}},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE97"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },


    ["t26.3"] = {   --对话，指世界树
      ["id"] = 3,
      ["next_id"] = 4,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/world_tree"},  ["direction"] = "UpRight", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/multiple_fight/multiple_fight_view", "core/Scroll View/Viewport/Content/world_tree"}},
      ["dialog"] = {["anchor"] = "BottomRight", ["offset"] = "-205, -2", ["dialog_content_id"] = "EE98"},
      ["npc"] = {["anchor"] = "BottomRight", ["offset"] = "-310, -70", ["flip"] = "true",["npc_show"] = {"nv_pu", "face1"}},
    },


      ["t26.4"] = {   --等待协议
      ["id"] = 4,
      ["next_id"] = 5,
      ["mask"] = {["enable"] = false, ["show"] = false},   
      ["wait_msg_id_list"] = {"ui/world_tree/world_tree_preview_view::OnViewReady"}, 
    },


   ["t26.5"] = {  --说话
      ["id"] = 5,
      ["next_id"] = 6,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE99"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },

   ["t26.6"] = {  --指引打
      ["id"] = 6,
      ["next_id"] = 7,
      ["mask"] = {["enable"] = true, ["show"] = false},
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE100"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
      ["hand_indicate_ui_path"] = {["ui_path"] = {"ui/world_tree/world_tree_preview_view", "core/world_tree_content_root/img_world_tree_dungeon_info_panel/btn_challenge"},  ["direction"] = "DownLeft", ["offset"] = "0,0"},
      ["highlight_ui_path_list"] = {["ui_path"] = {"ui/world_tree/world_tree_preview_view", "core/world_tree_content_root/img_world_tree_dungeon_info_panel/btn_challenge"}},
    },

  }
}








tutorial_chapter[27] = { --矿战
  ["function_open_id"] = 1611,
  ["at_ui_view_path"] = "ui/main/main_view",
  ["complete_step_id"] = 2,


  ["steps"] = {
    ["t27.1"] = {   --对话
    ["id"] = 1,
      ["next_id"] = 2,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE75"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },
    

    ["t27.2"] = {   --对话
    ["id"] = 2,
      ["next_id"] = 3,
      ["mask"] = {["enable"] = false},
      ["enable_next_step_button"] = true,
      ["dialog"] = {["anchor"] = "BottomLeft", ["offset"] = "208, -2", ["dialog_content_id"] = "EE76"},
      ["npc"] = {["anchor"] = "BottomLeft", ["offset"] = "-30, -70",  ["npc_show"] = {"nv_pu", "face1"}},
    },
  }
}


--]]





