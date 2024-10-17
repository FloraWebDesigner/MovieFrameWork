using Microsoft.AspNetCore.Mvc;
using MyMovie.Models;
using MyMovie.Interface;
using MyMovie.Models.ViewModels;
using MyMovie.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyMovie.Controllers
{

    public class MoviePageController : Controller
    {
        private readonly MovieInterface _MovieService;
        private readonly ViewerInterface _ViewerService;
        private readonly TicketInterface _TicketService;

        // dependency injection of service interface
        public MoviePageController(MovieInterface MovieService, ViewerInterface ViewerService, TicketInterface TicketService)
        {
            _MovieService = MovieService;
            _ViewerService = ViewerService;
            _TicketService = TicketService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MoviePage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<MovieDto?> MovieDtos = await _MovieService.ListMovies();
            return View(MovieDtos);
        }

        // GET: MoviePage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);


            if (MovieDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }

            IEnumerable<ViewerDto> AssociatedViewers = await _ViewerService.ListViewersForMovie(id);
            if (AssociatedViewers == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Viewers"] });
            }
            IEnumerable<TicketDto> AssociatedTickets = await _TicketService.ListTicketsForMovie(id);
            if (AssociatedTickets == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Tickets"] });
            }


            // information which drives a movie page
            MovieDetails MovieInfo = new MovieDetails()
                {
                    Movie = MovieDto,
                    MovieViewers = AssociatedViewers,
                    MovieTickets = AssociatedTickets
                };
                return View(MovieInfo);
            }


        // GET MoviePage/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }


        // POST MoviePage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(MovieDto MovieDto)
        {
            ServiceResponse response = await _MovieService.AddMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "MoviePage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);
            if (MovieDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(MovieDto);
            }
        }

        //POST MoviePage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, MovieDto MovieDto)
        {
            ServiceResponse response = await _MovieService.UpdateMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "MoviePage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);
            if (MovieDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(MovieDto);
            }
        }

        //POST MoviePage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _MovieService.DeleteMovie(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MoviePage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
