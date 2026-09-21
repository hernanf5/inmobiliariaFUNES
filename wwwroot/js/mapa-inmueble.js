document.addEventListener('DOMContentLoaded', function () {
    const mapaDiv = document.getElementById('mapa-inmueble');
    if (!mapaDiv) return; // esta vista no tiene el mapa de edición

    const inputLat = document.getElementById('Latitud');
    const inputLng = document.getElementById('Longitud');
    const inputDireccion = document.getElementById('Direccion');
    const botonBuscar = document.getElementById('btnBuscarDireccion');

    // San Luis, Argentina, como centro por defecto
    const latInicial = parseFloat(inputLat.value) || -33.295;
    const lngInicial = parseFloat(inputLng.value) || -66.3356;
    const zoomInicial = inputLat.value ? 16 : 6;

    const mapa = L.map('mapa-inmueble').setView([latInicial, lngInicial], zoomInicial);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(mapa);

    let marcador = null;
    if (inputLat.value && inputLng.value) {
        marcador = crearMarcador(latInicial, lngInicial);
    }

    function crearMarcador(lat, lng) {
        const m = L.marker([lat, lng], { draggable: true }).addTo(mapa);
        m.on('dragend', function () {
            const pos = m.getLatLng();
            inputLat.value = pos.lat.toFixed(6);
            inputLng.value = pos.lng.toFixed(6);
        });
        return m;
    }

    // Permite fijar el punto a mano con un click, además de la búsqueda por texto.
    mapa.on('click', function (e) {
        inputLat.value = e.latlng.lat.toFixed(6);
        inputLng.value = e.latlng.lng.toFixed(6);
        if (marcador) {
            marcador.setLatLng(e.latlng);
        } else {
            marcador = crearMarcador(e.latlng.lat, e.latlng.lng);
        }
    });

    if (botonBuscar) {
        botonBuscar.addEventListener('click', function () {
            const direccion = inputDireccion.value.trim();
            if (!direccion) return;

            botonBuscar.disabled = true;
            botonBuscar.textContent = 'Buscando...';

            fetch('https://nominatim.openstreetmap.org/search?format=json&limit=1&q=' + encodeURIComponent(direccion))
                .then(function (resp) { return resp.json(); })
                .then(function (resultados) {
                    if (resultados.length === 0) {
                        alert('No se encontró esa dirección. Podés hacer click en el mapa para marcarla a mano.');
                        return;
                    }
                    const lat = parseFloat(resultados[0].lat);
                    const lng = parseFloat(resultados[0].lon);
                    inputLat.value = lat.toFixed(6);
                    inputLng.value = lng.toFixed(6);
                    mapa.setView([lat, lng], 16);
                    if (marcador) {
                        marcador.setLatLng([lat, lng]);
                    } else {
                        marcador = crearMarcador(lat, lng);
                    }
                })
                .finally(function () {
                    botonBuscar.disabled = false;
                    botonBuscar.textContent = 'Buscar en el mapa';
                });
        });
    }
});