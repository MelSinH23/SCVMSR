using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCVMSR.Views.Helpers
{
    public static class HtmlHelpers
    {
        private static readonly Dictionary<string, string> Motivos = new Dictionary<string, string>
        {
            { "LicConGoce", "Licencia Con Goce de Salario" },
            { "LicSinGoce", "Licencia Sin Goce de Salario" },
            { "MaternidadPaternidad", "Maternidad/Paternidad" }
        };

        public static IHtmlString MotivoTexto(this HtmlHelper htmlHelper, string motivoValue)
        {
            return new HtmlString(Motivos.ContainsKey(motivoValue) ? Motivos[motivoValue] : motivoValue);
        }

    }
}