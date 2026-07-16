using Microsoft.Extensions.Caching.Memory;

namespace backend.Cache
{
    public class ReservationCache
    {
        private readonly IMemoryCache _cache;

        public ReservationCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string Reserve(Guid userId, Guid eventTypeId, DateTime startsAt)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            var token = Guid.NewGuid().ToString();

            _cache.Set(key, token, TimeSpan.FromMinutes(5));

            return token;
        }

        public bool Release(Guid userId, Guid eventTypeId, DateTime startsAt, string token)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            if (_cache.TryGetValue(key, out string? cachedToken) && cachedToken == token)
            {
                _cache.Remove(key);
                return true;
            }

            return false;
        }

        public bool IsReserved(Guid userId, Guid eventTypeId, DateTime startsAt)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            return _cache.TryGetValue(key, out _);
        }

        public bool IsReservedWithToken(Guid userId, Guid eventTypeId, DateTime startsAt, string token)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            return _cache.TryGetValue(key, out string? cachedToken) && cachedToken == token;
        }
    }
}
