--select n.Id, n.[Date], m.[Name] as Manufacturer, g.[Name] as Product, n.[Count], n.TotalSum, c.[Name] as Customer, c.Phone, n.Discount
--from tblNotebook as n
--inner join tblGroceries as g
--on n.ProductId = g.Id
--inner join tblManufacturers as m
--on n.ManufacturerId = m.Id
--inner join tblCustomers as c
--on n.CustomerId = c.Id

SELECT n.Id, n.[Date], m.[Name] as Manufacturer, g.[Name] as Product, n.[Count], n.TotalSum, c.[Name] as Customer, c.Phone, n.Discount
FROM tblNotebook as n
INNER JOIN tblGroceries as g
ON n.ProductId = g.Id
INNER JOIN tblManufacturers as m
ON n.ManufacturerId = m.Id
INNER JOIN tblCustomers as c
ON n.CustomerId = c.Id
ORDER BY s.Id 
OFFSET @start ROWS
FETCH NEXT @end ROWS ONLY