document.getElementById("form-insert").addEventListener("submit", function (event) {
    const testoInput = document.getElementById("testoComunicazioneAdd");
    if (testoInput.value.trim() === "") {
        alert("Il testo della comunicazione non può contenere solo spazi.");
        event.preventDefault(); // Prevent form submission
    }
});



document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form");

    forms.forEach(form => {
        form.addEventListener("submit", function (e) {
            const textarea = form.querySelector("textarea[name='Testo']");
            if (textarea && textarea.value.trim() === "") {
                e.preventDefault();
                alert("Il messaggio non può contenere solo spazi.");
            }
        });
    });
});






