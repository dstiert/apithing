using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };

            var postResp = client.PostAsync("/v1/values",
                                new StringContent(JsonConvert.SerializeObject(new { value = "12345" }),
                                Encoding.UTF8,
                                "application/json")).Result;

            Assert.That(postResp.Headers.Location, Is.Not.Null);
            var getResp = client.GetAsync(postResp.Headers.Location).Result;
            var stored = JsonConvert.DeserializeObject<StoredValue>(getResp.Content.ReadAsStringAsync().Result);
            Assert.That(stored.Value, Is.EqualTo("12345"));
        }

        private class StoredValue
        {
            public string Value { get; set; }
        }
    }
}