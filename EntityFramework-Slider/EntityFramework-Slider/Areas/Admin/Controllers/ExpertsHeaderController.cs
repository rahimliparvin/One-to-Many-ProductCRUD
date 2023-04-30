using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertsHeaderController : Controller
    {
        private readonly AppDbContext _context;
        public ExpertsHeaderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<ExpertsHeader> expertsHeaders = await _context.ExpertsHeaders.OrderByDescending(m => m.Id).ToListAsync();

            return View(expertsHeaders);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertsHeader expertsHeader)
        {

            if (expertsHeader.Title == null || expertsHeader.Description == null)
            {
                return View();
            }

            var dbExpertsHeader = await _context.ExpertsHeaders.FirstOrDefaultAsync(m=>m.Title.Trim().ToLower() == expertsHeader.Title.Trim().ToLower() &&
																					m.Description.Trim().ToLower() == expertsHeader.Description.Trim().ToLower() );

            if(dbExpertsHeader != null)
            {
				ModelState.AddModelError("Title", "This Title already exist");
				ModelState.AddModelError("Description", "This Description already exist");
				return View();
            }

       
            _context.ExpertsHeaders.AddAsync(expertsHeader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return BadRequest();

            var expertsHeader = _context.ExpertsHeaders.FirstOrDefault(m=>m.Id ==id);

            if (expertsHeader == null) return NotFound();

            _context.ExpertsHeaders.Remove(expertsHeader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           
        }


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (id == null) return BadRequest();

			var expertsHeader = _context.ExpertsHeaders.FirstOrDefault(m => m.Id == id);

			if (expertsHeader == null) return NotFound();

            return View(expertsHeader);

		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ExpertsHeader expertsHeader)
        {
			if (id == null) return BadRequest();

            if (expertsHeader.Title == null || expertsHeader.Description == null)
            {
                return View();
            }

            ExpertsHeader dbExpertsHeader = await _context.ExpertsHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

			if (dbExpertsHeader == null) return NotFound();

			if (dbExpertsHeader.Title.Trim().ToLower() == expertsHeader.Title.Trim().ToLower())
			{
				return RedirectToAction(nameof(Index));
			}


			_context.ExpertsHeaders.Update(expertsHeader);

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));

		}


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return BadRequest();

            var dbExpertsHeader = await _context.ExpertsHeaders.FirstOrDefaultAsync(m=>m.Id == id);

            if (dbExpertsHeader == null) return NotFound();

            return View(dbExpertsHeader);
        }
	}
}
