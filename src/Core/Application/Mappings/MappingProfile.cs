using Application.DTOs.Drone;
using Application.DTOs.DroneRequest;
using Application.DTOs.Medication;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateDroneDto, Drone>().ReverseMap();
        CreateMap<Drone, UpdateDroneDto>().ReverseMap();
        CreateMap<Drone, DroneDto>()
            .ForMember(d => d.DroneModel, 
                op => op.MapFrom(src => src.Model.ToString()))
            .ForMember(d => d.DroneState, 
                op => op.MapFrom(src => src.State.ToString()));


        CreateMap<CreateMedicationDto, Medication>().ReverseMap();
        CreateMap<Medication, MedicationDto>().ReverseMap();

        CreateMap<CreateDroneRequestDto, DroneRequest>().ReverseMap();
        CreateMap<CreateDroneRequestResponseDto, DroneRequest>().ReverseMap();
    }
}