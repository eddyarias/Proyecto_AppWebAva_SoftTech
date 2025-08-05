// Funciones JavaScript para la mensajería en Blazor
window.blazorMensajeria = {
    // Scroll automático al final del chat
    scrollToBottom: function(element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },

    // Notificaciones del navegador
    requestNotificationPermission: async function() {
        if ('Notification' in window) {
            const permission = await Notification.requestPermission();
            return permission === 'granted';
        }
        return false;
    },

    showBrowserNotification: function(title, body, icon = null) {
        if ('Notification' in window && Notification.permission === 'granted') {
            const notification = new Notification(title, {
                body: body,
                icon: icon || '/favicon.ico',
                badge: '/favicon.ico',
                tag: 'mensajeria'
            });

            // Auto cerrar después de 5 segundos
            setTimeout(() => {
                notification.close();
            }, 5000);

            return notification;
        }
        return null;
    },

    // Sonidos de notificación
    playNotificationSound: function() {
        try {
            // Crear audio context si no existe
            if (!window.audioContext) {
                window.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            }

            // Sonido simple de notificación
            const oscillator = window.audioContext.createOscillator();
            const gainNode = window.audioContext.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(window.audioContext.destination);

            oscillator.frequency.setValueAtTime(800, window.audioContext.currentTime);
            oscillator.frequency.exponentialRampToValueAtTime(400, window.audioContext.currentTime + 0.1);
            
            gainNode.gain.setValueAtTime(0.1, window.audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, window.audioContext.currentTime + 0.1);

            oscillator.start(window.audioContext.currentTime);
            oscillator.stop(window.audioContext.currentTime + 0.1);
        } catch (error) {
            console.warn('No se pudo reproducir el sonido de notificación:', error);
        }
    },

    // Detección de visibilidad de la página
    isPageVisible: function() {
        return !document.hidden;
    },

    // Almacenamiento local para configuraciones
    saveUserPreference: function(key, value) {
        try {
            localStorage.setItem(`mensajeria_${key}`, JSON.stringify(value));
        } catch (error) {
            console.warn('Error guardando preferencia:', error);
        }
    },

    getUserPreference: function(key, defaultValue = null) {
        try {
            const stored = localStorage.getItem(`mensajeria_${key}`);
            return stored ? JSON.parse(stored) : defaultValue;
        } catch (error) {
            console.warn('Error cargando preferencia:', error);
            return defaultValue;
        }
    },

    // Focus en elemento
    focusElement: function(element) {
        if (element) {
            element.focus();
        }
    },

    // Redimensionar textarea automáticamente
    autoResizeTextarea: function(element) {
        if (element) {
            element.style.height = 'auto';
            element.style.height = element.scrollHeight + 'px';
        }
    },

    // Copiar texto al portapapeles
    copyToClipboard: async function(text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (error) {
            console.error('Error al copiar texto:', error);
            return false;
        }
    }
};

// Funciones globales para compatibilidad con Blazor
window.scrollToBottom = function(element) {
    window.blazorMensajeria.scrollToBottom(element);
};

window.mostrarNotificacion = function(titulo, mensaje, tipo) {
    mostrarNotificacionFlotante(titulo, mensaje, tipo);
};

// Sistema de notificaciones flotantes
function mostrarNotificacionFlotante(titulo, mensaje, tipo = 'info') {
    const container = document.getElementById('notificacionesContainer') || 
                    document.querySelector('.notifications-container') ||
                    crearContenedorNotificaciones();
    
    const tipoClases = {
        'info': 'notification',
        'success': 'notification success',
        'warning': 'notification warning',
        'error': 'notification error'
    };
    
    const notificacion = document.createElement('div');
    notificacion.className = tipoClases[tipo] || 'notification';
    notificacion.innerHTML = `
        <div class="notification-content">
            <strong>${titulo}</strong><br>
            <span>${mensaje}</span>
        </div>
    `;
    
    container.appendChild(notificacion);
    
    // Auto cerrar después de 5 segundos
    const timer = setTimeout(() => {
        if (notificacion.parentNode) {
            notificacion.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => {
                if (notificacion.parentNode) {
                    container.removeChild(notificacion);
                }
            }, 300);
        }
    }, 5000);
    
    // Cerrar al hacer clic
    notificacion.addEventListener('click', () => {
        clearTimeout(timer);
        if (notificacion.parentNode) {
            notificacion.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => {
                if (notificacion.parentNode) {
                    container.removeChild(notificacion);
                }
            }, 300);
        }
    });

    // Reproducir sonido si la página no está visible
    if (document.hidden) {
        window.blazorMensajeria.playNotificationSound();
    }

    // Mostrar notificación del navegador si está permitido
    if (document.hidden && 'Notification' in window && Notification.permission === 'granted') {
        window.blazorMensajeria.showBrowserNotification(titulo, mensaje);
    }
}

function crearContenedorNotificaciones() {
    const container = document.createElement('div');
    container.id = 'notificacionesContainer';
    container.className = 'notifications-container';
    container.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        pointer-events: none;
    `;
    document.body.appendChild(container);
    return container;
}

// Manejo de eventos de teclado global
document.addEventListener('DOMContentLoaded', function() {
    // Solicitar permisos de notificación al cargar
    if ('Notification' in window && Notification.permission === 'default') {
        window.blazorMensajeria.requestNotificationPermission();
    }

    // Detectar cambios de visibilidad para notificaciones
    document.addEventListener('visibilitychange', function() {
        if (document.hidden) {
            console.log('Página oculta - activar notificaciones del navegador');
        } else {
            console.log('Página visible - usar notificaciones internas');
        }
    });

    // Manejo de shortcuts de teclado
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + K para abrir búsqueda rápida de conversaciones
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            const searchInput = document.querySelector('.conversaciones-panel input[type="search"]');
            if (searchInput) {
                e.preventDefault();
                searchInput.focus();
            }
        }

        // Escape para cerrar modales
        if (e.key === 'Escape') {
            const modals = document.querySelectorAll('.modal.show');
            modals.forEach(modal => {
                const closeBtn = modal.querySelector('.btn-close, [data-bs-dismiss="modal"]');
                if (closeBtn) {
                    closeBtn.click();
                }
            });
        }
    });

    // Auto-resize para textareas
    document.addEventListener('input', function(e) {
        if (e.target.tagName === 'TEXTAREA' && e.target.hasAttribute('data-auto-resize')) {
            window.blazorMensajeria.autoResizeTextarea(e.target);
        }
    });
});

// Utilidades para manejo de SignalR desde Blazor
window.signalRUtils = {
    // Verificar estado de conexión
    checkConnectionState: function(connection) {
        if (!connection) return 'Disconnected';
        
        const states = {
            0: 'Disconnected',
            1: 'Connecting',
            2: 'Connected',
            3: 'Disconnecting',
            4: 'Reconnecting'
        };
        
        return states[connection.state] || 'Unknown';
    },

    // Reintentar conexión
    retryConnection: async function(connection, maxRetries = 5) {
        let retries = 0;
        
        while (retries < maxRetries) {
            try {
                await connection.start();
                console.log('Reconectado a SignalR');
                return true;
            } catch (error) {
                retries++;
                console.warn(`Intento de reconexión ${retries}/${maxRetries} falló:`, error);
                
                if (retries < maxRetries) {
                    // Esperar tiempo exponencial antes del siguiente intento
                    await new Promise(resolve => setTimeout(resolve, Math.pow(2, retries) * 1000));
                }
            }
        }
        
        console.error('No se pudo reconectar a SignalR después de', maxRetries, 'intentos');
        return false;
    }
};

// Interceptor de errores para debugging
window.addEventListener('error', function(e) {
    if (e.message.includes('SignalR') || e.message.includes('HubConnection')) {
        console.error('Error de SignalR capturado:', e.error);
        mostrarNotificacionFlotante('Error de conexión', 'Se perdió la conexión con el servidor', 'error');
    }
});

// Service Worker para notificaciones offline (si está disponible)
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw-mensajeria.js').catch(function(error) {
        console.log('Service Worker de mensajería no se pudo registrar:', error);
    });
}

// Limpiar recursos al cerrar la página
window.addEventListener('beforeunload', function() {
    // Limpiar timers y conexiones si es necesario
    if (window.mensajeriaCleanup && typeof window.mensajeriaCleanup === 'function') {
        window.mensajeriaCleanup();
    }
});
