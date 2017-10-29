//The function which updates the table body through ajax using the wanted name
//and showing the matches
function SearchingName(wantedName) {
    $.ajax({
        url: '/User/CheckUsernameAvailability',
        data: {
            userName: wantedName
        },
        dataType: 'html',
        success: function (data) {
            if (data == 1) {
                $('#txtNameExist').html('Имя <strong>' + wantedName + '</strong> уже занято')
                    .addClass('text-danger').removeClass('text-success');
            }
            else
            {
                $('#txtNameExist').html('Имя <strong>' + wantedName + '</strong> свободно')
                    .addClass('text-success').removeClass('text-danger');
            }
        },
        error: function (err) {
            $('#txtNameExist').html('Введен недопустимый запрос')
            .addClass('text-danger').removeClass('text-success');
        }
    });
}


function SearchingEmail(inputEmail) {
    $.ajax({
        url: '/User/CheckUsernameAvailability',
        data: {
            email: inputEmail
        },
        dataType: 'html',
        success: function (data) {
            if (data == 1) {
                $('#txtEmailExist').html('Email <strong>' + inputEmail + '</strong> уже занят')
                    .addClass('text-danger').removeClass('text-success');
            }
            else {
                $('#txtEmailExist').html('Email <strong>' + inputEmail + '</strong> свободен')
                    .addClass('text-success').removeClass('text-danger');
            }
        },
        error: function (err) {
            $('#txtEmailExist').html('Введен недопустимый запрос')
            .addClass('text-danger').removeClass('text-success');
        }
    });
}

$(document).ready(function () {


    $('#txtUserName').keyup(function () {
        var pattern = $(this).val();
        if (pattern.match(/[^a-zA-ZА-ЯЁа-яё0-9_]/g) != null) {
            $('#txtNameExist').addClass('text-danger').removeClass('text-success');
            $('#txtNameExist').html('Введены недопустимые символы');
        }
        else
        {
            if (pattern.length > 0)
                SearchingName(pattern);
            else
                $('#txtNameExist').html('');
        }
        
    });

    $('#txtEmail').keyup(function () {
        var pattern = $(this).val();
        if (pattern.match(/^\w+([-+.'][^\s]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/) == null) {
            $('#txtEmailExist').addClass('text-danger').removeClass('text-success');
            $('#txtEmailExist').html('Формат электронной почты введен неверно');
        }
        else
        {
            if (pattern.length > 0)
                SearchingEmail(pattern);
            else
                $('#txtEmailExist').html('');
        }
    });
});
