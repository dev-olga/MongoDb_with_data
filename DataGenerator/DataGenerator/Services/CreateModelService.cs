using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Services
{
    using System.Linq.Expressions;

    using DataGenerator.Models;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using Task = System.Threading.Tasks.Task;

    static class CreateModelService
    {
        private static Random rdm = new Random();

        private static readonly int DEADLINE_PROBABILITY = 5;

        private static readonly int NAME_MAX_LENGTH = 20;
        private static readonly int NAME_MIN_LENGTH = 10;

        public static ObjectId CreateId()
        {
            byte[] tmp = new byte[12];
            rdm.NextBytes(tmp);
            return new ObjectId(tmp);
        }

        public static User CreateUser(ObjectId id, ObjectId[] userIds)
        {
            var len = Math.Min(rdm.Next(10), userIds.Length - 1);
            var wantToWork = new List<Id>();
            for (var i = 0; i < len; i++)
            {
                var tmpId = userIds[rdm.Next(userIds.Length)];
                if (tmpId != id)
                {
                    wantToWork.Add(new Id() { id = tmpId });
                }
            }
            var user = new User
                           {
                               _id = new Id { id = id },
                               first_name = FakeData.NameData.GetFirstName(),
                               last_name = FakeData.NameData.GetSurname(),
                               date_of_birth = new Date { date = 
                                   FakeData.DateTimeData.GetDatetime(
                                       new DateTime(1960, 1, 1),
                                       new DateTime(1993, 12, 31)).Date
                               },
                               work_phone = FakeData.PhoneNumberData.GetPhoneNumber(),
                               home_phone = FakeData.PhoneNumberData.GetPhoneNumber(),
                               cell_phone = FakeData.PhoneNumberData.GetPhoneNumber(),
                               department =
                                   FakeData.CollectionData.GetElement(
                                       new string[]
                                           {
                                               "Production", "Production", "Production", "Production", "Production",
                                               "Production QA", "Production QA", "Production QA", "Production Design"/*,
                                               "Human Resources"*/
                                           }),
                               work_status =
                                   FakeData.CollectionData.GetElement(
                                       new string[]
                                           {
                                               "fired", "working", "working", "working", "working", "working" 
                                           }),
                               want_to_work_with = wantToWork.ToArray()
                           };

            /*if (user.department == "Human Resources")
            {
                user.position = FakeData.CollectionData.GetElement(new[] { "HR Manager", "Support Manager" });
            }
            else*/ if (user.department == "Production QA")
            {
                user.position =
                    FakeData.CollectionData.GetElement(
                        new[]
                            {
                                "QA Automation Engineer", "QA Automation Trainee", "QA Engineer", "QA Engineer Trainee" 
                            });
            }
            else if (user.department == "Production Design")
            {
                user.position = FakeData.CollectionData.GetElement(new[] { "Designer Trainee", "Designer" });
            }
            else
            {
                user.position = string.Format(
                    "{0} {1}",
                    FakeData.CollectionData.GetElement(
                        new[] { ".NET", "Java", "Python", "Android", "iPhone", "JavaScript", "PHP" }),
                    FakeData.CollectionData.GetElement(
                        new[] { "Trainee", "Developer", "Developer", "Developer", "Developer" }));
            }
            user.login = (user.first_name.Substring(0, 1) + user.last_name).ToLower();
            user.email = (user.first_name + "." + user.last_name + "@test.com").ToLower();
            return user;
        }

        public static Project CreateProject(ObjectId id, List<User> usersList, List<int> tasksTimeAccountins, IEnumerable<User> pmsList)
        {
            var start_date = FakeData.DateTimeData.GetDatetime(
                                             new DateTime(2000, 1, 1),
                                             DateTime.Now.Date);
            var end_date = FakeData.DateTimeData.GetDatetime(start_date, new DateTime(2018, 1, 1));

            var project = new Project
                              {
                                  _id = new Id { id = id },
                                  end_date = new Date { date = end_date },
                                  estimated_budget =
                                      (int)Math.Round(rdm.NextDouble() * Math.Pow(10, rdm.Next(4, 7))),
                                  name = FakeData.TextData.GetSentence().Substring(0, rdm.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                                  start_date = new Date { date = start_date },
                                  status = end_date > DateTime.Now.Date
                                   ? "open"
                                   : FakeData.CollectionData.GetElement(new[] { "closed", "closed", "closed", "closed", "postponed" }),
                              };
            var pmCondidats = pmsList;
            var allowedUsers = usersList;
            var pmIndex = rdm.Next(pmCondidats.Count());
            var pm = pmCondidats.ElementAt(pmIndex);
            project.project_manager_id = pm._id;

            var usersCount = allowedUsers.Count();
            var partisipantsIndexies = new List<int>() { pmIndex };
            for (var i = 0; i < rdm.Next(Math.Min(usersCount, tasksTimeAccountins.Count)); i++)
            {
                var next = rdm.Next(0, usersCount);
                if (partisipantsIndexies.Contains(next))
                {
                    i--;
                }
                else
                {
                    partisipantsIndexies.Add(next);
                }
            }

            var participants = partisipantsIndexies.Select(t => allowedUsers.ElementAt(t)).ToList();
            var tasks = new List<Models.Task>();
            var taskIndex = 0;
            for (var userIndex = 0; userIndex < participants.Count; userIndex++)
            {
                var user = participants[userIndex];
                var tasksLastIndex = rdm.Next(taskIndex + 1, tasksTimeAccountins.Count - (participants.Count - userIndex) + 1);
                for (; taskIndex < tasksLastIndex; taskIndex++)
                {
                    tasks.Add(CreateTask(CreateId(), project, user._id.id, tasksTimeAccountins[taskIndex]));
                }
            }
            project.participants = participants.Select(p => p._id).ToArray();
            project.tasks = tasks.ToArray();
            return project;
        }

        private static Models.Task CreateTask(ObjectId id, Project project, ObjectId userId, int timesCount)
        {
            var start_date = FakeData.DateTimeData.GetDatetime(project.start_date.date, project.end_date.date);
            var maxDate = /*project.end_date.date > DateTime.Now ? DateTime.Now :*/ project.end_date.date;
            var task = new Models.Task
                           {
                               _id = new Id { id = id },
                               start_date = new Date { date = start_date },
                               end_date = new Date { date = FakeData.DateTimeData.GetDatetime(start_date, maxDate) },
                               description = FakeData.TextData.GetSentence(),
                               name = FakeData.TextData.GetSentence().Substring(0, rdm.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                               project_id = project._id,
                               responsible_person_id = new Id { id = userId },
                               status =
                                   FakeData.CollectionData.GetElement(
                                       new[] { "open", "in progress", "completed", "reopened", "closed" }),
                           };
            if(rdm.Next(DEADLINE_PROBABILITY) == 0)
            {
                task.description = task.description.Insert(rdm.Next(task.description.Length), " deadline ");
            }
            var times = new List<TimeTracking>();
            for (var i = 0; i < timesCount; i++)
            {
                var time = new TimeTracking
                           {
                               _id = new Id { id = CreateId() },
                               date =
                                   new Date
                                       {
                                           date = FakeData.DateTimeData.GetDatetime(task.start_date.date, task.end_date.date)
                                       },
                               description = FakeData.TextData.GetSentence(),
                               project_id = task.project_id,
                               task_id = task._id,
                               user_id = task.responsible_person_id,
                           };
                var spentTime = rdm.Next(1, 9);
                time.time_spent_in_hours = Math.Min(
                    spentTime,
                    task.end_date.date.Subtract(time.date.date).Hours);
                times.Add(time);
            }
            task.time_traking = times.ToArray();
            return task;
        }

        public static List<int> CreateDistribution(int count, int maxValue, int threadsCount = 10)
        {
            var rdm = new Random();
            var tasks = new List<Task<List<int>>>();
            for (var t = 0; t < threadsCount; t++)
            {
                var index = t;
                var task = new Task<List<int>>(
                    () =>
                    {
                        var c = count / threadsCount;

                        var min = (maxValue / threadsCount) * index;
                        var max = (maxValue / threadsCount) * (index + 1);

                        var list = new List<int>();
                        for (var i = 0; i < c; i++)
                        {
                            var next = rdm.Next(min, max) + 1;
                            if (list.Contains(next))
                            {
                                i--;
                            }
                            else
                            {
                                list.Add(next);
                            }
                        }
                        list.Sort();
                        return list;
                    });
                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            var res = tasks.SelectMany(t => t.Result).OrderBy(t => t).ToList();
            res[res.Count - 1] = maxValue;
            //res.Insert(0, 0);
            //res = res.Skip(1).Zip(res, (f, s) => f - s).ToList();
            return res;
        }
    }
}
