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
        var idMovimiento = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Movimiento:", idMovimiento);

        if (idMovimiento) {
            window.location.href = "/TbMovimientoes/Details/" + idMovimiento;
        }
    });
});