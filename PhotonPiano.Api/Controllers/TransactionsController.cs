using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Transaction;
using PhotonPiano.BusinessLogic.BusinessModel.Transaction;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public TransactionsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [CustomAuthorize(Roles = [Role.Student, Role.Staff])]
        [EndpointDescription("Get transactions with paging")]
        public async Task<ActionResult<List<TransactionModel>>> GetPagedTransactions([FromQuery] QueryPagedTransactionsRequest request)
        {
            var pagedResult =
                await _serviceFactory.TransactionService.GetPagedTransactions(
                    request.Adapt<QueryPagedTransactionsModel>(), base.CurrentAccount!);

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }
    }
}