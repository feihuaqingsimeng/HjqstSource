local t = {}
t.__index = t

function t.BindGameObject(gameObject,clickCallback)
  local o = {}
  setmetatable(o,t)
  
  o.clickCallback = clickCallback
  o.isSelect = false
  o.transform = gameObject:GetComponent(typeof(Transform))
  
  o:InitComponent()
  return o
end

function t:InitComponent()
  
  self.imgMark = self.transform:Find('img_mark'):GetComponent(typeof(Image))
  self.textName = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.textLeader = self.transform:Find('text_leader'):GetComponent(typeof(Text))
  self.textLv = self.transform:Find('text_lv'):GetComponent(typeof(Text))
  self.textMemberCount = self.transform:Find('text_member'):GetComponent(typeof(Text))
  self.goSelect = self.transform:Find('go_select').gameObject
  local btn = self.transform:GetComponent(typeof(Button))
  btn.onClick:RemoveAllListeners()
  btn.onClick:AddListener(function()
    if self.clickCallback then
      self.clickCallback(self)
    end
    
  end)
  
end

function t:SetConsortiaInfo(consortiaInfo)
  self.consortiaInfo = consortiaInfo
  self:Refresh()
end
function t:SetSelect(isSelect)
  self.isSelect = isSelect
  self.goSelect:SetActive(not isSelect)
end
function t:Refresh()
  self.imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(self.consortiaInfo.headNo)
  self.textName.text = self.consortiaInfo.name
  self.textLeader.text = self.consortiaInfo.creatorName
  self.textLv.text = string.format('Lv %d',self.consortiaInfo.lv)
  self.textMemberCount.text = string.format('%d/%d',self.consortiaInfo.curNum,self.consortiaInfo.maxNum)

end

return t