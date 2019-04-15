using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class Calendar : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Calendar(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public Term Term { get; set; }

        public async Task<IActionResult> OnGet(Guid? termId)
        {
            if (termId == null) return NotFound();
            
            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnGetResourcesAsync(Guid? termId)
        {
            await Task.Yield();
            
            if (termId == null) return NotFound();

            var rooms = _context.Rooms
                .Include(r => r.Building)
                .Include(r => r.ScheduledMeetingTimeRooms)
                .ThenInclude(smtr => smtr.ScheduledMeetingTime)
                .ThenInclude(smt => smt.CourseSection)
                .ThenInclude(cs => cs.TermPart)
                .Where(r => r.ScheduledMeetingTimeRooms.Any(smtr =>
                    smtr.ScheduledMeetingTime.CourseSection.TermPart.TermId == termId))
                .Select(room => new Resource
                {
                    Id = room.Id,
                    Building = room.Building.Name,
                    Title = room.Number
                });

            return new JsonResult(rooms);
        }

        public async Task<IActionResult> OnGetEvents(Guid? termId, DateTime? start, DateTime? end)
        {
            if (termId == null || start == null || end == null) return NotFound();

            var eventsByWeek = await _context.ScheduledMeetingTimeRooms
                .Include(smtr => smtr.ScheduledMeetingTime)
                .ThenInclude(smt => smt.CourseSection)
                .ThenInclude(cs => cs.TermPart)
                .Include(smtr => smtr.ScheduledMeetingTime)
                .ThenInclude(smt => smt.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .Where(smtr => smtr.ScheduledMeetingTime.CourseSection.TermPart.TermId == termId)
                .Where(smtr => smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate != null)
                .Where(smtr => smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate != null)
                .Where(smtr => smtr.ScheduledMeetingTime.StartTime != null)
                .Where(smtr => smtr.ScheduledMeetingTime.EndTime != null)
                .Where(smtr =>
                    (smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate < start &&
                     smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate > start) ||
                    (smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate >= start &&
                     smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate <= end) ||
                    (smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate < end &&
                     smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate > end) ||
                    (smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate <= start &&
                     smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate >= end))
                .Select(smtr => new EventWeekly
                {
                    ResourceId = smtr.RoomId,
                    Title = smtr.ScheduledMeetingTime.CourseSection.Course.Subject.Code +
                            smtr.ScheduledMeetingTime.CourseSection.Course.Number + 
                            " " +
                            smtr.ScheduledMeetingTime.CourseSection.Section.ToString("D3"),
                    StartDate = smtr.ScheduledMeetingTime.CourseSection.TermPart.StartDate.Value,
                    EndDate = smtr.ScheduledMeetingTime.CourseSection.TermPart.EndDate.Value,
                    StartTime = smtr.ScheduledMeetingTime.StartTime.Value,
                    EndTime = smtr.ScheduledMeetingTime.EndTime.Value,
                    Monday = smtr.ScheduledMeetingTime.Monday,
                    Tuesday = smtr.ScheduledMeetingTime.Tuesday,
                    Wednesday = smtr.ScheduledMeetingTime.Wednesday,
                    Thursday = smtr.ScheduledMeetingTime.Thursday,
                    Friday = smtr.ScheduledMeetingTime.Friday,
                    Saturday = smtr.ScheduledMeetingTime.Saturday,
                    Sunday = smtr.ScheduledMeetingTime.Sunday,
                })
                .ToListAsync();

            var events = new ConcurrentBag<Event>();

            await eventsByWeek.ParallelForEachAsync(async weekly =>
            {
                await Task.Yield();

                var startDate = weekly.StartDate < start ? start.Value : weekly.StartDate;
                var endDate = weekly.EndDate > end ? end.Value : weekly.EndDate;
                
                for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
                {
                    DateTime? startDateTime = null, endDateTime = null;
                    
                    if (date.DayOfWeek == DayOfWeek.Monday && weekly.Monday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Tuesday && weekly.Tuesday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Wednesday && weekly.Wednesday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Thursday && weekly.Thursday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Friday && weekly.Friday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Saturday && weekly.Saturday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Sunday && weekly.Sunday)
                    {
                        startDateTime = date.Date + weekly.StartTime;
                        endDateTime = date.Date + weekly.EndTime;
                    }

                    if (startDateTime != null && endDateTime != null)
                    {
                        events.Add(new Event
                        {
                            ResourceId = weekly.ResourceId,
                            Title = weekly.Title,
                            Start = startDateTime.Value,
                            End = endDateTime.Value
                        });
                    }
                }
            });

            return new JsonResult(events);
        }

        public class Resource
        {
            public Guid Id { get; set; }
            public string Building { get; set; }
            public string Title { get; set; }
        }

        public class Event
        {
            public Guid ResourceId { get; set; }
            public string Title { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
        
        public class EventWeekly
        {
            public Guid ResourceId { get; set; }
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public bool Saturday { get; set; }
            public bool Sunday { get; set; }
        }
    }
}