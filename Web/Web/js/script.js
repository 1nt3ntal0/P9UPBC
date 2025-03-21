// Función para mostrar mensajes
function mostrarMensaje(mensaje, tipo) {
    const mensajeElemento = document.getElementById('mensaje');
    if (mensajeElemento) {
        mensajeElemento.textContent = mensaje;
        mensajeElemento.className = `mensaje ${tipo}`;
    }
}

// Enviar datos a una URL con fetch
function enviarDatos(url, formData, redirigir = 'home.html') {
    fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.access_token) {
            localStorage.setItem('token', data.access_token);
            mostrarMensaje('Inicio de sesión exitoso', 'success');
            window.location.href = redirigir;
        } else {
            mostrarMensaje(data.mensaje || 'Error en la operación', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        mostrarMensaje('Error al conectar con el servidor', 'error');
    });
}

// Manejo de formularios genéricos
function manejarFormulario(formId, url, redirigir = null) {
    document.getElementById(formId)?.addEventListener('submit', function (e) {
        e.preventDefault();
        if (!this.checkValidity()) {
            this.classList.add('was-validated');
            return;
        }

        let formData = {};
        new FormData(this).forEach((value, key) => formData[key] = value);

        enviarDatos(url, formData, redirigir);
    });
}

// Verificar sesión y actualizar la navegación
function verificarSesion() {
    const token = localStorage.getItem('token');

    document.getElementById('pacienteNav').style.display = token ? 'block' : 'none';
    document.getElementById('clienteNav').style.display = token ? 'block' : 'none';
    document.getElementById('logoutNav').style.display = token ? 'block' : 'none';
    document.getElementById('loginNav').style.display = token ? 'none' : 'block';
    document.getElementById('nosotrosNav').style.display = token ? 'none' : 'block';
    document.getElementById('contactanosNav').style.display = token ? 'none' : 'block';
}

// Manejo del formulario de registro
function manejarFormularioRegistro() {
    document.getElementById("formUsuario")?.addEventListener("submit", function(event) {
        event.preventDefault();

        const usuario = document.getElementById("usuario").value;
        const contrasena = document.getElementById("contrasena").value;

        if (!usuario || !contrasena) {
            mostrarMensaje("Por favor, ingresa todos los campos.", "error");
            return;
        }

        fetch('http://127.0.0.1:5000/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username: usuario, password: contrasena })
        })
        .then(response => response.json())
        .then(data => {
            if (data.message === 'Usuario creado exitosamente') {
                mostrarMensaje('Usuario registrado exitosamente.', 'success');
                localStorage.setItem('clienteID', data.id); // Almacenar el ID del cliente
                window.location.href = 'paciente.html'; // Redirigir al formulario de paciente
            } else {
                mostrarMensaje(data.message || 'Error en la operación', 'error');
            }
        })
        .catch(error => {
            mostrarMensaje('Hubo un error al procesar la solicitud. Intenta nuevamente.', 'error');
            console.error(error);
        });
    });
}

// Manejo del formulario de paciente con FkID manual o almacenado
function manejarFormularioPaciente() {
    document.getElementById("formPaciente")?.addEventListener("submit", function(event) {
        event.preventDefault();

        const inputFkID = document.getElementById("inputFkID");
        const btnUsarAlmacenado = document.getElementById("btnUsarAlmacenado");
        const FkIDAlmacenado = localStorage.getItem("clienteID");

        // Botón para usar el ID almacenado
        btnUsarAlmacenado?.addEventListener("click", function () {
            if (FkIDAlmacenado) inputFkID.value = FkIDAlmacenado;
        });

        // Obtener el FkID manual o almacenado
        const FkID = inputFkID?.value.trim();
        if (!FkID) {
            mostrarMensaje("Error: Debe ingresar un ID de cliente.", "error");
            return;
        }

        // Capturar datos del formulario
        let formData = {};
        new FormData(this).forEach((value, key) => formData[key] = value);
        formData["FkID"] = FkID; // Asignar FkID seleccionado

        // Enviar datos al backend
        fetch('http://127.0.0.1:5000/paciente', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                mostrarMensaje('Paciente registrado correctamente.', 'success');
                window.location.href = `paciente.html?id=${data.paciente_id}`;
            } else {
                mostrarMensaje(data.message || 'Error al registrar paciente', 'error');
            }
        })
        .catch(error => {
            mostrarMensaje('Error al procesar la solicitud.', 'error');
            console.error(error);
        });
    });
}

// Función para buscar clientes existentes
function buscarClienteExistente(nombre) {
    fetch(`http://127.0.0.1:5000/buscar_cliente?nombre=${nombre}`)
        .then(response => response.json())
        .then(data => {
            const resultados = document.getElementById('resultadosBusqueda');
            resultados.innerHTML = '';

            if (data.length > 0) {
                data.forEach(cliente => {
                    const div = document.createElement('div');
                    div.className = 'mb-2';
                    div.innerHTML = `
                        <p>Nombre: ${cliente.usuario}, ID: ${cliente.id}</p>
                        <button class="btn btn-sm btn-success" onclick="seleccionarCliente(${cliente.id})">Seleccionar</button>
                    `;
                    resultados.appendChild(div);
                });
            } else {
                resultados.innerHTML = '<p>No se encontraron clientes.</p>';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            mostrarMensaje('Error al buscar clientes', 'error');
        });
}

// Función para seleccionar un cliente de la búsqueda
function seleccionarCliente(id) {
    document.getElementById('inputFkID').value = id;
    mostrarMensaje(`Cliente seleccionado: ID ${id}`, 'success');
}

// Función para agregar coordenadas
function agregarCoordenadas(paciente_id, latitude, longitude) {
    fetch('http://127.0.0.1:5000/add_coordenadas', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            paciente_id: paciente_id,
            latitude: latitude,
            longitude: longitude
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.message === 'Coordenadas agregadas exitosamente') {
            mostrarMensaje('Coordenadas agregadas correctamente.', 'success');
        } else {
            mostrarMensaje(data.message || 'Error al agregar coordenadas', 'error');
        }
    })
    .catch(error => {
        mostrarMensaje('Error al procesar la solicitud.', 'error');
        console.error(error);
    });
}

// Ejecutar funciones al cargar la página
document.addEventListener("DOMContentLoaded", function() {
    verificarSesion();
    manejarFormulario('formLogin', 'http://127.0.0.1:5000/login', 'home.html');
    manejarFormularioRegistro();
    manejarFormularioPaciente();

    // Evento para buscar clientes
    document.getElementById('btnBuscar')?.addEventListener('click', function () {
        const nombre = document.getElementById('buscarCliente').value.trim();
        if (nombre) {
            buscarClienteExistente(nombre);
        } else {
            mostrarMensaje('Por favor, ingresa un nombre para buscar.', 'error');
        }
    });

    // Evento para usar el ID almacenado
    document.getElementById('btnUsarAlmacenado')?.addEventListener('click', function () {
        const clienteID = localStorage.getItem('clienteID');
        if (clienteID) {
            document.getElementById('inputFkID').value = clienteID;
            mostrarMensaje(`ID almacenado (${clienteID}) seleccionado.`, 'success');
        } else {
            mostrarMensaje('No hay un ID almacenado.', 'error');
        }
    });
});