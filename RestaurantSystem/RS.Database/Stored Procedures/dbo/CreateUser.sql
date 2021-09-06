CREATE PROCEDURE [dbo].[CreateUser]
	@EmailAddress NVARCHAR(50),
	@Password NVARCHAR(MAX),
	@Salt NVARCHAR(MAX),
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50)
AS
	INSERT INTO dbo.Users
	(
		[EmailAddress],
		[Password],
		[Salt],
		[FirstName],
		[LastName]
	)
	OUTPUT
		INSERTED.UserId,
		INSERTED.EmailAddress,
		INSERTED.FirstName,
		INSERTED.LastName
	VALUES
	(
		@EmailAddress,
		@Password,
		@Salt,
		@FirstName,
		@LastName
	)
RETURN 0
