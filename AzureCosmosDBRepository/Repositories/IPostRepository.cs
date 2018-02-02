using AzureCosmosDBRepository.Entities;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository.Repositories
{
    public interface IPostRepository
    {
        Task CreatePostAsync(Post post);

        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(string id);

        Task<List<Post>> GetPostsByTitleAsync(string title);

        Task<List<Post>> GetPostsByDateRangeAsync(DateTime start, DateTime end);

        Task UpdatePostAsync(Post post, string id);

        Task DeletePostAsync(string id);
    }
}
