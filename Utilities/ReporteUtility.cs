using EduConnect_Front.Dtos;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace EduConnect_Front.Utilities
{
    public class ReporteUtility
    {
        public static ActionResult GenerarPdfTutorado(ControllerContext context, ReporteTutoradoDto reportData)
        {

            return new ViewAsPdf("ReporteTutorado", reportData)
            {
                FileName = $"ReporteTutorados_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };

        }
    }
}
