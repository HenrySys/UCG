$(document).ready(function () {
    const modoVista = $('form').data('mode') || 'Create';

    const chequeFacturaCargadas = JSON.parse($('#DetalleChequeFacturaEgresoJason').val() || '[]');

    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const idAsociacion = $('#tempDataSwal').data('asociacion') || $('#IdAsociacion').val();


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
            procesarColaSwal(); // Mostrar el siguiente si existe
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
                url: rutasMovimientoEgreso.obtenerAsociados,
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

                        if (valorSeleccionado) dropdown.val(valorSeleccionado); // Restaurar selección
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


    function cargarActas(idAsociacion, valorSeleccionado = null) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: rutasMovimientoEgreso.obtenerActas,
                data: { idAsociacion },
                success: function (response) {
                    const dropdown = $('#IdActa');
                    dropdown.empty();
                    dropdown.append(`<option value="" disabled selected>Seleccione un acta</option>`);
                    if (response.success) {
                        response.data.forEach(item => {
                            dropdown.append(`<option value="${item.idActa}">${item.numeroActa}</option>`);
                        });

                        if (valorSeleccionado) dropdown.val(valorSeleccionado); // Restaurar selección
                        resolve();
                    } else {
                        mostrarErrorSwal('Atención', response.message);
                        reject();
                    }
                },
                error: function (_, __, error) {
                    mostrarErrorSwal('Error', 'Error al cargar actas: ' + error);
                    reject();
                }
            });
        });
    }


    function cargarAcuerdos(idActa) {
        fetchDropdownData(rutasMovimientoEgreso.obtenerAcuerdos, { idActa }, '#modalIdAcuerdo', 'Seleccione un acuerdo', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idAcuerdo}">${item.numeroAcuerdo}</option>`);
            });
            $('#modalIdAcuerdo').trigger('change.select2');
        });
    }

    function cargarFacturas(idAsociacion) {
        const facturasYaAgregadas = $('#detailsTableEgreso tbody tr').map(function () {
            return $(this).find('td:eq(1)').text().trim(); 
        }).get();
        fetchDropdownData(rutasMovimientoEgreso.obtenerFacturas, { idAsociacion }, '#modalIdFactura', 'Seleccione una factura', function (data, dropdown) {
            let disponibles = 0;
            data.forEach(item => {
                if (!facturasYaAgregadas.includes(item.numeroFactura)) {
                    dropdown.append(`<option value="${item.idFactura}">${item.numeroFactura}</option>`);
                    disponibles++;
                }
            });

            if (disponibles === 0) {
                cerrarModalConMensaje('#detailModalEgreso', {
                    titulo: 'Facturas agotadas',
                    texto: 'Todas las facturas ya han sido agregadas.'
                });
            }

        });
    }

    function cargarCheques(idAsociacion) {
        fetchDropdownData(rutasMovimientoEgreso.obtenerCheques, { idAsociacion }, '#modalIdCheque', 'Seleccione un cheque', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idCheque}">${item.numeroCheque}</option>`);
            });

            $('#modalIdCheque').trigger('change.select2');

        });
    }
    function reconstruirEgresosUI() {
        $('#detailsTableEgreso tbody').empty();

        chequeFacturaCargadas.forEach((item, index) => {
            console.log(`Egreso ${index}:`, item);

            const monto = parseFloat(item.Monto) || 0;
            const descripcion = item.Descripcion ?? '';
            const numeroCheque = item.NumeroCheque ?? `Cheque ${item.IdCheque}`;
            const numeroFactura = item.NumeroFactura ?? `Factura ${item.IdFactura}`;
            const numeroAcuerdo = item.NumeroAcuerdo ?? `Acuerdo ${item.IdAcuerdo}`;

            const fila = `
             <tr 
                data-index="${index}" 
                data-idacuerdo="${item.IdAcuerdo}" 
                data-idfactura="${item.IdFactura}" 
                data-idcheque="${item.IdCheque}" 
                data-monto="${monto}" 
                data-descripcion="${descripcion}"
                data-numerocheque="${numeroCheque}" 
                data-numerofactura="${numeroFactura}" 
                data-numeroacuerdo="${numeroAcuerdo}">
                <td>${numeroCheque}</td>
                <td>${numeroFactura}</td>
                <td>${monto.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' })}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-edit-egreso">Editar</button>
                    <button type="button" class="btn btn-sm btn-danger btn-remove-egreso">Eliminar</button>
                </td>
            </tr>
        `;

            $('#detailsTableEgreso tbody').append(fila);
        });
        actualizarMontoTotal();
    }


    reconstruirEgresosUI();
    actualizarMontoTotal(); // después de reconstruir


    function recolectarDetalleChequeFactura() {
        const detalles = [];
        $('#detailsTableEgreso tbody tr').each(function () {
            const fila = $(this);
            detalles.push({
                IdCheque: parseInt(fila.data('idcheque')),
                IdFactura: parseInt(fila.data('idfactura')),
                IdAcuerdo: parseInt(fila.data('idacuerdo')),
                Monto: parseFloat(fila.data('monto')),
                Descripcion: fila.data('descripcion'),
                NumeroCheque: fila.data('numerocheque'),
                NumeroFactura: fila.data('numerofactura'),
                NumeroAcuerdo: fila.data('numeroacuerdo')
            });
        });
        return detalles;
    }


    function limpiarCamposModalEgreso() {
        $('#modalIdAcuerdo').val("0").removeClass('is-invalid');
        $('#modalIdFactura').val("0").removeClass('is-invalid');
        $('#modalIdCheque').val("0").removeClass('is-invalid');
        $('#modalMonto').val('').removeClass('is-invalid');
        $('#montoChequeRestante').remove();

        $('#summernoteAcuerdo').summernote('code', '');
        $('#summernoteAcuerdo').removeClass('is-invalid');

        $('.text-danger.mt-1').remove();
    }


    function configurarModalEgresos() {
        $('#btnAbrirModalEgreso').off('click').on('click', function () {
            if (esEdicionEgreso) {
                $('#detailModalEgreso').modal('show');
                return;
            }

            const idAsociacion = $('#IdAsociacion').val();
            const idActa = $('#IdActa').val();

            if (!idAsociacion || parseInt(idAsociacion) === 0) {
                Swal.fire({
                    icon: 'info',
                    title: 'Debe seleccionar una asociación',
                    text: 'Por favor, seleccione una asociación antes de agregar detalles.',
                    timer: 3000,
                    showConfirmButton: false
                });
                return;
            }

            if (!idActa || parseInt(idActa) === 0) {
                Swal.fire({
                    icon: 'info',
                    title: 'Debe seleccionar un acta',
                    text: 'Por favor, seleccione un acta antes de agregar detalles.',
                    timer: 3000,
                    showConfirmButton: false
                });
                return;
            }

            // Validar facturas antes de abrir el modal
            $.get(rutasMovimientoEgreso.obtenerFacturas, { idAsociacion }, function (response) {
                const facturasAgregadas = $('#detailsTableEgreso tbody tr').map(function () {
                    return $(this).find('td:eq(1)').text().trim();
                }).get();

                const disponibles = response.data.filter(f => !facturasAgregadas.includes(f.numeroFactura));

                if (disponibles.length === 0) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Facturas agotadas',
                        text: 'No hay facturas disponibles para agregar.',
                        timer: 3000,
                        showConfirmButton: false
                    });
                    return;
                }

                // Si hay facturas disponibles, entonces sí abrís el modal
                cargarAcuerdos(idActa);
                cargarCheques(idAsociacion);
                cargarFacturas(idAsociacion);
                limpiarErrores();
                limpiarCamposModalEgreso();

                $('#detailModalEgreso').modal('show');
            });
        });
    }





    if (modoVista === 'Create' || modoVista === 'Edit') {
        const idAsociacion = $('#IdAsociacion').val();
        const idActaSeleccionada = $('#IdActa').val();
        const idAsociadoSeleccionado = $('#IdAsociado').val();

        if (idAsociacion && parseInt(idAsociacion) > 0) {
            cargarAsociados(idAsociacion, idAsociadoSeleccionado);
            cargarActas(idAsociacion, idActaSeleccionada);
        }

        configurarModalEgresos();
    }


        
    $('#IdAsociacion').change(function () {
        const nuevaAsociacion = $(this).val();
        if (!nuevaAsociacion) return;

        cargarAsociados(nuevaAsociacion);
        cargarActas(nuevaAsociacion);

    });

    


    $('#addDetailEgresoBtn').on('click', function () {
        limpiarErrores();

        const idAcuerdo = $('#modalIdAcuerdo').val();
        const nombreAcuerdo = $('#modalIdAcuerdo option:selected').text().trim();

        const idFactura = $('#modalIdFactura').val();
        const nombreFactura = $('#modalIdFactura option:selected').text().trim();

        const idCheque = $('#modalIdCheque').val();
        const numeroCheque = $('#modalIdCheque option:selected').text().trim();

        const monto = parseFloat($('#modalMonto').val());
        const descripcionHTML = $('#summernoteAcuerdo').summernote('code');
        const descripcionTextoPlano = $('<div>').html(descripcionHTML).text().trim();

        let hayError = false;

        if (!idAcuerdo || idAcuerdo === "0") {
            $('#modalIdAcuerdo').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe seleccionar un acuerdo.</div>');
            hayError = true;
        }

        if (!idFactura || idFactura === "0") {
            $('#modalIdFactura').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe seleccionar una factura.</div>');
            hayError = true;
        }

        if (!idCheque || idCheque === "0") {
            $('#modalIdCheque').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe seleccionar un cheque.</div>');
            hayError = true;
        }

        if (isNaN(monto) || monto <= 0) {
            $('#modalMonto').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe ingresar un monto válido.</div>');
            hayError = true;
        }

        if (descripcionTextoPlano.length === 0) {
            $('#summernoteAcuerdo').addClass('is-invalid')
                .closest('.mb-3')
                .append('<div class="text-danger mt-1">La descripción no puede estar vacía.</div>');
            hayError = true;
        }

        if (hayError) return;

        if (filaEgresoEditando !== null) {
            // Actualizar fila existente
            filaEgresoEditando
                .attr('data-idacuerdo', idAcuerdo)
                .attr('data-idfactura', idFactura)
                .attr('data-idcheque', idCheque)
                .attr('data-monto', monto)
                .attr('data-descripcion', descripcionTextoPlano)
                .attr('data-numerocheque', numeroCheque)
                .attr('data-numerofactura', nombreFactura)
                .attr('data-numeroacuerdo', nombreAcuerdo);

            filaEgresoEditando.find('td:eq(0)').text(numeroCheque);
            filaEgresoEditando.find('td:eq(1)').text(nombreFactura);
            filaEgresoEditando.find('td:eq(2)').text(monto.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' }));

            filaEgresoEditando = null;
        } else {
            // Agregar nueva fila
            $('#detailsTableEgreso tbody').append(`
            <tr 
                data-idacuerdo="${idAcuerdo}" 
                data-idfactura="${idFactura}" 
                data-idcheque="${idCheque}" 
                data-monto="${monto}" 
                data-descripcion="${descripcionTextoPlano}"
                data-numerocheque="${numeroCheque}" 
                data-numerofactura="${nombreFactura}" 
                data-numeroacuerdo="${nombreAcuerdo}">
                <td>${numeroCheque}</td>
                <td>${nombreFactura}</td>
                <td>${monto.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' })}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-edit-egreso">Editar</button>
                    <button type="button" class="btn btn-sm btn-danger btn-remove-egreso">Eliminar</button>
                </td>
            </tr>
        `);
        }

        actualizarMontoTotal();
        limpiarErrores();
        $('#detailModalEgreso').modal('hide');
    });


    function verificarDisponibilidadDatosAsociacion() {
        const idAsociacion = $('#IdAsociacion').val();
        const modoVista = $('form').data('mode'); // 'Create' o 'Edit'

        // Si no hay asociación seleccionada
        if (!idAsociacion || parseInt(idAsociacion) === 0) {
            // En modo Create, mantener habilitado
            if (modoVista === 'Create') {
                $('#btnAbrirModalEgreso').prop('disabled', false);
            }
            return;
        }

        $.get('/TbMovimientoEgresos/VerificarDatosAsociacion', { idAsociacion }, function (response) {
            if (response.success) {
                const tieneCheques = response.tieneCheques;
                const tieneFacturas = response.tieneFacturas;

                if (!tieneCheques || !tieneFacturas) {
                    $('#btnAbrirModalEgreso').prop('disabled', true);

                    let mensaje = 'La asociación no tiene ';
                    if (!tieneCheques && !tieneFacturas) {
                        mensaje += 'cheques ni facturas.';
                    } else if (!tieneCheques) {
                        mensaje += 'cheques.';
                    } else {
                        mensaje += 'facturas.';
                    }

                    // Mostrar el error solo si estoy en Edit o ya hay una asociación válida
                    if (modoVista === 'Edit' || idAsociacion) {
                        mostrarErrorSwal('Datos insuficientes', mensaje);
                    }
                } else {
                    $('#btnAbrirModalEgreso').prop('disabled', false);
                }
            } else {
                $('#btnAbrirModalEgreso').prop('disabled', true);
                mostrarErrorSwal('Error al verificar asociación', response.message);
            }
        });
    }


    verificarDisponibilidadDatosAsociacion();


    $('#IdAsociacion').change(function () {
        verificarDisponibilidadDatosAsociacion();
    });

    function seleccionarComboForzado(selector, valor, textoFallback) {
        const $select = $(selector);
        if (!$select.find(`option[value="${valor}"]`).length) {
            $select.append(`<option value="${valor}">${textoFallback}</option>`);
        }
        $select.val(valor);
    }

    $('#detailsTableEgreso').on('click', '.btn-edit-egreso', function () {
        limpiarErrores();

        const fila = $(this).closest('tr');
        filaEgresoEditando = fila;
        esEdicionEgreso = true;

        const idAcuerdo = fila.attr('data-idacuerdo');
        const idFactura = fila.attr('data-idfactura');
        const idCheque = fila.attr('data-idcheque');
        const monto = fila.attr('data-monto');
        const descripcion = fila.attr('data-descripcion') ?? '';

        const numeroCheque = fila.attr('data-numerocheque') ?? `Cheque ${idCheque}`;
        const numeroFactura = fila.attr('data-numerofactura') ?? `Factura ${idFactura}`;
        const textoAcuerdo = fila.attr('data-numeroacuerdo') ?? `Acuerdo ${idAcuerdo}`;

        seleccionarComboForzado('#modalIdAcuerdo', idAcuerdo, textoAcuerdo);
        seleccionarComboForzado('#modalIdFactura', idFactura, numeroFactura);
        seleccionarComboForzado('#modalIdCheque', idCheque, numeroCheque);

        $('#modalMontoVisible').val(parseFloat(monto).toLocaleString('es-CR', { style: 'currency', currency: 'CRC' }));
        $('#modalMonto').val(monto);

        $('#summernoteAcuerdo').summernote({
            height: 125,
            placeholder: 'Escriba la descripción...',
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['view', ['codeview']]
            ]
        }).summernote('code', descripcion);

        if (modoVista === 'Edit') {
            $('#camposOcultablesEdit').hide();
        } else {
            $('#camposOcultablesEdit').show();
        }

        $('#detailModalEgreso').modal('show');
    });





    $('#detailModalEgreso').on('hidden.bs.modal', function () {
        limpiarErrores();
        limpiarCamposModalEgreso();
        filaEgresoEditando = null;
        esEdicionEgreso = false;

        $('#camposOcultablesEdit').show(); // Asegura que se restablezca al cerrar


    });

    $('#detailsTableEgreso').on('click', '.btn-remove-egreso', function () {
        const fila = $(this).closest('tr');
        Swal.fire({
            title: '¿Está seguro?',
            text: '¿Desea eliminar este egreso?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fila.remove();
                actualizarMontoTotal();

                Swal.fire('Eliminado', 'El egreso ha sido eliminado.', 'success');
            }
        });
    });
    $('form').on('submit', function (e) {
        e.preventDefault(); // Detener el envío por defecto
        limpiarErrores();

        const detallesChequFactura = recolectarDetalleChequeFactura();

        if (detallesChequFactura.length === 0) {
            $('#detailsTableEgreso').after('<div id="errorTablaEgreso" class="text-danger mt-1">Debe agregar al menos un egreso.</div>');
            return; // No continuar
        }

        // Si hay detalles, preguntar antes de enviar
        Swal.fire({
            title: '¿Desea guardar el movimiento?',
            text: 'Esta acción guardará los datos del egreso.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Sí, guardar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                // Serializar y enviar
                $('#DetalleChequeFacturaEgresoJason').val(JSON.stringify(detallesChequFactura));
                e.target.submit(); // Enviar el formulario manualmente
            }
        });
    });



    $('#modalIdCheque').on('change', function () {
        const idCheque = $(this).val();
        if (!idCheque || idCheque === "0") return;

        const detalles = $('#detailsTableEgreso tbody tr');
        let montoUsado = 0;

        detalles.each(function () {
            const idChequeDetalle = $(this).data('idcheque');
            const monto = parseFloat($(this).data('monto')) || 0;
            if (parseInt(idChequeDetalle) === parseInt(idCheque)) {
                montoUsado += monto;
            }
        });

        // Llamar al backend para obtener el monto base del cheque
        $.get(`/TbMovimientoEgresos/GetMontoCheque/${idCheque}`, function (data) {
            const montoBase = parseFloat(data.montoRestante || data.monto || 0);
            const montoDisponible = montoBase - montoUsado;

            $('#montoChequeRestante').remove();
            $('#modalIdCheque').after(`<div id="montoChequeRestante" class="mt-2 text-info">Monto restante disponible: ₡${montoDisponible.toLocaleString('es-CR')}</div>`);

            // Obtener el monto automáticamente desde la factura seleccionada
            $('#modalIdFactura').off('change').on('change', function () {
                const facturaId = $(this).val();
                if (!facturaId || facturaId === "0") return;

                $.get(`/TbMovimientoEgresos/GetMontoFactura/${facturaId}`, function (facturaData) {
                    const montoFactura = parseFloat(facturaData.montoTotal);
                    if (!isNaN(montoFactura)) {
                        const montoFormateado = montoFactura.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' });

                        // Mostrar en campo visible con formato
                        $('#modalMontoVisible')
                            .val(montoFormateado)
                            .attr('readonly', true)
                            .addClass('bg-light');

                        // Asignar valor decimal real al campo oculto que va al servidor
                        $('#modalMonto').val(montoFactura);

                        // Validar contra el monto restante
                        if (montoFactura > montoDisponible) {
                            Swal.fire({
                                icon: 'info',
                                title: 'Monto excedido',
                                text: 'El monto de la factura supera el monto disponible del cheque.',
                                timer: 3000,
                                showConfirmButton: false
                            });
                        }
                    }
                });
            });
        });
    });


    function actualizarMontoTotal() {
        let total = 0;
        $('#detailsTableEgreso tbody tr').each(function () {
            const monto = parseFloat($(this).data('monto')) || 0;
            total += monto;
        });

        // Asignar el total al campo Monto del modelo
        $('#Monto').val(total);

        // También puedes formatearlo visualmente si quieres mostrarlo en un campo visible aparte
        $('#Monto').closest('.mb-3').find('.text-info-monto-total').remove();
        $('#MontoTotal').after(`<div class="text-info text-info-monto-total mt-1">Total: ₡${total.toLocaleString('es-CR')}</div>`);
    }

   
   




});