$(document).ready(function () {
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');

    // 🔹 Confirmación antes de eliminar
    const deleteBtn = document.getElementById('btnConfirmDelete');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            Swal.fire({
                title: '¿Está seguro?',
                text: 'Esta acción eliminará permanentemente el Folio.',
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

    // 🔹 Mensaje de éxito
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: successMessage,
            confirmButtonText: 'Aceptar'
        });
    }

    // 🔹 Mensaje de error
    if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar'
        });
    }
});
