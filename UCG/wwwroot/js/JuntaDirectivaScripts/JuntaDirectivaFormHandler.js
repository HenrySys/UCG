$(document).ready(function () {

    const idAsociacion = $('#tempDataSwal').data('asociacion');
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const modoVista = $('form').data('mode');

    const asociadosMap = new Map();
    const puestosMap = new Map();

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbJuntaDirectivas/Index';
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

    const miembrosCargados = JSON.parse($('#MiembrosJuntaJson').val() || '[]');
    if (miembrosCargados.length > 0) {
        $.ajax({
            url: '/TbJuntaDirectivas/ObtenerNombresMiembros',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(miembrosCargados),
            success: function (response) {
                response.forEach(x => {
                    if (x.idAsociado && x.nombreAsociado) asociadosMap.set(x.idAsociado.toString(), x.nombreAsociado);
                    if (x.idPuesto && x.nombrePuesto) puestosMap.set(x.idPuesto.toString(), x.nombrePuesto);
                });
                reconstruirMiembrosUI(miembrosCargados);
            },
            error: function () {
                reconstruirMiembrosUI(miembrosCargados);
            }
        });
    } else {
        reconstruirMiembrosUI(miembrosCargados);
    }

    function fetchDropdownData(url, data, dropdownSelector, placeholder, callback) {
        $.ajax({
            url,
            data,
            success: function (response) {
                const dropdown = $(dropdownSelector).empty();
                dropdown.append(`<option value="0" disabled selected>${placeholder}</option>`);
                if (response.success) callback(response.data, dropdown);
                else mostrarErrorSwal('Atención', response.message);
            },
            error: function (_, __, error) {
                mostrarErrorSwal('Error', 'Error al cargar los datos: ' + error);
            }
        });
    }

    function mostrarErrorSwal(titulo, mensaje) {
        Swal.fire({ icon: 'warning', title: titulo, text: mensaje });
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

    function limpiarErrores() {
        $('.text-danger.mt-1').remove();
        $('.is-invalid').removeClass('is-invalid');
    }

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

    function prepararModalMiembro(asociacionId) {
        limpiarErrores();
        const asociadosYaAgregados = $('#detailsTableMiembros tbody tr').map(function () {
            return $(this).data('id-asociado').toString();
        }).get();
        const puestosYaAgregados = $('#detailsTableMiembros tbody tr').map(function () {
            return $(this).data('id-puesto').toString();
        }).get();

        fetchDropdownData(rutasJuntaDirectiva.obtenerAsociados, { idAsociacion: asociacionId }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
            if (!data || data.length === 0) {
                cerrarModalConMensaje('#detailModalMiembrosJunta', {
                    titulo: 'Sin asociados',
                    texto: 'No hay asociados disponibles para esta asociación.'
                });
                return;
            }

            let agregados = 0;
            data.forEach(item => {
                const id = item.idAsociado.toString();
                const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                asociadosMap.set(id, nombreCompleto);
                if (!asociadosYaAgregados.includes(id)) {
                    dropdown.append(`<option value="${id}">${nombreCompleto}</option>`);
                } else {
                    agregados++;
                }
            });

            if (agregados === data.length) {
                cerrarModalConMensaje('#detailModalMiembrosJunta', {
                    titulo: 'Todos agregados',
                    texto: 'Todos los asociados ya han sido agregados a la lista.'
                });
            }

            $('#modalIdPuesto option').each(function () {
                const val = $(this).val();
                const text = $(this).text();
                if (val !== '0') puestosMap.set(val, text);
                $(this).toggle(!puestosYaAgregados.includes(val));
            });
            $('#modalIdPuesto').val('0');
        });
    }

    function reconstruirMiembrosUI(miembros) {
        $('#detailsTableMiembros tbody').empty();
        miembros.forEach(m => {
            const nombreAsociado = asociadosMap.get(m.IdAsociado?.toString()) || `(Asociado ${m.IdAsociado})`;
            const nombrePuesto = puestosMap.get(m.IdPuesto?.toString()) || `(Puesto ${m.IdPuesto})`;
            const botones = (modoVista === 'Create') ? `<td><button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button></td>` : '';
            $('#detailsTableMiembros tbody').append(`
                <tr data-id-asociado="${m.IdAsociado}" data-id-puesto="${m.IdPuesto}">
                    <td>${nombreAsociado}</td>
                    <td>${nombrePuesto}</td>
                    ${botones}
                </tr>`);
        });
    }

    function manejarCargaModal(asociacionId) {
        $('#detailModalMiembrosJunta').off('show.bs.modal').on('show.bs.modal', function () {
            prepararModalMiembro(asociacionId);
        });
    }

    if (idAsociacion && parseInt(idAsociacion) > 0) {
        manejarCargaModal(idAsociacion);
    } else {
        $('#IdAsociacion').change(function () {
            const nuevaAsociacion = $(this).val();
            $('#detailsTableMiembros tbody').empty();

            if (!nuevaAsociacion) return;

            // Validación inmediata al cambiar la asociación
            fetchDropdownData(rutasJuntaDirectiva.obtenerAsociados, { idAsociacion: nuevaAsociacion }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                if (!data || data.length === 0) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Sin asociados',
                        text: 'No hay asociados disponibles para esta asociación.'
                    });
                }
            });

            manejarCargaModal(nuevaAsociacion); // esto prepara el modal para uso posterior
        });

    }

    $('#addMiembroBtn').on('click', function () {
        limpiarErroresMiembro();

        const idAsociado = $('#modalIdAsociado').val();
        const idPuesto = $('#modalIdPuesto').val();

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

        const nombreAsociado = asociadosMap.get(idAsociado) || `(Asociado ${idAsociado})`;
        const nombrePuesto = puestosMap.get(idPuesto) || `(Puesto ${idPuesto})`;

        $('#detailsTableMiembros tbody').append(`
            <tr data-id-asociado="${idAsociado}" data-id-puesto="${idPuesto}">
                <td>${nombreAsociado}</td>
                <td>${nombrePuesto}</td>
                <td>
                    <button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button>
                </td>
            </tr>`);

        limpiarCamposModalMiembro();
        $('#detailModalMiembrosJunta').modal('hide');
    });

    $('#detailModalMiembrosJunta').on('hidden.bs.modal', function () {
        limpiarCamposModalMiembro();
        limpiarErroresMiembro();
    });

    $('#detailsTableMiembros').on('click', '.removeRow', function () {
        $(this).closest('tr').remove();
    });

    function recolectarMiembrosJunta() {
        const miembros = [];
        $('#detailsTableMiembros tbody tr').each(function () {
            const idAsociado = $(this).data('id-asociado');
            const idPuesto = $(this).data('id-puesto');
            miembros.push({ IdAsociado: idAsociado, IdPuesto: idPuesto });
        });
        return miembros;
    }

    $('form').on('submit', function (e) {
        limpiarErrores();

        const miembros = recolectarMiembrosJunta();
        const nombreJunta = $('#Nombre').val().trim();

        let error = false;

        if (miembros.length === 0) {
            $('#detailsTableMiembros').after('<div class="text-danger mt-1">Debe agregar al menos 6 miembros.</div>');
            error = true;
        }

        if (!/^[A-Z][a-zA-Z\s]{4,}$/.test(nombreJunta)) {
            $('#Nombre').addClass('is-invalid')
                .after('<div class="text-danger mt-1">El nombre debe iniciar con mayúscula y tener al menos 5 caracteres.</div>');
            error = true;
        }

        if (error) {
            e.preventDefault();
        } else {
            $('#MiembrosJuntaJson').val(JSON.stringify(miembros));
        }
    });
});
