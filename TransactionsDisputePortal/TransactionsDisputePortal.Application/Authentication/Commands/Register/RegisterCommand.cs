using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Common.Interfaces;
using TransactionsDisputePortal.Domain.Entities;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Authentication.Commands.Register
{
    /// <summary>
    /// Command to register a new customer and generate JWT token
    /// </summary>
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            ICustomerRepository customerRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            var emailExists = await _customerRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                throw new ValidationException("A user with this email address already exists.");
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create customer
            var customer = Customer.Create(
                request.Email,
                passwordHash,
                request.FirstName,
                request.LastName,
                "Registration");

            _customerRepository.Add(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(
                customer.Id,
                customer.Email,
                customer.FirstName,
                customer.LastName);

            return new AuthResponseDto
            {
                UserId = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expires in 24 hours
            };
        }
    }
}
