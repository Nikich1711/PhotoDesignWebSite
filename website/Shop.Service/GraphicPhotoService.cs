using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shop.Service
{
    public class GraphicPhotoService : IGraphicPhoto
    {
        private readonly ApplicationDbContext _context;

        public GraphicPhotoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteGraphicPhoto(int id)
        {
            var GraphicPhoto = GetById(id);
            if (GraphicPhoto == null)
            {
                throw new ArgumentException();
            }
            _context.Remove(GraphicPhoto);
            _context.SaveChanges();
        }

        public void EditGraphicPhoto(GraphicPhoto GraphicPhoto)
        {
            var model = _context.GraphicPhotos.First(f => f.Id == GraphicPhoto.Id);
            _context.Entry<GraphicPhoto>(model).State = EntityState.Detached;
            _context.Update(GraphicPhoto);
            _context.SaveChanges();
        }
        public IEnumerable<GraphicPhoto> GetAll()
        {
            return _context.GraphicPhotos
                .Include(GraphicPhoto => GraphicPhoto.Category);
        }

        public GraphicPhoto GetById(int id)
        {
            return GetAll().FirstOrDefault(GraphicPhoto => GraphicPhoto.Id == id);
        }

        public IEnumerable<GraphicPhoto> GetFilteredGraphicPhotos(int id, string searchQuery)
        {

            if (string.IsNullOrEmpty(searchQuery) || string.IsNullOrWhiteSpace(searchQuery))
            {
                return GetGraphicPhotosByCategoryId(id);
            }

            return GetFilteredGraphicPhotos(searchQuery).Where(GraphicPhoto => GraphicPhoto.Category.Id == id);
        }

        public IEnumerable<GraphicPhoto> GetFilteredGraphicPhotos(string searchQuery)
        {
            var queries = string.IsNullOrEmpty(searchQuery) ? null : Regex.Replace(searchQuery, @"\s+", " ").Trim().ToLower().Split(" ");
            if (queries == null)
            {
                return GetPreferred(10);
            }

            return GetAll().Where(item => queries.Any(query => (item.Name.ToLower().Contains(query))));
        }

        public IEnumerable<GraphicPhoto> GetGraphicPhotosByCategoryId(int categoryId)
        {
            return GetAll().Where(GraphicPhoto => GraphicPhoto.Category.Id == categoryId);
        }

        public IEnumerable<GraphicPhoto> GetPreferred(int count)
        {
            return GetAll().OrderByDescending(GraphicPhoto => GraphicPhoto.Id).Where(GraphicPhoto => GraphicPhoto.IsPreferedGraphicPhoto && GraphicPhoto.InStock != 0).Take(count);
        }

        public void NewGraphicPhoto(GraphicPhoto GraphicPhoto)
        {
            _context.Add(GraphicPhoto);
            _context.SaveChanges();
        }
    }
}