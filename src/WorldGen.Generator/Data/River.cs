using System.Collections.Generic;

namespace WorldGen.Generator.Data
{
    /// <summary>
    /// A river, composed of segments linking from one to another.
    /// </summary>
    public class River
    {
        /// <summary>
        /// The river this river empties into (if any).
        /// </summary>
        public River TributaryOf { get; set; }

        /// <summary>
        /// The segements of this river.
        /// </summary>
        public List<RiverSegment> Segments { get; private set; }

        /// <summary>
        /// Construct a new river.
        /// </summary>
        public River()
        {
            Segments = new List<RiverSegment>();
        }

        /// <summary>
        /// Add a segment to this river.
        /// </summary>
        /// <param name="location">The location of the segment in the world.</param>
        /// <param name="previousSegment">The RiverSegment that empties into this segment, optional.</param>
        /// <returns>The newly created river segment.</returns>
        public RiverSegment AddSegment(CellAddress location, RiverSegment previousSegment = null)
        {
            var previousSegments = new List<RiverSegment>();

            if (previousSegment != null)
            {
                previousSegments.Add(previousSegment);
            }

            var segment = new RiverSegment(this, location, previousSegments);

            this.Segments.Add(segment);

            if (previousSegment != null)
            {
                previousSegment.NextSegment = segment;
            }

            return segment;
        }
    }
}
