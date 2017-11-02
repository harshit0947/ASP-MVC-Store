using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Store.Models;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Store.Controllers.Api
{
    public class ProductsController : ApiController
    {
        private ProductContext _context;

        public ProductsController()
        {
            _context=new ProductContext();
        }

        //GET /api/products
        public IEnumerable<Product> GetProducts()
        {
                var start = Convert.ToInt32(HttpContext.Current.Request.QueryString["start"]);
                var length = Convert.ToInt32(HttpContext.Current.Request.QueryString["length"]);
                var searchValue = HttpContext.Current.Request.QueryString["search[value]"];
                int colnum = Convert.ToInt32(HttpContext.Current.Request.QueryString["order[0][column]"]);
                string sortDir = HttpContext.Current.Request.QueryString["order[0][dir]"];
                string colName = HttpContext.Current.Request.QueryString["columns[" + colnum + "][data]"];
                Func<Product, Object> orderByFunc = null;

                if (searchValue != "")
                {
                    return _context.Products.Where(p => p.ProductName.Contains(searchValue));
                }
                else
                    switch (colName)
                    {
                        case "id":
                            orderByFunc = p => p.Id;
                            break;

                        case "productName":
                            orderByFunc = p => p.ProductName;
                            break;

                        case "price":
                            orderByFunc = p => p.Price;
                            break;
                    }

                var result = (sortDir.Equals("asc") ? (_context.Products.ToList().OrderBy(orderByFunc).Skip(start).Take(length)) : (_context.Products.ToList().OrderByDescending(orderByFunc).Skip(start).Take(length)));
                return result;
            }

        //GET /api/products/1
        public IHttpActionResult GetProduct(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id);

            if (product==null)
            {
                NotFound();
            }
            return Ok(product);
        }

        //POST /api/prpoducts
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return Created(new Uri(Request.RequestUri+"/"+product.Id), product);
        }

        //PUT /api/products/1
        [System.Web.Http.HttpPut]
        public void UpdateProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var productInDb = _context.Products.SingleOrDefault(p => p.Id == id);

            if (productInDb==null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                productInDb.ProductName = product.ProductName;
                productInDb.Description = product.Description;
                productInDb.Price = product.Price;
                _context.SaveChanges();

            }


        }

        //DELETE api/products/1
        [System.Web.Http.HttpDelete]
        public void DeleteProduct(int id)
        {
            var productInDb = _context.Products.SingleOrDefault(p => p.Id == id);

            if (productInDb == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Products.Remove(productInDb);
            _context.SaveChanges();
        }
    }
}
