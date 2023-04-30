using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 AppDbContext context,
                                 IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 4)
        {
            List<Product> products = await _productService.GetPaginationDatas(page, take);

            List<ProductListVM> mappedDatas = GetMappedDatas(products);

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductListVM> paginatedDatas = new(mappedDatas,page, pageCount);

			ViewBag.take = take;

			return View(paginatedDatas); 

        }


        private async Task<int> GetPageCountAsync(int take)
        {
            var productCount = await _productService.GetCountAsync();

            return (int)Math.Ceiling((decimal)productCount / take);
        }


        private List<ProductListVM> GetMappedDatas(List<Product> products)
        {

			List<ProductListVM> mappedDatas = new();

			foreach (var product in products)
			{
				ProductListVM productVM = new()
				{
					Id = product.Id,
					Name = product.Name,
					Description = product.Description,
					CategoryName = product.Category.Name,
					Count = product.Count,
					Price = product.Price,
					MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image,

				};

				mappedDatas.Add(productVM);
			}

            return mappedDatas;
		}

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.categories = await GetCategories();

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Create(ProductCreateVM model)
        {

            try
            {
                ViewBag.categories = await GetCategories();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                foreach (var photo in model.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {

                        ModelState.AddModelError("Photos", "File type must be image");
                        return View();

                    }


                    if (photo.CheckFileSize(200))
                    {

                        ModelState.AddModelError("Photos", "Photo size must be max 200Kb");
                        return View();

                    }

                }


                List<ProductImage> productImages = new();


                foreach (var photo in model.Photos)
                {

                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    FileHelper.SaveFileAsync(path,photo);

                    ProductImage productImage = new()
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage);
                }

                productImages.FirstOrDefault().IsMain = true;

                decimal convertedPrice = decimal.Parse(model.Price);

                Product newProduct = new()
                { 
                    Name = model.Name,
                    Description = model.Description,
                    Price = convertedPrice,
                    Count = model.Count,
                    CategoryId = model.CategoryId,
                    Images = productImages
                };

                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                throw;
            }
 
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct == null) return NotFound();

            ViewBag.name = Regex.Replace(dbProduct.Name, "<.*?>", String.Empty);

            ViewBag.desc = Regex.Replace(dbProduct.Description, "<.*?>", String.Empty);

            return View(dbProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {

            Product product = await _productService.GetFullDataById((int)id);

            _context.Products.Remove(product);

            foreach (var item in product.Images)
            {

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img",item.Image);

                FileHelper.DeleteFile(path);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) return BadRequest();

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct == null) return NotFound();

            ViewBag.categories = await GetCategories();

            string convertedPrice = dbProduct.Price.ToString();

            ProductEditVM productEdit = new ProductEditVM()
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Count = dbProduct.Count,
                Price = convertedPrice,
                CategoryId = dbProduct.CategoryId,
                Images = dbProduct.Images
            };

            


            return View(productEdit);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductEditVM model)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct == null) return NotFound();

            ViewBag.categories = await GetCategories();

            if (!ModelState.IsValid)
            {
                ProductEditVM productEditVMproduct = new()
                {
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    Count = dbProduct.Count,
                    Price = dbProduct.Price.ToString(),
                    CategoryId = dbProduct.CategoryId,
                    Images = dbProduct.Images,
                };
          
              return View(productEditVMproduct);
               
            }


   
            List<ProductImage> productImages = new();
                    
            if(model.Photos != null)
            {

                foreach (var item in model.Photos)
                {

                    string fileName = Guid.NewGuid().ToString() + "_" + item.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    FileHelper.SaveFileAsync(path, item);

                    ProductImage productImage = new()
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage);
                }

                productImages.FirstOrDefault().IsMain = true;


            }
           


            decimal convertedPrice = decimal.Parse(model.Price);


            dbProduct.Name = model.Name;
            dbProduct.Description = model.Description;
            dbProduct.Price = convertedPrice;
            dbProduct.Count = model.Count;
            dbProduct.CategoryId = model.CategoryId;
            if(model.Photos == null)
            {
                dbProduct.Images = dbProduct.Images;
                if (dbProduct.Images != null)
                {
                    dbProduct.Images.FirstOrDefault().IsMain = true;
                }
            }
            else
            {
                dbProduct.Images = productImages;
            }
 
            await _context.ProductImages.AddRangeAsync(productImages);
            _context.Products.Update(dbProduct);
            await  _context.SaveChangesAsync();


           return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if(id == null) return BadRequest();

            Product product = await _productService.GetFullDataById(id);

            if (product == null) return NotFound();

            ViewBag.name = Regex.Replace(product.Name, "<.*?>", String.Empty);

            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);

            return View(product);
        }
        


        public async Task<SelectList> GetCategories()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll();
            return new SelectList(categories, "Id", "Name");
        }


        [HttpPost]
        public async Task<IActionResult> PhotoDelete(int id)
        {
            if (id == null) return BadRequest();

            ProductImage productImage = await _context.ProductImages.Include(m=>m.Product).FirstOrDefaultAsync(m=>m.Id == id);

            if (productImage == null) return NotFound();

            Product dbProduct = await _productService.GetFullDataById(productImage.Product.Id);

            if(dbProduct.Images.Count() > 1)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", productImage.Image);

                FileHelper.DeleteFile(path);

                _context.ProductImages.Remove(productImage);

                dbProduct.Images.FirstOrDefault().IsMain = true;

                await _context.SaveChangesAsync();

               
            }
            else
            {
                ModelState.AddModelError("Image", "You can't delete the last picture without adding a new picture");
                return View();
            }

            return Ok();
        }
    }
}
