CREATE TABLE [dbo].[Users]
(
	[UserId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [EmailAddress] NVARCHAR(50) NOT NULL, 
    [Password] NVARCHAR(MAX) NOT NULL, 
    [Salt] NVARCHAR(MAX) NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
)
