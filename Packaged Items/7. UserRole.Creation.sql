Declare @userId BIGINT,
	   @roleId BIGINT;

Set @userId = (Select Top 1 u.UserId from [Security].[User] u)
Set @roleId = (Select Top 1 u.RoleId from [Security].[Role] u)

Insert INTO [Security].[UserRole] VALUES(@userId, @roleId, GETDATE())