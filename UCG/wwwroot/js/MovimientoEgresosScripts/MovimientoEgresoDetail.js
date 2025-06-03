$(document).ready(function () {

    $('#tablaDetalleEgreso').DataTable({
        paging: false,
        searching: false,
        info: false,
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });
    $('#tablaDetalleEgreso tbody').on('click', 'tr', function () {
        var idDetalleChequeFactura = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Acta Asistencia:", idDetalleChequeFactura);

        if (idDetalleChequeFactura) {
            window.location.href = "/TbDetalleChequeFacturas/Details/" + idDetalleChequeFactura;
        }
    });

    $('#btnAgrgarDetalleChequeFactura').click(function () {
        var idDetalleChequeFactura = $(this).data("id"); // <-- aquí sí se obtiene correctamente el ID del botón
        console.log("ID Acta:", idDetalleChequeFactura);

        if (idDetalleChequeFactura) {
            window.location.href = "/TbDetalleChequeFacturas/Create/" + idDetalleChequeFactura;
        }
    });
});