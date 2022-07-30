select m.[Name] as Manufacturer, g.[Name] as Product, Price, [Count]
from tblStorage as s
inner join tblManufacturers as m
on s.ManufacturerId = m.Id
inner join tblGroceries as g
on s.ProductId = g.Id