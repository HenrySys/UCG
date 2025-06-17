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
                window.location.href = '/TbFinancistums/Index';
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


    // Inicializa Summernote
    $('#summernoteAcuerdo').summernote({
        height: 125,
        placeholder: 'Escriba el acuerdo aqui...',
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['para', ['ul', 'ol']],
            ['view', ['codeview']]
        ],

    });

    $('form').on('submit', function () {
        const rawHtml = $('#summernoteAcuerdo').summernote('code');
        const cleanText = $('<div>').html(rawHtml).text().trim(); // Extrae solo el texto plano
        $('#summernoteAcuerdo').val(cleanText);
    });


    

});