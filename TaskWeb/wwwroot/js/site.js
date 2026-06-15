document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".js-due-date-toggle").forEach((checkbox) => {
        const form = checkbox.closest("form");
        if (!form) {
            return;
        }

        const dueDateField = form.querySelector(".due-date-field");
        if (!dueDateField) {
            return;
        }

        const sync = () => {
            dueDateField.classList.toggle("is-hidden", !checkbox.checked);
        };

        checkbox.addEventListener("change", sync);
        sync();
    });
});