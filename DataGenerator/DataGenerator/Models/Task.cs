using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Models
{
    public class Task
    {
        public Id _id { get; set; }

        public string name { get; set; }

        public Date start_date { get; set; }

        public Date end_date { get; set; }

        public string status { get; set; }
        
        public string description { get; set; }

        public Id project_id { get; set; }

        public Id responsible_person_id { get; set; }

        public TimeTracking[] time_traking { get; set; }

    }
}
