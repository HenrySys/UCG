$(document).ready(function () {
    const modoVista = $('form').data('mode'); // Se puede usar en cualquier función después

    const asistenciasCargadas = JSON.parse($('#ActaAsistenciaJason').val() || '[]');
    const acuerdosCargados = JSON.parse($('#ActaAcuerdoJason').val() || '[]');

    const idAsociacion = $('#tempDataSwal').data('asociacion');
    

    $('#FechaSesionTexto').change(function () {
        var fechaSession = $('#FechaSesionTexto').val();
        console.log('Fecha de la sesión:', fechaSession); 
    });


    const successMessage = $('#TempDataSuccessMessage').val();
    const errorMessage = $('#TempDataErrorMessage').val();

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbActums/Index';
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

    if (idAsociacion) {
        fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
            data.forEach(item => {
                const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
            });
        });
        $('#detailModalAsistencia').off('show.bs.modal').on('show.bs.modal', function () {
            limpiarErrores();
            const asociadosYaAgregados = $('#detailsTableAsistencia tbody tr').map(function () {
                return $(this).find('td:eq(1)').text();
            }).get();

            fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                let agregados = 0;
                data.forEach(item => {
                    if (!asociadosYaAgregados.includes(item.idAsociado.toString())) {
                        const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                        dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                    } else agregados++;
                });
                if (agregados === data.length) {
                    cerrarModalConMensaje('#detailModalAsistencia', {
                        titulo: 'Todos agregados',
                        texto: 'Todos los asociados ya han sido agregados a la lista.'
                    });
                }
            });
        });
    } else {
        $('#IdAsociacion').change(function () {
            const idAsociacion = $(this).val();
            $('#detailsTableAsistencia tbody').empty();

            if (!idAsociacion) return;

            fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                data.forEach(item => {
                    const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                    dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                });
            });

            $('#detailModalAsistencia').off('show.bs.modal').on('show.bs.modal', function () {
                limpiarErrores();
                const asociadosYaAgregados = $('#detailsTableAsistencia tbody tr').map(function () {
                    return $(this).find('td:eq(1)').text();
                }).get();

                fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion }, '#modalIdAsociado', 'Seleccione un asociado', function (data, dropdown) {
                    let agregados = 0;
                    data.forEach(item => {
                        if (!asociadosYaAgregados.includes(item.idAsociado.toString())) {
                            const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                            dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
                        } else agregados++;
                    });
                    if (agregados === data.length) {
                        cerrarModalConMensaje('#detailModalAsistencia', {
                            titulo: 'Todos agregados',
                            texto: 'Todos los asociados ya han sido agregados a la lista.'
                        });
                    }
                });
            });
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

    function limpiarCamposModalAcuerdo() {
        $('#nombreAcuerdo').val('');
        $('#montoAcuerdo').val('');
        $('#summernoteAcuerdo').summernote('destroy');
    }

    function reconstruirAsistenciasUI() {
        $('#detailsTableAsistencia tbody').empty();
        asistenciasCargadas.forEach(a => {
            const botones = (modoVista === 'Create') ? `
                <td>
                  <button type="button" class="btn btn-danger btn-sm removeRow" data-index="${index}">Eliminar</button>
                </td>
            ` : '';
            $('#detailsTableAsistencia tbody').append(`
                <tr>
                    <td>${a.Fecha}</td>
                    <td>${a.IdAsociado}</td>
                    ${botones}
                </tr>`);
        });
    }

    function reconstruirAcuerdosUI() {
        $('#detailsTableAcuerdo tbody').empty();

        acuerdosCargados.forEach((a, index) => {
            
                console.log(`Acuerdo ${index}:`, a);
            $('#detailsTableAcuerdo tbody').append(`
            <tr data-index="${index}" data-nombre="${a.Nombre}" data-descripcion="${a.Descripcion}" data-monto="${a.MontoAcuerdo}">
                <td>${a.Nombre}</td>
                <td>₡${parseFloat(a.MontoAcuerdo).toFixed(2)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-edit-acuerdo" data-index="${index}">Editar</button>
                    <button type="button" class="btn btn-danger btn-sm removeRow" data-index="${index}">Eliminar</button>
                </td>
            </tr>`);
        });
        actualizarMontoTotalAcordado();
    }


    reconstruirAsistenciasUI();
    reconstruirAcuerdosUI();

    function recolectarAsistencias() {
        const asistencias = [];
        const filas = $('#detailsTableAsistencia tbody tr');

        filas.each(function () {
            const fecha = $(this).find('td:eq(0)').text().trim();
            const idAsociado = parseInt($(this).find('td:eq(1)').text().trim());

            asistencias.push({
                Fecha: fecha,
                IdAsociado: idAsociado
            });
        });

        return asistencias;
    }

    function recolectarAcuerdos() {
        const acuerdos = [];
        const filas = $('#detailsTableAcuerdo tbody tr');

        filas.each(function () {
            const fila = $(this);
            const nombre = fila.find('td:eq(0)').text().trim();
            const montoTexto = fila.find('td:eq(1)').text().replace(/[₡,]/g, '').trim();
            const descripcion = fila.data('descripcion');

            acuerdos.push({
                Nombre: nombre,
                Descripcion: descripcion,
                MontoAcuerdo: parseFloat(montoTexto) || 0
            });
        });

        return acuerdos;
    }

    function actualizarMontoTotalAcordado() {
        let total = 0;

        $('#detailsTableAcuerdo tbody tr').each(function () {
            const montoTexto = $(this).find('td:eq(1)').text().replace(/[₡,]/g, '').trim();
            const monto = parseFloat(montoTexto);
            if (!isNaN(monto)) total += monto;
        });

        $('#montoTotalAcordado').val(total.toFixed(2));
    }

    $('form').on('submit', function (e) {
        limpiarErrores();

        const asistencias = recolectarAsistencias();
        const acuerdos = recolectarAcuerdos();

        if (asistencias.length === 0) {
            $('#detailsTableAsistencia').after('<div id="errorTablaAsistencia" class="text-danger mt-1">Debe agregar al menos una asistencia.</div>');
            e.preventDefault();
        } else {
            $('#ActaAsistenciaJason').val(JSON.stringify(asistencias));
        }

        if (acuerdos.length === 0) {
            $('#detailsTableAcuerdo').after('<div id="errorTablaAcuerdo" class="text-danger mt-1">Debe agregar al menos un acuerdo.</div>');
            e.preventDefault();
        } else {
            $('#ActaAcuerdoJason').val(JSON.stringify(acuerdos));
        }
    });


    // Agregar asistencia
    $('#addDetailAsistenciaBtn').on('click', function () {
        limpiarErrores();
        const idAsociado = $('#modalIdAsociado').val();
        const fecha = new Date().toISOString().split('T')[0]; 

        if (!idAsociado || idAsociado === "0") {
            $('#modalIdAsociado').addClass('is-invalid')
                .after('<div id="errorMensaje" class="text-danger mt-1">Debe seleccionar un asociado.</div>');
            return;
        }

        const existe = $('#detailsTableAsistencia tbody tr').toArray().some(tr => $(tr).find('td:eq(1)').text() === idAsociado);
        if (existe) {
            $('#modalIdAsociado').addClass('is-invalid')
                .after('<div id="errorMensaje" class="text-danger mt-1">Este asociado ya fue agregado.</div>');
            return;
        }

        $('#detailsTableAsistencia tbody').append(`
            <tr>
                <td>${fecha}</td>
                <td>${idAsociado}</td>
                <td><button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button></td>
            </tr>`);

        $('#modalIdAsociado').val('0');
        $('#detailModalAsistencia').modal('hide');
    });

    // Eliminar fila
    $('#detailsTableAsistencia, #detailsTableAcuerdo').on('click', '.removeRow', function () {
        $(this).closest('tr').remove();
        actualizarMontoTotalAcordado();
    });

    // === ACUERDO: SUMMERNOTE ===
    $('#detailModalAcuerdo').on('shown.bs.modal', function () {
        limpiarErrores();
        $('#summernoteAcuerdo').summernote({
            height: 125,
            placeholder: 'Escriba el acuerdo aquí...',
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['view', ['codeview']]
            ]
        });
    }).on('hidden.bs.modal', function () {
        $('#summernoteAcuerdo').summernote('destroy');
        limpiarErrores();
        limpiarCamposModalAcuerdo();
        filaAcuerdoEditando = null;
    });

    let filaAcuerdoEditando = null;

    $('#addDetailAcuerdoBtn').on('click', function () {
        limpiarErrores();

        const nombre = $('#nombreAcuerdo').val().trim();
        const descripcionHtml = $('#summernoteAcuerdo').summernote('code');
        const descripcionTexto = $('<div>').html(descripcionHtml).text().trim();
        const monto = $('#montoAcuerdo').val().trim();
        const montoNumerico = parseFloat(monto);

        let hayError = false;

        if (!nombre) {
            $('#nombreAcuerdo').addClass('is-invalid')
                .after('<div id="errorNombreAcuerdo" class="text-danger mt-1">Debe ingresar el nombre del acuerdo.</div>');
            hayError = true;
        } else if (!/^[A-Z][a-zA-Z\s]{4,}$/.test(nombre)) {
            $('#nombreAcuerdo').addClass('is-invalid')
                .after('<div id="errorNombreAcuerdo" class="text-danger mt-1">El nombre debe iniciar con mayúscula y tener al menos 5 letras.</div>');
            hayError = true;
        }

        if (descripcionTexto.length === 0) {
            $('#descripcionAcuerdo').addClass('is-invalid')
                .after('<div id="errorDescripcionAcuerdo" class="text-danger mt-1">La descripción no puede estar vacía.</div>');
            hayError = true;
        } else if (descripcionTexto.length > 100) {
            $('#descripcionAcuerdo').addClass('is-invalid')
                .after('<div id="errorDescripcionAcuerdo" class="text-danger mt-1">La descripción no puede superar los 100 caracteres.</div>');
            hayError = true;
        }

        if (!monto || isNaN(montoNumerico) || montoNumerico < 0) {
            $('#montoe').addClass('is-invalid')
                .after('<div id="errorMontoAcuerdo" class="text-danger mt-1">Ingrese un monto válido (número positivo).</div>');
            hayError = true;
        }



        if (hayError) return;


        if (filaAcuerdoEditando !== null) {
            filaAcuerdoEditando.attr('data-nombre', nombre);
            filaAcuerdoEditando.attr('data-descripcion', descripcionHtml);
            filaAcuerdoEditando.attr('data-monto', monto);
            filaAcuerdoEditando.find('td:eq(0)').text(nombre);
            filaAcuerdoEditando.find('td:eq(1)').text(`₡${parseFloat(monto).toFixed(2)}`);
            filaAcuerdoEditando = null;
        } else {
            $('#detailsTableAcuerdo tbody').append(`
              <tr data-nombre="${nombre}" data-descripcion="${descripcionHtml}" data-monto="${monto}">
                <td>${nombre}</td>
                <td>₡${parseFloat(monto).toFixed(2)}</td>
                <td>
                  <button type="button" class="btn btn-sm btn-warning btn-edit-acuerdo">Editar</button>
                  <button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button>
                </td>
              </tr>`);
        }

        limpiarCamposModalAcuerdo();
        $('#detailModalAcuerdo').modal('hide');

        actualizarMontoTotalAcordado();
    });

    $('#detailsTableAcuerdo').on('click', '.btn-edit-acuerdo', function () {
        limpiarErrores();
        filaAcuerdoEditando = $(this).closest('tr');

        const nombre = filaAcuerdoEditando.data('nombre');
        const descripcion = filaAcuerdoEditando.data('descripcion');
        const monto = filaAcuerdoEditando.data('monto');

        $('#nombreAcuerdo').val(nombre);
        $('#montoAcuerdo').val(monto);
        $('#summernoteAcuerdo').summernote({
            height: 125,
            placeholder: 'Escriba el acuerdo aquí...',
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['view', ['codeview']]
            ]
        });
        $('#summernoteAcuerdo').summernote('code', descripcion);
        $('#detailModalAcuerdo').modal('show');
    });


    const container = document.getElementById("tempDataSwal");
    const success = container?.dataset.success;
    const error = container?.dataset.error;

    if (success && success !== "") {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: success,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbActums/Index';
            }
        });
    } else if (error && error !== "") {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: error,
            confirmButtonText: 'Aceptar'
        });
    }
});
