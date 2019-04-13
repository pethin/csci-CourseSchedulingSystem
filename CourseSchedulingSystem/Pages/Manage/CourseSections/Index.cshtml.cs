﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class IndexModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public IndexModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CourseSection> CourseSection { get;set; }

        public async Task OnGetAsync()
        {
            CourseSection = await _context.CourseSections
                .Include(c => c.Course)
                .Include(c => c.InstructionalMethod)
                .Include(c => c.ScheduleType)
                .Include(c => c.TermPart).ToListAsync();
        }
    }
}