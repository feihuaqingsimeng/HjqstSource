local t = {}
local PREFAB_PATH = 'ui/common/common_top_bar'


function t:Create(transformParent)
  local o = {}
  self.__index = self
  setmetatable(o,self)
  local gameObject = Common.ResMgr.ResMgr.instance:Load(PREFAB_PATH)
  o.transform = GameObject.Instantiate(gameObject):GetComponent(typeof(Transform))
	o.transform:SetParent(transformParent,false)
  o.transform.localPosition = Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0)
  return o
end

function t:SetTitle(title)
  self.transform:Find('text_title'):GetComponent(typeof(Text)).text = title
end

function t:SetAsCommonStyle(title,backListener,showPveActionItem,showGoldItem,showDiamondItem,showHonorItem,showPVPActionItem,showExpeditionItem,showWorldTreeFruitItem)
  self.transform:Find('text_role_name').gameObject:SetActive(false)
  self:SetTitle(title)
  self.transform:Find('btn_back'):GetComponent(typeof(Button)).onClick:AddListener(backListener)
  self:SetAsCommonStyle2(showPveActionItem,showGoldItem,showDiamondItem,showHonorItem,showPVPActionItem,showExpeditionItem,showWorldTreeFruitItem)
end

function t:SetAsCommonStyle2(showPveActionItem,showGoldItem,showDiamondItem,showHonorItem,showPVPActionItem,showExpeditionItem,showWorldTreeFruitItem)
  local res_bars_root = self.transform:Find('res_bars_root')
  res_bars_root:Find('pve_action_bar').gameObject:SetActive(showPveActionItem)
  res_bars_root:Find('gold_bar').gameObject:SetActive(showGoldItem)
  res_bars_root:Find('diamond_bar').gameObject:SetActive(showDiamondItem)
  res_bars_root:Find('honor_bar').gameObject:SetActive(showHonorItem)
  res_bars_root:Find('pvp_action_bar').gameObject:SetActive(showPVPActionItem)
  res_bars_root:Find('expedition_bar').gameObject:SetActive(showExpeditionItem)
  res_bars_root:Find('world_tree_fruit').gameObject:SetActive(showWorldTreeFruitItem)
end
function t:SetAsMainViewStyle()
  self.transform:Find('img_bg').gameObject:SetActive(false)
  self.transform:Find('btn_back').gameObject:SetActive(false)
  self.transform:Find('text_title').gameObject:SetActive(false)
  self.transform:Find('text_role_name').gameObject:SetActive(false)
  local res_bars_root = self.transform:Find('res_bars_root')
  res_bars_root:Find('pvp_action_bar').gameObject:SetActive(false)
  res_bars_root:Find('expedition_bar').gameObject:SetActive(false)
  res_bars_root:Find('world_tree_fruit').gameObject:SetActive(false)
  res_bars_root:Find('honor_bar').gameObject:SetActive(false)
end

return t