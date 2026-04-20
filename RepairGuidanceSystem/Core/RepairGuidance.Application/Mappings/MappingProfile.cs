using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap();

            CreateMap<Tool, ToolDto>().ReverseMap();

            // Dto'daki ToolName Entity'deki Name'e karşılık geldiğinden
            // default yapılan otomatik eşleştirmede sorun olmaması için biz manuel gösterdik.
            CreateMap<UserTool, UserToolDto>()
                .ForMember(dest => dest.ToolName, opt => opt.MapFrom(src => src.Tool.Name)).ReverseMap();

            CreateMap<UserToolDto, UserTool>()
               .ForMember(dest => dest.Tool, opt => opt.Ignore()) 
               .ForMember(dest => dest.AppUser, opt => opt.Ignore());

            CreateMap<RepairRequest, RepairRequestDto>().ReverseMap();

            CreateMap<RepairStep, RepairStepDto>().ReverseMap();

            // Yeni isteği Dto şeklinde alırız, bunu entity'e tek yönlü dönüştürmek yeterlidir.
            CreateMap<CreateRepairRequestDto, RepairRequest>()
                .ForMember(dest=> dest.Id, opt=> opt.Ignore());

            CreateMap<UserRegisterDto, AppUser>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

        }


    }
}
