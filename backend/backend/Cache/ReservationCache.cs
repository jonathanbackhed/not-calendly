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

        public void AddReservation(Guid userId, Guid eventTypeId, DateTime startsAt)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            _cache.Set(key, true, TimeSpan.FromMinutes(5));
        }

        public bool IsReserved(Guid userId, Guid eventTypeId, DateTime startsAt)
        {
            var key = $"reservation:{userId}:{eventTypeId}:{startsAt:yyyy-MM-ddTHH:mm}";
            return _cache.TryGetValue(key, out _);
        }
    }
}
