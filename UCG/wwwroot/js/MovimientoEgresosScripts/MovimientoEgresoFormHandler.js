$(document).ready(function () {
    const modoVista = $('form').data('mode') || 'Create';

    const chequeFacturaCargadas = JSON.parse($('#DetalleChequeFacturaEgresoJason').val() || '[]');

    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const idAsociacion = $('#tempDataSwal').data('asociacion') || $('#IdAsociacion').val();

    let filaAcuerdoEditando = null;


    if (successMessage) {
        Swal.fire({ icon: 'success', title: '¡Éxito!', text: successMessage, confirmButtonText: 'Ir al listado' })
            .then(result => { if (result.isConfirmed) window.location.href = '/TbActums/Index'; });
    } else if (errorMessage) {
        Swal.fire({ icon: 'error', title: 'Error', text: errorMessage, confirmButtonText: 'Aceptar' });
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
        fetchDropdownData(rutasMovimiento.obtenerFolios, { idAsociacion }, '#IdFolio', 'Seleccione un folio', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idFolio}">${item.numeroFolio}</option>`);
            });
        });
    }

    function cargarAcuerdos(idAsociacion) {
        fetchDropdownData(rutasMovimiento.obtenerAcuerdos, { idAsociacion }, '#modalIdAcuerdo', 'Seleccione un acuerdo', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idAcuerdo}">${item.descripcion}</option>`);
            });
        });
    }

    function cargarFacturas(idAsociacion) {
        const facturasYaAgregadas = $('#detailsTableEgreso tbody tr').map(function () {
            return $(this).find('td:eq(1)').text().trim(); 
        }).get();
        fetchDropdownData(rutasMovimiento.obtenerFacturas, { idAsociacion }, '#modalIdFactura', 'Seleccione una factura', function (data, dropdown) {
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
        fetchDropdownData(rutasMovimiento.obtenerCheques, { idAsociacion }, '#modalIdCheque', 'Seleccione un cheque', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idCheque}">${item.numeroCheque}</option>`);
            });
        });
    }

    function recolectarDetalleChequeFactura() {
        const detalles = [];
        const filas = $('#detailsTableEgreso tbody tr');

        filas.each(function () {
            const idCheque = parseInt($(this).find('td:eq(0)').attr('data-id'), 10); 
            const idFactura = parseInt($(this).find('td:eq(1)').attr('data-id'), 10); 
            const monto = parseFloat($(this).find('td:eq(2)').text().trim());

            detalles.push({
                IdCheque: idCheque,
                IdFactura: idFactura,
                Monto: monto
            });
        });

        return detalles;
    }

    function configurarModalEgresos(idAsociacion) {
        $('#detailModalEgreso').off('show.bs.modal').on('show.bs.modal', function () {
            limpiarErrores(); 

            // Cargar acuerdos (sin cambios)
            cargarAcuerdos(idAsociacion);

            // Cargar cheques (sin cambios)
            cargarCheques(idAsociacion);

            // Cargar facturas evitando duplicadas
            cargarFacturas(idAsociacion);
            
        });
    }


});