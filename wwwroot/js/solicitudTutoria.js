
document.addEventListener("DOMContentLoaded", function () {
    const fechaInput = document.getElementById("fecha");
    const horaInput = document.getElementById("hora");

    // 🔹 Establece fecha mínima como hoy
    const hoy = new Date();
    const yyyy = hoy.getFullYear();
    const mm = String(hoy.getMonth() + 1).padStart(2, "0");
    const dd = String(hoy.getDate()).padStart(2, "0");
    fechaInput.min = `${yyyy}-${mm}-${dd}`;

    horaInput.disabled = true;

    fechaInput.addEventListener("change", function () {
        // ⚙️ Forzar la fecha seleccionada a horario local sin conversión UTC
        const partes = fechaInput.value.split("-");
        const fechaSeleccionada = new Date(partes[0], partes[1] - 1, partes[2]);

        // Crear referencia de "hoy" también normalizada (sin horas)
        const hoyLocal = new Date();
        hoyLocal.setHours(0, 0, 0, 0);
        fechaSeleccionada.setHours(0, 0, 0, 0);

        horaInput.disabled = false;
        horaInput.min = "06:00";
        horaInput.max = "21:00";

        // 🔹 Si la fecha seleccionada es HOY
        if (fechaSeleccionada.getTime() === hoyLocal.getTime()) {
            const ahora = new Date();
            const horaActual = ahora.getHours();
            const minutosActuales = ahora.getMinutes();

            if (horaActual >= 21) {
                horaInput.disabled = true;
                horaInput.value = "";
                alert("⚠️ Ya no puedes programar tutorías después de las 9:00 p. m.");
                return;
            }

            const minutosRedondeados = Math.ceil(minutosActuales / 5) * 5;
            const horaMin = `${String(Math.max(horaActual, 6)).padStart(2, "0")}:${String(minutosRedondeados).padStart(2, "0")}`;
            horaInput.min = horaMin;

            if (horaActual < 6) {
                horaInput.min = "06:00";
            }
        }
    });
});


