using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// HttpClient 扩展方法
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json.httpclientjsonextensions?view=net-6.0"/>
    /// </summary>
    public static class HttpClientExtensions
    {
        #region Get

        /// <summary>
        /// Sends a GET request to the specified Uri
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static async Task<T?> GetFromJsonAsync<T>(this HttpClient client, Uri requestUri)
        {
            var json = await client.GetStringAsync(requestUri);
            return JsonSerializer.Deserialize<T>(json, ObjectExtensions.DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// Sends a GET request to the specified Uri
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static Task<T?> GetFromJsonAsync<T>(this HttpClient client, string requestUri)
        {
            return client.GetFromJsonAsync<T>(new Uri(requestUri));
        }

        #endregion

        #region Post

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> PostFromJsonAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            content ??= new StringContent(string.Empty);
            var message = await client.PostAsync(requestUri, content, cancellationToken);
            message.EnsureSuccessStatusCode();
            var json = await message.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, ObjectExtensions.DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PostFromJsonAsync<T>(this HttpClient client, string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            return client.PostFromJsonAsync<T>(new Uri(requestUri), content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Task<T?> PostFromJsonAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content)
        {
            return client.PostFromJsonAsync<T>(requestUri, content, default);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Task<T?> PostFromJsonAsync<T>(this HttpClient client, string requestUri, HttpContent? content)
        {
            return client.PostFromJsonAsync<T>(new Uri(requestUri), content, default);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PostAsJsonFromJsonAsync<K, T>(this HttpClient client, Uri requestUri, K? obj, CancellationToken cancellationToken)
        {
            StringContent? content = null;
            if (obj != null)
            {
                var json = obj.ToJson();
                content = new StringContent(json);
                content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");
            }

            return client.PostFromJsonAsync<T>(requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PostAsJsonFromJsonAsync<K, T>(this HttpClient client, string requestUri, K obj, CancellationToken cancellationToken)
        {
            return client.PostAsJsonFromJsonAsync<K, T>(new Uri(requestUri), obj, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<T?> PostAsJsonFromJsonAsync<K, T>(this HttpClient client, Uri requestUri, K obj)
        {
            return client.PostAsJsonFromJsonAsync<K, T>(requestUri, obj, default);
        }

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<T?> PostAsJsonFromJsonAsync<K, T>(this HttpClient client, string requestUri, K obj)
        {
            return client.PostAsJsonFromJsonAsync<K, T>(new Uri(requestUri), obj, default);
        }

        #endregion

        #region Put

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> PutFromJsonAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            content ??= new StringContent(string.Empty);
            var message = await client.PutAsync(requestUri, content, cancellationToken);
            message.EnsureSuccessStatusCode();
            var json = await message.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, ObjectExtensions.DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PutFromJsonAsync<T>(this HttpClient client, string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            return client.PutFromJsonAsync<T>(new Uri(requestUri), content, cancellationToken);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Task<T?> PutFromJsonAsync<T>(this HttpClient client, Uri requestUri, HttpContent? content)
        {
            return client.PutFromJsonAsync<T>(requestUri, content, default);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Task<T?> PutFromJsonAsync<T>(this HttpClient client, string requestUri, HttpContent? content)
        {
            return client.PutFromJsonAsync<T>(new Uri(requestUri), content, default);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PutAsJsonFromJsonAsync<K, T>(this HttpClient client, Uri requestUri, K? obj, CancellationToken cancellationToken)
        {
            StringContent? content = null;
            if (obj != null)
            {
                var json = obj.ToJson();
                content = new StringContent(json);
                content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");
            }

            return client.PutFromJsonAsync<T>(requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> PutAsJsonFromJsonAsync<K, T>(this HttpClient client, string requestUri, K obj, CancellationToken cancellationToken)
        {
            return client.PutAsJsonFromJsonAsync<K, T>(new Uri(requestUri), obj, cancellationToken);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<T?> PutAsJsonFromJsonAsync<K, T>(this HttpClient client, Uri requestUri, K obj)
        {
            return client.PutAsJsonFromJsonAsync<K, T>(requestUri, obj, default);
        }

        /// <summary>
        /// Sends a Put request to the specified Uri containing the value serialized as JSON in the request body
        /// and returns the value that results from deserializing the response body as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<T?> PutAsJsonFromJsonAsync<K, T>(this HttpClient client, string requestUri, K obj)
        {
            return client.PutAsJsonFromJsonAsync<K, T>(new Uri(requestUri), obj, default);
        }

        #endregion
    }
}
