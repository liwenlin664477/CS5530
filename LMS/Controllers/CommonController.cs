using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        // TODO: Uncomment and change 'X' after you have scaffoled


        protected Team4LMSContext db;

        public CommonController()
        {
            db = new Team4LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */

        // TODO: Uncomment and change 'X' after you have scaffoled

        public void UseLMSContext(Team4LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            // TODO: Do not return this hard-coded array.
            var query = from d in db.Departments
                        select new
                        {
                            name = d.Name,
                            subject = d.Subject
                        };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var query = from d in db.Departments
                        select new
                        {
                            subject = d.Subject,
                            dname = d.Name,
                            courses = from c in db.Courses
                                      where c.Subject == d.Subject
                                      select new
                                      {
                                          number = c.Number,
                                          cname = c.Name
                                      }
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {

            var query = from c in db.Courses
                        where c.Subject == subject && c.Number == number
                        join cls in db.Classes on c.CourId equals cls.CourId
                        join p in db.Professors on cls.PId equals p.UId
                        select new
                        {
                            season = cls.Semester,
                            year = cls.Year,
                            location = cls.Location,
                            start = cls.StartTime,
                            end = cls.EndTime,
                            fname = p.FName,
                            lname = p.LName
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {

            var query = from c in db.Courses
                        where c.Number == num && c.Subject == subject
                        from cls in db.Classes
                        where cls.CourId == c.CourId
                        where cls.Semester == season
                        where cls.Year == year
                        from ac in db.AssignmentCategories
                        where ac.ClassId == cls.ClassId
                        where ac.Name == category
                        from a in db.Assignments
                        where a.Acid == ac.Acid
                        where a.Name == asgname
                        select a;


            return Content(query.ToArray().Length == 0 ? "" : query.First().Contents);
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {

            var query = from c in db.Courses
                        where c.Number == num
                        where c.Subject == subject
                        from cls in db.Classes
                        where cls.CourId == c.CourId
                        where cls.Semester == season
                        where cls.Year == year
                        from ac in db.AssignmentCategories
                        where cls.ClassId == ac.ClassId
                        where ac.Name == category
                        from a in db.Assignments
                        where a.Acid == ac.Acid
                        where a.Name == asgname
                        from s in db.Submission
                        where s.AssignId == a.AssignId
                        where s.UId == uid
                        select new { contents = s.Contents };


            return Content(query.ToArray().Length == 0 ? "" : query.First().contents);
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {

            var sQuery = from s in db.Students
                         where s.UId == uid
                         join d in db.Departments on s.Subject equals d.Subject
                         select new
                         {
                             fname = s.FName,
                             lname = s.LName,
                             uid = s.UId,
                             department = d.Name
                         };

            if (sQuery.Any())
            {
                return Json(sQuery.ToArray()[0]);
            }

            var pQuery = from p in db.Professors
                         where p.UId.Equals(uid)
                         join d in db.Departments on p.Subject equals d.Subject
                         select new
                         {
                             fname = p.FName,
                             lname = p.LName,
                             uid = p.UId,
                             department = d.Name
                         };

            if (pQuery.Any())
            {
                return Json(pQuery.ToArray()[0]);
            }

            var aQuery = from a in db.Administrators
                         where a.UId.Equals(uid)
                         select new
                         {
                             fname = a.FName,
                             lname = a.LName,
                             uid = a.UId,
                         };

            if (aQuery.Any())
            {
                return Json(aQuery.ToArray()[0]);
            }

            return Json(new { success = false });
        }

        /*******End code to modify********/

    }
}