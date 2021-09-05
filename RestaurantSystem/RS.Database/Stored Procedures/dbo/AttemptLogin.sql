CREATE PROCEDURE [dbo].[AttemptLogin]
	@EmailAddress NVARCHAR(50),
	@Password NVARCHAR(MAX)
AS
	SELECT
		UserId,
		EmailAddress,
		FirstName,
		LastName
	FROM
		dbo.Users u
	WHERE
		u.EmailAddress = @EmailAddress
		AND u.[Password] = @Password
RETURN 0
