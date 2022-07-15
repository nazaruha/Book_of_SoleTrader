IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[tblGroceries]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TABLE [dbo].[tblGroceries](
  [Id] [int] IDENTITY PRIMARY KEY NOT NULL,
  [Name] [nvarchar](100) NOT NULL UNIQUE,
);'