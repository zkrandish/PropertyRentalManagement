CREATE TABLE [dbo].[Apartments]
(
    [ApartmentId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [BuildingId] INT NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [StatusId] INT NOT NULL,
    [Price] DECIMAL(19, 4) NOT NULL,
    [ManagerId] INT NOT NULL,
    [TenantId] INT NULL, -- Assuming a tenant might not always be assigned
    FOREIGN KEY (BuildingId) REFERENCES [Buildings](BuildingId) ON DELETE CASCADE,
    FOREIGN KEY (ManagerId) REFERENCES [Users](UserId),
    FOREIGN KEY (TenantId) REFERENCES [Users](UserId)
)


