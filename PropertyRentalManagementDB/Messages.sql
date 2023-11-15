CREATE TABLE [dbo].[Messages]
(
	[MessageId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Receiver] INT NOT NULL,
    [Sender] INT NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [SendDate] DATETIME NOT NULL,
    FOREIGN KEY (Receiver) REFERENCES [Users](UserId) ,
    FOREIGN KEY (Sender) REFERENCES [Users](UserId) 
)
