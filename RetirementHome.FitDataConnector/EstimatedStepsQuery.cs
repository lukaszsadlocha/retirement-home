using Google.Apis.Fitness.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetirementHome.FitDataConnector
{
    public class EstimatedStepsQuery : FitnessQuery
    {
        private const string DataSource = "derived:com.google.step_count.delta:com.google.android.gms:estimated_steps";
        private const string DataType = "com.google.step_count.delta";

        public EstimatedStepsQuery(FitnessService service) :
            base(service, DataSource, DataType)
        {
        }

        /// <summary>
        /// Queries the weight.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>A list with weight data points.</returns>
        public IList<EstimatedStepDataPoint> Query(DateTime start, DateTime end)
        {
            var request = CreateRequest(start, end);
            var response = ExecuteRequest(request);

            var list = response
                .Bucket
                .SelectMany(b => b.Dataset)
                .Where(d => d.Point != null)
                .SelectMany(d => d.Point)
                .Where(p => p.Value != null)
                .SelectMany(p =>
                {
                    return p.Value.Select(v =>
                        new EstimatedStepDataPoint
                        {
                            Steps = v.IntVal.GetValueOrDefault(),
                            Stamp = GoogleTime.FromNanoseconds(p.StartTimeNanos).ToDateTime()
                        });
                })
                .ToList();

            return list;
        }

        /// <summary>
        /// Queries the weight.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>A list with weight data points.</returns>
        public IList<EstimatedStepDataPoint> QueryStepPerDay(DateTime start, DateTime end)
        {
            return Query(start, end)
                .OrderBy(w => w.Stamp)
                .GroupBy(w => w.Stamp.Date)
                .Select(g => new EstimatedStepDataPoint
                {
                    Stamp = g.Key
                })
                .ToList();
        }
    }
}
