local t = {}
local PREFAB_PATH = 'ui/consortia/private_donation/consortia_private_donation_panel'

local private_donation_item = require ('ui/consortia/view/private_donation/private_donation_item')

function t.Open (parent)
  local gameObject = GameObject.Instantiate(ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(parent, false)
  
  t.BindDelegate ()
  t.InitComponent()
  t.RegenerateItems ()
end

function t.BindDelegate ()
end

function t.UnbindDelegate ()
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('scroll_view/viewport/content'):GetComponent(typeof(ScrollContent))
  t.scrollContent.onResetItem:AddListener(t.OnResetItem)
  t.privateDonationItems = {}
  
  t.requestDonationButton = t.transform:Find('bottom/btn_request_donation'):GetComponent(typeof(Button))
  t.requestDonationButton.onClick:AddListener(t.ClickRequestDonationButtonHandler)
end

function t.RegenerateItems ()
  t.scrollContent:Init(50, true, 0)
end

function t.OnResetItem (go, index)
  local item = t.privateDonationItems[go]
  if not item then
    item = private_donation_item.BindGameObject(go)
    t.privateDonationItems[go] = item
  end
  item:SetData(nil)
end

function t.ClickRequestDonationButtonHandler ()
  local private_donation_request_view = require ('ui/consortia/view/private_donation/private_donation_request_view')
  private_donation_request_view.Open()
end

function t.Close ()
  GameObject.Destroy(t.transform.gameObject)
  t.UnbindDelegate()
end

return t