local t = {}
local PREFAB_PATH = 'ui/consortia/private_donation/private_donation_request_confirm_view'

function t.Open (heroPieceItemData)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay)
  t.transform = gameObject.transform
  
  t.InitComponent()
  t.SetData(heroPieceItemData)
end

function t.InitComponent ()
  t.titleText = t.transform:Find('core/img_frame/text_title'):GetComponent(typeof(Text))
  
  t.cancelButton = t.transform:Find('core/img_frame/btn_cancel'):GetComponent(typeof(Button))
  t.cancelButton.onClick:AddListener(t.ClickCancelButtonHandler)

  t.confirmButton = t.transform:Find('core/img_frame/btn_ok'):GetComponent(typeof(Button))
  t.confirmButton.onClick:AddListener(t.ClickConfirmButtonHandler)
end

function t.SetData (heroPieceItemData)
  local titleStringTemplate = LocalizationController.instance:Get('ui.private_donation_request_confirm_view.title')
  local heroName = LocalizationController.instance:Get(heroPieceItemData.name)
  heroName = ui_util.FormatStringWithinQualityColor(heroPieceItemData.quality, heroName)
  local count = 0
  if heroPieceItemData.quality == Quality.White then
    count = 4
  elseif heroPieceItemData.quality == Quality.Green then
    count = 4
  elseif heroPieceItemData.quality == Quality.Blue then
    count = 3
  elseif heroPieceItemData.quality == Quality.Purple then
    count = 2
  elseif heroPieceItemData.quality == Quality.Orange then
    count = 1
  elseif heroPieceItemData.quality == Quality.Red then
    count = 1
  end
  
  t.titleText.text = string.format(titleStringTemplate, heroName, count)
end

function t.Close ()
  UIMgr.instance:Close(PREFAB_PATH)
end

function t.ClickCancelButtonHandler ()
  t.Close()
end

function t.ClickConfirmButtonHandler ()
  t.Close()
end

return t