using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class User
    {
        private string _userName;

        public User()
        {
        }

        public Guid Id { get; set; }

        [ProtectedPersonalData]
        [Required]
        [Display(Name = "User Name")]
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value?.Trim();
                NormalizedUserName = _userName?.ToUpper();
            }
        }

        public string NormalizedUserName { get; set; }

        [Required]
        [Display(Name = "Locked Out")]
        public bool IsLockedOut { get; set; }

        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any user has the same name
                if (await context.Users
                    .Where(u => u.Id != Id)
                    .Where(u => u.NormalizedUserName == NormalizedUserName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A user already exists with the username {UserName}."));
            });
        }
    }
}