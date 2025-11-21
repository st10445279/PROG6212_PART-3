// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Site-wide JavaScript

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });
});

// Confirm delete actions
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

// Format currency
function formatCurrency(amount) {
    return 'R' + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// Validate file size
function validateFileSize(input, maxSizeMB = 5) {
    if (input.files && input.files[0]) {
        const fileSize = input.files[0].size / 1024 / 1024; // in MB
        if (fileSize > maxSizeMB) {
            alert(`File size exceeds ${maxSizeMB}MB. Please choose a smaller file.`);
            input.value = '';
            return false;
        }
    }
    return true;
}

