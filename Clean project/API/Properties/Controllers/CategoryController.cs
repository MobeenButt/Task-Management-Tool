using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace API.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admin can access
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found in token."));
        }

        /// Get all categories (Admin only)
        
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllCategories()
        {
            try
            {
                var categories = _categoryService.GetAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// Get category by ID (Admin only)
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetCategory(int id)
        {
            try
            {
                var category = _categoryService.GetById(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }

       
        /// Create new category (Admin only)
      
        [HttpPost]
        [Route("create")]
        public IActionResult CreateCategory([FromBody] CategoryDtos dto)
        {
            try
            {
                var adminUserId = GetUserId();
                _categoryService.Create(dto, adminUserId);
                return Ok(new { message = "Category created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// Update category (Admin only)
        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] UpdateCategoryDtos dto)
        {
            try
            {
                var adminUserId = GetUserId();
                dto.Id = id;
                _categoryService.Update(dto, adminUserId);
                return Ok(new { message = "Category updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// Delete category (Admin only - Soft Delete)
        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var adminUserId = GetUserId();
                _categoryService.Delete(id, adminUserId);
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        /// Get all active categories (All authenticated users)
        [AllowAnonymous] // Or just [Authorize] without role restriction
        [HttpGet]
        [Route("list")]
        public IActionResult GetCategoriesList()
        {
            try
            {
                var categories = _categoryService.GetAll();
                return Ok(categories.Select(c => new { c.Id, c.Name, c.Description }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
