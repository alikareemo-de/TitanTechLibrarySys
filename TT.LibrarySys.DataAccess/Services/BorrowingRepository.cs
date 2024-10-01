using System.Data.SqlClient;


namespace TT.LibrarySys.DataAccess.Models
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly DbConnectionHelper _dbConnectionHelper;
        private readonly IBooksRepository _booksRepository;
        public BorrowingRepository(DbConnectionHelper dbConnectionHelper, IBooksRepository booksRepository)
        {
            _dbConnectionHelper = dbConnectionHelper;
            _booksRepository = booksRepository;

        }

        public async Task AddBorrowingsAsync(int[] bookIds, int userId)
        {

            try
            {
                var books = await _booksRepository.GetAllBooksAsync();
                var availableBooks = books.Where(b => bookIds.Contains(b.BookId) && b.IsAvailable).ToList();

                using (var connection = _dbConnectionHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    foreach (var book in availableBooks)
                    {
                        var query = "INSERT INTO Borrowings (BookId, UserId, BorrowDate, Status) " +
                                    "VALUES (@BookId, @UserId, @BorrowDate, @Status)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@BookId", book.BookId);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@BorrowDate", DateTime.Now);
                            command.Parameters.AddWithValue("@Status", "Pending");

                            await command.ExecuteNonQueryAsync();
                        }

                        book.IsAvailable = false;
                        await _booksRepository.UpdateBookAsync(book);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateReturnDateAsync(int borrowingId, DateTime returnDate)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "UPDATE Borrowings SET ReturnDate = @ReturnDate, Status = 'Returned' WHERE BorrowingId = @BorrowingId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReturnDate", returnDate);
                    command.Parameters.AddWithValue("@BorrowingId", borrowingId);
                    await command.ExecuteNonQueryAsync();
                }

                var bookQuery = "UPDATE Books SET IsAvailable = 1 WHERE BookId = (SELECT BookId FROM Borrowings WHERE BorrowingId = @BorrowingId)";
                using (var bookCommand = new SqlCommand(bookQuery, connection))
                {
                    bookCommand.Parameters.AddWithValue("@BorrowingId", borrowingId);
                    await bookCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Borrowing>> GetBorrowingsByUserIdAsync(int userId)
        {
            var borrowings = new List<Borrowing>();

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT b.BorrowingId, b.UserId, b.BookId, b.BorrowDate, b.ReturnDate, b.Status,
                           k.Title, k.Author, k.ISBN
                    FROM Borrowings b
                    JOIN Books k ON b.BookId = k.BookId
                    WHERE b.UserId = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var borrowing = new Borrowing
                            {
                                BorrowingId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                BookId = reader.GetInt32(2),
                                BorrowDate = reader.GetDateTime(3),
                                ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Status = Enum.TryParse<BorrowingStatus>(reader.GetString(5), out var status) ? status : BorrowingStatus.Pending,

                                Book = new Book
                                {
                                    Title = reader.GetString(6),
                                    Author = reader.GetString(7),
                                    ISBN = reader.GetString(8)
                                }
                            };

                            borrowings.Add(borrowing);
                        }
                    }
                }
            }

            return borrowings.OrderBy(b => b.BorrowDate).ToList();
        }

        public async Task<Borrowing> GetBorrowingByIdAsync(int borrowingId)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"SELECT b.BorrowingId, b.BookId, b.UserId, b.BorrowDate, b.ReturnDate, b.Status, 
                                 bk.Title, bk.Author, bk.ISBN
                          FROM Borrowings b
                          JOIN Books bk ON b.BookId = bk.BookId
                          WHERE b.BorrowingId = @BorrowingId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BorrowingId", borrowingId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Borrowing
                            {
                                BorrowingId = reader.GetInt32(0),
                                BookId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                BorrowDate = reader.GetDateTime(3),
                                ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Status = Enum.TryParse<BorrowingStatus>(reader.GetString(5), out var status) ? status : BorrowingStatus.Pending,
                                Book = new Book
                                {
                                    Title = reader.GetString(6),
                                    Author = reader.GetString(7),
                                    ISBN = reader.GetString(8)
                                }
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<List<Borrowing>> GetPendingBorrowingsAsync()
        {
            var borrowings = new List<Borrowing>();

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT b.BorrowingId, b.UserId, b.BookId, b.BorrowDate, b.ReturnDate, b.Status,
                           k.Title, k.Author, k.ISBN, u.UserName, u.Email
                    FROM Borrowings b
                    JOIN Books k ON b.BookId = k.BookId
                    JOIN Users u ON b.UserId = u.UserId
                    WHERE b.Status = 'Pending'";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var borrowing = new Borrowing
                            {
                                BorrowingId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                BookId = reader.GetInt32(2),
                                BorrowDate = reader.GetDateTime(3),
                                ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Status = Enum.TryParse<BorrowingStatus>(reader.GetString(5), out var status) ? status : BorrowingStatus.Pending,

                                Book = new Book
                                {
                                    Title = reader.GetString(6),
                                    Author = reader.GetString(7),
                                    ISBN = reader.GetString(8)
                                },

                                User = new User
                                {
                                    UserName = reader.GetString(9),
                                    Email = reader.GetString(10)
                                }
                            };

                            borrowings.Add(borrowing);
                        }
                    }
                }
            }

            return borrowings.Where(b => b.Status == BorrowingStatus.Pending).ToList();
        }

        public async Task UpdateRequestStatusAsync(int borrowingId, BorrowingStatus status)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "UPDATE Borrowings SET Status = @Status WHERE BorrowingId = @BorrowingId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status.ToString());
                    command.Parameters.AddWithValue("@BorrowingId", borrowingId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Borrowing>> GetAllBorrowingsAsync()
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"SELECT b.BorrowingId, b.BookId, b.UserId, b.BorrowDate, b.ReturnDate, b.Status, 
                                 bk.Title, bk.Author, bk.ISBN
                          FROM Borrowings b
                          JOIN Books bk ON b.BookId = bk.BookId";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var borrowings = new List<Borrowing>();
                        while (await reader.ReadAsync())
                        {
                            borrowings.Add(new Borrowing
                            {
                                BorrowingId = reader.GetInt32(0),
                                BookId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                BorrowDate = reader.GetDateTime(3),
                                ReturnDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Status = Enum.TryParse<BorrowingStatus>(reader.GetString(5), out var status) ? status : BorrowingStatus.Pending,
                                Book = new Book
                                {
                                    Title = reader.GetString(6),
                                    Author = reader.GetString(7),
                                    ISBN = reader.GetString(8)
                                }
                            });
                        }

                        return borrowings.OrderByDescending(b => b.BorrowDate).ToList();
                    }
                }
            }
        }

    }
}
