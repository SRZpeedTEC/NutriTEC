using AutoMapper;
using NutriTEC.Application.DTOs.Measurements;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Mappings;

public class MeasurementMappingProfile : Profile
{
    public MeasurementMappingProfile()
    {
        // Creation mapping copies body metrics while the service owns identity and date normalization.
        CreateMap<CreateMeasurementRequest, Measure>()
            .ForMember(destination => destination.MeasureDateTime, options => options.Ignore())
            .ForMember(destination => destination.Client, options => options.Ignore());

        // Updates cannot change the composite key or client relationship of an existing measurement.
        CreateMap<UpdateMeasurementRequest, Measure>()
            .ForMember(destination => destination.MeasureDateTime, options => options.Ignore())
            .ForMember(destination => destination.ClientId, options => options.Ignore())
            .ForMember(destination => destination.Client, options => options.Ignore());

        // Read models keep the API surface independent from EF navigation properties.
        CreateMap<Measure, MeasurementResponse>()
            .ForMember(destination => destination.MeasurementDate, options => options.MapFrom(source => source.MeasureDateTime));
    }
}
