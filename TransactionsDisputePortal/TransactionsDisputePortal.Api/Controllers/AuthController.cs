using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionsDisputePortal.Application.Authentication.Commands.Login;
using TransactionsDisputePortal.Application.Authentication.Commands.Register;
using TransactionsDisputePortal.Application.Common.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Api.Controllers
{
    /// <summary>
    /// Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Register a new customer account
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register(
            [FromBody] RegisterRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var command = new RegisterCommand
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
