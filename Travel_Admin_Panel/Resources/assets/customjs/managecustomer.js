
$('#customerlist').on('click', '.btnviewcusted', function () {
    clearError($('#errLbl'));
    $('#modal-edit-customer #Cust_ID').val($(this).attr('data-id'));
    $('#modal-edit-customer #Cust_Name').val($(this).attr('data-name') == "" ? "NA" : $(this).attr('data-name'));
    $('#modal-edit-customer #Cust_Email').val($(this).attr('data-email') == "" ? "NA" : $(this).attr('data-email'));
    $('#modal-edit-customer #Cust_Address').val($(this).attr('data-address') == "" ? "NA" : $(this).attr('data-address'));
    $('#modal-edit-customer #Cust_Phone').val($(this).attr('data-phone') == "" ? "NA" : $(this).attr('data-phone'));
    $('#modal-edit-customer').modal('show');
});


$('#customereditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditcustomer()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Customer/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                customerEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                customerEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditcustomer() {
    clearError($('#errLbl'));
    var customerName = $('#Cust_Name');
    var customerEmail = $('#Cust_Email');
    var customerAddress = $('#Cust_Address');
    var customerPhone = $("#Cust_Phone");

    if (customerName.val().trim() == "") {
        setError($('#errLbl'), "Please enter Name of Customer");
        customerName.focus();
        return false;
    }
    else if (customerEmail.val().trim() == "") {
         setError($('#errLbl'), "Please enter email id");
         customerEmail.focus();
        return false;
    }

    else if (customerAddress.val().trim() == "") {
        setError($('#errLbl'), "Please enter address of a customer");
        customerAddress.focus();
        return false;
    }
    else if (customerPhone.val().trim() == "") {
        setError($('#errLbl'), "Please enter phone number of customer");
        customerPhone.focus();
        return false;
    }
    else {
        return true;
    }

}

//File parameters
//var _URL = window.URL || window.webkitURL;
//var imgWidth = 0;
//var imgHeight = 0;
//$('#CustomerImage').change(function () {
//    var file = $(this)[0].files[0];

//    img = new Image();
//    img.src = _URL.createObjectURL(file);
//    img.onload = function () {
//        imgWidth = this.width;
//        imgHeight = this.height;
//    }

//});

//function fileValidation(control, extension, size, errorControl) {

//    if (control.val() != "" && size != "") {
//        var fileExtension = control.val().split('.').pop();
//        var fileSize = (control[0].files[0].size / 1024);

//        if (fileExtension.toUpperCase() != extension.toUpperCase()) {
//            setError(errorControl, "Invalid file selected (" + fileExtension + "), only " + extension + " files allowed");
//            return false;
//        }
//        else if (imgWidth != 300 || imgHeight != 260) {
//            setError(errorControl, "Please select a file with width 300px and height 260px");
//            return false;
//        }
//        else if (fileSize > size) {
//            setError(errorControl, "Please select a file less than " + size + " KB");
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//    else {
//        setError(errorControl, "Please select a file");
//        return false;
//    }
//}
function customerEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-customer').modal('hide') }, 2000);
        $('#btnupdatecustlist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function customerEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
