using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.Models.ShoppingCart;

namespace Shop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IGraphicPhoto _GraphicPhotoService;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(IGraphicPhoto GraphicPhotoService, ShoppingCart shoppingCart)
        {
            _GraphicPhotoService = GraphicPhotoService;
            _shoppingCart = shoppingCart;
        }

        public IActionResult Index(bool isValidAmount = true, string returnUrl = "/")
        {
            _shoppingCart.GetShoppingCartItems();

            var model = new ShoppingCartIndexModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal(),
                ReturnUrl = returnUrl
            };

            if (!isValidAmount)
            {
                ViewBag.InvalidAmountText = "*There were not enough items in stock to add*";
            }

            return View("Index", model);
        }

        [HttpGet]
        [Route("/ShoppingCart/Add/{id}/{returnUrl?}")]
        public IActionResult Add(int id, int? amount = 1, string returnUrl = null)
        {
            var GraphicPhoto = _GraphicPhotoService.GetById(id);
            returnUrl = returnUrl.Replace("%2F", "/");
            bool isValidAmount = false;
            if (GraphicPhoto != null)
            {
                isValidAmount = _shoppingCart.AddToCart(GraphicPhoto, amount.Value);
            }

            return Index(isValidAmount, returnUrl);
        }

        public IActionResult Remove(int GraphicPhotoId)
        {
            var GraphicPhoto = _GraphicPhotoService.GetById(GraphicPhotoId);
            if (GraphicPhoto != null)
            {
                _shoppingCart.RemoveFromCart(GraphicPhoto);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Back(string returnUrl = "/")
        {
            return Redirect(returnUrl);
        }
    }
}