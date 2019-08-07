using AutoMapper;

namespace Back_end.Model
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Video, VideoDTO>();
            CreateMap<VideoDTO, Video>();
        }
    }
}
