document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("modalReportes");
    const btnAbrir = document.getElementById("abrirModalReportes");
    const btnCerrar = document.getElementById("cerrarModalReportes");
    const btnCancelar = document.getElementById("btnCancelar");
    const btnGenerar = document.getElementById("btnGenerar");

    // === Abrir modal ===
    btnAbrir.addEventListener("click", () => {
        modal.style.display = "flex";
    });

    // === Cerrar modal ===
    [btnCerrar, btnCancelar].forEach(btn => {
        btn.addEventListener("click", () => {
            modal.style.display = "none";
        });
    });

    // === Click fuera del modal ===
    window.addEventListener("click", (e) => {
        if (e.target === modal) modal.style.display = "none";
    });

    // === Generar reportes ===
    btnGenerar.addEventListener("click", () => {
        const chkTutores = document.getElementById("chkTutores").checked;
        const chkTutorados = document.getElementById("chkTutorados").checked;

        if (!chkTutores && !chkTutorados) {
            alert("⚠️ Debes seleccionar al menos un reporte.");
            return;
        }

        // URLs de los controladores
        const urlTutores = '/Coordinador/ReporteDemandaAcademicaPdf';
        const urlTutorados = '/Coordinador/ReporteCombinadoPdf';

        // Abrir los PDFs seleccionados
        if (chkTutores) window.open(urlTutores, '_blank');
        if (chkTutorados) window.open(urlTutorados, '_blank');

        // Cerrar modal
        modal.style.display = "none";
    });
});
