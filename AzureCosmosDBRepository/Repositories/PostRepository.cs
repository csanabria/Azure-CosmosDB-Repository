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
    public class PostRepository : IPostRepository
    {
        private DocumentClient _client;

        private readonly string _database;

        private readonly string _collection;

        public PostRepository(ICosmosDBManager cosmosDBManager, IConfiguration configuration)
        {
            Initialize(cosmosDBManager).Wait();
            _database = configuration["CosmosDB:Database"];
            _collection = configuration["CosmosDB:Collection"];
        }

        private async Task Initialize(ICosmosDBManager cosmosDBManager)
        {
            _client = await cosmosDBManager.GetClientAsync();
        }

        public async Task CreatePostAsync(Post post)
        {
            await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_database, _collection), post);
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            var result = new List<Post>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Post>();
                result.AddRange(response);
            }

            return result.Count != 0 ? result : null;
        }

        public async Task<Post> GetPostByIdAsync(string id)
        {
            return await _client.ReadDocumentAsync<Post>(
                UriFactory.CreateDocumentUri(_database, _collection, id));
        }

        public async Task<List<Post>> GetPostsByTitleAsync(string title)
        {
            var result = new List<Post>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .Where(p => p.Title.Contains(title))
                        .AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Post>();
                result.AddRange(response);
            }

            return result.Count != 0 ? result : null;
        }

        public async Task<List<Post>> GetPostsByDateRangeAsync(DateTime start, DateTime end)
        {
            var result = new List<Post>();

            var _queryOptions = new FeedOptions { MaxItemCount = -1 };

            var queryable = _client.CreateDocumentQuery<Post>(
                        UriFactory.CreateDocumentCollectionUri(_database, _collection), _queryOptions)
                        .Where(p => p.PublishDate >= start && p.PublishDate <= end)
                        .AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Post>();
                result.AddRange(response);
            }

            return result.Count != 0 ? result : null;
        }

        public async Task UpdatePostAsync(Post post, string id)
        {
            var document = await _client.ReadDocumentAsync<Post>(UriFactory.CreateDocumentUri(_database, _collection, id));
            if (document != null)
            {
                post.Id = id;
                await _client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(_database, _collection, id), post);
            }
        }

        public async Task DeletePostAsync(string id)
        {
            var document = await _client.ReadDocumentAsync<Post>(UriFactory.CreateDocumentUri(_database, _collection, id));
            if (document != null)
                await _client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(_database, _collection, id));
        }
    }
}
