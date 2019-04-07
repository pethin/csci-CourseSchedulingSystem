using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class Room
    {
        private string _number;

        public Room()
        {
        }

        public Room(string number, int capacity, bool isEnabled)
        {
            Number = number;
            Capacity = capacity;
            IsEnabled = isEnabled;
        }

        public Guid Id { get; set; }

        [Required]
        public Guid BuildingId { get; set; }
        public virtual Building Building { get; set; }


        public Guid CapabilityId { get; set; }
        public virtual RoomCapability Capability { get; set; }



        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Number
        {
            get => _number;
            set => _number = value.Trim().ToUpper();
        }

        [Required]
        [Range(0.000, int.MaxValue, ErrorMessage = "Capacity must be between greater than or equal to zero.")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Room Enabled")]
        public bool IsEnabled { get; set; }

        [NotMapped] public string Identifier => $@"{Building?.Code ?? $"{BuildingId.ToString()} - "}{Number}";

        public virtual ICollection<RoomRoomCapability> RoomRoomCapability { get; set; }
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any room has the same number and building
                if (await context.Room
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