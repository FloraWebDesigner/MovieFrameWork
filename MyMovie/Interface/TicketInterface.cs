using MyMovie.Models;

namespace MyMovie.Interface
{
    public interface TicketInterface
    {
        Task<IEnumerable<TicketDto>> ListTickets();

         Task<TicketDto?> FindTicket(int id);

        Task<ServiceResponse> UpdateTicket(TicketDto TicketDto);

        Task<ServiceResponse> AddTicket(TicketDto TicketDto);

        Task<ServiceResponse> DeleteTicket(int id);

        Task<IEnumerable<TicketDto>> ListTicketsForMovie(int id);

        Task<IEnumerable<TicketDto>> ListTicketsForViewer(int id);

        //Task<ServiceResponse> BookTicketForMovie(int ticket_id, int movie_id);

        //Task<ServiceResponse> CancelTicketForMovie(int ticket_id, int movie_id);

        //Task<ServiceResponse> BookTicketForViewer(int ticket_id, int customer_id);

        //Task<ServiceResponse> CancelTicketForViewer(int ticket_id, int movie_id)
    }
}
