function abrirModalCerrarSesion() {
    const popup = document.getElementById("modalCerrarSesion");

    // 🔹 Si está visible → lo oculta, si no → lo muestra
    if (popup.style.display === "block") {
        popup.style.display = "none";
    } else {
        popup.style.display = "block";
    }
}

function cerrarModalCerrarSesion() {
    document.getElementById("modalCerrarSesion").style.display = "none";
}

// 🔹 Cierra el popup si se hace clic fuera del mismo
document.addEventListener("click", function (event) {
    const popup = document.getElementById("modalCerrarSesion");
    const usuarioInfo = document.querySelector(".usuario-info");

    // Si el popup está abierto y el clic no fue dentro de él ni en el botón del usuario
    if (
        popup.style.display === "block" &&
        !popup.contains(event.target) &&
        !usuarioInfo.contains(event.target)
    ) {
        popup.style.display = "none";
    }
});