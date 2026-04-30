const slides = document.querySelectorAll(".slide");
const nextSlideButton = document.getElementById("nextSlide");
const prevSlideButton = document.getElementById("prevSlide");

let currentSlide = 0;

function showSlide(index) {
    slides.forEach(function (slide) {
        slide.classList.remove("active-slide");
    });

    slides[index].classList.add("active-slide");
}

function goToNextSlide() {
    currentSlide++;

    if (currentSlide >= slides.length) {
        currentSlide = 0;
    }

    showSlide(currentSlide);
}

function goToPreviousSlide() {
    currentSlide--;

    if (currentSlide < 0) {
        currentSlide = slides.length - 1;
    }

    showSlide(currentSlide);
}

if (slides.length > 0) {
    showSlide(currentSlide);
    setInterval(goToNextSlide, 4000);
}

if (nextSlideButton) {
    nextSlideButton.addEventListener("click", goToNextSlide);
}

if (prevSlideButton) {
    prevSlideButton.addEventListener("click", goToPreviousSlide);
}

const faqQuestions = document.querySelectorAll(".faq-question");

faqQuestions.forEach(function (question) {
    question.addEventListener("click", function () {
        const answer = question.nextElementSibling;
        answer.classList.toggle("show-answer");
    });
});