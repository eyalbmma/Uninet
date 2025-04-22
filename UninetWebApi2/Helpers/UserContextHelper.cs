using System;
using System.Security.Claims;

namespace UninetWebApi2.Helpers
{
    public static class UserContextHelper
    {
        /// <summary>
        /// Extracts systemType, userId (for adminUser), or systemGuid (for externalSystem) from the current user claims.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal (User)</param>
        /// <returns>(systemType, userId, systemGuid)</returns>
        public static (string systemType, int? userId, Guid? systemGuid) GetUserContext(ClaimsPrincipal user)
        {
            var systemType = user.FindFirst("systemType")?.Value;

            if (systemType == "adminUser")
            {
                var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                    return (systemType, userId, null);
            }
            else if (systemType == "externalSystem")
            {
                var systemGuidStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(systemGuidStr, out Guid systemGuid))
                    return (systemType, null, systemGuid);
            }

            return (null, null, null);
        }
    }
}
