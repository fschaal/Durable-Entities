using System.Net.Http;
using System.Threading.Tasks;
using HospitelBedManager.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;


namespace HospitelBedManager
{
    public static class BedManager
    {
        [FunctionName(nameof(AssignBed))]
        public static async Task<IActionResult> AssignBed(
            [HttpTrigger(AuthorizationLevel.Function, "get")]
            HttpRequest req,
            [DurableClient] IDurableEntityClient durableEntityClient,
            ILogger log)
        {
            var bedNumber = req.Query["bedNumber"];
            var entityId = new EntityId(nameof(BedEntity),bedNumber);
            
            log.LogInformation($"Received request for bed {bedNumber}");
            
            var bedEntity = await durableEntityClient.ReadEntityStateAsync<BedEntity>(entityId);
            if (bedEntity.EntityExists && bedEntity.EntityState.IsOccupied)
            {
                return new BadRequestObjectResult("Bed is already occupied.");
            }
            
            await durableEntityClient.SignalEntityAsync(entityId, nameof(BedEntity.AssignBedAsync));

            return new OkObjectResult($"Bed {bedNumber} has been set to occupied.");
        }
    }
}