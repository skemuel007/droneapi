using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone;
using Application.DTOs.Medication;
using Application.DTOs.Medication.Validators;
using Application.Features.Drone.Request.Queries;
using Application.Features.Medication.Request.Queries;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Medication.Handlers.Queries;

public class GetMedicationDetailRequestHandler : IRequestHandler<GetMedicationDetailRequest, BaseCommandResponse<MedicationDto>>
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IMapper _mapper;

    public GetMedicationDetailRequestHandler(IMedicationRepository medicationRepository,
        IMapper mapper)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<BaseCommandResponse<MedicationDto>> Handle(GetMedicationDetailRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new MedicationDetailsParamDtoValidator(_medicationRepository);
        var validationResult = await validator.ValidateAsync(request.MedicationDetailsParams);

        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse<MedicationDto>()
            {
                Message = "Get medication details validation error",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }
        
        var drone = await _medicationRepository.FirstOrDefaultAsync(predicate: (m =>
            m.Code == request.MedicationDetailsParams.Code ||
            m.Id == request.MedicationDetailsParams.Id));

        var medicationDto = _mapper.Map<MedicationDto>(drone);

        return new BaseCommandResponse<MedicationDto>()
        {
            Success = true,
            Data = medicationDto,
            Message = "Medication record found",
            StatusCode = HttpStatusCode.OK
        };
    }

}