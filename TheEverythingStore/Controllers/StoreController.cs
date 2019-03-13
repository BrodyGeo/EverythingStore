using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheEverythingStore.Models;

namespace TheEverythingStore.Controllers
{
    public class StoreController : Controller
    {
        /* Database conneciton */
        private DbModel db = new DbModel();

        // GET: Store
        public ActionResult Index()
        {
            var categories = db.Categories.OrderBy(c => c.Name).ToList();
            return View(categories);
        }

        /* Return the products of the selected category 
         * Path: /Store/Products/CategoryId
         */
        public ActionResult Products(int id)
        {
            var products = db.Products.Where(p => p.CategoryId == id).OrderBy(p => p.Name).ToList();
            return View(products);
        }

        /* Add the item to the cart
         * Path: /Store/AddToCart/ProductId
         */
        public ActionResult AddToCart(int id)
        {
            /* Identify the user */
            GetCartUsername();

            Product product = db.Products.SingleOrDefault(p => p.ProductId == id);

            Cart cart = new Cart();
            cart.ProductId = id;
            cart.Quantity = 1;
            cart.Price = product.Price;
            cart.Username = Session["CartUsername"].ToString();

            /* Save to database */
            db.Carts.Add(cart);
            db.SaveChanges();

            return RedirectToAction("ShoppingCart");
        }

        private void GetCartUsername()
        {
            /* Do we already have a cart id for this session? */
            if(Session["CartUsername"] == null)
            {
                /* Is the user logged in? */
                if(User.Identity.Name == "")
                {
                    Session["CartUsername"] = Guid.NewGuid();
                }
                else
                {
                    Session["CartUsername"] = User.Identity.Name;
                }
            }
        }

        /*  Gets the shopping cart
         * Path: /Store/ShopingCart
         */
        public ActionResult ShoppingCart()
        {
            GetCartUsername();
            String Username = Session["CartUsername"].ToString();

            var cartItems = db.Carts.Where(ci => ci.Username == Username).ToList();

            return View(cartItems);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            return View();
        }

        /* POST Store/Checkout */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(FormCollection values)
        {
            Order order = new Order();

            TryUpdateModel(order);

            order.OrderDate = DateTime.Now;
            order.UserId = User.Identity.Name;

            /* Get total */
            decimal cartTotal = 0;
            var cartItems = db.Carts.Where(ci => ci.Username == User.Identity.Name).ToList();

            /* Add all products in the carts prices together */
            cartTotal = (from c in cartItems
                         select (int)c.Quantity * c.Price).Sum();

            /* Add tax */
            cartTotal = cartTotal * (decimal)1.13;

            order.Total = cartTotal;

            /* Save the order */
            db.Orders.Add(order);
            db.SaveChanges();

            /* Save the order detail */
            foreach (Cart item in cartItems)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();

            /* Clear the cart */
            foreach(Cart item in cartItems)
            {
                db.Carts.Remove(item);
            }
            db.SaveChanges();

            return RedirectToAction("Details", "Orders", new { id = order.OrderId });
        }


    }
}