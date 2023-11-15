CREATE TABLE [dbo].[Appointments]
(
  [AppointmentId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Receiver] INT NOT NULL,
    [Sender] INT NOT NULL,
    [AppointmentDate] DATE NOT NULL,
    [from] TIME NOT NULL,
    [to] TIME NOT NULL,
    [Purpose] NVARCHAR(255) NOT NULL,
    FOREIGN KEY (Receiver) REFERENCES [Users](UserId) ,
    FOREIGN KEY (Sender) REFERENCES [Users](UserId) 
)
