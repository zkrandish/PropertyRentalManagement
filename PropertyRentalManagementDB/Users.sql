CREATE TABLE [dbo].[Users]
(
	[UserId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [Password] VARBINARY(256) NOT NULL, 
    [StatusId] INT NOT NULL,
    [UserTypeId] INT NOT NULL,
    
    FOREIGN KEY (StatusId) REFERENCES Statuses (StatusId),
    FOREIGN KEY (UserTypeId) REFERENCES UserTypes(UserTypeId)
)
