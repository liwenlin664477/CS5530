using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            EnrollmentGrade = new HashSet<EnrollmentGrade>();
        }

        public string Semester { get; set; }
        public uint Year { get; set; }
        public uint ClassId { get; set; }
        public uint CourId { get; set; }
        public string PId { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public virtual Courses Cour { get; set; }
        public virtual Professors P { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrade { get; set; }
    }
}
