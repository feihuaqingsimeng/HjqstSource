local t = {}
t.__index = t

local common_reward_icon = require('ui/common_icon/common_reward_icon')

function t.BindTransform(transform)
  local o = {}
  setmetatable(o,t)
  
  o.transform = transform
  o:InitComp()
  return o
  
end

function t:InitComp()
  self.textName = ui_util.FindComp(self.transform,'text_name',Text)
  self.textRank = ui_util.FindComp(self.transform,'text_rank',Text)
  self.textTimes = ui_util.FindComp(self.transform,'text_times',Text)
  self.imgRank = ui_util.FindComp(self.transform,'img_rank',Image)
  self.imgRankBg = ui_util.FindComp(self.transform,'num_rank_bg',Image)
  self.rewardRoot = ui_util.FindComp(self.transform,'award_root',Transform)
end
function t:SetData(turntableRankInfo)
  self.turntableRankInfo = turntableRankInfo
  self:Refresh()
end

function t:Refresh()
  self.textName.text = self.turntableRankInfo.name
  self.textRank.text = self.turntableRankInfo.rank
  self.textTimes.text = string.format(LocalizationController.instance:Get('ui.turntable_view.drawTimes'),self.turntableRankInfo.times)
  if self.turntableRankInfo.rank <= 3 then
    self.imgRank.gameObject:SetActive(true)
    self.textRank.gameObject:SetActive(false)
    self.imgRankBg.gameObject:SetActive(false)
    self.imgRank.sprite = ResMgr.instance:LoadSprite('sprite/main_ui/icon_'..self.turntableRankInfo.rank..'st')
  else
    self.imgRank.gameObject:SetActive(false)
    self.imgRankBg.gameObject:SetActive(true)
    self.textRank.gameObject:SetActive(true)
  end
  
  if self.icon == nil then
    self.icon = common_reward_icon.New(self.rewardRoot,self.turntableRankInfo.rankReward)
  else
    self.icon:SetGameResData(self.turntableRankInfo.rankReward)
  end
  
end

return t