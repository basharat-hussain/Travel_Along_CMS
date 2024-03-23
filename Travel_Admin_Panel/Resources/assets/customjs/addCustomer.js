$('#customeraddform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddCustomer()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Customer/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                CustomerAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                CustomerAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function CustomerAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function CustomerAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddCustomer() {
    clearError($('#errLbl'));
    var customerName = $('#Name');
    var customerEmail = $('#Email');
    var customerAddress = $("#Address");
    var customerPhone = $("#Phone");

    if (customerName.val().trim() == "") {
        setError($('#errLbl'), "Please enter name for customer");
        customerName.focus();
        return false;
    }
    else if (customerEmail.val() == "") {
        setError($('#errLbl'), "Please Enter email id");
        customerEmail.focus();
        return false;
    }

    else if (customerAddress.val().trim() == "") {
        setError($('#errLbl'), "Please enter Address of customer");
        customerAddress.focus();
        return false;
    }
    else if (customerPhone.val().trim() == "") {
        setError($('#errLbl'), "Please enter phone number");
        customerPhone.focus();
        return false;
    }

    else {
        return true;
    }

}


