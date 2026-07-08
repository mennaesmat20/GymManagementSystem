using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Session_ViewModels;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Utiltites
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapSession();
        }
        public void MapSession()
        {
            CreateMap<Session, SessionViewModel>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                                                  .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                                                  .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore()).ReverseMap();

            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }
    }
}
