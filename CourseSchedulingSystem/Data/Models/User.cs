using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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
    }
}