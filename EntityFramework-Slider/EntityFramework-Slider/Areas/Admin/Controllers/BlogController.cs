using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(AppDbContext context,
                              IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

		
		public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _context.Blogs.ToListAsync();

            return View(blogs);
        }

 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!blog.Photo.CheckFileType("image/"))
            {

                ModelState.AddModelError("Photo", "File type must be image");
                return View();

            }

            if (blog.Photo.CheckFileSize(200))
            {

                ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
                return View();

            }
          
            string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;

            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new(path, FileMode.Create))
            {
                await blog.Photo.CopyToAsync(stream);
            }

            blog.Image = fileName;

            var dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Header.Trim().ToLower() == blog.Header.Trim().ToLower());

            if (dbBlog != null)
            {
                ModelState.AddModelError("Header", "This Title already exist");
                return View();
            }


            _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null) return NotFound();

            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);

            FileHelper.DeleteFile(path);

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }



		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (id == null) return BadRequest();

			var dbBlog = _context.Blogs.FirstOrDefault(m => m.Id == id);

			if (dbBlog == null) return NotFound();

			return View(dbBlog);

		}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog newBlog)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(blog);
            }

            if (!newBlog.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type must be image");
                return View(blog);
            }


            if (newBlog.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
                return View(blog);
            }

            string fileName = Guid.NewGuid().ToString() + "_" + newBlog.Photo.FileName;

            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new(path, FileMode.Create))
            {
                await newBlog.Photo.CopyToAsync(stream);
            }

            string expath = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);

            FileHelper.DeleteFile(expath);

            newBlog.Image = fileName;

            _context.Blogs.Update(newBlog);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }



        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return BadRequest();

            var dbBlogs = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

            if (dbBlogs == null) return NotFound();

            return View(dbBlogs);
        }
    }
}
