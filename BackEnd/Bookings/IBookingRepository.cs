using System.Linq;
using XXX.CineCentral.Domain.Common.EntityFramework;
using XXX.CineCentral.Domain.Common.Repositories;
using XXX.CineCentral.Domain.Features;
using XXX.CineCentral.Domain.Sites;
using XXX.CineCentral.Domain.Time;
using System.Collections.Generic;

namespace XXX.CineCentral.Domain.Bookings
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetBySiteId(long siteId);
        IEnumerable<Booking> Get(CalendarDateRange dateRange);
        IEnumerable<Booking> Get(Site site, Feature feature, CalendarDateRange dateRange);
        IEnumerable<CalendarDateRange> GetValidBookingRanges(Site site, Feature feature);
        IEnumerable<Booking> RelevantBookingsForSelectedSiteAndFeature(Site site, Feature feature);
    }
}