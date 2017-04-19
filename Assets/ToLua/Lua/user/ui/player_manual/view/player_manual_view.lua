local t = {}
local PREFAB_PATH = 'ui/player_manual/player_manual_view'
local name = PREFAB_PATH

function t.Open ()
  if uimanager.GetView(name) then
    return
  end
  
  uimanager.RegisterView(name, t)
  local gameObject = Logic.UI.UIMgr.instance:Open(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace)
  t.transform = gameObject.transform
  
  t.InitComponent()
end

function t.InitComponent()
  local btn_close = t.transform:Find('core/btn_close'):GetComponent(typeof(Button))
  btn_close.onClick:AddListener(t.Close)
  
  t.category_toggles_root = t.transform:Find('core/category_toggles_root')
  
  t.toggle_hero = t.category_toggles_root:Find('toggle_hero'):GetComponent(typeof(Toggle))
  t.toggle_equipment = t.category_toggles_root:Find('toggle_equipment'):GetComponent(typeof(Toggle))
  t.toggle_material = t.category_toggles_root:Find('toggle_material'):GetComponent(typeof(Toggle))
  
  t.toggle_hero:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickCategoryToggleHandler)
  t.toggle_equipment:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickCategoryToggleHandler)
  t.toggle_material:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickCategoryToggleHandler)
  
  t.hero_functions_root = t.transform:Find('core/hero_functions_root')
  t.equipment_functions_root = t.transform:Find('core/equipment_functions_root')
  t.material_functions_root = t.transform:Find('core/material_functions_root')
  
  t.ClickCategoryToggleHandler(t.toggle_hero.gameObject)
  
  -- [[ 英雄 ]] --
  t.hero_function_toggles_root = t.transform:Find('core/hero_functions_root/hero_function_toggles_root')
  t.hero_function_toggles = {}
  for i = 1, 6 do
    t.hero_function_toggles[i] = t.hero_function_toggles_root:Find(tostring(i))
    t.hero_function_toggles[i]:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickHeroFunctionToggleHandler)
  end
  
  t.hero_function_pages_root = t.transform:Find('core/hero_functions_root/hero_function_pages_root')
  t.hero_function_pages = {}
  for i = 1, t.hero_function_pages_root.childCount do
    t.hero_function_pages_root:Find(tostring(i)).gameObject:SetActive(false)
    t.hero_function_pages[t.hero_function_toggles[i]] = t.hero_function_pages_root:Find(tostring(i)).gameObject
  end
  t.ClickHeroFunctionToggleHandler(t.hero_function_toggles[1].gameObject)
  
  -- [[ 装备 ]] --
  t.equipment_function_toggles_root = t.transform:Find('core/equipment_functions_root/equipment_function_toggles_root')
  t.equipment_function_toggles = {}
  for i = 1, 6 do
    t.equipment_function_toggles[i] = t.equipment_function_toggles_root:Find(tostring(i))
    t.equipment_function_toggles[i]:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickEquipmentFunctionToggleHandler)
  end
  
  t.equipment_function_pages_root = t.transform:Find('core/equipment_functions_root/equipment_function_pages_root')
  t.equipment_function_pages = {}
  for i = 1, t.equipment_function_pages_root.childCount do
    t.equipment_function_pages_root:Find(tostring(i)).gameObject:SetActive(false)
    t.equipment_function_pages[t.equipment_function_toggles[i]] = t.equipment_function_pages_root:Find(tostring(i)).gameObject
  end
  t.ClickEquipmentFunctionToggleHandler(t.equipment_function_toggles[1].gameObject)
  
  -- [[ 材料 ]] --
  t.material_function_toggles_root = t.transform:Find('core/material_functions_root/material_function_toggles_root')
  t.material_function_toggles = {}
  for i = 1, 6 do
    t.material_function_toggles[i] = t.material_function_toggles_root:Find(tostring(i))
    t.material_function_toggles[i]:GetComponent(typeof(Common.UI.Components.EventTriggerDelegate)).onClick:AddListener(t.ClickMaterialFunctionToggleHandler)
  end
  
  t.material_function_pages_root = t.transform:Find('core/material_functions_root/material_function_pages_root')
  t.material_function_pages = {}
  for i = 1, t.material_function_pages_root.childCount do
    t.material_function_pages_root:Find(tostring(i)).gameObject:SetActive(false)
    t.material_function_pages[t.material_function_toggles[i]] = t.material_function_pages_root:Find(tostring(i)).gameObject
  end
  t.ClickMaterialFunctionToggleHandler(t.material_function_toggles[1].gameObject)
end

function t.ClickCategoryToggleHandler (gameObject)
  local toggle = gameObject:GetComponent(typeof(Toggle))
  
  t.hero_functions_root.gameObject:SetActive(false)
  t.equipment_functions_root.gameObject:SetActive(false)
  t.material_functions_root.gameObject:SetActive(false)
  
  if toggle.isOn then
    if toggle == t.toggle_hero then
      t.hero_functions_root.gameObject:SetActive(true)
    elseif toggle == t.toggle_equipment then
      t.equipment_functions_root.gameObject:SetActive(true)
    elseif toggle == t.toggle_material then
      t.material_functions_root.gameObject:SetActive(true)
    end
    print(gameObject.name)
  end
end

function t.ClickHeroFunctionToggleHandler (gameObject)
  for k, v in pairs(t.hero_function_pages) do
    v:SetActive(k.gameObject == gameObject)
  end
end

function t.ClickEquipmentFunctionToggleHandler (gameObject)
  for k, v in pairs(t.equipment_function_pages) do
    v:SetActive(k.gameObject == gameObject)
  end
end

function t.ClickMaterialFunctionToggleHandler (gameObject)
  for k, v in pairs(t.material_function_pages) do
    v:SetActive(k.gameObject == gameObject)
  end
end

function t.Close ()
  t.transform = nil
  Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

return t