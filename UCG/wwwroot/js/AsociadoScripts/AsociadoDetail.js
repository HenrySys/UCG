$(document).ready(function () {
    $('#tablaFacturas').DataTable({
        paging: false,         // Sin paginaci�n
        searching: false,       // Con barra de b�squeda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaFacturas tbody').on('click', 'tr', function () {
            var idFacturas = $(this).data("id"); // Obtener ID de la fila seleccionada
            console.log("ID Factura:", idFacturas);

            if (idFacturas) {
                window.location.href = "/TbFacturas/Details/" + idFacturas;
            }
    });
    
    // $('#btnAgregarFacturas').click(function () {
    //     var idFactura = $(this).data("id"); // <-- aqu� s� se obtiene correctamente el ID del bot�n
    //     console.log("ID Factura:", idFactura);

    //     if (idFactura) {
    //         window.location.href = "/TbFacturas/Create/" + idFactura;
    //     }
    // });
    
});
