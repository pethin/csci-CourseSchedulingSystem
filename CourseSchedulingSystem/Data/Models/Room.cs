using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a room, e.g., THUR 100.</summary>
    /// <remarks>
    /// <para>Rooms must have a unique ID, unique (Building, Number).</para>
    /// <para>A room belongs to a building.</para>
    /// <para>A room has many scheduled meeting times.</para>
    /// </remarks>
    public class Room
    {
        private string _number;

        /// <summary>Creates a room with default fields.</summary>
        public Room()
        {
        }

        /// <summary>Gets or sets the primary key for this building.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the building ID for this room.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        public Guid BuildingId { get; set; }
        
        /// <summary>Navigation property for the building this room belongs to.</summary>
        public Building Building { get; set; }


        /// <summary>Gets or sets the number for this room.</summary>
        /// <remarks>This field is indexed.</remarks>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Number
        {
            get => _number;
            set => _number = value?.Trim().ToUpperInvariant();
        }

        /// <summary>Gets or sets the capacity for this room.</summary>
        [Required]
        [Range(0.000, int.MaxValue, ErrorMessage = "Capacity must be between greater than or equal to zero.")]
        public int Capacity { get; set; }

        /// <summary>Gets or sets the enabled status for this room.</summary>
        [Required]
        [Display(Name = "Enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>Gets the identifier for this room.</summary>
        /// <remarks>Preferably use `Room.Building.Code + Room.Number` in LINQ.</remarks>
        [NotMapped] public string Identifier => Building?.Code + Number;
        
        /// <summary>Navigation property for the scheduled meeting time associations for this room.</summary>
        public List<ScheduledMeetingTimeRoom> ScheduledMeetingTimeRooms { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any room has the same number and building
                if (await context.Rooms
                    .Where(rm => rm.Id != Id)
                    .Where(rm => rm.BuildingId == BuildingId)
                    .Where(rm => rm.Number == Number)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A room in this building exists with the identifier {Identifier}."));
            });
        }
    }
}