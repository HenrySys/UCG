$(document).ready(function () {
    const tempData = $('#tempDataSwal');
    const successMessage = tempData.data('success');
    const errorMessage = tempData.data('error');
    const juntaId = tempData.data('junta');

    const selectJunta = $('#IdJuntaDirectiva');
    const selectAsociado = $('#IdAsociado');
    const selectPuesto = $('#IdPuesto');

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

        if (juntaId) {
            setTimeout(() => {
                window.location.href = `/TbJuntaDirectivas/Details/${juntaId}`;
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

    function cargarAsociados(juntaId) {

        $.ajax({
            url: '/TbMiembroJuntaDirectivas/ObtenerAsociadosDisponiblesPorJunta',
            method: 'GET',
            data: { idJunta: juntaId },
            success: function (res) {
                selectAsociado.empty();

                if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                    selectAsociado.append('<option disabled selected>Seleccione un asociado</option>');
                    res.data.forEach(a => {
                        const nombreCompleto = `${a.nombre} ${a.apellido1}`;
                        selectAsociado.append(`<option value="${a.idAsociado}">${nombreCompleto}</option>`);
                    });
                } else {
                    selectAsociado.append('<option disabled selected>No hay asociados disponibles</option>');

                    if (!swalMostrado) {
                        Swal.fire({
                            icon: 'info',
                            title: 'Todos registrados',
                            text: res.message || 'Todos los asociados ya han sido registrados.',
                            timer: 2000,
                            showConfirmButton: false
                        });

                        if (juntaId) {
                            setTimeout(() => {
                                window.location.href = `/TbJuntaDirectivas/Details/${juntaId}`;
                            }, 2000);
                        }
                    }
                }
            },
            error: function () {
                selectAsociado.empty().append('<option disabled selected>Error al cargar asociados</option>');
                mostrarErrorSwal('Error', 'No se pudieron cargar los asociados.');
            }
        });
    }

    function cargarPuestos(juntaId) {
        $.ajax({
            url: '/TbMiembroJuntaDirectivas/ObtenerPuestosPorJunta',
            method: 'GET',
            data: { idJunta: juntaId },
            success: function (res) {
                selectPuesto.empty();

                if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                    selectPuesto.append('<option disabled selected>Seleccione un puesto</option>');
                    res.data.forEach(p => {
                        selectPuesto.append(`<option value="${p.idPuesto}">${p.nombre}</option>`);
                    });
                } else {
                    selectPuesto.append('<option disabled selected>No hay puestos disponibles</option>');

                    if (!swalMostrado) {
                        Swal.fire({
                            icon: 'info',
                            title: 'Puestos ocupados',
                            text: res.message || 'Todos los puestos ya han sido asignados en esta junta.',
                            timer: 2000,
                            showConfirmButton: false
                        });

                        if (juntaId) {
                            setTimeout(() => {
                                window.location.href = `/TbJuntaDirectivas/Details/${juntaId}`;
                            }, 2000);
                        }
                    }
                }
            },
            error: function () {
                selectPuesto.empty().append('<option disabled selected>Error al cargar puestos</option>');
                mostrarErrorSwal('Error', 'No se pudieron cargar los puestos.');
            }
        });
    }

    if (juntaId) {
        selectJunta.val(juntaId);
        cargarAsociados(juntaId);
        cargarPuestos(juntaId);
    }
});
