﻿$(document).ready(function () {
    //const idAsociacionTemp = $('#tempDataSwal').data('asociacion');
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const modoVista = $('form').data('mode');
    const miembrosCargados = JSON.parse($('#MiembrosJuntaJson').val() || '[]');

    console.log('Miembors:', miembrosCargados);

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

    let colaErroresSwal = [];
    let swalMostrandose = false;

    function mostrarErrorSwal(titulo, mensaje) {
        colaErroresSwal.push({ titulo, mensaje });
        procesarColaSwal();
    }

    function procesarColaSwal() {
        if (swalMostrandose || colaErroresSwal.length === 0) return;

        swalMostrandose = true;
        const { titulo, mensaje } = colaErroresSwal.shift();

        Swal.fire({
            icon: 'warning',
            title: titulo,
            text: mensaje
        }).then(() => {
            swalMostrandose = false;
            procesarColaSwal(); // Mostrar el siguiente si existe
        });
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

    function prepararModalMiembro() {
        $('#btnAbrirModalMiembro').off('click').on('click', function (e) {
            let IdAsociacion = $('#IdAsociacion').val();
            if (!IdAsociacion || parseInt(IdAsociacion) === 0) {
                e.preventDefault();
                Swal.fire({
                    icon: 'info',
                    title: 'Debe seleccionar una asociación',
                    text: 'Por favor, seleccione una asociación antes de agregar un miembro.',
                    timer: 3000,
                    showConfirmButton: false
                });
                return;
            }

            limpiarErrores();

            const asociadosYaAgregados = $('#detailsTableMiembros tbody tr')
                .map(function () {
                    return $(this).data('id-asociado')?.toString();
                }).get();

            const puestosYaAgregados = $('#detailsTableMiembros tbody tr')
                .map(function () {
                    return $(this).data('id-puesto')?.toString();
                }).get();

            fetchDropdownData(rutasJuntaDirectiva.obtenerAsociados, { idAsociacion: IdAsociacion }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                const asociadosDisponibles = data.filter(item => !asociadosYaAgregados.includes(item.idAsociado.toString()));

                if (asociadosDisponibles.length === 0) {
                    e.preventDefault(); //  evitar que se abra el modal
                    cerrarModalConMensaje('#detailModalMiembrosJunta', {
                        titulo: 'Sin asociados disponibles',
                        texto: 'Todos los asociados ya han sido asignados a la junta.'
                    });
                    return;
                }

                // Poblamos el dropdown solo con los que no están en la tabla
                asociadosDisponibles.forEach(item => {
                    const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                    dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                });

                // Filtro dinámico de puestos
                const puestosDisponibles = $('#modalIdPuesto option').filter(function () {
                    const val = $(this).val();
                    return val === '0' || !puestosYaAgregados.includes(val);
                });

                if (puestosDisponibles.length <= 1) { // solo opción por defecto
                    e.preventDefault(); //  evitar que se abra el modal
                    cerrarModalConMensaje('#detailModalMiembrosJunta', {
                        titulo: 'Sin puestos disponibles',
                        texto: 'Todos los puestos ya han sido asignados.'
                    });
                    return;
                }

                // Mostrar solo los puestos que aún no han sido asignados
                $('#modalIdPuesto option').each(function () {
                    const val = $(this).val();
                    $(this).toggle(!puestosYaAgregados.includes(val) || val === '0');
                });

                $('#modalIdPuesto').val('0');

                // Si todo bien, abrir modal
                $('#detailModalMiembrosJunta').modal('show');
            });

        });
    }

    function recolectarMiembros() {
        const miembros = [];
        $('#detailsTableMiembros tbody tr').each(function () {
            const fila = $(this);
            miembros.push({
                IdAsociado: parseInt(fila.attr('data-id-asociado')),
                IdPuesto: parseInt(fila.attr('data-id-puesto')),
                Nombre: fila.attr('data-nombre') || '',
                Puesto: fila.attr('data-puesto') || ''
            });
        });
        return miembros;
    }

    function reconstruirMiembrosUI() {
        $('#detailsTableMiembros tbody').empty();
        miembrosCargados.forEach((m) => {
            $('#detailsTableMiembros tbody').append(`
            <tr 
                data-id-asociado="${m.IdAsociado}" 
                data-id-puesto="${m.IdPuesto}" 
                data-nombre="${m.Nombre}" 
                data-puesto="${m.Puesto}">
                <td><span class="badge bg-primary">${m.Puesto}</span></td>
                <td>${m.Nombre}</td>
                <td>
                    <button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button>
                </td>
            </tr>`);
        });
    }

    if (modoVista === 'Create') {
        prepararModalMiembro(); 
    }

    $('#IdAsociacion').change(function () {
        const nuevaAsociacion = $(this).val();
        if (!nuevaAsociacion) return;

        $('#detailsTableMiembros tbody').empty();
        prepararModalMiembro();
    });

    reconstruirMiembrosUI();

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

        const nombreAsociado = $('#modalIdAsociado option:selected').text();
        const nombrePuesto = $('#modalIdPuesto option:selected').text();

        $('#detailsTableMiembros tbody').append(`
        <tr 
            data-id-asociado="${idAsociado}" 
            data-id-puesto="${idPuesto}" 
            data-nombre="${nombreAsociado}" 
            data-puesto="${nombrePuesto}">
            <td><span class="badge bg-primary">${nombrePuesto}</span></td>
            <td>${nombreAsociado}</td>
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
        const fila = $(this).closest('tr');
        Swal.fire({
            title: '¿Está seguro?',
            text: '¿Desea eliminar este miembro?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fila.remove();
                Swal.fire('Eliminado', 'El miembro ha sido eliminado.', 'success');
            }
        });
    });    

    $('form').on('submit', function (e) {
        limpiarErrores();

        const miembros = recolectarMiembros();
        const nombreJunta = $('#Nombre').val().trim();
        let error = false;

        if (miembros.length < 1) {
            $('#detailsTableMiembros').after('<div class="text-danger mt-1">Debe agregar al menos 1 miembros.</div>');
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


    if (modoVista === 'Edit') {
        $('#btnAbrirModalMiembro').off('click').on('click', function (e) {
            e.preventDefault();

            const idJunta = $('#IdJuntaDirectiva').val(); // Asegurate de tener este input hidden en la vista Edit

            if (!idJunta) {
                Swal.fire("Error", "No se pudo obtener el ID de la junta.", "error");
                return;
            }

            const miembros = recolectarMiembros();
            $('#MiembrosJuntaJson').val(JSON.stringify(miembros));

            Swal.fire({
                icon: 'info',
                title: 'Guarde los cambios',
                text: 'Debe guardar la junta antes de agregar miembros.',
                showCancelButton: true,
                confirmButtonText: 'Guardar y continuar',
                cancelButtonText: 'Cancelar'
            }).then(result => {
                if (result.isConfirmed) {
                    $('<input>').attr({
                        type: 'hidden',
                        name: 'RedirigirMiembro',
                        value: 'true'
                    }).appendTo('form');

                    $('form').submit();
                }
            });
        });
    }

    // 🔹 Confirmación antes de eliminar
    const deleteBtn = document.getElementById('btnConfirmDelete');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            Swal.fire({
                title: '¿Está seguro?',
                text: 'Esta acción eliminará permanentemente La Junta Directiva.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.closest('form').submit();
                }
            });
        });
    }

});
