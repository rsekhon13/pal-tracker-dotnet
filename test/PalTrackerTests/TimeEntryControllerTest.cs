using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PalTracker;
using Xunit;
using static Moq.Times;

namespace PalTrackerTests
{
    public class TimeEntryControllerTest
    {
        private readonly TimeEntryController _controller;
        private readonly Mock<ITimeEntryRepository> _repository;
        private readonly Mock<IOperationCounter<TimeEntry>> _operationCounter;

        public TimeEntryControllerTest()
        {
            _repository = new Mock<ITimeEntryRepository>();
            _operationCounter = new Mock<IOperationCounter<TimeEntry>>();
            _controller = new TimeEntryController(_repository.Object, _operationCounter.Object);
            _operationCounter.Setup(oc => oc.Increment(It.IsAny<TrackedOperation>()));
        }

        [Fact]
        public void Read()
        {
            var expected = new TimeEntry(1, 222, 333, Convert.ToDateTime("01/08/2008 12:00:01"), 24);
            _repository.Setup(r => r.Contains(1)).Returns(true);
            _repository.Setup(r => r.Find(1)).Returns(expected);

            var response = _controller.Read(1) as OkObjectResult;

            Assert.Equal(expected, response.Value);
            Assert.Equal(200, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Read), Once);
        }

        [Fact]
        public void Read_NotFound()
        {
            _repository.Setup(r => r.Contains(1)).Returns(false);

            var response = _controller.Read(1) as NotFoundResult;

            Assert.Equal(404, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Read), Once);
        }

        [Fact]
        public void Create()
        {
            var toCreate = new TimeEntry(222, 333, Convert.ToDateTime("01/08/2008 12:00:01"), 24);
            var expected = new TimeEntry(1, 222, 333, Convert.ToDateTime("01/08/2008 12:00:01"), 24);
            _repository.Setup(r => r.Create(toCreate)).Returns(expected);

            var response = _controller.Create(toCreate) as CreatedAtRouteResult;

            Assert.Equal(201, response.StatusCode);
            Assert.Equal("GetTimeEntry", response.RouteName);
            Assert.Equal(expected, response.Value);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Create), Once);
        }

        [Fact]
        public void List()
        {
            var timeEntries = new List<TimeEntry>
            {
                new TimeEntry(1, 222, 333, Convert.ToDateTime("01/08/2008 12:00:01"), 24),
                new TimeEntry(2, 999, 888, Convert.ToDateTime("05/12/2018 23:00:01"), 8)
            };

            _repository.Setup(r => r.List()).Returns(timeEntries);

            var response = _controller.List() as OkObjectResult;

            Assert.Equal(timeEntries, response.Value);
            Assert.Equal(200, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.List), Once);
        }

        [Fact]
        public void Update()
        {
            var theUpdate = new TimeEntry(999, 888, Convert.ToDateTime("05/12/2018 23:00:01"), 8);
            var updated = new TimeEntry(1, 999, 888, Convert.ToDateTime("05/12/2018 23:00:01"), 8);

            _repository.Setup(r => r.Update(1, theUpdate)).Returns(updated);
            _repository.Setup(r => r.Contains(1)).Returns(true);

            var response = _controller.Update(1, theUpdate) as OkObjectResult;

            Assert.Equal(updated, response.Value);
            Assert.Equal(200, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Update), Once);
        }

        [Fact]
        public void Update_NotFound()
        {
            var theUpdate = new TimeEntry(999, 888, Convert.ToDateTime("05/12/2018 23:00:01"), 8);

            _repository.Setup(r => r.Contains(1)).Returns(false);

            var response = _controller.Update(1, theUpdate) as NotFoundResult;

            Assert.Equal(404, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Update), Once);
        }

        [Fact]
        public void Delete()
        {
            _repository.Setup(r => r.Contains(1)).Returns(true);
            _repository.Setup(r => r.Delete(1));

            var response = _controller.Delete(1) as NoContentResult;

            Assert.Equal(204, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Delete), Once);
        }

        [Fact]
        public void Delete_NotFound()
        {
            _repository.Setup(r => r.Contains(1)).Returns(false);

            var response = _controller.Delete(1) as NotFoundResult;

            Assert.Equal(404, response.StatusCode);
            _operationCounter.Verify(oc => oc.Increment(TrackedOperation.Delete), Once);
        }
    }
}