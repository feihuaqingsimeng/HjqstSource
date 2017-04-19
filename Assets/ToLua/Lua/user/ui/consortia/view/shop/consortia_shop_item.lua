local t = {}
local PREFAB_PATH = 'ui/consortia/donate/consortia_donate_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')

local using={}
t.__index=t


function t.New(go)
  local r={}
  setmetatable(r,t)
  r:InitComponent(go)
  return r
end

function t.Init(view)
  t.view=view
end

function t.ClickIcon(self)
    if self.equipIcon then
       uimanager.ShowTipsOfEquip(self.equipIcon.equipInfo,self.transform.position)
    elseif self.itemIcon then
      uimanager.ShowTipsOfItem(self.itemIcon.itemInfo,self.transform.position,Vector2(100,100))
    end
end

function t:Setup(idx,buyed,consortiaInfo,data)
  -- print(idx,info.no,info.type,info.buyed)
  -- print('item',data.item)
  local ICON = require('ui/common_icon/common_reward_icon').New(self.iconRoot,require('ui/game/model/game_res_data').NewByString(data.item))
  if ICON.commonHeroIcon then
      ICON.commonHeroIcon:AddRoleDesButton()
      ICON.commonHeroIcon:SetClickType(1)
  end
  ICON.onClick:AddListener(t.ClickIcon)

  local price=tonumber(data.cost[3])
  self.price=price
  -- self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost'),data.cost[1],price)
  self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost.enough'),price)
  self.onClick:RemoveAllListeners()
  self.btn.enabled=true
  
  if consortiaInfo.roleGuild.contribute>=price then
    self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost.enough'),price)
  else
    self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost.notenough'),price)
  end

  if buyed then
    self.btnText.text=LocalizationController.instance:Get('ui.consortia_view.shop.buyed')
  else
    if consortiaInfo.lv>= data.lv then
        self.btnText.text=LocalizationController.instance:Get('ui.consortia_view.shop.buy')
        if consortiaInfo.roleGuild.contribute>=price then
          self.onClick:AddListener(function ()
            self.type=data.type
            self:ClickBuy()
          end)
        else
          self.onClick:AddListener(t.ClickBuyLackMoney)
          --self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost.notenough'),price)
          --black
          ui_util.SetGrayChildren(self.btn.transform,true,true)
        end
    else
      self.btn.enabled=false
      self.btnText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.lv'),data.lv)
      --self.valueText.text=string.format(LocalizationController.instance:Get('ui.consortia_view.shop.cost.notenough'),price)
    end
  end
end

function t:SetBuyed( ... )
  self.onClick:RemoveAllListeners()
  self.btnText.text=LocalizationController.instance:Get('ui.consortia_view.shop.buyed')
end

function t:ClickBuy()
  t.view.GuildShopBuyReq(self)
end

function t.ClickBuyLackMoney()
  uimanager.ShowTipsOnly(LocalizationController.instance:Get('ui.consortia_view.shop.tips_buydenied'))
end

function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.UnbindDelegate()
end

function t:InitComponent(go)
  local transform=go.transform
  self.iconRoot=transform:FindChild('iconRoot')
  self.valueText=transform:FindChild('values/Text'):GetComponent(typeof(Text))
  self.btn=transform:FindChild('Button'):GetComponent(typeof(Image))
  self.btnText=transform:FindChild('Button/Text'):GetComponent(typeof(Text))
  self.onClick=transform:FindChild('Button'):GetComponent(typeof(Button)).onClick
  
  go:SetActive(true)
end

return t
