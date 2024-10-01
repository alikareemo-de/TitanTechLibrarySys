using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySystem.BusinessLogic.Services
{
    public interface IBorrowingService
    {
        Task AddBorrowingsAsync(int[] bookIds, int userId);

        Task UpdateReturnDateAsync(int borrowingId, DateTime returnDate);
        Task<List<Borrowing>> GetPendingBorrowingsAsync();
        Task UpdateRequestStatusAsync(int borrowingId, BorrowingStatus status);

        Task<List<Borrowing>> GetBorrowingsByUserIdAsync(int userId);
    }
}
