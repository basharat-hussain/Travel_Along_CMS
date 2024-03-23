$('#changepasswordform').submit(function (event) {

    var formData = new FormData(this);
    if (validateChangePassword()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/User/ChangePassword',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                changePasswordSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                changePasswordFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function changePasswordSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errPassLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () {
            $('#modal-change-password').modal('hide');
        }, 2000);
    }
    else {
        setError($('#errPassLbl'), response[1]);
    }
}

function changePasswordFailure() {
    setError($('#errPassLbl'), "Oops! Unknown error occured!");
}

function validateChangePassword() {
    clearError($('#errPassLbl'));
    var currentPassword = $("#CurrentPassword");
    var newPassword = $("#NewPassword");
    var repeatNewPassword = $("#RepeatPassword");


    if (currentPassword.val().trim() == "") {
        setError($('#errPassLbl'), "Please enter current password");
        currentPassword.focus();
        return false;
    }
    else if (newPassword.val().trim() == "") {
        setError($('#errPassLbl'), "Please enter new password");
        newPassword.focus();
        return false;
    }
    else if (repeatNewPassword.val().trim() == "") {
        setError($('#errPassLbl'), "Please enter repeat new password");
        repeatNewPassword.focus();
        return false;
    }
    else if (newPassword.val().trim() != repeatNewPassword.val().trim()) {
        setError($('#errPassLbl'), "New and repeat password should match");
        newPassword.focus();
        return false;
    }
    else if (currentPassword.val().trim() == newPassword.val().trim()) {
        setError($('#errPassLbl'), "OOps! New and current password same");
        newPassword.focus();
        return false;
    }
    else {
        return true;
    }

}

//function checkOnlineStatus() {
//    $.ajax({
//        type: "POST",
//        url: '/User/IsConnectedToInternet',
//        dataType: "json",
//        contentType: false,
//        processData: false,
//        success: function (data) {
//            if (data == true) {
//                $("#onlinestatus").html("<i class='online'></i><span class='connected'>Connected</span>");

//            }
//            else {
//                $("#onlinestatus").html("<i class='offline'></i><span class='disconnected'>Disconnected</span>");
//            }
//        },
//        error: function () {
//            //alert('error');
//        }
//    });
//}
//checkOnlineStatus();
//setInterval(function () {
//    checkOnlineStatus();
//}, 5000);


