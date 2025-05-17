using Domain;
using Infrastracture;
using MediatR;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class UpdateWalorInstance
    {
        public class UpdateWalorInstanceCommand : IRequest<Result<Unit>>
        {
            public Guid WalorInstanceId { get; set; }
            public JsonDocument Data { get; set; }
        }

        public class Handler : IRequestHandler<UpdateWalorInstanceCommand, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(UpdateWalorInstanceCommand request, CancellationToken cancellationToken)
            {
                var walor = await _context.Walors.FindAsync(request.WalorInstanceId);

                if (walor == null)
                    return Result<Unit>.Failure("Walor not found");

                var walorTemplate = walor.Template;

                if (walorTemplate == null)
                    return Result<Unit>.Failure("Walor template not found for this instance");

                using (var schemaDocument = walorTemplate.Content)
                {
                    try
                    {
                        var schema = await JsonSchema.FromJsonAsync(schemaDocument.RootElement.ToString());

                        using (JsonDocument dataToValidate = request.Data)
                        {
                            var validationErrors = schema.Validate(dataToValidate.RootElement.ToString());

                            if (validationErrors.Any())
                            {
                                return Result<Unit>.Failure($"Walor data does not match the template");
                            }

                            walor.Data = request.Data;

                            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                            return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update walor instance.");
                        }
                    }
                    catch (JsonException ex)
                    {
                        return Result<Unit>.Failure($"Invalid JSON in walor template: {ex.Message}");
                    }

                }
            }
        }
    }

}
