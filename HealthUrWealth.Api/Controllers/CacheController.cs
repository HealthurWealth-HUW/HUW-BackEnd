using HealthUrWelath.Application.Common.Caching;
using Microsoft.AspNetCore.Mvc;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class CacheController : ControllerBase
    {
        private readonly IAppCache _cache;

        public CacheController(IAppCache cache)
        {
            _cache = cache;
        }

        // 🔥 Clear entire cache
        [HttpPost("clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            await _cache.ClearAllAsync();
            return Ok(new { message = "All cache cleared" });
        }

        // 🎯 Clear specific key
        [HttpPost("clear")]
        public async Task<IActionResult> ClearByKey([FromQuery] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("Cache key is required");

            await _cache.RemoveAsync(key);
            return Ok(new { message = $"Cache cleared for key: {key}" });
        }
    }
}
