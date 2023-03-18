using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.DataMapper;
using Shop.Web.Models.Category;
using Shop.Web.Models.GraphicPhoto;
using System;
using System.Linq;

namespace Shop.Web.Controllers
{
	public class GraphicPhotoController : Controller
	{
		private readonly ICategory _categoryService;
		private readonly IGraphicPhoto _GraphicPhotoService;
		private readonly Mapper _mapper;

		public GraphicPhotoController(ICategory categoryService, IGraphicPhoto GraphicPhotoService)
		{
			_categoryService = categoryService;
			_GraphicPhotoService = GraphicPhotoService;
			_mapper = new Mapper();
		}

		[Route("GraphicPhotos/{id}")]
		public IActionResult Index(int id)
		{
			var GraphicPhoto = _GraphicPhotoService.GetById(id);

			var model = new GraphicPhotoIndexModel
			{
				Id = GraphicPhoto.Id,
				Name = GraphicPhoto.Name,
				ImageUrl = GraphicPhoto.ImageUrl,
				InStock = GraphicPhoto.InStock,
				Price = GraphicPhoto.Price,
				Description = GraphicPhoto.ShortDescription + "\n" + GraphicPhoto.LongDescription,
				CategoryId = GraphicPhoto.Category.Id,
				CategoryName = GraphicPhoto.Category.Name
			};

			return View(model);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult New(int categoryId = 0)
		{
			GetCategoriesForDropDownList();
			NewGraphicPhotoModel model = new NewGraphicPhotoModel
			{
				CategoryId = categoryId
			};

			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Create GraphicPhoto";
			ViewBag.RouteId = categoryId;
			ViewBag.ControllerName = "Category";

			if (categoryId == 0)
			{
				ViewBag.CancelAction = "Index";
				ViewBag.ControllerName = "Home";
			}

			return View("CreateEdit", model);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult New(NewGraphicPhotoModel model)
		{
			if (ModelState.IsValid && _categoryService.GetById(model.CategoryId.Value) != null)
			{
				var GraphicPhoto = _mapper.NewGraphicPhotoModelToGraphicPhoto(model, true, _categoryService);
				_GraphicPhotoService.NewGraphicPhoto(GraphicPhoto);
				return RedirectToAction("Index", new { id = GraphicPhoto.Id });
			}
			GetCategoriesForDropDownList();

			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
			ViewBag.ControllerName = "Category";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Create GraphicPhoto";
			ViewBag.RouteId = model.CategoryId;

			return View("CreateEdit", model);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Edit(int id)
		{
			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Save Changes";
			ViewBag.ControllerName = "GraphicPhoto";
			ViewBag.RouteId = id;

			GetCategoriesForDropDownList();

			var GraphicPhoto = _GraphicPhotoService.GetById(id);
			if (GraphicPhoto != null)
			{
				var model = _mapper.GraphicPhotoToNewGraphicPhotoModel(GraphicPhoto);
				return View("CreateEdit", model);
			}

			return View("CreateEdit");
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(NewGraphicPhotoModel model)
		{
			if (ModelState.IsValid)
			{
				var GraphicPhoto = _mapper.NewGraphicPhotoModelToGraphicPhoto(model, false, _categoryService);
				_GraphicPhotoService.EditGraphicPhoto(GraphicPhoto);
				return RedirectToAction("Index", new { id = model.Id });
			}

			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Save Changes";
			ViewBag.ControllerName = "GraphicPhoto";
			ViewBag.RouteId = model.Id;
			GetCategoriesForDropDownList();

			return View("CreateEdit", model);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
		{
			var categoryId = _GraphicPhotoService.GetById(id).CategoryId;
			_GraphicPhotoService.DeleteGraphicPhoto(id);

			return RedirectToAction("Topic", "Category", new { id = categoryId, searchQuery = "" });
		}

		private void GetCategoriesForDropDownList()
		{
			var categories = _categoryService.GetAll().Select(category => new CategoryDropdownModel
			{
				Id = category.Id,
				Name = category.Name
			});
			ViewBag.Categories = new SelectList(categories, "Id", "Name");
		}
	}
}