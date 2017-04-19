local t = {}
t.data = {}
local item = {}
item.__index = item
t.rangeTypeIcons = {}

function item.New(table)
	local o = {}
	setmetatable(o,item)
	o.id = tonumber(table.SkillId)
	o.skillName = table.SkillName
	o.skillDesc = table.SkillDesc
	o.skillIcon = table.SkillIcon
	o.skillType = table.SkillType
	o.desTypeIcon = table.DesTypeIcon
  o.desTypeIcon2 = table.DesTypeIcon2
	o.showTime = table.ShowTime
	o.CD = tonumber(table.SkillCd)
	o.effectIds = table.EffectIds
	o.AoeFlyEffect = table.AoeFlyEffect
	o.AoeEffect = table.AoeEffect
	o.FlyEffectIds = table.FlyEffectIds
  if table.Timeline == '' or table.Timeline == nil then
    o.timeline = {}
  else
    o.timeline = string.split(table.Timeline,';')
  end
	
	o.MechanicsValue = table.MechanicsValue
	o.BootTime = table.BootTime
	o.Attackable = table.Attackable
	o.AudioId = table.AudioId
	o.AudioDelay = table.AudioDelay
	o.PauseTime = table.PauseTime
	return o
end

function item:SkillIcon()
  return 'sprite/skill/'..self.skillIcon
end

function item:DesTypeIcon()
  if  not self.desTypeIcon or self.desTypeIcon == '' then
    return ''
  end
  return 'sprite/main_ui/'..self.desTypeIcon
end
function item:DesTypeIcon2()
  if  not self.desTypeIcon2 or self.desTypeIcon2 == '' then
    return ''
  end
  return 'sprite/main_ui/'..self.desTypeIcon2
end
function item:RangeTypeIcon()
  if #self.timeline > 0 then
    local mechanics_data = gamemanager.GetData('mechanics_data')
    local m_id = string.split(self.timeline[1],':')
    local m = mechanics_data.GetDataById(tonumber(m_id[2]))
    if m then
      return 'sprite/skill/'..t.GetRangeTypeIcon(m.RangeType)
    end
  end
  return ''
end

function t.GetDataById(id)
	return t.data[id]
end

local function Start()
	local origin = dofile('skill')
	origin.ForEach(function(id,table)
		local newItem = item.New(table)
		t.data[id] = newItem
	end)
  t.InitRangeTypeIcons()
end

function t.InitRangeTypeIcons()
  t.rangeTypeIcons[1] = ''
  t.rangeTypeIcons[2] = 'r_2'
  t.rangeTypeIcons[3] = 'r_3'
  t.rangeTypeIcons[4] = 'r_4'
  t.rangeTypeIcons[5] = 'r_5'
  t.rangeTypeIcons[6] = 'r_6'
  t.rangeTypeIcons[7] = 'r_7'
  t.rangeTypeIcons[8] = 'r_8'
  t.rangeTypeIcons[9] = 'r_9'
  t.rangeTypeIcons[10] = 'r_10'
  t.rangeTypeIcons[11] = 'r_11'
  t.rangeTypeIcons[12] = 'r_12'
  t.rangeTypeIcons[13] = 'r_13'
  t.rangeTypeIcons[14] = 'r_14'
  t.rangeTypeIcons[15] = 'r_15'
  t.rangeTypeIcons[16] = 'r_16'
  t.rangeTypeIcons[17] = 'r_17'
  t.rangeTypeIcons[18] = 'r_18'
  t.rangeTypeIcons[19] = 'r_19'
  t.rangeTypeIcons[20] = 'r'
  t.rangeTypeIcons[21] = 'r_21'
  t.rangeTypeIcons[22] = 'r_22'
  t.rangeTypeIcons[23] = 'r_4'
  t.rangeTypeIcons[24] = ''
  t.rangeTypeIcons[25] = ''
  t.rangeTypeIcons[26] = ''
  t.rangeTypeIcons[27] = 'r_27'
  t.rangeTypeIcons[28] = 'r_28'
end

function t.GetRangeTypeIcon(rangeType)
  return t.rangeTypeIcons[rangeType]
end

function t.GetMechanicsValueByAdvanceLevel(value,dlevel)
  local result = 0
  local const_data = gamemanager.GetData('const_data')
  result = value * (1 + dlevel * const_data.skill_hurt_a)
  return result
end

Start()
return t