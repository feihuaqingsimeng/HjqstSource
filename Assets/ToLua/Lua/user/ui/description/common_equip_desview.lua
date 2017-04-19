local t={}
local PREFAB_PATH = "ui/description/common_equip_description_view_lua"

function t.OpenByInfo(equipmentInfo,pos,sizeDelta)
    local transform=UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.Tips,UIOpenMode.Overlay).transform
    t.textTitle=transform:FindChild('core/frame/text_title'):GetComponent(typeof(Text))
    t.textStory=transform:FindChild('core/frame/text_story'):GetComponent(typeof(Text))
    t.textUse=transform:FindChild('core/frame/bottom_root/text_use'):GetComponent(typeof(Text))
    t.attr_type=transform:FindChild('core/frame/attrtype/text_title'):GetComponent(typeof(Text))
    t.textFrom=transform:FindChild('core/frame/bottom_root/text_from'):GetComponent(typeof(Text))
    t.career_type=transform:FindChild('core/frame/careertype/text_title'):GetComponent(typeof(Text))
    t.attr_type_value=transform:FindChild('core/frame/attrtype/text_value'):GetComponent(typeof(Text))
    t.career_type_value=transform:FindChild('core/frame/careertype/text_value'):GetComponent(typeof(Text))

    t.rootPanel=transform:FindChild('core/frame')
    t.iconRoot=transform:FindChild('core/frame/icon_root')
    t.attrRoot=transform:FindChild('core/frame/attr_root')
    t.attrViewPrefab=transform:FindChild('core/frame/attr_type')
    t.storyBottomLineTran=transform:FindChild('core/frame/img_line')
    local btn=transform:FindChild('core/btn_full_close'):GetComponent(typeof(Button))
    btn.onClick:RemoveAllListeners()
    btn.onClick:AddListener(t.Close)

    t.originSizeDelta = t.rootPanel.sizeDelta
    t.defaultContentLineHeight = t.textUse.preferredHeight
    t.defaultBorderX = 10

    t.textTitle.color=ui_util.GetEquipQualityColor(equipmentInfo.data)
    t.transform = transform
    t.SetData(equipmentInfo,pos,sizeDelta)
end

function t.OpenById(id,pos,sizeDelta)
    local equip_info = require('ui/equip/model/equip_info')
    t.OpenByInfo(equip_info.New(0,id),pos,sizeDelta)
end

function t.SetData(equipInfo,worldPos,sizeDelta)
    t.equipInfo = equipInfo
    t.worldPos = worldPos
    if not sizeDelta then sizeDelta=Vector2(100,100) end
    t.sizeDelta = sizeDelta
    t.co=coroutine.start(t.RefreshCoroutine)
end

function t.RefreshCoroutine()
    t.rootPanel.gameObject:SetActive(false)
    coroutine.step()

    t.textTitle.text = LocalizationController.instance:Get(t.equipInfo.data.name)
    t.textStory.text = LocalizationController.instance:Get(t.equipInfo.data.description)

    t.textFrom.text = LuaCsTransfer.GetDropDataDes(BaseResType.Equipment,t.equipInfo.data.id)

    Common.Util.TransformUtil.ClearChildren(t.iconRoot,true)

    local icon = require('ui/common_icon/common_equip_icon').New(t.iconRoot)
    icon:SetEquipInfo(t.equipInfo)

    t.RefreshAttr()
    t.RefreshSpecialUse()

    local UseDeltaHeight =0
    if t.textUse.text and t.textUse.text ~='' then
    UseDeltaHeight=t.textUse.preferredHeight-t.defaultContentLineHeight
    else
    UseDeltaHeight=0-t.defaultContentLineHeight
    end

    local fromDeltaHeight =0
    if t.textFrom.text and t.textFrom.text ~='' then
    fromDeltaHeight=t.textFrom.preferredHeight-t.defaultContentLineHeight
    else
    fromDeltaHeight=0-t.defaultContentLineHeight
    end

    local storyDeltaHeight = t.textStory.preferredHeight-t.defaultContentLineHeight
    t.storyBottomLineTran.localPosition =t.storyBottomLineTran.localPosition+ Vector3(0,-storyDeltaHeight,0)
    t.textFrom.transform.localPosition =t.textFrom.transform.localPosition+ Vector3(0,UseDeltaHeight,0)
    t.rootPanel.sizeDelta = Vector2(t.originSizeDelta.x,t.originSizeDelta.y+UseDeltaHeight+storyDeltaHeight+fromDeltaHeight)

    local screenHalfHeight = UIMgr.instance.designResolution.y/2
    local localPosition = t.transform:InverseTransformPoint(t.worldPos)
    local x = 0
    local y = localPosition.y
    if localPosition.x>0 then
    x = localPosition.x-t.sizeDelta.x/2-t.rootPanel.sizeDelta.x/2-t.defaultBorderX
    else
    x = localPosition.x+t.sizeDelta.x/2+t.rootPanel.sizeDelta.x/2+t.defaultBorderX
    end

    if localPosition.y<t.rootPanel.sizeDelta.y/2-screenHalfHeight then
    y = t.rootPanel.sizeDelta.y/2-screenHalfHeight
    end
    if localPosition.y>screenHalfHeight-t.rootPanel.sizeDelta.y/2 then
    y = screenHalfHeight - t.rootPanel.sizeDelta.y/2
    end
    localPosition = Vector3(x,y,0)
    t.rootPanel.anchoredPosition3D = localPosition
    t.rootPanel.gameObject:SetActive(true)
end

function t.RefreshAttr()
      --show two new attrs
      local attr=t.equipInfo.data:GetFirstBaseAttr()
      t.attr_type.text=attr:GetName()..':'
      t.attr_type_value.text='+'..attr:GetValueString()

      local special_hero = t.equipInfo.data.special_hero
      if special_hero ==0 then
        t.career_type.text=LocalizationController.instance:Get('equip_tips_careerlimit')
        t.career_type_value.text=ui_util.GetHeroTypeDes(t.equipInfo.data.equipmentRoleType)
      else
        t.career_type.text=LocalizationController.instance:Get('equip_tips_herolimit')
        t.career_type_value.text=LocalizationController.instance:Get(gamemanager.GetData('hero_data').GetDataById(special_hero).name)
      end
end

function t.RefreshSpecialUse()
      -- local special_hero = t.equipInfo.data.special_hero
      -- if special_hero ==0 then
      --   t.textUse.text='type'.. ui_util.GetHeroTypeDes(t.equipInfo.data.equipmentRoleType)
      -- else
      --   t.textUse.text = 'use for' ..gamemanager.GetData('hero_data').GetDataById(special_hero).name
      -- end
      t.textUse.text=''
end

function t.Close()
    if t.co then
        coroutine.stop(t.co)
        t.co=nil
        UIMgr.instance:Close(PREFAB_PATH)
    end
end

return t