const API_BASE_URL = '/api/noticias';
let editingId = null;
let allNoticias = []; // Para almacenar todas las noticias y hacer filtrado

// Elementos del DOM
const form = document.getElementById('noticia-form');
const formTitle = document.getElementById('form-title');
const submitBtn = document.getElementById('submit-btn');
const cancelBtn = document.getElementById('cancel-btn');
const noticiasContainer = document.getElementById('noticias-container');
const contadorNoticias = document.getElementById('contador-noticias');

// Elementos de filtros
const filtroCategoria = document.getElementById('filtro-categoria');
const filtroMedio = document.getElementById('filtro-medio');
const buscarTexto = document.getElementById('buscar-texto');
const btnLimpiarFiltros = document.getElementById('btn-limpiar-filtros');

// Event listeners
document.addEventListener('DOMContentLoaded', function () {
    loadNoticias();

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        if (editingId) {
            updateNoticia(editingId);
        } else {
            createNoticia();
        }
    });

    cancelBtn.addEventListener('click', function () {
        resetForm();
    });

    // Event listeners para filtros
    filtroCategoria.addEventListener('change', aplicarFiltros);
    filtroMedio.addEventListener('change', aplicarFiltros);
    buscarTexto.addEventListener('input', aplicarFiltros);
    btnLimpiarFiltros.addEventListener('click', limpiarFiltros);
});

// Cargar todas las noticias
async function loadNoticias() {
    try {
        showLoading();
        const response = await fetch(API_BASE_URL);
        const noticias = await response.json();
        allNoticias = noticias;
        displayNoticias(noticias);
        updateContador(noticias.length);
    } catch (error) {
        console.error('Error al cargar noticias:', error);
        showAlert('Error al cargar las noticias', 'danger');
        noticiasContainer.innerHTML = '<div class="alert alert-danger">Error al cargar las noticias</div>';
    }
}

// Mostrar loading
function showLoading() {
    noticiasContainer.innerHTML = `
        <div class="text-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2">Cargando noticias...</p>
        </div>
    `;
}

// Mostrar noticias en el DOM
function displayNoticias(noticias) {
    if (noticias.length === 0) {
        noticiasContainer.innerHTML = `
            <div class="text-center py-5">
                <i class="bi bi-newspaper" style="font-size: 3rem; color: #6c757d;"></i>
                <p class="text-muted mt-3">No hay noticias disponibles</p>
            </div>
        `;
        return;
    }

    const noticiasHTML = noticias.map(noticia => `
        <div class="card noticia-card mb-3" data-categoria="${noticia.categoria}" data-medio="${noticia.medio}">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <h5 class="card-title mb-1">${noticia.titulo}</h5>
                    <div class="dropdown">
                        <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            <i class="bi bi-three-dots"></i>
                        </button>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item" href="#" onclick="editNoticia(${noticia.id})">
                                <i class="bi bi-pencil"></i> Editar
                            </a></li>
                            <li><a class="dropdown-item text-danger" href="#" onclick="deleteNoticia(${noticia.id})">
                                <i class="bi bi-trash"></i> Eliminar
                            </a></li>
                        </ul>
                    </div>
                </div>
                
                <div class="noticia-meta mb-3">
                    <div class="d-flex flex-wrap gap-2">
                        <span class="badge bg-primary">${noticia.categoria}</span>
                        <span class="badge bg-success">
                            <i class="bi bi-broadcast"></i> ${noticia.medio}
                        </span>
                        <span class="badge bg-secondary">
                            <i class="bi bi-person"></i> ${noticia.autor}
                        </span>
                        <span class="badge bg-info">
                            <i class="bi bi-calendar"></i> ${formatearFecha(noticia.fechaPublicacion)}
                        </span>
                    </div>
                </div>
                
                <p class="card-text">${truncateText(noticia.contenido, 150)}</p>
                
                <div class="d-flex justify-content-between align-items-center">
                    <small class="text-muted">
                        Publicado el ${formatearFechaCompleta(noticia.fechaPublicacion)}
                    </small>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" onclick="editNoticia(${noticia.id})" title="Editar">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-outline-danger" onclick="deleteNoticia(${noticia.id})" title="Eliminar">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `).join('');

    noticiasContainer.innerHTML = noticiasHTML;
}

// Crear nueva noticia
async function createNoticia() {
    const noticia = getFormData();

    if (!validarFormulario(noticia)) {
        return;
    }

    try {
        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(noticia)
        });

        if (response.ok) {
            showAlert('Noticia creada exitosamente', 'success');
            resetForm();
            loadNoticias();
        } else {
            const errorData = await response.json();
            showAlert('Error al crear la noticia: ' + (errorData.message || 'Error desconocido'), 'danger');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('Error al crear la noticia', 'danger');
    }
}

// Editar noticia
async function editNoticia(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/${id}`);
        const noticia = await response.json();

        // Llenar el formulario con los datos de la noticia
        document.getElementById('noticia-id').value = noticia.id;
        document.getElementById('titulo').value = noticia.titulo;
        document.getElementById('autor').value = noticia.autor;
        document.getElementById('categoria').value = noticia.categoria;
        document.getElementById('medio').value = noticia.medio;
        document.getElementById('contenido').value = noticia.contenido;

        // Cambiar el estado del formulario a modo edición
        editingId = id;
        formTitle.innerHTML = '<i class="bi bi-pencil-square"></i> Editar Noticia';
        submitBtn.innerHTML = '<i class="bi bi-check-circle"></i> Actualizar Noticia';
        submitBtn.className = 'btn btn-warning';
        cancelBtn.style.display = 'inline-block';

        // Scroll al formulario
        form.scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        console.error('Error al cargar la noticia:', error);
        showAlert('Error al cargar la noticia', 'danger');
    }
}

// Actualizar noticia
async function updateNoticia(id) {
    const noticia = getFormData();
    noticia.id = parseInt(id);

    if (!validarFormulario(noticia)) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(noticia)
        });

        if (response.ok) {
            showAlert('Noticia actualizada exitosamente', 'success');
            resetForm();
            loadNoticias();
        } else {
            const errorData = await response.json();
            showAlert('Error al actualizar la noticia: ' + (errorData.message || 'Error desconocido'), 'danger');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('Error al actualizar la noticia', 'danger');
    }
}

// Eliminar noticia
async function deleteNoticia(id) {
    if (!confirm('¿Estás seguro de que deseas eliminar esta noticia?\n\nEsta acción no se puede deshacer.')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            showAlert('Noticia eliminada exitosamente', 'success');
            loadNoticias();
        } else {
            showAlert('Error al eliminar la noticia', 'danger');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('Error al eliminar la noticia', 'danger');
    }
}

// Obtener datos del formulario
function getFormData() {
    return {
        titulo: document.getElementById('titulo').value.trim(),
        autor: document.getElementById('autor').value.trim(),
        categoria: document.getElementById('categoria').value,
        medio: document.getElementById('medio').value,
        contenido: document.getElementById('contenido').value.trim(),
        activa: true
    };
}

// Validar formulario
function validarFormulario(noticia) {
    if (noticia.titulo.length < 5) {
        showAlert('El título debe tener al menos 5 caracteres', 'warning');
        return false;
    }

    if (noticia.contenido.length < 50) {
        showAlert('El contenido debe tener al menos 50 caracteres', 'warning');
        return false;
    }

    if (!noticia.categoria) {
        showAlert('Debe seleccionar una categoría', 'warning');
        return false;
    }

    if (!noticia.medio) {
        showAlert('Debe seleccionar un medio de publicación', 'warning');
        return false;
    }

    if (noticia.autor.length < 2) {
        showAlert('El nombre del autor debe tener al menos 2 caracteres', 'warning');
        return false;
    }

    return true;
}

// Resetear formulario
function resetForm() {
    form.reset();
    document.getElementById('noticia-id').value = '';
    editingId = null;
    formTitle.innerHTML = '<i class="bi bi-plus-circle"></i> Crear Nueva Noticia';
    submitBtn.innerHTML = '<i class="bi bi-check-circle"></i> Crear Noticia';
    submitBtn.className = 'btn btn-primary';
    cancelBtn.style.display = 'none';
}

// Aplicar filtros
function aplicarFiltros() {
    const categoria = filtroCategoria.value.toLowerCase();
    const medio = filtroMedio.value.toLowerCase();
    const texto = buscarTexto.value.toLowerCase();

    const noticiasFiltradas = allNoticias.filter(noticia => {
        const cumpleCategoria = !categoria || noticia.categoria.toLowerCase() === categoria;
        const cumpleMedio = !medio || noticia.medio.toLowerCase() === medio;
        const cumpleTexto = !texto || noticia.titulo.toLowerCase().includes(texto);

        return cumpleCategoria && cumpleMedio && cumpleTexto;
    });

    displayNoticias(noticiasFiltradas);
    updateContador(noticiasFiltradas.length);
}

// Limpiar filtros
function limpiarFiltros() {
    filtroCategoria.value = '';
    filtroMedio.value = '';
    buscarTexto.value = '';
    displayNoticias(allNoticias);
    updateContador(allNoticias.length);
}

// Actualizar contador
function updateContador(count) {
    contadorNoticias.textContent = count;
}

// Funciones de utilidad
function formatearFecha(fecha) {
    return new Date(fecha).toLocaleDateString('es-ES');
}

function formatearFechaCompleta(fecha) {
    return new Date(fecha).toLocaleDateString('es-ES', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function truncateText(text, maxLength) {
    if (text.length <= maxLength) return text;
    return text.substr(0, maxLength) + '...';
}

// Mostrar alertas mejoradas
function showAlert(message, type) {
    // Remover alertas existentes
    const existingAlerts = document.querySelectorAll('.alert-message');
    existingAlerts.forEach(alert => alert.remove());

    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show alert-message`;
    alertDiv.innerHTML = `
        <strong>${type === 'success' ? '¡Éxito!' : type === 'danger' ? '¡Error!' : '¡Atención!'}</strong> ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(alertDiv);

    // Auto-remover después de 5 segundos
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}