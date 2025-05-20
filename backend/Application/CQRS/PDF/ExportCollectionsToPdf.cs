using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;

public class ExportCollectionsToPdf
{
    public class Query : IRequest<byte[]>
    {
        public Guid UserId { get; set; }
        public string? Category { get; set; }
    }

    public class Handler : IRequestHandler<Query, byte[]>
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var query = _context.Collections
                .Include(c => c.Walors)
                .Include(c => c.WalorTemplate)
                .Where(c => c.OwnerId == user.Id);

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(c => c.WalorTemplate.Category == request.Category);

            var collections = await query.ToListAsync(cancellationToken);

            var document = CreatePdfDocument(collections);
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        private IDocument CreatePdfDocument(List<Collection> collections)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Collections Export")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Column(col =>
                        {
                            foreach (var collection in collections)
                            {
                                col.Item().Text($"Title: {collection.Title}").Bold();
                                col.Item().Text($"Description: {collection.Description}");
                                col.Item().Text($"Template Category: {collection.WalorTemplate.Category}");
                                col.Item().Text("Walor Instances:");

                                // 1. Pobierz listę pól z szablonu
                                List<string> templateFields = ExtractFieldsFromTemplate(collection.WalorTemplate.Content);

                                col.Item().Table(table =>
                                {
                                    List<string> templateFields = ExtractFieldsFromTemplate(collection.WalorTemplate.Content);

                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Id
                                        foreach (var _ in templateFields)
                                            columns.RelativeColumn(3);
                                    });



                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Instance Id").Bold();
                                        foreach (var field in templateFields)
                                            header.Cell().Element(CellStyle).Text(field).Bold();
                                    });

                                    foreach (var walor in collection.Walors)
                                    {
                                        table.Cell().Element(CellStyle).Text(walor.Id.ToString());

                                        var data = walor.Data.RootElement;

                                        foreach (var field in templateFields)
                                        {
                                            string value = "null";

                                            if (data.TryGetProperty(field, out var prop))
                                            {
                                                switch (prop.ValueKind)
                                                {
                                                    case JsonValueKind.String:
                                                        value = prop.GetString() ?? "null";
                                                        break;
                                                    case JsonValueKind.Number:
                                                        value = prop.GetRawText();
                                                        break;
                                                    case JsonValueKind.True:
                                                    case JsonValueKind.False:
                                                        value = prop.GetBoolean().ToString();
                                                        break;
                                                    default:
                                                        value = prop.ToString();
                                                        break;
                                                }
                                            }

                                            table.Cell().Element(CellStyle).Text(value);
                                        }
                                    }
                                });

                                col.Item().PaddingVertical(10);
                            }
                        });

                });
            });

            static IContainer CellStyle(IContainer container)
            {
                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            }
        }

        // Metoda pomocnicza do wyciągania listy pól z JSON szablonu
        private List<string> ExtractFieldsFromTemplate(JsonDocument templateContent)
        {
            var fields = new List<string>();

            if (templateContent.RootElement.TryGetProperty("properties", out var properties) &&
                properties.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in properties.EnumerateObject())
                {
                    fields.Add(property.Name);
                }
            }

            return fields;
        }

    }
}
