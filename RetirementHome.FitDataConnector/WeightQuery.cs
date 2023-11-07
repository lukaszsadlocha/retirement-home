using Google.Apis.Fitness.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetirementHome.FitDataConnector
{
    /// <summary>
    /// Queries the weight data source of Google Fit.
    /// </summary>
    public class WeightQuery : FitnessQuery
    {
        private const string DataSource = "derived:com.google.weight:com.google.android.gms:merge_weight";
        private const string DataType = "com.google.weight.summary";

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
}
