
let filaSeleccionada = null;
let idSeleccionado = null;
// --- VER DETALLES ---
document.querySelectorAll(".boton-ver").forEach((boton) => {
    boton.addEventListener("click", function () {
        const datos = this.dataset;

        const lista = document.getElementById("detalles-lista");
        lista.innerHTML = `
            <li><b>Nombre:</b> ${datos.nombre} ${datos.apellido}</li>
            <li><b>Tipo de Documento:</b> ${datos.tipo}</li>
            <li><b>N° Documento:</b> ${datos.num}</li>
            <li><b>Teléfono:</b> ${datos.tel}</li>
            <li><b>Correo:</b> ${datos.correo}</li>
            <li><b>Carrera:</b> ${datos.carrera}</li>
            <li><b>Semestre:</b> ${datos.semestre}</li>
            <li><b>Rol:</b> ${datos.rol}</li>
            <li><b>Estado:</b> ${datos.estado}</li>
        `;

        document.getElementById("popup-detalles").style.display = "flex";
    });
});

// --- Cerrar popup detalles ---
document.querySelectorAll(".btn-cerrar-popup").forEach((btn) => {
    btn.addEventListener("click", () => {
        btn.closest(".popup").style.display = "none";
    });
});

// --- Evento para botón de eliminar/inactivar ---
document.querySelectorAll(".boton-eliminar").forEach((boton) => {
    boton.addEventListener("click", function () {
        filaSeleccionada = this.closest("tr");
        idSeleccionado = this.getAttribute("data-id");

        const estadoCelda = filaSeleccionada.querySelector(".estado");
        const estadoActual = estadoCelda ? estadoCelda.textContent.trim() : "Activo";

        const mensaje = estadoActual === "Activo"
            ? "¿Desea inactivar este usuario?"
            : "¿Desea activar este usuario?";
        document.querySelector("#popup-confirmacion p").textContent = mensaje;

        // Mostrar popup
        document.getElementById("popup-confirmacion").style.display = "flex";
    });
});

// --- Confirmar cambio de estado ---
document.getElementById("btn-si")?.addEventListener("click", async function () {
    if (!idSeleccionado) return;

    try {
        const response = await fetch(`/Administrador/EliminarUsuario?idUsuario=${idSeleccionado}`, {
            method: "POST" // 
        });

        if (response.ok) {
            // Actualiza visualmente
            const celdaEstado = filaSeleccionada.querySelector(".estado");
            const nuevoEstado = celdaEstado.textContent.trim() === "Activo" ? "Inactivo" : "Activo";
            celdaEstado.textContent = nuevoEstado;
        } else {
            const errText = await response.text();
            alert("Error al eliminar usuario: " + errText);
        }
    } catch (error) {
        console.error("Error al conectar con el servidor:", error);
        alert("No se pudo contactar con el servidor.");
    }

    document.getElementById("popup-confirmacion").style.display = "none";
});

// --- Cancelar popup ---
document.getElementById("btn-no")?.addEventListener("click", function () {
    document.getElementById("popup-confirmacion").style.display = "none";
});