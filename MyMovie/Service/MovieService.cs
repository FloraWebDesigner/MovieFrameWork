using MyMovie.Interface;
using MyMovie.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MyMovie.Data;

namespace MyMovie.Services
{
    public class MovieService : MovieInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieDto>> ListMovies()
        {
            // all Movies
            List<Movie> Movies = await _context.Movies
                .ToListAsync();
            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();
            // foreach Movie record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    movie_id = Movie.movie_id,
                    movie_name = Movie.movie_name,
                    year = (int)Movie.year,
                    introduction = (string)Movie.introduction,
                    rate = (decimal)Movie.rate,
                    duration = (string)Movie.duration,
                    director = (string)Movie.director,
                    star = (string)Movie.star,
                    ticket_quantity = (int)Movie.ticket_quantity

                });
            }
            return MovieDtos;
        }

        public async Task<MovieDto?> FindMovie(int id)
        {
            // first or default async will get the first movie matching the {id}
            var Movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.movie_id == id);

            // no Movie found
            if (Movie == null)
            {
                return null;
            }
            // create an instance of MovieDto
            MovieDto MovieDto = new MovieDto()
            {
                movie_id = Movie.movie_id,
                movie_name = Movie.movie_name,
                year = (int)Movie.year,
                introduction = (string)Movie.introduction,
                rate = (decimal)Movie.rate,
                duration = (string)Movie.duration,
                director = (string)Movie.director,
                star = (string)Movie.star,
                ticket_quantity = (int)Movie.ticket_quantity
            };
            return MovieDto;

        }

        public async Task<ServiceResponse> UpdateMovie(MovieDto MovieDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Movie
            Movie Movie = new Movie()
            {
                movie_id = MovieDto.movie_id,
                movie_name = MovieDto.movie_name,
                year = (int)MovieDto.year,
                introduction = (string)MovieDto.introduction,
                rate = (decimal)MovieDto.rate,
                duration = (string)MovieDto.duration,
                director = (string)MovieDto.director,
                star = (string)MovieDto.star,
                ticket_quantity = (int)MovieDto.ticket_quantity
            };
            // flags that the object has changed
            _context.Entry(Movie).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Movies set ... where movie_id={id}
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

        public async Task<ServiceResponse> AddMovie(MovieDto MovieDto)
        {
            ServiceResponse serviceResponse = new();
            // Create instance of Movie
            Movie Movie = new Movie()
            {
                movie_id = MovieDto.movie_id,
                movie_name = MovieDto.movie_name,
                year = (int)MovieDto.year,
                introduction = (string)MovieDto.introduction,
                rate = (decimal)MovieDto.rate,
                duration = (string)MovieDto.duration,
                director = (string)MovieDto.director,
                star = (string)MovieDto.star,
                ticket_quantity = (int)MovieDto.ticket_quantity
            };
            // SQL Equivalent: Insert into Movies (..) values (..)
            Console.WriteLine($"Movie Name: {MovieDto.movie_name}, Year: {MovieDto.year}, Rate: {MovieDto.rate}");
            try
            {
                _context.Movies.Add(Movie);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Movie.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Movie.movie_id;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteMovie(int id)
        {
            ServiceResponse response = new();
            // Movie must exist in the first place
            var Movie = await _context.Movies.FindAsync(id);
            if (Movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Movie cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                _context.Movies.Remove(Movie);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Movie");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<MovieDto> TicketCountForMovie(int id)
        {
            // first or default async will get the first ticket matching the {id}
            var Movie = await _context.Movies
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.movie_id == id);

            // no Movie found
            if (Movie == null)
            {
                return null;
            }
            // create an instance of MovieDto
            MovieDto MovieDto = new MovieDto()
            {
                movie_id = Movie.movie_id,
                movie_name = Movie.movie_name,
                ticket_quantity = (int)Movie.ticket_quantity,
                ticket_sold=Movie.Tickets.Count()
            };
            return MovieDto;
        }

        public async Task<IEnumerable<MovieDto>> ListMoviesForViewer(int id)
        {
            // WHERE customer_id == id
            List<Movie> Movies = await _context.Movies
            .Include(m => m.Tickets)
                .Where(m => m.Tickets.Any(t => t.Viewer.customer_id == id))
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();
            // foreach Viewer record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    movie_id = Movie.movie_id,
                    movie_name = Movie.movie_name,
                    year = (int)Movie.year,
                    introduction = (string)Movie.introduction,
                    rate = (decimal)Movie.rate,
                    duration = (string)Movie.duration,
                    director = (string)Movie.director,
                    star = (string)Movie.star,
                    ticket_quantity = (int)Movie.ticket_quantity
                });
            }
            // return 200 OK with MovieDtos
            return MovieDtos;
        }

    }
}

