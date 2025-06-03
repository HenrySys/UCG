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
            .then(result => { if (result.isConfirmed) window.location.href = '/TbActums/Index'; });
    } else if (errorMessage) {
        Swal.fire({ icon: 'error', title: 'Error', text: errorMessage, confirmButtonText: 'Aceptar' });
    }

    $('#summernoteAcuerdo').summernote({
        placeholder: 'Ingrese la descripción...',
        tabsize: 2,
        height: 120
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

    function cargarAsociados(idAsociacion) {
        fetchDropdownData(rutasMovimientoEgreso.obtenerAsociados, { idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
            data.forEach(item => {
                const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
            });
        });
    }

    function cargarActas(idAsociacion) {
        fetchDropdownData(rutasMovimientoEgreso.obtenerActas, { idAsociacion }, '#IdActa', 'Seleccione un acta', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idActa}">${item.numeroActa}</option>`);
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

            $('#modalIdFactura').trigger('change.select2');


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

            const fila = `
                <tr data-index="${index}" 
                    data-idacuerdo="${item.IdAcuerdo}" 
                    data-idfactura="${item.IdFactura}" 
                    data-idcheque="${item.IdCheque}" 
                    data-monto="${item.Monto}" 
                    data-descripcion="${item.Descripcion}">
            
                    <td>${item.NumeroCheque}</td>
                    <td>${item.NumeroFactura}</td>
                    <td>${item.Monto.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' })}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-warning btn-edit-egreso" data-index="${index}">Editar</button>
                        <button type="button" class="btn btn-danger btn-sm btn-remove-egreso" data-index="${index}">Eliminar</button>
                    </td>
                </tr>
            `;

            $('#detailsTableEgreso tbody').append(fila);
        });
    }
    reconstruirEgresosUI();

    function recolectarDetalleChequeFactura() {
        const detalles = [];
        const filas = $('#detailsTableEgreso tbody tr');

        filas.each(function () {
            const fila = $(this);

            const idCheque = parseInt(fila.data('idcheque'), 10);
            const idFactura = parseInt(fila.data('idfactura'), 10);
            const idAcuerdo = parseInt(fila.data('idacuerdo'), 10);
            const monto = parseFloat(fila.data('monto'));
            const descripcion = fila.data('descripcion');

            detalles.push({
                IdCheque: idCheque,
                IdFactura: idFactura,
                IdAcuerdo: idAcuerdo,
                Monto: monto,
                Descripcion: descripcion
            });
        });

        return detalles;
    }

    function limpiarCamposModalEgreso() {
        $('#modalIdAcuerdo').val("0").removeClass('is-invalid');
        $('#modalIdFactura').val("0").removeClass('is-invalid');
        $('#modalIdCheque').val("0").removeClass('is-invalid');
        $('#modalMonto').val('').removeClass('is-invalid');

        $('#summernoteAcuerdo').summernote('code', '');
        $('#summernoteAcuerdo').removeClass('is-invalid');

        $('.text-danger.mt-1').remove();
    }


    function configurarModalEgresos() {
        $('#detailModalEgreso').off('show.bs.modal').on('show.bs.modal', function (e) {
            if (esEdicionEgreso) return; // si es edición, permitir sin validar

            const idAsociacion = $('#IdAsociacion').val();
            const idActa = $('#IdActa').val();

            if (!idAsociacion || parseInt(idAsociacion) === 0) {
                e.preventDefault();
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
                e.preventDefault();
                Swal.fire({
                    icon: 'info',
                    title: 'Debe seleccionar un acta',
                    text: 'Por favor, seleccione un acta antes de agregar detalles.',
                    timer: 3000,
                    showConfirmButton: false
                });
                return;
            }

            limpiarErrores();
            limpiarCamposModalEgreso();

            cargarAcuerdos(idActa);
            cargarCheques(idAsociacion);
            cargarFacturas(idAsociacion);
        });
    }




    if (modoVista === 'Create') {
        const idAsociacion = $('#IdAsociacion').val();
        if (idAsociacion && parseInt(idAsociacion) > 0) {
            cargarAsociados(idAsociacion);
            cargarActas(idAsociacion);
        }
        configurarModalEgresos(); // Solo se configura una vez
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
        const descripcionTextoPlano = $('<div>').html($('#summernoteAcuerdo').summernote('code')).text().trim();

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

        // ✅ ACTUALIZAR correctamente los atributos
        if (filaEgresoEditando !== null) {
            filaEgresoEditando
                .attr('data-idacuerdo', idAcuerdo)
                .attr('data-idfactura', idFactura)
                .attr('data-idcheque', idCheque)
                .attr('data-monto', monto)
                .attr('data-descripcion', descripcionTextoPlano); // <== ESTA ES LA CLAVE

            filaEgresoEditando.find('td:eq(0)').text(numeroCheque);
            filaEgresoEditando.find('td:eq(1)').text(nombreFactura);
            filaEgresoEditando.find('td:eq(2)').text(monto.toLocaleString('es-CR', { style: 'currency', currency: 'CRC' }));

            // Limpieza de referencia de edición
            filaEgresoEditando = null;
        } else {
            $('#detailsTableEgreso tbody').append(`
            <tr data-idacuerdo="${idAcuerdo}" data-idfactura="${idFactura}" data-idcheque="${idCheque}" data-monto="${monto}" data-descripcion="${descripcionTextoPlano}">
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

        limpiarErrores();
        $('#detailModalEgreso').modal('hide');
    });



    $('#detailsTableEgreso').on('click', '.btn-edit-egreso', function () {
        limpiarErrores();
        const fila = $(this).closest('tr');

        const idAcuerdo = fila.data('idacuerdo');
        const idFactura = fila.data('idfactura');
        const idCheque = fila.data('idcheque');
        const monto = fila.data('monto');
        const descripcion = fila.attr('data-descripcion');

        $('#modalIdAcuerdo').val(idAcuerdo);
        $('#modalIdFactura').val(idFactura);
        $('#modalIdCheque').val(idCheque);
        $('#modalMonto').val(monto);

        $('#summernoteAcuerdo').summernote({
            height: 125,
            placeholder: 'Escriba la descripción...',
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['view', ['codeview']]
            ]
        });
        $('#summernoteAcuerdo').summernote('code', descripcion);

        filaEgresoEditando = fila;
        esEdicionEgreso = true; // activar modo edición

        $('#detailModalEgreso').modal('show');
    });


    $('#detailModalEgreso').on('hidden.bs.modal', function () {
        limpiarErrores();
        limpiarCamposModalEgreso();
        filaEgresoEditando = null;
        esEdicionEgreso = false;
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
                Swal.fire('Eliminado', 'El egreso ha sido eliminado.', 'success');
            }
        });
    });

    $('form').on('submit', function (e) {
        limpiarErrores();

        const detallesChequFactura = recolectarDetalleChequeFactura();

        if (detallesChequFactura.length === 0) {
            $('#detailsTableEgreso').after('<div id="errorTablaEgreso" class="text-danger mt-1">Debe agregar al menos un egreso.</div>');
            e.preventDefault();
        } else {
            $('#DetalleChequeFacturaEgresoJason').val(JSON.stringify(detallesChequFactura));
        }   
    });

});