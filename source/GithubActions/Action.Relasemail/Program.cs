using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Action.Relasemail
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sendgridApiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var sendgridTemplateId = Environment.GetEnvironmentVariable("SENDGRID_TEMPLATE_ID");
            var sender = Environment.GetEnvironmentVariable("SENDER_MAIL");
            var receipientsVariable = Environment.GetEnvironmentVariable("RECEIPIENTS");
            var pathEventFile = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH");

            var client = new SendGridClient(sendgridApiKey);

            if (string.IsNullOrWhiteSpace(receipientsVariable))
                Environment.Exit(1);

            var receipients = receipientsVariable.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var actionPayload = JsonConvert.DeserializeObject<GithubModels.ActionPayload>(await File.ReadAllTextAsync(pathEventFile));

            var message = new SendGridMessage();
            message.SetTemplateId(sendgridTemplateId);

            // cleanup Body
            var body = actionPayload.Release.Body
                .Replace(Environment.NewLine, "<br/>");


            var dynamicTemplateData = new
            {
                release = new
                {
                    name = actionPayload.Release.Name,
                    tag = actionPayload.Release.TagName,
                    body = body,
                    link = actionPayload.Release.HtmlUrl.ToString()
                },
                autor = new
                {
                    avatarurl = actionPayload.Release.Author.AvatarUrl,
                    name = actionPayload.Release.Author.Login
                }
            };

            message.SetTemplateData(dynamicTemplateData);

            foreach (var s in receipients)
            {
                message.AddTo(new EmailAddress(s));
            }

            message.SetFrom(new EmailAddress(sender));

            await client.SendEmailAsync(message);

            Console.WriteLine("Has been sent");
        }
    }
}
