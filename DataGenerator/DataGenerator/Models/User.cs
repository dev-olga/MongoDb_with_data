namespace DataGenerator.Models
{
    using System.Runtime.Serialization;

    public class User
    {
        public Id _id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public Date date_of_birth { get; set; }

        public string email { get; set; }

        public string work_phone { get; set; }

        public string home_phone { get; set; }

        public string cell_phone { get; set; }

        public string position { get; set; }

        public string department { get; set; }

        public string work_status { get; set; }

        public string login { get; set; }

        public Id[] want_to_work_with { get; set; }
    }
}
