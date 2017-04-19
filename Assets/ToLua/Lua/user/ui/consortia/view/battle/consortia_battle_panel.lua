local t = {}
local PREFAB_PATH = 'ui/consortia/battle/consortia_battle_panel'

local consortia_model = gamemanager.GetModel('consortia_model')
local consortia_controller = gamemanager.GetCtrl('consortia_controller')
local consortia_info = require('ui/consortia/model/consortia_info')
local battle_road_item = require('ui/consortia/view/battle/battle_road_item')
local battle_road_info = require('ui/consortia/model/battle_road_info')

function t.Open (parent)
  local gameObject = GameObject.Instantiate(ResMgr.instance:Load(PREFAB_PATH))
  t.transform = gameObject:GetComponent(typeof(Transform))
  t.transform:SetParent(parent,false)
  
  t.enemyConsortia = consortia_info.New(100)
  t.battleRoadInfoList = {}
  for i = 1,9 do
    t.battleRoadInfoList[i] = battle_road_info.New(i)
  end
  t.battleItems = {}
  
  t.InitComponent()
  t.BindDelegate ()
  t.InitScrollContent()
  t.Refresh()
end
function t.Close()
  if t.transform then
    GameObject.Destroy(t.transform.gameObject)
    t.UnbindDelegate()
    t.transform = nil
  end
end
function t.BindDelegate ()
  
end
function t.UnbindDelegate()
  
end
function t.InitScrollContent()
  local count = #t.battleRoadInfoList
  t.scrollContent:Init(count,false,0)
end

function t.InitComponent()
  t.scrollContent = t.transform:Find('bottom/Scroll View/Viewport/Content'):GetComponent(typeof(ScrollContentExpand))
  t.scrollContent:AddResetItemListener(t.ResetItemHandler)
  
  local my_consortia = t.transform:Find('top/my_consortia')
  t.tranMyConsortia = {}
  t.tranMyConsortia.imgMark = my_consortia:Find('mark'):GetComponent(typeof(Image))
  t.tranMyConsortia.textLevel = my_consortia:Find('text_lv'):GetComponent(typeof(Text))
  t.tranMyConsortia.textName = my_consortia:Find('text_name'):GetComponent(typeof(Text))
  t.tranMyConsortia.imgVectory = my_consortia:Find('img_vectory'):GetComponent(typeof(Image))
  
  local enemy_consortia = t.transform:Find('top/enemy_consortia')
  t.tranEnemyConsortia = {}
  t.tranEnemyConsortia.root = enemy_consortia:Find('root')
  t.tranEnemyConsortia.imgMark = enemy_consortia:Find('root/mark'):GetComponent(typeof(Image))
  t.tranEnemyConsortia.textLevel = enemy_consortia:Find('root/text_lv'):GetComponent(typeof(Text))
  t.tranEnemyConsortia.textName = enemy_consortia:Find('root/text_name'):GetComponent(typeof(Text))
  t.tranEnemyConsortia.imgVectory = enemy_consortia:Find('root/img_vectory'):GetComponent(typeof(Image))
  
  t.textPoint = t.transform:Find('top/point_root/text_point'):GetComponent(typeof(Text))
  t.textMatchTime = t.transform:Find('top/text_next_time'):GetComponent(typeof(Text))
  
  
end
function t.Refresh()
  t.tranMyConsortia.imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(consortia_model.consortiaInfo.headNo)
  t.tranMyConsortia.textLevel.text = string.format('Lv%d',consortia_model.consortiaInfo.lv)
  t.tranMyConsortia.textName.text = consortia_model.consortiaInfo.name
  t.tranMyConsortia.imgVectory.gameObject:SetActive(false)
  
  if t.enemyConsortia then
    t.tranEnemyConsortia.root.gameObject:SetActive(true)
    t.tranEnemyConsortia.imgMark.sprite = ui_util.GetConsortiaMarkIconSprite(t.enemyConsortia.headNo)
    t.tranEnemyConsortia.textLevel.text = string.format(LocalizationController.instance:Get('common.role_icon.common_lv'),t.enemyConsortia.lv)
    t.tranEnemyConsortia.textName.text = t.enemyConsortia.name
    t.tranEnemyConsortia.imgVectory.gameObject:SetActive(false)
  else
    t.tranEnemyConsortia.root.gameObject:SetActive(false)
  end
end

-------------------click event----------------
function t.ResetItemHandler(go,index)
  local item = t.battleItems[go]
  if not item then
    item = battle_road_item.BindGameObject(go)
    t.battleItems[go] = item
  end
  item:SetData(t.battleRoadInfoList[index + 1])
end

return t