using XXX.CineCentral.Domain.Common.DomainExceptions;
using System.Collections.Generic;
using System.Linq;
using XXX.CineCentral.Domain.Common.EntityFramework;
using XXX.CineCentral.Domain.Performances;
using XXX.CineCentral.Domain.Performances.Events;
using XXX.CineCentral.Domain.Sites;
using XXX.CineCentral.Domain.Time;
using XXX.CineCentral.Domain.Features;

namespace XXX.CineCentral.Domain.Bookings
{
    public interface IBookingValidator
    {
        void ValidatePerformancesOfOldBookingFallWithinBooking(Booking booking, Site oldSite, Feature oldFeature);
        void ValidateBookingWithDependentPerformanceCannotBeDeleted(Booking booking);
        void ValidateBookingsDoNotOverlap(Booking booking);
    }

    public class BookingValidator : IBookingValidator
    {
        private readonly IPerformanceRepository _performanceRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingValidator(IPerformanceRepository performanceRepository, IBookingRepository bookingRepository)
        {
            _performanceRepository = performanceRepository;
            _bookingRepository = bookingRepository;
        }

        public void ValidatePerformancesOfOldBookingFallWithinBooking(Booking booking, Site oldSite, Feature oldFeature)
        {
            var auditoriumIds = oldSite.Auditoriums.Select(x => x.Id).ToList();

            var affectedPerformances = _performanceRepository.PerformancesForSelectedFeatureAndAuditorium(auditoriumIds, oldFeature).ToList();
            var relevantBookings = _bookingRepository.RelevantBookingsForSelectedSiteAndFeature(oldSite, oldFeature).ToList();

            affectedPerformances.ForEach(x =>
            {
                var withinBookings = PerformanceValidations.IsPerformanceWithinBookings(relevantBookings.AsQueryable(), x);
                if (!withinBookings)
                {
                    throw new DomainException(DomainExceptionType.CannotUpdateBookingAsPerformanceExists,
                        new Dictionary<string, object>
                        {
                            {"featureName", oldFeature.Name},
                            {"siteName", oldSite.Name},
                            {"performanceShowtime", x.Showtime.GetLocalOffset(oldSite.TimeZone) }
                        });
                }
            });
        }

        public void ValidateBookingWithDependentPerformanceCannotBeDeleted(Booking booking)
        {
            var performancesExistForBooking = _performanceRepository.ValidateIfBookingHasPerformances(booking);
            if (performancesExistForBooking)
            {
                throw new DomainException(DomainExceptionType.DependantPerformancesExistForBooking);
            }
        }

        public void ValidateBookingsDoNotOverlap(Booking booking)
        {
            var overlappingBooking = _bookingRepository.Get(booking.Site, booking.Feature, booking.DateRange)
                .FirstOrDefault(x => x.Id != booking.Id);

            if (overlappingBooking != null)
            {
                throw new DomainException(DomainExceptionType.CannotHaveOverlappingBookingForTheSameFeature,
                    new Dictionary<string, object>
                    {
                        {"featureName", booking.Feature.Name},
                        {"existingDateRange", overlappingBooking.DateRange.ToString()},
                        {"newDateRange", booking.DateRange.ToString()}
                    });
            }
        }
    }
}
