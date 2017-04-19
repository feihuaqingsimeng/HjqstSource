local t={}
local name='seven_hilarity_box'

local activity_ctrl = gamemanager.GetCtrl('activity_controller')
local activity_model = gamemanager.GetModel('activity_model')

local seven_hilarity_data = gamemanager.GetData('seven_hilarity_data')

local common_reward_icon = require('ui/common_icon/common_reward_icon')
local game_res_data = require('ui/game/model/game_res_data')

local BoxPic1 = {open = 'jdtbox1_2',  close = 'jdtbox1_1'}
local BoxPic2 = {open = 'jdtbox2_2',  close = 'jdtbox2_1'}
local BoxPic3 = {open = 'jdtbox3_2',  close = 'jdtbox3_1'}
local BoxPic4 = {open = 'jdtbox4_2',  close = 'jdtbox4_1'}

t._TextTip = nil
t._TextBoxValue1 = nil
t._TextBoxValue2 = nil
t._TextBoxValue3 = nil
t._TextBoxValue4 = nil

t._IconBox1 = nil
t._IconBox2 = nil
t._IconBox3 = nil
t._IconBox4 = nil

t._EffectBox1 = nil
t._EffectBox2 = nil
t._EffectBox3 = nil
t._EffectBox4 = nil

t._BtnBox1 = nil
t._BtnBox2 = nil
t._BtnBox3 = nil
t._BtnBox4 = nil
t._Slider = nil
t.transform = nil

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform     
    self._TextTip = self.transform:FindChild('img_tip/text_tip'):GetComponent(typeof(Text))
    
    self._TextBoxValue1 = self.transform:FindChild('button_box_1/text_count'):GetComponent(typeof(Text))
    self._TextBoxValue2 = self.transform:FindChild('button_box_2/text_count'):GetComponent(typeof(Text))
    self._TextBoxValue3 = self.transform:FindChild('button_box_3/text_count'):GetComponent(typeof(Text))
    self._TextBoxValue4 = self.transform:FindChild('button_box_4/text_count'):GetComponent(typeof(Text))
    
    self._EffectBox1 = self.transform:FindChild('button_box_1/ui_baoxiang').gameObject
    self._EffectBox1:SetActive(false)
    
    self._EffectBox2 = self.transform:FindChild('button_box_2/ui_baoxiang').gameObject
    self._EffectBox2:SetActive(false)
    
    self._EffectBox3 = self.transform:FindChild('button_box_3/ui_baoxiang').gameObject
    self._EffectBox3:SetActive(false)
    
    self._EffectBox4 = self.transform:FindChild('button_box_4/ui_baoxiang').gameObject
    self._EffectBox4:SetActive(false)
    
    self._BtnBox1 = self.transform:FindChild('button_box_1'):GetComponent(typeof(Button))
    self._BtnBox1.onClick:RemoveAllListeners()
    self._BtnBox1.onClick:AddListener(function()	self:OnBtnBox1() end)
    
    self._BtnBox2 = self.transform:FindChild('button_box_2'):GetComponent(typeof(Button))
    self._BtnBox2.onClick:RemoveAllListeners()
    self._BtnBox2.onClick:AddListener(function()	self:OnBtnBox2() end)
    
    self._BtnBox3 = self.transform:FindChild('button_box_3'):GetComponent(typeof(Button))
    self._BtnBox3.onClick:RemoveAllListeners()
    self._BtnBox3.onClick:AddListener(function()	self:OnBtnBox3() end)
    
    self._BtnBox4 = self.transform:FindChild('button_box_4'):GetComponent(typeof(Button))
    self._BtnBox4.onClick:RemoveAllListeners()
    self._BtnBox4.onClick:AddListener(function()	self:OnBtnBox4() end)
    
    t._IconBox1 = self._BtnBox1.transform:GetComponent(typeof(Image))
    t._IconBox2 = self._BtnBox2.transform:GetComponent(typeof(Image))
    t._IconBox3 = self._BtnBox3.transform:GetComponent(typeof(Image))
    t._IconBox4 = self._BtnBox4.transform:GetComponent(typeof(Image))
    
    self._Slider = self.transform:FindChild('progress'):GetComponent(typeof(Slider))
  end
  
  self:Init()
end

function t:Init()
  local itemList1 = seven_hilarity_data.GetDataByDayAndType(0, 1)
  self._TextBoxValue1.text = itemList1[1].task
  
  local itemList2 = seven_hilarity_data.GetDataByDayAndType(0, 2)
  self._TextBoxValue2.text = itemList2[1].task
  
  local itemList3 = seven_hilarity_data.GetDataByDayAndType(0, 3)
  self._TextBoxValue3.text = itemList3[1].task
  
  local itemList4 = seven_hilarity_data.GetDataByDayAndType(0, 4)
  self._TextBoxValue4.text = itemList4[1].task

  local box = activity_model.SevenHilarityCompleteNums
  self._TextTip.text = string.format(LocalizationController.instance:Get("event_1012_boxTip"), box)  
  
  local IsGetBox1 = activity_model:IsGetBoxReward(tonumber(itemList1[1].task))
  if IsGetBox1 then  t._IconBox1.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic1["open"])
  else t._IconBox1.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic1["close"]) end
  self._EffectBox1:SetActive(box >= tonumber(itemList1[1].task) and not IsGetBox1)

  local IsGetBox2 = activity_model:IsGetBoxReward(tonumber(itemList2[1].task))
  if IsGetBox2 then  t._IconBox2.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic2["open"])
  else t._IconBox2.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic2["close"]) end
  self._EffectBox2:SetActive(box >= tonumber(itemList2[1].task) and not IsGetBox2)

  local IsGetBox3 = activity_model:IsGetBoxReward(tonumber(itemList3[1].task))
  if IsGetBox3 then  t._IconBox3.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic3["open"])
  else t._IconBox3.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic3["close"]) end
  self._EffectBox3:SetActive(box >= tonumber(itemList3[1].task) and not IsGetBox3)
  
  local IsGetBox4 = activity_model:IsGetBoxReward(tonumber(itemList4[1].task))
  if IsGetBox4 then  t._IconBox4.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic4["open"])
  else t._IconBox4.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/'..BoxPic4["close"]) end
  self._EffectBox4:SetActive(box >= tonumber(itemList4[1].task) and not IsGetBox4)
  
  t._Slider.value = box/tonumber(itemList4[1].task)
end

function t:Open()
   activity_ctrl.OnDlegateGetSevenDayCompleteAwardResp:AddListener(t.Refresh)
end

function t.Refresh()  
  t:Init()
end

function t:OnBtnBox1()  
  self:OnBtnBox(1)
end

function t:OnBtnBox2()
  self:OnBtnBox(2)
end

function t:OnBtnBox3()
  self:OnBtnBox(3)
end

function t:OnBtnBox4()
  self:OnBtnBox(4)
end

function t:OnBtnBox(type)
  local itemList = seven_hilarity_data.GetDataByDayAndType(0, type)
  local view = dofile('ui/activity/view/seven_hilarity/seven_hilarity_box_reward')
  view.Open(0, type, tonumber(itemList[1].task))
end

function t:Close()
 activity_ctrl.OnDlegateGetSevenDayCompleteAwardResp:RemoveListener(t.Refresh)
end

return t