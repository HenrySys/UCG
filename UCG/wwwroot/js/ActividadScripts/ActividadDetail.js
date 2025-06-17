$(document).ready(function () {
    $('#tablaFondos').DataTable({
        paging: false,         // Sin paginación
        searching: false,       // Con barra de búsqueda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaFondos tbody').on('click', 'tr', function () {
        var idActividad = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Factura:", idActividad);

        if (idActividad) {
            window.location.href = "/TbFondosR/Details/" + idActividad;
        }
    });

    $('#btnAgregarFacturas').click(function () {
        var idFactura = $(this).data("id"); // <-- aquí sí se obtiene correctamente el ID del botón
        console.log("ID Factura:", idFactura);

        if (idFactura) {
            window.location.href = "/TbFacturas/Create/" + idFactura;
        }
    });

});
