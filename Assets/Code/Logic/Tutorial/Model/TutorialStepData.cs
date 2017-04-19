using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;

namespace Logic.Tutorial.Model
{
    public class TutorialStepData
    {
        public int id = 0;
        public int nextID = 0;
        public Dictionary<string, string> expandDataDic = new Dictionary<string, string>();

        // In Fight
        public float delayTime = 0;
        public int heroInstanceID = 0;
        public int skillID = 0;
        public int damage = 0;
        // In Fight

        // Mask
        public bool enableMask = false;
        public bool showMask = false;
        // Mask

        public bool enableNextStepButton = false;

        // Dialog
        public bool showDialog = false;
        public string dialogAnchor;
        public Vector2 dialogOffset = Vector2.zero;
        public string dialogContentID;
        // Dialog

        // NPC
        public bool showNPC;
        public string npcAnchor;
        public Vector2 npcOffset = Vector2.zero;
        public bool npcFlip = false;
        public string npcShowID;
        public string npcFace;
		public int npcSoundId;
        // NPC

        // Illustrate
        public bool showIllustrateImage = false;
        public string illustrateImageName;
        public Vector3 illustrateImageAnchoredPosition;
        // Illustrate

        // Hand Indicate
        public bool showHandIndicator = false;
        public string handIndicateDirection = "DownLeft";
        public Vector2 handIndicatorOffset = new Vector2(0, 50);
        public string[] handIndicateUIPath;
        // Hand Indicate

        // Arrow Indicate
        public bool showArrowIndicator = false;
        public string arrowIndicateDirection = "Left";
        public Vector2 arrowIndicatorOffset = new Vector2(50, 0);
        public string[] arrowIndicateUIPath;
        // Arrow Indicate

        public string[] highlightUIPath = null;
		public Vector2 highlightUIOffset = Vector2.zero;
		public List<string[]> maskedUIPathList = new List<string[]>();
        public List<string> waitMSGIDList = new List<string>();
		public List<string> forceCompleteMSGIDList = new List<string>();
		public List<string> onCompleteMSGList = new List<string>();

        public static TutorialStepData CreateFromLuaTable(LuaTable luaTable)
        {
            TutorialStepData tutorialStepData = new TutorialStepData(luaTable);
            return tutorialStepData;
        }

        public TutorialStepData(LuaTable stepLuaTable)
        {
            if (stepLuaTable["id"] != null)
                id = stepLuaTable["id"].ToString().ToInt32();
            if (stepLuaTable["next_id"] != null)
                nextID = stepLuaTable["next_id"].ToString().ToInt32();
            if (stepLuaTable["expand_data"] != null)
            {
                LuaTable luaTable = (LuaTable)stepLuaTable["expand_data"];
                foreach (DictionaryEntry kvp in luaTable.ToDictTable())
                {
                    expandDataDic.Add(kvp.Key.ToString(), kvp.Value.ToString());
                }
            }
            if (stepLuaTable["delay_time"] != null)
                delayTime = stepLuaTable["delay_time"].ToString().ToFloat();
            if (stepLuaTable["hero_instance_id"] != null)
                heroInstanceID = stepLuaTable["hero_instance_id"].ToString().ToInt32();
            if (stepLuaTable["skill_id"] != null)
                skillID = stepLuaTable["skill_id"].ToString().ToInt32();
            if (stepLuaTable["damage"] != null)
                damage = stepLuaTable["damage"].ToString().ToInt32();
            if (stepLuaTable["mask"] != null)
            {
                LuaTable luaTable = (LuaTable)stepLuaTable["mask"];
                if (luaTable["enable"] != null)
                    enableMask = luaTable["enable"].ToString().ToBoolean();
                if (luaTable["show"] != null)
                    showMask = luaTable["show"].ToString().ToBoolean();
            }
            if (stepLuaTable["enable_next_step_button"] != null)
                enableNextStepButton = stepLuaTable["enable_next_step_button"].ToString().ToBoolean();
            if (stepLuaTable["dialog"] != null)
            {
                showDialog = true;
                LuaTable luaTable = (LuaTable)stepLuaTable["dialog"];
                dialogAnchor = luaTable["anchor"].ToString();
                dialogOffset = luaTable["offset"].ToString().ToVector2();
                dialogContentID = luaTable["dialog_content_id"].ToString();
            }
            if (stepLuaTable["npc"] != null)
            {
                showNPC = true;
                LuaTable luaTable = (LuaTable)stepLuaTable["npc"];
                npcAnchor = luaTable["anchor"].ToString();
                npcOffset = luaTable["offset"].ToString().ToVector2();
                if (luaTable["flip"] != null)
                {
                    npcFlip = luaTable["flip"].ToString().ToBoolean();
                }
                npcShowID = ((LuaTable)luaTable["npc_show"]).ToArray<string>()[0];
                npcFace = ((LuaTable)luaTable["npc_show"]).ToArray<string>()[1];
				if (luaTable["npc_sound"] != null)
				{
					npcSoundId = luaTable["npc_sound"].ToString().ToInt32();
				}
            }
            if (stepLuaTable["illustrate_image"] != null)
            {
                showIllustrateImage = true;
                LuaTable luaTable = (LuaTable)stepLuaTable["illustrate_image"];
                illustrateImageName = luaTable["image"].ToString();
                illustrateImageAnchoredPosition = luaTable["position"].ToString().ToVector2();
            }
            if (stepLuaTable["hand_indicate_ui_path"] != null)
            {
                showHandIndicator = true;
                LuaTable luaTable = (LuaTable)stepLuaTable["hand_indicate_ui_path"];
                if (luaTable["direction"] != null)
                    handIndicateDirection = luaTable["direction"].ToString();
                if (luaTable["offset"] != null)
                    handIndicatorOffset = luaTable["offset"].ToString().ToVector2();
                handIndicateUIPath = ((LuaTable)luaTable["ui_path"]).ToArray<string>();
            }
            if (stepLuaTable["arrow_indicate_ui_path"] != null)
            {
                showArrowIndicator = true;
                LuaTable luaTable = (LuaTable)stepLuaTable["arrow_indicate_ui_path"];
                if (luaTable["direction"] != null)
                    arrowIndicateDirection = luaTable["direction"].ToString();
                if (luaTable["offset"] != null)
                    arrowIndicatorOffset = luaTable["offset"].ToString().ToVector2();
                arrowIndicateUIPath = ((LuaTable)luaTable["ui_path"]).ToArray<string>();
            }
            if (stepLuaTable["highlight_ui_path_list"] != null)
            {
//                LuaTable luaTable = (LuaTable)stepLuaTable["highlight_ui_path_list"];
//                foreach (LuaTable pathLuaTable in luaTable.ToArrayTable())
//                {
//                    highlightUIPathList.Add(pathLuaTable.ToArray<string>());
//                }

				LuaTable luaTable = (LuaTable)stepLuaTable["highlight_ui_path_list"];
				highlightUIPath = ((LuaTable)luaTable["ui_path"]).ToArray<string>();
				if (luaTable["offset"] != null)
					highlightUIOffset = luaTable["offset"].ToString().ToVector2();
            }
			if (stepLuaTable["masked_ui_path_list"] != null)
			{
				LuaTable luaTable = (LuaTable)stepLuaTable["masked_ui_path_list"];
				foreach (LuaTable pathLuaTable in luaTable.ToArrayTable())
				{
					maskedUIPathList.Add(pathLuaTable.ToArray<string>());
				}
			}
            if (stepLuaTable["wait_msg_id_list"] != null)
            {
                LuaTable luaTable = (LuaTable)stepLuaTable["wait_msg_id_list"];
                waitMSGIDList = new List<string>(luaTable.ToArray<string>());
            }
			if (stepLuaTable["force_complete_msg_id_list"] != null)
			{
				LuaTable luaTable = (LuaTable)stepLuaTable["force_complete_msg_id_list"];
				forceCompleteMSGIDList = new List<string>(luaTable.ToArray<string>());
			}
			if (stepLuaTable["on_complete_msg_list"] != null)
			{
				LuaTable luaTable = (LuaTable)stepLuaTable["on_complete_msg_list"];
				onCompleteMSGList = new List<string>(luaTable.ToArray<string>());
			}
        }

        public string GetExpandData(string key)
        {
            string result = string.Empty;
            expandDataDic.TryGetValue(key, out result);
            return result;
        }
    }
}
