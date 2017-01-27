using System.Collections.Generic;

namespace WorldGen.Generator.Data
{
    /// <summary>
    /// A segment of a river.
    /// </summary>
    public class RiverSegment
    {
        /// <summary>
        /// The address of the cell containing this RiverSegment.
        /// </summary>
        public CellAddress Location { get; private set; }

        private RiverSegment _nextSegment;

        /// <summary>
        /// The river segments that empty into this segment.
        /// </summary>
        public List<RiverSegment> PreviousSegments { get; private set; }

        /// <summary>
        /// The parent river containing this segment.
        /// </summary>
        public River ParentRiver { get; private set; }

        /// <summary>
        /// The flow amount of the river in this segment.
        /// </summary>
        public int Flow { get; set; }

        internal RiverSegment(River parent, CellAddress location, IEnumerable<RiverSegment> previous)
        {
            ParentRiver = parent;
            Flow = 1;
            Location = location;
            PreviousSegments = new List<RiverSegment>(previous);
        }

        /// <summary>
        /// The next segment in the river. Will be null if this segment empties into the ocean.
        /// </summary>
        public RiverSegment NextSegment
        {
            get { return _nextSegment; }
            set
            {
                if (_nextSegment != null)
                {
                    _nextSegment.PreviousSegments.Remove(this);
                }

                _nextSegment = value;
                _nextSegment.PreviousSegments.Add(this);
            }
        }
    }
}
