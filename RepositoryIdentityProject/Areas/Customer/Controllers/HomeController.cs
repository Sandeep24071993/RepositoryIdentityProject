using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using RepositoryIdentity.Models.Models;
using RepositoryIdentity.DataAccess.Repository.IRepository;

namespace RepositoryIdentityWeb.Areas.Customer.Controllers
{
    // BootstrapWatch and IconBootstrap website
    // For the areas Projectname -> add -> New scafold item -> Mvc area -> areas name (Area -> add -> area)
    // Add attribute area Admine and customer in exist controller 
    // For Identity -> install identity package in data access and use IdentityDbContext in the place of DbContext
    // And use base.onmodelcreating(Modelbuilder modelbuilder); in IdentityDbContext
    // And add all indentity framework in web project -> Add -> New Scaffold item -> Identity -> take Db Context class and check all override files
    // and search applicationdbcontext in solution explrer and copy inherite class and paste in place of --
    // -- DbContex in data access project and then delete identityDbContext class which is newcreate ok 
    // goto programe.cs remove email part if you no email need comfirmation and add authentication before authorization
    // And goto appsetting remove second connectin string which is make by identity
    // Register in services (called or say basic configuration or tells application) AddRazorPages(); and also add app.MapRazorpages(); in middle wear pipeline for routing
    // And also add ApplicationUser class in register.cs in createuser() method
    // For Role Manager add in programe.cs IdentityRole ..
    // And Also add Managerrole in register.cs services ans dependencies and many more related role ...
    // So all role for inclued in database so use in register in onget() method add
    // When create error related by email sender then add EmailSender class in Utility project and


    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork  _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(productList);
        }
        public IActionResult Details(int? ProductId)
        {
            Product product = _unitOfWork.Product.Get(u=>u.Id== ProductId, includeProperties: "Category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
