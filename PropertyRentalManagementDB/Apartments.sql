CREATE TABLE [dbo].[Apartments]
(

    [ApartmentId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [BuildingId] INT NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [StatusId] INT NOT NULL,
    [Price] DECIMAL(19, 4) NOT NULL,
    [UserId] INT NOT NULL,
    FOREIGN KEY (BuildingId) REFERENCES [Buildings](buildingId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES [Users](UserId) ON DELETE CASCADE
)


