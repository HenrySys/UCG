$(document).ready(function () {
    const successMessage = $('#tempDataSwal').data('success');
    const errorMessage = $('#tempDataSwal').data('error');
    // ?? Confirmaci�n antes de eliminar
    const deleteBtn = document.getElementById('btnConfirmDelete');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            Swal.fire({
                title: '�Est� seguro?',
                text: 'Esta acci�n eliminar� permanentemente el Egreso.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'S�, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.closest('form').submit();
                }
            });
        });
    }else if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar'
        });
    }
});