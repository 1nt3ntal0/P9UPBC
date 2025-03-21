// Función reutilizable para mostrar mensajes
function mostrarMensaje(mensaje, tipo) {
    const mensajeElemento = document.getElementById('mensaje');
    if (mensajeElemento) {
        mensajeElemento.textContent = mensaje;
        mensajeElemento.className = `mensaje ${tipo}`;
    }
}

// Función para manejar el envío del formulario de contacto
function manejarFormularioContacto(formId) {
    document.getElementById(formId)?.addEventListener('submit', function (e) {
        e.preventDefault();  // Evita que el formulario se envíe de manera tradicional

        // Validar formulario
        if (!this.checkValidity()) {
            this.classList.add('was-validated');
            return;
        }

        // Crear objeto con los datos del formulario
        let formData = {};
        new FormData(this).forEach((value, key) => {
            formData[key] = value;
        });

        // Enviar los datos del formulario a un servidor o procesarlos
        // Aquí lo único que hacemos es simular un mensaje de éxito
        setTimeout(() => {
            mostrarMensaje('Tu mensaje ha sido enviado correctamente.', 'success');
        }, 500); // Simula un pequeño retraso en la respuesta
    });
}

// Cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    // Manejo del formulario de contacto
    manejarFormularioContacto('formContacto');
});
