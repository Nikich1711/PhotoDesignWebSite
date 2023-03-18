using Shop.Data.Models;
using System.Collections.Generic;

namespace Shop.Data
{
    public interface IGraphicPhoto
    {
        IEnumerable<GraphicPhoto> GetAll();
        IEnumerable<GraphicPhoto> GetPreferred(int count);
        IEnumerable<GraphicPhoto> GetGraphicPhotosByCategoryId(int categoryId);
        IEnumerable<GraphicPhoto> GetFilteredGraphicPhotos(int id, string searchQuery);
        IEnumerable<GraphicPhoto> GetFilteredGraphicPhotos(string searchQuery);
        GraphicPhoto GetById(int id);
        void NewGraphicPhoto(GraphicPhoto GraphicPhoto);
        void EditGraphicPhoto(GraphicPhoto GraphicPhoto);
        void DeleteGraphicPhoto(int id);
    }
}