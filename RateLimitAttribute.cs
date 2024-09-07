using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteGiayKingShoes
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        // Define the number of requests allowed and the timespan
        private const int RequestLimit = 100;
        private const double DurationInSeconds = 60;

        // Use a static Dictionary to store the IP addresses and their request counts
        private static Dictionary<string, Tuple<DateTime, int>> IpAddresses = new Dictionary<string, Tuple<DateTime, int>>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ipAddress = filterContext.HttpContext.Request.UserHostAddress;

            if (!IpAddresses.ContainsKey(ipAddress))
            {
                IpAddresses[ipAddress] = new Tuple<DateTime, int>(DateTime.Now, 1);
            }
            else
            {
                var entry = IpAddresses[ipAddress];
                var requestCount = entry.Item2;

                if (entry.Item1.AddSeconds(DurationInSeconds) >= DateTime.Now)
                {
                    if (requestCount < RequestLimit)
                    {
                        IpAddresses[ipAddress] = new Tuple<DateTime, int>(entry.Item1, requestCount + 1);
                    }
                    else
                    {
                        filterContext.Result = new ContentResult
                        {
                            Content = "Too many requests. Please try again later."
                        };
                    }
                }
                else
                {
                    IpAddresses[ipAddress] = new Tuple<DateTime, int>(DateTime.Now, 1);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}