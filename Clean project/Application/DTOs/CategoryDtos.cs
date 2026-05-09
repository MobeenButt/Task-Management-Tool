using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CategoryDtos
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateCategoryDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByUsername { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
}
