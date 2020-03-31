using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace HospitelBedManager.Entities
{
    public class BedEntity
    {
        public string BedNumber { get; set; }
        public bool IsOccupied { get; set; }
        
        private readonly ILogger _logger;

        public BedEntity(ILogger logger, string bedNumber)
        {
            _logger = logger;
            BedNumber = bedNumber;
        }
        
        [FunctionName(nameof(BedEntity))]
        public static async Task HandleEntityOperation(
            [EntityTrigger] IDurableEntityContext context,
            ILogger logger)
        {
            await context.DispatchAsync<BedEntity>(logger, context.EntityKey);
        }
        
        public Task<bool> IsOccupiedBedAsync()
        {
            var IsOccupied = BedNumber == "123";
            return Task.FromResult(IsOccupied);
        }
        
        public Task AssignBedAsync(string bedNumber)
        {
            IsOccupied = true;
            return Task.CompletedTask;
        }
    }
}