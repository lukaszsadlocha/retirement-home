using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetirementHome.FitDataConnector
{
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
                TotalMilliseconds = nanoseconds.GetValueOrDefault(0) / 1000000
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
                TotalMilliseconds = TotalMilliseconds + (long)timeSpan.TotalMilliseconds
            };
        }

        /// <summary>
        /// Converts this instance into a <c>DateTime</c> object.
        /// </summary>
        /// <returns>The date time.</returns>
        public DateTime ToDateTime()
        {
            return zero.AddMilliseconds(TotalMilliseconds);
        }
    }
}