using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace VTQT.SharedMvc.Helpers
{
    public class SendDiscordHelper : ISendDiscordHelper
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWorkContext _workContext;
        public SendDiscordHelper(IHttpClientFactory clientFactory,
            IWorkContext workContext)
        {
            _clientFactory = clientFactory;
            _workContext = workContext;
        }



        public async Task<object> SendMessage(string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            try
            {
                var avatar = CommonHelper.GetAppSetting<string>($"DiscordMessage:Avatar");
                var username = CommonHelper.GetAppSetting<string>($"DiscordMessage:UserName");
                var Webhook = CommonHelper.GetAppSetting<string>($"DiscordMessage:Webhook");
                if (string.IsNullOrEmpty(Webhook))
                    return false;
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(username), "username");
                formdata.Add(new StringContent(content), "content");
                formdata.Add(new StringContent(avatar), "avatar_url");
                var client = _clientFactory.CreateClient();
                var response = await client.PostAsync(Webhook, formdata);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                return response;
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }

        }
    }

    public class DiscordMessageModel
    {
        public string Webhook { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public string Content { get; set; }
    }


}
