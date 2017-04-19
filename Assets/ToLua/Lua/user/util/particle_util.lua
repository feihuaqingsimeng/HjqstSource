local t = {}

function t.CreateParticleByCanvas(path,canvas,parentTransform)
  return t.CreateParticle(path,canvas.sortingLayerName,canvas.sortingOrder,parentTransform)
end

function t.CreateParticle(path,layerName,sortingOrder,parentTransform)
  local go = GameObject.Instantiate(Common.ResMgr.ResMgr.instance:Load(path))
  if go == nil then
    print('can not load gameobject in path:'..path)
    return nil
  end
  
  t.ChangeParticleSortingOrder(go,layerName,sortingOrder)
  if  parentTransform then
    go.transform:SetParent(parentTransform,false)
  end
  return go
end

function t.ChangeParticleSortingOrderByCanvas(go,canvas)
  t.ChangeParticleSortingOrder(go,canvas.sortingLayerName,canvas.sortingOrder)
end

function t.ChangeParticleSortingOrder(go,layerName,sortingOrder)
  local renders = go:GetComponentsInChildren(typeof(UnityEngine.Renderer),true)
  local length = renders.Length-1
  for i = 0,length do
    renders[i].sortingLayerName = layerName
    renders[i].sortingOrder = renders[i].sortingOrder + sortingOrder
  end
end

return t