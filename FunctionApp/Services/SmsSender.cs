using System.Threading.Tasks;
using FunctionApp.Models;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FunctionApp.Services
{
    public class SmsSender : ISender
    {
        private readonly Settings _settings;

        public SmsSender(IOptions<Settings> options)
        {
            _settings = options.Value;
        }

        public Task SendAsync(Notification notification)
        {
            var twilio = _settings.Twilio;
            if (!twilio.IsEnabled || notification == null)
            {
                return Task.CompletedTask;
            }
            
            TwilioClient.Init(twilio.Id, twilio.Token);
            return MessageResource.CreateAsync(
                body: notification.ToString(),
                @from: new Twilio.Types.PhoneNumber(twilio.From),
                to: new Twilio.Types.PhoneNumber(twilio.To)
            );
        }
    }
}