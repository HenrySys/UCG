$(document).ready(function () {
    const successMessage = '@(TempData["SuccessMessage"] ?? "")';
    const idActa = '@(TempData["IdActa"] ?? "")';
    const errorMessage = '@(TempData["ErrorMessage"] ?? "")';

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
});