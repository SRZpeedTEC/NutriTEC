using AutoMapper;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // Request-to-entity mappings keep controller and service code focused on workflow decisions.
        CreateMap<RegisterClientRequest, User>()
            .ForMember(destination => destination.UserId, options => options.Ignore())
            .ForMember(destination => destination.HashPassword, options => options.Ignore())
            .ForMember(destination => destination.Age, options => options.Ignore())
            .ForMember(destination => destination.Client, options => options.Ignore());

        CreateMap<RegisterClientRequest, Client>()
            .ForMember(destination => destination.ClientId, options => options.Ignore())
            .ForMember(destination => destination.User, options => options.Ignore())
            .ForMember(destination => destination.UserId, options => options.Ignore())
            .ForMember(destination => destination.Measures, options => options.Ignore());

        CreateMap<RegisterClientRequest, Measure>()
            .ForMember(destination => destination.MeasureDateTime, options => options.Ignore())
            .ForMember(destination => destination.Client, options => options.Ignore())
            .ForMember(destination => destination.ClientId, options => options.Ignore());
    }
}
