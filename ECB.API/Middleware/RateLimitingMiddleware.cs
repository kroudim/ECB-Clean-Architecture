using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

/// <summary>
/// Middleware for rate-limiting requests per IP address using a token bucket algorithm.
/// </summary>
public class RateLimitingMiddleware
    {
    private readonly RequestDelegate _next;
    // Stores a token bucket for each IP address
    private static readonly ConcurrentDictionary<string, TokenBucket> Buckets = new();

    // Settings for rate-limiting: 5 requests per 60 seconds per IP
    private const int Capacity = 5;
    private const double RefillIntervalSeconds = 60;

    /// <summary>
    /// Constructs the middleware with the next request delegate.
    /// </summary>
    public RateLimitingMiddleware(RequestDelegate next)
        {
        _next = next;
        }

    /// <summary>
    /// Invokes the rate-limiting logic for each HTTP request.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
        {
        // Get the client's IP address
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Get or create a token bucket for this IP
        var bucket = Buckets.GetOrAdd(ip, _ => new TokenBucket(Capacity, RefillIntervalSeconds));

        // Try to grant the request based on the bucket tokens
        if (!bucket.GrantRequest())
            {
            // If rate limit exceeded, return HTTP 429
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync($"Rate limit exceeded. Allowed {Capacity} requests per {RefillIntervalSeconds} seconds. Try again later.");
            return;
            }

        // Continue to the next middleware in the pipeline
        await _next(context);
        }
    }

/// <summary>
/// Implements a simple token bucket for rate-limiting.
/// </summary>
public class TokenBucket
    {
    private int _tokens;
    private readonly int _capacity;
    private readonly double _intervalSeconds;
    private DateTime _lastRefill;

    /// <summary>
    /// Constructs a token bucket with specified capacity and refill interval.
    /// </summary>
    public TokenBucket(int capacity, double intervalSeconds)
        {
        _capacity = capacity;
        _intervalSeconds = intervalSeconds;
        _tokens = capacity;
        _lastRefill = DateTime.UtcNow;
        }

    /// <summary>
    /// Attempts to grant a request by consuming a token, refilling if needed.
    /// </summary>
    public bool GrantRequest()
        {
        Refill();
        if (_tokens > 0)
            {
            _tokens--;
            return true;
            }
        return false;
        }

    /// <summary>
    /// Refills tokens if the interval has elapsed since the last refill.
    /// </summary>
    private void Refill()
        {
        var now = DateTime.UtcNow;
        var secondsSinceLast = (now - _lastRefill).TotalSeconds;
        if (secondsSinceLast >= _intervalSeconds)
            {
            _tokens = _capacity;
            _lastRefill = now;
            }
        }
    }