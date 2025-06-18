$(document).ready(function () {
    var tabla = $('#miTabla').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por pagina",
            "zeroRecords": "No se encontraron resultados",
            "info": "Mostrando pagina _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado de _MAX_ registros totales)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });
    $('#miTabla tbody').on('click', 'tr', function () {
        var idMovimientoIngreso = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID MovimientoIngreso:", idMovimientoIngreso);

        if (idMovimientoIngreso) {
            window.location.href = "/TbMovimientoIngresos/Details/" + idMovimientoIngreso;
        }
    });

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