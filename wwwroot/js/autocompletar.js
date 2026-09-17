document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('[data-autocompletar]').forEach(function (contenedor) {
        const inputTexto = contenedor.querySelector('.autocompletar-texto');
        const inputId = contenedor.querySelector('.autocompletar-id');
        const listaResultados = contenedor.querySelector('.autocompletar-resultados ul');
        const configScript = contenedor.nextElementSibling;
        const url = configScript.dataset.url;

        let temporizador = null;

        inputTexto.addEventListener('input', function () {
            inputId.value = '';
            const texto = inputTexto.value.trim();

            clearTimeout(temporizador);
            if (texto.length < 2) {
                listaResultados.classList.add('hidden');
                return;
            }


            temporizador = setTimeout(function () {
                fetch(url + '?q=' + encodeURIComponent(texto))
                    .then(function (resp) { return resp.json(); })
                    .then(function (data) {
                        listaResultados.innerHTML = '';
                        if (!data.datos || data.datos.length === 0) {
                            listaResultados.classList.add('hidden');
                            return;
                        }
                        data.datos.forEach(function (item) {
                            const li = document.createElement('li');
                            li.textContent = item.texto;
                            li.className = 'px-3 py-2 text-sm cursor-pointer hover:bg-indigo-50';
                            li.addEventListener('click', function () {
                                inputTexto.value = item.texto;
                                inputId.value = item.id;
                                listaResultados.classList.add('hidden');
                            });
                            listaResultados.appendChild(li);
                        });
                        listaResultados.classList.remove('hidden');
                    });
            }, 300);
        });

        document.addEventListener('click', function (e) {
            if (!contenedor.contains(e.target)) {
                listaResultados.classList.add('hidden');
            }
        });
    });
});