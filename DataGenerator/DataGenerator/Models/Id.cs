using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Models
{
    using System.Runtime.Serialization;

    using MongoDB.Bson;

    [DataContract]
    public class Id
    {
        public ObjectId id { get; set; }

        [DataMember(Name = "$oid")]
        public string id_string {
            get
            {
                return id.ToString();
            }
            set
            {
                id = new ObjectId(value);
            }
        }
    }
}
