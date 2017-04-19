local t = activity_base_view.activity_view()
local name = 'activity_betaactivity_view'

t.submitBugToggle = nil
t.writeStrategyToggle = nil
t.answerQuestionToggle = nil

function t:Start(btype)
  activity_base_view.Start(self, btype)
  self.submitBugToggle = t.transform:Find('toggle_group/toggle_submit_bug'):GetComponent(typeof(Toggle))
  self.writeStrategyToggle = t.transform:Find('toggle_group/toggle_write_strategy'):GetComponent(typeof(Toggle))
  self.answerQuestionToggle = t.transform:Find('toggle_group/toggle_answer_question'):GetComponent(typeof(Toggle))
  
  self.submitBugToggle = t.transform:Find('toggle_group/toggle_submit_bug'):GetComponent(typeof(Toggle))
  self.writeStrategyToggle = t.transform:Find('toggle_group/toggle_write_strategy'):GetComponent(typeof(Toggle))
  self.answerQuestionToggle = t.transform:Find('toggle_group/toggle_answer_question'):GetComponent(typeof(Toggle))

  self.submitBugToggle:GetComponent(typeof(ToggleContent)):Set(1, LocalizationController.instance:Get('ui.activity_view.beta_activity.submit_bug_toggle_text'))
  self.writeStrategyToggle:GetComponent(typeof(ToggleContent)):Set(2, LocalizationController.instance:Get('ui.activity_view.beta_activity.write_strategy_toggle_text'))
  self.answerQuestionToggle:GetComponent(typeof(ToggleContent)):Set(3, LocalizationController.instance:Get('ui.activity_view.beta_activity.answer_question_toggle_text'))

  self.toggles = {}
  self.toggles[1] = self.submitBugToggle
  self.toggles[2] = self.writeStrategyToggle
  self.toggles[3] = self.answerQuestionToggle
  
  for k, v in ipairs(self.toggles) do
    local toggleDelegate = v.transform:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate))
    toggleDelegate.onClick:RemoveAllListeners()
    toggleDelegate.onClick:AddListener(self.ClickToggleHandler)
  end
  
  self.activityTitleText = t.transform:Find('img_frame/text_title'):GetComponent(typeof(Text))
  self.activityPlatformText = t.transform:Find('img_frame/text_activity_platform'):GetComponent(typeof(Text))
  self.activityTimeText = t.transform:Find('img_frame/text_activity_time'):GetComponent(typeof(Text))
  
  self.scrollRect = t.transform:Find('img_frame/scroll_view'):GetComponent(typeof(ScrollRect))
  
  self.activityDesText = self.transform:Find('img_frame/scroll_view/viewport/content/text_des'):GetComponent(typeof(Text))
  self.openWebPageButton = self.transform:Find('img_frame/btn_open_web_page'):GetComponent(typeof(Button))
  self.openWebPageButton.onClick:AddListener(self.ClickOpenWebPageButton)
  
  self.ClickToggleHandler (self.submitBugToggle.gameObject)
end

function t:Open() 
  activity_base_view.Open(self)
end

function t.ClickToggleHandler (toggleGameObject)
  if toggleGameObject == t.submitBugToggle.gameObject then
    t.RefreshAsSubmitBug ()
  elseif toggleGameObject == t.writeStrategyToggle.gameObject then
    t.RefreshAsWriteStrategy ()
  elseif toggleGameObject == t.answerQuestionToggle.gameObject then
    t.RefreshAsAnswerQuestion ()
  end
end

function t.ClickOpenWebPageButton ()
  --[[
  local global_data = gamemanager.GetData('global_data')
  LuaInterface.LuaCsTransfer.OpenURL(global_data.bbs_addres)
  ]]--
  
  if t.writeStrategyToggle.isOn then
    LuaInterface.LuaCsTransfer.OpenURL('http://www.yukusoft.com/gift/front/vote/index.do')
  elseif t.answerQuestionToggle.isOn then
    LuaInterface.LuaCsTransfer.OpenURL('https://sojump.com/jq/10745920.aspx')
  end
end

function t.RefreshAsSubmitBug ()
  t.activityTitleText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.submit_bug.title')
  t.activityPlatformText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.submit_bug.platform')
  t.activityTimeText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.submit_bug.time')
  t.activityDesText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.submit_bug.des')
  t.openWebPageButton.gameObject:SetActive(false)
  t.scrollRect.normalizedPosition = Vector2(0, 1)
end

function t.RefreshAsWriteStrategy ()
  t.activityTitleText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.write_strategy.title')
  t.activityPlatformText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.write_strategy.platform')
  t.activityTimeText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.write_strategy.time')
  t.activityDesText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.write_strategy.des')
  t.openWebPageButton.gameObject:SetActive(false)
  t.scrollRect.normalizedPosition = Vector2(0, 1)
end

function t.RefreshAsAnswerQuestion ()
  t.activityTitleText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.answer_question.title')
  t.activityPlatformText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.answer_question.platform')
  t.activityTimeText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.answer_question.time')
  t.activityDesText.text = LocalizationController.instance:Get('ui.activity_view.beta_activity.answer_question.des')
  t.openWebPageButton.gameObject:SetActive(false)
  t.scrollRect.normalizedPosition = Vector2(0, 1)
end

function t:Close()
  activity_base_view.Close(self)
end

return t