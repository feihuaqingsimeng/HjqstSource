local t = {}
t.__index = t

local PREFAB_PATH = 'ui/illustrated_handbook/equip/equip_panel'

local common_equip_icon = require('ui/common_icon/common_equip_icon')
local illustration_model = gamemanager.GetModel('illustration_model')
local illustration_content_view = require('ui/illustration/view/illustration_content_view')
local equip_attr = require('ui/equip/model/equip_attr')
local drop_message_data = gamemanager.GetData('drop_message_data')
local game_res_data = require('ui/game/model/game_res_data')

function t.Open(parent,useSaveState)
  
  local gameObject = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject.transform
  t.transform:SetParent(parent,false)
  
  t.InitComponent()
  t.useSaveState = useSaveState
  t.selectIllustrationInfo = nil
  t.scrollItems = {}
  t.selectIcon = nil
  
  return t
  
end

function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.transform = nil
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.onResetItemHandler)
  t.scrollBar = t.transform:Find('Scroll View/scrollbar'):GetComponent(typeof(Scrollbar))
  t.goEquipPrefab = t.transform:Find('Scroll View/illustratedButton').gameObject
  t.goEquipPrefab:SetActive(false)
  local goItemPrefab = t.transform:Find('Scroll View/illustratedItemButton').gameObject
  goItemPrefab:SetActive(false)
  --info
  local left_root = t.transform:Find('left_root')
  t.textName = left_root:Find('info/text_name'):GetComponent(typeof(Text))
  t.textDes = left_root:Find('info/text_des'):GetComponent(typeof(Text))
  t.tranIconRoot = left_root:Find('info/icon_root')
  t.tranAttrRoot = left_root:Find('info/attr_root')
  t.tranAttr1 = {}
  t.tranAttr1.textName = t.tranAttrRoot:Find('attr1/text_name'):GetComponent(typeof(Text))
  t.tranAttr1.textValue = t.tranAttrRoot:Find('attr1/text_value'):GetComponent(typeof(Text))

  --path
  t.goodsPathButtons = {}
  for i = 0,2 do
    t.goodsPathButtons[i] = left_root:Find('path/btn_path'..(i+1)):GetComponent(typeof(GoodsJumpButton))
  end
  t.textNoneTip = ui_util.FindComp(left_root,'text_none_tip',Text)
end

function t.Refresh()
  local count = illustration_model.currentSelectList.Count
  if t.useSaveState then
    t.scrollContent:Init(count,false,0)
    t.scrollContent:ScrollToPosition(illustration_model.scrollPercent)
  else
    t.scrollContent:Init(count,false,0.2)
  end
  t.useSaveState = false
  t.RefreshInformation()
end

function t.RefreshScrollContent()
  t.scrollContent:RefreshAllContentItems()
end
--刷新装备信息
function t.RefreshInformation()
  t.textName.text = LocalizationController.instance:Get(t.selectIllustrationInfo.equipInfo.data.name)
  t.textName.color = ui_util.GetEquipQualityColor(t.selectIllustrationInfo.equipInfo.data)
  t.textDes.text = ''--LocalizationController.instance:Get(t.selectIllustrationInfo.equipInfo.data.description)
  if t.selectIcon == nil then
    t.selectIcon = common_equip_icon.New(t.tranIconRoot)
  end
  t.selectIcon:SetEquipInfo(t.selectIllustrationInfo.equipInfo)
  t.selectIcon:ShowStrengthenLevel(false)
  --path
  
  local pathList = LuaCsTransfer.GetDropGoodsPathDatas(BaseResType.Equipment,t.selectIllustrationInfo.id,0,0)
  local length  = 0
  if pathList == nil then
    t.textNoneTip.text = '无'
    t.textNoneTip.gameObject:SetActive(true)
  elseif pathList.Length == 0 then
    t.textNoneTip.gameObject:SetActive(true)
     local resData = game_res_data.New(BaseResType.Equipment,t.selectIllustrationInfo.id,0,0)
    local dropData = drop_message_data.GetDataByMockGameResData(resData)
    t.textNoneTip.text = LocalizationController.instance:Get(dropData.des)
  else
    length = pathList.Length
    t.textNoneTip.gameObject:SetActive(false)
  end
  for k,v in pairs(t.goodsPathButtons) do
      if k < length then
        local str = pathList[k]
        str = string.split2number(str,',')
        v:Set(str[1],str[2])
      end
      v.gameObject:SetActive(k < length)
    end
  
  local attr = t.selectIllustrationInfo.equipInfo.data:GetFirstBaseAttr()
  t.tranAttr1.textName.text=attr:GetName()..':'
  t.tranAttr1.textValue.text=attr:GetValueString()
end

function t.GetScrollBarValue()
  return t.scrollBar.value
end

--------------------click event-------------------------------------

function t.onResetItemHandler(go,index)
  local item = t.scrollItems[go]
  if not item then
    item = illustration_content_view.BindTransform(go.transform,t.goEquipPrefab)
    t.scrollItems[go] = item
  end
  local scrollItemData = illustration_model.currentSelectList:Get(index)
  local illustrationList = scrollItemData.illustrationInfoList
  item:SetData(scrollItemData.titleStr,illustrationList,IllustrationType.equip)
  item:SetOnClickItemCallback(t.OnClickItemBtnHandler)
  if t.selectIllustrationInfo == nil and illustrationList ~= nil and illustrationList.Count > 0 then
    t.selectIllustrationInfo = illustrationList:Get(0)
  end
  item:SetSelect(t.selectIllustrationInfo)
end
function t.OnClickItemBtnHandler(illustrationInfo)
  if t.selectIllustrationInfo == illustrationInfo then
    return
  end
  t.selectIllustrationInfo = illustrationInfo
  for k,v in pairs(t.scrollItems) do
    v:SetSelect(t.selectIllustrationInfo)
  end
  t.RefreshInformation()
end
return t