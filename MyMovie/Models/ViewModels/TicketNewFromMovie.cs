namespace MyMovie.Models.ViewModels
{
    public class TicketNewFromMovie
    {
        // For a list of viewers to choose from
        public IEnumerable<ViewerDto> AllViewers { get; set; }
        public required MovieDto MovieDto { get; set; }

        public TicketDto Ticket { get; set; }
    }
}
