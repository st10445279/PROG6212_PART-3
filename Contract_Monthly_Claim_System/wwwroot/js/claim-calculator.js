// Claim calculation logic
document.addEventListener('DOMContentLoaded', function () {
    const hoursInput = document.getElementById('hoursWorked');
    const rateInput = document.getElementById('hourlyRate');
    const totalInput = document.getElementById('totalAmount');

    if (hoursInput && rateInput && totalInput) {
        function calculateTotal() {
            const hours = parseFloat(hoursInput.value) || 0;
            const rate = parseFloat(rateInput.value) || 0;
            const total = hours * rate;
            totalInput.value = 'R ' + total.toFixed(2);
        }

        hoursInput.addEventListener('input', calculateTotal);
        rateInput.addEventListener('input', calculateTotal);
    }
});
