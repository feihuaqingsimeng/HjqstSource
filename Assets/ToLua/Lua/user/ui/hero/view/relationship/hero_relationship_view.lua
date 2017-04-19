local t = {}
local PREFAB_PATH = 'ui/hero_relationship/hero_relationship_view'

local hero_relationship_item = dofile('ui/hero/view/relationship/hero_relationship_item')
local hero_relationship_childview = dofile('ui/hero/view/relationship/hero_relationship_childview')
local hero_relationship_heroitem = dofile('ui/hero/view/relationship/hero_relationship_heroitem')

local temp
local hero_relationship_data = gamemanager.GetData('hero_relationship_data')
local using={}
local hero_model = gamemanager.GetModel('hero_model')

local herosNotuse={}

local function Start()
	hero_relationship_item.Init(t)
	hero_relationship_childview.Init(t)
	hero_relationship_heroitem.Init(t)
	-- for k,v in pairs(model.datas) do
	-- 	print(k,#v)
	-- end
end

function t.Open(info)
	--print('open relation ship ! ',info.instanceID,info.heroData.id)
	t.info=info

	t.DealInfoList()

	t.transform = Logic.UI.UIMgr.instance:Open(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay).transform
	t.BindDelegate()
	t.InitComponent()
	t.list=hero_relationship_data.GetDataBydataid(info.heroData.id)
	--print('队伍数量 ',#t.list)
  local count = 0
	if not t.list or #t.list<1 then
    count = 0
	else
    count = #t.list
	end
  t.content:Init(count,true,0)
  t.transform:Find('core/container/text_noship_tip').gameObject:SetActive(count == 0)
end

function t.DealInfoList( ... )	
	t.relations={}
	local list={}
	for k,v in pairs(hero_model.GetAllHeroInfoList()) do
		--print('all heros:',k,v.modelId)
		table.insert(list,v)
	end

	local instanceID=t.info.instanceID
	for k,v in pairs(list) do
		--if instanceID == v.instanceID then
			--print('patch current select hero')
			for i=1,#v.relations do
				local rel={}
				rel.id=v.relations[i].id
				rel.friends=v.relations[i].friends
				rel.instanceID=v.instanceID
				table.insert(t.relations,rel)
			end
		--end
	end

	t.actived_heros={}
	for i=1,#t.relations do
		for j=1,#t.relations[i].friends do
			for k,v in pairs(list) do
				if v.instanceID == t.relations[i].friends[j] then
					list[k]=nil
					table.insert(t.actived_heros,v)
					break
				end
			end
		end
	end
	t.originList=list

	t.ids_relations={}
	t.stars_relations={}
	--print('激活数量',#t.relations)
	for i=1,#t.relations do
		--local sb=' 关系id:'..t.relations[i].id ..', 主英雄instanceID: '..t.relations[i].instanceID..' 单个英雄instanceID: '
		for j=1,#t.relations[i].friends do
			--sb=' '..sb..t.relations[i].friends[j] ..','
			instanceID = hero_model.GetHeroInfo(t.relations[i].friends[j])
			if not instanceID then print('error:instanceID not exist in bag:',t.relations[i].friends[j]) end
			table.insert(t.ids_relations,instanceID.heroData.id)
			table.insert(t.stars_relations,instanceID.advanceLevel)
		end
		--print('已激活',i,sb)
	end

	t.ids={}
	t.stars={}

	for k,v in pairs(list) do
		table.insert(t.ids,v.heroData.id)
		table.insert(t.stars,v.advanceLevel)
	end
end

function t.onResetItem(go,idx)
	idx=idx+1
	-- print('current:index',idx)
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
	temp=hero_relationship_item.New(go)
	table.insert(using,temp)
	return temp
end

function t.GenHeroitem()
	if #herosNotuse>0 then
		local it=herosNotuse[1]
		table.remove(herosNotuse,1)
		return it
	else
		return hero_relationship_heroitem.New(t.heroitemtrans)
	end
end

function t.RecycleHeroitem(item)
	item:Close()
	table.insert(herosNotuse,item)
end

function t.BindDelegate()
  gamemanager.GetModel('hero_model').OnHeroRelationShipRespDelegate:AddListener(t.HeroRelationShipResp)
end
function t.UnbindDelegate()
  gamemanager.GetModel('hero_model').OnHeroRelationShipRespDelegate:RemoveListener(t.HeroRelationShipResp)
end

function t.HeroRelationShipReq(req)
	t.curRelations={}
	t.curRelations.id=req.id
	t.curRelations.friends={}
	t.curRelations.activeFlag=req.activeFlag
	for i=1,#req.friends do
		table.insert(t.curRelations.friends,req.friends[i])
	end
	-- print('关系id',req.id)
	--print('请求激活:主英雄id',req.heroId)
	-- print('激活',req.activeFlag)
	-- print('伙伴英雄id集合')
	-- for i=1,#req.friends do
	-- 	print(req.friends[i])
	-- end

	gamemanager.GetCtrl('hero_controller').HeroRelationShipReq(req)
end

function t.HeroRelationShipResp()
	if t.curRelations.activeFlag == 1 then
		uimanager.ShowTipsOnly(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.activateok'))
		t.Refresh()
	elseif t.curRelations.activeFlag == 0 then
    uimanager.ShowTipsOnly(LuaCsTransfer.LocalizationGet('ui.hero_relationship_view.free'))    
		hero_relationship_childview.HeroRelationShipResp()
    t.Refresh()
	end
end

function t.Refresh( ... )
	t.DealInfoList()
	local item=nil
	for i=1,#using do
		item=using[i]
		item:Close()
		item:Setup(item.index,t.list[item.index])
	end
end

function t.ClickHero(item)
	if item.normal then
		--print('show tips')
	elseif item.notexist then
		--print('show source')
		-- print(item.data[1],item.data[2],item.data[3],item.data[4])
		LuaCsTransfer.OpenGoodsJumpPath(2,item.data[1],item.data[2])
	elseif item.using then
		hero_relationship_childview.Open(item.data[1],item.data[2])
	end
end

function t.InitComponent()
  	t.content=t.transform:FindChild('core/container/scrollview/viewport/content'):GetComponent(typeof(Common.UI.Components.ScrollContent))
	t.content.onResetItem:AddListener(t.onResetItem)
	t.heroitemtrans=t.transform:FindChild('core/container/scrollview/viewport/item')
	
	hero_relationship_childview.New(t.transform:FindChild('core/child'))
	t.transform:FindChild('core/btn_close'):GetComponent(typeof(Button)).onClick:AddListener(t.Close)
  t.transform:FindChild('core/text_title').gameObject:SetActive(true)
  t.transform:FindChild('core/text_title2').gameObject:SetActive(false)
	t.transform.gameObject:SetActive(true)
end

function t.Close( ... )
	using={}
  	herosNotuse={}
  	t.UnbindDelegate()
	Logic.UI.UIMgr.instance:Close(PREFAB_PATH)
end

Start()
return t