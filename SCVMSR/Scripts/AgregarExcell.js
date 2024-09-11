$(document).ready(function () {
    $('#uploadForm').submit(function (event) {
        event.preventDefault();

        var formData = new FormData($(this)[0]);

        $.ajax({
            url: '@Url.Action("CargarEmpleados", "Empleados")',
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            xhr: function () {
                var xhr = $.ajaxSettings.xhr();
                xhr.upload.onprogress = function (e) {
                    if (e.lengthComputable) {
                        var percent = (e.loaded / e.total) * 100;
                        $('#progressBar').width(percent + '%');
                        $('#progressLabel').text(percent.toFixed(0) + '%');
                    }
                };
                return xhr;
            },
            beforeSend: function () {
                $('#submitButton').prop('disabled', true);
                $('#progress').css('visibility', 'visible');
            },
            success: function (result) {
                toastr.success(result.message);
            },
            error: function (xhr) {
                toastr.error(xhr.responseJSON.message);
            },
            complete: function () {
                $('#submitButton').prop('disabled', false);
                $('#progress').css('visibility', 'hidden');
                $('#progressBar').width('0%');
                $('#progressLabel').text('0%');
            }
        });
    });
});