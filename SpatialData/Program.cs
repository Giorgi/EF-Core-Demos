using Microsoft.EntityFrameworkCore;
using SpatialData.DataContext;

namespace SpatialData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new NycContext();

            var atlanticCommons = context.NycStreets.First(s => s.Name == "Atlantic Commons");

            #region What neighborhood and borough is Atlantic Commons in?
            var neighborhood = context.NycNeighborhoods.FirstOrDefault(n => n.Geom.Intersects(atlanticCommons.Geom));

            #endregion
            Console.WriteLine(neighborhood.Name);

            #region What streets does Atlantic Commons join with?

            var streets = context.NycStreets.Where(b => b.Geom.IsWithinDistance(atlanticCommons.Geom, 0.1)).ToList();

            #endregion

            #region Approximately how many people live on (within 50 meters of) Atlantic Commons?

            var totalPopulation = context.NycCensusBlocks.Where(b => b.Geom.IsWithinDistance(atlanticCommons.Geom, 50))
                                                                    .Sum(b => b.PopnTotal);
            #endregion

            #region What neighborhood is the ‘Broad St’ station in?

            var neighborhoods = from neighborhoodInfo in context.NycNeighborhoods
                                from station in context.NycSubwayStations
                                where neighborhoodInfo.Geom.Contains(station.Geom) &&
                                      station.Name == "Broad St"
                                select neighborhoodInfo;

            foreach (var n in neighborhoods)
            {
                Console.WriteLine($"{n.Name} {n.Boroname}");
            }
            #endregion

            #region What subway station is in ‘Little Italy’? What subway route is it on?

            var stations = from neighborhoodInfo in context.NycNeighborhoods
                           from station in context.NycSubwayStations
                           where neighborhoodInfo.Geom.Contains(station.Geom) &&
                                 neighborhoodInfo.Name == "Little Italy"
                           select station;

            Console.WriteLine(stations.First().Name);

            #endregion

            #region What is the population of ‘Battery Park’ neighborhood?

            totalPopulation = (from neighborhoodInfo in context.NycNeighborhoods
                               from censusBlock in context.NycCensusBlocks
                               where neighborhoodInfo.Geom.Intersects(censusBlock.Geom)
                                     && neighborhoodInfo.Name == "Battery Park"
                               select censusBlock.PopnTotal).Sum(); 
            #endregion


            Console.WriteLine("Hello, World!");
        }
    }
}