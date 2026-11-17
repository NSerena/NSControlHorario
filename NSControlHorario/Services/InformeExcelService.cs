
using ClosedXML.Excel;
using NSControlHorarioApp.Models;
using System.IO;

namespace NSControlHorarioApp.Services
{
    public class InformeExcelService
    {
        public byte[] GenerarExcel(List<InformeHoras> informes)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Informe de horas");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Empleado";
            worksheet.Cell(1, 3).Value = "Horas trabajadas";
            worksheet.Cell(1, 4).Value = "Vacaciones";
            worksheet.Cell(1, 5).Value = "Baja médica";
            worksheet.Cell(1, 6).Value = "Permiso retribuido";
            worksheet.Cell(1, 7).Value = "Permiso no retribuido";
            worksheet.Cell(1, 8).Value = "Incidencias";

            var encabezado = worksheet.Range(1, 1, 1, 8);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.LightBlue;

            int fila = 2;

            foreach (var informe in informes)
            {
                worksheet.Cell(fila, 1).Value = informe.Fecha;
                worksheet.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy";

                worksheet.Cell(fila, 2).Value = informe.NombreEmpleado;

                worksheet.Cell(fila, 3).Value = (double)(informe.HorasTrabajadas / 24m);
                worksheet.Cell(fila, 4).Value = (double)(informe.HorasVacaciones / 24m);
                worksheet.Cell(fila, 5).Value = (double)(informe.HorasBaja / 24m);
                worksheet.Cell(fila, 6).Value = (double)(informe.HorasPermisoRetribuido / 24m);
                worksheet.Cell(fila, 7).Value = (double)(informe.HorasPermisoNoRetribuido / 24m);

                worksheet.Range(fila, 3, fila, 7).Style.NumberFormat.Format = "[h]:mm";

                worksheet.Cell(fila, 8).Value =
                    informe.TieneFichajePendiente ? "Fichaje pendiente" : "";

                fila++;
            }

            int filaTotal = fila;

            worksheet.Cell(filaTotal, 1).Value = "TOTALES";

            for (int columna = 3; columna <= 7; columna++)
            {
                worksheet.Cell(filaTotal, columna).FormulaA1 =
                    $"SUM({worksheet.Cell(2, columna).Address}:{worksheet.Cell(filaTotal - 1, columna).Address})";
            }

            // Formato de horas
            worksheet.Range(filaTotal, 3, filaTotal, 7)
                .Style.NumberFormat.Format = "[h]:mm";

            // Destacar la fila
            var rangoTotales = worksheet.Range(filaTotal, 1, filaTotal, 8);
            rangoTotales.Style.Font.Bold = true;
            rangoTotales.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            rangoTotales.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Cell(filaTotal, 8).Style.Font.Bold = true;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
