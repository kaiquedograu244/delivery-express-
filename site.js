(function () {
    document.querySelectorAll("[data-step]").forEach(function (btn) {
        btn.addEventListener("click", function () {
            var wrap = btn.closest(".qty");
            var input = wrap ? wrap.querySelector("input") : null;
            if (!input) return;
            var step = Number(btn.getAttribute("data-step") || "0");
            var min = Number(input.getAttribute("min") || "1");
            var max = Number(input.getAttribute("max") || "20");
            var current = Number(input.value || "1");
            if (!Number.isFinite(current)) current = min;
            var next = Math.min(max, Math.max(min, current + step));
            input.value = String(next);
            var form = btn.closest("form");
            if (form && form.classList.contains("line-form")) form.submit();
        });
    });

    document.querySelectorAll("form[data-once]").forEach(function (form) {
        form.addEventListener("submit", function () {
            var button = form.querySelector("[type=submit]");
            if (button) button.disabled = true;
        });
    });
})();
