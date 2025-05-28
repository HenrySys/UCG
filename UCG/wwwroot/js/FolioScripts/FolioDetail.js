$(document).ready(function () {
    $('#tablaActas').DataTable({
        paging: false,         // Sin paginación
        searching: false,       // Con barra de búsqueda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaActas tbody').on('click', 'tr', function () {
            var idActas = $(this).data("id"); // Obtener ID de la fila seleccionada
            console.log("ID Acta:", idActas);

            if (idActas) {
                window.location.href = "/TbActums/Details/" + idActas;
            }
    });
    
    $('#btnAgregarActas').click(function () {
        var idActa = $(this).data("id"); // <-- aquí sí se obtiene correctamente el ID del botón
        console.log("ID Acta:", idActa);

        if (idActa) {
            window.location.href = "/TbActums/Create/" + idActa;
        }
    });
    
});
