USE [Location]
GO

create table [dbo].[Location](
    [Target]   nvarchar(50),
     [Destination]   nvarchar(50),
      [Distance]  decimal(18,0),
      [counter]  int,
	  [ID] int primary key not null)
 
GO
