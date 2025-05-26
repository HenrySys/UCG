$(document).ready(function () {
    var tabla = $('#miTabla').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por p�gina",
            "zeroRecords": "No se encontraron resultados",
            "info": "Mostrando p�gina _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado de _MAX_ registros totales)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "�ltimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });
    $('#miTabla tbody').on('click', 'tr', function () {
        var idCheque = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Cheque:", idCheque);

        if (idCheque) {
            window.location.href = "/TbCheques/Details/" + idCheque;
        }
    });

});