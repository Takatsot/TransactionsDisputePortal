using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Authentication.Commands.Login
{
    /// <summary>
    /// Command to authenticate a user and generate JWT token
    /// </summary>
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            ICustomerRepository customerRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find customer by email
            var customer = await _customerRepository.FindByEmailAsync(request.Email, cancellationToken);

            if (customer == null)
            {
                throw new ValidationException("Invalid email or password.");
            }

            // Check if account is active
            if (!customer.IsActive)
            {
                throw new ForbiddenAccessException("This account has been deactivated.");
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            {
                throw new ValidationException("Invalid email or password.");
            }

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
