/*
* The different roles that the user can have in the system
*/

CREATE TABLE [dbo].[Roles]
(
	[RoleId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[RoleName] NVARCHAR(50) NOT NULL
)
