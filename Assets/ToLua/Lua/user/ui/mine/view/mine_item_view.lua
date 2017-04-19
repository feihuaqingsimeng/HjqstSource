local t={}
t.__index= t
t.m_imgIcon =nil
t.m_btnIcon = nil
t.m_labNum = nil
t.m_objState = nil
local mineModel = gamemanager.GetModel('mine_model')


function t.NewByGameObject(gameObject)
  local o = {}
  setmetatable(o,t) 
  o.transform = gameObject:GetComponent(typeof(Transform))
  o.mineInfo = nil
  o.onClick = void_delegate.New() 
  o:InitComponent()
  return o
end

function t:InitComponent()
  self.m_imgIcon = self.transform:Find('btn_mineitem'):GetComponent(typeof(Image))
  self.m_btnIcon = self.transform:Find('btn_mineitem'):GetComponent(typeof(Button))
  self.m_labNum  = self.transform:Find('btn_mineitem/text_num'):GetComponent(typeof(Text))
  self.m_objState = self.transform:Find('btn_mineitem/state')
  self:ResetListener ()
end
function t:ResetListener ()
  self.m_btnIcon.onClick:RemoveAllListeners()
  self.m_btnIcon.onClick:AddListener(
    function()
      self.onClick:InvokeOneParam(self.mineInfo)
    end)
end

function t:SetMineInfo(info)
  self.mineInfo = info
  local mineData = mineModel.GetMineData(self.mineInfo.mineNo) 
  self.m_imgIcon.sprite = ResMgr.instance:LoadSprite(string.format('sprite/main_ui/image_ore_%d',mineData.quality))
  self.m_imgIcon:SetNativeSize()
  if mineData.man_max < 999 then
    self.m_labNum.text = self.mineInfo.occNum..'/'..mineData.man_max
  else
    self.m_labNum.text =LocalizationController.instance:Get('ui.mine_view.mine_view_unlimite')
  end
 -- print("ineModel.m_selfMineInfo.ownMineNo:",mineModel.m_selfMineInfo.ownMineNo,"self.mineInfo.mineNo:",self.mineInfo.mineNo)
  self.m_objState.gameObject:SetActive(mineModel.m_selfMineInfo.ownMineNo == self.mineInfo.mineNo)
end

return t

