DECLARE @permissionIds TABLE (PermissionId INT, [Deny] bit, DisplayName VARCHAR(50));
DECLARE @resourceName VARCHAR(50) = N'Factories'
	,@description VARCHAR(150) = N'Access permission set for factories'
	,@isActive BIT = 1
	,@resourceId UNIQUEIDENTIFIER = NEWID()
	,@resourceIdentifier bigint
	,@parentRID bigint
	,@parentResourceName VARCHAR(50) = N'Customers';

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 1, 0, N'Add factory');-- PermissionId - Create

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 2, 1, N'Read');-- PermissionId - Read

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 3, 0, N'Edit factory');-- PermissionId - Update

INSERT INTO @permissionIds (PermissionId, [Deny], DisplayName)
VALUES (- 4, 0, N'Delete factory');-- PermissionId - Delete

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

	   SET @resourceIdentifier = SCOPE_IDENTITY()
	END
	ELSE
	BEGIN								
		SET @resourceId = (
				SELECT ResourceId
				FROM [Security].[Resource]
				WHERE NAME = @resourceName
				)
		SET @resourceIdentifier = (
				SELECT ResourceIdentifier
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

	
    SET @parentRID = (
		  SELECT ResourceIdentifier
		  FROM [Security].[Resource]
		  WHERE NAME = @parentResourceName
		  )
    
    IF ISNULL(@parentRID, -1) > 0 AND ISNULL(@resourceIdentifier, -1) > 0 
    BEGIN
	   IF Not Exists (Select Top 1 1 FROM [Security].[ResourceHierarchy] WHERE ResourceIdentifier = @resourceIdentifier AND ParentResourceIdentifier = @parentRID)
	   BEGIN
		  INSERT [Security].[ResourceHierarchy] VALUES(@resourceIdentifier, @parentRID)
	   END
    END
END
