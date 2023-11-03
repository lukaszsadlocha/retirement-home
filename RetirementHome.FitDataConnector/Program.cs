using Google.Apis.Discovery.v1;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;

namespace RetirementHome.FitDataConnector;

/// <summary>
///     This example uses the discovery API to list all APIs in the discovery repository.
///     https://developers.google.com/discovery/v1/using.
///     <summary>
internal class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Console.WriteLine("Discovery API Sample");
        Console.WriteLine("====================");
        try
        {
            new Program().Run().Wait();
        }
        catch (AggregateException ex)
        {
            foreach (var e in ex.InnerExceptions) Console.WriteLine("ERROR: " + e.Message);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task Run()
    {
        // Create the service.

        var clientServiceInitializer = new BaseClientService.Initializer
        {
            ApplicationName = "Retirement home",
            ApiKey = "AIzaSyCJDlHFpt1NI8_354BQf9u2b8XdlNHIXOM"
        };

        var service = new FitnessService(clientServiceInitializer);

        var query = new WeightQuery(service);
        var list = query.QueryWeightPerDay(DateTime.Now.AddDays(-60), DateTime.Now);

        var dataTable = list
            .Select(w => new object[]
            {
                w.Stamp,
                w.MinWeight,
                w.MaxWeight
            })
            .ToArray();
    }
}

/// <summary>
/// Weight data point.
/// </summary>
public class WeightDataPoint
{
    /// <summary>
    /// Gets or sets the weight.
    /// </summary>
    /// <value>
    /// The weight.
    /// </value>
    public double? Weight { get; set; }

    /// <summary>
    /// Gets or sets the stamp.
    /// </summary>
    /// <value>
    /// The stamp.
    /// </value>
    public DateTime Stamp { get; set; }

    /// <summary>
    /// Gets or sets the maximum weight.
    /// </summary>
    /// <value>
    /// The maximum weight.
    /// </value>
    public double? MaxWeight { get; set; }

    /// <summary>
    /// Gets or sets the minimum weight.
    /// </summary>
    /// <value>
    /// The minimum weight.
    /// </value>
    public double? MinWeight { get; set; }
}


/// <summary>
/// Queries the weight data source of Google Fit.
/// </summary>
public class WeightQuery : FitnessQuery
{
    //dataStreamId
    private const string DataSource = "raw:com.google.height:com.google.android.apps.fitness:user_input";
    private const string DataType = "com.google.height";

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightQuery"/> class.
    /// </summary>
    /// <param name="service">The service.</param>
    public WeightQuery(FitnessService service) :
        base(service, DataSource, DataType)
    {
    }

    /// <summary>
    /// Queries the weight.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>A list with weight data points.</returns>
    public IList<WeightDataPoint> QueryWeight(DateTime start, DateTime end)
    {
        var request = CreateRequest(start, end);
        var response = ExecuteRequest(request);

        return response
            .Bucket
            .SelectMany(b => b.Dataset)
            .Where(d => d.Point != null)
            .SelectMany(d => d.Point)
            .Where(p => p.Value != null)
            .SelectMany(p =>
            {
                return p.Value.Select(v =>
                    new WeightDataPoint
                    {
                        Weight = v.FpVal.GetValueOrDefault(),
                        Stamp = GoogleTime.FromNanoseconds(p.StartTimeNanos).ToDateTime()
                    });
            })
            .ToList();
    }

    /// <summary>
    /// Queries the weight.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>A list with weight data points.</returns>
    public IList<WeightDataPoint> QueryWeightPerDay(DateTime start, DateTime end)
    {
        return QueryWeight(start, end)
            .OrderBy(w => w.Stamp)
            .GroupBy(w => w.Stamp.Date)
            .Select(g => new WeightDataPoint
            {
                Stamp = g.Key,
                MaxWeight = g.Max(w => w.Weight),
                MinWeight = g.Min(w => w.Weight),
            })
            .ToList();
    }
}


/// <summary>
///     Helper class that hides the complexity of the fitness API.
/// </summary>
public abstract class FitnessQuery
{
    private readonly string _dataSourceId;
    private readonly string _dataType;
    private readonly FitnessService _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FitnessQuery" /> class.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="dataSourceId">The data source identifier.</param>
    /// <param name="dataType">Type of the data.</param>
    public FitnessQuery(FitnessService service, string dataSourceId, string dataType)
    {
        _service = service;
        _dataSourceId = dataSourceId;
        _dataType = dataType;
    }

    /// <summary>
    ///     Creates the request.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>The request.</returns>
    protected virtual AggregateRequest CreateRequest(DateTime start, DateTime end,
        TimeSpan? bucketDuration = null)
    {
        var bucketTimeSpan = bucketDuration.GetValueOrDefault(TimeSpan.FromDays(1));

        return new AggregateRequest
        {
            AggregateBy = new AggregateBy[]
            {
                new()
                {
                    DataSourceId = _dataSourceId,
                    DataTypeName = _dataType
                }
            },

            BucketByTime = new BucketByTime
            {
                DurationMillis = (long)bucketTimeSpan.TotalMilliseconds
            },

            StartTimeMillis = GoogleTime.FromDateTime(start).TotalMilliseconds,
            EndTimeMillis = GoogleTime.FromDateTime(end).TotalMilliseconds
        };
    }

    /// <summary>
    ///     Executes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>
    ///     The response.
    /// </returns>
    protected virtual AggregateResponse ExecuteRequest(AggregateRequest request, string userId = "me")
    {
        var agg = _service.Users.Dataset.Aggregate(request, userId);
        return agg.Execute();
    }
    
    /// <summary>
/// Helps converting between miliseconds since 1970-1-1 and C# <c>DateTime</c> object.
/// </summary>
public class GoogleTime
{
    private static readonly DateTime zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the total milliseconds.
    /// </summary>
    /// <value>
    /// The total milliseconds.
    /// </value>
    public long TotalMilliseconds { get; private set; }

    /// <summary>
    /// Prevents a default instance of the <see cref="GoogleTime"/> class from being created.
    /// </summary>
    private GoogleTime()
    {
    }

    /// <summary>
    /// Create a time object from the given date time.
    /// </summary>
    /// <param name="dt">The date time.</param>
    /// <returns>The time object.</returns>
    public static GoogleTime FromDateTime(DateTime dt)
    {
        if (dt < zero)
        {
            throw new Exception("Invalid Google datetime.");
        }

        return new GoogleTime
        {
            TotalMilliseconds = (long)(dt - zero).TotalMilliseconds,
        };
    }

    /// <summary>
    /// Creates a time object from the given nanoseconds.
    /// </summary>
    /// <param name="nanoseconds">The ns.</param>
    /// <returns>The time object.</returns>
    public static GoogleTime FromNanoseconds(long? nanoseconds)
    {
        if (nanoseconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nanoseconds), "Must be greater than 0.");
        }

        return new GoogleTime
        {
            TotalMilliseconds = (long)(nanoseconds.GetValueOrDefault(0) / 1000000)
        };
    }

    /// <summary>
    /// Gets the current time.
    /// </summary>
    /// <value>
    /// The current time.
    /// </value>
    public static GoogleTime Now
    {
        get { return FromDateTime(DateTime.Now); }
    }

    /// <summary>
    /// Adds the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns></returns>
    public GoogleTime Add(TimeSpan timeSpan)
    {
        return new GoogleTime
        {
            TotalMilliseconds = this.TotalMilliseconds + (long)timeSpan.TotalMilliseconds
        };
    }

    /// <summary>
    /// Converts this instance into a <c>DateTime</c> object.
    /// </summary>
    /// <returns>The date time.</returns>
    public DateTime ToDateTime()
    {
        return zero.AddMilliseconds(this.TotalMilliseconds);
    }
}
}