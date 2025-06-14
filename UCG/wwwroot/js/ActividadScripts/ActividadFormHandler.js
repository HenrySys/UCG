$(document).ready(function () {

    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Exito!',
            text: successMessage,
            confirmButtonText: 'Ir al listado'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbActividads/Index';
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
            url: url,
            data: data,
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

    $('#IdAsociacion').change(function () {
        const idAsociacion = $(this).val();
        if (!idAsociacion) return;

        // Cargar asociados
        fetchDropdownData(
            rutasFolio.obtenerAsociados,
            { idAsociacion: idAsociacion },
            '#IdAsociado',
            'Seleccione un asociado',
            function (data, dropdown) {
                data.forEach(item => {
                    dropdown.append(`<option value="${item.idAsociado}">${item.nombre}</option>`);
                });
            }
        );

        // Cargar actas
        fetchDropdownData(
            rutasFolio.obtenerActas,
            { idAsociacion: idAsociacion },
            '#IdActa',
            'Seleccione un acta',
            function (data, dropdown) {
                data.forEach(item => {
                    dropdown.append(`<option value="${item.idActa}">${item.numeroActa}</option>`);
                });
            }
        );
    });

});
