
function createFields(count) {
    $("#DayCount").val(count);
    var fieldHTML = "";
    for (var i = 1; i <= count; i++) {
        fieldHTML += "<div class='col-sm-4'>" +
            "<div class='form-group'>" +
            "<label class='control-label'>Day" + i + "</label>" +
            "<div class='append-icon' >" +
            "<input type='text' class='form-control daystitle' data-val='true' id='Pkg_DayTitle" + i + "' name='Pkg_DayTitle" + i + "'" +
            "placeholder='Title for Day" + i + "' />" +
            "<i class='fa fa-edit'></i>" +
            "</div>" +
            "</div>" +
            "<div class='form-group'> " +
            //"<label class='control-label'>Day" + i + "</label>" +
            "<div class='append-icon'>" +
            "<textarea class='form-control daysdata' data-val='true' id='Pkg_Day" + i + "' name='Pkg_Day" + i + "'" +
            "placeholder='Description for Day" + i + "' type='text' value='' rows='3' maxlength='2000'></textarea>" +
            "<i class='fa fa-info'></i>" +
            "</div></div></div>";
    }
    $('#package-fields').html(fieldHTML);
}

$('#Pkg_Duration').change(function () {

    var index = $("#Pkg_Duration option:selected").index();
    switch (index) {
        case 0:
            alert("Please select package duration");
            break;
        case 1:
            createFields(3);
            break;
        case 2:
            createFields(4);
            break;
        case 3:
            createFields(5);
            break;
        case 4:
            createFields(6);
            break;
        case 5:
            createFields(7);
            break;
        case 6:
            createFields(8);
            break;
        case 7:
            createFields(9);
            break;
        case 8:
            createFields(10);
            break;
        case 9:
            createFields(11);
            break;
        case 10:
            createFields(12);
            break;
        case 11:
            createFields(13);
            break;
        case 12:
            createFields(14);
            break;

    }
});

$('#packagecreateform').submit(function (event) {
    var formData = new FormData(this);
    if (validateAddPackage()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Package/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                packageAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                packageAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function packageAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function packageAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddPackage() {
    clearError($('#errLbl'));
    var packageLocation = $("#Pkg_LocationID");
    var packageTitle = $('#Pkg_Title');
    var packageDuration = $("#Pkg_Duration");
    var packageImage = $('#Pkg_PackageImage');
    var packageRate = $("#Pkg_Rate");
    var packageTitles = $('.daystitle');
    var packageDescriptions = $('.daysdata');
    //var packageDescription = $("Pkg_Description");
    var packageInclusions = $('#Pkg_Inclusions');
    var packageExclusions = $('#Pkg_Exclusions');

    if ($('#Pkg_LocationID option:selected').index() == 0) {
        setError($('#errLbl'), "Please select package location");
        packageLocation.focus();
        return false;
    }
    else if (packageTitle.val().trim() == "") {
        setError($('#errLbl'), "Please enter any title for package");
        packageTitle.focus();
        return false;
    }
    else if ($('#Pkg_Duration option:selected').index() == 0) {
        setError($('#errLbl'), "Please select package duration");
        packageDuration.focus();
        return false;
    }
    else if (packageImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        packageImage.focus();
        return false;
    }
    else if (packageRate.val().trim() == "") {
        setError($('#errLbl'), "Please enter rate for package");
        packageRate.focus();
        return false;
    }
    else if (!validatePackageTitles(packageTitles)) {
        return false;
    }
    else if (!validatePackageDescriptions(packageDescriptions)) {
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
        return fileValidation(packageImage, "jpg", "200", $('#errLbl'));
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

function validatePackageTitles(packageTitles) {
    var isValidated = true;
    packageTitles.each(function () {
        if ($(this).val().trim() == "") {
            setError($('#errLbl'), "Please fill day title");
            isValidated = false;
            $(this).focus();
            return isValidated;
        }
    });
    return isValidated;
}

function validatePackageDescriptions(packageDescriptions) {
    var isValidated = true;
    packageDescriptions.each(function () {
        if ($(this).val().trim() == "") {
            setError($('#errLbl'), "Please fill day descriptions");
            isValidated = false;
            $(this).focus();
            return isValidated;
        }
    });
    return isValidated;
}