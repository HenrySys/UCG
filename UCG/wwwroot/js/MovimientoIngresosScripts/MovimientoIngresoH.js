$(document).ready(function () {

    const modoVista = $('form').data('mode') || 'Create';

    const documentosIngresosCargadas = JSON.parse($('#DetalleDocumentoIngresoJson').val() || '[]');

    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');

    let filaEgresoEditando = null;
    let esEdicionEgreso = false;

    if (successMessage) {
        Swal.fire({ icon: 'success', title: '¡Éxito!', text: successMessage, confirmButtonText: 'Ir al listado' })
            .then(result => { if (result.isConfirmed) window.location.href = '/TbMovimientoEgresos/Index'; });
    } else if (errorMessage) {
        Swal.fire({ icon: 'error', title: 'Error', text: errorMessage, confirmButtonText: 'Aceptar' });
    }

    $('#summernoteAcuerdo').summernote({
        height: 125,
        placeholder: 'Escriba el acuerdo aquí...',
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['para', ['ul', 'ol']],
            ['view', ['codeview']]
        ],

    });

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
            procesarColaSwal(); 
        });
    }

    function limpiarErrores() {
        $('.text-danger.mt-1').remove();
        $('.is-invalid').removeClass('is-invalid');
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

    function cargarAsociados(idAsociacion, valorSeleccionado = null) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: rutasFactura.obtenerAsociados,
                data: { idAsociacion },
                success: function (response) {
                    const dropdown = $('#IdAsociado');
                    dropdown.empty();
                    dropdown.append(`<option value="" disabled selected>Seleccione un asociado</option>`);
                    if (response.success) {
                        response.data.forEach(item => {
                            const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                            dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                        });

                        if (valorSeleccionado) dropdown.val(valorSeleccionado);
                        resolve();
                    } else {
                        mostrarErrorSwal('Atención', response.message);
                        reject();
                    }
                },
                error: function (_, __, error) {
                    mostrarErrorSwal('Error', 'Error al cargar asociados: ' + error);
                    reject();
                }
            });
        });
    }

    function cargarFinancistas(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerFinancistas, { idAsociacion }, '#modalFinancista', 'Seleccione un financista', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idFinancista}">${item.nombre}</option>`);
            });
        });
    }

    function cargarClientes(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerClientes, { idAsociacion }, '#modalCliente', 'Seleccione un cliente', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idCliente}">${item.nombre} ${item.apellido1 ?? ''} ${item.apellido2 ?? ''}</option>`);
            });
        });
    }

    function cargarActividades(idAsociacion) {
        const actividadesYaAgregadas = $('#detailsTableIngreso tbody tr').map(function () {
            return $(this).find('td:eq(0)').text().trim(); 
        }).get();

        fetchDropdownData(rutasFactura.obtenerActividades, { idAsociacion }, '#modalIdActividad', 'Seleccione una actividad', function (data, dropdown) {
            let disponibles = 0;
            data.forEach(item => {
                if (!actividadesYaAgregadas.includes(item.nombre)) {
                    dropdown.append(`<option value="${item.idActividad}">${item.nombre}</option>`);
                    disponibles++;
                }
            });

            if (disponibles === 0) {
                cerrarModalConMensaje('#detailModalIngreso', {
                    titulo: 'Actividades agotadas',
                    texto: 'Todas las actividades ya han sido agregadas.'
                });
            }
        });
    }

    function cargarConceptos(idAsociacion) {
        fetchDropdownData(rutasFactura.obtenerConceptos, { idAsociacion }, '#IdConceptoAsociacion', 'Seleccione un concepto', function (data, dropdown) {
            if (!data || !Array.isArray(data)) return;

            data.forEach(item => {
                dropdown.append(`<option value="${item.idConceptoAsociacion}" data-tipoemisor="${item.tipoOrigen.toLowerCase()}">${item.descripcionPersonalizada}</option>`);
            });

            const selected = $('#IdConceptoAsociacion').data('selected');
            if (selected) {
                $('#IdConceptoAsociacion').val(selected).trigger('change');
            }
        });
    }

    function ocultarTodosLosCampos() {
        $('#grupoCliente, #grupoFinancista, #grupoActividad').addClass('d-none');
    }

    function mostrarCampoSegunTipo(tipoEmisor, idAsociacion) {
        ocultarTodosLosCampos();

        switch (tipoEmisor) {
            case 'cliente':
                $('#grupoCliente').removeClass('d-none');
                cargarClientes(idAsociacion);
                break;
            case 'financista':
                $('#grupoFinancista').removeClass('d-none');
                cargarFinancistas(idAsociacion);
                break;
            case 'actividad':
                $('#grupoActividad').removeClass('d-none');
                cargarActividades(idAsociacion);
                break;
        }
    }


    // Cargar asociados cuando cambie la asociación
    $('#IdAsociacion').on('change', function () {
        const idAsociacionSeleccionada = $(this).val();
        if (idAsociacionSeleccionada) {
            cargarAsociados(idAsociacionSeleccionada);
            $('#IdConceptoAsociacion').empty().append('<option disabled selected>Seleccione un concepto</option>');
            cargarConceptos(idAsociacionSeleccionada);
        }
    });


    // Al cargar la página
    const idConceptoSeleccionado = $('#IdConceptoAsociacion').val();
    const tipoEmisorDesdeModelo = $('#IdConceptoAsociacion option:selected').data('tipoemisor');
    const idAsociacion = $('#IdAsociacion').val();

    if (idConceptoSeleccionado && tipoEmisorDesdeModelo && idAsociacion) {
        $('#IdConceptoAsociacion').data('selected', idConceptoSeleccionado);
        mostrarCampoSegunTipo(tipoEmisorDesdeModelo, idAsociacion);
    }

    $('#IdConceptoAsociacion').on('change', function () {
        const tipoEmisor = $(this).find('option:selected').data('tipoemisor');
        const idAsociacionActual = $('#IdAsociacion').val();

        if (tipoEmisor && idAsociacionActual) {
            mostrarCampoSegunTipo(tipoEmisor, idAsociacionActual);
        }
    });

    // Carga inicial si es vista de creación
    if (modoVista === 'Create' && idAsociacion && $('#IdConceptoAsociacion option').length <= 1) {
        const idConceptoActual = $('#IdConceptoAsociacion').val();
        $('#IdConceptoAsociacion').data('selected', idConceptoActual);
        cargarConceptos(idAsociacion);
    }

    function reconstruirDetalleDocumentoIngresoUI() {
        $('#detailsTableIngreso tbody').empty();
        documentosIngresosCargadas.forEach((d, index) => {
            let origen = d.IdCliente ? 'Cliente' : d.IdFinancista ? 'Financista' : d.IdActividad ? 'Actividad' : '-';
            $('#detailsTableIngreso tbody').append(`
                <tr data-index="${index}">
                    <td>${d.NumComprobante}</td>
                    <td>${d.FechaComprobante || ''}</td>
                    <td>${obtenerTextoMetodoPago(d.MetodoPago)}</td>
                    <td>${parseFloat(d.Monto || 0).toFixed(2)}</td>
                    <td>${origen}</td>
                    <td>
                        <button type="button" class="btn btn-warning btn-sm editar-detalle" data-index="${index}">Editar</button>
                        <button type="button" class="btn btn-danger btn-sm eliminar-detalle" data-index="${index}">Eliminar</button>
                    </td>
                </tr>
            `);
        });
        actualizarMontoTotalIngreso();
    }

    reconstruirDetalleDocumentoIngresoUI();

    function obtenerTextoMetodoPago(valor) {
        switch (valor) {
            case 1: return 'Efectivo';
            case 2: return 'Sinpe';
            case 3: return 'Transferencia';  
            default: return '-';
        }
    }

    function obtenerValorMetodoPagoTexto(texto) {
        switch (texto.toLowerCase()) {
            case 'efectivo': return 1;
            case 'sinpe': return 2;
            case 'transferencia': return 3;
            default: return 0;
        }
    }

    function recolectarDetallesDocumentoIngreso() {
        const detalles = [];

        $('#detailsTableIngreso tbody tr').each(function () {
            const fila = $(this);
            const detalle = {
                NumComprobante: fila.find('td:eq(0)').text().trim(),
                FechaComprobante: fila.find('td:eq(1)').text().trim(),
                MetodoPago: obtenerValorMetodoPagoTexto(fila.find('td:eq(2)').text().trim()),
                Monto: parseFloat(fila.find('td:eq(3)').text().trim()),
                Origen: fila.find('td:eq(4)').text().trim()
            };
            detalles.push(detalle);
        });

        return detalles;
    }


    $('form').on('submit', function (e) {
        limpiarErrores();

        const detalles = recolectarDetallesDocumentoIngreso();

        if (detalles.length === 0) {
            $('#detailsTableIngreso').after('<div id="errorTablaIngreso" class="text-danger mt-1">Debe agregar al menos un documento de ingreso.</div>');
            e.preventDefault();
            return;
        }

        //  Guarda los detalles correctamente
        $('#DetalleDocumentoIngresoJson').val(JSON.stringify(detalles));

        const montoInput = $('#MontoTotal input');
        let monto = montoInput.val();
        if (monto && monto.includes('.')) {
            monto = monto.replace('.', ','); 
            montoInput.val(monto);
        }
    });



    // Eliminar fila de detalle con confirmación para documentos de ingreso
    $('#detailsTableIngreso').on('click', '.eliminar-detalle', function () {
        const fila = $(this).closest('tr');
        Swal.fire({
            title: '¿Está seguro?',
            text: '¿Desea eliminar este documento de ingreso?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fila.remove();
                actualizarMontoTotalIngreso(); // Debe existir si querés recalcular el total
                Swal.fire('Eliminado', 'El documento ha sido eliminado.', 'success');
            }
        });
    });


    function limpiarCamposModalDetalleFactura() {
        // Limpiar campos de texto, fecha y número
        $('#modalNumComprobante').val('');
        $('#modalFechaComprobante').val('');
        $('#modalMonto').val('');
        $('#modalDescripcion').val('');
        $('#modalMetodoPago').val('');

        // Limpiar selects de origen
        $('#modalCliente').val('');
        $('#modalFinancista').val('');
        $('#modalIdActividad').val('');

        // Eliminar clases de error
        $('#detailModalIngreso .is-invalid').removeClass('is-invalid');
        $('#detailModalIngreso .text-danger.mt-1').remove();
    }


    $('#addDetailIngresoBtn').on('click', function () {
        limpiarErrores();
        let hayError = false;

        const numComprobante = $('#modalNumComprobante').val()?.trim();
        const fechaComprobante = $('#modalFechaComprobante').val();
        const monto = parseFloat($('#modalMonto').val());
        const metodoPago = parseInt($('#modalMetodoPago').val());
        const descripcion = $('#modalDescripcion').val()?.trim();
        const idCliente = $('#modalCliente').val();
        const idFinancista = $('#modalFinancista').val();
        const idActividad = $('#modalIdActividad').val();

        if (!numComprobante) {
            $('#modalNumComprobante').addClass('is-invalid').after('<div class="text-danger mt-1">Debe ingresar el número de comprobante.</div>');
            hayError = true;
        }
        if (!fechaComprobante) {
            $('#modalFechaComprobante').addClass('is-invalid').after('<div class="text-danger mt-1">Debe ingresar la fecha del comprobante.</div>');
            hayError = true;
        }
        if (isNaN(monto) || monto <= 0) {
            $('#modalMonto').addClass('is-invalid').after('<div class="text-danger mt-1">Debe ingresar un monto válido.</div>');
            hayError = true;
        }
        if (isNaN(metodoPago) || metodoPago < 1 || metodoPago > 3) {
            $('#modalMetodoPago').addClass('is-invalid').after('<div class="text-danger mt-1">Seleccione un método de pago válido.</div>');
            hayError = true;
        }
        if (!descripcion) {
            $('#modalDescripcion').addClass('is-invalid').after('<div class="text-danger mt-1">Debe ingresar una descripción.</div>');
            hayError = true;
        }
        if (!idCliente && !idFinancista && !idActividad) {
            mostrarErrorSwal('Origen requerido', 'Debe seleccionar un cliente, financista o actividad.');
            hayError = true;
        }

        if (hayError) return;

        let origen = idCliente ? 'Cliente' : idFinancista ? 'Financista' : idActividad ? 'Actividad' : '-';

        const fila = `
            <tr>
                <td>${numComprobante}</td>
                <td>${fechaComprobante}</td>
                <td>${obtenerTextoMetodoPago(metodoPago)}</td>
                <td>₡${monto.toFixed(2)}</td>
                <td>${origen}</td>
                <td>
                    <button type="button" class="btn btn-warning btn-sm editar-detalle">Editar</button>
                    <button type="button" class="btn btn-danger btn-sm eliminar-detalle">Eliminar</button>
                </td>
            </tr>
        `;

        $('#detailsTableIngreso tbody').append(fila);
        $('#detailModalIngreso').modal('hide');
        limpiarCamposModalDetalleFactura();
        actualizarMontoTotalIngreso();
    });

    // Abrir modal al hacer clic en el botón "Agregar"
    $('#btnAbrirModalIngreso').on('click', function () {
        $('#detailModalIngreso').modal('show');
    });



    $('#modalIdActividad').on('change', function () {
        const idActividad = $(this).val();

        if (idActividad) {
            $.ajax({
                url: rutasFactura.obtenerMontoActividad,
                data: { idActividad },
                success: function (response) {
                    if (response.success && response.monto !== undefined) {
                        $('#modalMonto').val(response.monto.toFixed(2)).prop('readonly', true);
                    } else {
                        $('#modalMonto').prop('readonly', false);
                        mostrarErrorSwal('Atención', 'No se pudo obtener el monto de la actividad.');
                    }
                },
                error: function (_, __, error) {
                    $('#modalMonto').prop('readonly', false);
                    mostrarErrorSwal('Error', 'Error al obtener el monto: ' + error);
                }
            });
        } else {
            $('#modalMonto').prop('readonly', false);
        }
    });


    function actualizarMontoTotalIngreso() {
        let total = 0;

        $('#detailsTableIngreso tbody tr').each(function () {
            const monto = parseFloat($(this).find('td:eq(3)').text()) || 0; // Columna del monto
            total += monto;
        });

        // Actualizar el input readonly
        $('#MontoTotal input').val(total.toFixed(2));
    }


    actualizarMontoTotalIngreso();

});