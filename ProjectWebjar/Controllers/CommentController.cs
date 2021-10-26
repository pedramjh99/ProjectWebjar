using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ProjectWebjar.Data;
using ProjectWebjar.Models;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ProjectWebjar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ProjectWebjarContext _context;

        public CommentController(ProjectWebjarContext context)
        {
            _context = context;
        }

        [HttpPost] //Add Comment for Product
        
        public async Task<ActionResult> PostComment(CommentViewModel cmVM)
        {

            Comment comment = new Comment(cmVM.Name,cmVM.Message,cmVM.ProductId);
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet] //Show Comment

        public List<CommentViewModel> GetComment()
        {

            var model = new CommentViewModel();
            var comments = _context.Comments.Select(p => new CommentViewModel()
            {
                ProductId = p.ProductId,
                Message = p.Message,
                Name = p.Name
            }).ToList();

            return comments;
        }
    }
}

