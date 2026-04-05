document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.setAttribute('novalidate', 'novalidate');
    });

    const modalEl = document.getElementById('valConfirmModal');
    if (modalEl) {
        const confirmModal = new bootstrap.Modal(modalEl);
        const confirmBtn = document.getElementById('valConfirmBtn');
        const confirmMsg = document.getElementById('valConfirmMessage');
        let currentFormToSubmit = null;

        const confirmButtons = document.querySelectorAll('button[onclick*="confirm"]');

        confirmButtons.forEach(btn => {
            const originalOnclick = btn.getAttribute('onclick');
            let msg = "Are you sure you want to perform this action?";

            const match = originalOnclick.match(/confirm\(['"](.*?)['"]\)/);
            if (match && match[1]) {
                msg = match[1];
            }

            btn.removeAttribute('onclick');

            btn.addEventListener('click', function (e) {
                e.preventDefault();
                currentFormToSubmit = btn.closest('form');
                confirmMsg.textContent = msg;
                confirmModal.show();
            });
        });

        confirmBtn.addEventListener('click', function () {
            if (currentFormToSubmit) {
                currentFormToSubmit.submit();
            }
        });
    }
});