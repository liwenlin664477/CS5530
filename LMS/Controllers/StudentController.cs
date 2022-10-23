using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {

            var query = from e in db.EnrollmentGrade
                        join cs in db.Classes on e.ClassId equals cs.ClassId
                        join c in db.Courses on cs.CourId equals c.CourId
                        where e.UId == uid
                        select new
                        {
                            subject = c.Subject,
                            number = c.Number,
                            name = c.Name,
                            season = cs.Semester,
                            year = cs.Year,
                            grade = e.Grade
                        };


            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {

            var query = from c in db.Courses
                        join cs in db.Classes on c.CourId equals cs.CourId
                        join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.Acid equals a.Acid
                        join sm in db.Submission
                        on new { A = a.AssignId, B = uid } equals new { A = sm.AssignId, B = sm.UId }
                        into AS
                        from assign in AS.DefaultIfEmpty()
                        where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.DueTime,
                            score = assign == null ? null : (uint?)assign.Score
                        };


            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {

            Submission s;
            var query1 = from c in db.Courses
                         join cs in db.Classes on c.CourId equals cs.CourId
                         join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                         join a in db.Assignments on ac.Acid equals a.Acid
                         where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category && a.Name == asgname
                         select new { 
                                assignID =a.AssignId
                         };


            //
            var query2 = from sm in db.Submission
                         where sm.UId == uid && sm.AssignId == query1.ToArray()[0].assignID
                         select sm;
            //
            if (!query2.Any())
            {
                s = new Submission
                {
                    Contents = contents,
                    DateAndTime = DateTime.Now,
                    AssignId = query1.ToArray()[0].assignID,
                    UId = uid,
                    Score = 0
                };
                db.Add(s);
            }
            //
            else
            {
                s = query2.ToArray()[0];
                s.Contents = contents;
                s.DateAndTime = DateTime.Now;
                db.Update(s);
            }
            db.SaveChanges();
            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {

            var query1 = from c in db.Courses
                        from cs in db.Classes
                        where c.Subject == subject && c.Number == num && cs.Semester == season && cs.Year == year && c.CourId == cs.CourId
                        select new { 
                            classID = cs.ClassId    
                        };

            var query2 = from e in db.EnrollmentGrade
                         join cs in db.Classes on e.ClassId equals cs.ClassId
                         join c in db.Courses on cs.CourId equals c.CourId
                         where e.UId == uid
                         select new
                         {
                            allMyClasses = e.ClassId
                         };

            foreach (var q in query2) { 
                if(q.allMyClasses == query1.ToArray()[0].classID )
                    return Json(new { success = false });
            }

            EnrollmentGrade newEnrollmentGrade = new EnrollmentGrade { ClassId = query1.ToArray()[0].classID, UId = uid, Grade = "--" };
            
            db.Add(newEnrollmentGrade);
            db.SaveChanges();
            return Json(new { success = true });

        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {

            var query = from e in db.EnrollmentGrade
                        where e.UId == uid
                        select e;

            double grade = 0.0;
            int numberOfClass = 0;
            foreach (var q in query)
            {
                if (q.Grade == "A")
                {
                    grade += 4.0;
                }
                else if (q.Grade == "A-")
                {
                    grade += 3.7;
                }
                else if (q.Grade == "B+")
                {
                    grade += 3.3;
                }
                else if (q.Grade == "B")
                {
                    grade += 3.0;
                }
                else if (q.Grade == "B-")
                {
                    grade += 2.7;
                }
                else if (q.Grade == "C+")
                {
                    grade += 2.3;
                }
                else if (q.Grade == "C")
                {
                    grade += 2.0;
                }
                else if (q.Grade == "C-")
                {
                    grade += 1.7;
                }
                else if (q.Grade == "D+")
                {
                    grade += 1.3;
                }
                else if (q.Grade == "D")
                {
                    grade += 1.0;
                }
                else if (q.Grade == "D-")
                {
                    grade += 0.7;
                }
                else if (q.Grade == "E")
                {
                    grade += 0;
                }
                numberOfClass++;
            }

            double result = 0.0;
            if (numberOfClass == 0)
            {
                result = 0.0;
            }
            else
            {
                result = grade / numberOfClass;
            }
            return Json(new { gpa = result });
        }

        /*******End code to modify********/

    }
}