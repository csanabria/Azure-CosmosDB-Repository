using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository
{
    public interface ICosmosDBManager
    {
        Task<DocumentClient> GetClientAsync();
    }
}
