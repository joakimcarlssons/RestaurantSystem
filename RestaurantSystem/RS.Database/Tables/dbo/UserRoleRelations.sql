/*
* Relation table for users and roles assigned to the user
*/

CREATE TABLE [dbo].[UserRoleRelations]
(
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES dbo.Users(UserId),
	[RoleId] INT NOT NULL FOREIGN KEY REFERENCES dbo.Roles(RoleId)
)
