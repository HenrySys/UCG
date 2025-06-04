$(document).ready(function () {

    $('#tablaMiembros').DataTable({
        paging: false,
        searching: false,
        info: false,
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });
    $('#tablaMiembros tbody').on('click', 'tr', function () {
        var idJuntaDirectiva = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Acta Asistencia:", idJuntaDirectiva);

        if (idJuntaDirectiva) {
            window.location.href = "/TbMiembroJuntaDirectivas/Details/" + idJuntaDirectiva;
        }
    });

    $('#btnAgregarMiembros').click(function () {
        var idJuntaDirectiva = $(this).data("id"); // <-- aquí sí se obtiene correctamente el ID del botón
        console.log("ID Acta:", idJuntaDirectiva);

        if (idJuntaDirectiva) {
            window.location.href = "/TbMiembroJuntaDirectivas/Create/" + idJuntaDirectiva;
        }
    });
});