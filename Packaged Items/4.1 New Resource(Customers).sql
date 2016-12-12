DECLARE @permissionIds TABLE (PermissionId INT, [Deny] bit, DisplayName VARCHAR(50));
DECLARE @resourceName VARCHAR(50) = N'Customers'
	,@description VARCHAR(150) = N'Access permission set for customers'
	,@isActive BIT = 1
	,@resourceId UNIQUEIDENTIFIER = NEWID();

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 1, 0, N'Add new customer');-- PermissionId - Create

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 2, 1, N'Read');-- PermissionId - Read

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 3, 0, N'Edit customer info pop up');-- PermissionId - Update

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 4, 0, N'Delete customer');-- PermissionId - Delete

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
		,pi.DisplayName
	FROM @permissionIds pi
	LEFT JOIN [Security].ResourcePermission rp ON pi.PermissionId = rp.PermissionId
		AND rp.ResourceId = @resourceId
	WHERE rp.ResourceId IS NULL AND ISNULL(@resourceName, '') <> ''
END
