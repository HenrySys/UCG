document.addEventListener("DOMContentLoaded", () => {
    const conceptosSelect = document.getElementById("IdConceptoAsociacion");
    const idAsociacionSelect = document.getElementById("IdAsociacion");
    const tipoOrigenInput = document.getElementById("TipoOrigenIngreso");

    const grupoDonante = document.getElementById("grupoDonante");
    const grupoActividad = document.getElementById("grupoActividad");
    const grupoFinancista = document.getElementById("grupoFinancista");

    const selectDonante = document.getElementById("IdDonante");
    const selectActividad = document.getElementById("IdActividad");
    const selectFinancista = document.getElementById("IdFinancista");


    const documentosIngreso = [];

    const tipoOrigenMap = {
        1: "donante",
        2: "actividad",
        3: "financista"
    };

    const tipoOrigenMapReverse = {
        "donante": 1,
        "actividad": 2,
        "financista": 3
    };

    if (idAsociacionSelect && conceptosSelect) {
        idAsociacionSelect.addEventListener("change", () => {
            const idAsociacion = idAsociacionSelect.value;
            if (!idAsociacion) return;

            fetch(`/TbMovimientoIngresos/ObtenerConceptosPorAsociacion?idAsociacion=${idAsociacion}`)
                .then(r => r.json())
                .then(data => {
                    if (!data.success) {
                        Swal.fire("Error", data.message, "error");
                        return;
                    }

                    conceptosSelect.innerHTML = '<option value="">Seleccione un concepto</option>';
                    data.data.forEach(concepto => {
                        const option = document.createElement("option");
                        option.value = concepto.idConceptoAsociacion;
                        option.text = concepto.descripcionPersonalizada;
                        option.dataset.tipoOrigen = tipoOrigenMap[concepto.tipoOrigen] || "";
                        conceptosSelect.appendChild(option);
                    });
                });
        });
    }

    if (conceptosSelect) {
        conceptosSelect.addEventListener("change", () => {
            const selected = conceptosSelect.options[conceptosSelect.selectedIndex];
            const tipoOrigenTexto = selected?.dataset?.tipoOrigen || "";
            const tipoOrigenNumero = tipoOrigenMapReverse[tipoOrigenTexto] || "";
            tipoOrigenInput.value = tipoOrigenNumero;

            // Campos del formulario
            grupoDonante.classList.add("d-none");
            grupoActividad.classList.add("d-none");
            grupoFinancista.classList.add("d-none");

            if (tipoOrigenTexto === "donante") {
                grupoDonante.classList.remove("d-none");
                
                cargarEntidades("donante", selectDonante);
              
            } else if (tipoOrigenTexto === "actividad") {
                grupoActividad.classList.remove("d-none");
            
                cargarEntidades("actividad", selectActividad);
           
            } else if (tipoOrigenTexto === "financista") {
                grupoFinancista.classList.remove("d-none");
          
                cargarEntidades("financista", selectFinancista);
   
            }
        });
    }


    function cargarEntidades(tipo, select) {
        const idAsociacion = idAsociacionSelect.value;
        if (!idAsociacion) return;

        let url = "";

        if (tipo === "donante") {
            url = `/TbMovimientoIngresos/ObtenerClientesPorAsociacion?idAsociacion=${idAsociacion}`;
        } else if (tipo === "actividad") {
            url = `/TbMovimientoIngresos/ObtenerActividadesPorAsociacion?idAsociacion=${idAsociacion}`;
        } else if (tipo === "financista") {
            url = `/TbMovimientoIngresos/ObtenerFinancistasPorAsociacion?idAsociacion=${idAsociacion}`;
        } else {
            return;
        }

        fetch(url)
            .then(r => r.json())
            .then(data => {
                if (!data.success) {
                    Swal.fire("Error", data.message, "error");
                    return;
                }

                select.innerHTML = "<option value=''>Seleccione</option>";
                data.data.forEach(entidad => {
                    const option = document.createElement("option");
                    option.value = entidad.idCliente || entidad.idActividad || entidad.idFinancista;
                    option.text = entidad.nombre;
                    select.appendChild(option);
                });
            });
    }


    // -------------------- Modal Lógica --------------------
    function actualizarTablaIngreso() {
        const tbody = document.querySelector("#detailsTableIngreso tbody");
        tbody.innerHTML = "";
        let total = 0;

        documentosIngreso.forEach(doc => {
            const row = `<tr>
                <td>${doc.NumComprobante}</td>
                <td>${["", "Efectivo", "Sinpe", "Transferencia"][doc.MetodoPago]}</td>
                <td>?${parseFloat(doc.Monto).toFixed(2)}</td>
            </tr>`;
            tbody.innerHTML += row;
            total += parseFloat(doc.Monto);
        });
        const monto = total.toFixed(2); // deja el punto como separador decimal
        document.querySelector("#MontoTotalIngresado").value = `?${monto.replace(".", ",")}`; // solo visual
        document.querySelector("#MontoTotalIngresadoHidden").value = monto.replace('.', ',');// backend recibe 2000.00 correctamente

        document.querySelector("#DetalleDocumentoIngresoJson").value = JSON.stringify(documentosIngreso);
    }

    document.getElementById("btnAbrirModalIngreso").addEventListener("click", () => {
        document.getElementById("modalNumComprobante").value = "";
        document.getElementById("modalFechaComprobante").value = "";
        document.getElementById("modalMonto").value = "";
        document.getElementById("modalMetodoPago").value = "1";
        document.getElementById("modalDescripcion").value = "";
        new bootstrap.Modal(document.getElementById("detailModalIngreso")).show();
    });

    document.getElementById("addDetailIngresoBtn").addEventListener("click", () => {
        const tipoOrigen = parseInt(document.getElementById("TipoOrigenIngreso")?.value);
        const doc = {
            NumComprobante: document.getElementById("modalNumComprobante").value,
            FechaComprobante: document.getElementById("modalFechaComprobante").value,
            Monto: document.getElementById("modalMonto").value,
            MetodoPago: document.getElementById("modalMetodoPago").value,
            Descripcion: document.getElementById("modalDescripcion").value,
            IdConceptoAsociacion: document.getElementById("IdConceptoAsociacion")?.value,
            IdActividad: tipoOrigen == 2 ? document.getElementById("IdActividad")?.value : null,
            IdCliente: tipoOrigen == 1 ? document.getElementById("IdDonante")?.value : null,
            IdFinancista: tipoOrigen == 3 ? document.getElementById("IdFinancista")?.value : null
         
        };

        if (!doc.NumComprobante || !doc.FechaComprobante || !doc.Monto) {
            Swal.fire("Atención", "Todos los campos obligatorios deben estar completos.", "warning");
            return;
        }

        documentosIngreso.push(doc);
        actualizarTablaIngreso();
        bootstrap.Modal.getInstance(document.getElementById("detailModalIngreso")).hide();
    });

    if (conceptosSelect.value) {
        const event = new Event('change');
        conceptosSelect.dispatchEvent(event);
    }
});
