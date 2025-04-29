$(document).ready(function () {
    $('#tablaAsistencias').DataTable({
        paging: false,         // Sin paginación
        searching: false,       // Con barra de búsqueda
        info: false,           // Sin "Mostrando X de Y"
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });

    $('#tablaAcuerdos').DataTable({
        paging: false,
        searching: false,
        info: false,
        language: {
            search: "Buscar:",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles"
        }
    });
    $('#tablaAsistencias tbody').on('click', 'tr', function () {
            var idActaAsistencia = $(this).data("id"); // Obtener ID de la fila seleccionada
            console.log("ID Acta Asistencia:", idActaAsistencia);

            if (idActaAsistencia) {
                window.location.href = "/TbActaAsistenciums/Details/" + idActaAsistencia;
            }
    });

    $('#tablaAcuerdos tbody').on('click', 'tr', function () {
            var idActaAcuerdo = $(this).data("id"); // Obtener ID de la fila seleccionada
            console.log("ID Acta Acuerdo:", idActaAcuerdo);

            if (idActaAcuerdo) {
                window.location.href = "/TbAcuerdoes/Details/" + idActaAcuerdo;
            }
    });


    $('#btnAgregarAsistencia').click(function () {
        var idActa = $(this).data("id"); // <-- aquí sí se obtiene correctamente el ID del botón
        console.log("ID Acta:", idActa);

        if (idActa) {
            window.location.href = "/TbActaAsistenciums/Create/" + idActa;
        }
    });
});
