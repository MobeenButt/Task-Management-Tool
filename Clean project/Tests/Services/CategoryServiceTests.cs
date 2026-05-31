using Application.DTOs;
using Application.Interfaces;
using Infrastructure.CategoryServices;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using Xunit;

namespace Tests.Services
{
    public class CategoryServiceTests : IDisposable
    {
        private readonly Infrastructure.ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _categoryService = new CategoryService(_context);
        }

        [Fact]
        public void GetAll_ShouldReturnAllCategories()
        {
            // Act
            var result = _categoryService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Work");
            Assert.Contains(result, c => c.Name == "Personal");
        }

        [Fact]
        public void GetById_WithValidId_ShouldReturnCategory()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = _categoryService.GetById(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Work", result.Name);
            Assert.Equal("Work related tasks", result.Description);
        }

        [Fact]
        public void GetById_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            int invalidId = 999;

            // Act & Assert
            Assert.Throws<Exception>(() => _categoryService.GetById(invalidId));
        }

        [Fact]
        public void Create_WithValidData_ShouldCreateCategory()
        {
            // Arrange
            var createDto = new CategoryDtos
            {
                Name = "Health",
                Description = "Health and fitness tasks"
            };
            int adminUserId = 1;

            // Act
            _categoryService.Create(createDto, adminUserId);

            // Assert
            var allCategories = _categoryService.GetAll();
            Assert.Equal(3, allCategories.Count);
            Assert.Contains(allCategories, c => c.Name == "Health");
        }

        [Fact]
        public void Create_WithDuplicateName_ShouldThrowException()
        {
            // Arrange
            var createDto = new CategoryDtos
            {
                Name = "Work", // Already exists
                Description = "Duplicate work category"
            };
            int adminUserId = 1;

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _categoryService.Create(createDto, adminUserId));
            Assert.Equal("Category already exists", exception.Message);
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateCategory()
        {
            // Arrange
            var updateDto = new UpdateCategoryDtos
            {
                Id = 1,
                Name = "Updated Work",
                Description = "Updated work description"
            };
            int adminUserId = 1;

            // Act
            _categoryService.Update(updateDto, adminUserId);

            // Assert
            var updatedCategory = _categoryService.GetById(1);
            Assert.Equal("Updated Work", updatedCategory.Name);
            Assert.Equal("Updated work description", updatedCategory.Description);
        }

        [Fact]
        public void Update_WithDuplicateName_ShouldThrowException()
        {
            // Arrange
            var updateDto = new UpdateCategoryDtos
            {
                Id = 1,
                Name = "Personal", // Already exists as another category
                Description = "Trying to use existing name"
            };
            int adminUserId = 1;

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _categoryService.Update(updateDto, adminUserId));
            Assert.Equal("Category name already exists", exception.Message);
        }

        [Fact]
        public void Update_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            var updateDto = new UpdateCategoryDtos
            {
                Id = 999,
                Name = "Non-existent",
                Description = "This category doesn't exist"
            };
            int adminUserId = 1;

            // Act & Assert
            Assert.Throws<Exception>(() => _categoryService.Update(updateDto, adminUserId));
        }

        [Fact]
        public void Delete_WithValidId_ShouldSoftDeleteCategory()
        {
            // Arrange
            int categoryId = 1;
            int adminUserId = 1;

            // Act
            _categoryService.Delete(categoryId, adminUserId);

            // Assert
            Assert.Throws<Exception>(() => _categoryService.GetById(categoryId));
            
            // Verify category still exists in database but marked as deleted
            var categoryInDb = _context.Categories.IgnoreQueryFilters().FirstOrDefault(c => c.Id == categoryId);
            Assert.NotNull(categoryInDb);
            Assert.True(categoryInDb.IsDeleted);
        }

        [Fact]
        public void Delete_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            int invalidId = 999;
            int adminUserId = 1;

            // Act & Assert
            Assert.Throws<Exception>(() => _categoryService.Delete(invalidId, adminUserId));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}