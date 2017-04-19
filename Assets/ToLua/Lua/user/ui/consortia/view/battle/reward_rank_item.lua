local t = {}
t.__index = t

local common_reward_icon = require('ui/common_icon/common_reward_icon')

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.transform = go:GetComponent(typeof(Transform))
  o.rewardItems = {}
  
  o:InitComponent()
  
  return o
end
function t:InitComponent()
  self.imgRank = self.transform:Find('img_top_three'):GetComponent(typeof(Image))
  self.textRank = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.tranRewardRoot = self.transform:Find('reward_root')
end

function t:SetRewardData(rewardRankData)
  self.rewardRankData = rewardRankData
  self:Refresh()
end
function t:Refresh()
  if self.rewardRankData.rank_min == self.rewardRankData.rank_max then
    self.textRank.text = string.format('第%d名',self.rewardRankData.rank_min)
  else
    self.textRank.text = string.format('第%d—%d名',self.rewardRankData.rank_min,self.rewardRankData.rank_max)
  end
  local rewards = self.rewardRankData.award
  local index = 1
  for k,v in ipairs(rewards) do
    local item = self.rewardItems[k]
    if item then
      item:SetActive(true)
      item:SetGameResData(v)
    else
      item = common_reward_icon.New(self.tranRewardRoot,v)
      self.rewardItems[k] = item
    end
    item:AddDesButton()
    index = k
  end
  local count = #self.rewardItems
  if index < count then
    for i = index + 1,count do
      self.rewardItems[i]:SetActive(false)
    end
  end
end
return t