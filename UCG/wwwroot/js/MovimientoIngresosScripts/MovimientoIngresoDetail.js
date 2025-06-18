$(document).ready(function () {
    $('#tablaDocumentos').DataTable({
        paging: false,         // Sin paginaci�n
        searching: false,       // Con barra de b�squeda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });
});
