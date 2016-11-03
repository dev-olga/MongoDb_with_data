using System;

namespace DataGenerator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Json;

   // using DataGenerator.Models;
    using System.Threading.Tasks;

    using DataGenerator.Models;
    using DataGenerator.Services;

    using MongoDB.Bson;

    using Task = System.Threading.Tasks.Task;

    class Program
    {
        private static readonly int USERS_COUNT = 100000;
        private static readonly int PROJECTS_COUNT = 10000;
        private static readonly int TASKS_COUNT = 200000;
        private static readonly int TIME_ACCOUNTING_COUNT = 1000000;

        private static readonly int ThreadCount = 1;
        //private static readonly int USERS_COUNT = 1000;
        //private static readonly int PROJECTS_COUNT = 100;
        //private static readonly int TASKS_COUNT = 2000;
        //private static readonly int TIME_ACCOUNTING_COUNT = 10000;
        
        static void Main(string[] args)
        {
            var userIds = new ObjectId[USERS_COUNT];
            for (var i = 0; i < userIds.Length; i++)
            {
                userIds[i] = CreateModelService.CreateId();
            }
            List<User> users = new List<User>();
            for (var i = 0; i < USERS_COUNT; i++)
            {
                var user = CreateModelService.CreateUser(userIds[i], userIds);
                users.Add(user);

                if ((i+1) % 100 == 0)
                {
                    Console.Write("Users: {0:F2}% \r", ((i+1) / (decimal)USERS_COUNT * 100));
                }
            }

            Console.WriteLine("\r\nSaving users...");
            using (var usersFile = new StreamWriter("users.json"))
            {
                var ser = new DataContractJsonSerializer(typeof(User));
                foreach (var user in users)
                {
                    using (var ms = new MemoryStream())
                    {
                        ser.WriteObject(ms, user);
                        ms.Position = 0;
                        var sr = new StreamReader(ms);
                        usersFile.WriteLine(sr.ReadToEnd());
                        sr.Dispose();
                    }
                }
            }
            Console.WriteLine();
            //Console.WriteLine("Tasks Distribution...");
            var tasksDistribution = CreateModelService.CreateDistribution(PROJECTS_COUNT, TASKS_COUNT);
            //Console.WriteLine("Time Accounting Distribution...");
            var timeAccountingDistribution = CreateModelService.CreateDistribution(TASKS_COUNT, TIME_ACCOUNTING_COUNT);
            timeAccountingDistribution.Insert(0, 0);
            var timeAccountingList = timeAccountingDistribution.Skip(1).Zip(timeAccountingDistribution, (f, s) => f - s).ToList();

            using (var projectsFile = new StreamWriter("projects.json"))
            {
                var pmsList = users.Where(p => p.position.Contains("Developer")).ToList();
                var ser = new DataContractJsonSerializer(typeof(Project));
                for (var i = 0; i < PROJECTS_COUNT; i++)
                {
                    List<int> timeAccountings = i > 0
                                                ? timeAccountingList.Skip(tasksDistribution[i - 1])
                                                        .Take(tasksDistribution[i] - tasksDistribution[i - 1]).ToList()
                                                : timeAccountingList.Take(tasksDistribution[i]).ToList();
                    var project = CreateModelService.CreateProject(
                        CreateModelService.CreateId(),
                        users,
                        timeAccountings,
                        pmsList);
                    using (var ms = new MemoryStream())
                    {
                        ser.WriteObject(ms, project);
                        ms.Position = 0;
                        using (var sr = new StreamReader(ms))
                        {
                            projectsFile.WriteLine(sr.ReadToEnd());
                        }
                    }

                    if ((i + 1) % 100 == 0 || i == PROJECTS_COUNT - 1)
                    {
                        Console.Write("Projects: {0:F2}% \r", ((i + 1) / (decimal)PROJECTS_COUNT * 100));
                    }
                }
            }

            Console.WriteLine("\r\nHappy End ^_^");
            Console.ReadLine();
        }
    }
}
