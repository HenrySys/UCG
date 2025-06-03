$(document).ready(function () {
    const tempData = $('#tempDataSwal');
    const successMessage = tempData.data('success');
    const errorMessage = tempData.data('error');
    const actaId = tempData.data('acta');

    const selectActa = $('#IdActa');
    const selectAsociado = $('#IdAsociado');

    let swalMostrado = false;

    if (successMessage) {
        swalMostrado = true; 
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: successMessage,
            timer: 2500,
            showConfirmButton: false
        });

        if (actaId) {
            setTimeout(() => {
                window.location.href = `/TbActums/Details/${actaId}`;
            }, 2500);
        }
    } else if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage
        });
    }

    function mostrarErrorSwal(titulo, mensaje) {
        Swal.fire({
            icon: 'warning',
            title: titulo,
            text: mensaje
        });
    }

    function cargarAsociados(idActa, idAsociacion) {
        $.ajax({
            url: '/TbActaAsistenciums/ObtenerAsociadosPorAsociacion',
            method: 'GET',
            data: { idAsociacion, idActa },
            success: function (res) {
                selectAsociado.empty();

                if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                    selectAsociado.append('<option disabled selected>Seleccione un asociado</option>');
                    res.data.forEach(a => {
                        selectAsociado.append(`<option value="${a.idAsociado}">${a.nombre} ${a.apellido1}</option>`);
                    });
                } else {
                    selectAsociado.append('<option disabled selected>No hay asociados disponibles</option>');

                    // ✅ Si ya se mostró un Swal (por TempData), no repetir
                    if (!swalMostrado) {
                        Swal.fire({
                            icon: 'info',
                            title: 'Todos registrados',
                            text: res.message || 'Todos los asociados ya han sido registrados.',
                            timer: 2000,
                            showConfirmButton: false
                        });

                        if (actaId) {
                            setTimeout(() => {
                                window.location.href = `/TbActums/Details/${actaId}`;
                            }, 2000);
                        }
                    }
                }
            },
            error: function () {
                selectAsociado.empty();
                selectAsociado.append('<option disabled selected>Error al cargar asociados</option>');
                mostrarErrorSwal('Error', 'No se pudieron cargar los asociados.');
            }
        });
    }

    selectActa.on('change', function () {
        const idActa = $(this).val();
        if (!idActa) return;

        $.ajax({
            url: '/TbActaAsistenciums/ObtenerAsociacionPorActa',
            method: 'GET',
            data: { idActa },
            success: function (res) {
                if (res.success) {
                    cargarAsociados(idActa, res.idAsociacion);
                } else {
                    mostrarErrorSwal('Error', res.message);
                }
            },
            error: function () {
                mostrarErrorSwal('Error', 'Error al obtener la asociación.');
            }
        });
    });

    if (actaId) {
        selectActa.val(actaId).trigger('change');
    }
});
