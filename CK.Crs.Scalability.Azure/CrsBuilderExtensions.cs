using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability.Azure
{
    public static class CrsBuilderExtensions
    {
        public static ICrsBuilder UseAzure(this ICrsBuilder builder, string connectionString, string queuePrefixName)
        {
            builder.UseAzureInput(new AzureQueueConfiguration
            {
                ConnectionString = connectionString,
                QueueName = queuePrefixName + "-input"
            });
            builder.UseAzureOutput(new AzureQueueConfiguration
            {
                ConnectionString = connectionString,
                QueueName = queuePrefixName + "-output"
            });
            return builder;
        }

        public static ICrsBuilder UseAzureInput(this ICrsBuilder builder, AzureQueueConfiguration conf)
        {
            builder.Input.AddListener(new AzureQueueListener( conf ));
            return builder;
        }
        public static ICrsBuilder UseAzureOutput(this ICrsBuilder builder, AzureQueueConfiguration conf)
        {
            builder.Output.AddDispatcher(new AzureQueueDispatcher( conf ));
            return builder;
        }
    }

}
