using EntityFramework_Slider.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<int> GetCountAsync();
        Task<List<Category>> GetPaginationDatas(int page, int take);


	}
}
