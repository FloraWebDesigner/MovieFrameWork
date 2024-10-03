using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMovie;
using MyMovie.Models;
using MyMovie.Service;
using MyMovie.Interface;
using MyMovie.Services;

namespace MyMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieInterface _MovieService;

        // dependency injection of service interfaces
        public MovieController(MovieInterface MovieService)
        {
            _MovieService = MovieService;
        }

        /// <summary>
        /// Returns a single Movie specified by its {id}
        /// </summary>
        /// <param name="id">The Movie id</param>
        /// <returns>
        /// 200 OK
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Movie/Find/1 -> {
        /// {"movie_id": 1,"movie_name": "Green Book","year": 2018,"introduction": "A working-class Italian-American bouncer becomes the driver for an African-American classical pianist on a tour of venues through the 1960s American South.", "rate": 8.2, "duration": "2h 10m", "director": "Peter Farrelly", "star": "Nick Vallelonga,Brian Hayes,CurriePeter Farrelly","ticket_quantity": 50}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<MovieDto>> FindMovie(int id)
        {
            var Movie = await _MovieService.FindMovie(id);

            // if the Movie could not be located, return 404 Not Found
            if (Movie == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Movie);
            }
        }


        /// <summary>
        /// Returns a list of Movies
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{MovieDto},{MovieDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Movie/List -> [{MovieDto},{MovieDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> ListMovies()
        {
            // empty list of data transfer object MovieDto
            IEnumerable<MovieDto> MovieDtos = await _MovieService.ListMovies();
            // return 200 OK with MovieDtos
            return Ok(MovieDtos);
        }


        /// <summary>
        /// Updates a Movie
        /// </summary>
        /// <param name="id">The ID of the Movie to update</param>
        /// <param name="MovieDto">The required information to update the Movie (movie_id,movie_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Movie/Update/1
        /// Request Headers: Content-Type: application/json
        /// Request Body: {MovieDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public async Task<ActionResult> UpdateMovie(int id, MovieDto MovieDto)
        {
            // {id} in URL must match movie_id in POST Body
            if (id != MovieDto.movie_id)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _MovieService.UpdateMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            //Status = Updated
            return NoContent();

        }

        /// <summary>
        /// Adds a Movie
        /// </summary>
        /// <param name="MovieDto">The required information to add the Movie (movie_id,movie_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Movie/Find/{movie_id}
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Movie/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {MovieDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Movie/Find/{movie_id}
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Movie>> AddMovie(MovieDto MovieDto)
        {
            ServiceResponse response = await _MovieService.AddMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Movie/FindMovie/{response.CreatedId}", MovieDto);
        }

        /// <summary>
        /// Deletes the Movie
        /// </summary>
        /// <param name="id">The id of the Movie to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Movie/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            ServiceResponse response = await _MovieService.DeleteMovie(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();

        }

        /// <summary>
        /// Returns a single Movie specified by its {id}
        /// </summary>
        /// <param name="id">The Movie id</param>
        /// <returns>
        /// 200 OK
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Movie/Find/1 -> {
        /// {"movie_id": 1,"movie_name": "Green Book","year": 2018,"introduction": "A working-class Italian-American bouncer becomes the driver for an African-American classical pianist on a tour of venues through the 1960s American South.", "rate": 8.2, "duration": "2h 10m", "director": "Peter Farrelly", "star": "Nick Vallelonga,Brian Hayes,CurriePeter Farrelly","ticket_quantity": 50}
        /// </example>
        [HttpGet(template: "TicketCountForMovie/{id}")]
        public async Task<ActionResult<MovieDto>> TicketCountForMovie(int id)
        {
            var Movie = await _MovieService.TicketCountForMovie(id);

            // if the Movie could not be located, return 404 Not Found
            if (Movie == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Movie);
            }
        }

        //ListMoviesForViewer
        [HttpGet(template: "ListForViewer/{id}")]
        public async Task<IActionResult> ListMoviesForViewer(int id)
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<MovieDto> MovieDtos = await _MovieService.ListMoviesForViewer(id);
            // return 200 OK with ViewerDtos
            return Ok(MovieDtos);
        }
    }
}
