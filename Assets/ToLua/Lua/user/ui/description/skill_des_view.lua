local t = {}
local PREFAB_PATH = 'ui/description/common_skill_description_view'



function t.Open(skillId,starLevel,starMinLevel,rootPosition,rootSizeDelta)
  local gameObject = UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.Tips,UIOpenMode.Overlay)
  t.transform = gameObject:GetComponent(typeof(Transform))
  print(skillId,starLevel,starMinLevel,rootPosition,rootSizeDelta)
  t.InitComponent()
  local skill_data = gamemanager.GetData('skill_data')
  t.starLevel = starLevel
  t.starMinLevel = starMinLevel
  t.dlevel = starLevel - starMinLevel
  t.skillData = skill_data.GetDataById(skillId)
  t.defaultContentLineHeight = t.textContent.preferredHeight
  t.defaultBorderX = 10
  t.rootWorldPos = rootPosition
  t.rootSizeDelta = rootSizeDelta
  t.originSizeDelta = t.tranPanel.sizeDelta
  
  t.tranPanel.gameObject:SetActive(false)
  t.refreshCoroutine = coroutine.start(t.RefreshCoroutine)
  
end

function t.Close()
  UIMgr.instance:Close(PREFAB_PATH)
  if t.refreshCoroutine then
    coroutine.stop(t.refreshCoroutine)
  end
end

function t.InitComponent()
  t.transform:Find('bg_core/bg'):GetComponent(typeof(Button)).onClick:AddListener(function()
      t.Close()
    end)
  t.tranPanel = t.transform:Find('core/frame')
  t.textTitle = t.tranPanel:Find('title_root/text_title'):GetComponent(typeof(Text))
  t.imgCombo = t.tranPanel:Find('title_root/img_combo'):GetComponent(typeof(Image))
  t.img_range = t.tranPanel:Find('title_root/img_range'):GetComponent(typeof(Image))
  t.textCD = t.tranPanel:Find('text_cd'):GetComponent(typeof(Text))
  t.textContent = t.tranPanel:Find('text_skill_des'):GetComponent(typeof(Text))
end

function t.RefreshCoroutine()
  coroutine.wait(0.05)
  local name = ''
  local cd = 0
  local des = ''
  local subDes = ''
  local skill_data = gamemanager.GetData('skill_data')
  --一般技能
  if t.skillData then
    name = LocalizationController.instance:Get(t.skillData.skillName)
    des = LocalizationController.instance:Get(t.skillData.skillDesc)
    local level = t.starLevel - t.starMinLevel
    cd = t.skillData.CD
    level = 0
    --print('level',t.starLevel,t.starMinLevel,level)
    
    local skillValueList = Logic.Skill.SkillUtil.GetMechanicsValueType(t.skillData.id)
    local count = skillValueList.Count
   -- print('count',count)
    if(count > 0) then
      for i = 1,count do
        local v = skillValueList:get_Item(i-1)
        local mechanicsTypeInt = v.mechanicsType:ToInt()
        --print('mechanicsTypeInt',mechanicsTypeInt)
        --print('mechanicsType',v.mechanicsType)
        local value = v.mechanicsValue1
        --print('mechanicsValue1',value)
        if v.isGrowup then
          value = skill_data.GetMechanicsValueByAdvanceLevel(value,t.dlevel)
        end
        value = tonumber(string.format('%.2f',value))
        local matchStr = nil
        if v.target == Logic.Enums.TargetType.Ally then
          matchStr = '{f'..mechanicsTypeInt..','
        else
          matchStr = '{'..mechanicsTypeInt..','
        end
        if(v.mechanicsValueType == Logic.Enums.MechanicsValueType.Time) then
          matchStr = matchStr..'t'
        elseif(v.mechanicsValueType == Logic.Enums.MechanicsValueType.Probabiblity) then
          matchStr = matchStr..'p'
        elseif(v.mechanicsValueType == Logic.Enums.MechanicsValueType.Extra) then
          matchStr = matchStr..'e'
        else
          matchStr = matchStr..'v'
        end
        if(string.match(des,matchStr..'1%%}')) then
          value = value * 100
          des = string.gsub(des,matchStr..'1%%}',tostring(math.abs(value)))       
        else
          des = string.gsub(des,matchStr..'1}',tostring(math.abs(value)))
        end
        if(v.mechanicsType == Logic.Enums.MechanicsType.DrainDamage) then          
          local value2 = v.mechanicsValue2
          value2 = tonumber(string.format('%.2f',value2))
          print('mechanicsValue2',value2)
          if v.isGrowup then
            value2 = skill_data.GetMechanicsValueByAdvanceLevel(value2,t.dlevel)
          end
          if(string.match(des,matchStr..'2%%}')) then
            value2 = value2 * 100
            des = string.gsub(des,matchStr..'2%%}',tostring(math.abs(value2)))
          else
            des = string.gsub(des,matchStr..'2}',tostring(math.abs(value2)))
          end
        end
        --des.gsub(des,'%%','%%%%')
      end
    end
  else--处理主角天赋
    
  end
  t.textContent.text = des
  t.textTitle.text = name
  local path = ''
  if t.skillData then
    path = t.skillData:DesTypeIcon()
  end
  if path == '' then
    t.imgCombo.gameObject:SetActive(false)
  else
    t.imgCombo.gameObject:SetActive(true)
    t.imgCombo.sprite = ResMgr.instance:LoadSprite(path)
  end
  local rangeIconPath = ''
  if t.skillData then
    rangeIconPath = t.skillData:RangeTypeIcon()
  end
  if rangeIconPath == '' then
    t.img_range.gameObject:SetActive(false)
  else
    t.img_range.gameObject:SetActive(true)
    t.img_range.sprite = ResMgr.instance:LoadSprite(rangeIconPath)
  end
  
  if cd == 0 then
    t.textCD.text = ''
  else
    t.textCD.text = string.format('%ds',cd)
  end
  
    
  --默认有一行高度了
  local desDeltaHeight = t.textContent.preferredHeight-t.defaultContentLineHeight
  t.tranPanel.sizeDelta = Vector2(t.originSizeDelta.x,t.originSizeDelta.y+desDeltaHeight)
  --位置 边界判断
  local designResolution = Vector2(960,640)
  local screenHalfHeight = designResolution.y/2
  local localPosition = t.transform:InverseTransformPoint(t.rootWorldPos)
  local x = 0
  local y = localPosition.y
  if localPosition.x > 0 then
    x = localPosition.x-t.rootSizeDelta.x/2-t.tranPanel.sizeDelta.x/2-t.defaultBorderX
  else
    x = localPosition.x+t.rootSizeDelta.x/2+t.tranPanel.sizeDelta.x/2+t.defaultBorderX
  end
  if localPosition.y < t.tranPanel.sizeDelta.y/2 - screenHalfHeight then
    y = t.tranPanel.sizeDelta.y/2-screenHalfHeight;
  end
  if localPosition.y > screenHalfHeight - t.tranPanel.sizeDelta.y/2 then
    y = screenHalfHeight - t.tranPanel.sizeDelta.y/2;
  end
  t.tranPanel.localPosition = Vector3(x,y,0)
  t.tranPanel.gameObject:SetActive(true)
end


--[[des = '攻击力增加{0,100}{0,2}'
    local level = 0

    --六星遍历（处理格式：攻击力增加{1,100}{2,200}持续时间{1,5}{2,6}s）变为（攻击力增加100持续时间5秒）
    for i = 0,6 do
      while true do
        local indexStr = '{'..i..','
        local start1,s_end1 = string.find(des,indexStr)
        local start2 = nil 
        local s_end2 = nil
        if start1 then
          start2,s_end2 = string.find(des,'}',s_end1)
          if i == level then
            des = string.gsub(des,string.sub(des,start1,start2),string.sub(des,s_end1+1,start2-1))
          else
            des = string.gsub(des,string.sub(des,start1,start2),'')
          end
        else
          break 
        end
      end
    end
    print(des)]]
return t