CREATE PROCEDURE [dbo].[GetUsers]
	@UserId int = NULL
AS
	IF (@UserId IS NULL)
	BEGIN
		SELECT
			UserId
			,FirstName
			,LastName
			,EmailAddress
		FROM
			dbo.Users
	END
	ELSE BEGIN
		SELECT
			UserId
			,FirstName
			,LastName
			,EmailAddress
		FROM
			dbo.Users u
		WHERE
			u.UserId = @UserId
	END
RETURN 0
