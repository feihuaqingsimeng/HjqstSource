local t = {}
t.__index=t

local function Start()

end

function t.Init(pv)
	t.pv=pv
end

function t.New(go)
	local r={}	
	setmetatable(r,t)
	r:InitComponent(go)
	return r
end

function t:InitComponent(go)
	self.go=go
	self.heroitems={}
	local transform = self.go.transform
	self.text_relationship_name=transform:FindChild('text_relationship_name'):GetComponent(typeof(Text))
	self.attribute1trans=transform:FindChild('attr1')
	self.attribute2trans=transform:FindChild('attr2')
	self.text_relationship_attribute_1=transform:FindChild('attr1/text_relationship_attribute_1'):GetComponent(typeof(Text))
	self.text_relationship_attribute_2=transform:FindChild('attr2/text_relationship_attribute_2'):GetComponent(typeof(Text))
	self.text_relationship_attribute_value_1=transform:FindChild('attr1/text_relationship_attribute_value_1'):GetComponent(typeof(Text))
	self.text_relationship_attribute_value_2=transform:FindChild('attr2/text_relationship_attribute_value_2'):GetComponent(typeof(Text))
	self.img_line=transform:FindChild('img_line')
  self.btn_active=transform:FindChild('btn_active')
  self.btn_Image = self.btn_active:GetComponent(typeof(Image))
  self.btn_Text = transform:FindChild('btn_active/Text'):GetComponent(typeof(Text))
	  
	self.onClick=self.btn_active:GetComponent(typeof(Button)).onClick
	self.img_actived=transform:FindChild('img_actived')
	self.heropt=transform:FindChild('heros_root')
end

function t:GenHeroitem()
	local it=t.pv.GenHeroitem()
	it:Reset(self.heropt)
	table.insert(self.heroitems,it)
	return it
end

function t:Setup(index,data)
	self.index = index
	self.data=data
	self.text_relationship_name.text=LuaCsTransfer.LocalizationGet(data.name)
  
	if data.attribute_type1 then
		local attr=require('ui/equip/model/equip_attr').New(data.attribute_type1,data.attribute1)
		self.text_relationship_attribute_1.text=attr:GetName()
		self.text_relationship_attribute_value_1.text='+'..attr:GetValueString()
		self.attribute1trans.gameObject:SetActive(true)
	else self.attribute1trans.gameObject:SetActive(false)	end
  
	if data.attribute_type2 then
		local attr = require('ui/equip/model/equip_attr').New(data.attribute_type2, data.attribute2)
		self.text_relationship_attribute_2.text = attr:GetName()
		self.text_relationship_attribute_value_2.text = '+'..attr:GetValueString()
		self.attribute2trans.gameObject:SetActive(true)
	else self.attribute2trans.gameObject:SetActive(false)	end

	local hero=nil
	local a,b = nil,nil
  
  ui_util.ClearChildren(self.heropt, true)  
  for i=1,#data.friend_id do
		hero = self:GenHeroitem()
		hero:Setup(data.friend_id[i])
		if t.Patch(t.pv.ids,t.pv.stars,data.friend_id[i][1],data.friend_id[i][2]) >0 then	hero:SetStarEnough()
		else
			self.allow = false
      self:SetButton(true)
			if t.Patch(t.pv.ids_relations,t.pv.stars_relations,data.friend_id[i][1],data.friend_id[i][2]) >0 then
				hero:SetUsing()
			else
				if t.contain_orderly(t.pv.ids,data.friend_id[i][1]) then
					hero:SetStarNotEnough()
				else
					hero:SetNotExist()
				end
			end
		end
	end  
  
	for i=1,#t.pv.relations do
		hero = t.pv.relations[i]
		if hero.id == data.id and hero.instanceID == t.pv.info.instanceID then --关系id
			self:SetActivedHeros(data.friend_id) --hero.friends for instanceID 
      self:SetButton(true)
      self.allow = false
			return
		end
	end
  self:SetButton(false)
	self.allow = true	
  local isActive = self:IsActive()
  ui_util.SetGrayChildren(self.btn_active.transform,not isActive)
end

function t.Patch(idArray,starArray,modelId,stars)
	for i=1,#idArray do
		if idArray[i] == modelId then
			if starArray[i] >= stars then
				return i
			end
		end
	end	
	return 0
end

function t:SetActivedHeros(heros)
	local hero = nil
	local info = nil
	local model = gamemanager.GetModel('hero_model')

  ui_util.ClearChildren(self.heropt, true) 
	for i = 1, #heros do
		hero = self:GenHeroitem()
		hero:Setup(heros[i])
	end	 
end

function t:SetButton(bIsActive)
  if not bIsActive then 
    self.btn_Text.text = LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.btn_activate')
    self.btn_Image.sprite = ResMgr.instance:LoadSprite(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.btn_activate_bg'))
    self.onClick:RemoveAllListeners()
    self.onClick:AddListener(function()	self:ClickActive() end)
  else
    self.btn_Text.text = LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.btn_cancle')
    self.btn_Image.sprite = ResMgr.instance:LoadSprite(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.btn_cancle_bg'))
    self.onClick:RemoveAllListeners()
    self.onClick:AddListener(function()	self:ClickCancle() end)
  end
end

function t.contain_orderly(t,value)
	if not t or #t==0 then return false end	
	for i = 1, #t do
		if t[i] == value then
			return true
		end
	end
	return false
end
function t:IsActive()
  if self.allow then
		local data=self.data
		local instid =-1
		local modelId=1
		local starlv =1
		local oldlv = 1
		local key=1
    
		for j = 1, #data.friend_id do
			instid = -1
			modelId = data.friend_id[j][1]
			starlv = data.friend_id[j][2]
			oldlv = 10
			for k, v in pairs(t.pv.originList) do
				if v.heroData.id==modelId and v.advanceLevel >= starlv then					
					if v.advanceLevel < oldlv then
						key = k
						oldlv = v.advanceLevel
						instid = v.instanceID			
					end		
				end
			end	
			if instid==-1 then    
        return false  
			end
		end
    return true
	end
  return false
end
function t:ClickActive()
  if self:IsActive() then
    
  end
  print(self.allow )
	if self.allow then
		local req={}
		req.id=self.data.id
		req.heroId=t.pv.info.instanceID
		req.activeFlag=1
		req.friends={}

		local data=self.data
		local instid =-1
		local modelId=1
		local starlv =1
		local oldlv = 1
		local key=1
    
		for j = 1, #data.friend_id do
			instid = -1
			modelId = data.friend_id[j][1]
			starlv = data.friend_id[j][2]
			oldlv = 10
			for k, v in pairs(t.pv.originList) do
				if v.heroData.id==modelId and v.advanceLevel >= starlv then					
					if v.advanceLevel < oldlv then
						key = k
						oldlv = v.advanceLevel
						instid = v.instanceID			
					end		
				end
			end	
			if instid==-1 then    return
			else
				t.pv.originList[key]=nil
				table.insert(req.friends,instid)
			end
		end
    print('HeroRelationShipReq')
		t.pv.curItem = self
		t.pv.HeroRelationShipReq(req)
	else
		uimanager.ShowTipsOnly(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.notactivate'))
	end
	
end

function t:ClickCancle()
  print("self.allow", self.allow)
  if not self.allow then
		local req={}
		req.id=self.data.id
		req.heroId=t.pv.info.instanceID
		req.activeFlag=0
		req.friends={}

		local data=self.data
		local instid =-1
		local modelId=1
		local starlv =1
		local oldlv = 1
		local key=1
    
		for j=1,#data.friend_id do
			instid = -1
			modelId = data.friend_id[j][1]
			starlv = data.friend_id[j][2]
			oldlv = 10
			for k,v in pairs(t.pv.originList) do
				if v.heroData.id == modelId and v.advanceLevel >= starlv then
					--print('初步匹配',v.instanceID)
					if v.advanceLevel < oldlv then
						key = k
						oldlv= v.advanceLevel
						instid= v.instanceID			
					end		
				end
			end	
			if instid ~= -1 then
				t.pv.originList[key] = nil
				table.insert(req.friends, instid)
			end
		end
		t.pv.curItem = self
		t.pv.HeroRelationShipReq(req)
	else
		uimanager.ShowTipsOnly(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.notcancle'))
	end
end

function t:SetActivedBtn( ... )
	 self:SetButton(true)
end

function t:Close( ... )
	--for i=1,#self.heroitems do
		--t.pv.RecycleHeroitem(self.heroitems[i])
	--end
	self.heroitems={}
end

return t