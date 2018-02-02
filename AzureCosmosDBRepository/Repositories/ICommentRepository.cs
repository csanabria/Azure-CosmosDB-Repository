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
    public interface ICommentRepository
    {
        Task CreateCommentAsync(Comment comment, string postId);

        Task<Comment> GetCommentByIdAsync(string id);

        Task<List<Comment>> GetCommentsByPublisherAsync(string publisher);

        Task<List<Comment>> GetCommentsByPostAsync(string postId);

        Task<List<Comment>> GetCommentsByDateRangeAsync(DateTime start, DateTime end);

        Task DeleteCommentAsync(string id, string postId);
    }
}
