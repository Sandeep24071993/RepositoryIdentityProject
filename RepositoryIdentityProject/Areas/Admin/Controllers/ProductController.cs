
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepositoryIdentity.DataAccess.Data;
using RepositoryIdentity.DataAccess.Repository;
using RepositoryIdentity.DataAccess.Repository.IRepository;
using RepositoryIdentity.Models.Models;
using RepositoryIdentity.Models.Models.ViewModels;
using RepositoryIdentity.Utility;

namespace RepositoryIdentityWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Roll_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   
        }
        public IActionResult Index()
        {
            var data = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(data);
        }

        public IActionResult Upsert(int? id) // Update and Insert both
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product() // both are properties CategoryList and Product of ProductVM
            };
            if (id==null || id==0)
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
            }
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);    
                    string productPath = Path.Combine(wwwRootPath,@"Images\Product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete the old image 
                        var oldImagePath = Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                           System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(filestream);    
                    }
                    productVM.Product.ImageUrl = @"\Images\Product\" + fileName;   
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
             
                _unitOfWork.Save();
                TempData["success"] = "Product Created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                 return View(productVM);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.Product.Get(o => o.Id == id);
            //Product? product1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //Product? product2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? product = _unitOfWork.Product.Get(o => o.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product Delete successfully";
            return RedirectToAction("Index");
        }
    }
}
