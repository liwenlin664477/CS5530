using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from c in db.Courses
                        from cs in db.Classes
                        where c.CourId == cs.CourId
                        from e in db.EnrollmentGrade
                        where cs.ClassId == e.ClassId
                        from st in db.Students
                        where e.UId == st.UId
                        where c.Subject == subject && c.Number == num && cs.Semester == season && cs.Year == year
                        select new
                        {
                            fname = st.FName,
                            lname = st.LName,
                            uid = st.UId,
                            dob = st.Dob,
                            grade = e.Grade
                        };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {

            if (category == null)
            {
                var query = from c in db.Courses
                            join cs in db.Classes on c.CourId equals cs.CourId
                            join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                            join a in db.Assignments on ac.Acid equals a.Acid
                            where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year
                            select new
                            {
                                aname = a.Name,
                                cname = ac.Name,
                                due = a.DueTime,
                                submissions = a.Submission.Count
                            };

                return Json(query.ToArray());
            }
            else
            {
                var query = from c in db.Courses
                            join cs in db.Classes on c.CourId equals cs.CourId
                            join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                            join a in db.Assignments on ac.Acid equals a.Acid
                            where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category
                            select new
                            {
                                aname = a.Name,
                                cname = ac.Name,
                                due = a.DueTime,
                                submissions = a.Submission.Count
                            };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {

            var query = from c in db.Courses
                        join cs in db.Classes on c.CourId equals cs.CourId
                        join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                        where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year
                        select new
                        {
                            name = ac.Name,
                            weight = ac.GradingWeight
                        };

            return Json(query.ToArray());

        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {

            var query = from c in db.Courses
                         join cs in db.Classes on c.CourId equals cs.CourId
                         where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year
                         select new { 
                            classID = cs.ClassId,                         
                         };

            AssignmentCategories newAssignmentCategories = new AssignmentCategories { Name = category, ClassId = query.ToArray()[0].classID, GradingWeight = (byte)catweight };

            db.Add(newAssignmentCategories);
            db.SaveChanges();
            return Json(new { success = true });

        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {

            var query =  from c in db.Courses
                          join cs in db.Classes on c.CourId equals cs.CourId
                          join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                          where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category
                          select new { 
                            classID = cs.ClassId,
                            acid = ac.Acid
                          };

            Assignments newAssignment = new Assignments { Name = asgname, Contents = asgcontents, Acid = query.ToArray()[0].acid, DueTime = asgdue, Points = (uint)asgpoints };

            db.Add(newAssignment);
            db.SaveChanges();
            var query3 = from e in db.EnrollmentGrade
                         where e.ClassId == query.ToArray()[0].classID
                         select new { 
                         uid = e.UId
                         };

            foreach (var q in query3)
                UpdateGrades(q.uid, query.ToArray()[0].classID, query.ToArray()[0].acid);

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query = from c in db.Courses
                        join cs in db.Classes on c.CourId equals cs.CourId
                        join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                        join a in db.Assignments on ac.Acid equals a.Acid
                        join sm in db.Submission on a.AssignId equals sm.AssignId
                        join s in db.Students on sm.UId equals s.UId
                        where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category
                        && a.Name == asgname
                        select new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = sm.UId,
                            time = sm.DateAndTime,
                            score = sm.Score
                        };


            return Json(query.ToArray());

        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {

            var query1 = (from c in db.Courses
                          join cs in db.Classes on c.CourId equals cs.CourId
                          join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                          join a in db.Assignments on ac.Acid equals a.Acid
                          join sm in db.Submission on a.AssignId equals sm.AssignId
                          join s in db.Students on sm.UId equals s.UId
                          where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category && s.UId == uid && a.Name == asgname
                          select sm).ToArray()[0];

            var query2 = from c in db.Courses
                         join cs in db.Classes on c.CourId equals cs.CourId
                         join ac in db.AssignmentCategories on cs.ClassId equals ac.ClassId
                         join a in db.Assignments on ac.Acid equals a.Acid
                         join sm in db.Submission on a.AssignId equals sm.AssignId
                         join s in db.Students on sm.UId equals s.UId
                         where c.Number == num && c.Subject == subject && cs.Semester == season && cs.Year == year && ac.Name == category && s.UId == uid
                         select new 
                          {
                              classID = cs.ClassId,
                              acid = ac.Acid,
                          };
            query1.Score = (uint)score;
            db.Submission.Update(query1);
            db.SaveChanges();
            UpdateGrades(uid, query2.ToArray()[0].classID, query2.ToArray()[0].acid);
            return Json(new { success = true });

        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {

            var query = from cs in db.Classes
                        from c in db.Courses
                        where cs.CourId == c.CourId && cs.PId == uid
                        select new
                        {
                            subject = c.Subject,
                            number = c.Number,
                            name = c.Name,
                            season = cs.Semester,
                            year = cs.Year
                        };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Compute and update all the assignment scores in all the assignment categories in sum of the weight.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="classID"></param>
        /// <param name="acid"></param>
        private void UpdateGrades(string uid, uint classID, uint acid)
        {
            
           var query1 = (from e in db.EnrollmentGrade
                          where e.ClassId == classID && e.UId == uid
                          select e).ToArray()[0];

            var query2 = from ac in db.AssignmentCategories
                         where ac.Acid == acid
                         join a in db.Assignments on acid equals a.Acid
                         join sm in db.Submission on new { A = a.AssignId, B = uid } equals new { A = sm.AssignId, B = sm.UId }
                         into newTable
                         from t in newTable.DefaultIfEmpty()
                         select new
                         {
                             acName = ac.Name,
                             weight = ac.GradingWeight,
                             points = a.Points,
                             score = t == null ? null : (uint?)t.Score
                         };

            Dictionary<string, Tuple<double, double, double>> studentAllScore = new Dictionary<string, Tuple<double, double, double>>();
            double originalWeight = 0;
            double newScore = 0;
            double newTotalPoints = 0;
            for(int i = 0; i < query2.Count(); i++)
            {
                if (!studentAllScore.ContainsKey(query2.ToArray()[i].acName)) {
                    double studentScore = 0;
                    if (query2.ToArray()[i].score != null) {
                        studentScore = (double)query2.ToArray()[i].score;
                    }
                    studentAllScore.Add(query2.ToArray()[i].acName, new Tuple<double, double, double>(query2.ToArray()[i].weight, studentScore, query2.ToArray()[i].points));
                    originalWeight = query2.ToArray()[i].weight;
                    newScore = studentScore;
                    newTotalPoints = query2.ToArray()[i].points;
                }
                else
                {
                    newScore += query2.ToArray()[i].score == null ? 0 : (double)query2.ToArray()[i].score;
                    newTotalPoints += query2.ToArray()[i].points;
                    Tuple<double, double, double> studentScore = new Tuple<double, double, double>(originalWeight, newScore, newTotalPoints);
                    studentAllScore[query2.ToArray()[i].acName] = studentScore;
                }
            }

            double sumOfWeights = 0.0;
            double sumOfScore = 0.0;    
            for (int i = 0; i < studentAllScore.Keys.Count(); i++)
            {
                sumOfWeights += studentAllScore[studentAllScore.Keys.ToArray()[i]].Item1;
                sumOfScore += (studentAllScore[studentAllScore.Keys.ToArray()[i]].Item2 / studentAllScore[studentAllScore.Keys.ToArray()[i]].Item3) * studentAllScore[studentAllScore.Keys.ToArray()[i]].Item1;
            }

            sumOfScore *= 100 / sumOfWeights;

            query1.Grade = getLetterGrade(sumOfScore);
            db.Update(query1);
            db.SaveChanges();
        }

        private String getLetterGrade(double score)
        {
            if (score >= 93.0f)
            {
                return "A";
            }
            else if (score >= 90.0f)
            {
                return "A-";
            }
            else if (score >= 87.0f)
            {
                return "B+";
            }
            else if (score >= 83.0f)
            {
                return "B";
            }
            else if (score >= 80.0f)
            {
                return "B-";
            }
            else if (score >= 77.0f)
            {
                return "C+";
            }
            else if (score >= 73.0f)
            {
                return "C";
            }
            else if (score >= 70.0f)
            {
                return "C-";
            }
            else if (score >= 67.0f)
            {
                return "D+";
            }
            else if (score >= 63.0f)
            {
                return "D";
            }
            else if (score >= 60.0f)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }

        /*******End code to modify********/

    }
}