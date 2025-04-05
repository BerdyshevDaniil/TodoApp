using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Controllers;
using TodoApp.Data;
using TodoApp.Models;
using Xunit;

namespace TodoApp.Tests
{
    public class TodoControllerTests : IDisposable
    {
        private readonly TodoContext _context;

        public TodoControllerTests()
        {
            _context = TestHelper.CreateInMemoryDbContext();
        }

        public void Dispose()
        {
            _context.Database.CloseConnection(); // Закрываем соединение после каждого теста
            _context.Dispose();
        }

        [Fact]
        public async Task GetTodoItems_ReturnsEmptyList_WhenNoItemsExist()
        {
            // Arrange
            var controller = new TodoController(_context);

            // Act
            var result = await controller.GetTodoItems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TodoItem>>>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(actionResult.Value);
            Assert.Empty(items);
        }

        [Fact]
        public async Task GetTodoItems_ReturnsItems_WhenItemsExist()
        {
            // Arrange
            var controller = new TodoController(_context);
            var todoItem = new TodoItem { Id = 1, Title = "Test Task", IsCompleted = false };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.GetTodoItems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TodoItem>>>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(actionResult.Value);
            var itemList = items.ToList();
            Assert.Single(itemList);
            Assert.Equal("Test Task", itemList[0].Title);
        }

        [Fact]
        public async Task PostTodoItem_CreatesItem_WhenValidItem()
        {
            // Arrange
            var controller = new TodoController(_context);
            var newItem = new TodoItem { Title = "New Task", IsCompleted = false };

            // Act
            var result = await controller.PostTodoItem(newItem);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdItem = Assert.IsType<TodoItem>(createdResult.Value);
            Assert.Equal("New Task", createdItem.Title);
            Assert.Equal(1, createdItem.Id); // Проверяем, что Id сгенерирован

            // Проверяем, что элемент сохранен в базе
            var itemInDb = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == createdItem.Id);
            Assert.NotNull(itemInDb);
            Assert.Equal("New Task", itemInDb.Title);
        }
    }
}