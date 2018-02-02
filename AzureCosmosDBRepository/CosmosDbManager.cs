using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository
{
    public class CosmosDBManager : ICosmosDBManager
    {
        private readonly IConfiguration _configuration;

        private DocumentClient _client;

        public CosmosDBManager(IConfiguration appSettingsService)
        {
            _configuration = appSettingsService;
        }

        public async Task<DocumentClient> GetClientAsync()
        {
            if (_client == null)
                await InitializeClient();
            return _client;
        }

        private async Task InitializeClient()
        {
            var endpointUrl = _configuration["CosmosDB:EndpointURL"];
            var primaryKey = _configuration["CosmosDB:Key"];
            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            _client = new DocumentClient(new Uri(endpointUrl), primaryKey, connectionPolicy);
            await _client.OpenAsync();
        }
    }
}
