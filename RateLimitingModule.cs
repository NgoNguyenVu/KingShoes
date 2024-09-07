using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteGiayKingShoes
{
    public class RateLimitingModule : IHttpModule
    {
        private static readonly ConcurrentDictionary<string, RequestCounter> RequestCounts = new ConcurrentDictionary<string, RequestCounter>();

        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private const int MaxRequestsPerSecond = 5;
        private const int MaxRequestsPerMinute = 100;
        private const int MaxRequestsPerHour = 1000;
        private const int MaxRequestsPerDay = 10000;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var clientIp = context.Request.UserHostAddress;

            if (IsRateLimited(clientIp))
            {
                context.Response.StatusCode = 429; // Too Many Requests
                context.Response.StatusDescription = "Too Many Requests";
                context.Response.End();
            }
        }

        private bool IsRateLimited(string clientIp)
        {
            var now = DateTime.UtcNow;
            var requestCounter = RequestCounts.GetOrAdd(clientIp, new RequestCounter());

            lock (requestCounter)
            {
                // Remove expired requests
                requestCounter.RequestTimes.RemoveAll(time => time < now - OneDay);

                // Calculate request counts for each time window
                int requestsLastSecond = requestCounter.RequestTimes.Count(time => time > now - OneSecond);
                int requestsLastMinute = requestCounter.RequestTimes.Count(time => time > now - OneMinute);
                int requestsLastHour = requestCounter.RequestTimes.Count(time => time > now - OneHour);
                int requestsLastDay = requestCounter.RequestTimes.Count(time => time > now - OneDay);

                if (requestsLastSecond >= MaxRequestsPerSecond ||
                    requestsLastMinute >= MaxRequestsPerMinute ||
                    requestsLastHour >= MaxRequestsPerHour ||
                    requestsLastDay >= MaxRequestsPerDay)
                {
                    return true;
                }

                requestCounter.RequestTimes.Add(now);
                return false;
            }
        }

        public void Dispose()
        {
        }

        private class RequestCounter
        {
            public List<DateTime> RequestTimes { get; } = new List<DateTime>();
        }
    }
}