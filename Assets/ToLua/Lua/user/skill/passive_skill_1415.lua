--每次攻击，20%几率弱点攻击
function findTarget_1415()
    local random = math.random()
    local probability = 0.2 --概率
    if(random < probability)  then
        return 1
    else
        return 0
    end
end