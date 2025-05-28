$(document).ready(function () {
    const modoVista = $('form').data('mode') || 'Create';

    const asistenciasCargadas = JSON.parse($('#ActaAsistenciaJason').val() || '[]');
    const acuerdosCargados = JSON.parse($('#ActaAcuerdoJason').val() || '[]');

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
        $('#tipoAcuerdo').val('').prop('selectedIndex', 0); // <-- esto reinicia el select
        $('#summernoteAcuerdo').summernote('destroy');
        $('#summernoteAcuerdo').val('');
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
        fetchDropdownData(rutasMovimiento.obtenerAsociados, { idAsociacion }, '#IdAsociado', 'Seleccione un asociado', function (data, dropdown) {
            data.forEach(item => {
                const nombreCompleto = `${item.nombre} ${item.apellido1}`;
                dropdown.append(`<option value="${item.idAsociado}">${nombreCompleto}</option>`);
            });
        });
    }

    function cargarFolios(idAsociacion) {
        fetchDropdownData(rutasMovimiento.obtenerFolios, { idAsociacion }, '#IdFolio', 'Seleccione un folio', function (data, dropdown) {
            data.forEach(item => {
                dropdown.append(`<option value="${item.idFolio}">${item.numeroFolio}</option>`);
            });
        });
    }

    function configurarModalAsistencia(idAsociacion) {
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
    }
    function reconstruirAsistenciasUI() {
        $('#detailsTableAsistencia tbody').empty();
        asistenciasCargadas.forEach((a, index) => {
            const nombreCompleto = `${a.Nombre} ${a.Apellido1 || ''}`.trim();
            const botones = (modoVista === 'Create') ? `
            <td>
              <button type="button" class="btn btn-danger btn-sm removeRow" data-index="${index}">Eliminar</button>
            </td>
        ` : '';

            $('#detailsTableAsistencia tbody').append(`
            <tr>
                <td>${a.Fecha}</td>
                <td>${nombreCompleto}</td>
                ${botones}
            </tr>`
            );
        });
    }


    function reconstruirAcuerdosUI() {
        $('#detailsTableAcuerdo tbody').empty();

        acuerdosCargados.forEach((a, index) => {

            console.log(`Acuerdo ${index}:`, a);
            $('#detailsTableAcuerdo tbody').append(`
            <tr data-index="${index}" data-nombre="${a.Nombre}" data-descripcion="${a.Descripcion}">
                <td>${a.Nombre}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-edit-acuerdo" data-index="${index}">Editar</button>
                    <button type="button" class="btn btn-danger btn-sm removeRow" data-index="${index}">Eliminar</button>
                </td>
            </tr>`);
        });
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

            const nombre = fila.find('td:eq(1)').text().trim();
            const descripcion = fila.data('descripcion');
            const tipo = fila.data('tipo');
            acuerdos.push({
                Nombre: nombre,
                Descripcion: descripcion,
                Tipo: tipo
            });
        });

        return acuerdos;
    }

    // Actualizar el número de acta al cambiar el folio o la fecha de sesión
    function actualizarNumeroActa() {
        var folioId = $('#IdFolio').val();
        var fechaSesion = $('#FechaSesionTexto').val();

        if (folioId && fechaSesion) {
            $.get('/TbActums/ObtenerNumeroActa', { idFolio: folioId, fecha: fechaSesion }, function (data) {
                if (data.numeroActa) {
                    $('#NumeroActa').val(data.numeroActa);
                }
            });
        }
    }

    if (modoVista === 'Create' && idAsociacion && parseInt(idAsociacion) > 0) {
        cargarAsociados(idAsociacion);
        configurarModalAsistencia(idAsociacion);
        cargarFolios(idAsociacion);
    }

    // Actualizar el número de acta al cargar la página
    $('#IdFolio, #FechaSesionTexto').change(actualizarNumeroActa);

    $('#IdAsociacion').change(function () {
        const nuevaAsociacion = $(this).val();
        if (!nuevaAsociacion) return;

        $('#detailsTableAsistencia tbody').empty();
        cargarAsociados(nuevaAsociacion);
        configurarModalAsistencia(nuevaAsociacion);
        cargarFolios(nuevaAsociacion);
    });

    $('#detailsTableAsistencia, #detailsTableAcuerdo').on('click', '.removeRow', function () {
        $(this).closest('tr').remove();
        actualizarMontoTotalAcordado();
    });

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

    $('#addDetailAcuerdoBtn').on('click', function () {
        limpiarErrores();

        const nombre = $('#nombreAcuerdo').val().trim();
        const tipo = $('#tipoAcuerdo').val();
        const tipoTexto = $('#tipoAcuerdo option:selected').text();
        const descripcionHtml = $('#summernoteAcuerdo').summernote('code');
        const descripcionTextoPlano = $('<div>').html($('#summernoteAcuerdo').summernote('code')).text().trim();

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

        if (!tipo) {
            $('#tipoAcuerdo').addClass('is-invalid')
                .after('<div id="errorTipoAcuerdo" class="text-danger mt-1">Debe seleccionar un tipo de acuerdo.</div>');
            hayError = true;
        }

        if (descripcionTextoPlano.length === 0) {
            $('#descripcionAcuerdo').addClass('is-invalid')
                .after('<div id="errorDescripcionAcuerdo" class="text-danger mt-1">La descripción no puede estar vacía.</div>');
            hayError = true;
        } else if (descripcionTextoPlano.length > 100) {
            $('#descripcionAcuerdo').addClass('is-invalid')
                .after('<div id="errorDescripcionAcuerdo" class="text-danger mt-1">La descripción no puede superar los 100 caracteres.</div>');
            hayError = true;
        }

        if (hayError) return;

        if (filaAcuerdoEditando !== null) {
            filaAcuerdoEditando.attr('data-nombre', nombre);
            filaAcuerdoEditando.attr('data-descripcion', descripcionTextoPlano);
            filaAcuerdoEditando.attr('data-tipo', tipo);
            filaAcuerdoEditando.find('td:eq(0)').text(nombre);
            filaAcuerdoEditando = null;
        } else {
            $('#detailsTableAcuerdo tbody').append(`
            <tr data-nombre="${nombre}" data-descripcion="${descripcionTextoPlano}" data-tipo="${tipo}">
                <td>
                    <span class="badge bg-info text-dark">${tipoTexto}</span>
                </td>
                <td>${nombre}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-warning btn-edit-acuerdo">Editar</button>
                    <button type="button" class="btn btn-danger btn-sm removeRow">Eliminar</button>
                </td>
            </tr>`);
        }

        limpiarCamposModalAcuerdo();
        $('#detailModalAcuerdo').modal('hide');
    });



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

    $('#detailsTableAcuerdo').on('click', '.btn-edit-acuerdo', function () {
        limpiarErrores();
        filaAcuerdoEditando = $(this).closest('tr');

        const nombre = filaAcuerdoEditando.data('nombre');
        const descripcion = filaAcuerdoEditando.data('descripcion');
        const tipo = filaAcuerdoEditando.data('tipo'); // <-- Añadido

        $('#nombreAcuerdo').val(nombre);
        $('#tipoAcuerdo').val(tipo); // <-- Añadido
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

    
        
    
});
