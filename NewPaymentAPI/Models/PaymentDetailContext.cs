using Microsoft.EntityFrameworkCore;

namespace NewPaymentAPI.Models
{
    public class PaymentDetailContext : DbContext
    {
        public PaymentDetailContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<PayementDetail> PayementDetails { get; set; }
    }
}
