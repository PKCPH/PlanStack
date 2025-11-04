using AutoMapper;

namespace PlanStack.UnitTest.Managers
{
    public static class AutoMapperManager
    {
        private static readonly object _lock = new object();

        private static IMapper _mapper;

        // This will initialize AutoMapperManager generating the mapping config
        public static void Initialize()
        {
            var config = new MapperConfiguration(cfg => {
                // Gets all profiles from Assably
                cfg.AddMaps(["PlanStack.Backend.App.WebAPI"]);
            });

            AutoMapperManager._mapper = config.CreateMapper();
        }

        // Will return and instance of the currently generated mapper
        public static IMapper GetMapper()
        {
            lock (_lock)
            {
                // Initializing Mapper
                if (AutoMapperManager._mapper == null)
                    AutoMapperManager.Initialize();

                return AutoMapperManager._mapper;
            }
        }
    }
}