using System.Collections.Generic;
using Moq;
using PalTracker;
using Steeltoe.Management.Endpoint.Info;
using Xunit;

namespace PalTrackerTests
{
    public class TimeEntryInfoContributorTest
    {
        private readonly Mock<IInfoBuilder> _builder;
        private readonly Mock<IOperationCounter<TimeEntry>> _counter;
        private readonly TimeEntryInfoContributor _contributor;

        public TimeEntryInfoContributorTest()
        {
            _builder = new Mock<IInfoBuilder>();
            _counter = new Mock<IOperationCounter<TimeEntry>>();
            _contributor = new TimeEntryInfoContributor(_counter.Object);
        }

        [Fact]
        public void Contribute()
        {
            var counts = new Dictionary<TrackedOperation, int>
            {
                {TrackedOperation.Create, 0},
                {TrackedOperation.Read, 3},
                {TrackedOperation.List, 1},
                {TrackedOperation.Update, 7},
                {TrackedOperation.Delete, 3}
            };

            _builder.Setup(b => b.WithInfo(It.IsAny<string>(), It.IsAny<object>()));
            _counter.Setup(c => c.GetCounts()).Returns(counts);
            _counter.SetupGet(c => c.Name).Returns("TimeEntryOperations");

            _contributor.Contribute(_builder.Object);

            _builder.Verify(b => b.WithInfo("TimeEntryOperations", counts));
        }
    }
}