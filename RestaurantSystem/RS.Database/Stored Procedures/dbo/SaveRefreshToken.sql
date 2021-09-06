CREATE PROCEDURE [dbo].[SaveRefreshToken]
	@UserId INT,
	@Token NVARCHAR(MAX),
	@JwtId NVARCHAR(MAX),
	@IsUsed BIT,
	@IsRevoked BIT,
	@AddedDate DATETIME2(7),
	@ExpiryDate DATETIME2(7)
AS
-- Check if the token exists
-- Then just update it
	IF EXISTS
	(
		SELECT	1
		FROM	dbo.RefreshTokens rt
		WHERE	rt.JwtId = @JwtId
	)
	BEGIN
		UPDATE	token
		SET		Token = @Token,
				JwtId = @JwtId,
				IsUsed = @IsUsed,
				IsRevoked = @IsRevoked,
				AddedDate = @AddedDate,
				ExpiryDate = @ExpiryDate
		FROM	dbo.RefreshTokens token
		WHERE	token.JwtId = @JwtId
	END
-- Else create a new one
	ELSE BEGIN
		INSERT INTO dbo.RefreshTokens
		(
			UserId,
			Token,
			JwtId,
			IsUsed,
			IsRevoked,
			AddedDate,
			ExpiryDate
		)
		OUTPUT
			INSERTED.TokenId,
			INSERTED.UserId,
			INSERTED.Token,
			INSERTED.JwtId,
			INSERTED.IsUsed,
			INSERTED.IsRevoked,
			INSERTED.AddedDate,
			INSERTED.ExpiryDate
		VALUES
		(
			@UserId,
			@Token,
			@JwtId,
			@IsUsed,
			@IsRevoked,
			@AddedDate,
			@ExpiryDate
		)
	END

RETURN 0
