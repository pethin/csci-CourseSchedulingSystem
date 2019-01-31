using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class User
    {
        public User() { }

        /// <param name="userName">The user name.</param>
        public User(string userName) : this()
        {
            UserName = userName;
        }

        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        [PersonalData]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [ProtectedPersonalData]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public List<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString() => UserName;
    }
}
