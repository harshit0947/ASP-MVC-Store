using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Store.Models;

namespace Store.Controllers
{
    public class ProductController : Controller
    {
        private ProductContext _context;
        static List<Product> _products = new List<Product>();

        public ProductController()
        {
            _context = new ProductContext();
        }

        // GET: Product
        public ActionResult Index()
        {
            var productInDb= _context.Products.ToList();
            return View(productInDb);
        }

        public ActionResult New()
        {
            return View("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View("Add");
            }
            else
            {
                var idExist = _context.Products.SingleOrDefault(p => p.Id == product.Id);
                if (idExist != null)
                {
                    return HttpNotFound();
                }
                else
                {
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Product");
                }
                  
            }
          
        }

        public ActionResult DeleteProduct(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id );
            if (product==null)
            {
                return HttpNotFound();
            }
            else
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        public ActionResult UpdateProduct(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id);
            if (product==null)
            {
                return HttpNotFound();
            }
            else
            return View("UpdateForm", product);
        }

        [HttpPost]
        public ActionResult SaveUpdate(Product product)
        {
            var productInDb = _context.Products.Single(p => p.Id == product.Id);

            productInDb.Description = product.Description;
            productInDb.Price = product.Price;
            productInDb.ProductName = product.ProductName;

            _context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        public ActionResult Description(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id);
            return View("Description", product);
        }

        public ActionResult Customer()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public ActionResult AddCart(int id)
        {
            var listproduct = _products.FirstOrDefault(p => p.Id == id);

            if (listproduct != null)
            {
                if (listproduct.Id == id)
                {
                    listproduct.QuantityInCart += 1;
                }

            }
            else
            {
                Product product = _context.Products.SingleOrDefault(p => p.Id == id);
                product.QuantityInCart = 1;
                _products.Add(product);
            }
            TempData["products"] = _products;
            return RedirectToAction("Customer","Product");
        }

        public ActionResult RemoveCart(int id)
        {
            List<Product> cartList = TempData.Peek("products") as List<Product>;
            Product prod = cartList.Where(e => e.Id == id).FirstOrDefault();

            if (prod.QuantityInCart>1)
            {
                prod.QuantityInCart -= 1;
            }
            else
            {
                cartList.Remove(prod);
            }
            _products = cartList;
            TempData["products"] = _products;
            return View("Cart");
        }

        public ActionResult Cart()
        {
            return View();
        }
    }
}