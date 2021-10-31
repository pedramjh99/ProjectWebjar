using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProjectWebjar.Data;
using ProjectWebjar.Models;
using ProjectWebjar.Repository;

namespace ProjectWebjar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly ProjectWebjarContext _context;
        private readonly IDistributedCache _distributedCache;
        
        public CommentController(ProjectWebjarContext context,IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache; 
        }

        #region SQL DB
        //----------------------------------------------SQL SERVER--------------------------------------------\\
        [HttpPost] //Add Comment for Product 

        //public async Task<ActionResult> PostComment(CommentViewModel cmVM)
        //{

        //    Comment comment = new Comment(cmVM.Name, cmVM.Message, cmVM.ProductId);
        //    _context.Add(comment);
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        //[HttpGet] //Show Comment 

        //public List<CommentViewModel> GetComment()
        //{

        //    var model = new CommentViewModel();
        //    var comments = _context.Comments.Select(p => new CommentViewModel()
        //    {
        //        ProductId = p.ProductId,
        //        Message = p.Message,
        //        Name = p.Name
        //    }).ToList();

        //    return comments;
        //}

        #endregion 

        #region NO SQL DB

        //--------------------------------------NOSQL With Mongo And Redis-----------------------------------\\


        [HttpPost] //Add Comment for Product 

        public void AddComment(CommentViewModel cmVM)
        {

            CommentRepository comment = new CommentRepository();

            comment.Add(new Comment
            (
                cmVM.Name,
                cmVM.Message,
                cmVM.ProductId
            ));
        }

        [HttpGet] //Show Comment 

        public List<Comment> GetComment()
        {
            CommentRepository comment = new CommentRepository();

            var cmcacheJson =  _distributedCache.GetAsync("CommentRepository").Result;
            if(cmcacheJson != null)
            {
                comment = JsonSerializer.Deserialize<CommentRepository>(cmcacheJson);
            }
            else
            {
                string jsondata = JsonSerializer.Serialize(comment.GetList());
                byte[] encodedjson = Encoding.UTF8.GetBytes(jsondata);

                var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _distributedCache.SetAsync("Comment",encodedjson,option);
            }

            return  comment.GetList();
            
        }

        #endregion
        
    }
}