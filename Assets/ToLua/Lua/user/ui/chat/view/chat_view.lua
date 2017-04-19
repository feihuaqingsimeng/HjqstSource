local t = {}
local PREFAB_PATH = 'ui/chat/groupchat_view'

local chat_controller = gamemanager.GetCtrl('chat_controller')
local chat_model = gamemanager.GetModel('chat_model')

local temp
local using={}
local usingFriend={}
local usingPrivate={}
local Localization
local playerName

local function Start()
    t.toggle=dofile('ui/chat/view/chat_toggle_item')
    t.guilditem=dofile('ui/chat/view/chat_content_item')
    t.privateitem=dofile('ui/chat/view/private_chat_item')
    t.frienditem=dofile('ui/chat/view/private_friend_item')
    t.frienditem.Init(t)

    t.data=gamemanager.GetData('consortia_shop_data')
    Localization=LocalizationController.instance
    t.cd=tonumber(require('global').GetItem('chat_cd').value)
    
    playerName=gamemanager.GetModel('game_model').accountName

    uimanager.RegisterView('chat_view',t)
end

function t.Open()
  t.transform = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay).transform
  t.InitComponent()

  t.BindDelegate()
  t.Init()
  t.open=true
  -- t.ChatInfoReq()
end

function t.Close()
    if t.oldFriend then
        t.oldFriend.select=false
        t.oldFriend=nil
    end
    
    t.open=false

    t.UnTick()
    t.UnbindDelegate()
    UIMgr.instance:Close(PREFAB_PATH)
    -- Observers.Facade.Instance:SendNotification('OnRedPointChange')
end

function t.BindDelegate()
  chat_model.onUpdateByComeMessageWorld:AddListener(t.ByComeMessage)
  chat_model.onUpdateByComeMessageSystem:AddListener(t.ByComeMessageSystem)
  chat_model.onUpdateCheckRoleGuildDelegate:AddListener(t.CheckRoleGuild)
end

function t.UnbindDelegate()
  chat_model.onUpdateByComeMessageWorld:RemoveListener(t.ByComeMessage)
  chat_model.onUpdateByComeMessageSystem:RemoveListener(t.ByComeMessageSystem)
  chat_model.onUpdateCheckRoleGuildDelegate:RemoveListener(t.CheckRoleGuild)
end

function t.SetFriendNotOnline()
    print_warning(string.format('SetFriendNotOnline:%s',t.oldFriend.name))
    t.oldFriend.online=false
    for i=1,#usingFriend do
        if usingFriend[i].data == t.oldFriend then
            usingFriend[i]:SetOnline(false)
            return
        end
    end
    print_warning(string.format('没有找到当前好友:%s',t.oldFriend.name))
end

function t.ByComeMessageSystem(isMeSend)
    if t.currentChatType == ChatType.World or t.currentChatType == ChatType.Guild then
        if isMeSend then
            -- local rs = chat_model.GetSystemChannel(1)
            -- LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
            -- print(rs.id)
            LuaCsTransfer.SystemNoticeAdd(rs.id,rs.params)
        else
            local rs = chat_model.GetSystemChannel(2)
            -- LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
            LuaCsTransfer.SystemNoticeAdd(rs)
        end
    end
end

function t.ByComeMessage(isMeSend)
    t.toggles[3]:HasNews(chat_model.HasNew())

    local scrollValue = t.scrollContent1.scrollRect.verticalScrollbar.value
    if t.currentChatType == ChatType.World then
        t.scrollContent1:RefreshCount(#chat_model.worldChatInfoList,-1)
    elseif t.currentChatType == ChatType.Guild then
        t.scrollContent1:RefreshCount(#chat_model.guildChatInfoList,-1)
    elseif t.currentChatType == ChatType.Private then
        -- print('ByComeMessage ',t.oldFriend)

        if chat_model.curTalker then
            if t.oldFriend then
                if chat_model.curTalker == t.oldFriend.name then
                    chat_model.SetNew(t.oldFriend.name,false)
                    t.scrollContent2:RefreshCount(#chat_model.privateChatInfoList[chat_model.curTalker],-1)
                elseif chat_model.curTalker == playerName then
                    if chat_model.curRever == t.oldFriend.name then
                        chat_model.SetNew(t.oldFriend.name,false)
                        t.scrollContent2:RefreshCount(#chat_model.privateChatInfoList[chat_model.curRever],-1)
                    end
                end
            end
            chat_model.curTalker=nil
            chat_model.curRever=nil
        end

    -- elseif t.currentChatType == ChatType.System then
    --     -- print('ByComeMessage ',#chat_model.privateChatInfoList[t.oldFriend.name])
    --     --t.scrollContent2:RefreshCount(#chat_model.privateChatInfoList[t.oldFriend.name],-1)
    --     if isMeSend then
    --         local rs = chat_model.GetSystemChannel(1)
    --         -- LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
    --         print(rs[1])
    --         print(#rs[2])
    --         LuaCsTransfer.SystemNoticeAdd(rs[1],rs[2])
    --     else
    --         local rs = chat_model.GetSystemChannel(2)
    --         -- LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
    --         LuaCsTransfer.SystemNoticeAdd(rs[1])
    --     end
    --     return
    end

    if isMeSend then
        if t.currentChatType ~= ChatType.Private then
            t.scrollContent1:ScrollToBottom(0.3)
        else
            if t.oldFriend then
                 t.scrollContent2:ScrollToBottom(0.3)
            end
        end
    else
        if scrollValue<=0.1 then
            if t.currentChatType ~= ChatType.Private then
                t.scrollContent1:ScrollToBottom(0.3)
            else
                if t.oldFriend then
                  t.scrollContent2:ScrollToBottom(0.3)
                end
            end
        end
    end
end

function t.CheckRoleGuild()
    --chat_model.hasGuild
end

function t.ChatInfoReq()


  chat_controller.ChatInfoReq()
end

function t.Init()
    t.canSendMessage=true
    -- SystemNoticeView.Create(systemNoticeRoot)
    LuaCsTransfer.SystemNoticeCreate(t.systemNoticeRoot)
    
    t.toggles[3]:HasNews(chat_model.HasNew())

    t.Refresh()
end

function t.Refresh()
    if t.currentChatType == ChatType.World then
        -- print('t.Refresh World()',#chat_model.worldChatInfoList)
        t.scrollContent1:Init(#chat_model.worldChatInfoList,false,0)
        t.scrollContent1:ScrollToBottom(0.1)
    elseif t.currentChatType == ChatType.Guild then
        -- print('t.Refresh Guild()',#chat_model.guildChatInfoList)
        t.scrollContent1:Init(#chat_model.guildChatInfoList,false,0)
        t.scrollContent1:ScrollToBottom(0.1)
    elseif t.currentChatType == ChatType.Private then
        if chat_model.privateChatInfoList[t.oldFriend.name] then
            -- print('t.Refresh Private',#chat_model.privateChatInfoList[t.oldFriend.name])
            t.scrollContent2:Init(#chat_model.privateChatInfoList[t.oldFriend.name],false,0)
            t.scrollContent2:ScrollToBottom(0.1)
        else
            -- print('t.Refresh Private 0')
            t.scrollContent2:Init(0,false,0)
        end
    end
end

function t.InitComponent()
    local transform=t.transform   

    t.general=transform:FindChild('core/Scroll View').gameObject
    t.systemNoticeRoot=transform:FindChild('core/SystemBoardNoticeView')

    t.scrollContent1=transform:FindChild('core/Scroll View/Viewport/Content'):GetComponent(typeof(Common.UI.Components.ScrollContentExpand))
    t.scrollContent1:AddResetItemListener(t.onResetItem)
    t.scrollContent2=transform:FindChild('core/private/right/scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContentExpand))
    t.scrollContent2:AddResetItemListener(t.onResetItemPrivate)

    t.private=transform:FindChild('core/private').gameObject
    t.scrollContentFriend=transform:FindChild('core/private/left/scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
    t.scrollContentFriend.onResetItem:AddListener(t.onResetItemFriend)

    t.privatesv=transform:FindChild('core/private/right/scrollview').gameObject
    t.privatebottom=transform:FindChild('core/private/bottom').gameObject
    t.privatechattitle=transform:FindChild('core/private/right/title'):GetComponent(typeof(Text))

    t.select=transform:FindChild('core/private/left/scrollview/viewport/select')
    t.bottom1=transform:FindChild('core/bottom').gameObject

    t.inputText1=transform:FindChild('core/bottom/InputField'):GetComponent(typeof(InputField))
    t.inputText2=transform:FindChild('core/private/bottom/InputField'):GetComponent(typeof(InputField))

    t.toggles={}
    local tog=t.toggle.New(transform:FindChild('core/toggle_root/toggle1'),ChatType.World)
    tog.onClick:AddListener(t.ClickToggle)
    table.insert(t.toggles,tog)
    tog=t.toggle.New(transform:FindChild('core/toggle_root/toggle2'),ChatType.Guild)
    tog.onClick:AddListener(t.ClickToggle)
    table.insert(t.toggles,tog)
    tog=t.toggle.New(transform:FindChild('core/toggle_root/toggle3'),ChatType.Private)
    tog.onClick:AddListener(t.ClickToggle)
    table.insert(t.toggles,tog)

    t.currentChatType = ChatType.World
    t.tog=t.toggles[1]
    t.tog:SetSelect(true)

    transform:FindChild('core/btn_close'):GetComponent(typeof(Button)).onClick:AddListener(t.Close)
    transform:FindChild('core/bottom/btn_commit'):GetComponent(typeof(Button)).onClick:AddListener(t.Commit)
    transform:FindChild('core/private/bottom/btn_commit'):GetComponent(typeof(Button)).onClick:AddListener(t.Commit)
    transform.gameObject:SetActive(true)
end

function t.Commit()
    if t.currentChatType == ChatType.World then
        local text=t.inputText1.text
        if not text or text =='' then return end
        if t.canSendMessage then
            local s = text--Common.Util.BlackListWordUtil.WordsFilter(text)
            local info =chat_model.CreateChatInfo(s,t.currentChatType)
            t.ClearEditData()
            chat_controller.ChatReq(info.chatType,info.talkerName,info.content)
            t.StartCD()
        else
            require('ui/tips/view/auto_destroy_tip_view').Open(Localization:Get('ui.chat_view.toomuchop'))
        end
    elseif  t.currentChatType == ChatType.Guild then
        local text=t.inputText1.text
        if not text or text =='' then return end
        local s = text--Common.Util.BlackListWordUtil.WordsFilter(text)
        local info =chat_model.CreateChatInfo(s,t.currentChatType)
        t.ClearEditData()
        chat_controller.ChatReq(info.chatType,info.talkerName,info.content)
    else
        local text=t.inputText2.text
        if not text or text =='' then return end
        local s = text--Common.Util.BlackListWordUtil.WordsFilter(text)
        local info =chat_model.CreateChatInfo(s,t.currentChatType)
        t.ClearEditData()
        if t.oldFriend then
            chat_controller.ChatReq(info.chatType,t.oldFriend.name,info.content)
        end
    end
end

function t.ClearEditData( ... )
    t.inputText1.text=''
    t.inputText2.text=''
end

function t.StartCD( ... )
    coroutine.start(t.startcd)
end

function t.startcd()
    t.canSendMessage=false
    coroutine.wait(t.cd)
    t.canSendMessage=true
end

function t.ClickToggle(tog,typeid)
    -- print('ClickToggle ',t.currentChatType,typeid)
    if t.currentChatType ~= typeid then
        if typeid ~= ChatType.World and typeid ~= ChatType.Guild then 
            t.systemNoticeRoot.gameObject:SetActive(false)
            t.general:SetActive(false)
            t.private:SetActive(true)
            -- print('#chat_model.friendInfoList',#chat_model.friendInfoList)
            t.scrollContentFriend:Init(#chat_model.friendInfoList,false,0)
            t.tog=tog
            t.currentChatType = typeid
            t.privatechattitle.text='点击左侧好友聊天'

            if chat_model.HasFriend() then
                t.privatechattitle.gameObject:SetActive(true)
            else
                t.privatechattitle.gameObject:SetActive(false)
            end

            t.tog:SetSelect(true)
            t.bottom1:SetActive(false)
            return
        end    -- temp!
        
        if typeid == ChatType.Guild then
            if not chat_model.hasGuild then 
                uimanager.ShowTipsOnly(Localization:Get('ui.chat_view.noguild'))
                if t.tog then
                    t.tog:SetSelect(true)
                end
                -- t.currentChatType = typeid
                return
            end
        end

        t.bottom1:SetActive(true)
        t.privatesv:SetActive(false)
        t.privatebottom:SetActive(false)
        t.systemNoticeRoot.gameObject:SetActive(true)

        if t.oldFriend then
            t.oldFriend.select=false
            t.oldFriend=nil
        end

        t.select:SetParent(t.transform)
        t.select.gameObject:SetActive(false)

        t.general:SetActive(true)
        t.private:SetActive(false)

        t.tog=tog
        t.currentChatType = typeid
        t.Refresh()
    end
end

function t.onResetItem(go,idx)
    idx=idx+1
    -- print('setitem',idx)
    temp = t.GenItem(go)
    if t.currentChatType == ChatType.World then
        temp:Setup(chat_model.worldChatInfoList[idx],ChatType.World)
    elseif t.currentChatType == ChatType.Guild then
        temp:Setup(chat_model.guildChatInfoList[idx],ChatType.Guild)
    end
end

function t.onResetItemPrivate(go,idx)
    idx=idx+1
    -- print('setitem',idx)
    temp = t.GenItemPrivate(go)
    temp:Setup(chat_model.privateChatInfoList[t.oldFriend.name][idx])
end

function t.onResetItemFriend(go,idx)
    idx=idx+1
    -- print('setitem',idx)
    temp = t.GenItemFriend(go)
    temp:Setup(chat_model.friendInfoList[idx],chat_model.CheckNew(chat_model.friendInfoList[idx].name))
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
end

function t.GenItem(go)
  for i=1,#using do
    if using[i].go==go then
      using[i]:Close()
      return using[i]
    end
  end
  temp=t.guilditem.New(go)
  table.insert(using,temp)
  return temp
end

function t.GenItemFriend(go)
  for i=1,#usingFriend do
    if usingFriend[i].go==go then
      usingFriend[i]:Close()
      return usingFriend[i]
    end
  end
  temp=t.frienditem.New(go)
  temp.onClick:AddListener(t.ClickFriend)
  table.insert(usingFriend,temp)
  return temp
end

function t.GenItemPrivate(go)
  for i=1,#usingPrivate do
    if usingPrivate[i].go==go then
      usingPrivate[i]:Close()
      return usingPrivate[i]
    end
  end
  temp=t.privateitem.New(go)
  table.insert(usingPrivate,temp)
  return temp
end

function t.ClickFriend(item)
    if not item.data.online and not item.data.new then
        print('not online')
        uimanager.ShowTipsOnly('对方不在线')
        return
    end

    if t.oldFriend  then
        if t.oldFriend == item.data then
            return
        end

        t.oldFriend.select=false
    end

    t.privatechattitle.text=string.format('与%s聊天',item.data.name)
    t.privatesv:SetActive(true)
    t.privatebottom:SetActive(true)

    t.oldFriend=item.data

    item:SetSelect()
    -- chat_model.SetNew(item.data.name,false)
    t.toggles[3]:HasNews(chat_model.HasNew())
    Observers.Facade.Instance:SendNotification('OnRedPointChange')

    t.Refresh()
end

Start()
return t
