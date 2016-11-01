using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Date
    {
        [DataMember(Name = "$date")]
        public string date_string 
        {
            get
            {
                return this.date.ToString("yyyy-MM-dd") + "T00:00:00Z";
            }
            set
            {
                this.date = DateTime.Parse(value);
            }
        }

        public DateTime date { get; set; }
    }
}
