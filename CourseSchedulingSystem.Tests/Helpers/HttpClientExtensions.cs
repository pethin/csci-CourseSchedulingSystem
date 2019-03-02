using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using CourseSchedulingSystem.Models;
using Xunit;

namespace CourseSchedulingSystem.Tests.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpClient> ActAsAsync(this HttpClient client, ApplicationUser user)
        {
            var response = await client.PostAsync("/api/Impersonation", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Id", user.Id.ToString())
            }));
            response.EnsureSuccessStatusCode();

            return client;
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IHtmlElement submitButton)
        {
            return client.SendAsync(form, submitButton, new Dictionary<string, string>());
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            var submitElement = Assert.Single(form.QuerySelectorAll("[type=submit]"));
            var submitButton = Assert.IsAssignableFrom<IHtmlElement>(submitElement);

            return client.SendAsync(form, submitButton, formValues);
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IHtmlElement submitButton,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            foreach (var (key, value) in formValues)
            {
                var element = Assert.IsAssignableFrom<IHtmlInputElement>(form[key]);
                element.Value = value;
            }

            var submit = form.GetSubmission(submitButton);
            var target = (Uri)submit.Target;
            if (submitButton.HasAttribute("formaction"))
            {
                var formAction = submitButton.GetAttribute("formaction");
                target = new Uri(formAction, UriKind.Relative);
            }
            var submission = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), target)
            {
                Content = new StreamContent(submit.Body)
            };

            foreach (var header in submit.Headers)
            {
                submission.Headers.TryAddWithoutValidation(header.Key, header.Value);
                submission.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return client.SendAsync(submission);
}
    }
}