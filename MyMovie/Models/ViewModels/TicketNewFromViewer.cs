namespace MyMovie.Models.ViewModels
{
    public class TicketNewFromViewer
    {
            // For a list of viewers to choose from
        //    public IEnumerable<ViewerDto> AllViewers { get; set; }

        //choose which tickets the movie refers
        // public required IEnumerable<MovieDto> MovieOptions { get; set; }

        // For a list of movies to choose from
        public IEnumerable<MovieDto> AllMovies { get; set; }

        public required ViewerDto ViewerDto { get; set; }

        public TicketDto Ticket { get; set; }

    }
}
