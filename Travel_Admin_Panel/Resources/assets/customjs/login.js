/// <reference path="bh_general.js" />
function validateLogin() {
    clearError($('#errLbl'));
    var userName = $('#UserName');
    var password = $("#Password");

    if (userName.val().trim() == "") {
        setError($('#errLbl'), "Please enter username");
        userName.focus();
        return false;
    }
    else if (password.val().trim() == "") {
        setError($('#errLbl'), "Please enter password");
        password.focus();
        return false;
    }
    else {
        $("#btnSignIn").html("<i class='fa fa-spinner fa-spin'></i> Please wait...").attr('disabled', 'disabled');
        return true;
    }
}

function successLogin(response) {

    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        location.href = "/Home/Dashboard"
    }
    else {
        setError($('#errLbl'), response[1]);
        $("#btnSignIn").html("Sign In").removeAttr('disabled');
    }
}

function failureLogin() {
    setError($('#errLbl'), "Unknown error occured!");
    $("#btnSignIn").html("Sign In").removeAttr('disabled');

}