local t = {}
local PREFAB_PATH = 'ui/hero_relationship/hero_relationship_childview'
local temp
local model
local using={}
t.list = {}

local function Start()
	t.item=dofile('ui/hero/view/relationship/hero_relationship_childitem')
	model=gamemanager.GetModel('hero_model')

end

function t.Init(pv)
	t.pv=pv
	t.item.Init(t)

end

function t.New(transform)
	t.transform=transform
	t.InitComponent()
end

function t.Open(modelId,star)
--	print('open childview ',modelId,star)
	t.hero:SetGameResData(require('ui/game/model/game_res_data').New(0,modelId,0,star),false)
	t.hero:ShowLv(false)
	
	t.list={}
	local info=nil
	local flag=false
	for k,v in pairs(model.GetAllHeroInfoList()) do
		if #v.relations>0 then
			for i=1,#v.relations do
				if flag then flag=false break end
				for j=1,#v.relations[i].friends do
					info=model.GetHeroInfo(v.relations[i].friends[j])
					if info.heroData.id == modelId and info.advanceLevel>= star then
						local hero={}
						hero.modelId=v.heroData.id
						hero.stars=v.advanceLevel
						hero.id=v.relations[i].id
						hero.instanceID=v.instanceID
						table.insert(t.list,hero)
						flag=true
						break
					end
				end
			end
		end
	end

	--print('匹配数量 ',#t.list)
	t.content:Init(#t.list,true,0)
	t.transform.gameObject:SetActive(true)
end

function t.onResetItem(go,idx)
	idx=idx+1
	--print('current:index',idx)
	temp=t.GenItem(go)
	temp:Setup(idx,t.list[idx])
end

function t.GenItem(go)
	for i=1,#using do
		if using[i].go==go then
			using[i]:Close()
			return using[i]
		end
	end
	temp=t.item.New(go)
	table.insert(using,temp)
	return temp
end

function t.HeroRelationShipReq(req,item)
	t.curItem=item
	t.pv.HeroRelationShipReq(req)
end

function t.HeroRelationShipResp()
	using={}
  if next(t.list) == nil then do return end end
  
	table.remove(t.list, t.curItem.index)
	if #t.list<1 then	t.curItem:Close() do return end	end
	t.content:Init(#t.list,true,0)
end

function t.InitComponent()
	t.content=t.transform:FindChild('core/container/scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
	t.content.onResetItem:AddListener(t.onResetItem)
	t.herotrans=t.transform:FindChild('core/heroicon')
	t.hero=require('ui/common_icon/common_hero_icon').New(t.transform:FindChild('core/heroicon'))

	t.transform:FindChild('core/text_title'):GetComponent(typeof(Text)).text=LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.text_title_child')
	t.transform:FindChild('core/btn_close'):GetComponent(typeof(Button)).onClick:AddListener(t.Close)
end

function t.Close( ... )
	using={}
  	t.pv.Refresh()
	t.transform.gameObject:SetActive(false)
end

Start()
return t