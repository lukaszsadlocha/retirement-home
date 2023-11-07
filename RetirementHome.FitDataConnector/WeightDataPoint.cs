using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetirementHome.FitDataConnector
{
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
}
