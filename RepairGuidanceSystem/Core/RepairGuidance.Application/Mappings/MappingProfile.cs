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

            CreateMap<RepairRequest, RepairRequestDto>().ReverseMap();

            CreateMap<RepairStep, RepairStepDto>().ReverseMap();

            // Yeni isteği Dto şeklinde alırız, bunu entity'e tek yönlü dönüştürmek yeterlidir.
            CreateMap<CreateRepairRequestDto, RepairRequest>();
        
        }


    }
}
