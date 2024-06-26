document.addEventListener("DOMContentLoaded", function () {
    var passwordInput = document.getElementById("Password");
    var strengthIndicator = document.getElementById("strengthIndicator");
    var strengthText = document.getElementById("strengthText");

    passwordInput.addEventListener("input", function () {
        var strength = getPasswordStrength(passwordInput.value);
        strengthIndicator.value = strength.score;
        strengthText.textContent = strength.text;
    });

    function getPasswordStrength(password) {
        var score = 0;
        var text = "Muito Fraca";

        if (password.length >= 8) {
            score += 1;
        }
        if (/[A-Z]/.test(password)) {
            score += 1;
        }
        if (/[a-z]/.test(password)) {
            score += 1;
        }
        if (/[0-9]/.test(password)) {
            score += 1;
        }
        if (/[^A-Za-z0-9]/.test(password)) {
            score += 1;
        }

        switch (score) {
            case 0:
            case 1:
                text = "Muito Fraca";
                break;
            case 2:
                text = "Fraca";
                break;
            case 3:
                text = "Razoável";
                break;
            case 4:
                text = "Boa";
                break;
            case 5:
                text = "Forte";
                break;
        }

        return { score: score, text: text };
    }
});
