$(document).ready(function () {
    var tabla = $('#miTabla').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por página",
            "zeroRecords": "No se encontraron resultados",
            "info": "Mostrando página _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado de _MAX_ registros totales)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });
    $('#miTabla tbody').on('click', 'tr', function () {
        var idActa = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Acta:", idActa);

        if (idActa) {
            window.location.href = "/TbActums/Details/" + idActa;
        }
    });

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

    // SweetAlert desde TempData
    const swalContainer = document.getElementById('tempDataSwal');
    if (swalContainer) {
        const success = swalContainer.dataset.success?.trim();
        const error = swalContainer.dataset.error?.trim();

        if (success) {
            Swal.fire({
                icon: 'success',
                title: 'Exito',
                text: success,
                confirmButtonText: 'OK'
            });
        }

        if (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error,
                confirmButtonText: 'OK'
            });
        }
    }
});
