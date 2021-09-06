CREATE PROCEDURE [dbo].[GetRefreshToken]
	@Token NVARCHAR(MAX)
AS
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
RETURN 0
