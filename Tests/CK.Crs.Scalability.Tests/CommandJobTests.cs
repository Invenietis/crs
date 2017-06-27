using CK.Core;
using System;
using Xunit;

namespace CK.Crs.Scalability.Tests
{
    public class CommandJobTests
    {
        [Fact]
        public void CommandJobMetadataFromToXml()
        {
            CommandJobMetada jobMeta = new CommandJobMetada(new ActivityMonitor(), new CommandAction(Guid.NewGuid())
            {
                CallbackId = "callback",
                Command = new object(),
                Description = new CommandDescription(typeof(object))
            });
            Assert.NotNull(jobMeta.MonitorToken);

            var xelement = jobMeta.ToXml();
            Assert.True(xelement.HasElements);
            Assert.Equal(xelement.Name, "Metadata");

            Assert.Equal(xelement.Element("CallbackId").Value, "callback");
            Assert.NotNull(xelement.Element("MonitorToken").Value);

            var jobMeta2 = new CommandJobMetada(xelement);
            Assert.Equal(jobMeta.CallbackId, jobMeta2.CallbackId);
            Assert.Equal(jobMeta.CommandType, jobMeta2.CommandType);
            Assert.Equal(jobMeta.ContentType, jobMeta2.ContentType);
            Assert.NotNull( jobMeta2.MonitorToken );
        }
    }
}
