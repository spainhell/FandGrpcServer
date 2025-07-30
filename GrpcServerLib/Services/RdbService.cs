using Grpc.Core;

namespace GrpcServerLib.Services
{
    public class RdbService : GrpcServerLib.RdbService.RdbServiceBase
    {
        public override Task<OpenRDBResponse> OpenRDB(OpenRDBRequest request, ServerCallContext context)
        {
            Console.WriteLine("Request OpenRDB: " + request.RdbName + "!");
            int chaptersCount = FandDllInterop.OpenRDB(request.RdbName);
            return Task.FromResult(new OpenRDBResponse
            {
                RecordsCount = chaptersCount
            });
        }
    }
}
