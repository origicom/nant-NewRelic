using System;
using System.IO;
using System.Net;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace NewRelicTask
{
    [TaskName("newrelic")]
    public class NewRelicTask : Task
    {
        [TaskAttribute("license", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string License { get; set; }

        [TaskAttribute("revision")]
        [StringValidator(AllowEmpty = false)]
        public string Revision { get; set; }

        [TaskAttribute("app_name")]
        [StringValidator(AllowEmpty = false)]
        public string ApplicationName { get; set; }

        [TaskAttribute("application_id")]
        [StringValidator(AllowEmpty = false)]
        public string ApplicationId { get; set; }

        [TaskAttribute("description")]
        [StringValidator(AllowEmpty = false)]
        public string Description { get; set; }

        [TaskAttribute("changelog")]
        [StringValidator(AllowEmpty = false)]
        public string ChangeLog { get; set; }

        [TaskAttribute("user")]
        [StringValidator(AllowEmpty = false)]
        public string User { get; set; }

        [TaskAttribute("url", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string NewRelicUrl { get; set; }

        [TaskAttribute("host")]
        [StringValidator(AllowEmpty = false)]
        public string Host { get; set; }

        protected override void ExecuteTask()
        {
            try
            {
                ValidateAppNameAndAppId();

                var strBuilder = new StringBuilder();
                if(string.IsNullOrEmpty(ApplicationId))
                  strBuilder.AppendFormat("deployment[app_name]={0}", ApplicationName);
                else
                  strBuilder.AppendFormat("deployment[application_id]={0}", ApplicationId);

                if (!string.IsNullOrEmpty(Revision))
                    strBuilder.AppendFormat("&deployment[revision]={0}", Revision);
                if (!string.IsNullOrEmpty(Host))
                    strBuilder.AppendFormat("&deployment[host]={0}", Host);
                if (!string.IsNullOrEmpty(Description))
                    strBuilder.AppendFormat("&deployment[description]={0}", Description);
                if (!string.IsNullOrEmpty(ChangeLog))
                    strBuilder.AppendFormat("&deployment[changelog]={0}", ChangeLog);
                if (!string.IsNullOrEmpty(User))
                    strBuilder.AppendFormat("&deployment[user]={0}", User);


                var encoding = new ASCIIEncoding();
                var bytes = encoding.GetBytes(strBuilder.ToString());

                var request = WebRequest.Create(NewRelicUrl);
                request.ContentType  = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.Headers.Add("x-api-key", License);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                var response = request.GetResponse();

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var sr = new StreamReader(responseStream);
                        var responseString = sr.ReadToEnd();
                        Project.Log(Level.Info, responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Project.Log(Level.Error, "Exception when sending request to New Relic. Message: {0}. StackTrace: {1}",
                            ex.Message, ex.StackTrace);
            }
        }

        private void ValidateAppNameAndAppId()
        {
            if (string.IsNullOrEmpty(ApplicationId) && string.IsNullOrEmpty(ApplicationName))
                throw new ArgumentException("application_id or app_name must be provided");
            if (!string.IsNullOrEmpty(ApplicationId) && !string.IsNullOrEmpty(ApplicationName))
                throw new ArgumentException("application_id and app_name can not both be provided, pick one");
        }
    }
}
