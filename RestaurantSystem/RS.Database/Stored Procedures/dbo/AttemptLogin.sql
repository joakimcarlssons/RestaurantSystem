CREATE PROCEDURE [dbo].[AttemptLogin]
	@EmailAddress NVARCHAR(50),
	@Password NVARCHAR(MAX)
AS
	BEGIN TRY
		BEGIN TRAN

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

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 BEGIN ROLLBACK TRAN END
	END CATCH
