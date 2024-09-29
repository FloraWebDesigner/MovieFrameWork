using MyMovie.Interface;
using MyMovie.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MyMovie.Data;

namespace MyMovie.Service
{
    public class TicketService : TicketInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketDto>> ListTickets()
        {
            // include will join the Ticket with 1 movie, 1 customer
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .ToListAsync();
            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    ticket_id = Ticket.ticket_id,
                    ticket_no = Ticket.ticket_no,
                    movie_id = Ticket.Movie.movie_id,
                    movie_name = Ticket.Movie.movie_name,
                    customer_id = Ticket.Viewer.customer_id,
                    customer_name = Ticket.Viewer.first_name+Ticket.Viewer.last_name,
                    identity = Ticket.Viewer.identity
                });
            }
            // return TicketDtos
            return TicketDtos;

        }


        public async Task<TicketDto?> FindTicket(int id)
        {
            // include will join ticket with 1 movie, 1 customer
            // first or default async will get the first ticket matching the {id}
            var Ticket = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .FirstOrDefaultAsync(i => i.ticket_id == id);

            // no Ticket found
            if (Ticket == null)
            {
                return null;
            }
            // create an instance of TicketDto
            TicketDto TicketDto = new TicketDto()
            {
                ticket_id = Ticket.ticket_id,
                ticket_no = Ticket.ticket_no,
                movie_id = Ticket.Movie.movie_id,
                movie_name = Ticket.Movie.movie_name,
                customer_id = Ticket.Viewer.customer_id,
                customer_name = Ticket.Viewer.first_name + Ticket.Viewer.last_name,
                identity = Ticket.Viewer.identity
            };
            return TicketDto;

        }


        public async Task<ServiceResponse> UpdateTicket(TicketDto TicketDto)
        {
            ServiceResponse serviceResponse = new();
            Movie? movie = await _context.Movies.FindAsync(TicketDto.movie_id);
            Viewer? viewer = await _context.Viewers.FindAsync(TicketDto.customer_id);
            // Posted data must link to valid entity
            if (movie == null || viewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                //404 Not Found
                return serviceResponse;
            }

            // Create instance of Ticket
            Ticket Ticket = new Ticket()
            {
                ticket_id = Convert.ToInt32(TicketDto.ticket_id),
                ticket_no = TicketDto.ticket_no,
                Movie = movie,
                Viewer = viewer,
            };
            // flags that the object has changed
            _context.Entry(Ticket).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Tickets set ... where TicketId={id}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record");
                return serviceResponse;
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;

            return serviceResponse;
        }


        public async Task<ServiceResponse> AddTicket(TicketDto TicketDto)
        {
            ServiceResponse serviceResponse = new();
            Movie? movie = await _context.Movies.FindAsync(TicketDto.movie_id);
            Viewer? viewer = await _context.Viewers.FindAsync(TicketDto.customer_id);
            // Posted data must link to valid entity
            if (movie == null || viewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found. ");
                }
                if (viewer == null)
                {
                    serviceResponse.Messages.Add("Viewer was not found.");
                }
                return serviceResponse;
            }

            Ticket Ticket = new Ticket()
            {
                ticket_id = Convert.ToInt32(TicketDto.ticket_id),
                ticket_no = TicketDto.ticket_no,
                Movie = movie,
                Viewer = viewer,
            };
            // SQL Equivalent: Insert into Tickets (..) values (..)

            try
            {
                _context.Tickets.Add(Ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the ticket.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Ticket.ticket_id;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteTicket(int id)
        {
            ServiceResponse response = new();
            // Ticket must exist in the first place
            var Ticket = await _context.Tickets.FindAsync(id);
            if (Ticket == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Ticket cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Tickets.Remove(Ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the ticket");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<TicketDto>> ListTicketsForMovie(int id)
        {
            // WHERE movie_id == id
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .Where(t => t.Movie.movie_id == id)
                .ToListAsync();

            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    ticket_id = Ticket.ticket_id,
                    ticket_no = Ticket.ticket_no,
                    movie_id = Ticket.Movie.movie_id,
                    movie_name = Ticket.Movie.movie_name,
                    customer_id = Ticket.Viewer.customer_id,
                    customer_name = Ticket.Viewer.first_name + Ticket.Viewer.last_name,
                    identity = Ticket.Viewer.identity
                });
            }
            // return 200 OK with TicketDtos
            return TicketDtos;

        }

        public async Task<IEnumerable<TicketDto>> ListTicketsForViewer(int id)
        {
            // WHERE customer_id == id
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .Where(t => t.Viewer.customer_id == id)
                .ToListAsync();

            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach  Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    ticket_id = Ticket.ticket_id,
                    ticket_no = Ticket.ticket_no,
                    movie_id = Ticket.Movie.movie_id,
                    movie_name = Ticket.Movie.movie_name,
                    customer_id = Ticket.Viewer.customer_id,
                    customer_name = Ticket.Viewer.first_name + Ticket.Viewer.last_name,
                    identity = Ticket.Viewer.identity
                });
            }
            // return 200 OK with TicketDtos
            return TicketDtos;

        }
    }
}
