$(document).ready(function () {
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbConceptoMovimientoes/Index';
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

    function toggleTipoCampos() {
        const tipo = $('#TipoMovimiento').val();

        // Restaurar placeholders si no existen
        if ($("#TipoOrigenIngreso option[value='']").length === 0) {
            $('#TipoOrigenIngreso').prepend('<option disabled selected value="">Seleccione un tipo de origen</option>');
        }
        if ($("#TipoEmisorEgreso option[value='']").length === 0) {
            $('#TipoEmisorEgreso').prepend('<option disabled selected value="">Seleccione un tipo de emisor</option>');
        }

        // Mostrar/Ocultar y manejar requeridos
        if (tipo === "1") { // Ingreso
            $('#grupoIngreso, #labelGrupoIngreso').show();
            $('#grupoEgreso, #labelGrupoEgreso').hide();
            $('#TipoOrigenIngreso').attr('required', true);
            $('#TipoEmisorEgreso').removeAttr('required').val('');
        } else if (tipo === "2") { // Egreso
            $('#grupoIngreso, #labelGrupoIngreso').hide();
            $('#grupoEgreso, #labelGrupoEgreso').show();
            $('#TipoEmisorEgreso').attr('required', true);
            $('#TipoOrigenIngreso').removeAttr('required').val('');
        } else {
            $('#grupoIngreso, #labelGrupoIngreso').hide();
            $('#grupoEgreso, #labelGrupoEgreso').hide();
            $('#TipoOrigenIngreso, #TipoEmisorEgreso').removeAttr('required').val('');
        }
    }

    toggleTipoCampos();
    $('#TipoMovimiento').on('change', toggleTipoCampos);

    // Validación antes del submit
    $('form').on('submit', function (e) {
        let tipo = $('#TipoMovimiento').val();
        let error = false;

        // Limpiar errores previos
        $('#TipoOrigenIngreso').removeClass('is-invalid');
        $('#TipoEmisorEgreso').removeClass('is-invalid');
        $('.dynamic-error').remove();

        if (tipo === "1") { // Ingreso
            if (!$('#TipoOrigenIngreso').val()) {
                $('#TipoOrigenIngreso').addClass('is-invalid')
                error = true;
            }
        }

        if (tipo === "2") { // Egreso
            if (!$('#TipoEmisorEgreso').val()) {
                $('#TipoEmisorEgreso').addClass('is-invalid')
                error = true;
            }
        }

        if (error) e.preventDefault();
    });
});
