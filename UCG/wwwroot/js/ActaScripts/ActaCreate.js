$(document).ready(function () {
    // === UTILIDADES ===

    let asistencias = JSON.parse(localStorage.getItem('asistencias')) || [];
    let acuerdos = JSON.parse(localStorage.getItem('acuerdos')) || [];
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
        asistencias.forEach(a => {
            $('#detailsTableAsistencia tbody').append(`
                <tr>
                    <td>${a.Fecha}</td>
                    <td>${a.IdAsociado}</td>
                    <td><button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button></td>
                </tr>`);
        });
    }

    function reconstruirAcuerdosUI() {
        $('#detailsTableAcuerdo tbody').empty();
        acuerdos.forEach((a, index) => {
            $('#detailsTableAcuerdo tbody').append(`
                <tr data-index="${index}" data-nombre="${a.Nombre}" data-descripcion="${a.Descripcion}" data-monto="${a.Monto}">
                    <td>${a.Nombre}</td>
                    <td>₡${parseFloat(a.Monto).toFixed(2)}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-warning btn-edit-acuerdo" data-index="${index}">Editar</button>
                        <button type="button" class="btn btn-danger btn-sm removeRow" data-index="${index}">Eliminar</button>
                    </td>
                </tr>`);
        });
    }

    function guardarEnLocalStorage() {
        localStorage.setItem('asistencias', JSON.stringify(asistencias));
        localStorage.setItem('acuerdos', JSON.stringify(acuerdos));
    }

    function limpiarLocalStorage() {
        localStorage.removeItem('asistencias');
        localStorage.removeItem('acuerdos');
    }

    // Cargar UI inicial
    reconstruirAsistenciasUI();
    reconstruirAcuerdosUI();

    // === EVENTO CAMBIO DE ASOCIACIÓN ===
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

    // Agregar asistencia
    $('#addDetailAsistenciaBtn').on('click', function () {
        limpiarErrores();
        const idAsociado = $('#modalIdAsociado').val();
        const fecha = new Date().toISOString();

        if (!idAsociado || idAsociado === "0") {
            $('#modalIdAsociado').addClass('is-invalid')
                .after('<div id="errorMensaje" class="text-danger mt-1">Debe seleccionar un asociado.</div>');
            return;
        }

        if (asistencias.some(a => a.IdAsociado == idAsociado)) {
            $('#modalIdAsociado').addClass('is-invalid')
                .after('<div id="errorMensaje" class="text-danger mt-1">Este asociado ya fue agregado.</div>');
            return;
        }

        asistencias.push({ Fecha: fecha, IdAsociado: parseInt(idAsociado) });
        guardarEnLocalStorage();
        reconstruirAsistenciasUI();
        $('#modalIdAsociado').val('0');
        $('#detailModalAsistencia').modal('hide');
    });

    // Eliminar fila
    $('#detailsTableAsistencia, #detailsTableAcuerdo').on('click', '.removeRow', function () {
        const fila = $(this).closest('tr');
        const tablaId = fila.closest('table').attr('id');
        if (tablaId === 'detailsTableAsistencia') {
            const idAsociado = fila.find('td:eq(1)').text();
            asistencias = asistencias.filter(a => a.IdAsociado != idAsociado);
        } else {
            const nombre = fila.data('nombre');
            acuerdos = acuerdos.filter(a => a.Nombre !== nombre);
        }
        guardarEnLocalStorage();
        fila.remove();
    });

    // === VALIDACIÓN FORMULARIO ===
    $('form').on('submit', function (e) {
        limpiarErrores();

        const $filasAsistencia = $('#detailsTableAsistencia tbody tr');
        if ($filasAsistencia.length === 0) {
            $('#detailsTableAsistencia').after('<div id="errorTablaAsistencia" class="text-danger mt-1">Debe agregar al menos una asistencia.</div>');
            e.preventDefault();
        } else {
            asistencias = []; // reiniciar
            $filasAsistencia.each(function () {
                const fecha = $(this).find('td:eq(0)').text();
                const idAsociado = $(this).find('td:eq(1)').text();
                asistencias.push({ Fecha: fecha, IdAsociado: parseInt(idAsociado) });
            });
            $('#ActaAsistenciaJason').val(JSON.stringify(asistencias));
        }

        const $filasAcuerdo = $('#detailsTableAcuerdo tbody tr');
        if ($filasAcuerdo.length === 0) {
            $('#detailsTableAcuerdo').after('<div id="errorTablaAcuerdo" class="text-danger mt-1">Debe agregar al menos un acuerdo.</div>');
            e.preventDefault();
        } else {
            acuerdos = []; // reiniciar
            $filasAcuerdo.each(function () {
                const nombre = $(this).data('td:eq(0)');
                const descripcion = $(this).data('td:eq(1)');
                const monto = $(this).data('td:eq(2)');
                acuerdos.push({ Nombre: nombre, Descripcion: descripcion, Monto: parseFloat(monto) });
            });
            $('#ActaAcuerdoJason').val(JSON.stringify(acuerdos));
        }
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
    // Agregar o editar acuerdo (persistencia incluida)
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
            $('#montoAcuerdo').addClass('is-invalid')
                .after('<div id="errorMontoAcuerdo" class="text-danger mt-1">Ingrese un monto válido (número positivo).</div>');
            hayError = true;
        }

        if (hayError) return;

        const nuevoAcuerdo = {
            Nombre: nombre,
            Descripcion: descripcionHtml,
            Monto: monto
        };

        if (filaAcuerdoEditando !== null) {
            const index = parseInt(filaAcuerdoEditando.attr('data-index'));
            acuerdos[index] = nuevoAcuerdo;
            filaAcuerdoEditando = null;
        } else {
            acuerdos.push(nuevoAcuerdo);
        }

        guardarEnLocalStorage();
        reconstruirAcuerdosUI();
        limpiarCamposModalAcuerdo();
        $('#detailModalAcuerdo').modal('hide');
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
    
});
