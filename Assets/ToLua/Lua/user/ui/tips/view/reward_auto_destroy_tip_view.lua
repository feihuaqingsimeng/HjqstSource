local t = {}
local PREFAB_PATH = "ui/tips/common_reward_auto_destroy_tips_view"

t.dataList = {}
t.dataListCount = 0
t.RefreshCoroutine = nil
t.goTipList = {}
local hero_data = gamemanager.GetData('hero_data')
local equip_data = gamemanager.GetData('equip_data')
local item_data = gamemanager.GetData('item_data')

function t.Open(gameResData)
  local dataTable = {}
  dataTable[1] = gameResData
  t.OpenByList(dataTable,false)
end
--isCombineSameData 合并相同类型
function t.OpenByList(gameResDataList,isCombineSameData)
  if table.count(gameResDataList) == 0 then
    print(ui_util.FormatToRedText('奖励列表为空'))
    return
  end
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.InitComponent()
    
  for k,v in pairs(gameResDataList) do
    t.dataListCount = t.dataListCount + 1
    t.dataList[t.dataListCount] = v
  end
  if not t.RefreshCoroutine then
    t.RefreshCoroutine = coroutine.start(t.Refresh)
  end
end

function t.InitComponent()
  t.tranTipRoot = t.transform:Find('core/tip_root')
  t.goTip = t.transform:Find('core/text_tips').gameObject
  t.goTip:SetActive(false)
end
function t.Refresh()
  local tips = 'error'
  local i = 1
  local resData = nil
  while true do
    if i > t.dataListCount then
      coroutine.wait(0.1)
      
    else
      resData = t.dataList[i]
      if resData.type == BaseResType.Hero then
        local heroData = hero_data.GetDataById(resData.id)
        tips = string.format(LocalizationController.instance:Get("ui.reward_view.gain_hero_lua"),resData.star,LocalizationController.instance:Get( heroData.name),resData.count)
      elseif resData.type == BaseResType.Equipment then
        local equipData = equip_data.GetDataById(resData.id)
        tips = string.format(LocalizationController.instance:Get("ui.reward_view.gain_equip_lua"),LocalizationController.instance:Get(equipData.name),resData.count)
        
      elseif resData.type == BaseResType.Item then
        local itemData = item_data.GetDataById(resData.id)
        tips = string.format(LocalizationController.instance:Get("ui.reward_view.gain_item_lua"),LocalizationController.instance:Get(itemData.name),resData.count)
      else
        local itemData = item_data.GetBasicResItemByType(resData.type)
        tips = string.format(LocalizationController.instance:Get("ui.reward_view.gain_base_res_lua"),LocalizationController.instance:Get(itemData.name),resData.count);
      end
      --已显示出的tip上移
      for k,v in pairs(t.goTipList) do
        if v then
          CommonMoveByAnimation.Get(v):Init(0.15,0,Vector3(0,30,0))
        end
      end
      --创建新的text
      local tipObj = GameObject.Instantiate(t.goTip)
      tipObj:SetActive(true)
      tipObj.transform:SetParent(t.tranTipRoot,false)
      tipObj.transform.localPosition = Vector3(0,0,0)
      tipObj:GetComponent(typeof(Text)).text = tips
      CommonFadeToAnimation.Get(tipObj):init(0,1,0.3,0)
      t.goTipList[tipObj] = tipObj
      coroutine.start(t.FadeOutCoroutine,2,tipObj)
      coroutine.wait(1)
      
      i = i + 1
    end
  end
end

function t.FadeOutCoroutine(delay,tipObj)
  coroutine.wait(delay)
  local time = 0.3
  CommonFadeToAnimation.Get(tipObj):init(1,0,time,0)
  coroutine.wait(time)
  --GameObject.DestroyImmediate(tipObj)--destroy 有问题
  tipObj.transform:SetParent(tipObj.transform.parent.parent)
  if t.tranTipRoot.childCount == 0 then
    t.Close()
  end
end

function t.Close()
  coroutine.stop(t.RefreshCoroutine)
  t.RefreshCoroutine = nil
  t.goTipList = {}
  t.dataListCount = 0
  t.dataList = {}
  UIMgr.instance:Close(PREFAB_PATH)
  
end

return t