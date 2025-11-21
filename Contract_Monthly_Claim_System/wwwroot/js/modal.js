// Modal functionality
function showRejectModal() {
    document.getElementById('rejectModal').style.display = 'block';
}

function closeRejectModal() {
    document.getElementById('rejectModal').style.display = 'none';
}

// Close modal when clicking outside
window.onclick = function (event) {
    const modal = document.getElementById('rejectModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}
