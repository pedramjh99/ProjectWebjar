using ProjectWebjar.Data;
using ProjectWebjar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebjar.Repository
{
    public interface IPicsRepository
    {
        public Task Delete(Product productpic);
        public Task<List<Product>> FindPicsGraterThan(double Size);
    }
    public class PicsRepository : IPicsRepository
    {
        private readonly ProjectWebjarContext _context;
        public PicsRepository(ProjectWebjarContext context)
        {
            _context = context;
        }
        public async Task Delete(Product productpic)
        {
            _context.Remove(productpic.PicturePath);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> FindPicsGraterThan(double Size)
        {
            return _context.Products.Where(p => p.PicturePath.Length >= Size).ToList();
        }
    }
}
