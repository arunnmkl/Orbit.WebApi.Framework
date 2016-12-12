DECLARE @permissionIds TABLE (PermissionId INT, [Deny] bit);
DECLARE @resourceName VARCHAR(50) = N'Impersonate'
	,@description VARCHAR(150) = N'User Impersonation'
	,@isActive BIT = 1
	,@resourceId UNIQUEIDENTIFIER = NEWID();

INSERT INTO @permissionIds (PermissionId, [Deny])
VALUES (- 5, 0);-- PermissionId - Impersonate

BEGIN
	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [Security].[Resource]
			WHERE NAME = @resourceName
			)
		AND ISNULL(@resourceName, '') <> ''
	BEGIN
		INSERT INTO [Security].Resource (
			ResourceId
			,NAME
			,Description
			,IsActive
			)
		VALUES (
			@resourceId
			,@resourceName
			,@description
			,@isActive
			);
	END
	ELSE
	BEGIN
		SET @resourceId = (
				SELECT ResourceId
				FROM [Security].[Resource]
				WHERE NAME = @resourceName
				)
	END

	INSERT INTO [Security].ResourcePermission (
		ResourceId
		,PermissionId
		,[Deny]
		,Created
		,DisplayName
		)
	SELECT @resourceId
		,pi.PermissionId
		,pi.[Deny]
		,GETDATE()
		,@resourceName
	FROM @permissionIds pi
	LEFT JOIN [Security].ResourcePermission rp ON pi.PermissionId = rp.PermissionId
		AND rp.ResourceId = @resourceId
	WHERE rp.ResourceId IS NULL AND ISNULL(@resourceName, '') <> ''
END
