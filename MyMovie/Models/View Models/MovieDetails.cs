namespace MyMovie.Models.View_Models
{
    public class MovieDetails
    {
        //A movie page must have a movie
        public required MovieDto Movie { get; set; }

        //A movie page can have many tickets
        //public IEnumerable<TicketDto>? CategoryProducts { get; set; }
    }
}