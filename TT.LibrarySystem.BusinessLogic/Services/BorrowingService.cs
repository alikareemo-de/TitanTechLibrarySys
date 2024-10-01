using TT.LibrarySys.DataAccess.Models;

namespace TT.LibrarySystem.BusinessLogic.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowingRepository _borrowingRepository;

        public BorrowingService(IBorrowingRepository borrowingRepository)
        {

            _borrowingRepository = borrowingRepository;
        }
        public async Task UpdateReturnDateAsync(int borrowingId, DateTime returnDate)
        {
            await _borrowingRepository.UpdateReturnDateAsync(borrowingId, returnDate);
        }

        public async Task<List<Borrowing>> GetPendingBorrowingsAsync()
        {
            var borrowings = await _borrowingRepository.GetPendingBorrowingsAsync();
            return borrowings;
        }
        public async Task UpdateRequestStatusAsync(int borrowingId, BorrowingStatus status)
        {
            await _borrowingRepository.UpdateRequestStatusAsync(borrowingId, status);
        }
        public async Task AddBorrowingsAsync(int[] bookIds, int userId)
        {
            await _borrowingRepository.AddBorrowingsAsync(bookIds, userId);
        }
        public async Task<List<Borrowing>> GetBorrowingsByUserIdAsync(int userId) => await _borrowingRepository.GetBorrowingsByUserIdAsync(userId);

    }
}
