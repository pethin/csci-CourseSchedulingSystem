using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a meeting type, e.g., Class or Additional Class time.</summary>
    /// <remarks>
    /// <para>A meeting type must have a unique ID, unique Code, and unique Name.</para>
    /// <para>A meeting type has many scheduled meeting times.</para>
    /// </remarks>
    public class MeetingType
    {
        private string _name;
        private string _code;

        /// <summary>Creates a meeting type with default fields.</summary>
        public MeetingType()
        {
        }

        /// <summary>Creates a meeting type with specified fields.</summary>
        public MeetingType(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        /// <summary>Static instance of Class meeting type.</summary>
        /// <remarks>This instance should be seeded into the database by the SchedulingSchemaSeeder.</remarks>
        [NotMapped] public static readonly MeetingType ClassMeetingType =
            new MeetingType(Guid.Parse("00000000-0000-0000-0000-000000000001"), "CLAS", "Class");

        /// <summary>Static instance of AdditionalClassTime meeting type.</summary>
        /// <remarks>This instance should be seeded into the database by the SchedulingSchemaSeeder.</remarks>
        [NotMapped] public static readonly MeetingType AdditionalClassTimeMeetingType =
            new MeetingType(Guid.Parse("00000000-0000-0000-0000-000000000002"), "CLSS", "Additional Class time");

        /// <summary>Gets or sets the primary key for this meeting type.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the code for this building.</summary>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value?.Trim().ToUpperInvariant();
        }

        /// <summary>Gets or sets the name for this building.</summary>
        /// <remarks>The normalized version of this field must be unique.</remarks>
        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value?.Trim();
                NormalizedName = _name?.ToUpperInvariant();
            }
        }

        /// <summary>Gets the normalized name for this building.</summary>
        /// <remarks>This field is automated.</remarks>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Navigation property for the scheduled meeting times this meeting type has.</summary>
        public List<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }
    }
}