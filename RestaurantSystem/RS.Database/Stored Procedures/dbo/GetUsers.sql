CREATE PROCEDURE [dbo].[GetUsers]
	@UserId int = NULL
AS
	BEGIN TRY
		BEGIN TRAN
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
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 BEGIN ROLLBACK TRAN END
	END CATCH
