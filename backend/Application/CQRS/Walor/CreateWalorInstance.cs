using Domain;
using Infrastracture;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class CreateWalorInstance
    {
        public class CreateWalorInstanceCommand : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
            public JsonDocument Data { get; set; }
        }

        public class Handler : IRequestHandler<CreateWalorInstanceCommand, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(CreateWalorInstanceCommand request, CancellationToken cancellationToken)
            {
                var collection = await _context.Collections
    .Include(c => c.WalorTemplate)
    .FirstOrDefaultAsync(c => c.Id == request.CollectionId);

                if (collection == null)
                    return Result<Unit>.Failure("collection not found");

                try
                {

                    var templateContent = EnsureParsedJson(collection.WalorTemplate.Content);
                    var schema = await JsonSchema.FromJsonAsync(templateContent.RootElement.ToString());


                    var parsedData = EnsureParsedJson(request.Data);

                    // Walidacja
                    var validationErrors = schema.Validate(parsedData.RootElement.ToString());
                    if (validationErrors.Any())
                    {
                        return Result<Unit>.Failure("Walor data does not match the template");
                    }

                    var walor = new WalorInstance
                    {
                        CollectionId = collection.Id,
                        TemplateId = collection.WalorTemplateId,
                        Data = parsedData
                    };

                    _context.Walors.Add(walor);
                    var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                    return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error saving walor");
                }
                catch (JsonException ex)
                {
                    return Result<Unit>.Failure($"Invalid JSON: {ex.Message}");
                }
            }
        }
        private static JsonDocument EnsureParsedJson(JsonDocument doc)
        {
            if (doc.RootElement.ValueKind == JsonValueKind.String)
            {
                var jsonText = doc.RootElement.GetString();
                return JsonDocument.Parse(jsonText!);
            }

            return doc;
        }
    }

}
