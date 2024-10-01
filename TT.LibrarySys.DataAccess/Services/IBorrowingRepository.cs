
namespace TT.LibrarySys.DataAccess.Models
{
    public interface IBorrowingRepository
    {
        Task AddBorrowingsAsync(int[] bookIds, int userId);

        Task<List<Borrowing>> GetBorrowingsByUserIdAsync(int userId);
        Task<List<Borrowing>> GetAllBorrowingsAsync();
        Task<Borrowing> GetBorrowingByIdAsync(int borrowingId);
        Task UpdateReturnDateAsync(int borrowingId, DateTime returnDate);
        Task<List<Borrowing>> GetPendingBorrowingsAsync();
        Task UpdateRequestStatusAsync(int borrowingId, BorrowingStatus status);

    }
}
