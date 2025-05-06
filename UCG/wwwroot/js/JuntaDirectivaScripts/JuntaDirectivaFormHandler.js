$(document).ready(function () {

    const successMessage = $('#TempDataSuccessMessage').val();
    const errorMessage = $('#TempDataErrorMessage').val();

    const idAsociacion = $('#tempDataSwal').data('asociacion');



    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbJuntaDirectiva/Index';
            }
        });
    } else if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar'
        });
    }

    function cerrarModalConMensaje(modalSelector, mensaje, icono = 'info') {
        Swal.fire({
            icon: icono,
            title: mensaje.titulo,
            text: mensaje.texto,
            timer: 3000,
            showConfirmButton: false
        });
        $(modalSelector).modal('hide');
    }

    function mostrarErrorSwal(title, text) {
        Swal.fire({
            icon: 'error',
            title,
            text,
            confirmButtonText: 'Aceptar'
        });
    }

    if (idAsociacion && parseInt(idAsociacion) > 0) {
        fetchDropdownData(rutasJuntaDirectiva.obtenerActas, { idAsociacion }, '#IdActa', 'Seleccione un acta', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idActa}">${item.NumeroActa}</option>`);
            });
        });

        $('#detailModalMiembrosJunta').off('show.bs.modal').on('show.bs.modal', function () {
            limpiarErrores();
            const asociadosYaAgregados = $('#detailsTableMiembros tbody tr').map(function () {
                return $(this).find('td:eq(1)').text();
            }).get();

            fetchDropdownData(rutasJuntaDirectiva.obtenerAsociados, { idAsociacion }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                let agregados = 0;
                data.forEach(item => {
                    if (!asociadosYaAgregados.includes(item.idAsociado.toString())) {
                        const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                        dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                    } else agregados++;
                });
                if (agregados === data.length) {
                    cerrarModalConMensaje('#detailModalMiembrosJunta', {
                        titulo: 'Todos agregados',
                        texto: 'Todos los asociados ya han sido agregados a la lista.'
                    });
                }
            });
        });


    } else {

    }
      

    function fetchDropdownData(url, data, dropdownSelector, placeholder, callback) {
        $.ajax({
            url,
            data,
            success: function (response) {
                const dropdown = $(dropdownSelector).empty();
                dropdown.append(`<option disabled selected>${placeholder}</option>`);
                if (response.success) callback(response.data, dropdown);
                else mostrarErrorSwal('Atención', response.message);
            },
            error: function (_, __, error) {
                mostrarErrorSwal('Error', 'Error al cargar los datos: ' + error);
            }
        });
    }

    function recolectarMiembrosJunta() {
        const miembros = [];
        const filas = $('#detailsTableMiembros tbody tr');

        filas.each(function () {
            const idAsociado = parseInt($(this).data('id-asociado'));
            const idPuesto = parseInt($(this).data('id-puesto'));

            miembros.push({
                IdAsociado: idAsociado,
                IdPuesto: idPuesto
            });
        });

        return miembros;
    }

    // Función para manejar miembros de la Junta Directiva
    let filaMiembroEditando = null;

    $('#addMiembroBtn').on('click', function () {
        limpiarErroresMiembro();

        const idAsociado = $('#modalIdAsociado').val();
        const nombreAsociado = $('#modalIdAsociado option:selected').text();
        const idPuesto = $('#modalIdPuesto').val();
        const nombrePuesto = $('#modalIdPuesto option:selected').text();

        let hayError = false;

        if (!idAsociado || idAsociado === '0') {
            $('#modalIdAsociado').addClass('is-invalid')
                .after('<div id="errorAsociadoMiembro" class="text-danger mt-1">Debe seleccionar un asociado.</div>');
            hayError = true;
        }

        if (!idPuesto || idPuesto === '0') {
            $('#modalIdPuesto').addClass('is-invalid')
                .after('<div id="errorPuestoMiembro" class="text-danger mt-1">Debe seleccionar un puesto.</div>');
            hayError = true;
        }

        if (hayError) return;

        if (filaMiembroEditando !== null) {
            filaMiembroEditando.attr('data-id-asociado', idAsociado);
            filaMiembroEditando.attr('data-id-puesto', idPuesto);
            filaMiembroEditando.find('td:eq(0)').text(nombreAsociado);
            filaMiembroEditando.find('td:eq(1)').text(nombrePuesto);
            filaMiembroEditando = null;
        } else {
            $('#detailsTableMiembros tbody').append(`
                <tr data-id-asociado="${idAsociado}" data-id-puesto="${idPuesto}">
                    <td>${nombreAsociado}</td>
                    <td>${nombrePuesto}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-warning btn-edit-miembro">Editar</button>
                        <button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button>
                    </td>
                </tr>`);
        }

        actualizarMiembrosJson();
        limpiarCamposModalMiembro();
        $('#detailModalMiembrosJunta').modal('hide');
    });

    $('#detailsTableMiembros').on('click', '.btn-edit-miembro', function () {
        limpiarErroresMiembro();
        filaMiembroEditando = $(this).closest('tr');

        const idAsociado = filaMiembroEditando.data('id-asociado');
        const idPuesto = filaMiembroEditando.data('id-puesto');

        $('#modalIdAsociado').val(idAsociado);
        $('#modalIdPuesto').val(idPuesto);

        $('#detailModalMiembrosJunta').modal('show');
    });

    $('#detailsTableMiembros').on('click', '.removeRow', function () {
        $(this).closest('tr').remove();
        actualizarMiembrosJson();
    });

    function limpiarErroresMiembro() {
        $('#modalIdAsociado').removeClass('is-invalid');
        $('#modalIdPuesto').removeClass('is-invalid');
        $('#errorAsociadoMiembro').remove();
        $('#errorPuestoMiembro').remove();
    }

    function limpiarCamposModalMiembro() {
        $('#modalIdAsociado').val('0');
        $('#modalIdPuesto').val('0');
    }

    

});
