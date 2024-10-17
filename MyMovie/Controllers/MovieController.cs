﻿using System;
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
using Microsoft.AspNetCore.Authorization;

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
        /// 302 Redirect (/Identity/Account/Login)
        /// or
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Movie/Update/1
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {MovieDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        [Authorize]
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
        /// 302 Redirect (/Identity/Account/Login)
        /// or
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
        [Authorize]
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
        /// 302 Redirect (/Identity/Account/Login)
        /// or
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
        [Authorize]
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

        /// <summary>
        /// Returns a list of movies specified by its viewer
        /// </summary>
        /// <param name="id">The viewer id</param>
        /// <returns>
        /// 200 OK
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// /api/Movie/ListForViewer/5 ->
        ///  {
  //  "movie_id": 1,
  //  "movie_name": "Green Book",
  //  "year": 2018,
  //  "introduction": "A working-class Italian-American bouncer becomes the driver for an African-American classical pianist on a tour of venues through the 1960s American South.",
  //  "rate": 8.2,
  //  "duration": "2h",
  //  "director": "Peter Farrelly",
  //  "star": "Nick Vallelonga,Brian Hayes,CurriePeter Farrelly",
  //  "ticket_quantity": 50,
  //  "ticket_sold": 0,
  //  "ticket_available": 0
  //},
  //{
  //  "movie_id": 2,
  //  "movie_name": "The Others",
  //  "year": 2001,
  //  "introduction": "In 1945, immediately following the end of Second World War, a woman who lives with her two photosensitive children on her darkened old family estate in the Channel Islands becomes convinced that the home is haunted.",
  //  "rate": 7.6,
  //  "duration": "1h 44m",
  //  "director": "Alejandro  Amenábar",
  //  "star": "Nicole Kidman,Christopher Eccleston,Fionnula Flanagan",
  //  "ticket_quantity": 30,
  //  "ticket_sold": 0,
  //  "ticket_available": 0
  //},
  //{
  //  "movie_id": 5,
  //  "movie_name": "The Invisible Guest",
  //  "year": 2016,
  //  "introduction": "A young businessman wakes up in a hotel room locked from the inside with the dead body of his lover next to him. He hires a prestigious lawyer, and over one night they work together to clarify what happened in a frenetic race against time.",
  //  "rate": 8,
  //  "duration": "1h 46m",
  //  "director": "Oriol Paulo",
  //  "star": "Mario Casas,Ana Wagener,Jose Coronado",
  //  "ticket_quantity": 30,
  //  "ticket_sold": 0,
  //  "ticket_available": 0
  //},
  //{
  //  "movie_id": 7,
  //  "movie_name": "Shutter Island",
  //  "year": 2010,
  //  "introduction": "Teddy Daniels and Chuck Aule, two US marshals, are sent to an asylum on a remote island in order to investigate the disappearance of a patient, where Teddy uncovers a shocking truth about the place.",
  //  "rate": 8.2,
  //  "duration": "2h 18m",
  //  "director": "Martin Scorsese",
  //  "star": "Leonardo DiCaprio,Emily Mortimer,Mark Ruffalo",
  //  "ticket_quantity": 50,
  //  "ticket_sold": 0,
  //  "ticket_available": 0
  //}
/// </example>
//ListMoviesForViewer
[HttpGet(template: "ListForViewer/{id}")]
        [Authorize]
        public async Task<IActionResult> ListMoviesForViewer(int id)
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<MovieDto> MovieDtos = await _MovieService.ListMoviesForViewer(id);
            // return 200 OK with ViewerDtos
            return Ok(MovieDtos);
        }
    }
}
