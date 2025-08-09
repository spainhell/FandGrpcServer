using Grpc.Core;

namespace GrpcServerLib.Services
{
    public class RdbService : GrpcServerLib.RdbService.RdbServiceBase
    {
        public override Task<OpenRDBResponse> OpenRDB(OpenRDBRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Request OpenRDB: '{request.RdbName}'");
            int chaptersCount = FandDllInterop.OpenRDB(request.RdbName);
            return Task.FromResult(new OpenRDBResponse
            {
                RecordsCount = chaptersCount
            });
        }

        public override Task<ChaptersCountResponse> GetChaptersCount(Empty request, ServerCallContext context)
        {
            Console.WriteLine("GRPC request GetChaptersCount");
            int chaptersCount = FandDllInterop.GetRecordsCount();
            return Task.FromResult(new ChaptersCountResponse
            {
                Count = chaptersCount
            });
        }

        public override Task<ChaptersListResponse> GetChaptersList(Empty request, ServerCallContext context)
        {
            Console.WriteLine("GRPC request GetChaptersList");
            int chaptersCount = FandDllInterop.GetRecordsCount();
            List<LoadChapterResponse> chaptersList = new List<LoadChapterResponse>();

            for (int i = 1; i <= chaptersCount; i++)
            {
                int loadResult = FandDllInterop.LoadRecord(i);
                string chapterType = FandDllInterop.GetChapterTypeString();
                string chapterName = FandDllInterop.GetChapterNameString();
                chaptersList.Add(new LoadChapterResponse()
                {
                    ChapterNumber = i,
                    ChapterType = chapterType,
                    ChapterName = chapterName,
                    ChapterText = ""
                });
            }
            
            return Task.FromResult(new ChaptersListResponse
            {
                ChaptersList = { chaptersList }
            });
        }

        public override Task<LoadChapterResponse> LoadChapter(LoadChapterRequest request, ServerCallContext context)
        {
              Console.WriteLine($"Request LoadChapter: '{request.ChapterNumber}'");
              int loadResult = FandDllInterop.LoadRecord(request.ChapterNumber);
              string chapterType = FandDllInterop.GetChapterTypeString();
              string chapterName = FandDllInterop.GetChapterNameString();
              string chapterText = FandDllInterop.GetChapterCodeString();
              return Task.FromResult(new LoadChapterResponse
              {
                  ChapterNumber = request.ChapterNumber,
                  ChapterType = chapterType,
                  ChapterName = chapterName,
                  ChapterText = chapterText
              });
        }

        public override Task<SaveChapterResponse> SaveChapter(SaveChapterRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Request SaveChapter: '{request.ChapterNumber}'");
            int saveResult = FandDllInterop.UpdateChapter(request.ChapterNumber, request.ChapterType, request.ChapterName, request.ChapterText);

            return Task.FromResult(new SaveChapterResponse
            {
                TotalRecordsCount = saveResult
            });
        }

        public override Task<CloseRdbResponse> CloseRdb(Empty request, ServerCallContext context)
        {
            int result = FandDllInterop.CloseRdb();
            return Task.FromResult(new CloseRdbResponse
            {
                FinalRecordsCount = result
            });
        }
    }
}
