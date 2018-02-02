using AzureCosmosDBRepository.Entities;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository.Repositories
{
    public class CommentRepository : ICommentRepository
    {      
        private DocumentClient _client;

        private readonly string _database;

        private readonly string _collection;

        private readonly IPostRepository _postRepository;

        public CommentRepository(ICosmosDBManager cosmosDBManager, IConfiguration configuration, IPostRepository postRepository)
        {
            Initialize(cosmosDBManager).Wait();
            _database = configuration["CosmosDB:Database"];
            _collection = configuration["CosmosDB:Collection"];
            _postRepository = postRepository;
        }

        private async Task Initialize(ICosmosDBManager cosmosDBManager)
        {
            _client = await cosmosDBManager.GetClientAsync();
        }

        public async Task CreateCommentAsync(Comment comment, string postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);

            if (post != null)
            {
                post.Comments.Add(comment);
                await _postRepository.UpdatePostAsync(post, postId);
            }
        }

        public async Task<Comment> GetCommentByIdAsync(string id)
        {
            return await _client.CreateDocumentQuery<Post>(
                UriFactory.CreateDocumentCollectionUri(_database, _collection))
                .Where(p => p.Comments.Any(c => c.Id == id))
                .SelectMany(p => p.Comments)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetCommentsByPublisherAsync(string publisher)
        {
            var result = new List<Comment>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .AsDocumentQuery();

            var partialResponse = new List<Post>();
            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Post>();
                partialResponse.AddRange(response);
            }

            result = partialResponse.Where(p => p.Comments.Any(c => c.Publisher == publisher))
                .SelectMany(p => p.Comments)
                .ToList();

            return result.Count != 0 ? result : null;
        }

        public async Task<List<Comment>> GetCommentsByPostAsync(string postId)
        {
            var result = new List<Comment>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .Where(p => p.Id == postId)
                        .SelectMany(p => p.Comments)
                        .AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Comment>();
                result.AddRange(response);
            }

            return result.Count != 0 ? result : null;
        }

        public async Task<List<Comment>> GetCommentsByDateRangeAsync(DateTime start, DateTime end)
        {
            var result = new List<Comment>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .AsDocumentQuery();

            var partialResponse = new List<Post>();
            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Post>();
                partialResponse.AddRange(response);
            }

            result = partialResponse.Where(p => p.Comments.Any(c => c.PublishDate >= start && c.PublishDate <= end))
                        .SelectMany(p => p.Comments)
                        .ToList();

            return result.Count != 0 ? result : null;
        }    

        public async Task DeleteCommentAsync(string id, string postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);

            if (post != null)
            {
                post.Comments.RemoveAll(c => c.Id == id);
                await _postRepository.UpdatePostAsync(post, post.Id);
            }
        }
    }
}
