using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Data.Extension
{
    public static class ClaimsPrincipalEx
    {

        public static string EmailAdd(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email).Value;
        }

        private static string PermissionGroup(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Group").Value;
        }

        public static bool IsBranchUser(this ClaimsPrincipal principal, string group = "Branch User")
        {
            var mGroup = ClaimsPrincipal.Current.PermissionGroup();
            return mGroup == group;

        }
        public static bool IsAOUser(this ClaimsPrincipal principal, string group = "AO User")
        {
            var mGroup = ClaimsPrincipal.Current.PermissionGroup();
            return mGroup == group;

        }

        public static bool IsSuperUser(this ClaimsPrincipal principal, string group = "Super User")
        {
            var mGroup = ClaimsPrincipal.Current.PermissionGroup();
            return mGroup == group;

        }
        public static bool IsAdmin(this ClaimsPrincipal principal, string group = "Admin")
        {
            var mGroup = ClaimsPrincipal.Current.PermissionGroup();
            return mGroup == group;

        }

        public static List<string> GetRoles(this ClaimsPrincipal principal)
        {
            List<string> groups = new List<string>();
            groups.AddRange(principal.FindAll(aa => aa.Type == ClaimTypes.Role).Select(xx => xx.Value));
            return groups;
        }
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            //List<string> groups = new List<string>();
            return principal.FindFirst(aa => aa.Type == ClaimTypes.Email).Value;
            // return groups;
        }

    }
}
