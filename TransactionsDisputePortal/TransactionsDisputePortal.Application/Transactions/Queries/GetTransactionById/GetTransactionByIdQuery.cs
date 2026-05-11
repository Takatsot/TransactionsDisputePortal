using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Transactions.Queries.GetTransactionById
{
    /// <summary>
    /// Query to get a single transaction by ID with full details
    /// </summary>
    public class GetTransactionByIdQuery : IRequest<TransactionDetailDto>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDetailDto>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransactionDetailDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _transactionRepository.FindByIdWithDisputeAsync(request.Id, cancellationToken);

            if (transaction == null || transaction.CustomerId != request.CustomerId)
            {
                throw new NotFoundException(nameof(Domain.Entities.Transaction), request.Id);
            }

            return _mapper.Map<TransactionDetailDto>(transaction);
        }
    }
}
