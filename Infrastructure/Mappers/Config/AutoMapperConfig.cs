using AutoMapper;

namespace MusicWebAppBackend.Infrastructure.Mappers.Config
{
    public static class AutoMapperConfig
    {
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Initialize mapper
        /// </summary>
        /// <param name="config">Mapper configuration</param>
        public static void Init(MapperConfiguration config)
        {
            Mapper = config.CreateMapper();
        }
    }
}
