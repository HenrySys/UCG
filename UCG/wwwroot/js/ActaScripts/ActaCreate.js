$(document).ready(function () {
    // Función para manejar las solicitudes AJAX y poblar los dropdowns
    function fetchDropdownData(url, data, dropdownSelector, placeholder, callback) {
        $.ajax({
            url: url,
            data: data,
            success: function (response) {
                var dropdown = $(dropdownSelector);
                dropdown.empty();
                dropdown.append(`<option value="">${placeholder}</option>`);

                if (response.success) {
                    callback(response.data, dropdown);
                } else {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Atención',
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error al cargar los datos: ' + error
                });
            }
        });
    }

    $('#IdAsociacion').change(function () {
        var idAsociacion = $(this).val();
        if (idAsociacion) {
            // Obtener Asociados
            fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion: idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                $.each(data, function (index, item) {
                    var nombreCompleto = item.nombre + ' ' + item.apellido1;
                    dropdown.append('<option value="' + item.idAsociado + '">' + nombreCompleto + '</option>');
                });
            });
        }

    });

});