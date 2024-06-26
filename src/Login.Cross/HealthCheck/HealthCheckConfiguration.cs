﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Cross.HealthCheck
{
    public class MonitoredItemsSettings
    {
        public string MonitoredType { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Url { get; set; }
    }

    public static class HealthCheckConfiguration
    {
        public static IHealthChecksBuilder AddMonitoredItems(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            var monitoredItems = configuration.GetSection("MonitoredItems").Get<List<MonitoredItemsSettings>>();

            if (monitoredItems == null || !monitoredItems.Any())
                return builder;

            foreach (var monitoredItem in monitoredItems)
                switch (monitoredItem.MonitoredType)
                {
                    case "Url":
                        builder.AddUrlGroup(new Uri(monitoredItem.Url), monitoredItem.Name);
                        break;
                    case "PostgreSQL":
                        builder.AddSqlServer(monitoredItem.ConnectionString, name: monitoredItem.Name);
                        break;
                }

            return builder;
        }
    }
}
