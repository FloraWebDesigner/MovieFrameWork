using MyMovie.Interface;
using MyMovie.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MyMovie.Data;
using MyMovie.Data.Migrations;

namespace MyMovie.Services
{
    public class ViewerService : ViewerInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public ViewerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ViewerDto>> ListViewers()
        {
            // all Viewers
            List<Viewer> Viewers = await _context.Viewers
                .ToListAsync();
            // empty list of data transfer object ViewerDto
            List<ViewerDto> ViewerDtos = new List<ViewerDto>();
            // foreach Viewer record in database
            foreach (Viewer Viewer in Viewers)
            {
                // create new instance of ViewerDto, add to list
                ViewerDtos.Add(new ViewerDto()
                {
                    customer_id = Viewer.customer_id,
                    first_name = Viewer.first_name,
                    last_name = Viewer.last_name,
                    identity = Viewer.identity,
                    membership = Viewer.membership,
                    age = (int)Viewer.age

                });
            }
            return ViewerDtos;
        }

        public async Task<ViewerDto?> FindViewer(int id)
        {
            // first or default async will get the first viewer matching the {id}
            var Viewer = await _context.Viewers
                .FirstOrDefaultAsync(v => v.customer_id == id);

            // no Viewer found
            if (Viewer == null)
            {
                return null;
            }
            // create an instance of ViewerDto
            ViewerDto ViewerDto = new ViewerDto()
            {
                customer_id = Viewer.customer_id,
                first_name = Viewer.first_name,
                last_name = Viewer.last_name,
                identity = Viewer.identity,
                membership = Viewer.membership,
                age = (int)Viewer.age
            };
            return ViewerDto;

        }

        public async Task<ServiceResponse> UpdateViewer(ViewerDto ViewerDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Viewer
            Viewer Viewer = new Viewer()
            {
                customer_id = ViewerDto.customer_id,
                first_name = ViewerDto.first_name,
                last_name = ViewerDto.last_name,
                identity = ViewerDto.identity,
                membership = ViewerDto.membership,
                age = ViewerDto.age
            };
            // flags that the object has changed
            _context.Entry(Viewer).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Viewers set ... where Viewer_id={id}
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

        public async Task<ServiceResponse> AddViewer(ViewerDto ViewerDto)
        {
            ServiceResponse serviceResponse = new();
            // Create instance of Viewer
            Viewer Viewer = new Viewer()
            {
                customer_id = ViewerDto.customer_id,
                first_name = ViewerDto.first_name,
                last_name = ViewerDto.last_name,
                identity = ViewerDto.identity,
                membership = ViewerDto.membership,
                age = ViewerDto.age
            };
            // SQL Equivalent: Insert into Viewers (..) values (..)

            try
            {
                _context.Viewers.Add(Viewer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Viewer.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Viewer.customer_id;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteViewer(int id)
        {
            ServiceResponse response = new();
            // Viewer must exist in the first place
            var Viewer = await _context.Viewers.FindAsync(id);
            if (Viewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Viewer cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                _context.Viewers.Remove(Viewer);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Viewer");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<IEnumerable<ViewerDto>> ListViewersForMovie(int id)
        {
            // WHERE movie_id == id
            List<Viewer> Viewers = await _context.Viewers
            .Include(v => v.Tickets)
                .Where(v => v.Tickets.Any(t => t.Movie.movie_id == id))
                .ToListAsync();

            // empty list of data transfer object ViewertDto
            List<ViewerDto> ViewerDtos = new List<ViewerDto>();
            // foreach Viewer record in database
            foreach (Viewer Viewer in Viewers)
            {
                // create new instance of ViewerDto, add to list
                ViewerDtos.Add(new ViewerDto()
                {
                    customer_id = Viewer.customer_id,
                    first_name = Viewer.first_name,
                    last_name = Viewer.last_name,
                    identity = Viewer.identity,
                    membership = Viewer.membership,
                    age = Convert.ToInt32(Viewer.age)
                });
            }
            // return 200 OK with ViewerDtos
            return ViewerDtos;
        }

        public async Task<ServiceResponse> RemoveViewerForMovie(int id)
        {

            ServiceResponse response = new();
            
            // WHERE movie_id == id         
            var Viewers = await _context.Viewers
                .Include(v => v.Tickets)
                .Where(v => v.Tickets.Any(t => t.Movie.movie_id == id))
                .ToListAsync();
            if (Viewers == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Viewer cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                // RemoveRange - Reference //stackoverflow.com/questions/30623096/enable-removerange-to-remove-by-predicate-on-entity
                _context.Viewers.RemoveRange(Viewers);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Viewer");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

    }
    
}

