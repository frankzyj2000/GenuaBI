$(function () {
    "use strict";
    var itemKey = 'GenuinaBIUserId';
    var saveUserIdToLocal = function () {
        if ($('#inputRememberMe').is(':checked')) {
            localStorage.setItem(itemKey, $('#inputUsername').val());
        }
        else {
            localStorage.removeItem(itemKey);
        };
    };

    var getUserIdFromLocal = function () {
        var userId = localStorage.getItem(itemKey);
        if (userId != null) {
            $('#inputUsername').val(userId);
            $('#inputRememberMe').attr('checked', true);
        }
        else
        {
            $('#inputUsername').val('');
        }
    };

    getUserIdFromLocal(); // must be first before iCheck change

    $('input').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
    $('#btnSignIn').click(saveUserIdToLocal);
});