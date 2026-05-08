using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Common.Interfaces;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Commands.CreateDispute
{
    /// <summary>
    /// Command to create a new dispute
    /// </summary>
    public class CreateDisputeCommand : IRequest<DisputeDto>
    {
        public Guid CustomerId { get; set; }
        public Guid TransactionId { get; set; }
        public DisputeReason Reason { get; set; }
        public string Description { get; set; } = null!;
        public List<AttachmentInfo> Attachments { get; set; } = new List<AttachmentInfo>();
    }

    /// <summary>
    /// Represents attachment file information
    /// </summary>
    public class AttachmentInfo
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = null!;
    }

    public class CreateDisputeCommandHandler : IRequestHandler<CreateDisputeCommand, DisputeDto>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDisputeRepository _disputeRepository;
        private readonly IDisputeHistoryRepository _disputeHistoryRepository;
        private readonly IDisputeAttachmentRepository _disputeAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CreateDisputeCommandHandler(
            ITransactionRepository transactionRepository,
            IDisputeRepository disputeRepository,
            IDisputeHistoryRepository disputeHistoryRepository,
            IDisputeAttachmentRepository disputeAttachmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _disputeRepository = disputeRepository ?? throw new ArgumentNullException(nameof(disputeRepository));
            _disputeHistoryRepository = disputeHistoryRepository ?? throw new ArgumentNullException(nameof(disputeHistoryRepository));
            _disputeAttachmentRepository = disputeAttachmentRepository ?? throw new ArgumentNullException(nameof(disputeAttachmentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<DisputeDto> Handle(CreateDisputeCommand request, CancellationToken cancellationToken)
        {
            // Validate transaction exists and belongs to customer
            var transaction = await _transactionRepository.FindAsync(
                t => t.Id == request.TransactionId && t.CustomerId == request.CustomerId,
                cancellationToken);

            if (transaction == null)
            {
                throw new NotFoundException(nameof(Transaction), request.TransactionId);
            }

            // Check if transaction can be disputed
            if (!transaction.CanBeDisputed)
            {
                throw new ValidationException(
                    "This transaction cannot be disputed. It may already have an active dispute, " +
                    "be too old (>90 days), or not be in completed status.");
            }

            // Check for existing dispute
            var existingDispute = await _disputeRepository.HasActiveDisputeAsync(request.TransactionId, cancellationToken);
            if (existingDispute)
            {
                throw new ValidationException("This transaction already has an active dispute.");
            }

            // Create dispute
            var currentUser = await _currentUserService.GetAsync();
            var currentUserId = currentUser?.Id ?? "System";
            var dispute = Dispute.Create(
                request.TransactionId,
                request.CustomerId,
                request.Reason,
                request.Description,
                currentUserId);

            _disputeRepository.Add(dispute);

            // Mark transaction as disputed
            transaction.MarkAsDisputed(currentUserId);
            _transactionRepository.Update(transaction);

            // Save changes to generate dispute Id
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Now add initial history entry with the generated Id using repository
            var historyEntry = DisputeHistory.Create(
                dispute.Id,
                DisputeStatus.Pending,
                "Dispute created",
                currentUserId);
            
            _disputeHistoryRepository.Add(historyEntry);

            // Add attachments if any
            foreach (var attachmentInfo in request.Attachments)
            {
                var attachment = DisputeAttachment.Create(
                    dispute.Id,
                    attachmentInfo.FileName,
                    attachmentInfo.FileType,
                    attachmentInfo.FileSize,
                    attachmentInfo.StoragePath,
                    currentUserId);
                
                _disputeAttachmentRepository.Add(attachment);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DisputeDto>(dispute);
        }
    }
}
