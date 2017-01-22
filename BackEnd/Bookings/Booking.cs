using System;
using System.ComponentModel.DataAnnotations.Schema;
using XXX.CineCentral.Domain.Common;
using XXX.CineCentral.Domain.Common.DomainExceptions;
using XXX.CineCentral.Domain.Features;
using XXX.CineCentral.Domain.Sites;
using XXX.CineCentral.Domain.Time;

namespace XXX.CineCentral.Domain.Bookings
{
    public class Booking: Entity
    {
        public override long Id { get; protected set; }
        [Index]
        public DateTime StartMappedToDatabase { get; private set; }
        [Index]
        public DateTime? InclusiveEndMappedToDatabase { get; private set; }
        public long FeatureId { get; private set; }
        public virtual Feature Feature { get; private set; }
        public long SiteId { get; private set; }
        public virtual Site Site { get; private set; }

        [NotMapped]
        public CalendarDateRange DateRange
        {
            get
            {
                return new CalendarDateRange(new CalendarDate(StartMappedToDatabase), new CalendarDate(InclusiveEndMappedToDatabase));
            }
            set
            {
                SetStartDate(value.Start);
                InclusiveEndMappedToDatabase = value.InclusiveEnd.Date;
            }
        }


        private void SetStartDate(CalendarDate startDate)
        {
            if (startDate.Date.HasValue)
            {
                StartMappedToDatabase = startDate.Date.Value;    
            }
            else
            {
                throw new DomainException(DomainExceptionType.StartDateMustBeProvided);
            }
        }

        private decimal _term;
        public decimal Term
        {
            get { return _term; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new DomainException(DomainExceptionType.InvalidBookingTerm);
                }
                _term = value;
            }
        }

        protected Booking()
        {
        }

        public Booking(CalendarDateRange dateRange, Feature feature, Site site, decimal term)
        {
            DateRange = dateRange;
            Feature = feature;
            FeatureId = feature.Id;
            Site = site;
            SiteId = site.Id;
            Term = term;
        }

        public void Update(long featureId, long siteId, decimal term, CalendarDateRange bookingDateRange)
        {
            FeatureId = featureId;
            SiteId = siteId;
            Term = term;
            DateRange = bookingDateRange;
        }

        public override string ToString()
        {
            return String.Format("[Feature: {0}] [Site: {1}] [Date Range: {2}]",
                Feature,
                Site,
                DateRange);
        }
    }
}
