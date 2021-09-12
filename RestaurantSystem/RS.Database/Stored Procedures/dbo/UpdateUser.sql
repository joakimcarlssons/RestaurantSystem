CREATE PROCEDURE [dbo].[UpdateUser]
	@UserId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(50)
AS
	BEGIN TRY
		BEGIN TRAN
			UPDATE u
			SET
				FirstName		= @FirstName
				,LastName		= @LastName
				,EmailAddress	= @EmailAddress
			FROM
				dbo.Users u
			WHERE
				u.UserId = @UserId

		COMMIT TRAN

		SELECT 1
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 
		BEGIN 
			ROLLBACK TRAN 
			SELECT 0
		END
	END CATCH
