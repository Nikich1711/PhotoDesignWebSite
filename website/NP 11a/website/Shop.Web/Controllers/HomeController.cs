using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.DataMapper;
using Shop.Web.Models;
using Shop.Web.Models.Category;
using Shop.Web.Models.GraphicPhoto;
using Shop.Web.Models.Home;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shop.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IGraphicPhoto _GraphicPhotoService;
        private readonly Mapper _mapper;

        public HomeController(IGraphicPhoto GraphicPhotoService)
        {
            _GraphicPhotoService = GraphicPhotoService;
            _mapper = new Mapper();
        }

        [Route("/")]
        public IActionResult Index()
        {
            var preferedGraphicPhotos = _GraphicPhotoService.GetPreferred(10);
            var model = _mapper.GraphicPhotosToHomeIndexModel(preferedGraphicPhotos);
            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery) || string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("Index");
            }

            var searchedGraphicPhotos = _GraphicPhotoService.GetFilteredGraphicPhotos(searchQuery);
            var model = _mapper.GraphicPhotosToHomeIndexModel(searchedGraphicPhotos);

            return View(model);
        }
    }
}