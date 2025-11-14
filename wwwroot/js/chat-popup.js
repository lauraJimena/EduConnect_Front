

$(function () {
    // ✅ Mostrar/Ocultar lista de chats
    $('.boton-chat').on('click', function () {
        const $popup = $('#popupChat');
        if ($popup.is(':visible')) {
            $popup.hide();
        } else {
            $.get('/Chats/Lista', function (data) {
                $popup.html(data).show();
            });
        }
    });
});


// ✅ Abrir conversación al hacer clic en un chat
$(document).on('click', '.popup-chat-item', function () {
    const chatId = $(this).data('chat-id');
    const nombre = $(this).data('nombre');
    const materia = $(this).data('materia');
    const idUsuarioLogueado = $('#popupChat').data('id-usuario'); // <-- Esto debe configurarse en Razor

    console.log("Chat seleccionado:", chatId);

    $.get(`/Chats/Mensajes?idChat=${chatId}`, function (data) {
        $('#popupChat').html(`
            <div class="chat-conversacion" data-chat-id="${chatId}" data-id-usuario="${idUsuarioLogueado}">
                <div class="chat-conversacion-header">
                    <span class="btn-volver" onclick="volverALista()">←</span>
                     <div class="chat-header-info">
                        <strong>${nombre}</strong><br>
                        <small>${materia}</small>
                    </div>
                </div>
                ${data}
                <div class="chat-conversacion-input">
                    <input type="text" id="mensajeInput" placeholder="Escribe un mensaje..." />
                    <button id="btnEnviar">➤</button>
                </div>
            </div>
        `);

        $('#contenedorMensajes').scrollTop($('#contenedorMensajes')[0].scrollHeight);
    });
});

// ✅ Volver a la lista de chats
function volverALista() {
    $.get('/Chats/Lista', function (data) {
        $('#popupChat').html(data);
    });
}

// ✅ Enviar mensaje con Enter
$(document).on('keydown', '#mensajeInput', function (e) {
    if (e.key === "Enter") {
        $('#btnEnviar').click();
    }
});

// ✅ Botón de enviar (LIMPIO Y CORRECTO)
$(document).on('click', '#btnEnviar', function () {
    const mensaje = $('#mensajeInput').val().trim();
    if (mensaje === "") return;

    const chatId = $('.chat-conversacion').data('chat-id');
    const idEmisor = $('.chat-conversacion').data('id-usuario');

    console.log("ENVIANDO MENSAJE A CONTROLADOR MVC:", { chatId, idEmisor, mensaje });

    $.ajax({
        url: '/Chats/EnviarMensaje', // ✅ Llama al controlador MVC
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            IdChat: chatId,
            IdEmisor: idEmisor,
            Contenido: mensaje
        }),
        success: function () {
            const nuevoMensaje = `<div class="mensaje mensaje-emisor">${mensaje}</div>`;
            $('#contenedorMensajes').prepend(nuevoMensaje);

            $('#mensajeInput').val('');
            $('#contenedorMensajes').scrollTop($('#contenedorMensajes')[0].scrollHeight);
        },
        error: function (xhr) {
            alert("Error al enviar el mensaje: " + xhr.responseText);
        }
    });
});

