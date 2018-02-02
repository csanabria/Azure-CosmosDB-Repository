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
    public class CommentsController : Controller
    {
        private readonly ICommentRepository _repository;

        public CommentsController(ICommentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("byPublisher")]
        [SwaggerOperation("Get Comment By Publisher")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Comment))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetByPublisher([FromQuery]string publisher)
        {
            if (string.IsNullOrEmpty(publisher) || string.IsNullOrWhiteSpace(publisher))
                return new BadRequestObjectResult("Publisher has invalid value");

            var response = await _repository.GetCommentsByPublisherAsync(publisher);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }

        [HttpGet("byDateRage")]
        [SwaggerOperation("Get Comment By Date Range")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Comment))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(BadRequestResult))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, typeof(NoContentResult))]
        public async Task<IActionResult> GetByDateRange([FromQuery]FilterByDateRange dateRange)
        {
            if (dateRange.Start == null || dateRange.End == null || (dateRange.Start > dateRange.End))
                return new BadRequestObjectResult("Start or End has invalid value");

            var response = await _repository.GetCommentsByDateRangeAsync(dateRange.Start, dateRange.End);

            if (response != null)
                return new OkObjectResult(response);
            else
                return new NoContentResult();
        }
    }
}
