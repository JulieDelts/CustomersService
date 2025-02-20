

namespace CustomersService.Application.Interfaces
{
    public interface ICommonHttpClient
    {
        Task<T> SendGetRequestAsync<T>(string path);
        Task<K> SendPostRequestAsync<T, K>(string path, T postRequestModel);
    }
}
