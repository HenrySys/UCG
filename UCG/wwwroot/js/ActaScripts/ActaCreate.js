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
    $(document).ready(function () {

        // Limpiar mensajes de error
        function limpiarErrores() {
            $('#errorMensaje').remove(); // si tenés un div de errores
            $('#modalIdAsociado').removeClass('is-invalid');
        }

        // Limpiar al abrir el modal
        $('#detailModal').on('show.bs.modal', function () {
            limpiarErrores();
        });

        // Agregar asistencia
        $('#addDetailBtn').on('click', function () {
            limpiarErrores(); // Siempre limpiamos errores antes de validar

            const idAsociado = $('#modalIdAsociado').val();
            const fecha = new Date().toISOString();

            if (!idAsociado || idAsociado === "0") {
                $('#modalIdAsociado').addClass('is-invalid');
                if (!$('#errorMensaje').length) {
                    $('#modalIdAsociado').after('<div id="errorMensaje" class="text-danger mt-1">Debe seleccionar un asociado.</div>');
                }
                return; // Detener si hay error
            }

            $('#detailsTable tbody').append(`
                <tr>
                    <td>${fecha}</td>
                    <td>${idAsociado}</td>
                    <td><button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button></td>
                </tr>
            `);

            // Limpiar campo y cerrar modal
            $('#modalIdAsociado').val('0');
            // 💥 Forzar desenfocar antes de cerrar
            document.activeElement.blur();
            $('#detailModal').modal('hide');
        });


        // Eliminar fila
        $('#detailsTable').on('click', '.removeRow', function () {
            $(this).closest('tr').remove();
        });

        // Convertir tabla a JSON antes de enviar
        $('form').on('submit', function (e) {
            limpiarErrores();

            const asistencias = [];

            $('#detailsTable tbody tr').each(function () {
                const fecha = $(this).find('td:eq(0)').text();
                const idAsociado = $(this).find('td:eq(1)').text();

                asistencias.push({
                    Fecha: fecha,
                    IdAsociado: parseInt(idAsociado)
                });
            });

          
            $('#ActaAsistenciaJason').val(JSON.stringify(asistencias));
        });

    });





});