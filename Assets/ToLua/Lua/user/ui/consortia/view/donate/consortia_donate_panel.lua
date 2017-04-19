local t = {}
local PREFAB_PATH = 'ui/consortia/donate/consortia_donate_panel'

local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_data = gamemanager.GetData('consortia_data')

function t.Open(parent)
  t.transform =Instantiate(ResMgr.instance:Load(PREFAB_PATH)).transform
  t.transform:SetParent(parent)
  t.transform.localScale=Vector3.one
  t.transform.localPosition=Vector3.zero

  t.BindDelegate ()
  t.InitComponent()
end

function t.Close()
  GameObject.Destroy(t.transform.gameObject)
  t.UnbindDelegate()
end

function t.BindDelegate()
  consortia_model.onUpdateGuildSyncDelegate:AddListener(t.GuildSyncDelegate)
  consortia_model.onUpdateGuildPresentRespDelegate:AddListener(t.GuildPresentResp)
end

function t.UnbindDelegate()
  consortia_model.onUpdateGuildSyncDelegate:RemoveListener(t.GuildSyncDelegate)
  consortia_model.onUpdateGuildPresentRespDelegate:RemoveListener(t.GuildPresentResp)
end

function t.RefreshConsortiaExp ()
  local currentConsortiaInfo = consortia_model.consortiaInfo
  
  if not currentConsortiaInfo:IsMaxLevel() then  
    t.consortiaExpSlider.value = currentConsortiaInfo:ExpPercent()
    t.consortiaExpText.text = string.format('%d/%d', currentConsortiaInfo.exp,consortia_data.GetDataById(currentConsortiaInfo.lv+1).exp)
  else
    t.consortiaExpSlider.value = 1
    t.consortiaExpText.text = tostring(currentConsortiaInfo:CurrenExp())
  end
end

function t.GuildSyncDelegate()
  local info=consortia_model.consortiaInfo
  t.contributeText.text=''..info.roleGuild.contribute
  t.remaintimesText.text=''..info.roleGuild.remainPresentTimes
  t.remaintimes=info.roleGuild.remainPresentTimes
  
  t.RefreshConsortiaExp ()
end

function t.GuildPresentResp()
  uimanager.ShowTipsOnly(LocalizationController.instance:Get('ui.consortia_view.donateok'))
end

function t.InitComponent()
  local info=consortia_model.consortiaInfo
  local data=gamemanager.GetData('consortia_data')
  local obj = t.transform:FindChild('center/card')

  t.transform:FindChild('title/text_name'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('ui.consortia_view.donate')
  t.remaintimes=info.roleGuild.remainPresentTimes
  --if lv==id
  data=data.GetDataById(info.lv)
  local card = 1
  for i=1,3 do
    card=t.transform:FindChild('center/card'..i)
    t.SetCard(card,data,i)
  end
  
  local currentConsortiaInfo = consortia_model.consortiaInfo
  
  t.consortiaExpSlider = t.transform:FindChild('bottom/slider_consortia_exp'):GetComponent(typeof(Slider))
  t.consortiaExpText = t.transform:FindChild('bottom/text_consortia_exp'):GetComponent(typeof(Text))
  
  t.contributeText=t.transform:FindChild('bottom/donate/Text'):GetComponent(typeof(Text))
  t.transform:FindChild('bottom/donate/title'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('ui.consortia_view.donatedname')
  t.contributeText.text=''..info.roleGuild.contribute

  t.remaintimesText=t.transform:FindChild('bottom/remaintimes/Text'):GetComponent(typeof(Text))
  t.transform:FindChild('bottom/remaintimes/title'):GetComponent(typeof(Text)).text=LocalizationController.instance:Get('ui.consortia_view.remain_time')
  t.remaintimesText.text=''..info.roleGuild.remainPresentTimes


  t.RefreshConsortiaExp ()
  t.transform.gameObject:SetActive(true)
end

function t.SetCard(transform,data,index)
    local price=data.guild_donation_gold[index]
    local donate=data.guild_donation_award[index]
    local exp=data.guild_donation_exp[index]

    local name = transform:FindChild('name'):GetComponent(typeof(Text))
    if index ==1 then
      name.text=LocalizationController.instance:Get('ui.consortia_view.donate1')
    elseif index ==2 then
      name.text=LocalizationController.instance:Get('ui.consortia_view.donate2')
    elseif index ==3 then
      name.text=LocalizationController.instance:Get('ui.consortia_view.donate3')
    end

    --[[local flash=transform:FindChild('icon'):GetComponent(typeof(Image))
    if price.type == BaseResType.Gold then
      flash.sprite=ResMgr.instance:LoadSprite('sprite/item_icon/icon_coin_3')
    elseif price.type == BaseResType.Diamond then
      flash.sprite=ResMgr.instance:LoadSprite('sprite/item_icon/icon_gem_2')
    elseif price.type == BaseResType.Crystal then
      flash.sprite=ResMgr.instance:LoadSprite('sprite/item_icon/icon_gem_3')
    end]]

    transform:FindChild('values/donateText'):GetComponent(typeof(Text)).text=string.format(LocalizationController.instance:Get('ui.consortia_view.donated'),donate.count)
    transform:FindChild('values/expText'):GetComponent(typeof(Text)).text=string.format(LocalizationController.instance:Get('ui.consortia_view.exp'),exp)

    transform:FindChild('Button/icon'):GetComponent(typeof(Image)).sprite=ResMgr.instance:LoadSprite(LuaCsTransfer.GetIcon(price.type,0))
    transform:FindChild('Button/Text'):GetComponent(typeof(Text)).text=''..price.count

    transform:FindChild('Button'):GetComponent(typeof(Button)).onClick:AddListener(function()
      t.ClickCard(index,price)
    end)
end

function t.ClickCard(index,price)
  -- print('clickcard req',index)
  if t.remaintimes <1 then
    uimanager.ShowTipsOnly(LocalizationController.instance:Get('ui.consortia_view.remaintimestips'))
    return
  end
  if not gamemanager.GetModel('game_model').CheckBaseResEnoughByType(price.type,price.count) then
    return 
  end

  if price.type == BaseResType.Diamond then
    if gamemanager.GetModel('consume_tip_model').GetConsumeTipEnable(ConsumeTipType.DiamondConsumeGuildDonate) then
      local str = LocalizationController.instance:Get(gamemanager.GetData('consume_tip_data').GetDataById(1019).des)
      require('ui/tips/view/confirm_cost_tip_view').Open(BaseResType.Diamond,price.count,str,
        function()
            consortia_controller.GuildPresentReq(index)
        end
      ,ConsumeTipType.DiamondConsumeGuildDonate)
      return
    end
  end
  consortia_controller.GuildPresentReq(index)
end

return t
