using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProjectWebjar.Data;
using ProjectWebjar.Models;

namespace ProjectWebjar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ProjectWebjarContext _db;

        public CommentController(ProjectWebjarContext db)
        {
            _db = db;
        }

        [HttpPost] //Add Comment for Product

        public async Task<ActionResult> PostComment(CommentViewModel cmVM)
        {

            Comment comment  = new Comment();
            comment.Name = cmVM.Name;
            comment.Message = cmVM.Message;
            comment.ProductId = cmVM.ProductId;
            _db.Add(comment);
            await _db.SaveChangesAsync();
            return Ok();

        }

        [HttpGet] //Show Comment

        public List<CommentViewModel> GetComment()
        {
         
            var model =new CommentViewModel();
            var comments = _db.Comments.Select(p => new CommentViewModel()
            {
                ProductId = p.ProductId,
                Message = p.Message,
                Name = p.Name
            }).ToList();

            return comments;

        }
    }
}

