using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Models
{
    public class Project
    {
        public Id _id { get; set; }

        public string name { get; set; }
        public Id project_manager_id { get; set; }

        public Date start_date { get; set; }

        public Date end_date { get; set; }

        public string status { get; set; }

        public Id[] participants { get; set; }

        public int estimated_budget { get; set; }

        public Task[] tasks { get; set; }
    }
}
