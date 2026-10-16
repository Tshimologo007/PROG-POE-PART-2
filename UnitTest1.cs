using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using RaceCompetitionAPI.Controllers;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Tests
{
    public class EventsControllerTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Event CreateSampleEvent()
        {
            return new Event
            {
                event_Name = "City Fun Run",
                organiserID = 1,
                townID = 1,
                description = "A 5 km fun run",
                event_Date = new DateTime(2027, 5, 10),
                event_Type = "Running",
                venue = "City Stadium",
                race_Distance = "5 km"
            };
        }

        [Fact]
        public async Task GetEvents_ReturnsAllEvents()
        {
            using var context = CreateContext();
            context.Events.Add(CreateSampleEvent());
            context.Events.Add(new Event
            {
                event_Name = "Marathon",
                organiserID = 1,
                townID = 1,
                description = "A marathon event",
                event_Date = new DateTime(2027, 6, 10),
                event_Type = "Running",
                venue = "Main Stadium",
                race_Distance = "42 km"
            });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            var result = await controller.GetEvents();

            Assert.Equal(2, result.Value!.Count());
        }

        [Fact]
        public async Task GetEventbyID_ExistingEvent_ReturnsEvent()
        {
            using var context = CreateContext();
            var sampleEvent = CreateSampleEvent();
            context.Events.Add(sampleEvent);
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            var result = await controller.GetEventbyID(sampleEvent.eventID);

            Assert.NotNull(result.Value);
            Assert.Equal("City Fun Run", result.Value!.event_Name);
        }

        [Fact]
        public async Task GetEventbyID_NonExistingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);

            var result = await controller.GetEventbyID(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateEvent_ValidEvent_ReturnsCreated()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);
            var newEvent = CreateSampleEvent();

            var result = await controller.CreateEvent(newEvent);

            Assert.IsType<CreatedAtActionResult>(result);

            var savedEvent = await context.Events.FirstOrDefaultAsync();
            Assert.NotNull(savedEvent);
            Assert.Equal("City Fun Run", savedEvent!.event_Name);
        }

        [Fact]
        public async Task UpdateEvent_NonExistingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);

            var result = await controller.UpdateEvent(999, CreateSampleEvent());

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        [Fact]
        public async Task DeleteEvent_ExistingEvent_ReturnsNoContent()
        {
            using var context = CreateContext();
            var sampleEvent = CreateSampleEvent();
            context.Events.Add(sampleEvent);
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            var result = await controller.DeleteEvent(sampleEvent.eventID);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(await context.Events.ToListAsync());
        }

        [Fact]
        public async Task DeleteEvent_NonExistingEvent_ReturnsNotFound()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);

            var result = await controller.DeleteEvent(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
