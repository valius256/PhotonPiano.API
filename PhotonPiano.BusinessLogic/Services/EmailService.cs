using Microsoft.Extensions.Options;
using PhotonPiano.BackgroundJob;
using PhotonPiano.BusinessLogic.BusinessModel.Email;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models;
using Razor.Templating.Core;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PhotonPiano.BusinessLogic.Services;

public class EmailService : IEmailService
{
    private readonly IDefaultScheduleJob _defaultScheduleJob;
    private readonly IRazorTemplateEngine _razorTemplateEngine;
    private readonly SmtpAppSetting _smtpAppSetting;

    public EmailService(IRazorTemplateEngine razorTemplateEngine,
        IDefaultScheduleJob defaultScheduleJob,
        IOptions<SmtpAppSetting> smtpAppSetting)
    {
        _razorTemplateEngine = razorTemplateEngine;
        _defaultScheduleJob = defaultScheduleJob;
        _smtpAppSetting = smtpAppSetting.Value;
    }

    public async Task SendAsync(string templateName, List<string> toAddress, List<string>? ccAddresses,
        Dictionary<string, string> param, bool isInQueue = false)
    {
        if (!isInQueue)
        {
            _defaultScheduleJob.Enqueue<IEmailService>(m =>
                m.SendAsync(templateName, toAddress, ccAddresses, param, true));
            return;
        }

        var body = await _razorTemplateEngine.RenderAsync($"/Views/{templateName}.cshtml", param);
        var subject = param.ContainsKey("Subject") ? param["Subject"] : "[PhotonPiano] Notification";

        var template = new SendEmailModel
        {
            Subject = subject,
            Body = body,
            ToEmail = toAddress,
            CcEmail = ccAddresses
        };

        await SendSingleEmailAsync(template);
    }

    private async Task SendSingleEmailAsync(SendEmailModel template)
    {
        using (var client = new SmtpClient(_smtpAppSetting.SmtpHost, _smtpAppSetting.SmtpPort))
        {
            client.EnableSsl = _smtpAppSetting.EnableSsl;
            client.Credentials = new NetworkCredential(_smtpAppSetting.SmtpUserName, _smtpAppSetting.AppVerify);

            using (var message = new MailMessage())
            {
                try
                {
                    message.From = new MailAddress(_smtpAppSetting.SmtpUserName);

                    // Add to and cc addresses
                    template.ToEmail.ForEach(to => message.To.Add(to));
                    if (template.CcEmail is not null) template.CcEmail.ForEach(cc => message.CC.Add(cc));

                    message.Subject = ReplaceParam(template.Subject, new Dictionary<string, string>());
                    message.Body = ReplaceParam(template.Body, new Dictionary<string, string>());
                    message.IsBodyHtml = true;

                    message.BodyEncoding = Encoding.UTF8;
                    message.SubjectEncoding = Encoding.UTF8;

                    await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    private static string ReplaceParam(string data, Dictionary<string, string> parameters)
    {
        foreach (var parameter in parameters) data = data.Replace($"[{parameter.Key}]", parameter.Value);
        return data;
    }
}