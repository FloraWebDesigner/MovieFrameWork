using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovie.Models
{
    public class Viewer
    {
        // viewer table properties
        [Key]
        [Column("viewer_id")]
        public int customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string? identity { get; set; }
        public string? membership { get; set; }
        public int? age { get; set; }

        //A viewer has many tickets
        public ICollection<Ticket>? Tickets { get; set; }
    }

    public class ViewerDto
    {
        [Column("viewer_id")]
        public int customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string identity { get; set; }
        public string membership { get; set; }
        public int age { get; set; }
    }
}
