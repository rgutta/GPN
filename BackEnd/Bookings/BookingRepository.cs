using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using XXX.CineCentral.Domain.Common.EntityFramework;
using XXX.CineCentral.Domain.Common.Repositories;
using XXX.CineCentral.Domain.Features;
using XXX.CineCentral.Domain.Sites;
using XXX.CineCentral.Domain.Time;

namespace XXX.CineCentral.Domain.Bookings
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        protected override IQueryable<Booking> Include(IQueryable<Booking> query)
        {
            return query
                .Include(x => x.Site)
                .Include(x => x.Feature);
        }

        private static IQueryable<Booking> Get(IQueryable<Booking> query, CalendarDateRange dateRange)
        {
            var filterStart = dateRange.Start.Date;
            var filterInclusiveEnd = dateRange.InclusiveEnd.Date;
            return query
                .Where(x => filterStart == null
                    || (x.InclusiveEndMappedToDatabase != null && x.InclusiveEndMappedToDatabase >= filterStart.Value)
                    || x.InclusiveEndMappedToDatabase == null)
                .Where(x => filterInclusiveEnd == null
                    || x.StartMappedToDatabase <= filterInclusiveEnd.Value);
        }

        public IEnumerable<Booking> GetBySiteId(long siteId)
        {
            var query = _dbContext.Set<Booking>()
                .Where(x => x.SiteId == siteId);
            return Include(query);
        }

        public IEnumerable<Booking> Get(CalendarDateRange dateRange)
        {
            var query = Get(_dbContext.Set<Booking>(), dateRange);
            return Include(query);
        }

        public IEnumerable<Booking> Get(Site site, Feature feature, CalendarDateRange dateRange)
        {
            var query = _dbContext.Set<Booking>()
                .Where(x => x.SiteId == site.Id)
                .Where(x => x.FeatureId == feature.Id);
            var byDateRangeQuery = Get(query, dateRange);
            return Include(byDateRangeQuery);
        }

        public IEnumerable<Booking> RelevantBookingsForSelectedSiteAndFeature(Site site, Feature feature)
        {
            return _dbContext.Set<Booking>().Where(x =>
                   x.FeatureId == feature.Id
                   && x.SiteId == site.Id);
        }

        public IEnumerable<CalendarDateRange> GetValidBookingRanges(Site site, Feature feature)
        {
            return _dbContext.Set<Booking>()
                .Where(x => x.SiteId == site.Id
                            && x.FeatureId == feature.Id)
                .Select(x => new { Start = x.StartMappedToDatabase, InclusiveEnd = x.InclusiveEndMappedToDatabase })
                .AsEnumerable()
                .Select(x => new CalendarDateRange(
                    new CalendarDate(x.Start),
                    new CalendarDate(x.InclusiveEnd)));
        }
    }
}
