document.addEventListener("DOMContentLoaded", function () {
    var passwordInput = document.getElementById("password");
    var confirmPasswordInput = document.getElementById("confirmPassword");
    var passwordStrength = document.getElementById("password-strength");
    var passwordMatch = document.getElementById("password-match");

    passwordInput.addEventListener("input", function () {
        var password = passwordInput.value;
        var strength = calculatePasswordStrength(password);
        passwordStrength.textContent = "Password Strength: " + strength;
    });

    confirmPasswordInput.addEventListener("input", function () {
        var confirmPassword = confirmPasswordInput.value;
        var password = passwordInput.value;
        if (confirmPassword === password) {
            passwordMatch.textContent = "Passwords match!";
        } else {
            passwordMatch.textContent = "Passwords do not match!";
        }
    });

    // Function to calculate password strength
    function calculatePasswordStrength(password) {
        // Implement your password strength check here
        var regex = /^(?=.*[A-Z])(?=.*[!@#$%^&*])(?=.{6,})/;
        if (regex.test(password)) {
            return "Strong";
        } else {
            return "Weak (at least 6 characters long, one capital letter, one special character)";
        }
    }
});
