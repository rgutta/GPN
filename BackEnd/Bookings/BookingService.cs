using System.Collections.Generic;
using XXX.CineCentral.Domain.Common.EntityFramework;
using XXX.CineCentral.Domain.Features;
using XXX.CineCentral.Domain.Performances;
using XXX.CineCentral.Domain.Sites;
using XXX.CineCentral.Domain.Time;

namespace XXX.CineCentral.Domain.Bookings
{
    public interface IBookingService
    {
        IEnumerable<CalendarDateRange> GetValidBookingRanges(Site site, Feature feature);
        IEnumerable<Performance> GetPerformancesForBooking(Booking booking);
        Booking CreateFromDto(string start, string end, long featureId, long siteId, decimal term);
        void Update(Booking booking, long featureId, long siteId, decimal term, CalendarDateRange bookingDateRange);
        void Delete(Booking booking);
    }

    public class BookingService : IBookingService
    {
        private readonly IPerformanceRepository _performanceRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IFeatureRepository _featureRepository;
        private readonly IBookingValidator _bookingValidator;

        public BookingService(IPerformanceRepository performanceRepository, IBookingRepository bookingRepository,
            ISiteRepository siteRepository, IFeatureRepository featureRepository, IBookingValidator bookingValidator)
        {
            _performanceRepository = performanceRepository;
            _bookingRepository = bookingRepository;
            _siteRepository = siteRepository;
            _featureRepository = featureRepository;
            _bookingValidator = bookingValidator;
        }

        public IEnumerable<CalendarDateRange> GetValidBookingRanges(Site site, Feature feature)
        {
            return _bookingRepository.GetValidBookingRanges(site, feature);
        }

        public IEnumerable<Performance> GetPerformancesForBooking(Booking booking)
        {
            return _performanceRepository.GetPerformancesForBooking(booking);
        }

        public Booking CreateFromDto(string start, string end, long featureId, long siteId, decimal term)
        {
            var booking = new Booking(new CalendarDateRange(new CalendarDate(start), new CalendarDate(end)),
                _featureRepository.Get(featureId), _siteRepository.Get(siteId), term);

            _bookingValidator.ValidateBookingsDoNotOverlap(booking);
            return booking;
        }

        public void Update(Booking booking, long featureId, long siteId, decimal term, CalendarDateRange bookingDateRange)
        {
            var oldSite = booking.Site;
            var oldFeature = booking.Feature;

            booking.Update(featureId, siteId, term, bookingDateRange);
            _bookingValidator.ValidatePerformancesOfOldBookingFallWithinBooking(booking, oldSite, oldFeature);
            _bookingValidator.ValidateBookingsDoNotOverlap(booking);
        }

        public void Delete(Booking booking)
        {
            _bookingValidator.ValidateBookingWithDependentPerformanceCannotBeDeleted(booking);
        }

    }
}