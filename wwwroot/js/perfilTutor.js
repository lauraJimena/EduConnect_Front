// === Generar estrellas de calificación ===
document.querySelectorAll('.calificacion').forEach(container => {
    const rating = parseInt(container.dataset.rating);
    const estrellasDiv = container.querySelector('.estrellas');
    let html = '';

    for (let i = 1; i <= 5; i++) {
        html += i <= rating
            ? '<i class="fas fa-star" style="color:#fbc959;"></i>'
            : '<i class="far fa-star" style="color:#ccc;"></i>';
    }

    estrellasDiv.innerHTML = html;
});

// === Modal abrir/cerrar ===
const modal = document.getElementById("modalValoracion");
const abrir = document.getElementById("abrirModal");
const cerrar = document.getElementById("cerrarModal");
const cerrarSuperior = document.getElementById("cerrarModalSuperior"); // ✅ nuevo botón “X”

// ✅ Función para cerrar con animación suave
function cerrarModalConAnimacion() {
    if (!modal) return;
    modal.classList.add("cerrando");
    setTimeout(() => {
        modal.style.display = "none";
        modal.classList.remove("cerrando");
    }, 200); // duración igual a la animación CSS
}

if (abrir && modal) {
    abrir.addEventListener("click", () => {
        modal.style.display = "flex";
        modal.classList.remove("cerrando");
    });
}

if (cerrar) {
    cerrar.addEventListener("click", cerrarModalConAnimacion);
}

if (cerrarSuperior) { // ✅ botón “X”
    cerrarSuperior.addEventListener("click", cerrarModalConAnimacion);
}

window.addEventListener("click", (e) => {
    if (e.target === modal) cerrarModalConAnimacion();
});

// === Enviar formulario con Fetch ===
const form = document.getElementById("formValoracion");
if (form) {
    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const data = new FormData(form);
        const response = await fetch('/Tutorado/AgregarValoracion', {
            method: 'POST',
            body: data
        });

        if (response.ok) {
            alert("✅ Comentario enviado correctamente");
            cerrarModalConAnimacion();
            setTimeout(() => location.reload(), 500);
        } else {
            const error = await response.text();
            alert("❌ Error: " + error);
        }
    });
}

// === PAGINACIÓN DE COMENTARIOS ===
const comentarios = Array.from(document.querySelectorAll("#comentarios-container .comentario-card"));
const porPagina = 3;
let paginaActual = 1;

function mostrarPagina(pagina) {
    const inicio = (pagina - 1) * porPagina;
    const fin = inicio + porPagina;

    comentarios.forEach((c, i) => {
        c.style.display = (i >= inicio && i < fin) ? "block" : "none";
    });

    document.getElementById("paginaActual").textContent = pagina;
    document.getElementById("btnPrev").disabled = pagina === 1;
    document.getElementById("btnNext").disabled = fin >= comentarios.length;
}

// Inicializar paginación
if (comentarios.length > 0) {
    mostrarPagina(paginaActual);

    document.getElementById("btnPrev").addEventListener("click", () => {
        if (paginaActual > 1) {
            paginaActual--;
            mostrarPagina(paginaActual);
        }
    });

    document.getElementById("btnNext").addEventListener("click", () => {
        if (paginaActual * porPagina < comentarios.length) {
            paginaActual++;
            mostrarPagina(paginaActual);
        }
    });
}

