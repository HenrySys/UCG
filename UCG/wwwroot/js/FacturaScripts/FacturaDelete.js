$(document).ready(function () {
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');

    // 🔹 Mostrar mensaje de éxito si existe
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: successMessage,
            confirmButtonText: 'Aceptar'
        });
    }
    // 🔹 Mostrar mensaje de error si existe
    else if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar'
        });
    }

    // 🔹 Confirmación antes de eliminar
    const deleteBtn = document.getElementById('btnConfirmDelete');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            Swal.fire({
                title: '¿Está seguro?',
                text: 'Esta acción eliminará permanentemente la Factura.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.closest('form').submit();
                }
            });
        });
    }
});
