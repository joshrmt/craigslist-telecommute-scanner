using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraigslistScanner.Data {

    public class Job {

        public int Id { get; set; }

        public long PostId { get; set; }

        public String Name { get; set; }

        public String Url { get; set; }

        public DateTime Date { get; set; }

        public DateTime LastUpdated { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        public String Body { get; set; }
    }
}
