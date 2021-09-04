CREATE PROCEDURE [dbo].[UpdateUser]
	@UserId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Email nvarchar(50)
AS
	UPDATE u
	SET
		FirstName	= @FirstName
		,LastName	= @LastName
		,Email		= @Email
	FROM
		dbo.Users u
	WHERE
		u.UserId = @UserId
RETURN 0
