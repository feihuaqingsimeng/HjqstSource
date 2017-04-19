--免疫浮空
function addBuff_335(character,buffType)
    if(buffType == Logic.Enums.BuffType.Float)  then
      return 0
    else
      return 1
    end
end


--为了播特效
function addHaloBuff_335(character)
	local mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(3355)
    local lastTime = 6000  --持续时间
    character:AddBuff(character,character,null,mechanics,Logic.Enums.BuffType.Tumble,Logic.Enums.SkillLevelBuffAddType.None,Logic.Enums.BuffAddType.None,lastTime,0,1,1,false,true)
end

function InitCharacterDatas_33(instanceID)
  local lastTime = 999
  fightdataTable['foreverBuffIcon'..instanceID] = {'skill/29',lastTime}
end