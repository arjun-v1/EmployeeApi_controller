using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public CacheController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("status")]
        public IActionResult GetCacheStatus()
        {
            try
            {
                var field = typeof(MemoryCache).GetField("_coherentState", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var coherentState = field?.GetValue(_cache);
                var entriesCollection = coherentState?.GetType()
                    .GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                
                var count = ((IDictionary?)entriesCollection?.GetValue(coherentState))?.Count ?? 0;
                
                return Ok(new { 
                    CacheEntryCount = count,
                    Message = count > 0 ? "Cache has entries" : "Cache is empty"
                });
            }
            catch
            {
                return Ok(new { CacheEntryCount = "Unknown", Message = "Unable to read cache count" });
            }
        }

        [HttpDelete("clear")]
        public IActionResult ClearCache()
        {
            if (_cache is MemoryCache memCache)
            {
                var field = typeof(MemoryCache).GetField("_coherentState", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var coherentState = field?.GetValue(memCache);
                var clearMethod = coherentState?.GetType().GetMethod("Clear");
                clearMethod?.Invoke(coherentState, null);
                
                return Ok(new { Message = "Cache cleared successfully" });
            }
            return BadRequest(new { Message = "Unable to clear cache" });
        }

        [HttpGet("keys")]
        public IActionResult GetCacheKeys()
        {
            return Ok(new { 
                ExpectedKeys = new[] { 
                    "all_employees", 
                    "all_employees_with_company", 
                    "employee_{id}" 
                },
                Message = "These are the cache keys used by EmployeeManager"
            });
        }
    }
}