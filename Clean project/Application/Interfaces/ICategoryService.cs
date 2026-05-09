using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
       public List<CategoryResponseDto> GetAll();
        public CategoryResponseDto GetById(int id);
        public void Create(CategoryDtos dto, int userId);
        public void Update(UpdateCategoryDtos dto, int userId);
        public void Delete(int id,int adminUserId);
    }
}
