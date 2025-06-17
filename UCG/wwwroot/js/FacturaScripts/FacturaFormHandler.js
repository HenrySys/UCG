$(document).ready(function () {
    const modoVista = $('form').data('mode') || 'Create';
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const idAsociacion = $('#tempDataSwal').data('asociacion') || $('#IdAsociacion').val();
    const detallesFacturaCargados = JSON.parse($('#DetalleFacturaJason').val() || '[]');

    console.log("Detalles cargados desde el input oculto:", detallesFacturaCargados);
    


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
    function limpiarErrores() {
        $('.text-danger.mt-1').remove();
        $('.is-invalid').removeClass('is-invalid');
    }

    function limpiarCamposModalDetalleFactura() {
        $('#modalDescripcion').val('');
        $('#modalUnidad').val('');
        $('#modalCantidad').val('');
        $('#modalPrecioUnitario').val('');
        $('#modalPorcentajeIva').val('');
        $('#modalPorcentajeDescuento').val('');
        $('#modalDescuento').val('');
        $('#modalBaseImponible').val('');
        $('#modalMontoIva').val('');
        $('#modalTotalLinea').val('');
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


    function reconstruirDetalleFacturaUI() {
        $('#detailsTableFactura tbody').empty();

        detallesFacturaCargados.forEach((d, index) => {
            $('#detailsTableFactura tbody').append(`
            <tr data-index="${index}">
                <td>${d.Descripcion}</td>
                <td>${d.Unidad || ''}</td>
                <td>${d.Cantidad}</td>
                <td>${parseFloat(d.PrecioUnitario || 0).toFixed(2)}</td>
                <td>${parseFloat(d.PorcentajeIva || 0).toFixed(2)}</td>
                <td>${parseFloat(d.PorcentajeDescuento || 0).toFixed(2)}</td>
                <td>${parseFloat(d.Descuento || 0).toFixed(2)}</td>
                <td>${parseFloat(d.MontoIva || 0).toFixed(2)}</td>
                <td>${parseFloat(d.BaseImponible || 0).toFixed(2)}</td>
                <td class="col-total-linea" data-valor="${parseFloat(d.TotalLinea || 0)}">${parseFloat(d.TotalLinea || 0).toFixed(2)}</td>
                <td>
                            <button type="button" class="btn btn-warning btn-sm editar-detalle" data-index="${index}">Editar</button>

                    <button type="button" class="btn btn-danger btn-sm eliminar-detalle" data-index="${index}">Eliminar</button>
                </td>
            </tr>
        `);
            console.log("Fila reconstruida:", d);
        });

        actualizarMontoTotalFactura(); // Opcional: actualiza el total si lo necesitás al reconstruir
        
    }

   

    // Cálculo automático de campos del detalle
    function recalcularDetalleFactura() {
        const cantidad = parseFloat($('#modalCantidad').val()) || 0;
        const precioUnitario = parseFloat($('#modalPrecioUnitario').val()) || 0;
        const porcentajeDescuento = parseFloat($('#modalPorcentajeDescuento').val()) || 0;
        const porcentajeIva = parseFloat($('#modalPorcentajeIva').val()) || 0;

        const subtotal = cantidad * precioUnitario;

        const descuento = subtotal * (porcentajeDescuento / 100);
        const baseImponible = subtotal - descuento;
        const montoIva = baseImponible * (porcentajeIva / 100);
        const totalLinea = baseImponible + montoIva;

        $('#modalDescuento').val(descuento.toFixed(2));
        $('#modalBaseImponible').val(baseImponible.toFixed(2));
        $('#modalMontoIva').val(montoIva.toFixed(2));
        $('#modalTotalLinea').val(totalLinea.toFixed(2));
    }

    reconstruirDetalleFacturaUI();
    // Disparar el cálculo cuando cambian los campos relevantes
    $('#modalCantidad, #modalPrecioUnitario, #modalPorcentajeDescuento, #modalPorcentajeIva').on('input', recalcularDetalleFactura);

    function actualizarMontoTotalFactura() {
        let total = 0;

        $('#detailsTableFactura tbody tr').each(function () {
            const totalLinea = parseFloat($(this).find('.col-total-linea').data('valor')) || 0;
            total += totalLinea;
        });


        $('#MontoTotal').val(total.toFixed(2));
        
    }


    let filaDetalleEditando = null;

    function limpiarErroresDetalleFactura() {
        $('#detailModalDetalleFactura .is-invalid').removeClass('is-invalid');
        $('#detailModalDetalleFactura .text-danger').remove();
    }


    // Limpiar errores al abrir el modal
    $('#detailModalDetalleFactura').on('show.bs.modal', function () {
        limpiarErroresDetalleFactura();
    });


    $('#addDetalleBtn').on('click', function () {
        limpiarErroresDetalleFactura(); // Limpia errores previos

        let hayError = false;

        const descripcion = $('#modalDescripcion').val()?.trim() || '';
        const unidad = $('#modalUnidad').val()?.trim() || '';
        const cantidad = parseFloat($('#modalCantidad').val());
        const precioUnitario = parseFloat($('#modalPrecioUnitario').val());
        const porcentajeIva = parseFloat($('#modalPorcentajeIva').val());
        const porcentajeDescuento = parseFloat($('#modalPorcentajeDescuento').val());
        const descuento = parseFloat($('#modalDescuento').val()) || 0;
        const baseImponible = parseFloat($('#modalBaseImponible').val()) || 0;
        const montoIva = parseFloat($('#modalMontoIva').val()) || 0;
        const totalLinea = parseFloat($('#modalTotalLinea').val()) || 0;

        // === VALIDACIONES ===

        if (!descripcion) {
            $('#modalDescripcion').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe ingresar una descripción.</div>');
            hayError = true;
        }

        if (!unidad) {
            $('#modalUnidad').addClass('is-invalid')
                .after('<div class="text-danger mt-1">Debe ingresar la unidad.</div>');
            hayError = true;
        }

        if (isNaN(cantidad) || cantidad <= 0) {
            $('#modalCantidad').addClass('is-invalid')
                .after('<div class="text-danger mt-1">La cantidad debe ser mayor que cero.</div>');
            hayError = true;
        }

        if (isNaN(precioUnitario) || precioUnitario <= 0) {
            $('#modalPrecioUnitario').addClass('is-invalid')
                .after('<div class="text-danger mt-1">El precio unitario debe ser mayor que cero.</div>');
            hayError = true;
        }

        if (isNaN(porcentajeDescuento) || porcentajeDescuento < 0 || porcentajeDescuento > 100) {
            $('#modalPorcentajeDescuento').addClass('is-invalid')
                .after('<div class="text-danger mt-1">El porcentaje de descuento debe estar entre 0 y 100.</div>');
            hayError = true;
        }

        if (isNaN(porcentajeIva) || porcentajeIva < 0 || porcentajeIva > 100) {
            $('#modalPorcentajeIva').addClass('is-invalid')
                .after('<div class="text-danger mt-1">El porcentaje de IVA debe estar entre 0 y 100.</div>');
            hayError = true;
        }

        if (hayError) return;

        // === AGREGAR O ACTUALIZAR FILA ===

        const celdas = `
        <td>${descripcion}</td>
        <td>${unidad}</td>
        <td>${cantidad}</td>
        <td>${precioUnitario.toFixed(2)}</td>
        <td>${porcentajeIva.toFixed(2)}</td>
        <td>${porcentajeDescuento.toFixed(2)}</td>
        <td>${descuento.toFixed(2)}</td>
        <td>${montoIva.toFixed(2)}</td>
        <td>${baseImponible.toFixed(2)}</td>
        <td class="col-total-linea" data-valor="${totalLinea}">${totalLinea.toFixed(2)}</td>
        <td>
            <button type="button" class="btn btn-warning btn-sm editar-detalle">Editar</button>
            <button type="button" class="btn btn-danger btn-sm eliminar-detalle">Eliminar</button>
        </td>
    `;

        if (filaDetalleEditando !== null) {
            filaDetalleEditando.html(celdas);
            filaDetalleEditando = null;
        } else {
            $('#detailsTableFactura tbody').append(`<tr>${celdas}</tr>`);
        }

        $('#detailModalDetalleFactura').modal('hide');
        limpiarCamposModalDetalleFactura();
        actualizarMontoTotalFactura();
    });



    function recolectarDetallesFactura() {
        const detalles = [];
        const filas = $('#detailsTableFactura tbody tr');

        filas.each(function () {
            const fila = $(this);
            detalles.push({
                Descripcion: fila.find('td:eq(0)').text().trim(),
                Unidad: fila.find('td:eq(1)').text().trim(),
                Cantidad: parseFloat(fila.find('td:eq(2)').text()) || 0,
                PrecioUnitario: parseFloat(fila.find('td:eq(3)').text()) || 0,
                PorcentajeIva: parseFloat(fila.find('td:eq(4)').text()) || 0,
                PorcentajeDescuento: parseFloat(fila.find('td:eq(5)').text()) || 0,
                Descuento: parseFloat(fila.find('td:eq(6)').text()) || 0,
                MontoIva: parseFloat(fila.find('td:eq(7)').text()) || 0,
                BaseImponible: parseFloat(fila.find('td:eq(8)').text()) || 0,
                TotalLinea: parseFloat(fila.find('td:eq(9)').text()) || 0
            });
        });

        return detalles;
    }

    // Eliminar fila de detalle con confirmación
    $('#detailsTableFactura').on('click', '.eliminar-detalle', function () {
        const fila = $(this).closest('tr');
        Swal.fire({
            title: '¿Está seguro?',
            text: '¿Desea eliminar este detalle de factura?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fila.remove();
                actualizarMontoTotalFactura(); // Actualiza el total al eliminar
                Swal.fire('Eliminado', 'El detalle ha sido eliminado.', 'success');
            }
        });
    });

    $('form').on('submit', function (e) {
        limpiarErrores(); // si tenés validación previa
        const detalles = recolectarDetallesFactura();

        if (detalles.length === 0) {
            $('#detailsTableFactura').after('<div id="errorTablaFactura" class="text-danger mt-1">Debe agregar al menos un detalle.</div>');
            e.preventDefault();
        } else {
            $('#DetalleFacturaJason').val(JSON.stringify(detalles));
            const monto = $('#MontoTotal').val();
            if (monto && monto.includes('.')) {
                $('#MontoTotal').val(monto.replace('.', ','));
            }
        }
    });

    $('#detailsTableFactura').on('click', '.editar-detalle', function () {
        limpiarErroresDetalleFactura(); // Limpia errores previos

        const fila = $(this).closest('tr');
        filaDetalleEditando = fila; // Guarda la fila que se está editando

        // Extrae los datos de la fila
        const descripcion = fila.find('td:eq(0)').text().trim();
        const unidad = fila.find('td:eq(1)').text().trim();
        const cantidad = parseFloat(fila.find('td:eq(2)').text()) || 0;
        const precioUnitario = parseFloat(fila.find('td:eq(3)').text()) || 0;
        const porcentajeIva = parseFloat(fila.find('td:eq(4)').text()) || 0;
        const porcentajeDescuento = parseFloat(fila.find('td:eq(5)').text()) || 0;
        const descuento = parseFloat(fila.find('td:eq(6)').text()) || 0;
        const montoIva = parseFloat(fila.find('td:eq(7)').text()) || 0;
        const baseImponible = parseFloat(fila.find('td:eq(8)').text()) || 0;
        const totalLinea = parseFloat(fila.find('td:eq(9)').text()) || 0;

        // Carga los datos en los campos del modal
        $('#modalDescripcion').val(descripcion);
        $('#modalUnidad').val(unidad);
        $('#modalCantidad').val(cantidad);
        $('#modalPrecioUnitario').val(precioUnitario);
        $('#modalPorcentajeIva').val(porcentajeIva);
        $('#modalPorcentajeDescuento').val(porcentajeDescuento);
        $('#modalDescuento').val(descuento);
        $('#modalBaseImponible').val(baseImponible);
        $('#modalMontoIva').val(montoIva);
        $('#modalTotalLinea').val(totalLinea);

        // Abre el modal
        $('#detailModalDetalleFactura').modal('show');
    });




});
