local t = {}
local PREFAB_PATH = 'ui/tips/common_reward_tips_view_lua'
t.__index=t

local game_res_data=require('ui/game/model/game_res_data')

--四段式列表(gameResData)
function t.Create(dataList)

  t.transform=Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay).transform
  t.rewardIconPrefab=t.transform:FindChild('core/reward_templete').gameObject
  t.rewardIconRoot=t.transform:FindChild('core/Scroll View/content')
  t.transform:FindChild('core/title/text_name'):GetComponent(typeof(Text)).text = LuaInterface.LuaCsTransfer.LocalizationGet('event_public_award')
  t.btnClose = t.transform:FindChild('bg_core/btn_background'):GetComponent(typeof(Button))
  t.btnClose.onClick:AddListener(t.OnClickCloseHandler)
  
  coroutine.start(t.ShowBtnCoroutine)
  
  t.SetDataList(dataList)
  return t
end

--四段式列表(字符串)
function t.CreateOriginal(list)
  
  local datas = {}
  for i=1,#list do
    table.insert(datas,game_res_data.NewByString(list[i]))
  end
  return t.Create(datas)
end
--四段式列表(c# gameResData)
function t.CreateByCSharpGameResDataList(cSharpResDataList)
  local count = cSharpResDataList.Count
  local datas = {}
  for i = 0,count-1 do
    local resData = cSharpResDataList:get_Item(i)
    table.insert(datas,game_res_data.New(resData.type:ToInt(),resData.id,resData.count,resData.star))
  end
  return t.Create(datas)
end

function t.SetDataList(dataList)
  if not dataList then return end
  ui_util.ClearChildren(t.rewardIconRoot,true)
  local count = #dataList
  
  for  i=1,count do
    local clone = Instantiate(t.rewardIconPrefab)
    local resData = dataList[i]

    local icon = require('ui/common_icon/common_reward_icon').New(clone.transform,resData)
    icon:AddDesButton()

    local iconTran = icon.transform:GetComponent(typeof(RectTransform))
    iconTran:SetAsFirstSibling()
    local tran = clone.transform
    tran:SetParent(t.rewardIconRoot,false)
    tran.gameObject:SetActive(true)
    
    local countText = tran:FindChild("text_count"):GetComponent(typeof(Text))
    local nameText = tran:FindChild("text_name"):GetComponent(typeof(Text))
    countText.text = string.format(LuaInterface.LuaCsTransfer.LocalizationGet("common.x_count_lua"),resData.count)
    icon:HideCount() 
    
    local bData = nil    
    if resData.type == BaseResType.Hero then bData = gamemanager.GetData('hero_data').GetDataById(resData.id)
    elseif resData.type == BaseResType.Equipment then bData = gamemanager.GetData('equip_data').GetDataById(resData.id)
    elseif resData.type == BaseResType.Item then bData = gamemanager.GetData('item_data').GetDataById(resData.id) 
    else bData = gamemanager.GetData('item_data').GetBasicResItemByType(resData.type) end
    
    if nil ~= bData then nameText.text = LocalizationController.instance:Get(bData.name) end
  end

  t.rewardIconPrefab:SetActive(false)  
end

function t.SetRewardButtonEnable(enable)
  t.getRewardBtn.enabled = enable
  ui_util.SetImageGray(t.getRewardBtn:GetComponent(typeof(Image)),not enable)
  local text = t.getRewardBtn:GetComponentInChildren(typeof(Text))
  if text then
    if enable then
      text.color = Color.white
    else
      text.color = Color.gray
    end
    local outline = text:GetComponent(typeof(Outline))
    if outline then
      outline.enabled = false
    end
  end
end

function t.ShowBtnCoroutine()
  t.btnClose.enabled = false
  coroutine.wait(0.2)
  t.btnClose.enabled = true
end

function t.OnClickCloseHandler()
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

return t