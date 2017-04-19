local t = {}
t.__index = t

local illustration_hero_button = require('ui/illustration/view/hero/illustration_hero_button')
local illustration_equip_button = require('ui/illustration/view/equip/illustration_equip_button')
local illustration_item_button = require('ui/illustration/view/item/illustration_item_button')

function t.BindTransform(transform,goItemPrefab)
  local o = {}
  setmetatable(o,t)
  o.transform = transform
  o.goItemPrefab = goItemPrefab
  o.isHideTitle = false
  o.illustrationInfoList = nil
  o.itemList = MList.New('illustration_hero_button')
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.titleRoot = self.transform:Find('title_root')
  self.textTitle = self.titleRoot:Find('text_title'):GetComponent(typeof(Text))
  self.contentRoot = self.transform:Find('content_root')
  self.attrRoot = self.transform:Find('attr_root')
  if self.attrRoot then
    self.textCollectNum = self.attrRoot:Find('text_collect_num'):GetComponent(typeof(Text))
    self.attrTable = {}
    for i = 1,3 do
      local root = self.attrRoot:Find('attr_root/attr'..i)
      self.attrTable[i] = {}
      self.attrTable[i].root = root
      self.attrTable[i].textName = root:Find('text_name'):GetComponent(typeof(Text))
      self.attrTable[i].textValue = root:Find('text_value'):GetComponent(typeof(Text))
    end
  end
  
end
--设置英雄图鉴小页签数据  roleList:MList<illustration_info>类型
function t:SetData(titleStr,illustrationList,illustrationType)
  if titleStr then
    self.textTitle.text = titleStr
  end
  
  self.illustrationType = illustrationType
  self.illustrationInfoList = illustrationList
  self.isHideTitle = illustrationList ~= nil
  self.titleRoot.gameObject:SetActive(not self.isHideTitle)
  self:UpdateList()
  if self.attrRoot then
    self.attrRoot.gameObject:SetActive(false)
  end
end

function t:SetCollectAttrData(collectNum,role_attr_dic)
  self.titleRoot.gameObject:SetActive(false)
  self.attrRoot.gameObject:SetActive(true)
  local rootSize = self.transform.sizeDelta
  self.transform.sizeDelta = Vector2(rootSize.x,math.abs(self.contentRoot.anchoredPosition.y) + self.attrRoot.sizeDelta.y)
  --hide child
  for i = 0,self.itemList.Count-1 do
    self.itemList:Get(i):SetActive(false)
  end
  --attr
  self.textCollectNum.text = collectNum
  local role_attr_list = role_attr_dic:GetValuesList()
  for k,v in pairs(self.attrTable) do
    local attr = role_attr_list[k]
    v.root.gameObject:SetActive(attr ~= nil)
    if attr  then
      v.textName.text = attr:GetName()
      v.textValue.text = string.format('+%s',attr:GetValueString())
    end
  end
end
--点击item回调(参数illustrationInfo)
function t:SetOnClickItemCallback(callback)
  self.onClickCallback = callback
end

function t:SetSelect(illustrationInfo)
  for k,v in pairs(self.itemList:GetDatas()) do
    v:SetSelect(v.illustrationInfo == illustrationInfo)
  end
end
--更新列表
function t:UpdateList()
  local roleInfo = nil 
  local count = 0
  if self.illustrationInfoList then
    count = self.illustrationInfoList.Count
  end
  local childCount = self.itemList.Count
  local illus_btn = nil
  local go = nil
  -- update old
  local tempCount = count-1
  if tempCount >= childCount -1 then
    tempCount = childCount -1
  end
  
  for i = 0,tempCount do
    illus_btn = self.itemList:Get(i)
    illus_btn:SetActive(true)
    illus_btn:SetIllustrationInfo(self.illustrationInfoList:Get(i))
  end
  --create new 
  for i = childCount, count-1 do
    go = GameObject.Instantiate(self.goItemPrefab)
    go:SetActive(true)
    go.transform:SetParent(self.contentRoot,false)
    if self.illustrationType == IllustrationType.hero then
      illus_btn = illustration_hero_button.BindTransform( go.transform)
    elseif self.illustrationType == IllustrationType.equip then
      illus_btn = illustration_equip_button.BindTransform(go.transform)
    else
      illus_btn = illustration_item_button.BindTransform(go.transform)
    end
    illus_btn:SetOnClickItemCallback(function(illustrationInfo)
        if self.onClickCallback then
          self.onClickCallback(illustrationInfo)
        end
      end)
    illus_btn:SetIllustrationInfo(self.illustrationInfoList:Get(i))
    self.itemList:Add(illus_btn)
  end
  --hide child
  for i = count,childCount-1 do
    self.itemList:Get(i):SetActive(false)
  end
  --update size
  local grid = self.contentRoot:GetComponent(typeof(GridLayoutGroup))
  if self.isHideTitle then
    local extra = 1 
    if count % grid.constraintCount == 0 then
      extra = 0
    end
    local row = math.floor(count/grid.constraintCount) + extra
    local h = grid.cellSize.y * row + grid.spacing.y * (row - 1)
    local rootSize = self.transform.sizeDelta
    self.transform.sizeDelta = Vector2(rootSize.x,math.abs(self.contentRoot.anchoredPosition.y) + h)
  else
    local rootSize = self.transform.sizeDelta
    self.transform.sizeDelta = Vector2(rootSize.x,math.abs(self.contentRoot.anchoredPosition.y) + self.titleRoot.sizeDelta.y)
  end
  
end


return t