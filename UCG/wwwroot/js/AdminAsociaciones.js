$(document).ready(function () {
    // Agregar un campo de búsqueda al principio de .cards-container
    const buscador = `
        <div style="text-align:center; margin-bottom: 2rem;">
            <input id="buscadorTarjetas" type="text" class="form-control" placeholder="Buscar módulo..." style="max-width: 400px; margin: auto;">
        </div>
    `;
    $('.cards-container').before(buscador);

    $('#buscadorTarjetas').on('keyup', function () {
        const texto = $(this).val().toLowerCase().trim();
        let totalCoincidencias = 0;

        $('.cards-container .card').each(function () {
            const contenido = $(this).text().toLowerCase();
            const coincide = contenido.includes(texto);
            $(this).toggle(coincide);
            if (coincide) totalCoincidencias++;
        });

        if (texto !== '' && totalCoincidencias === 0) {
            Swal.fire({
                icon: 'info',
                title: 'Sin coincidencias',
                text: 'No se encontró ningún módulo con ese nombre.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    });
});
