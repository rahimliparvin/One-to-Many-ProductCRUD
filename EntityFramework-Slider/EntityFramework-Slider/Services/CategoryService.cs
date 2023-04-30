﻿using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAll() => await _context.Categories.ToListAsync();
     
		public async Task<int> GetCountAsync() => await _context.Categories.CountAsync();

		public async Task<List<Category>> GetPaginationDatas(int page, int take) => await _context.Categories.Skip((page * take) - take).Take(take).ToListAsync();

	}
}
