local t = {}
t.__index=t

local textMaster
-- local textNotMaster

local function Start()
  textMaster=LocalizationController.instance:Get('ui.consortia_view.chat.master')
  -- textNotMaster=LocalizationController.instance:Get('ui.consortia_view.chat.notmaster')
end

function t.New(go)
    local r={}
    setmetatable(r,t)
    r.go=go
    r:InitComponent()
    r.contentImgBgDefaultHeight = 20
    
    return r
end

function t:InitComponent()
    local transform=self.go.transform
    self.root=transform:GetComponent(typeof(RectTransform))

	self.leftRoot=transform:FindChild('left').gameObject
    self.rightRoot=transform:FindChild('right').gameObject
    self.rightTitleText=transform:FindChild('right/text_title'):GetComponent(typeof(Text))
    self.rightContentBg=transform:FindChild('right/img_bg'):GetComponent(typeof(RectTransform))
    self.rightContentText=transform:FindChild('right/img_bg/text_content'):GetComponent(typeof(Text))
	
	self.leftTitleText=transform:FindChild('left/text_title'):GetComponent(typeof(Text))
     self.leftContentBg=transform:FindChild('left/img_bg'):GetComponent(typeof(RectTransform))
    self.leftContentText=transform:FindChild('left/img_bg/text_content'):GetComponent(typeof(Text))

end

function t:Setup(chatInfo,chatType)
  local titleText
	local contentText
	local contentBg

	-- print(chatInfo.time)

	self.leftRoot:SetActive(not chatInfo.isMe)
	self.rightRoot:SetActive(chatInfo.isMe)

	if chatInfo.isMe then
		titleText = self.rightTitleText
		contentBg = self.rightContentBg
		contentText = self.rightContentText
    titleText.text = string.format("%s [%s]",chatInfo.talkerName,chatInfo.time)
		--[[if chatType == ChatType.World then
			titleText.text = string.format("[%s] %s",chatInfo.time,chatInfo.talkerName)
		else
			if chatInfo.timeago <=0 then
				titleText.text = string.format("[%s] %s",chatInfo.time,chatInfo.talkerName)
			elseif chatInfo.timeago <=10 then
				titleText.text = string.format("[%s天前] %s",chatInfo.timeago,chatInfo.talkerName)
			else
				titleText.text = string.format("[10天前] %s",chatInfo.time,chatInfo.talkerName)
			end
		end]]
	else
		titleText = self.leftTitleText
		contentBg = self.leftContentBg
		contentText = self.leftContentText
		titleText.transform.localPosition=Vector3(13,-7,0)
    titleText.text = string.format("%s [%s]",chatInfo.talkerName,chatInfo.time)
		--[[if chatType == ChatType.World then
			titleText.text = chatInfo.talkerName
		else
			-- print('chatInfo.master:',chatInfo.master)
			if chatInfo.master then
        titleText.text
				if chatInfo.timeago <=0 then
					titleText.text = string.format("[%s] <color=#1af412>%s%s</color>",chatInfo.time,chatInfo.talkerName)
				elseif chatInfo.timeago <=10 then
					titleText.text = string.format("[%s天前] <color=#1af412>%s%s</color>",chatInfo.timeago,chatInfo.talkerName)
				else
					titleText.text = string.format("[10天前] <color=#1af412>%s%s</color>",chatInfo.time,chatInfo.talkerName)
				end

				titleText.transform.localPosition=Vector3(6,-7,0)
			else
				if chatInfo.timeago <=0 then
					titleText.text = string.format("[%s] %s",chatInfo.time,chatInfo.talkerName)
				elseif chatInfo.timeago <=10 then
					titleText.text = string.format("[%s天前] %s",chatInfo.timeago,chatInfo.talkerName)
				else
					titleText.text = string.format("[10天前] %s",chatInfo.time,chatInfo.talkerName)
				end
			end
		end]]
	end

	contentText.text = chatInfo.content
		
	local h = contentText.preferredHeight
	local w = contentText.preferredWidth
  
	
	local titleTextW = titleText.preferredWidth
	local delta = contentText.rectTransform.sizeDelta.x-w
	if (delta < 0 ) then
		delta = 0
	end
  
  if(contentBg) then
    local contentBgExtraW = 30
    local bgW = contentText.preferredWidth+contentBgExtraW
    if bgW > contentText.rectTransform.sizeDelta.x then
      bgW = contentText.rectTransform.sizeDelta.x + contentBgExtraW
    end
		contentBg.sizeDelta = Vector2(bgW,h+self.contentImgBgDefaultHeight)
    if chatInfo.isMe then
      contentBg.transform.localPosition = Vector3(titleText.transform.localPosition.x -titleTextW,contentBg.transform.localPosition.y,0)
    else
      contentBg.transform.localPosition = Vector3(titleText.transform.localPosition.x +titleTextW,contentBg.transform.localPosition.y,0)
    end
	end
  

	-- print(chatInfo.isMe,titleText.transform.localPosition.x,titleTextW,delta)

	local rootH = -contentText.transform.localPosition.y + h+10
	self.root.sizeDelta = Vector2(self.root.sizeDelta.x,rootH)
end

function t:Close( ... )
	-- print('content_item close')
end

Start()
return t
