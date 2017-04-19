local t = {}
t.__index = t

function t.BindGameObject(go)
  local o = {}
  setmetatable(o,t)
  o.transform = go:GetComponent(typeof(Transform))
  o:InitComponent()
  return o
end
function t:InitComponent()
  local black = self.transform:Find('black_bg')
  if black then
    self.goBlackBg = black.gameObject
  end
  local white = self.transform:Find('white_bg')
  if white then
    self.goWhiteBg = white.gameObject
  end
  self.imgMark = self.transform:Find('mark_bg/img_mark'):GetComponent(typeof(Image))
  self.textName = self.transform:Find('text_name'):GetComponent(typeof(Text))
  self.textLevel = self.transform:Find('text_lv'):GetComponent(typeof(Text))
  self.textRank = self.transform:Find('text_rank'):GetComponent(typeof(Text))
  self.textVectoryCount = self.transform:Find('text_vectoryCount'):GetComponent(typeof(Text))
  self.textPoint = self.transform:Find('text_point'):GetComponent(typeof(Text))
  
end

function t:ShowBg(isShowBlack)
  self.goBlackBg:SetActive(isShowBlack)
  self.goWhiteBg:SetActive(not isShowBlack)

end
function t:SetBattleRankInfo(consortiaBattleRankInfo)
  self.battleRankInfo = consortiaBattleRankInfo
  self:Refresh()
end

function t:Refresh()
  self.imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(self.battleRankInfo.headNo)
  self.textName.text = self.battleRankInfo.name
  self.textLevel.text = self.battleRankInfo.lv
  self.textRank.text = self.battleRankInfo.rank
  self.textVectoryCount.text = self.battleRankInfo.vectoryCount
  self.textPoint.text = self.battleRankInfo.point
end

return t