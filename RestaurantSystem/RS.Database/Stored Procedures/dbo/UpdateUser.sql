CREATE PROCEDURE [dbo].[UpdateUser]
	@UserId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(50)
AS
	UPDATE u
	SET
		FirstName		= @FirstName
		,LastName		= @LastName
		,EmailAddress	= @EmailAddress
	FROM
		dbo.Users u
	WHERE
		u.UserId = @UserId
RETURN 0
