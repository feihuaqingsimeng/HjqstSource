local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_rank_view'

local consortia_rank_item = require('ui/consortia/view/battle/consortia_rank_item')
local reward_rank_item = require('ui/consortia/view/battle/reward_rank_item')
local consortia_battle_rank_info = require('ui/consortia/model/consortia_battle_rank_info')
local consortia_rank_reward_data = gamemanager.GetData('consortia_rank_reward_data')

function t.Open()
  local gameObject = UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  
  t.selectToggleIndex = 0
  t.isFirstEnter = true
  t.isFirstInitReward = true
  t.consortiaItems = {}--公会排名item
  t.rewardItems = {}--排名奖励item
  t.myConsortiaItem = nil--自己的公会item
  t.consortiaBattleRankInfoList = {}--公会排名info
  t.myConsortiaBattleRankInfo = consortia_battle_rank_info.New(100)--我的排名info
  
    for i = 1,20 do
      t.consortiaBattleRankInfoList[i] = consortia_battle_rank_info.New(i)
    end
  t.rewardRankDataList = {}--排行奖励
  local index = 1
  for k,v in pairs(consortia_rank_reward_data.GetDatas()) do
    t.rewardRankDataList[index] = v
    index = index + 1
  end
  t.InitComponent()
  t.BindDelegate()
  
  t.goToggles[1]:GetComponent(typeof(Toggle)).isOn = true
  t.ClickToggleHandler(t.goToggles[1])
end

function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  t.UnbindDelegate()
end
function t.BindDelegate()

end
function t.UnbindDelegate()
  
end

function t.InitComponent()
  local root = t.transform:Find('core/root')
  t.goToggles = {}
  local tranToggle =  root:Find('toggles/Toggle_consortia_rank')
  tranToggle:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
  t.goToggles[1] = tranToggle.gameObject
  tranToggle = root:Find('toggles/Toggle_reward_rank')
  tranToggle:GetComponent(typeof(EventTriggerDelegate)).onClick:AddListener(t.ClickToggleHandler)
  t.goToggles[2] = tranToggle.gameObject
  
  local btnClose = root:Find('btn_close'):GetComponent(typeof(Button))
  btnClose.onClick:AddListener(t.ClickCloseHandler)
  
  t.tranSonsortiaRankRoot = root:Find('consortia_rank')
  t.consortiaScrollContent = t.tranSonsortiaRankRoot:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContent))
  t.consortiaScrollContent.onResetItem:AddListener(t.OnConsortiaResetItemHandler)
  
  t.tranRewardRankRoot = root:Find('reward_rank')
  t.rewardScrollContent = t.tranRewardRankRoot:Find('Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContent))
  t.rewardScrollContent.onResetItem:AddListener(t.OnRewardResetItemHandler)
  
  t.myConsortiaItem = consortia_rank_item.BindGameObject(t.tranSonsortiaRankRoot:Find('my').transform)
end
function t.Refresh()
  t.InitScrollContent()
  t.RefreshMyConsortia()
end

function t.InitScrollContent()
  if t.selectToggleIndex == 1 then
    t.tranSonsortiaRankRoot.gameObject:SetActive(true)
    t.tranRewardRankRoot.gameObject:SetActive(false)
    
    local count = #t.consortiaBattleRankInfoList
    t.consortiaScrollContent:Init(count,t.isFirstEnter,0)
  else
    local count = #t.rewardRankDataList
    t.tranSonsortiaRankRoot.gameObject:SetActive(false)
    t.tranRewardRankRoot.gameObject:SetActive(true)
    if t.isFirstInitReward then
      t.rewardScrollContent:Init(count,false,0)
      t.isFirstInitReward = false
    end
    
  end
  t.isFirstEnter = false
end
function t.RefreshMyConsortia()
  t.myConsortiaItem:SetBattleRankInfo(t.myConsortiaBattleRankInfo)
end

-------------------click event---------------------------
function t.ClickToggleHandler(go)
  local index = 0
  for k,v in ipairs(t.goToggles) do
    if v == go then
      index = k
      break
    end
  end
  if t.selectToggleIndex == index then
    return
  end
  t.selectToggleIndex = index
  t.Refresh()
end
function t.ClickCloseHandler()
  t.Close()
end
function t.OnConsortiaResetItemHandler(go,index)
  local item = t.consortiaItems[go]
  if item == nil then
    item = consortia_rank_item.BindGameObject(go)
    t.consortiaItems[go] = item
  end
  item:ShowBg(index%2 == 0)
  item:SetBattleRankInfo(t.consortiaBattleRankInfoList[index+1])
end

function t.OnRewardResetItemHandler(go,index)
  local item = t.rewardItems[go]
  if item == nil then
    item = reward_rank_item.BindGameObject(go)
    t.rewardItems[go] = item
  end
  item:SetRewardData(t.rewardRankDataList[index+1])
end


return t