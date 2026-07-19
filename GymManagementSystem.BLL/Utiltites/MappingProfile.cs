using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Member_ViewModels;
using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;
using GymManagementSystem.BLL.ViewModels.Session_ViewModels;
using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Utiltites
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapSession();
            MapMember();
            Maptrainer();
            MapPlans();
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

        public void MapMember()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
                .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street))
                .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City));

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => new Address
                    {
                        BuildingNumber = src.BuildingNumber,
                        Street = src.Street,
                        City = src.City
                    })
                )
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));
        }

        public void Maptrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialty.ToString()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialty))
                .ReverseMap()
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialties))
                .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
                .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street))
                .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City));

            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => new Address
                    {
                        BuildingNumber = src.BuildingNumber,
                        Street = src.Street,
                        City = src.City
                    })
                )
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialties));
        }

        public void MapPlans()
        {
            CreateMap<Plan, PlanViewModel>().ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationDays)).ReverseMap();
            CreateMap<Plan, PlanToUpdateViewModel>()
                .ForMember(dest => dest.PlanName,opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore());
        }
    }
}
