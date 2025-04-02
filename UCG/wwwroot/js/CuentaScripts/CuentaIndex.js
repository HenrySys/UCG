$(document).ready(function () {
    $('#miTabla').DataTable({
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
    $("#miTabla tbody tr").click(function () {
        var idCuenta = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Acta:", idCuenta);
        // Redirigir a la página de detalles
        if (idCuenta) {
            window.location.href = "/TbCuentums/Details/" + idCuenta;
        }
    });
});
