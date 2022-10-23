using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            EnrollmentGrade = new HashSet<EnrollmentGrade>();
            Submission = new HashSet<Submission>();
        }

        public string UId { get; set; }
        public string Subject { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime Dob { get; set; }

        public virtual Departments SubjectNavigation { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrade { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
