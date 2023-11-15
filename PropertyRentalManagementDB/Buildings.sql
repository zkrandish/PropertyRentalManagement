CREATE TABLE [dbo].[Buildings]
(
	[BuildingId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Province] NVARCHAR(50) NOT NULL, 
    [City] NVARCHAR(50) NOT NULL, 
    [PostalCode] NVARCHAR(50) NOT NULL, 
    [UserId] INT FOREIGN KEY REFERENCES [Users](UserId) NOT NULL

)
