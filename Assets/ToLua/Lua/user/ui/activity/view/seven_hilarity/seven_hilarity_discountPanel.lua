local t={}
local name='seven_hilarity_page'

t.day = 1
t.type = 1
t.item1 = nil
t.item2 = nil

function t:Start(transform)
  if nil == self.transform then 
		self.transform = transform 
    
    local Obj1 = self.transform:FindChild('item1')
    self.item1 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_discount_item")
    self.item1:Start(Obj1)
    
    local Obj2 = self.transform:FindChild('item2')
    self.item2 = dofile("ui/activity/view/seven_hilarity/seven_hilarity_discount_item")
    self.item2:Start(Obj2)
  end
end

function t:Init(bday, btype)
  self.day = bbay
  self.type = btype
  self.item1:Init(bday, btype, 1)
  self.item2:Init(bday, btype, 2)
end

function t:Refresh()
  self.item1:Refresh()
  self.item2:Refresh()
end

function t:Open()
  self.transform.gameObject:SetActive(true)
end

function t:Close()
  self.transform.gameObject:SetActive(false) 
end

return t