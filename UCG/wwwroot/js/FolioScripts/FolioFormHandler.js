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
                window.location.href = '/TbFolios/Index';
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
                dropdown.append(`<option value="0" disabled selected>${placeholder}</option>`);
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

    $('#IdAsociacion').change(function () {
        const IdAsociacion = $(this).val();

        if (!IdAsociacion) return;

        fetchDropdownData(
            rutasFolio.obtenerAsociados,
            { idAsociacion: IdAsociacion },
            '#IdAsociado',
            'Seleccione un asociado',
            function (data, dropdown) {
                if (!data || data.length === 0) {
                    mostrarErrorSwal('Sin asociados', 'No hay asociados disponibles para esta asociación.');
                } else {
                    data.forEach(item => {
                        dropdown.append(`<option value="${item.idAsociado}">${item.nombre}</option>`);
                    });
                }
            }
        );
    });

    $('#summernoteObservacion').summernote({
        height: 125,
        placeholder: 'Escriba el acuerdo aqui...',
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['para', ['ul', 'ol']],
            ['view']
        ],

    });

    $('form').on('submit', function () {
        const rawHtml = $('#summernoteObservacion').summernote('code');
        const cleanText = $('<div>').html(rawHtml).text().trim(); // Extrae solo el texto plano
        $('#summernoteObservacion').val(cleanText);
    });

});