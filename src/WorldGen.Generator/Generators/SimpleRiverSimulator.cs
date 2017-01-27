using System;
using System.Collections.Generic;
using System.Linq;
using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A simple river simulator, places springs and runs rivers to the ocean, aborting if it 
    /// reaches a local minima in the heightmap.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class SimpleRiverSimulator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell, IRiverCell
    {
        private const double SPRING_CHANCE_MOD = 0.004;
        private const int RIVER_MIN_LENGTH = 3;
        
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Simple River Simulator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var springs = PlaceSprings(world);

            foreach (var spring in springs)
            {
                RunRiver(world, spring);
            }
        }

        private List<CellAddress> PlaceSprings(TWorld world)
        {
            var rnd = new Random(world.Seed + "springs".GetHashCode());

            var springs = new List<CellAddress>();

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    if (!IsOcean(x, y, world))
                    {
                        if (rnd.NextDouble() <= world.GetCell(x, y).Rainfall * SPRING_CHANCE_MOD)
                        {
                            springs.Add(new CellAddress(x, y));
                        }
                    }
                }
            }

            return springs;
        }

        private void RunRiver(TWorld world, CellAddress springLocation)
        {
            var rnd = new Random(world.Seed + "rivers".GetHashCode());

            var current = springLocation;

            var river = new River();

            var previousSegment = (RiverSegment)null;

            while (!IsOcean(current.X, current.Y, world))
            {
                var currentCell = world.GetCell(current.X, current.Y);

                if (currentCell.RiverSegmentHere != null)
                {
                    // Now a tributary
                    if (river.Segments.Count >= RIVER_MIN_LENGTH)
                    {
                        currentCell.RiverSegmentHere.PreviousSegments.Add(previousSegment);
                        previousSegment.NextSegment = currentCell.RiverSegmentHere;

                        river.TributaryOf = currentCell.RiverSegmentHere.ParentRiver;
                        //world.AddRiver(river);

                        AddFlow(currentCell.RiverSegmentHere, 1);

                        return;
                    }

                    //Too short, abort
                    return;
                }

                previousSegment = river.AddSegment(current, previousSegment);

                current = PickNextLocation(world, current, river, rnd);

                if (current == null)
                {
                    //Dead end, abort
                    return;
                }
            }

            //Add the current location to the end of the river.
            river.AddSegment(current, previousSegment);

            if (river.Segments.Count < RIVER_MIN_LENGTH)
            {
                //Too short, abort
                return;
            }

            //Add river to world
            //world.AddRiver(river);
        }

        private void AddFlow(RiverSegment riverSegment, int flow)
        {
            while (riverSegment != null)
            {
                riverSegment.Flow += flow;

                riverSegment = riverSegment.NextSegment;
            }
        }

        private CellAddress PickNextLocation(TWorld world, CellAddress current, River river, Random rnd)
        {
            var eligable = new List<CellAddress>();

            var oceans = new List<CellAddress>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var n = new CellAddress(i + current.X, j + current.Y);

                    if ((i == 0 || j == 0) && n != current
                        && !river.Segments.Any(x => x.Location == n)
                        && world.InRange(n.X, n.Y))
                    {
                        eligable.Add(n);

                        if (world.GetCell(n.X, n.Y).Height < world.SeaLevel)
                        {
                            oceans.Add(n);
                        }
                    }
                }
            }

            if (oceans.Any())
            {
                return rnd.PickRandom(oceans);
            }

            eligable = eligable
                .Where(ca => world.GetCell(ca.X, ca.Y).Height <= world.GetCell(current.X, current.Y).Height)
                .ToList();

            if (eligable.Any(ca => world.GetCell(ca.X, ca.Y).RiverSegmentHere != null))
            {
                return eligable.First(ca => world.GetCell(ca.X, ca.Y).RiverSegmentHere != null);
            }

            if (eligable.Any())
            {
                return rnd.PickRandom(eligable);
            }

            return null;
        }

        private bool IsOcean(int x, int y, TWorld world)
        {
            return world.GetCell(x, y).Height < world.SeaLevel;
        }
    }
}
