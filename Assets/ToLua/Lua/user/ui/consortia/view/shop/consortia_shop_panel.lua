local t = {}
local PREFAB_PATH = 'ui/consortia/shop/consortia_shop_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')

local temp
local using={}
local cdSeconds
local LocalizationGet

local function Start()
  t.item=dofile('ui/consortia/view/shop/consortia_shop_item')
  t.item.Init(t)
  t.data=gamemanager.GetData('consortia_shop_data')
  Localization=LocalizationController.instance
end

function t.Open(parent)
  t.BindDelegate()

  cdSeconds=-1
  local refresh=string.split(gamemanager.GetData('global_data').guild_shop_refresh,';')
  local cur=LuaCsTransfer.GetTime()
  local goal=nil
  for i=1,#refresh do
    goal=LuaCsTransfer.GetTime_Stamp(refresh[i])
    if goal > cur then
      cdSeconds=goal-cur  +2  --invent of server time
      break
    end
  end

  -- print('cdSeconds ',cdSeconds)
  -- cdSeconds=5
  consortia_controller.GuildShopInfoReq()

  if cdSeconds>0 then
    t.Tick()
  end

  t.transform =Instantiate(ResMgr.instance:Load(PREFAB_PATH)).transform
  t.transform:SetParent(parent)
  t.transform.localScale=Vector3.one
  t.transform.localPosition=Vector3.zero

  t.InitComponent()
end

function t.Close()
  t.UnTick()
  t.UnbindDelegate()
  GameObject.Destroy(t.transform.gameObject)
end

function t.BindDelegate()
  consortia_model.onUpdateGuildShopInfoDelegate:AddListener(t.GuildShopInfo)
  consortia_model.onUpdateGuildShopBuyRespDelegate:AddListener(t.GuildShopBuyResp)
  consortia_model.onUpdateGuildShopSynInfoRespDelegate:AddListener(t.GuildShopSynInfoResp)
end

function t.UnbindDelegate()
  consortia_model.onUpdateGuildShopInfoDelegate:RemoveListener(t.GuildShopInfo)
  consortia_model.onUpdateGuildShopBuyRespDelegate:RemoveListener(t.GuildShopBuyResp)
  consortia_model.onUpdateGuildShopSynInfoRespDelegate:RemoveListener(t.GuildShopSynInfoResp)
end

function t.GuildShopInfo()
  t.list={}
  local info=nil
  for i=1,#consortia_model.guildShopInfo do
    info=consortia_model.guildShopInfo[i]
    if table.contain(consortia_model.hasBuy,info.type) then
        table.insert(t.list,{id=info.no,type=info.type,buyed=true})
    else
        table.insert(t.list,{id=info.no,type=info.type,buyed=false})
    end
  end

  t.content:Init(#t.list,true,0)
end

function t.GuildShopSynInfoResp()
  print('商店同步')
end

function t.GuildShopBuyResp()
  print('响应购买')
  consortia_model.mGuildShopBuyResp(t.item.price)
  uimanager.ShowTipsOnly(Localization:Get('ui.consortia_view.shop.tips_buyed'))
  t.item:SetBuyed()
  t.donateText.text=string.format('<color=#00b2fe>%s</color>',consortia_model.consortiaInfo.roleGuild.contribute)
end

function t.GuildShopBuyReq(item)
  print('准备购买',item.type)
  t.item=item
  consortia_controller.GuildShopBuyReq(item.type)
end

function t.InitComponent()
  local transform=t.transform
  transform:FindChild('title/text_name'):GetComponent(typeof(Text)).text=Localization:Get('ui.consortia_view.shop')

  t.content=transform:FindChild('scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
  t.content.onResetItem:AddListener(t.onResetItem)

  transform:FindChild('bottom/donate/title'):GetComponent(typeof(Text)).text=Localization:Get('ui.consortia_view.shop.donated')
  t.donateText=transform:FindChild('bottom/donate/Text'):GetComponent(typeof(Text))
  t.donateText.text=string.format('<color=#00b2fe>%s</color>',consortia_model.consortiaInfo.roleGuild.contribute)
  t.textCountDown = transform:Find('title/text_countDown'):GetComponent(typeof(Text))
  t.textCountDown.text = ''
  local refresh=string.split(gamemanager.GetData('global_data').guild_shop_refresh,';')
  transform:FindChild('bottom/tips/title'):GetComponent(typeof(Text)).text=string.format(Localization:Get('ui.consortia_view.shop.tips'),refresh[1],refresh[2],refresh[3])

  transform.gameObject:SetActive(true)
end

function t.Tick()
  t.cotick=coroutine.start(t.tick)
end

function t.UnTick()
  if not t.cotick then return end
  coroutine.stop(t.cotick)
end

function t.tick()
  while true do
    if cdSeconds == 0 then break end
    coroutine.wait(1)
    t.ticked()
  end
end

function t.ticked()
  cdSeconds=cdSeconds-1
  -- print('ticked ',cdSeconds)
  if cdSeconds == 0 then
    -- print('tickedok')
    
    consortia_controller.GuildShopInfoReq()
  end
  t.textCountDown.text = string.format(Localization:Get('ui.consortia_view.shop.time'), TimeUtil.FormatSecondToHour(cdSeconds))
end

function t.onResetItem(go,idx)
  idx=idx+1
  temp=t.GenItem(go)
  temp:Setup(idx,
             t.list[idx].buyed,
             consortia_model.consortiaInfo,
             t.data.GetDataById(t.list[idx].id))
end

function t.GenItem(go)
  for i=1,#using do
    if using[i].go==go then
      using[i]:Close()
      return using[i]
    end
  end
  temp=t.item.New(go)
  table.insert(using,temp)
  return temp
end


Start()
return t
