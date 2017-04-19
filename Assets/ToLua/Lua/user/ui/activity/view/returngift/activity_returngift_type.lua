local t={}
local name='activity_returngift_type'

local activity_model = gamemanager.GetModel('activity_model')

t._TextValue    = nil
t._ImageIcon    = nil
t._ImageIconBg  = nil
t._TransRedPoint = nil

t.transform = nil
t.info = nil

t.DelegateOnClick = void_delegate.New()

t.FundationConfig = 
{
  bronze_pig = { name = "bronze_pig",type = 1,ImageIcon = 'sprite/main_ui/image_pig_3',ImageIconBg ="sprite/main_ui/ui_equip_shop_4" , textName = "event_1006_small_title1", textName1 = "event_1006_small_title11"},
  silver_pig = { name = "silver_pig",type = 2,ImageIcon = 'sprite/main_ui/image_pig_2',ImageIconBg ="sprite/main_ui/ui_equip_shop_3" , textName = "event_1006_small_title2", textName1 = "event_1006_small_title21"},
  gold_pig   = { name = "gold_pig",  type = 3,ImageIcon = 'sprite/main_ui/image_pig_1',ImageIconBg ="sprite/main_ui/ui_equip_shop_6" , textName = "event_1006_small_title3", textName1 = "event_1006_small_title31"},
}

function t:Start(transform, info)
  if nil == self.transform then 
		self.transform = transform    
    self._TextValue = self.transform:FindChild('Text'):GetComponent(typeof(Text))
    self._ImageIcon = self.transform:FindChild('ImageIcon'):GetComponent(typeof(Image))
    self._ImageIconBg = self.transform:FindChild('ImageIconBg'):GetComponent(typeof(Image))
    self._TransRedPoint = self.transform:FindChild('new')
    
    local button = self.transform:GetComponent(typeof(Button))
    button.onClick:RemoveAllListeners()
    button.onClick:AddListener(function()	self:OnClick() end)
    
    self.transform.gameObject:SetActive(false)
  end
  self.info = info
  
  local data = self:GetFundationConfigByType(self.info.subType)
  if nil == data then do return end end
  
  self.info.TitleName = data["textName1"]
  self:Init()
end

function t:Init()
  local data = self:GetFundationConfigByType(self.info.subType)
  if nil == data then do return end end
  
  self._TextValue.text = LocalizationController.instance:Get(data["textName"])
  self._ImageIcon.sprite = ResMgr.instance:LoadSprite(data["ImageIcon"])
  self._ImageIconBg.sprite = ResMgr.instance:LoadSprite(data["ImageIconBg"])
  
  self:SetRedPont()
  
  self.transform.gameObject:SetActive(true)
end

function t:SetRedPont()
  local isCanAwared = activity_model:IsActivityCanAwardBySubType(self.info.type, self.info.subType)
  self._TransRedPoint.gameObject:SetActive(isCanAwared)
end

function t:OnClick()
  self.DelegateOnClick:InvokeOneParam(self.info) 
end

--获取基金的配置信息
function t:GetFundationConfigByType(btype)
  for k, v in pairs(self.FundationConfig) do
    if v["type"] == btype then return v end
  end
end
return t