using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Application.Interfaces.ServicesInterfaces;

using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly SmtpSettings _smtp;
        private readonly ILogger<InvitationService> _logger;


        public InvitationService(ILogger<InvitationService> logger, IInvitationRepository invitationRepository, IOptions<SmtpSettings> smtpOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _invitationRepository = invitationRepository;
            _smtp = smtpOptions.Value;
        }

        public async Task SendInvitationAsync(string email, string message, Guid invitedByUserId)
        {
            try
            {
                var invitation = new Invitation
                {
                    Email = email,
                    Message = message,
                    InvitedByUserId = invitedByUserId
                };
                await _invitationRepository.AddAsync(invitation);
                await _invitationRepository.SaveChangesAsync();
                _logger.LogError("Failed to send invitation email: " + _smtp.Host+ _smtp.Port);

                using var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    EnableSsl = _smtp.EnableSsl,
                    Credentials = new System.Net.NetworkCredential(_smtp.Username, _smtp.Password)
                };
                _logger.LogInformation($"SMTP From: {_smtp.From}");
                var mail = new MailMessage(_smtp.From, email)
                {
                    Subject = _smtp.Subject ?? "دعوة",
                    Body = message,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send invitation email: " + ex.Message);
            }
        }




        public async Task<IEnumerable<Invitation>> GetAllAsync()
        {
            return await _invitationRepository.GetAllAsync();
        }

        public async Task DeleteAsync(Guid invitationId, Guid requestedByUserId)
        {
            await _invitationRepository.DeleteAsync(invitationId, requestedByUserId);
        }
    }
}
