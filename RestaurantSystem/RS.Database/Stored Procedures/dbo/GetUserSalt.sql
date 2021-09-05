CREATE PROCEDURE [dbo].[GetUserSalt]
	@EmailAddress NVARCHAR(50)
AS
	SELECT	Salt
	FROM	dbo.Users
	WHERE	EmailAddress = @EmailAddress
RETURN 0
