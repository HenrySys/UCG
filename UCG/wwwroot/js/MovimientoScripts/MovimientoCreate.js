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

    // Al cambiar la asociación
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

            // Obtener Cuentas
            fetchDropdownData(rutasMovimiento.obtenerCuentas,{ idAsociacion: idAsociacion }, '#IdCuenta', 'Seleccione una cuenta', function (data, dropdown) {
                $.each(data, function (index, item) {
                    dropdown.append('<option value="' + item.idCuenta + '">' + item.numeroCuenta + '</option>');
                });
            });

            // Limpiar el concepto y categoría
            $('#IdConceptoMovimiento').empty().append('<option value="">Seleccione un concepto</option>');
            $('#IdCategoria').empty().append('<option value="">Seleccione una categoria</option>');
        } else {
            $('#IdCuenta').empty().append('<option value="">Seleccione una cuenta</option>');
        }
    });

    // Al cambiar el tipo de movimiento
    $('#TipoMovimiento').change(function () {
        var tipoMovimiento = $(this).val();
        var idAsociacion = $('#IdAsociacion').val();
        if (tipoMovimiento && idAsociacion) {
            fetchDropdownData(rutasMovimiento.obtenerConceptos, { tipoMovimiento: tipoMovimiento, idAsociacion: idAsociacion }, '#IdConceptoMovimiento', 'Seleccione un concepto', function (data, dropdown) {
                $.each(data, function (index, item) {
                    dropdown.append('<option value="' + item.idConceptoMovimiento + '">' + item.concepto + '</option>');
                });
            });
        } else {
            $('#IdConceptoMovimiento').empty().append('<option value="">Seleccione un concepto</option>');
        }
    });

    // Al cambiar el concepto de movimiento
    $('#IdConceptoMovimiento').change(function () {
        var idConcepto = $(this).val();
        if (idConcepto) {
            fetchDropdownData(rutasMovimiento.obtenerCategoria, { idConcepto: idConcepto }, '#IdCategoria', 'Seleccione una categoria', function (data, dropdown) {
                $.each(data, function (index, item) {
                    dropdown.append('<option value="' + item.idCategoria + '">' + item.nombreCategoria + '</option>');
                });
            });
        } else {
            $('#IdCategoria').empty().append('<option value="">Seleccione una categoria</option>');
        }
    });
});