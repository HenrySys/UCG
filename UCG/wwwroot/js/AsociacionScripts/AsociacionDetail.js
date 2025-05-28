$(document).ready(function () {
    $('#tablaAsociados').DataTable({
        paging: false,         // Sin paginaci�n
        searching: false,       // Con barra de b�squeda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaAsociados tbody').on('click', 'tr', function () {
            var idAsoaciados = $(this).data("id"); // Obtener ID de la fila seleccionada
            console.log("ID Asociado:", idAsoaciados);

            if (idAsoaciados) {
                window.location.href = "/TbAsociadoes/Details/" + idAsoaciados;
            }
    });
    
    $('#btnAgregarAsociados').click(function () {
        var idAsociado = $(this).data("id"); // <-- aqu� s� se obtiene correctamente el ID del bot�n
        console.log("ID Asociado:", idAsociado);

        if (idAsociado) {
            window.location.href = "/TbAsociadoes/Create/" + idAsociado;
        }
    });
    
});
