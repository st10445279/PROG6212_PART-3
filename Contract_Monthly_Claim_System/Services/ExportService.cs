using System.Globalization;
using ClosedXML.Excel;
using Contract_Monthly_Claim_System.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Contract_Monthly_Claim_System.Services
{
    public class ExportService : IExportService
    {
        public byte[] ExportClaimsToExcel(List<Claim> claims)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Claims");

            // Headers
            worksheet.Cell(1, 1).Value = "Claim ID";
            worksheet.Cell(1, 2).Value = "Lecturer Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Hours Worked";
            worksheet.Cell(1, 5).Value = "Hourly Rate";
            worksheet.Cell(1, 6).Value = "Total Amount";
            worksheet.Cell(1, 7).Value = "Status";
            worksheet.Cell(1, 8).Value = "Submitted Date";
            worksheet.Cell(1, 9).Value = "Approved Date";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            int row = 2;
            foreach (var claim in claims)
            {
                worksheet.Cell(row, 1).Value = claim.ClaimId;
                worksheet.Cell(row, 2).Value = claim.Lecturer != null ? $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}" : "N/A";
                worksheet.Cell(row, 3).Value = claim.Lecturer?.Email ?? "N/A";
                worksheet.Cell(row, 4).Value = claim.HoursWorked;
                worksheet.Cell(row, 5).Value = claim.HourlyRate;
                worksheet.Cell(row, 6).Value = claim.TotalAmount;
                worksheet.Cell(row, 7).Value = claim.Status;
                worksheet.Cell(row, 8).Value = claim.SubmittedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 9).Value = claim.ApprovedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportClaimsToPdf(List<Claim> claims)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text("Approved Claims Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Claim ID");
                                header.Cell().Element(CellStyle).Text("Lecturer");
                                header.Cell().Element(CellStyle).Text("Email");
                                header.Cell().Element(CellStyle).Text("Hours");
                                header.Cell().Element(CellStyle).Text("Rate");
                                header.Cell().Element(CellStyle).Text("Total Amount");
                                header.Cell().Element(CellStyle).Text("Status");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var claim in claims)
                            {
                                table.Cell().Element(CellStyle).Text(claim.ClaimId.ToString());
                                table.Cell().Element(CellStyle).Text(claim.Lecturer != null ? $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}" : "N/A");
                                table.Cell().Element(CellStyle).Text(claim.Lecturer?.Email ?? "N/A");
                                table.Cell().Element(CellStyle).Text(claim.HoursWorked.ToString("F2"));
                                table.Cell().Element(CellStyle).Text($"R{claim.HourlyRate:F2}");
                                table.Cell().Element(CellStyle).Text($"R{claim.TotalAmount:F2}");
                                table.Cell().Element(CellStyle).Text(claim.Status);

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)).SemiBold();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
