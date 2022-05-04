using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T?> GetObjectAsync<T>(this HttpClient client, Uri requestUri)
        {
            var json = await client.GetStringAsync(requestUri);
            return JsonSerializer.Deserialize<T>(json, ObjectExtensions.DefaultJsonSerializerOptions);
        }

        public static Task<T?> GetObjectAsync<T>(this HttpClient client, string requestUri)
        {
            return client.GetObjectAsync<T>(new Uri(requestUri));
        }

        public static async Task<T?> PostObjectAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            content ??= new StringContent(string.Empty);
            var message = await client.PostAsync(requestUri, content, cancellationToken);
            message.EnsureSuccessStatusCode();
            var json = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, ObjectExtensions.DefaultJsonSerializerOptions);
        }

        public static Task<T?> PostObjectAsync<T>(this HttpClient client, string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            return client.PostObjectAsync<T>(new Uri(requestUri), content, cancellationToken);
        }

        public static Task<T?> PostObjectAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content)
        {
            return client.PostObjectAsync<T>(requestUri, content, default);
        }

        public static Task<T?> PostObjectAsync<T>(this HttpClient client, string requestUri, HttpContent? content)
        {
            return client.PostObjectAsync<T>(new Uri(requestUri), content, default);
        }

        public static Task<T?> PostObjectAsync<K, T>(this HttpClient client, Uri requestUri, K? obj, CancellationToken cancellationToken)
        {
            StringContent? content = null;
            if (obj != null)
            {
                var json = obj.ToJson();
                content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }
            return client.PostObjectAsync<T>(requestUri, content, cancellationToken);
        }

        public static Task<T?> PostObjectAsync<K, T>(this HttpClient client, string requestUri, K obj, CancellationToken cancellationToken)
        {
            return client.PostObjectAsync<K, T>(new Uri(requestUri), obj, cancellationToken);
        }

        public static Task<T?> PostObjectAsync<K, T>(this HttpClient client, Uri requestUri, K obj)
        {
            return client.PostObjectAsync<K, T>(requestUri, obj, default);
        }

        public static Task<T?> PostObjectAsync<K, T>(this HttpClient client, string requestUri, K obj)
        {
            return client.PostObjectAsync<K, T>(new Uri(requestUri), obj, default);
        }
    }
}
