const registerForm = document.getElementById("registerForm");
const message = document.getElementById("message");

registerForm.addEventListener("submit", async function (event) {
    event.preventDefault();

    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const email = document.getElementById("email").value.trim();
    const studyLevel = document.getElementById("studyLevel").value;
    const password = document.getElementById("password").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    message.textContent = "";
    message.style.color = "red";

    if (
        firstName === "" ||
        lastName === "" ||
        email === "" ||
        studyLevel === "" ||
        password === "" ||
        confirmPassword === ""
    ) {
        message.textContent = "Please fill in all fields.";
        return;
    }

    if (!email.includes("@") || !email.includes(".")) {
        message.textContent = "Please enter a valid email address.";
        return;
    }

    if (password.length < 6) {
        message.textContent = "Password must be at least 6 characters long.";
        return;
    }

    if (password !== confirmPassword) {
        message.textContent = "Password and confirm password do not match.";
        return;
    }

    const registerData = {
        username: firstName + " " + lastName,
        email: email,
        passwordHash: password
    };

    try {
        const response = await fetch("http://localhost:5095/api/users", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(registerData)
        });

        const result = await response.json();

        if (!response.ok) {
            message.textContent = result.message || "Registration failed.";
            return;
        }

        localStorage.setItem("userId", result.id);
        localStorage.setItem("username", result.username);
        localStorage.setItem("email", result.email);
        localStorage.setItem("studyLevel", studyLevel);

        message.style.color = "green";
        message.textContent = "Account created successfully.";

        registerForm.reset();

        setTimeout(function () {
            window.location.href = "../login.html";
        }, 1000);

    } catch (error) {
        message.textContent = "Could not connect to the backend. Make sure the backend is running.";
        console.error(error);
    }
});