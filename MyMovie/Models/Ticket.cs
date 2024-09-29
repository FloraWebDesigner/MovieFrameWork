using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovie.Models
{
    public class Ticket
    {
        // ticket table properties
        [Key]
        public int ticket_id { get; set; }
        public int ticket_no { get; set; }

        //Each ticket belongs to one movie
        public virtual Movie Movie { get; set; }

        //Each ticket belongs to one viewer
        public virtual Viewer Viewer { get; set; }

        [ForeignKey("Viewer")]
        public int viewer_id { get; set; }

    }
    public class TicketDto
    {
        public int? ticket_id { get; set; }
        public int ticket_no { get; set; }

        public int movie_id { get; set; }

        [Column("viewer_id")]
        public int customer_id { get; set; }

        //flattened from Ticket -> Movie
        public string? movie_name { get; set; }
        public int ticket_quantity { get; set; }

        //flattened from Ticket -> Viewer: first_name + last_name
        public string? customer_name { get; set; }
        public string? identity { get; set; }


    }
}
