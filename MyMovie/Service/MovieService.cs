using MyMovie.Interface;
using MyMovie.Models;
using Microsoft.EntityFrameworkCore;
using System;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MyMovie.Data;
using System.ComponentModel.DataAnnotations;
using MyMovie.Data.Migrations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                MovieDto MovieDto=new MovieDto()
                {
                    movie_id = Movie.movie_id,
                    movie_name = Movie.movie_name,
                    year = (int)Movie.year,
                    introduction = (string)Movie.introduction,
                    rate = (decimal)Movie.rate,
                    duration = (string)Movie.duration,
                    director = (string)Movie.director,
                    star = (string)Movie.star,
                    ticket_quantity = (int)Movie.ticket_quantity,
                    HasPic=Movie.HasPic,

                };
                if (Movie.HasPic)
                {
                    MovieDto.MovieImgPath = $"/img/movies/{Movie.movie_id}{Movie.PicExtension}";
                }

                

                MovieDtos.Add(MovieDto);
            }



            return MovieDtos;
        }

        public async Task<MovieDto?> FindMovie(int id)
        {
            // first or default async will get the first movie matching the {id}
            var Movie = await _context.Movies
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.movie_id == id);

            // no Movie found
            if (Movie == null)
            {
                return null;
            }

            // add ticket_sold in  FindMovie instead of a separate TicketCountForMovie - Oct 10
            int ticket_sold = Movie.Tickets.Count();
            int ticket_quantity = (int)Movie.ticket_quantity;
            int ticket_available = Math.Max(0, ticket_quantity - ticket_sold);
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

                ticket_quantity = ticket_quantity,
                ticket_sold = ticket_sold,

                // calculate the rest number of ticket
                ticket_available = ticket_quantity - ticket_sold,
                // identify if there's a poster pic
                HasPic = (bool)Movie.HasPic,
            };

            if (Movie.HasPic)
            {
                // if yes, then get the url of that image
                MovieDto.MovieImgPath = $"/img/movies/{Movie.movie_id}{Movie.PicExtension}";
            }
          
            return MovieDto;

        }

        public async Task<ServiceResponse> UpdateMovie(MovieDto MovieDto)
        {
            ServiceResponse serviceResponse = new();

            Movie? Movie = await _context.Movies.FindAsync(MovieDto.movie_id);


                       if (Movie == null)
            {
                serviceResponse.Messages.Add("Movie could not be found");
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            Movie.movie_id = MovieDto.movie_id;
            Movie.movie_name = MovieDto.movie_name;
            Movie.year = (int)MovieDto.year;
            Movie.introduction = (string)MovieDto.introduction;
            Movie.rate = (decimal)MovieDto.rate;
            Movie.duration = (string)MovieDto.duration;
            Movie.director = (string)MovieDto.director;
            Movie.star = (string)MovieDto.star;
            Movie.ticket_quantity = (int)MovieDto.ticket_quantity;


            // flags that the object has changed
            _context.Entry(Movie).State = EntityState.Modified;
            // handled by another method
            _context.Entry(Movie).Property(p => p.HasPic).IsModified = false;
            _context.Entry(Movie).Property(p => p.PicExtension).IsModified = false;

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


        public async Task<ServiceResponse> AddMovie(MovieDto MovieDto, IFormFile MoviePic)
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
                ticket_quantity = (int)MovieDto.ticket_quantity,
                HasPic = (bool)MovieDto.HasPic,

            };
            if (Movie.HasPic)
            {

                MovieDto.MovieImgPath = $"/img/movies/{Movie.movie_id}{Movie.PicExtension}";
            };
        
            // SQL Equivalent: Insert into Movies (..) values (..)
            Console.WriteLine($"Movie Name: {MovieDto.movie_name}, Year: {MovieDto.year}, Path: {MovieDto.MovieImgPath}");
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

        // combined the below code into the FindMovie - Oct-10
        public async Task<MovieDto?> TicketCountForMovie(int id)
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
                ticket_sold = Movie.Tickets.Count()
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




        public async Task<ServiceResponse> UpdateMovieImg(int id, IFormFile MoviePic)
        {
            Console.WriteLine($"UpdateMovieImg called with id={id}");
            ServiceResponse response = new();

            Movie? Movie = await _context.Movies.FindAsync(id);
            if (Movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Movie {id} not found");
                return response;
            }

            if (MoviePic.Length > 0)
            {


                // remove old picture if exists
                if (Movie.HasPic)
                {
                    string OldFileName = $"{Movie.movie_id}{Movie.PicExtension}";
                    string OldFilePath = Path.Combine("wwwroot/img/movies/", OldFileName);
                    if (File.Exists(OldFilePath))
                    {
                        File.Delete(OldFilePath);
                    }

                }


                //establish valid file types (can be changed to other file extensions if desired!)
                List<string> Extensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
                string MoviePicExtension = Path.GetExtension(MoviePic.FileName).ToLowerInvariant();
                if (!Extensions.Contains(MoviePicExtension))
                {
                    response.Messages.Add($"{MoviePicExtension} is not a valid file extension");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                string FileName = $"{id}{MoviePicExtension}";
                string FilePath = Path.Combine("wwwroot/img/movies/", FileName);

                using (var targetStream = File.Create(FilePath))
                {
                    MoviePic.CopyTo(targetStream);
                    Console.WriteLine("File has been copied to the target location.");
                }

                // check if file was uploaded
                if (File.Exists(FilePath))
                {
                    Console.WriteLine("File exists at the path after upload.");
                    Movie.PicExtension = MoviePicExtension;
                    Movie.HasPic = true;

                    _context.Entry(Movie).State = EntityState.Modified;

                    try
                    {
                        // SQL Equivalent: Update Movies set ... where MovieId={id}
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Movie updated: HasPic={Movie.HasPic}, PicExtension={Movie.PicExtension}");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add("An error occurred updating the record");

                        return response;
                    }
                }

            }
            else
            {
                response.Messages.Add("No File Content");
                response.Status = ServiceResponse.ServiceStatus.Error;
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

    }
}

