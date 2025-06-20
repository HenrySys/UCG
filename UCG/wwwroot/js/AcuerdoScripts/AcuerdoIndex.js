﻿$(document).ready(function () {
    var tabla = $('#miTabla').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por página",
            "zeroRecords": "No se encontraron resultados",
            "info": "Mostrando página _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado de _MAX_ registros totales)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });
    // Asignar evento con delegación
    $('#miTabla tbody').on('click', 'tr', function () {
        var idAcuerdo = $(this).data("id"); 
        console.log("ID Acuerdo:", idAcuerdo);

        if (idAcuerdo) {
            window.location.href = "/TbAcuerdoes/Details/" + idAcuerdo;
        }
    });

    // SweetAlert desde TempData
    const swalContainer = document.getElementById('tempDataSwal');
    if (swalContainer) {
        const success = swalContainer.dataset.success?.trim();
        const error = swalContainer.dataset.error?.trim();

        if (success) {
            Swal.fire({
                icon: 'success',
                title: 'Exito',
                text: success,
                confirmButtonText: 'OK'
            });
        }

        if (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error,
                confirmButtonText: 'OK'
            });
        }
    }
});
