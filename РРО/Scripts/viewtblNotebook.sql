select n.Id, n.[Date], m.[Name] as Manufacturer, g.[Name] as Product, n.[Count], n.TotalSum, c.[Name] as Customer, c.Phone, n.Discount
from tblNotebook as n
inner join tblGroceries as g
on n.ProductId = g.Id
inner join tblManufacturers as m
on n.ManufacturerId = m.Id
inner join tblCustomers as c
on n.CustomerId = c.Id