using MyMovie.Models;

namespace MyMovie.Interface
{
    public interface ViewerInterface
    {
        // definitions for implementations of actions to create, read, update, delete
        // base CRUD
        Task<IEnumerable<ViewerDto>> ListViewers();

        Task<ViewerDto?> FindViewer(int id);

        Task<ServiceResponse> UpdateViewer(ViewerDto ViewerDto);

        Task<ServiceResponse> AddViewer(ViewerDto ViewerDto);

        Task<ServiceResponse> DeleteViewer(int id);

        Task<IEnumerable<ViewerDto>> ListViewersForMovie(int id);

        Task<ServiceResponse> RemoveViewerForMovie(int id);
    }
}
