local t = {}
local PREFAB_PATH = 'ui/player_information_check/player_information_check_simple_view'
local name = PREFAB_PATH


local global_data = gamemanager.GetData('global_data')
local hero_data = gamemanager.GetData('hero_data')
local common_hero_icon = require('ui/common_icon/common_hero_icon')
--参数playerInfomationInfo:玩家信息[参考player_infomation_info.lua]
function t.Open(playerInfomationInfo,fightCallback)
  
  uimanager.RegisterView(name,t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.playerInfomationInfo = playerInfomationInfo
  t.fightCallback = fightCallback
  t.InitComponent()
  t.Refresh()
end

function t.Close()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

function t.InitComponent()
  local frame = t.transform:Find('core/frame')
  t.btnClose = ui_util.FindComp(frame,'btn_close',Button)
  t.btnClose.onClick:AddListener(t.Close)
  t.btnFight = ui_util.FindComp(frame,'btn_fight',Button)
  t.btnFight.onClick:AddListener(function()
      t.Close()
      if t.fightCallback then
        t.fightCallback(t.playerInfomationInfo)
      end
    end)
  t.imgIcon = ui_util.FindComp(frame,'img_icon',Image)
  t.textLevel = ui_util.FindComp(frame,'text_lv',Text)
  t.textName = ui_util.FindComp(frame,'text_name',Text)
  t.textPower = ui_util.FindComp(frame,'text_power',Text)
  t.heroRoot = ui_util.FindComp(frame,'hero_root',Transform)
  t.textFormation = ui_util.FindComp(frame,'text_formation',Text)
end

function t.Refresh()
  
      t.imgIcon.sprite = ResMgr.instance:LoadSprite(t.playerInfomationInfo.headIconPath)
			t.textLevel.text = string.format(LocalizationController.instance:Get("common.role_icon.common_lv"),t.playerInfomationInfo.level)
			t.textName.text = t.playerInfomationInfo.name
			t.textPower.text = t.playerInfomationInfo:Power()
      t.textFormation.text = LocalizationController.instance:Get( t.playerInfomationInfo.formationInfo.formationData.name)
      ui_util.ClearChildren(t.heroRoot,true)
      for k,v in pairs(t.playerInfomationInfo.roleCheckInfoDictionary:GetDatas()) do
        local icon = common_hero_icon.New(t.heroRoot)
        icon:SetRoleInfo(v.roleInfo)
        icon:AddRoleDesButtonByOtherPlayer(false)
      end  
end

----------------------------click event-------------------------
return t