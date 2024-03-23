$('#activitylist').on('click', '.btnimage', function () {
    if ($(this).attr('data-image') == "") {
        alert("Sorry no image found");
    }
    else {
        $('#modal-image .modal-body img').attr('src', $(this).attr('data-image'));
        $('#modal-image .modal-footer p').html($(this).attr('data-title'));
        $('#modal-image').modal('show');
    }
});

$('#activitylist').on('click', '.btnviewactvty', function () {
    $('#modal-view-activity .location').html($(this).attr('data-location') == "" ? "NA" : $(this).attr('data-location'));
    $('#modal-view-activity .actvty-title').html($(this).attr('data-title') == "" ? "NA" : $(this).attr('data-title'));
    $('#modal-view-activity .actvty-description').html($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));

    $('#modal-view-activity').modal('show');
});

$('#activitylist').on('click', '.btnviewactvtyed', function () {
    clearError($('#errLbl'));
    $('#modal-edit-activity #Actvty_LocationID').val($(this).attr('data-locationid'));
    $('#modal-edit-activity #Actvty_Title').val($(this).attr('data-title') == "" ? "NA" : $(this).attr('data-title'));
    $('#modal-edit-activity #Actvty_Description').val($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));
    $('#modal-edit-activity #Actvty_ID').val($(this).attr('data-id') == "" ? "NA" : $(this).attr('data-id'));
    $('#modal-edit-activity #Actvty_ImageLink, #modal-edit-activity #uploader ').val($(this).attr('data-image') == "" ? "no file selected" : $(this).attr('data-image'));
    $('#modal-edit-activity').modal('show');
});


$('#activityeditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditActivity()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Activity/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                activityEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                activityEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditActivity() {
    clearError($('#errLbl'));
    var activityTitle = $('#Actvty_Title');
    var activityImage = $('#Actvty_ActivityImage');
    var activityDescription = $("#Actvty_Description");

    
    if (activityTitle.val().trim() == "") {
        setError($('#errLbl'), "Please enter any title for activity");
        activityTitle.focus();
        return false;
    }
    else if (activityDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter activity description");
        activityDescription.focus();
        return false;
    }
    else if (activityImage.val() != "") {
        return fileValidation(activityImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}

//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#Actvty_ActivityImage').change(function () {
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
        else if (imgWidth != 350 || imgHeight != 250) {
            setError(errorControl, "Please select a file with width 350px and height 250px");
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
function activityEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-activity').modal('hide') }, 2000);
        $('#btnupdateactvtylist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function activityEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
