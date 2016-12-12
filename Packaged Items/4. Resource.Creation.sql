DECLARE @permissionIds TABLE (PermissionId INT, [Deny] bit);
DECLARE @resourceName VARCHAR(50) = N'RefreshToken'
	,@description VARCHAR(150) = N'Refresh Token'
	,@isActive BIT = 1
	,@resourceId UNIQUEIDENTIFIER = NEWID();

INSERT INTO @permissionIds (PermissionId, [Deny])
VALUES (- 1, 1);-- PermissionId - Create

INSERT INTO @permissionIds (PermissionId, [Deny])
VALUES (- 2, 0);-- PermissionId - Read

INSERT INTO @permissionIds (PermissionId, [Deny])
VALUES (- 3, 1);-- PermissionId - Update

INSERT INTO @permissionIds (PermissionId, [Deny])
VALUES (- 4, 1);-- PermissionId - Delete

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
		)
	SELECT @resourceId
		,pi.PermissionId
		,pi.[Deny]
		,GETDATE()
	FROM @permissionIds pi
	LEFT JOIN [Security].ResourcePermission rp ON pi.PermissionId = rp.PermissionId
		AND rp.ResourceId = @resourceId
	WHERE rp.ResourceId IS NULL AND ISNULL(@resourceName, '') <> ''
END
