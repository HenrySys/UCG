$(document).ready(function () {
    $('#tablaDocumentos').DataTable({
        paging: false,         // Sin paginación
        searching: false,       // Con barra de búsqueda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaDocumentos tbody').on('click', 'tr', function () {
        var idDoc = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Factura:", idDoc);

        if (idDoc) {
            window.location.href = "/TbDocumentoIngresoes/Details/" + idDoc;
        }
    });

   

});
