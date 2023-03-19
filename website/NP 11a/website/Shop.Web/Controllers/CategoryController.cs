using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.Models.Category;
using Shop.Web.Models.GraphicPhoto;
using System.Linq;
using Shop.Web.DataMapper;

namespace Shop.Web.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategory _categoryService;
		private readonly IGraphicPhoto _GraphicPhotoService;
		private readonly Mapper _mapper;

		public CategoryController(ICategory categoryService, IGraphicPhoto GraphicPhotoService)
		{
			_categoryService = categoryService;
			_GraphicPhotoService = GraphicPhotoService;
			_mapper = new Mapper();
		}

		public IActionResult Index()
		{
			var categories = _categoryService.GetAll().
				Select(category => new CategoryListingModel
				{
					Name = category.Name,
					Description = category.Description,
					Id = category.Id,
					ImageUrl = category.ImageUrl
				});

			var model = new CategoryIndexModel
			{
				CategoryList = categories
			};

			return View(model);
		}

		public IActionResult Topic(int id, string searchQuery)
		{
			var category = _categoryService.GetById(id);
			var GraphicPhotos = _GraphicPhotoService.GetFilteredGraphicPhotos(id, searchQuery);

			var GraphicPhotoListings = GraphicPhotos.Select(GraphicPhoto => new GraphicPhotoListingModel
			{
				Id = GraphicPhoto.Id,
				Name = GraphicPhoto.Name,
				InStock = GraphicPhoto.InStock,
				Price = GraphicPhoto.Price,
				ShortDescription = GraphicPhoto.ShortDescription,
				Category = _mapper.GraphicPhotoToCategoryListing(GraphicPhoto),
				ImageUrl = GraphicPhoto.ImageUrl
			});

			var model = new CategoryTopicModel
			{
				Category = _mapper.CategoryToCategoryListing(category),
				GraphicPhotos = GraphicPhotoListings
			};

			return View(model);
		}

		public IActionResult Search(int id, string searchQuery)
		{
			return RedirectToAction("Topic", new { id, searchQuery });
		}

		[Authorize(Roles = "Admin")]
		public IActionResult New()
		{
			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Create Category";
			return View("CreateEdit");
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult New(CategoryListingModel model)
		{
			if (ModelState.IsValid)
			{
				var category = _mapper.CategoryListingToModel(model);
				_categoryService.NewCategory(category);
				return RedirectToAction("Topic", new { id = category.Id, searchQuery = "" });
			}

			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Create Category";

			return View("CreateEdit", model);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Edit(int id)
		{
			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Save Changes";
			ViewBag.RouteId = id;

			var category = _categoryService.GetById(id);
			if (category != null)
			{
				var model = _mapper.CategoryToCategoryListing(category);
				return View("CreateEdit", model);
			}

			return View("CreateEdit");
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(CategoryListingModel model)
		{
			if (ModelState.IsValid)
			{
				var category = _mapper.CategoryListingToModel(model);
				_categoryService.EditCategory(category);
				return RedirectToAction("Topic", new { id = category.Id, searchQuery = "" });
			}

			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Save Changes";
			ViewBag.RouteId = model.Id;

			return View("CreateEdit", model);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
		{
			_categoryService.DeleteCategory(id);

			return RedirectToAction("Index");
		}
	}
}