local t = {}
t.__index=t

local textMaster
local textNotMaster

local function Start()
  textMaster=LocalizationController.instance:Get('ui.consortia_view.chat.master')
  textNotMaster=LocalizationController.instance:Get('ui.consortia_view.chat.notmaster')
end

function t.New(go)
    local r={}
    setmetatable(r,t)
    r.go=go
    r:InitComponent()
    return r
end

function t:InitComponent()
  local transform=self.go.transform
  self.root=transform:GetComponent(typeof(RectTransform))
  self.contentImgBgDefaultHeight = 20
	self.leftRoot=transform:FindChild('left').gameObject
  self.rightRoot=transform:FindChild('right').gameObject
  self.rightTitleText=transform:FindChild('right/text_title'):GetComponent(typeof(Text))
  self.rightContentBg=transform:FindChild('right/img_bg'):GetComponent(typeof(RectTransform))
  self.rightContentText=transform:FindChild('right/img_bg/text_content'):GetComponent(typeof(Text))
	
	self.leftTitleText=transform:FindChild('left/text_title'):GetComponent(typeof(Text))
  self.leftContentBg=transform:FindChild('left/img_bg'):GetComponent(typeof(RectTransform))
  self.leftContentText=transform:FindChild('left/img_bg/text_content'):GetComponent(typeof(Text))
end

function t:Setup(chatInfo)
  local titleText
	local contentText
	local contentBg

	if chatInfo.isMe then
		titleText = self.rightTitleText
		contentBg = self.rightContentBg
		contentText = self.rightContentText
	else
		titleText = self.leftTitleText
		contentBg = self.leftContentBg
		contentText = self.leftContentText

		-- titleText.text = string.format("%s",chatInfo.talkerName)
	end

  titleText.text = string.format("%s [%s]",chatInfo.talkerName,chatInfo.time)
	contentText.text = chatInfo.content

	self.leftRoot:SetActive(not chatInfo.isMe)
	self.rightRoot:SetActive(chatInfo.isMe)

	local h = contentText.preferredHeight
	local w = contentText.preferredWidth
	local titleTextW = titleText.preferredWidth
	
	local delta = contentText.rectTransform.sizeDelta.x-w
	if delta < 0 then
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

	local rootH = -contentText.transform.localPosition.y + h+10
	self.root.sizeDelta = Vector2(self.root.sizeDelta.x,rootH)
end


function t:Close( ... )
	-- print('content_item close')
end

Start()
return t
