SELECT 
    u.Email,
    r.Name AS RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur
    ON u.Id = ur.UserId
INNER JOIN AspNetRoles r
    ON ur.RoleId = r.Id
WHERE u.Email = 'day3doctor@cardiacapi.com';