using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;
using Grpc.Net.Client.Web;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine("--> Calling GRPC Service "+_configuration["GrpcPlatform"]);
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            // , new GrpcChannelOptions
            // {
            //     HttpHandler = new GrpcWebHandler{
            //         //http version set to 1_1 beacause this is the version 
            //         // the server accepts but the default version that grpc uses is http2
            //         //so i used a workaround here and set the version to http 1_1                
            //         HttpVersion = new Version(1,1),
            //         InnerHandler = new HttpClientHandler()
            //     }
                
            // });
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}