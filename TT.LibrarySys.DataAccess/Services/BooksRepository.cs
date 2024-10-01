using System.Data.SqlClient;

namespace TT.LibrarySys.DataAccess.Models

{
    public class BooksRepository : IBooksRepository
    {
        private readonly DbConnectionHelper _dbConnectionHelper;

        public BooksRepository(DbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task UpdateBookAvailabilityAsync(int bookId, bool isAvailable)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "UPDATE Books SET IsAvailable = @IsAvailable WHERE BookId = @BookId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsAvailable", isAvailable);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT BookId, Title, Author, ISBN, IsAvailable FROM Books";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var books = new List<Book>();
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                ISBN = reader.GetString(3),
                                IsAvailable = reader.GetBoolean(4)
                            });
                        }

                        return books.OrderBy(b => b.Title).ToList();
                    }
                }
            }
        }

        public async Task<List<Book>> GetAvailableBooksAsync()
        {
            var books = new List<Book>();

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM Books WHERE IsAvailable = 1";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                ISBN = reader.GetString(3),
                                IsAvailable = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }

            return books.Where(b => b.IsAvailable).ToList();
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            var books = new List<Book>();

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"SELECT * FROM Books 
                      WHERE Title LIKE @SearchTerm 
                      OR Author LIKE @SearchTerm 
                      OR ISBN LIKE @SearchTerm";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                ISBN = reader.GetString(3),
                                IsAvailable = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }

            return books;
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT BookId, Title, Author, ISBN, IsAvailable FROM Books WHERE BookId = @BookId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookId", bookId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Book
                            {
                                BookId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                ISBN = reader.GetString(3),
                                IsAvailable = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task UpdateBookAsync(Book book)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "UPDATE Books SET IsAvailable = @IsAvailable WHERE BookId = @BookId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);
                    command.Parameters.AddWithValue("@BookId", book.BookId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
