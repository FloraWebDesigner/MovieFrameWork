using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyMovie.Models
{
    public class Movie
    {
        // movie table properties
        [Key]
        public int movie_id { get; set; }
        public string movie_name { get; set; }

        // change year type to int
        public int? year { get; set; }
        public string? introduction { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        public decimal? rate { get; set; }
        public string? duration { get; set; }
        public string? director { get; set; }
        public string? star { get; set; }
        public int? ticket_quantity { get; set; }

        //A movie releases many tickets
        public ICollection<Ticket>? Tickets { get; set; }

    }

    public class MovieDto
    {
        public int movie_id { get; set; }
        public string movie_name { get; set; }

        // change year type to int
        public int year { get; set; }
        public string introduction { get; set; }
        public decimal rate { get; set; }
        public string duration { get; set; }
        public string director { get; set; }
        public string star { get; set; }
        public int ticket_quantity { get; set; }

        // to synthesis the information
        public int ticket_sold {  get; set; }
        // add ticket available  - Oct 10
        public int ticket_available { get; set; }
    }
}
