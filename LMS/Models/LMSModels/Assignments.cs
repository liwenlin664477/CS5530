using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submission = new HashSet<Submission>();
        }

        public string Name { get; set; }
        public uint AssignId { get; set; }
        public uint Acid { get; set; }
        public string Contents { get; set; }
        public DateTime DueTime { get; set; }
        public uint Points { get; set; }

        public virtual AssignmentCategories Ac { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
