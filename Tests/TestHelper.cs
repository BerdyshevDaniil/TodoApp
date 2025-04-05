using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

namespace TodoApp.Tests
{
    public static class TestHelper
    {
        public static TodoContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseSqlite("DataSource=:memory:") // In-memory SQLite
                .Options;

            var context = new TodoContext(options);
            context.Database.OpenConnection(); // Необходимо для in-memory SQLite
            context.Database.EnsureCreated(); // Создает схему базы данных
            return context;
        }
    }
}