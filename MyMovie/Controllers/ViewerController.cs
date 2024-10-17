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
using Microsoft.AspNetCore.Authorization;

namespace MyMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewerController : ControllerBase
    {
        private readonly ViewerInterface _ViewerService;

        // dependency injection of service interfaces
        public ViewerController(ViewerInterface ViewerService)
        {
            _ViewerService = ViewerService;
        }

        /// <summary>
        /// Returns a single Viewer specified by its {id}
        /// </summary>
        /// <param name="id">customer_id</param>
        /// <returns>
        /// 200 OK
        /// {ViewerDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Viewer/Find/1 -> {
        /// {"customer_id": 1,  "first_name": "Ada",  "last_name": "Smith",  "identity": "general audience",  "membership": "N", "age": 50}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ViewerDto>> FindViewer(int id)
        {
            var Viewer = await _ViewerService.FindViewer(id);

            // if the Viewer could not be located, return 404 Not Found
            if (Viewer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Viewer);
            }
        }


        /// <summary>
        /// Returns a list of Viewers
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ViewerDto},{ViewerDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Viewer/List -> [{ViewerDto},{ViewerDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ViewerDto>>> ListViewers()
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<ViewerDto> ViewerDtos = await _ViewerService.ListViewers();
            // return 200 OK with ViewerDtos
            return Ok(ViewerDtos);
        }


        /// <summary>
        /// Updates a Viewer
        /// </summary>
        /// <param name="id">The ID of the Viewer to update</param>
        /// <param name="ViewerDto">The required information to update the Viewer (customer_id,Viewer_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Viewer/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ViewerDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateViewer(int id, ViewerDto ViewerDto)
        {
            
            // {id} in URL must match customer_id in POST Body
            if (id != ViewerDto.customer_id)
            {
                //400 Bad Request
                return BadRequest();
            }
            ViewerDto.membership = string.IsNullOrEmpty(ViewerDto.membership) ? "N" : "Y";
            ServiceResponse response = await _ViewerService.UpdateViewer(ViewerDto);

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
        /// Adds a Viewer
        /// </summary>
        /// <param name="ViewerDto">The required information to add the Viewer (customer_id,Viewer_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Viewer/Find/{customer_id}
        /// {ViewerDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Viewer/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ViewerDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Viewer/Find/{customer_id}
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Viewer>> AddViewer(ViewerDto ViewerDto)
        {
            ViewerDto.membership = string.IsNullOrEmpty(ViewerDto.membership) ? "N" : "Y";
            ServiceResponse response = await _ViewerService.AddViewer(ViewerDto);
            

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Viewer/FindViewer/{response.CreatedId}", ViewerDto);
        }

        /// <summary>
        /// Deletes the Viewer
        /// </summary>
        /// <param name="id">The id of the Viewer to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Viewer/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteViewer(int id)
        {
            ServiceResponse response = await _ViewerService.DeleteViewer(id);

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

        //ListViewersForMovie
        [HttpGet(template: "ListForMovie/{id}")]
        public async Task<IActionResult> ListViewersForMovie(int id)
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<ViewerDto> ViewerDtos = await _ViewerService.ListViewersForMovie(id);
            // return 200 OK with ViewerDtos
            return Ok(ViewerDtos);
        }

        [HttpDelete(template:"RemoveForMovie/{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveViewerForMovie(int id)
        {
            ServiceResponse response = await _ViewerService.RemoveViewerForMovie(id);

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

    }
}
