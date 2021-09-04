CREATE PROCEDURE [dbo].[GetUsers]
	@UserId int
AS
	IF (@UserId IS NULL)
	BEGIN
		SELECT
			UserId
			,FirstName
			,LastName
			,Email
		FROM
			dbo.Users
	END
	ELSE BEGIN
		SELECT
			UserId
			,FirstName
			,LastName
			,Email
		FROM
			dbo.Users u
		WHERE
			u.UserId = @UserId
	END
RETURN 0
