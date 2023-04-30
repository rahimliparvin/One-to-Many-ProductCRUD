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
    public class SliderInfoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        public SliderInfoController(AppDbContext context,
                                    IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {

            List<SliderInfo> sliderInfos = await _context.SliderInfos.ToListAsync();

            return View(sliderInfos);
            
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!sliderInfo.Photo.CheckFileType("image/"))
            {

                ModelState.AddModelError("Photo", "File type must be image");
                return View();

            }


            if (sliderInfo.Photo.CheckFileSize(200))
            {

                ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
                return View();

            }

            string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName;

            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new(path, FileMode.Create))
            {
                await sliderInfo.Photo.CopyToAsync(stream);
            }

            sliderInfo.SignatureImage = fileName;

            await _context.SliderInfos.AddAsync(sliderInfo);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (id == null) return BadRequest();

			var sliderInfo = _context.SliderInfos.FirstOrDefault(m => m.Id == id);

			if (sliderInfo == null) return NotFound();

			return View(sliderInfo);

		}


		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id , SliderInfo sliderInfo)
        {


			if (id == null) return BadRequest();

			SliderInfo dbSliderInfo = await _context.SliderInfos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

			if (dbSliderInfo == null) return NotFound();

			if (!ModelState.IsValid)
			{
				return View(dbSliderInfo);
			}

			if (!sliderInfo.Photo.CheckFileType("image/"))
			{

				ModelState.AddModelError("Photo", "File type must be image");
				return View(dbSliderInfo);

			}


			if (sliderInfo.Photo.CheckFileSize(200))
			{

				ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
				return View(dbSliderInfo);

			}

			string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);

			FileHelper.DeleteFile(oldPath);

			string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName;

			string newPath = Path.Combine(_env.WebRootPath, "img", fileName);

			await FileHelper.SaveFileAsync(newPath, sliderInfo.Photo);

			sliderInfo.SignatureImage = fileName;

			_context.SliderInfos.Update(sliderInfo);

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));



		}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            if (id == null) return BadRequest();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo == null) return NotFound();


            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", sliderInfo.SignatureImage);


            FileHelper.DeleteFile(path);

            _context.SliderInfos.Remove(sliderInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo == null) return NotFound();

            return View(sliderInfo);

        }
    }
}
