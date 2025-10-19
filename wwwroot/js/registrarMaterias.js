document.addEventListener("DOMContentLoaded", function () {
    const cards = Array.from(document.querySelectorAll(".materia-card"));
    const porPagina = 8;
    let paginaActual = 1;
    let cardsFiltradas = [...cards]; // lista dinámica según filtro

    // === Inicializar selección ===
    cards.forEach(card => {
        const checkbox = card.querySelector("input[type='checkbox']");
        if (checkbox.checked) card.classList.add("selected");

        card.addEventListener("click", (e) => {
            e.preventDefault();
            checkbox.checked = !checkbox.checked;
            card.classList.toggle("selected", checkbox.checked);
        });
    });

    // === Mostrar materias según página ===
    function mostrarPagina(pagina) {
        const inicio = (pagina - 1) * porPagina;
        const fin = inicio + porPagina;

        cards.forEach(card => card.style.display = "none"); // Oculta todas
        cardsFiltradas.forEach((card, index) => {
            if (index >= inicio && index < fin) card.style.display = "block";
        });

        document.getElementById("btnAnterior").disabled = pagina === 1;
        document.getElementById("btnSiguiente").disabled = fin >= cardsFiltradas.length;
        document.getElementById("paginaActual").textContent = `Página ${pagina}`;
    }

    // === Paginación ===
    document.getElementById("btnAnterior").addEventListener("click", () => {
        if (paginaActual > 1) {
            paginaActual--;
            mostrarPagina(paginaActual);
        }
    });

    document.getElementById("btnSiguiente").addEventListener("click", () => {
        if ((paginaActual * porPagina) < cardsFiltradas.length) {
            paginaActual++;
            mostrarPagina(paginaActual);
        }
    });

    // === Búsqueda y filtro ===
    const filtro = document.getElementById("filtroSemestre");
    const busqueda = document.getElementById("busquedaMateria");
    const btnBuscar = document.getElementById("btnBuscar");

    function aplicarFiltro() {
        const texto = busqueda.value.toLowerCase();
        const semestre = filtro.value;

        // 🔹 Aplica los filtros sobre todas las cards
        cardsFiltradas = cards.filter(card => {
            const nombre = card.dataset.nombre;
            const sem = card.dataset.semestre;
            const coincideNombre = nombre.includes(texto);
            const coincideSem = !semestre || semestre === sem;
            return coincideNombre && coincideSem;
        });

        // 🔹 Reinicia la paginación
        paginaActual = 1;
        mostrarPagina(paginaActual);
    }

    btnBuscar.addEventListener("click", aplicarFiltro);
    filtro.addEventListener("change", aplicarFiltro);
    busqueda.addEventListener("keyup", e => {
        if (e.key === "Enter") aplicarFiltro();
    });

    // === Mostrar primera página al cargar ===
    mostrarPagina(paginaActual);
});
