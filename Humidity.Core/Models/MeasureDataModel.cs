using System;

namespace Humidity.Core.Models
{
    /// <summary>
    /// The data model for rendering the charts
    /// </summary>
    public class MeasureDataModel
    {
        /// <summary>
        /// The displayed time on the X-axis
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        public double Value { get; set; }
    }
}
