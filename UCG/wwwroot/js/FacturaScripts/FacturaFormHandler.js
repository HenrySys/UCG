$(document).ready(function () {
    const modoVista = $('form').data('mode') || 'Create';
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const idAsociacion = $('#tempDataSwal').data('asociacion') || $('#IdAsociacion').val();

    // Agregado: tipoEmisor desde Razor para postback
    const tipoEmisorDesdeModelo = $('#TipoEmisor').val(); // <- actualiza el campo oculto

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbFacturas/Index';
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

    function fetchDropdownData(url, data, selector, placeholder, renderFn) {
        $.ajax({
            url,
            data,
            success: function (response) {
                const dropdown = $(selector).empty();
                dropdown.append(`<option value="" disabled selected>${placeholder}</option>`);
                if (response.success) {
                    renderFn(response.data, dropdown);
                } else {
                    mostrarErrorSwal('Atención', response.message);
                }
            },
            error: function (_, __, error) {
                mostrarErrorSwal('Error', 'Error al cargar los datos: ' + error);
            }
        });
    }

    function fetchDropdownData(url, data, selector, placeholder, renderFn) {
        $.ajax({
            url,
            data,
            success: function (response) {
                const dropdown = $(selector).empty();
                dropdown.append(`<option value="" disabled selected>${placeholder}</option>`);
                if (response.success) {
                    renderFn(response.data, dropdown);

                    const selected = $(selector).data('selected');
                    if (selected) {
                        $(selector).val(selected).trigger('change');
                    }
                } else {
                    mostrarErrorSwal('Atención', response.message);
                }
            },
            error: function (_, __, error) {
                mostrarErrorSwal('Error', 'Error al cargar los datos: ' + error);
            }
        });
    }

    function cargarConceptos(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerConceptos, { idAsociacion }, '#IdConceptoAsociacion', 'Seleccione un concepto', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idConceptoAsociacion}" data-tipoemisor="${item.tipoEmisor.toLowerCase()}">${item.descripcionPersonalizada}</option>`);
            });

            // Seleccionar el valor guardado manualmente después del renderizado
            const selected = $('#IdConceptoAsociacion').data('selected');
            if (selected) {
                $('#IdConceptoAsociacion').val(selected).trigger('change');
            }
        });
    }

    function cargarAsociados(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerAsociados, { idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idAsociado}">${item.nombre} ${item.apellido1}</option>`);
            });
        });
    }

    function cargarProveedor(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerProveedores, { idAsociacion }, '#IdProveedor', 'Seleccione un proveedor', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idProveedor}">${item.nombreEmpresa}</option>`);
            });
        });
    }

    function cargarColaborador(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerColaboradores, { idAsociacion }, '#IdColaborador', 'Seleccione un colaborador', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idColaborador}">${item.nombre}</option>`);
            });
        });
    }

    function ocultarTodosLosCampos() {
        $('#grupoProveedor, #grupoAsociado, #grupoColaborador').hide();
    }

    function mostrarCampoSegunTipo(tipoEmisor, idAsociacion) {
        ocultarTodosLosCampos();
        switch (tipoEmisor) {
            case 'proveedor':
                $('#grupoProveedor').show();
                cargarProveedor(idAsociacion);
                break;
            case 'asociado':
                $('#grupoAsociado').show();
                cargarAsociados(idAsociacion);
                break;
            case 'colaborador':
                $('#grupoColaborador').show();
                cargarColaborador(idAsociacion);
                break;
        }
    }

    $('#IdAsociacion').change(function () {
        const nuevaAsociacion = $(this).val();
        if (!nuevaAsociacion) return;
        cargarConceptos(nuevaAsociacion);
    });

    $('#IdConceptoAsociacion').on('change', function () {
        const tipoEmisor = $(this).find('option:selected').data('tipoemisor');
        const idAsociacionActual = $('#IdAsociacion').val();
        $('#TipoEmisor').val(tipoEmisor); // <- actualiza el campo oculto

        if (tipoEmisor && idAsociacionActual) {
            mostrarCampoSegunTipo(tipoEmisor, idAsociacionActual);
        }
    });

    const idConceptoSeleccionado = $('#IdConceptoAsociacion').val();


    // ← ← ← ESTE bloque asegura que se muestre correctamente el campo tras postback
    if (idConceptoSeleccionado && tipoEmisorDesdeModelo && idAsociacion) {
        $('#IdConceptoAsociacion').data('selected', idConceptoSeleccionado);
        mostrarCampoSegunTipo(tipoEmisorDesdeModelo, idAsociacion);
    }

    if (modoVista === 'Create' && idAsociacion && $('#IdConceptoAsociacion option').length <= 1) {
        const idConceptoActual = $('#IdConceptoAsociacion').val();
        $('#IdConceptoAsociacion').data('selected', idConceptoActual);
        cargarConceptos(idAsociacion);
    }
});
