using Application.DTOs;
using Application.Interfaces;
using Domain.Entites;

namespace Infrastructure.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CategoryResponseDto> GetAll()
        {
            return _context.Categories
                .Join(_context.Users,
                    category => category.CreatedBy,
                    user => user.Id,
                    (category, user) => new CategoryResponseDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        CreatedBy = category.CreatedBy,
                        CreatedByUsername = user.Username,
                        CreatedDate = category.CreatedDate
                    })
                .ToList();
        }

        public CategoryResponseDto GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                throw new Exception("Category not found");

            var user = _context.Users.FirstOrDefault(u => u.Id == category.CreatedBy);

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedBy = category.CreatedBy,
                CreatedByUsername = user?.Username,
                CreatedDate = category.CreatedDate
            };
        }

        public void Create(CategoryDtos dto, int adminUserId)
        {
            // Check if category already exists
            var existingCategory = _context.Categories
                .FirstOrDefault(c => c.Name.ToLower() == dto.Name.ToLower());

            if (existingCategory != null)
                throw new Exception("Category already exists");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = adminUserId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(UpdateCategoryDtos dto, int adminUserId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == dto.Id);
            if (category == null)
                throw new Exception("Category not found");

            // Check if new name already exists (excluding current category)
            var existingCategory = _context.Categories
                .FirstOrDefault(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != dto.Id);

            if (existingCategory != null)
                throw new Exception("Category name already exists");

            category.Name = dto.Name;
            category.Description = dto.Description;

            _context.SaveChanges();
        }

        public void Delete(int id, int adminUserId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                throw new Exception("Category not found");

            // Soft delete
            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.DeletedBy = adminUserId;

            _context.SaveChanges();
        }
    }
}
