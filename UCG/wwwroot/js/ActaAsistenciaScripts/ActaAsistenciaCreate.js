$(document).ready(function () {
    const container = $('#tempDataSwal');
    const successMessage = container.data('success');
    const errorMessage = container.data('error');
    const idAsociacion = container.data('asociacion');
    const idActa = container.data('acta');

    function fetchDropdownData(url, data, dropdownSelector, placeholder, callback) {
        $.ajax({
            url,
            data,
            success: function (response) {
                const dropdown = $(dropdownSelector).empty();
                dropdown.append(`<option disabled selected>${placeholder}</option>`);

                if (response.success) {
                    if (response.data.length === 0) {
                        dropdown.append(`<option disabled selected>No hay asociados disponibles</option>`);
                        $('#btnSubmit').prop('disabled', true);
                        return;
                    }

                    callback(response.data, dropdown);
                    $('#btnSubmit').prop('disabled', false);
                } else {
                    dropdown.append(`<option disabled selected>No hay asociados disponibles</option>`);

                    $('#btnSubmit').prop('disabled', true);
                }
            },
            error: function (_, __, error) {
                mostrarErrorSwal('Error', 'Error al cargar los datos: ' + error);
                $('#btnSubmit').prop('disabled', true);
            }
        });
    }

    function mostrarErrorSwal(titulo, mensaje) {
        Swal.fire({
            icon: 'error',
            title: titulo,
            text: mensaje,
            confirmButtonText: 'Aceptar'
        });
    }

    // Si viene desde el detalle del acta (con ID en la ruta)
    if (idAsociacion && idActa) {
        fetchDropdownData(
            '/TbActaAsistenciums/ObtenerAsociadosPorAsociacion',
            { idAsociacion, idActa },
            '#IdAsociado',
            'Seleccione un asociado',
            function (data, dropdown) {
                data.forEach(item => {
                    dropdown.append(`<option value="${item.idAsociado}">${item.nombre} ${item.apellido1}</option>`);
                });
            }
        );
    }

    // Si el acta se elige manualmente en el dropdown
    $('#IdActa').change(function () {
        const idActa = $(this).val();
        console.log('Acta seleccionada:', idActa);

        if (!idActa) return;

        $.ajax({
            url: '/TbActaAsistenciums/ObtenerAsociacionPorActa',
            data: { idActa },
            success: function (response) {
                if (response.success) {
                    const idAsociacion = response.idAsociacion;

                    fetchDropdownData(
                        '/TbActaAsistenciums/ObtenerAsociadosPorAsociacion',
                        { idAsociacion, idActa },
                        '#IdAsociado',
                        'Seleccione un asociado',
                        function (data, dropdown) {
                            data.forEach(item => {
                                dropdown.append(`<option value="${item.idAsociado}">${item.nombre} ${item.apellido1}</option>`);
                            });
                        }
                    );
                } else {
                    mostrarErrorSwal('Error', response.message);
                    $('#btnSubmit').prop('disabled', true);
                }
            },
            error: function () {
                mostrarErrorSwal('Error', 'Error al obtener la asociación.');
                $('#btnSubmit').prop('disabled', true);
            }
        });
    });

    $('#IdAsociado').change(function () {
        const idAsociado = $(this).val();
        console.log('Asociado seleccionado:', idAsociado);
    });

    if (successMessage && idActa) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Ir al Acta'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/TbActums/Details/' + idActa;
            }
        });
    }

    if (errorMessage) {
        mostrarErrorSwal('Error', errorMessage);
    }

    
});



