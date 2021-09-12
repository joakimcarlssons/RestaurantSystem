CREATE PROCEDURE [dbo].[GetUserRoles]
	@UserId INT
AS
	BEGIN TRY
		BEGIN TRAN

			-- Get all roles associated with a certain user
			SELECT
				R.RoleId,
				R.RoleName
			FROM
				dbo.Roles R
				INNER JOIN dbo.UserRoleRelations URR
				ON URR.RoleId = R.RoleId AND URR.UserId = @UserId

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 BEGIN ROLLBACK TRAN END
	END CATCH
