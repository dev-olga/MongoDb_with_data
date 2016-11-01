using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Models
{
    public class TimeTracking
    {
        public Id _id { get; set; }

        public string description { get; set; }

        public Id task_id { get; set; }

        public Id user_id { get; set; }

        public Id project_id { get; set; }

        public Date date { get; set; }

        public decimal time_spent_in_hours { get; set; }
    }
}
