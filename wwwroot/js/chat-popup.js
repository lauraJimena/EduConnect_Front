$(function () {
    $('.boton-chat').on('click', function () {
        console.log("clic");
        var $popup = $('#popupChat');
        if ($popup.is(':visible')) {
            $popup.hide();
        } else {
            $.get('/Chats/Lista', function (data) {
                $popup.html(data).show();
            });
        }
    });
});