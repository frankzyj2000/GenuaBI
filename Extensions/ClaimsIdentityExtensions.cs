using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace GenuinaBI.Extensions
{
    [Serializable]
    public class JavaScriptErrorException : Exception
    {
        public JavaScriptErrorException(string message)
            : base(message)
        {
        }
    }

    public static class ClaimsIdentityExtensions
    {
        public static bool IsAdmin(this ClaimsIdentity identity)
        {
            var claims = identity.Claims;
            return bool.Parse(GetClaim(claims, "IsAdmin")); //todo: replace IsAdmin with a Const
        }

        public static string GetCulture(this ClaimsIdentity identity)
        {
            var claims = identity.Claims;
            return GetClaim(claims, "Culture"); //todo: replace Culture with a Const
        }

        public static string GetName(this ClaimsIdentity identity)
        {
            var claims = identity.Claims;
            return GetClaim(claims, ClaimTypes.Name); ;
        }

        public static string GetUserId(this ClaimsIdentity identity)
        {
            var claims = identity.Claims;
            return GetClaim(claims, ClaimTypes.NameIdentifier);
        }

        private static string GetClaim(IEnumerable<Claim> claims, string key)
        {
            var claim = claims.ToList().FirstOrDefault(c => c.Type == key);
            if (claim == null)
                return null;
            return claim.Value;
        }
    }

    //A Razor Helper used to do Cache Busting for individual javascript file
    public static class StaticFile
    {
        public static string Version(string rootRelativePath)
        {
            if (HttpRuntime.Cache[rootRelativePath] == null)
            {
                var absolutePath = HostingEnvironment.MapPath(rootRelativePath);
                var lastChangedDateTime = File.GetLastWriteTime(absolutePath);

                if (rootRelativePath.StartsWith("~"))
                {
                    rootRelativePath = rootRelativePath.Substring(1);
                }

                var versionedUrl = rootRelativePath + "?v=" + lastChangedDateTime.Ticks;

                HttpRuntime.Cache.Insert(rootRelativePath, versionedUrl, new CacheDependency(absolutePath));
            }

            return HttpRuntime.Cache[rootRelativePath] as string;
        }
    }
}