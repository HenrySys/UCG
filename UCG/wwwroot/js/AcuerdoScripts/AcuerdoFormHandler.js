$(document).ready(function () {
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    const idActa = $('#tempDataSwal').data('acta');

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
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar'
        });
    }

    // Inicializa Summernote
    $('#summernoteAcuerdo').summernote({
        placeholder: 'Ingrese la descripción del acuerdo',
        tabsize: 2,
        height: 120
    });
});
