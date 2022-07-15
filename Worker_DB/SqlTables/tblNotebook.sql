IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[tblStorage]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TABLE [dbo].[tblStorage](
  [ManufacturerId] [int] NOT NULL,
  [ProductId] [int] NOT NULL,
  [Price] [int] NOT NULL,
  [Count] [int] NOT NULL,

  CONSTRAINT [PK_tblStorage] PRIMARY KEY CLUSTERED
  ( [ManufacturerId] ASC, [ProductId] ASC ),

  CONSTRAINT [FK_tblStorage_tblManufacturers] FOREIGN KEY([ManufacturerId])
   REFERENCES [dbo].[tblManufacturers] ([Id]),

  CONSTRAINT [FK_tblStorage_tblGroceries] FOREIGN KEY([ProductId])
   REFERENCES [dbo].[tblGroceries] ([Id]),
);'