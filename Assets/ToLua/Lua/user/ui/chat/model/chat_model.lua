local t = {}
local name = 'chat_model'
local roleName
local chat_ctrl

t.onUpdateChatRespDelegate = void_delegate.New() -- 
t.onUpdateChatInfoRespDelegate = void_delegate.New() -- 
t.onUpdateCheckRoleGuildDelegate = void_delegate.New() -- 
t.onUpdateByComeMessageWorld = void_delegate.New() -- 
t.onUpdateByComeMessageSystem = void_delegate.New() -- 

local function Start ()  
    t.friendInfoList={}
    t.worldChatInfoList={}
    t.privateChatInfoList={}
    t.guildChatInfoList={}

    t.systemChatInfoList={}
    t.systemChatInfoList.channel1={}
    t.systemChatInfoList.channel2={}

    gamemanager.RegisterModel(name, t)
end

---------------------------update by protocol-------------------
function t.GetSystemChannel(id)
    if id == 1 then
        local rs = t.systemChatInfoList.channel1[1]
        table.remove(t.systemChatInfoList.channel1,1)
        return rs
    else
        local rs = t.systemChatInfoList.channel2[1]
        table.remove(t.systemChatInfoList.channel2,1)
        return rs
    end
end

function t.ChatResp(resp)
        -- print('----------------')
    if resp.chatType == ChatType.World then--世界频道
        local info = {}
        info.chatType = ChatType.World
        info.content = resp.content
        info.talkerName = resp.sendRoleName
        info.isMe = roleName==resp.sendRoleName
        info.time=TimeUtil.FormatTimeToString(resp.sendTime/1000,'MM/dd HH:mm')
        print('chatresp',info.time)
        -- info.timeago=math.floor((math.floor(data.sendTime/1000)-LuaCsTransfer.GetTime())/3600)

        t.AddChatInfo(info)
        t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
    elseif resp.chatType == ChatType.System then--系统广播
        local s = ''
        if resp.noticeNo ~= 0 then
          local broadcastData = gamemanager.GetData('broadcast_data').GetDataById(resp.noticeNo)
          local paramCount = #resp.noticParams
          local baseDes = LocalizationController.instance:Get(broadcastData.des)
          if paramCount == 0 then
            LuaCsTransfer.SystemNoticeAdd(baseDes)
          else
            --特殊id广播 特殊处理
            if broadcastData.id == 2001 and paramCount >= 2 then
              local heroid = tonumber(resp.noticParams[2])
              local heroData = gamemanager.GetData('hero_data').GetDataById(heroid)
              if heroData then
                s = string.format(baseDes,resp.noticParams[1],ui_util.FormatStringWithinQualityColor(heroData.quality,LocalizationController.instance:Get(heroData.name)))
                LuaCsTransfer.SystemNoticeAdd(s)
              else 
                print('can not find herodata by id:',heroid)
              end
            elseif broadcastData.id == 2002 and paramCount >= 3 then
              local heroid = tonumber(resp.noticParams[2])
              local heroData = gamemanager.GetData('hero_data').GetDataById(heroid)
              if heroData then
                s = string.format(baseDes,resp.noticParams[1],ui_util.FormatStringWithinQualityColor(heroData.quality, LocalizationController.instance:Get(heroData.name)),resp.noticParams[3])
                LuaCsTransfer.SystemNoticeAdd(s)
              else 
                print('can not find herodata by id:',heroid)
              end
            elseif  broadcastData.id == 2003 and paramCount >= 2 then --恭喜{0}玩家获得橙色品质装备{1}，如虎添翼，战无不胜
              local equipId = tonumber(resp.noticParams[2])
              local equipData = gamemanager.GetData('equip_data').GetDataById(equipId)
              if equipData then
                s = string.format(baseDes,resp.noticParams[1],ui_util.FormatStringWithinQualityColor(equipData.quality, LocalizationController.instance:Get(equipData.name)))
                LuaCsTransfer.SystemNoticeAdd(s)
              else
                print("can not find equipdata by id:",equipId);
              end
            else
              local str=''
              if #resp.noticParams>0 then
                for i=1,#resp.noticParams do
                    str=str..resp.noticParams[i]
                    if i ~= #resp.noticParams then
                        str=str..';'
                    end
                end
              end
              LuaCsTransfer.SystemNoticeAdd(resp.noticeNo,str)
            end
          end
        else
            LuaCsTransfer.SystemNoticeAdd(resp.content)
        end
    elseif resp.chatType == ChatType.Guild then
        local info = {}
        info.chatType = ChatType.Guild
        info.content = resp.content
        info.talkerName = resp.sendRoleName
        info.isMe = roleName==resp.sendRoleName
        info.master = resp.noticeNo == 1
        info.time=TimeUtil.FormatTimeToString(resp.sendTime/1000,'MM/dd HH:mm')
        info.timeago=math.floor((math.floor(resp.sendTime/1000)-LuaCsTransfer.GetTime())/3600)

        t.AddChatInfo(info)
        t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
    else 
        local info = {}
        info.chatType = ChatType.Private
        info.content = resp.content
        info.talkerName = resp.sendRoleName
        info.revName = resp.revRoleName
        info.isMe = roleName==resp.sendRoleName
        info.time=TimeUtil.FormatTimeToString(resp.sendTime/1000,'MM/dd HH:mm')
        info.timeago=math.floor((math.floor(resp.sendTime/1000)-LuaCsTransfer.GetTime())/3600)

        if chat_ctrl.IsViewOpen() then
            t.AddChatInfo(info)
        else
            t.AddChatInfoPrivate(info)
        end
        t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
    end

    -- print('ChatResp: ',resp.content,resp.noticeNo)
    -- t.onUpdateChatRespDelegate:Invoke()
end

function t.ChatInfoResp(resp)
    if t.alreadyreq then
        t.guildChatInfoList={}
        if resp.hasGuild then
            -- print('resp.hasGuild')
            t.hasGuild=true
        end

        for i = 1,#resp.chatList do
            local data = resp.chatList[i]
            if data.chatType == ChatType.Guild then
              local info = {}
              info.chatType = data.chatType
              info.content = data.content
              info.talkerName = data.sendRoleName
              info.master = data.noticeNo == 1
              info.isMe = roleName==data.sendRoleName
              info.time=TimeUtil.FormatTimeToString(data.sendTime/1000,'MM/dd HH:mm')
              info.timeago=math.floor((math.floor(data.sendTime/1000)-LuaCsTransfer.GetTime())/3600)

              t.AddChatInfo(info)
              t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
            end
        end
        return
    end
    t.alreadyreq = true

    chat_ctrl=gamemanager.GetCtrl('chat_controller')
    roleName=gamemanager.GetModel('game_model').accountName
   
    t.worldChatInfoList={}
    -- t.privateChatInfoList={}
    t.guildChatInfoList={}

    t.systemChatInfoList={}
    t.systemChatInfoList.channel1={}
    t.systemChatInfoList.channel2={}

    if resp.hasGuild then
        t.hasGuild=true
    end
    -- print_warning(string.format('resp.hasGuild:%s',resp.hasGuild))

    for i = 1,#resp.chatList do
        local data = resp.chatList[i]
        if data.chatType == ChatType.World then
          local info = {}
          info.chatType = data.chatType
          info.content = data.content
          info.talkerName = data.sendRoleName
          info.isMe = roleName==data.sendRoleName
          info.time=TimeUtil.FormatTimeToString(data.sendTime/1000,'MM/dd HH:mm')
          -- info.timeago=math.floor((math.floor(data.sendTime/1000)-LuaCsTransfer.GetTime())/3600)

          t.AddChatInfo(info)
          t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
        elseif data.chatType == ChatType.System then
            -- print_warning('ChatType.System')
            local s = resp.content
            if resp.noticeNo ~= 0 then
                  local str=''
                  if #resp.noticParams>0 then
                    for i=1,#resp.noticParams do
                        str=str..resp.noticParams[i]
                        if i ~= #resp.noticParams then
                            str=str..';'
                        end
                    end
                  end
            -- print(s)
                  table.insert(t.systemChatInfoList.channel1,{id=resp.noticeNo,params=str})
                  t.onUpdateByComeMessageSystem:InvokeOneParam(true)
            else
              -- SystemNoticeProxy.instance.AddSystemNotice(s)
                  table.insert(t.systemChatInfoList.channel2,s)
                  t.onUpdateByComeMessageSystem:InvokeOneParam(false)
            end
        elseif data.chatType == ChatType.Guild then
          local info = {}
          info.chatType = data.chatType
          info.content = data.content
          info.talkerName = data.sendRoleName
          info.master = data.noticeNo == 1
          info.isMe = roleName==data.sendRoleName
          info.time=TimeUtil.FormatTimeToString(data.sendTime/1000,'MM/dd HH:mm')
          info.timeago=math.floor((math.floor(data.sendTime/1000)-LuaCsTransfer.GetTime())/3600)
                    

          t.AddChatInfo(info)
          t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
      elseif data.chatType == ChatType.Private then
          -- print('++++++++++++++:',data.sendRoleName,data.revRoleName)
          local info = {}
          info.chatType = ChatType.Private
          info.content = data.content
          info.talkerName = data.sendRoleName
          info.revName = data.revRoleName
          info.isMe = roleName==data.sendRoleName
          info.time=TimeUtil.FormatTimeToString(data.sendTime/1000,'MM/dd HH:mm')
          info.timeago=math.floor((math.floor(data.sendTime/1000)-LuaCsTransfer.GetTime())/3600)
          -- print(data.sendTime)
          -- print(LuaCsTransfer.GetTimeAgo(data.sendTime)/3600)

          t.AddChatInfo(info)
          t.onUpdateByComeMessageWorld:InvokeOneParam(info.isMe)
        end
    end

    for k,v in pairs(t.privateChatInfoList) do
        v.new=false
    end

    -- print('msg quantitiy,' , #t.worldChatInfoList)
    -- t.onUpdateChatInfoRespDelegate:Invoke()
end

function t.CheckRoleGuildResp(resp)
    if resp.isInGuild then
        t.hasGuild=true
    else
        t.hasGuild=false
    end

    t.onUpdateCheckRoleGuildDelegate:Invoke()
end

function t.AddFriendToList(msg)
    local data=string.split(msg,':')
    local id=tonumber(data[1])
    for i=1,#t.friendInfoList do
        if t.friendInfoList[i].id == id then
            -- print('update friend online:',t.friendInfoList[i].online)
            local ol=data[5]=='1'
            t.friendInfoList[i].online=ol
            if ol then
                t.friendInfoList[i].ol=1
            else
                t.friendInfoList[i].ol=0
            end
            t.friendInfoList[i].online=ol
            table.sort(t.friendInfoList,function(a,b)
                return a.ol>b.ol
            end)
          return
        end
    end

    local f={}
    f.id=id
    f.name=data[2]
    f.lv=tonumber(data[3])
    f.icon=ui_util.ParseHeadIcon(tonumber(data[4]))

    local ol=data[5]=='1'
    f.online=ol
    if ol then
        f.ol=1
    else
        f.ol=0
    end

    table.insert(t.friendInfoList,f)
    table.sort(t.friendInfoList,function(a,b)
        return a.ol>b.ol
    end)
    t.privateChatInfoList[f.name]={}
    -- print('-AddFriendToList-',f.name)
end

function t.RemoveFriendToList(id)
    id=tonumber(id)
    for i=1,#t.friendInfoList do
        if t.friendInfoList[i].id == id then
            table.remove(t.friendInfoList,i)
            return
        end
    end
end

---------------------------update by model-------------------
function t.AddChatInfo(info)
    if not info then return end
    if info.chatType == ChatType.World then
        table.insert(t.worldChatInfoList,info)
        if #t.worldChatInfoList > 50 then
            table.remove(t.worldChatInfoList,0)
        end
    elseif info.chatType == ChatType.Guild then
        table.insert(t.guildChatInfoList,info)
        if #t.guildChatInfoList > 50 then
            table.remove(t.guildChatInfoList,0)
        end
    elseif info.chatType == ChatType.Private then
        -- print(roleName, info.talkerName,info.revName)
        for k,v in pairs(t.privateChatInfoList) do
            -- print('pairs ',k)
            if info.talkerName == k and info.revName == roleName then
                table.insert(v,info)
                v.new=true
                -- print('收到来信 ',k)
                t.curTalker= k
                break
            end
            if info.talkerName == roleName and info.revName == k then
                table.insert(v,info)
                -- print('我发出的信息 ',k)
                t.curTalker= roleName
                t.curRever= k
                break
            end
        end
    end
end

function t.AddChatInfoPrivate(info)
    if info.chatType == ChatType.Private then
        -- print(roleName, info.talkerName,info.revName)
        for k,v in pairs(t.privateChatInfoList) do
            -- print('pairs ',k)
            if info.talkerName == k and info.revName == roleName then
                table.insert(v,info)
                v.new=true
                -- print('收到来信 ',k)
                Observers.Facade.Instance:SendNotification('OnRedPointChange')
                break
            end
        end
    end
end

function t.CreateChatInfo(s,chatType)
    if(#s>140) then
        s =string.sub(s,1,140)
    end

    local info = {}
    info.talkerName = roleName
    info.chatType = chatType
    info.isMe = true
    info.content = s
    return info
end

--外部红点检测
function t.HasNew()
    for k,v in pairs(t.privateChatInfoList) do
        if v.new then
            return true
        end
    end
    return false
end

function t.CheckNew(name)
    -- print('CheckNew:',name)
    local p = t.privateChatInfoList[name]
    if p then
        -- print('find p ',p.new)
        return p.new
    end
    print_warning(string.format('CheckNew:not exist friend:%s',name))
    return false
end

function t.SetNew(name,new)
    local p = t.privateChatInfoList[name]
    if p then
        p.new=new
        -- print('SetNew:',name,new)
        return
    end

    print_warning(string.format('SetNew:not exist friend:%s',name))
end

function t.HasFriend()
    for i=1,#t.friendInfoList do
        if t.friendInfoList[i].online then
            return true
        end
    end
    return false
end

Start()
return t