using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AzureCosmosDBRepository.DTOs;
using AzureCosmosDBRepository.Entities;
using AzureCosmosDBRepository.Filters;
using AzureCosmosDBRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AzureCosmosDBRepository.Features
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        #region Private Members

        private readonly IPostRepository _repository;

        private readonly ICommentRepository _commentsRepository;

        #endregion

        #region Constructor

        public PostsController(IPostRepository repository, ICommentRepository commentsRepository)
        {
            _repository = repository;
            _commentsRepository = commentsRepository;
        }

        #endregion

        #region Posts

        [HttpGet("")]
        [SwaggerOperation("Get Posts")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Post))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> Get()
        {
            var response = await _repository.GetPostsAsync();

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();

        }
        [HttpGet("{id}", Name = "GetById")]
        [SwaggerOperation("Get Post By Id")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Post))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _repository.GetPostByIdAsync(id);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpGet("byTitle")]
        [SwaggerOperation("Get Post By Title")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Post))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetByTitle([FromQuery]string title)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
                return new BadRequestObjectResult("Title has invalid value");

            var response = await _repository.GetPostsByTitleAsync(title);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpGet("byDateRange")]
        [SwaggerOperation("Get Post By Date Range")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Post))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(BadRequestResult))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetByDateRange([FromQuery]FilterByDateRange dateRange)
        {
            if (dateRange.Start == null || dateRange.End == null || (dateRange.Start > dateRange.End))
                return new BadRequestObjectResult("Start or End has invalid value");

            var response = await _repository.GetPostsByDateRangeAsync(dateRange.Start, dateRange.End);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpPost]
        [SwaggerOperation("Create New Post")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(BadRequestResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> Post([FromBody]PostDTO value)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)));

            var id = Guid.NewGuid().ToString();
            var post = new Post()
            {
                Id = id,
                Title = value.Title,
                Abstract = value.Abstract,
                Content = value.Content,
                PublishDate = DateTime.Now,
                Comments = new List<Comment>()
            };            

            await _repository.CreatePostAsync(post);

            return new CreatedAtRouteResult("GetById", new { Id = id }, post);
        }

        [HttpPut("{id}")]
        [SwaggerOperation("Update Existing Post")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(BadRequestResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> Put(string id, [FromBody]PostDTO value)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)));

            var post = await _repository.GetPostByIdAsync(id);
            if (post != null)
            {
                post.Title = value.Title;
                post.Abstract = value.Abstract;
                post.Content = value.Content;
                await _repository.UpdatePostAsync(post, id);
            }

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete Post")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> Delete(string id)
        {
            await _repository.DeletePostAsync(id);
            return new NoContentResult();
        }

        #endregion

        #region Comments

        [HttpGet("{postId}/comments")]
        [SwaggerOperation("Get Comments")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Comment))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetComments(string postId)
        {
            var response = await _commentsRepository.GetCommentsByPostAsync(postId);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpGet("{postId}/comments/{id}", Name = "GetCommentById")]
        [SwaggerOperation("Get Comment By Id")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Comment))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetCommentById(string postId, string id)
        {
            var response = await _commentsRepository.GetCommentByIdAsync(id);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpPost("{postId}/comments")]
        [SwaggerOperation("Create New Post")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(BadRequestResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> PostComment(string postId, [FromBody]CommentDTO value)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)));

            var id = Guid.NewGuid().ToString();
            var comment = new Comment()
            {
                Id = id,
                Publisher = value.Publisher,
                Content = value.Content,
                PublishDate = DateTime.Now
            };

            await _commentsRepository.CreateCommentAsync(comment, postId);

            return new CreatedAtRouteResult("GetCommentById", new { Id = id }, comment);
        }

        [HttpDelete("{postId}/comments/{id}")]
        [SwaggerOperation("Delete Comment")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> DeleteComment(string postId, string id)
        {
            await _commentsRepository.DeleteCommentAsync(id, postId);
            return new NoContentResult();
        }

        #endregion
    }
}
