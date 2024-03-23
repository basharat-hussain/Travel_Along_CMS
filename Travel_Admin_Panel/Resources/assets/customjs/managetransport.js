$('#transportlist').on('click', '.btnimage', function () {
    if ($(this).attr('data-image') == "") {
        alert("Sorry no image found");
    }
    else {
        $('#modal-image .modal-body img').attr('src', $(this).attr('data-image'));
        $('#modal-image .modal-footer p').html($(this).attr('data-title'));
        $('#modal-image').modal('show');
    }
});



$('#transportlist').on('click', '.btnviewtported', function () {
    clearError($('#errLbl'));
    $('#modal-edit-transport #Tport_ID').val($(this).attr('data-id') == "" ? "NA" : $(this).attr('data-id'));
    $('#modal-edit-transport #Tport_Name').val($(this).attr('data-title') == "" ? "NA" : $(this).attr('data-title'));
    $('#modal-edit-transport #Tport_Features').val($(this).attr('data-features') == "" ? "NA" : $(this).attr('data-features'));
    $('#modal-edit-transport #Tport_Rate').val($(this).attr('data-rate') == "" ? "NA" : $(this).attr('data-rate'));
    $('#modal-edit-transport #Tport_ImageLink, #modal-edit-transport #uploader ').val($(this).attr('data-image') == "" ? "no file selected" : $(this).attr('data-image'));
    $('#modal-edit-transport').modal('show');
});


$('#transporteditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditTransport()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Transport/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                transportEditSuccess(data);
                $('#tblLoader').hide();
                $("#btnupdatetportlist").trigger('click');
            },
            error: function () {
                transportEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditTransport() {
    clearError($('#errLbl'));
    var transportName = $('#Tport_Name');
    var transportFeatures = $('#Tport_Features');
    var transportImage = $('#Tport_TransportImage');
    var transportRate = $("#Tport_Rate");

    
    if (transportName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for Transport");
        transportName.focus();
        return false;
    }
    else if (transportFeatures.val().trim() == "") {
        setError($('#errLbl'), "Please enter transport features");
        transportFeatures.focus();
        return false;
    }
    else if (transportRate.val().trim() == "") {
        setError($('#errLbl'), "Please enter transport rate");
        transportRate.focus();
        return false;
    }
    else if (transportImage.val() != "") {
        return fileValidation(transportImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}

//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#TransportImage').change(function () {
    var file = $(this)[0].files[0];

    img = new Image();
    img.src = _URL.createObjectURL(file);
    img.onload = function () {
        imgWidth = this.width;
        imgHeight = this.height;
    }

});

function fileValidation(control, extension, size, errorControl) {

    if (control.val() != "" && size != "") {
        var fileExtension = control.val().split('.').pop();
        var fileSize = (control[0].files[0].size / 1024);

        if (fileExtension.toUpperCase() != extension.toUpperCase()) {
            setError(errorControl, "Invalid file selected (" + fileExtension + "), only " + extension + " files allowed");
            return false;
        }
        else if (imgWidth != 300 || imgHeight != 190) {
            setError(errorControl, "Please select a file with width 300px and height 190px");
            return false;
        }
        else if (fileSize > size) {
            setError(errorControl, "Please select a file less than " + size + " KB");
            return false;
        }
        else {
            return true;
        }
    }
    else {
        setError(errorControl, "Please select a file");
        return false;
    }
}
function transportEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-transport').modal('hide') }, 2000);
        $('#btnupdatedestlist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function transportEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
