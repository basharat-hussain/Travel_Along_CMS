$('#packagelist').on('click', '.btnimage', function () {
    if ($(this).attr('data-image') == "") {
        alert("Sorry no image found");
    }
    else {
        $('#modal-image .modal-body img').attr('src', $(this).attr('data-image'));
        $('#modal-image .modal-footer p').html($(this).attr('data-title'));
        $('#modal-image').modal('show');
    }
});

$('#packageeditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditPackage()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Package/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                packageEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                packageEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function packageEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-package').modal('hide') }, 2000);
        $("#btnupdatepacklist").trigger('click');
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function packageEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateEditPackage() {
    clearError($('#errLbl'));
    var packageTitle = $('#Pkg_Title');
    var packageDuration = $("#Pkg_Duration");
    var packageImage = $('#Pkg_PackageImage');
    var packageDays = $('.daysdata');
    var packageInclusions = $('#Pkg_Inclusion');
    var packageExclusions = $('#Pkg_Exclusion');
    debugger;
    if (packageTitle.val().trim() == "") {
        setError($('#errLbl'), "Please enter any title for package");
        packageTitle.focus();
        return false;
    }
    else if ($('#Pkg_Duration option:selected').index() == 0) {
        setError($('#errLbl'), "Please select package duration");
        packageDuration.focus();
        return false;
    }
    else if (!validatePackageDays(packageDays)) {
        return false;
    }
    else if (packageInclusions.val().trim() == "") {
        setError($('#errLbl'), "Please enter inclusions for the package");
        packageInclusions.focus();
        return false;
    }
    else if (packageExclusions.val().trim() == "") {
        setError($('#errLbl'), "Please enter exclusions for the package");
        packageExclusions.focus();
        return false;
    }
    else if (packageImage.val() != "") {
        return fileValidation(packageImage, "jpg", "1", $('#errLbl'));
    }
    else {
        return true;
    }

}

//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#Pkg_PackageImage').change(function () {
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
        else if (imgWidth != 850 || imgHeight != 550) {
            setError(errorControl, "Please select a file with width 850px and height 550px");
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


function validatePackageDays(days) {
    var isValidated = true;
    days.each(function () {
        if ($(this).val().trim() == "") {
            setError($('#errLbl'), "Please fill package day data");
            isValidated = false;
            $(this).focus();
            return isValidated;
        }
    });
    return isValidated;
}

function successPackageView() {
    $('#modal-view-package').modal('show');
}

function failurePackageView() {
    alert("Failed to view Package");
}

function failurePackageEditView() {
    alert("Failed to load Package for edit");
}

function successPackageEditView() {
    $('#modal-edit-package').modal('show');
}