$(document).ready(function () {
    $("#movimientosTable tbody tr").click(function () {
        var idMovimiento = $(this).data("id"); // Obtener ID de la fila seleccionada
        console.log("ID Movimiento:", idMovimiento);
        // Redirigir a la página de detalles
        if (idMovimiento) {
            window.location.href = "/TbMovimientoes/Details/" + idMovimiento;
        }
    });

   
});