CREATE PROCEDURE [dbo].[GetRefreshToken]
	@Token NVARCHAR(MAX)
AS
	BEGIN TRY
		BEGIN TRAN
			SELECT
				TokenId,
				UserId,
				Token,
				JwtId,
				IsUsed,
				IsRevoked,
				AddedDate,
				ExpiryDate
			FROM
				dbo.RefreshTokens rt
			WHERE
				rt.Token = @Token
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 BEGIN ROLLBACK TRAN END
	END CATCH
