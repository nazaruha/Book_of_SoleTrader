SELECT n.Id, n.[Date], m.[Name] as Manufacturer, g.[Name] as Product, n.[Count], n.TotalSum, c.[Name] as Customer, c.Phone, n.Discount
FROM tblNotebook as n
INNER JOIN tblGroceries as g
ON n.ProductId = g.Id
INNER JOIN tblManufacturers as m
ON n.ManufacturerId = m.Id
INNER JOIN tblCustomers as c
ON n.CustomerId = c.Id
ORDER BY n.ManufacturerId 
OFFSET @start ROWS
FETCH NEXT @end ROWS ONLY