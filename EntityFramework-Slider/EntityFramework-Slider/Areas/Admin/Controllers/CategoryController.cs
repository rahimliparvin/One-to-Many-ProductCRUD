﻿using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly  ICategoryService _categoryService;
        private readonly AppDbContext _context;

        public CategoryController(ICategoryService categoryService, 
                                  AppDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1 , int take = 5 )
        {

			List<Category> categories = await _categoryService.GetPaginationDatas(page, take);

			List<CategoryListVM> mappedDatas = GetMappedDatas(categories);

			int pageCount = await GetPageCountAsync(take);

			Paginate<CategoryListVM> paginatedDatas = new(mappedDatas, page, pageCount);

			ViewBag.take = take;

			return View(paginatedDatas);
		}


		private async Task<int> GetPageCountAsync(int take)
		{
			var categoryCount = await _categoryService.GetCountAsync();

			return (int)Math.Ceiling((decimal)categoryCount / take);
		}


		private List<CategoryListVM> GetMappedDatas(List<Category> categories)
		{

			List<CategoryListVM> mappedDatas = new();

			foreach (var category in categories)
			{
				CategoryListVM categoryVM = new()
				{
					Id = category.Id,
					Name = category.Name
				};

				mappedDatas.Add(categoryVM);
			}

		 	return mappedDatas;
		}
	

        [HttpGet]
		public IActionResult Create()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

                var existData = await _context.Categories.FirstOrDefaultAsync(m => m.Name.Trim().ToLower() ==
                                                                         category.Name.Trim().ToLower());

                if (existData != null)
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                //int num = 1;
                //int num2 = 0;

                //var result = num / num2;


                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
          
        }

        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


		[HttpPost]
		public async Task<IActionResult> SoftDelete(int? id)
		{
			if (id == null) return BadRequest();

			Category category = await _context.Categories.FindAsync(id);

			if (category == null) return NotFound();

            category.SoftDelete = true;

			await _context.SaveChangesAsync();

            return Ok();

		}



		[HttpGet]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return BadRequest();

			Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();
 
            return View(category);

		}


        [HttpPost]
        public async Task<IActionResult> Edit(int? id,Category category)
        {

			if (!ModelState.IsValid)
			{
				return View();
			}



			if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m=>m.Id == id);

            if (dbCategory == null) return NotFound();

            if (dbCategory.Name.Trim().ToLower() == category.Name.Trim().ToLower())
            {
                return RedirectToAction(nameof(Index));
            }

            // dbCategory.Name = category.Name;

            _context.Categories.Update(category);
              
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            return View(category);

        }


    }
}
 