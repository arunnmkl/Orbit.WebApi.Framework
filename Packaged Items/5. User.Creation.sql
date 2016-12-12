DECLARE @SecurityId UNIQUEIDENTIFIER = NEWID()
	,@Username VARCHAR(50) = N'oauth'
	,@Password VARCHAR(50) = N'h7ctywWpXLW/oTqXNdYzrw==' -- Decrypted String --> password
	,@EmailAddress VARCHAR(50) = N'email@dreamorbit.com'
	,@Enabled BIT = 1
	,@Created DATETIME = GETDATE()
	,@userId BIGINT

IF NOT EXISTS (
		SELECT TOP 1 1
		FROM [Security].[User] u
		WHERE u.[Username] = @Username
		)
BEGIN
	INSERT INTO [Security].[User] (
		[SecurityId]
		,[Username]
		,[EmailAddress]
		,[Enabled]
		,[Created]
		)
	VALUES (
		@SecurityId
		,@Username
		,@EmailAddress
		,@Enabled
		,@Created
		)

    Set @userId = SCOPE_IDENTITY()

    INSERT INTO [Security].[UserPassword]
    (UserId,
	[Password],
	UpdatedBy
    )
		 SELECT @userId,
			   @Password,
			   @userId
		 FROM [Security].[User] U;

    Select @userId UserId
END
GO


