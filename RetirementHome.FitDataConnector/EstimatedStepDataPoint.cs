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
    public class EstimatedStepDataPoint
    {
        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public int? Steps { get; set; }

        /// <summary>
        /// Gets or sets the stamp.
        /// </summary>
        /// <value>
        /// The stamp.
        /// </value>
        public DateTime Stamp { get; set; }
    }
}
